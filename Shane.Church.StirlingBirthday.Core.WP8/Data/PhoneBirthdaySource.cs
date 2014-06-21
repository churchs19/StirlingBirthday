using Microsoft.Phone.UserData;
using Shane.Church.StirlingBirthday.Core.Data;
using Shane.Church.StirlingBirthday.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Shane.Church.StirlingBirthday.Core.WP.Data
{
	public class GetEntriesCompletedEventArgs : EventArgs
	{
		public GetEntriesCompletedEventArgs()
			: base()
		{

		}

		public IQueryable<BirthdayContact> Contacts { get; set; }
	}

	public class PhoneBirthdaySource : IBirthdaySource
	{
		private static List<Contact> _contactsList;
		private static List<Contact> ContactsList
		{
			get
			{
				if (_contactsList == null)
					_contactsList = new List<Contact>();
				return _contactsList;
			}
		}

		private static Dictionary<string, byte[]> _contactPictures;
		private static Dictionary<string, byte[]> ContactPictures
		{
			get
			{
				if (_contactPictures == null)
					_contactPictures = new Dictionary<string, byte[]>();
				return _contactPictures;
			}
		}

		public PhoneBirthdaySource()
		{
			_contactPictures = new Dictionary<string, byte[]>();
		}

		public Task<IQueryable<BirthdayContact>> GetAllEntriesAsync(bool forceRefresh = false)
		{
			TaskCompletionSource<IQueryable<BirthdayContact>> tcs = new TaskCompletionSource<IQueryable<BirthdayContact>>();
			if (ContactsList.Count == 0 || forceRefresh)
			{
				Contacts contacts = new Contacts();
				contacts.SearchCompleted += (s, e) =>
				{
					try
					{
                        ContactsList.Clear();
						if (e.Results.Any())
						{
							var items = (from r in e.Results
										 where r.Birthdays.Any()
										 select r);
							ContactsList.AddRange(items);
							tcs.TrySetResult(GetBirthdayContacts().AsQueryable());
						}
						else
						{
							tcs.TrySetResult(new List<BirthdayContact>().AsQueryable());
						}
					}
					catch (InvalidOperationException iex)
					{
						tcs.TrySetException(iex);
					}
				};
				contacts.SearchAsync(String.Empty, FilterKind.None, "Load Contacts");
				return tcs.Task;
			}
			else
			{
				tcs.SetResult(GetBirthdayContacts().AsQueryable());
				return tcs.Task;
			}
		}

		public delegate void GetEntriesComplete(object sender, GetEntriesCompletedEventArgs args);
		public event GetEntriesComplete GetEntriesCompleted;

		private Contacts _phoneContacts;

		public void BeginGetAllEntries(bool forceRefresh = false)
		{
			if (ContactsList.Count == 0 || forceRefresh)
			{
				if (_phoneContacts == null)
				{
					_phoneContacts = new Contacts();
					_phoneContacts.SearchCompleted += _phoneContacts_SearchCompleted;
				}
				_phoneContacts.SearchAsync(String.Empty, FilterKind.None, "Load Contacts");
			}
			else
			{
				if (GetEntriesCompleted != null)
				{
					GetEntriesCompleted(this, new GetEntriesCompletedEventArgs() { Contacts = GetBirthdayContacts().AsQueryable() });
				}
			}
		}

		void _phoneContacts_SearchCompleted(object sender, ContactsSearchEventArgs e)
		{
            ContactsList.Clear(); 
            try
			{
				if (e.Results.Any())
				{
					var items = (from r in e.Results
								 where r.Birthdays.Any()
								 select r);
					ContactsList.AddRange(items);
					if (GetEntriesCompleted != null)
					{
						GetEntriesCompleted(this, new GetEntriesCompletedEventArgs() { Contacts = GetBirthdayContacts().AsQueryable() });
					}
				}
				else
				{
					if (GetEntriesCompleted != null)
						GetEntriesCompleted(this, new GetEntriesCompletedEventArgs() { Contacts = new List<BirthdayContact>().AsQueryable() });
				}
			}
#if DEBUG
			catch (InvalidOperationException iex)
			{
				DebugUtility.SaveDiagnosticException(iex);
#else
            catch
            {
#endif
				if (GetEntriesCompleted != null)
					GetEntriesCompleted(this, new GetEntriesCompletedEventArgs() { Contacts = new List<BirthdayContact>().AsQueryable() });
			}
		}

		private IEnumerable<BirthdayContact> GetBirthdayContacts()
		{
			foreach (var c in ContactsList)
			{
				yield return c.GetBirthdayContact();
			}
		}

		public async Task<IQueryable<BirthdayContact>> GetFilteredEntriesAsync(Expression<Func<BirthdayContact, bool>> filter, bool forceRefresh = false)
		{
			try
			{
				var entries = await GetAllEntriesAsync(forceRefresh);
				var compiledFilter = filter.Compile();
				return entries.Where(it => compiledFilter(it));
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public Task<BirthdayContact> GetContactByNameAsync(string contactName)
		{
			TaskCompletionSource<BirthdayContact> tcs = new TaskCompletionSource<BirthdayContact>();
			Contacts contacts = new Contacts();
			contacts.SearchCompleted += (s, e) =>
			{
				try
				{
					var c = e.Results.FirstOrDefault();
					if (c != null)
					{
						var result = c.GetBirthdayContact();
						tcs.TrySetResult(result);
					}
					else
					{
						tcs.TrySetException(new NotFoundException("Contact not found: " + contactName));
					}
				}
				catch (InvalidOperationException iex)
				{
					tcs.TrySetException(iex);
				}
			};
			contacts.SearchAsync(contactName, FilterKind.DisplayName, "Load Contacts");
			return tcs.Task;
		}

		public async Task<byte[]> GetContactPictureAsync(string contactName, bool forceRefresh = false)
		{
			if (ContactsList.Count == 0 || forceRefresh)
			{
				await GetAllEntriesAsync(forceRefresh);
			}
			if (_contactPictures.ContainsKey(contactName))
				return _contactPictures[contactName];
			else
			{
				var c = ContactsList.Where(it => it.DisplayName.ToLower() == contactName.ToLower()).FirstOrDefault();
				if (c != null)
				{
					using (Stream pictureStream = c.GetPicture())
					{
						if (pictureStream != null)
						{
							var picture = new byte[pictureStream.Length];
							using (MemoryStream pictureMemoryStream = new MemoryStream(picture))
							{
								if (pictureStream.CanRead)
								{
									await pictureStream.CopyToAsync(pictureMemoryStream);
								}
							}
							_contactPictures.Add(contactName, picture);
							return picture;
						}
					}
				}
			}
			return null;
		}

		public byte[] GetContactPicture(string contactName)
		{
			if (_contactPictures.ContainsKey(contactName))
				return _contactPictures[contactName];
			else
			{
				var c = ContactsList.Where(it => it.DisplayName.ToLower() == contactName.ToLower()).FirstOrDefault();
				if (c != null)
				{
					using (Stream pictureStream = c.GetPicture())
					{
						if (pictureStream != null)
						{
							var picture = new byte[pictureStream.Length];
							using (MemoryStream pictureMemoryStream = new MemoryStream(picture))
							{
								if (pictureStream.CanRead)
								{
									pictureStream.CopyTo(pictureMemoryStream);
								}
							}
							_contactPictures.Add(contactName, picture);
							return picture;
						}
					}
				}
				return null;
			}
		}
	}
}
