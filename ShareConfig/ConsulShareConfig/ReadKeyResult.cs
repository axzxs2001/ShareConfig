﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ConsulShareConfig
{
    /// <summary>
    /// Read Key Result
    /// </summary>
    class ReadKeyResult
    {
        /// <summary>
        /// CreateIndex
        /// </summary>
        public int CreateIndex { get; set; }
        /// <summary>
        /// ModifyIndex
        /// </summary>
        public int ModifyIndex { get; set; }
        /// <summary>
        /// LockIndex
        /// </summary>
        public int LockIndex { get; set; }
        /// <summary>
        /// Key
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// Flags
        /// </summary>
        public int Flags { get; set; }
        /// <summary>
        /// Value
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Decode Value
        /// </summary>
        public string DecodeValue
        {
            get
            {
                if (!string.IsNullOrEmpty(Value))
                {
                    byte[] bytes = Convert.FromBase64String(Value);
                    return Encoding.UTF8.GetString(bytes);
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// Session
        /// </summary>
        public string Session { get; set; }
    }
}
