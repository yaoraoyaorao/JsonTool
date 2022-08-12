using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonTool.Core
{
    public class DisplayFormat
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Remarks { get; set; }
        public E_FieldType FieldType { get; set; }
    }

    public class FieldType
    {

        public E_FieldType Type { get; set; }
        public string FieldTypeName { get; set; }

    }

    /// <summary>
    /// 字段类型
    /// </summary>
    public enum E_FieldType
    {
        Int,
        Float,
        String,
        Boolean,
        List,
        Dic,
        Class
    }

}
