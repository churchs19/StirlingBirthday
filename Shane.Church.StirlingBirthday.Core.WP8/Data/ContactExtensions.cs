using Microsoft.Phone.UserData;
using Shane.Church.StirlingBirthday.Core.Data;
using System;
using System.Linq;

namespace Shane.Church.StirlingBirthday.Core.WP.Data
{
	public static class ContactExtensions
	{
		public static BirthdayContact GetBirthdayContact(this Contact item)
		{
			BirthdayContact c = new BirthdayContact();

			var itemDate = item.Birthdays.FirstOrDefault().Add(TimeZoneInfo.Local.GetUtcOffset(DateTime.Today) - TimeZoneInfo.Local.BaseUtcOffset);
			c.Date = DateTime.SpecifyKind(itemDate, DateTimeKind.Utc);
			c.DisplayName = item.DisplayName;
			c.Email = item.EmailAddresses.FirstOrDefault() == null ? null : item.EmailAddresses.FirstOrDefault().EmailAddress;
			c.FirstName = item.CompleteName != null && item.CompleteName.FirstName != null ? item.CompleteName.FirstName : "";
			c.LastName = item.CompleteName != null && item.CompleteName.LastName != null ? item.CompleteName.LastName : "";
			c.MobilePhone = item.PhoneNumbers.Where(it => it.Kind == PhoneNumberKind.Mobile).Any() ? item.PhoneNumbers.Where(it => it.Kind == PhoneNumberKind.Mobile).FirstOrDefault().PhoneNumber : null;
			c.HomePhone = item.PhoneNumbers.Where(it => it.Kind == PhoneNumberKind.Home).Any() ? item.PhoneNumbers.Where(it => it.Kind == PhoneNumberKind.Home).FirstOrDefault().PhoneNumber : null;
			c.WorkPhone = item.PhoneNumbers.Where(it => it.Kind == PhoneNumberKind.Work).Any() ? item.PhoneNumbers.Where(it => it.Kind == PhoneNumberKind.Work).FirstOrDefault().PhoneNumber : null;

			return c;
		}
	}
}
