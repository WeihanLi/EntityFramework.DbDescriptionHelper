#if NET45
using System.Data.Entity;
#else
using Microsoft.EntityFrameworkCore;
#endif

using System;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace EntityFramework.DbDescriptionHelper
{
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

        /// <summary>
        /// GenerateDbDescriptionSqlText
        /// </summary>
        /// <param name="contextType">typeof custom DbContext</param>
        /// <returns>generated db description sql</returns>
        public virtual string GenerateDbDescriptionSqlText(Type contextType)
        {
            if (contextType == null)
            {
                throw new ArgumentNullException(nameof(contextType), "contextType can not be null.");
            }
#if NET45
            if (!(typeof(DbContext)).IsAssignableFrom(contextType))
#else
            if (!(typeof(DbContext)).GetTypeInfo().IsAssignableFrom(contextType.GetTypeInfo()))
#endif
            {
                throw new ArgumentException("contextType should extends from DbContext.", nameof(contextType));
            }
#if NET45
            var types = contextType.GetRuntimeProperties().Where(p =>
           p.PropertyType.IsGenericType &&
           p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>)).Select(p => p.PropertyType.GetGenericArguments().FirstOrDefault());
#else
            var types = contextType.GetRuntimeProperties().Where(p =>
           p.PropertyType.GetTypeInfo().IsGenericType &&
           p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>)).Select(p => p.PropertyType.GetTypeInfo().GenericTypeArguments.FirstOrDefault());
#endif
            if (types != null && types.Any())
            {
                StringBuilder sbSqlDescText = new StringBuilder();
                foreach (var type in types)
                {
#if NET45
                    var attribute = type.GetCustomAttribute(typeof(TableDescriptionAttribute)) as TableDescriptionAttribute;
#else
                    var attribute = type.GetTypeInfo().GetCustomAttribute(typeof(TableDescriptionAttribute)) as TableDescriptionAttribute;
#endif
                    string tableName = "", tableDesc = "";
                    if (attribute != null)
                    {
                        tableName = attribute.Name;
                        tableDesc = attribute.Description;
                    }
                    if (String.IsNullOrEmpty(tableName))
                    {
                        tableName = type.Name;
                    }
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
                            if (String.IsNullOrEmpty(columnName))
                            {
                                columnName = property.Name;
                            }
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

        /// <summary>
        /// generate db description
        /// </summary>
        /// <param name="context">database context</param>
        public virtual void GenerateDbDescription(DbContext context)
        {
            string sqlText = GenerateDbDescriptionSqlText(context.GetType());
            if (sqlText.Length > 0)
            {
                context.Database.ExecuteSqlCommand(sqlText);
            }
        }

        /// <summary>
        /// generate db description
        /// </summary>
        /// <param name="context">database context</param>
        public virtual async Task GenerateDbDescriptionAsync(DbContext context)
        {
            string sqlText = GenerateDbDescriptionSqlText(context.GetType());
            if (sqlText.Length > 0)
            {
                await context.Database.ExecuteSqlCommandAsync(sqlText);
            }
        }
    }
}