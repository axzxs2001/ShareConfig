using System;
using System.Collections.Generic;
using System.Text;

namespace ShareConfig.Core
{
    /// <summary>
    /// key entity
    /// </summary>
    public class Key
    {
        /// <summary>
        /// name space
        /// </summary>
        public virtual string NameSpace
        { get; set; }
        /// <summary>
        /// environment
        /// </summary>
        public virtual string Environment
        { get; set; }
        /// <summary>
        /// version
        /// </summary>
        public virtual string Version
        { get; set; }
        /// <summary>
        /// tag
        /// </summary>
        public virtual string Tag
        { get; set; }

        public override string ToString()
        {
            
            return $"{(string.IsNullOrEmpty(NameSpace) ? RegxString : NameSpace)}/{(string.IsNullOrEmpty(Environment) ? RegxString : Environment)}/{(string.IsNullOrEmpty(Version) ? RegxString : Version)}/{(string.IsNullOrEmpty(Tag) ? RegxString : Tag)}";
        }

        public const string RegxString = "([\\w*.]*)";
    }
}
