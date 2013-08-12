using Microsoft.Phone.UserData;
using Shane.Church.StirlingBirthday.Core.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shane.Church.StirlingBirthday.Core.WP.Data
{
    public static class ContactExtensions
    {
        public static async Task<BirthdayContact> GetBirthdayContact(this Contact item, bool loadPicture = true)
        {
            BirthdayContact c = new BirthdayContact();
            c.Date = DateTime.SpecifyKind(item.Birthdays.FirstOrDefault(), DateTimeKind.Utc);
            c.DisplayName = item.DisplayName;
            c.Email = item.EmailAddresses.FirstOrDefault() == null ? null : item.EmailAddresses.FirstOrDefault().EmailAddress;
            c.FirstName = item.CompleteName.FirstName;
            c.LastName = item.CompleteName.LastName;
            c.MobilePhone = item.PhoneNumbers.Where(it => it.Kind == PhoneNumberKind.Mobile).Any() ? item.PhoneNumbers.Where(it => it.Kind == PhoneNumberKind.Mobile).FirstOrDefault().PhoneNumber : null;
            c.HomePhone = item.PhoneNumbers.Where(it => it.Kind == PhoneNumberKind.Home).Any() ? item.PhoneNumbers.Where(it => it.Kind == PhoneNumberKind.Home).FirstOrDefault().PhoneNumber : null;
            c.WorkPhone = item.PhoneNumbers.Where(it => it.Kind == PhoneNumberKind.Work).Any() ? item.PhoneNumbers.Where(it => it.Kind == PhoneNumberKind.Work).FirstOrDefault().PhoneNumber : null;

            if (loadPicture)
            {
                Stream pictureStream = item.GetPicture();
                if (pictureStream != null)
                {
                    c.Picture = new byte[pictureStream.Length];
                    MemoryStream pictureMemoryStream = new MemoryStream(c.Picture);
                    if (pictureStream.CanRead)
                    {
                        await pictureStream.CopyToAsync(pictureMemoryStream);
                    }
                }
            }

            return c;
        }
    }
}
