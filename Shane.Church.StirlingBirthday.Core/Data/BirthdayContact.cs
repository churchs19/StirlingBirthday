using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shane.Church.StirlingBirthday.Core.Data
{
	public class BirthdayContact
	{
		string FirstName { get; set; }
		string LastName { get; set; }
		string DisplayName { get; set; }
		string Date { get; set; }
		Uri Picture { get; set; }
		string HomePhone { get; set; }
		string MobilePhone { get; set; }
		string WorkPhone { get; set; }
		string Email { get; set; }
	}
}
