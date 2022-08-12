using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace JsonTool.Core
{
    /// <summary>
    /// 动态创建类文件 并保存
    /// </summary>
    public class ClassCreate
    {
        /// <summary>
        /// 代码生产的编译单元
        /// </summary>
        private CodeCompileUnit targetUnit;

        private CodeTypeDeclaration targetClass;

        /// <summary>
        /// 命名空间
        /// </summary>
        private List<string> namespaceNmaes;

        /// <summary>
        /// 字段
        /// </summary>
        private List<CodeMemberField> codeMemberFields;

        /// <summary>
        /// 属性
        /// </summary>
        private List<CodeMemberProperty> codeMemberPropertys;

        public ClassCreate()
        {
            namespaceNmaes = new List<string>();
            codeMemberFields = new List<CodeMemberField>();
            codeMemberPropertys = new List<CodeMemberProperty>();
        }

        /// <summary>
        /// 添加字段
        /// </summary>
        /// <param name="fileName">字段名</param>
        /// <param name="type">字段类型</param>
        /// <param name="visitLevel">访问级别 默认是private</param>
        /// <param name="comments">字段描述信息</param>
        public void AddFields(string fileName, Type type, MemberAttributes visitLevel = MemberAttributes.Private, string comments = null)
        {
            //判断如果字段名名重复则直接跳出
            if (codeMemberFields.Exists(t => t.Name == fileName)) return;

            CodeMemberField field = new CodeMemberField();
            //设置访问级别
            field.Attributes = visitLevel;
            //设置字段名
            field.Name = fileName;
            //设置字段类型
            field.Type = new CodeTypeReference(type);

            //设置字段描述信息
            field.Comments.Add(new CodeCommentStatement("<summary>", true));
            field.Comments.Add(new CodeCommentStatement(comments ?? fileName, true));
            field.Comments.Add(new CodeCommentStatement("</summary>", true));

            //添加
            codeMemberFields.Add(field);

            AddNamespace(type);
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="fileName">字段名</param>
        /// <param name="type">字段类型</param>
        /// <param name="hasGet">是否有Get</param>
        /// <param name="hasSet">是否有Set</param>
        /// <param name="comments">字段描述</param>
        public void AddProperty(string propertyName, Type type, bool hasGet = true, bool hasSet = true, string comments = null)
        {
            //判断是否有相同的字段名
            if (codeMemberPropertys.Exists(t => t.Name == propertyName)) return;

            //创建属性
            CodeMemberProperty property = new CodeMemberProperty();
            //属性的访问级别
            property.Attributes = MemberAttributes.Public;
            //属性名
            property.Name = propertyName;
            //属性类型
            property.Type = new CodeTypeReference(type);
            //设置Get
            property.HasGet = hasGet;
            //设置Set
            property.HasSet = hasSet;

            //设置属性描述信息
            property.Comments.Add(new CodeCommentStatement("<summary>", true));
            property.Comments.Add(new CodeCommentStatement(comments ?? propertyName, true));
            property.Comments.Add(new CodeCommentStatement("</summary>", true));

            //字段名
            var fileName = propertyName[0].ToString().ToLower() + propertyName.Substring(1);
            if (!codeMemberFields.Exists(t => t.Name == fileName))
                AddFields(fileName, type, MemberAttributes.Private, comments);
            //设置Get
            property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fileName)));
            CodeFieldReferenceExpression reference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fileName);
            //设置Get
            property.GetStatements.Add(new CodeAssignStatement(reference, new CodeArgumentReferenceExpression("value")));

            //添加属性
            codeMemberPropertys.Add(property);

            AddNamespace(type);
        }

        /// <summary>
        /// 添加命名空间
        /// </summary>
        /// <param name="type"></param>
        private void AddNamespace(Type type)
        {
            if (namespaceNmaes.IndexOf(type.Namespace) == -1)
                namespaceNmaes.Add(type.Namespace);
        }

        public bool GenerateCode(string namespaceName, string className, TypeAttributes visitLevel = TypeAttributes.Public, string filePath = null)
        {
            try
            {
                targetUnit = new CodeCompileUnit();
                CodeNamespace newClass = new CodeNamespace(namespaceName);
                targetClass = new CodeTypeDeclaration(className);
                targetClass.IsClass = true;
                targetClass.TypeAttributes = visitLevel;
                //targetClass.IsPartial = true;
                newClass.Types.Add(targetClass);
                targetUnit.Namespaces.Add(newClass);

                //导入命名空间
                namespaceNmaes.ForEach(item =>
                {
                    newClass.Imports.Add(new CodeNamespaceImport(item));
                });

                //导入字段
                codeMemberFields.ForEach(item =>
                {
                    targetClass.Members.Add(item);
                });

                //导入属性
                codeMemberPropertys.ForEach(item =>
                {
                    targetClass.Members.Add(item);
                });

                CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
                CodeGeneratorOptions options = new CodeGeneratorOptions();
                options.BracingStyle = "C";
                //写入数据
                using (StreamWriter sourceWrite = new StreamWriter(filePath ?? $"{className}.cs"))
                {
                    provider.GenerateCodeFromCompileUnit(targetUnit, sourceWrite, options);
                }
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
    }
}
