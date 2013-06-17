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
using System.Threading.Tasks;
using Telerik.Windows.Data;
using Shane.Church.StirlingBirthday.Core.Data;
using Microsoft.Phone.Shell;
using Shane.Church.StirlingBirthday.WP.Resources;

namespace Shane.Church.StirlingBirthday.WP
{
	public partial class MainPage : PhoneApplicationPage
	{
		MainViewModel _model;

		// Constructor
		public MainPage()
		{
			InitializeComponent();

			InitializeApplicationBar();
			//Shows the trial reminder message, according to the settings of the TrialReminder.
			//(App.Current as App).trialReminder.Notify();

			//Shows the rate reminder message, according to the settings of the RateReminder.
			(App.Current as App).rateReminder.Notify();

			_model = KernelService.Kernel.Get<MainViewModel>();

			PropertyGroupDescriptor grouping = new PropertyGroupDescriptor("Group");
			grouping.SortMode = ListSortMode.Descending;
			JumpListAll.GroupDescriptors.Add(grouping);
			JumpListAll.GroupPickerItemsSource = null;
			JumpListAll.GroupPickerItemsSource = _model.MonthNames;
		}

		private void InitializeApplicationBar()
		{
			ApplicationBar = new ApplicationBar();
			ApplicationBar.Mode = ApplicationBarMode.Minimized;
			ApplicationBar.Opacity = 0.7;
			ApplicationBar.BackgroundColor = (Color)Application.Current.Resources["AppColor1"];
			ApplicationBar.ForegroundColor = (Color)Application.Current.Resources["AppColorWhite"];

			ApplicationBarIconButton appBarButtonReview = new ApplicationBarIconButton(new Uri("/Images/Rating.png", UriKind.Relative));
			appBarButtonReview.Text = AppResources.RateLabel;
			appBarButtonReview.Click += appBarReview_Click;
			ApplicationBar.Buttons.Add(appBarButtonReview);

			ApplicationBarIconButton appBarButtonAbout = new ApplicationBarIconButton(new Uri("/Images/About.png", UriKind.Relative));
			appBarButtonAbout.Text = AppResources.AboutLabel;
			appBarButtonAbout.Click += appBarAbout_Click;
			ApplicationBar.Buttons.Add(appBarButtonAbout);

			ApplicationBarIconButton appBarButtonSettings = new ApplicationBarIconButton(new Uri("/Images/Settings.png", UriKind.Relative));
			appBarButtonSettings.Text = AppResources.SettingsLabel;
			appBarButtonSettings.Click += appBarButtonSettings_Click;
			ApplicationBar.Buttons.Add(appBarButtonSettings);

			ApplicationBarIconButton appBarButtonPin = new ApplicationBarIconButton(new Uri("/Images/Pin.png", UriKind.Relative));
			appBarButtonPin.Text = AppResources.PinLabel;
			appBarButtonPin.Click += appBarButtonPin_Click;
		}

		void appBarButtonPin_Click(object sender, EventArgs e)
		{
			if (_model.PinCommand.CanExecute(null))
				_model.PinCommand.Execute(null);
		}

		private void appBarButtonSettings_Click(object sender, EventArgs e)
		{
			if (_model.SettingsCommand.CanExecute(null))
				_model.SettingsCommand.Execute(null);
		}

		private void appBarAbout_Click(object sender, EventArgs e)
		{
			if (_model.AboutCommand.CanExecute(null))
				_model.AboutCommand.Execute(null);
		}

		private void appBarReview_Click(object sender, EventArgs e)
		{
			if (_model.RateCommand.CanExecute(null))
				_model.RateCommand.Execute(null);
		}

		protected override async void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
			await _model.LoadData();
			ListBoxUpcoming.ItemsSource = null;
			ListBoxUpcoming.ItemsSource = _model.UpcomingContacts;
			ListBoxPast.ItemsSource = null;
			ListBoxPast.ItemsSource = _model.PastContacts;
			JumpListAll.ItemsSource = null;
			JumpListAll.ItemsSource = _model.AllContacts;
			base.OnNavigatedTo(e);
		}

		private void ListBoxUpcoming_DataRequested(object sender, EventArgs e)
		{
			_model.LoadNextUpcomingContacts();
			if (_model.TotalUpcomingCount == _model.UpcomingContacts.Count)
				ListBoxUpcoming.DataVirtualizationMode = Telerik.Windows.Controls.DataVirtualizationMode.None;
		}

		private void ListBoxPast_DataRequested(object sender, EventArgs e)
		{
			_model.LoadNextPastContacts();
			if (_model.TotalPastCount == _model.PastContacts.Count)
				ListBoxPast.DataVirtualizationMode = Telerik.Windows.Controls.DataVirtualizationMode.None;
		}

		private void JumpListAll_DataRequested(object sender, EventArgs e)
		{
			_model.LoadNextContacts();
			if (_model.TotalCount == _model.AllContacts.Count)
				JumpListAll.DataVirtualizationMode = Telerik.Windows.Controls.DataVirtualizationMode.None;
		}

		private void JumpListAll_GroupPickerItemTap(object sender, Telerik.Windows.Controls.GroupPickerItemTapEventArgs e)
		{
			var keys = JumpListAll.Groups.Select(it => ((MonthGroup)it.Key).MonthIndex).ToList();
			var nextKeyQuery = _model.ContactGroups.Where(it => it.MonthIndex >= ((MonthGroup)e.DataItem).MonthIndex).Select(it => it.MonthIndex);
			var nextKey = nextKeyQuery.Any() ? nextKeyQuery.Min() : _model.ContactGroups.Select(it => it.MonthIndex).Min();
			while (!JumpListAll.Groups.Select(it => ((MonthGroup)it.Key).MonthIndex).Contains(nextKey))
			{
				_model.LoadNextContacts();
				JumpListAll.RefreshData();
			}
			foreach (DataGroup group in JumpListAll.Groups)
			{
				if (object.Equals(((MonthGroup)e.DataItem).MonthIndex, ((MonthGroup)group.Key).MonthIndex))
				{
					e.ClosePicker = true;
					e.ScrollToItem = true;
					e.DataItemToNavigate = group;
					return;
				}
			}
		}
	}
}
