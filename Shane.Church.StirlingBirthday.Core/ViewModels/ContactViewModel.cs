using Shane.Church.StirlingBirthday.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shane.Church.StirlingBirthday.Core.ViewModels
{
	public abstract class ContactViewModel
	{
		private IContact _contact;

		public ContactViewModel(IContact contact)
		{

		}
	}
}
