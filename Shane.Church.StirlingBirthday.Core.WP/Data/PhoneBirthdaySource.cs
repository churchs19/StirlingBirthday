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
    public class PhoneBirthdaySource : IBirthdaySource
    {
        private List<Contact> _contacts;
        private Dictionary<string, byte[]> _contactPictures;

        public PhoneBirthdaySource()
        {
            _contacts = new List<Contact>();
            _contactPictures = new Dictionary<string, byte[]>();
        }

        public Task<IQueryable<BirthdayContact>> GetAllEntriesAsync(bool forceRefresh = false)
        {
            TaskCompletionSource<IQueryable<BirthdayContact>> tcs = new TaskCompletionSource<IQueryable<BirthdayContact>>();
            if (_contacts.Count == 0 || forceRefresh)
            {
                Contacts contacts = new Contacts();
                contacts.SearchCompleted += (s, e) =>
                {
                    try
                    {
                        var items = (from r in e.Results
                                     where r.Birthdays.Any()
                                     select r);
                        _contacts.AddRange(items);
                        tcs.TrySetResult(GetBirthdayContacts().AsQueryable());
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

        private IEnumerable<BirthdayContact> GetBirthdayContacts()
        {
            foreach (var c in _contacts)
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

        public async Task<byte[]> GetContactPicture(string contactName, bool forceRefresh = false)
        {
            if (_contacts.Count == 0 || forceRefresh)
            {
                await GetAllEntriesAsync(forceRefresh);
            }
            if (_contactPictures.ContainsKey(contactName))
                return _contactPictures[contactName];
            else
            {
                var c = _contacts.Where(it => it.DisplayName.ToLower() == contactName.ToLower()).FirstOrDefault();
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
    }
}
