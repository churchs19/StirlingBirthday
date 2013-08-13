using System;
using System.Linq;
using System.Linq.Expressions;
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
        Task<IQueryable<BirthdayContact>> GetAllEntriesAsync(bool forceRefresh = false);

        /// <summary>
        /// Get filtered entries asynchronously
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="includeDeleted"></param>
        /// <returns></returns>
        Task<IQueryable<BirthdayContact>> GetFilteredEntriesAsync(Expression<Func<BirthdayContact, bool>> filter, bool forceRefresh = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contactName"></param>
        /// <param name="loadPicture"></param>
        /// <returns></returns>
        Task<BirthdayContact> GetContactByNameAsync(string contactName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contactName"></param>
        /// <param name="forceRefresh"></param>
        /// <returns></returns>
        Task<byte[]> GetContactPicture(string contactName, bool forceRefresh = false);
    }
}
