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
using Microsoft.Phone.Shell;
using Shane.Church.StirlingBirthday.ViewModels;
using Microsoft.Phone.Tasks;
using Coding4Fun.Phone.Controls;
using Shane.Church.StirlingBirthday.Controls;

namespace Shane.Church.StirlingBirthday
{
	public partial class MainPage : PhoneApplicationPage
	{
		ProgressIndicator _progress = null;

		// Constructor
		public MainPage()
		{
			InitializeComponent();

			_progress = new ProgressIndicator();
			_progress.IsVisible = false;
			_progress.IsIndeterminate = true;
			_progress.Text = "Loading...";
			SystemTray.SetProgressIndicator(this, _progress);
		}

		private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
		{
			LittleWatson.CheckForPreviousException();
			_progress.IsVisible = true;
			App.ViewModel.Begin_LoadData().ContinueWith(t =>
			{
				if (t.IsCompleted)
				{
					Deployment.Current.Dispatcher.BeginInvoke(() =>
					{
						this.DataContext = App.ViewModel;
						_progress.IsVisible = false;
						BirthdayTile tile = new BirthdayTile();
						tile.UpdateTile(App.ViewModel.Upcoming.Take(3));
					});
				}
			});
		}

		private void Text_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				PhoneNumber number = (sender as FrameworkElement).Tag as PhoneNumber;

				if (number != null)
				{
					SmsComposeTask text = new SmsComposeTask();
					text.To = number.Number;
					text.Body = "Happy Birthday!";
					text.Show();
				}
			}
			catch { }
		}

		private void Call_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				PhoneNumber number = (sender as FrameworkElement).Tag as PhoneNumber;

				CallNumber(number);
			}
			catch { }
		}

		private void CallNumber(PhoneNumber number)
		{
			try
			{
				if (number != null)
				{
					PhoneCallTask call = new PhoneCallTask();
					call.DisplayName = number.DisplayName;
					call.PhoneNumber = number.Number;
					call.Show();
				}
			}
			catch {	}
		}

		private void Email_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				PhoneNumber number = (sender as FrameworkElement).Tag as PhoneNumber;

				if (number != null)
				{
					EmailComposeTask email = new EmailComposeTask();
					email.To = number.Number;
					email.Subject = "Happy Birthday!";
					email.Body = "Happy Birthday!";
					email.Show();
				}
			}
			catch { }
		}

		private void iconButtonSettings_Click(object sender, EventArgs e)
		{
			NavigationService.Navigate(new Uri(@"/Settings.xaml", UriKind.Relative));
		}

		private void iconButtonAbout_Click(object sender, EventArgs e)
		{
			AboutPrompt prompt = new AboutPrompt();
			prompt.Show("Shane Church", null, "shane@s-church.net", "http://www.s-church.net");
		}
	}
}