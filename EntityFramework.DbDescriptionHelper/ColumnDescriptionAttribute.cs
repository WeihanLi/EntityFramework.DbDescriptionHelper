using System;

namespace EntityFramework.DbDescriptionHelper
{
    /// <summary>
    /// ColumnDescription
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ColumnDescriptionAttribute : Attribute
    {
        /// <summary>
        /// ColumnDescription
        /// </summary>
        /// <param name="description">description</param>
        public ColumnDescriptionAttribute(string description)
        {
            Description = description;
        }

        /// <summary>
        /// ColumnDescription
        /// </summary>
        /// <param name="name">column name</param>
        /// <param name="description">column description</param>
        public ColumnDescriptionAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }

        /// <summary>
        /// column name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// column description
        /// </summary>
        public string Description { get; }
    }
}