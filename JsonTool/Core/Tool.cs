using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace JsonTool.Core
{
    public class Tool
    {
        /// <summary>
        /// 打开选择文件框
        /// </summary>
        /// <param name="title">标题名</param>
        /// <param name="filter">"文件信息|*.后缀"</param>
        /// <returns></returns>
        public static string OpenPath(string title,string filter,out string fileName)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            string CurrentPath = "";
            openFileDialog.Title = title;
            openFileDialog.Filter = filter;
            
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                CurrentPath = openFileDialog.FileName;
                fileName = openFileDialog.SafeFileName;
                return CurrentPath;
            }
            fileName = string.Empty;
            return string.Empty;
        }

        /// <summary>
        /// 加载文本文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static string LoadFile(string path)
        {
            if (!File.Exists(path))
            {
                MessageBox.Show("未找到文件，请检查路径是否正确");
                return string.Empty;
            }
            return File.ReadAllText(path);
        }

        /// <summary>
        /// 动态获取类
        /// </summary>
        /// <param name="source"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static object? DP(string source,string className)
        {

            Assembly assmbly = Compile(source, Assembly.Load(new AssemblyName("System.Runtime")), typeof(object).Assembly);
            return assmbly.CreateInstance(className);
        }

        public static Assembly Compile(string text, params Assembly[] referencedAssemblies)
        {
            var references = referencedAssemblies.Select(it => MetadataReference.CreateFromFile(it.Location));
            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
            var assemblyName = "_" + Guid.NewGuid().ToString("D");
            var syntaxTrees = new SyntaxTree[] { CSharpSyntaxTree.ParseText(text) };
            var compilation = CSharpCompilation.Create(assemblyName, syntaxTrees, references, options);
            using (var stream = new MemoryStream())
            {
                var compilationResult = compilation.Emit(stream);
                if (compilationResult.Success)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    return Assembly.Load(stream.ToArray());
                }
                throw new InvalidOperationException("Compilation error");
            }
        }
    }
}
