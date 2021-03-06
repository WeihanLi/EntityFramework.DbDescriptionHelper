﻿#if NET45
using System.Data.Entity;
#else
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
#endif

using System;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Threading.Tasks;


namespace WeihanLi.EntityFramework.DbDescriptionHelper
{
    /// <inheritdoc />
    /// <summary>
    /// SqlServerDbDescriptionInitializer
    /// </summary>
    public class SqlServerDbDescriptionInitializer : IDbDescriptionInitializer
    {
        /// <summary>
        /// tableDescriptionSqlFormat
        /// @params
        /// 0：table name
        /// 1：table description
        /// </summary>
        private static readonly string tableDescFormat = @"
BEGIN
IF EXISTS (
       SELECT 1
    FROM sys.extended_properties p,
         sys.tables t,
         sys.schemas s
    WHERE t.schema_id = s.schema_id
          AND p.major_id = t.object_id
          AND p.minor_id = 0
          AND p.name = N'MS_Description'
          AND s.name = N'dbo'
          AND t.name = N'{0}'
    )
        EXECUTE sp_updateextendedproperty N'MS_Description', N'{1}', N'SCHEMA', N'dbo',  N'TABLE', N'{0}';
ELSE
        EXECUTE sp_addextendedproperty N'MS_Description', N'{1}', N'SCHEMA', N'dbo',  N'TABLE', N'{0}';
END";

        /// <summary>
        /// ColumnDescriptionSqlFormat
        /// @params
        /// 0：table name
        /// 1：column name
        /// 2：column description
        /// </summary>
        private static readonly string columnDescFormat = @"
BEGIN
IF EXISTS (
        select 1
        from
            sys.extended_properties p,
            sys.columns c,
            sys.tables t,
            sys.schemas s
        where
            t.schema_id = s.schema_id and
            c.object_id = t.object_id and
            p.major_id = t.object_id and
            p.minor_id = c.column_id and
            p.name = N'MS_Description' and
            s.name = N'dbo' and
            t.name = N'{0}' and
            c.name = N'{1}'
    )
        EXECUTE sp_updateextendedproperty N'MS_Description', N'{2}', N'SCHEMA', N'dbo',  N'TABLE', N'{0}', N'COLUMN', N'{1}';
ELSE
        EXECUTE sp_addextendedproperty N'MS_Description', N'{2}', N'SCHEMA', N'dbo',  N'TABLE', N'{0}', N'COLUMN', N'{1}';
END";

        /// <inheritdoc />
        /// <summary>
        /// GenerateDbDescriptionSqlText
        /// </summary>
        /// <param name="context">DbContext</param>
        /// <returns>generated db description sql</returns>
        public virtual string GenerateDbDescriptionSqlText(DbContext context)
        {
            Type contextType = context.GetType();
            if (contextType == null)
            {
                throw new ArgumentNullException(nameof(contextType), "contextType can not be null.");
            }
            if (!(typeof(DbContext)).IsAssignableFrom(contextType))
            {
                throw new ArgumentException("contextType should extends from DbContext.", nameof(contextType));
            }
            var types = contextType.GetRuntimeProperties().Where(p =>
           p.PropertyType.IsGenericType &&
           p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>)).Select(p => p.PropertyType.GetGenericArguments().FirstOrDefault());
            if (types.Any())
            {
                var sbSqlDescText = new StringBuilder();
                foreach (var type in types)
                {
                    var attribute = type.GetCustomAttribute(typeof(TableDescriptionAttribute)) as TableDescriptionAttribute;

                    string tableName = attribute?.Name, tableDesc = attribute?.Description;
#if NET45
                    if (String.IsNullOrEmpty(tableName))
                    {
                        tableName = type.Name;
                    }
#else
                    tableName = context.Model.FindEntityType(type).Relational().TableName;
#endif
                    if (!String.IsNullOrEmpty(tableDesc))
                    {
                        //生成表描述sql
                        sbSqlDescText.AppendFormat(tableDescFormat, tableName, tableDesc);
                        sbSqlDescText.AppendLine();
                    }
                    var properties = type.GetRuntimeProperties();
                    foreach (var property in properties)
                    {
                        var columnAttribute = property.GetCustomAttribute(typeof(ColumnDescriptionAttribute)) as ColumnDescriptionAttribute;
                        if (columnAttribute != null)
                        {
                            string columnName = columnAttribute.Name, columnDesc = columnAttribute.Description;

                            if (String.IsNullOrEmpty(columnDesc))
                            {
                                continue;
                            }
#if NET45                            
                            if (String.IsNullOrEmpty(columnName))
                            {
                                columnName = property.Name;
                            }
#else
                            columnName = context.Model.FindEntityType(type).FindProperty(property.Name).Relational().ColumnName;
#endif
                            // 生成字段描述
                            sbSqlDescText.AppendFormat(columnDescFormat, tableName, columnName, columnDesc);
                            sbSqlDescText.AppendLine();
                        }
                    }
                }
                return sbSqlDescText.ToString();
            }
            return "";
        }

        /// <inheritdoc />
        /// <summary>
        /// generate db description
        /// </summary>
        /// <param name="context">database context</param>
        public virtual void GenerateDbDescription(DbContext context)
        {
            string sqlText = GenerateDbDescriptionSqlText(context);
            if (sqlText.Length > 0)
            {
                context.Database.ExecuteSqlCommand(sqlText);
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// generate db description
        /// </summary>
        /// <param name="context">database context</param>
        public virtual async Task GenerateDbDescriptionAsync(DbContext context)
        {
            string sqlText = GenerateDbDescriptionSqlText(context);
            if (sqlText.Length > 0)
            {
                await context.Database.ExecuteSqlCommandAsync(sqlText);
            }
        }
    }

#if NET45
    public static class EntityFrameworkExtensions
    {
    }

#else
    public static class DotNetCoreExtensions
    {
        #region EntityFrameworkExtensions
        public static int ExecuteSqlCommand(this DatabaseFacade database, string sql, params object[] parameters)
        {
            return database.ExecuteSqlCommand(new RawSqlString(sql), parameters);
        }

        public static async Task<int> ExecuteSqlCommandAsync(this DatabaseFacade database, string sql, params object[] parameters)
        {
            return await database.ExecuteSqlCommandAsync(new RawSqlString(sql), parameters);
        }
        #endregion
    }
#endif
}