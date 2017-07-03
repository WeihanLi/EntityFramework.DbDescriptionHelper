using System;

namespace EntityFramework.DbDescriptionHelper
{
    /// <summary>
    /// TableDescription
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TableDescriptionAttribute : Attribute
    {
        /// <summary>
        /// TableDescription
        /// </summary>
        /// <param name="description">description</param>
        public TableDescriptionAttribute(string description)
        {
            Description = description;
        }

        /// <summary>
        /// TableDescription
        /// </summary>
        /// <param name="name">table name</param>
        /// <param name="description">table description</param>
        public TableDescriptionAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }

        /// <summary>
        /// table name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// table description
        /// </summary>
        public string Description { get; }
    }
}