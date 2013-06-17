using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Ninject;
using Shane.Church.StirlingBirthday.Core.ViewModels;
using Shane.Church.StirlingBirthday.Core.Services;
using Telerik.Windows.Controls;

namespace Shane.Church.StirlingBirthday.WP
{
	public partial class Settings : PhoneApplicationPage
	{
		public Settings()
		{
			InitializeComponent();

			SettingsViewModel model = KernelService.Kernel.Get<SettingsViewModel>();
			model.HandleChangeAgentError = (ame) =>
			{

			};
		}
	}
}