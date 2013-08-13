using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shane.Church.StirlingBirthday.Core.Data
{
	public class BirthdayContact
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string DisplayName { get; set; }
		public DateTime Date { get; set; }
		public string HomePhone { get; set; }
		public string MobilePhone { get; set; }
		public string WorkPhone { get; set; }
		public string Email { get; set; }
		public int DaysUntil
		{
			get
			{
				DateTime today = DateTime.Today;
				DateTime next = Date.Date.AddYears(today.Year - Date.Date.Year);

				if (next < today)
					next = next.AddYears(1);

				return (next - today).Days;
			}
		}
		public int DaysSince
		{
			get
			{
				DateTime today = DateTime.Today;
				DateTime prev = Date.Date.AddYears(today.Year - Date.Date.Year);

				if (prev > today)
					prev = prev.AddYears(-1);

				return (today - prev).Days;
			}
		}
		public int Age
		{
			get
			{
				DateTime today = DateTime.Today;
				DateTime next = Date.Date.AddYears(today.Year - Date.Date.Year);

				if (next < today)
					next = next.AddYears(1);

				return next.Year - Date.Date.Year;
			}
		}
	}
}
