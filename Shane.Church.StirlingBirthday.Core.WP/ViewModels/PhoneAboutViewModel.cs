using Shane.Church.StirlingBirthday.Core.ViewModels;
using Shane.Church.StirlingBirthday.Core.WP.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Shane.Church.StirlingBirthday.Core.WP.ViewModels
{
	public class PhoneAboutViewModel : AboutViewModel
	{
		public override void Initialize()
		{
			RateThisAppCommand = new RateThisAppCommand();
			SendAnEmailCommand = new SendAnEmailCommand();

			var versionAttrib = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
			Version = versionAttrib.Version.ToString();
		}
	}
}
