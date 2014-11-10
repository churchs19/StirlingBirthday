using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Ninject;
using Shane.Church.StirlingBirthday.Core.ViewModels;
using Shane.Church.StirlingBirthday.Core.Services;

namespace Shane.Church.StirlingBirthday.WP
{
	public partial class About : PhoneApplicationPage
	{
        IWebNavigationService _webNav;

		public About()
		{
			InitializeComponent();

            _webNav = KernelService.Kernel.Get<IWebNavigationService>();

			this.DataContext = KernelService.Kernel.Get<AboutViewModel>();
		}

        private void PrivacyButton_Click(object sender, RoutedEventArgs e)
        {
            _webNav.NavigateTo(new Uri("http://bit.ly/stirlingbirthdayprivacy"));
        }
	}
}
