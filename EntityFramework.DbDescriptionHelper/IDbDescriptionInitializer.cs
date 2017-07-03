using System;

#if NET45
using System.Data.Entity;
#else
using Microsoft.EntityFrameworkCore;
#endif

namespace EntityFramework.DbDescriptionHelper
{
    /// <summary>
    /// IDbDescriptionInitializer
    /// </summary>
    public interface IDbDescriptionInitializer
    {
        /// <summary>
        /// GenerateDbDescriptionSqlText
        /// </summary>
        /// <param name="contextType">typeof custom DbContext</param>
        /// <returns>generated db description sql</returns>
        string GenerateDbDescriptionSqlText(Type contextType);

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