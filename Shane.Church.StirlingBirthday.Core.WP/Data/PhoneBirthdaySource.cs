using Microsoft.Phone.UserData;
using Shane.Church.StirlingBirthday.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Shane.Church.StirlingBirthday.Core.WP.Data
{
	public class PhoneBirthdaySource : IBirthdaySource
	{
		List<BirthdayContact> _birthdayContacts;

		public PhoneBirthdaySource()
		{
			_birthdayContacts = new List<BirthdayContact>();
		}

		public Task<IQueryable<BirthdayContact>> GetAllEntriesAsync(bool forceRefresh = false, bool loadPicture = true)
		{
			TaskCompletionSource<IQueryable<BirthdayContact>> tcs = new TaskCompletionSource<IQueryable<BirthdayContact>>();
			if (_birthdayContacts.Count == 0 || forceRefresh)
			{
				Contacts contacts = new Contacts();
				contacts.SearchCompleted += async (s, e) =>
				{
					try
					{
						var items = (from r in e.Results
									 where r.Birthdays.Any()
									 select r).ToList();
						List<BirthdayContact> results = new List<BirthdayContact>();
						foreach (Contact c in items)
						{
							results.Add(await c.GetBirthdayContact(loadPicture));
						}
						tcs.TrySetResult(results.AsQueryable());
					}
					catch (InvalidOperationException) { }
				};
				contacts.SearchAsync(String.Empty, FilterKind.None, "Load Contacts");
				return tcs.Task;
			}
			else
			{
				tcs.SetResult(_birthdayContacts.AsQueryable());
				return tcs.Task;
			}
		}

		public async Task<IQueryable<BirthdayContact>> GetFilteredEntriesAsync(Expression<Func<BirthdayContact, bool>> filter, bool forceRefresh = false, bool loadPicture = true)
		{
			var entries = await GetAllEntriesAsync(forceRefresh, loadPicture);
			var compiledFilter = filter.Compile();
			return entries.Where(it => compiledFilter(it));
		}
	}
}
