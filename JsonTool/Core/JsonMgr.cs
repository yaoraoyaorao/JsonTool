using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JsonTool.Core
{
    internal class JsonMgr
    {
        /// <summary>
        /// 加载json数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T LoadData<T>(string path) where T : new()
        {
            string jsonStr = File.ReadAllText(path);
            T data = JsonMapper.ToObject<T>(jsonStr);
            return data;
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        public static void Save(object data, string path)
        {
            string jsonStr = JsonMapper.ToJson(data);

            File.WriteAllText(path, jsonStr);
        }

        /// <summary>
        /// 加载json文件创建控件
        /// </summary>
        public static void CreateControl(object data)
        {
            Type datatype = data.GetType();
            FieldInfo[] infos = datatype.GetFields();
            string content = "";
            for (int i = 0; i < infos.Length; i++)
            {
                content += " " + infos[i].FieldType.Name;
            }

        }

        /// <summary>
        /// 加载创建控件
        /// </summary>
        /// <param name="value"></param>
        private static void LoadControl(object value)
        {
            //获取类型
            Type dataType = value.GetType();
            if (dataType == typeof(int))
            {
                //创建int控件 label(字段名):值
            }
        }
    }
}
