using System;
using System.Collections.Generic;
using System.Text;

namespace ShareConfig.Core
{
    /// <summary>
    /// Key实体类
    /// </summary>
    public class Key
    {
        public virtual string NameSpace
        { get; set; }
        public virtual string Environment
        { get; set; }

        public virtual string Version
        { get; set; }

        public virtual string Tag
        { get; set; }

        public override string ToString()
        {
            return $"{NameSpace}/{Environment}/{Version}/{Tag}";
        }
    }
}
