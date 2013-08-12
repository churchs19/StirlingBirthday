using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Shane.Church.StirlingBirthday.Core.Data
{
    public interface IBirthdaySource
    {
        /// <summary>
        /// Get all entries asynchronously
        /// </summary>
        /// <param name="includeDeleted"></param>
        /// <returns></returns>
        Task<IQueryable<BirthdayContact>> GetAllEntriesAsync(bool forceRefresh = false, bool loadPicture = true);

        /// <summary>
        /// Get filtered entries asynchronously
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="includeDeleted"></param>
        /// <returns></returns>
        Task<IQueryable<BirthdayContact>> GetFilteredEntriesAsync(Expression<Func<BirthdayContact, bool>> filter, bool forceRefresh = false, bool loadPicture = true);
    }
}
