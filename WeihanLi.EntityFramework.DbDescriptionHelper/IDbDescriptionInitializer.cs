using System;

#if NET45
using System.Data.Entity;
#else
using Microsoft.EntityFrameworkCore;
#endif

namespace WeihanLi.EntityFramework.DbDescriptionHelper
{
    /// <summary>
    /// IDbDescriptionInitializer
    /// </summary>
    public interface IDbDescriptionInitializer
    {
        /// <summary>
        /// GenerateDbDescriptionSqlText
        /// </summary>
        /// <param name="context">database context</param>
        /// <returns>generated db description sql</returns>
        string GenerateDbDescriptionSqlText(DbContext context);

        /// <summary>
        /// generate db description
        /// </summary>
        /// <param name="context">database context</param>
        void GenerateDbDescription(DbContext context);

        /// <summary>
        /// generate db description
        /// </summary>
        /// <param name="context">database context</param>
        System.Threading.Tasks.Task GenerateDbDescriptionAsync(DbContext context);
    }
}