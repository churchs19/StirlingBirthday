using Inneractive.Nokia.Ad;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Ninject;
using Shane.Church.StirlingBirthday.Core.Data;
using Shane.Church.StirlingBirthday.Core.Services;
using Shane.Church.StirlingBirthday.Core.ViewModels;
using Shane.Church.StirlingBirthday.Strings;
using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;

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

			InitializeAdControl();

#if !PERSONAL
			//Shows the trial reminder message, according to the settings of the TrialReminder.
			(App.Current as App).trialReminder.Notify();

			//Shows the rate reminder message, according to the settings of the RateReminder.
			(App.Current as App).rateReminder.Notify();
#endif

			_model = KernelService.Kernel.Get<MainViewModel>();
            _model.DataLoaded += _model_DataLoaded;

			PropertyGroupDescriptor grouping = new PropertyGroupDescriptor("Group");
			grouping.SortMode = ListSortMode.Descending;
			JumpListAll.GroupDescriptors.Add(grouping);
			JumpListAll.GroupPickerItemsSource = null;
			JumpListAll.GroupPickerItemsSource = _model.MonthNames;
		}

		#region Ad Control
		private void InitializeAdControl()
		{
#if !PERSONAL
			if ((App.Current as App).trialReminder.IsTrialMode())
			{
				AdControl.AdReceived += new InneractiveAd.IaAdReceived(AdControl_AdReceived);
				AdControl.AdFailed += new InneractiveAd.IaAdFailed(AdControl_AdFailed);
				AdControl.DefaultAdReceived += new InneractiveAd.IaDefaultAdReceived(AdControl_DefaultAdReceived);
			}
			else
			{
				AdPanel.Children.Remove(AdControl);
				AdControl = null;
			}
#else
			AdPanel.Children.Remove(AdControl);
			AdControl = null;
#endif
		}

		void AdControl_DefaultAdReceived(object sender)
		{
			AdControl.Visibility = System.Windows.Visibility.Visible;
		}

		private void AdControl_AdReceived(object sender)
		{
			AdControl.Visibility = System.Windows.Visibility.Visible;
		}

		private void AdControl_AdFailed(object sender)
		{
			AdControl.Visibility = System.Windows.Visibility.Collapsed;
		}
		#endregion

        #region App Bar
        private void InitializeApplicationBar()
		{
			ApplicationBar = new ApplicationBar();
			ApplicationBar.Mode = ApplicationBarMode.Minimized;
			ApplicationBar.Opacity = 1.0;
			ApplicationBar.BackgroundColor = (Color)Application.Current.Resources["AppColor1"];
			ApplicationBar.ForegroundColor = (Color)Application.Current.Resources["AppColorWhite"];

            //ApplicationBarIconButton appBarButtonPeople = new ApplicationBarIconButton(new Uri("/Images/People.png", UriKind.Relative));
            //appBarButtonPeople.Text = Shane.Church.StirlingBirthday.Strings.Resources.OpenPeopleAppLabel;
            //appBarButtonPeople.Click += appBarButtonPeople_Click;
            //ApplicationBar.Buttons.Add(appBarButtonPeople);

            //ApplicationBarIconButton appBarButtonAddBirthday = new ApplicationBarIconButton(new Uri("/Images/Add-New.png", UriKind.Relative));
            //appBarButtonAddBirthday.Text = Shane.Church.StirlingBirthday.Strings.Resources.AddContactLabel;
            //appBarButtonAddBirthday.Click += appBarMenuItemAddBirthday_Click;
            //ApplicationBar.Buttons.Add(appBarButtonAddBirthday);

			ApplicationBarIconButton appBarButtonReview = new ApplicationBarIconButton(new Uri("/Images/Rating.png", UriKind.Relative));
			appBarButtonReview.Text = Shane.Church.StirlingBirthday.Strings.Resources.RateLabel;
			appBarButtonReview.Click += appBarReview_Click;
			ApplicationBar.Buttons.Add(appBarButtonReview);

			ApplicationBarIconButton appBarButtonAbout = new ApplicationBarIconButton(new Uri("/Images/About.png", UriKind.Relative));
            appBarButtonAbout.Text = Shane.Church.StirlingBirthday.Strings.Resources.AboutLabel;
			appBarButtonAbout.Click += appBarAbout_Click;
			ApplicationBar.Buttons.Add(appBarButtonAbout);



            //ApplicationBarIconButton appBarButtonPin = new ApplicationBarIconButton(new Uri("/Images/Pin.png", UriKind.Relative));
            //appBarButtonPin.Text = Shane.Church.StirlingBirthday.Strings.Resources.PinLabel;
            //appBarButtonPin.Click += appBarButtonPin_Click;

#if DEBUG
			ApplicationBarMenuItem appBarMenuItemLaunchAgent = new ApplicationBarMenuItem("Debug Scheduled Agent");
			appBarMenuItemLaunchAgent.Click += (s, e) =>
			{
				KernelService.Kernel.Get<IAgentManagementService>().StartAgent(true);
			};
			ApplicationBar.MenuItems.Add(appBarMenuItemLaunchAgent);

			ApplicationBarMenuItem appBarMenuItemUnhandledException = new ApplicationBarMenuItem("Unhandled Exception");
			appBarMenuItemUnhandledException.Click += (s, e) =>
				{
					throw new Exception("Testing diagnostics");
				};
			ApplicationBar.MenuItems.Add(appBarMenuItemUnhandledException);
#endif
		}

        void appBarButtonPeople_Click(object sender, EventArgs e)
        {
            if (_model.ViewPeopleCommand.CanExecute(null))
                _model.ViewPeopleCommand.Execute(null);
        }

        void appBarMenuItemAddBirthday_Click(object sender, EventArgs e)
        {
            if (_model.AddContactCommand.CanExecute(null))
                _model.AddContactCommand.Execute(null);
        }

		void appBarButtonPin_Click(object sender, EventArgs e)
		{
			if (_model.PinCommand.CanExecute(null))
				_model.PinCommand.Execute(null);
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
        #endregion

        protected override async void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
            base.OnNavigatedTo(e);

            if (!_model.IsLoading)
            {
                await _model.LoadData();
            }

			var tileService = KernelService.Kernel.Get<ITileUpdateService>();
			await tileService.SaveUpcomingImages();
			await tileService.UpdateTile();
		}

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if(ApplicationUsageHelper.ApplicationRunsCountForCurrentVersion == 1)
            {
                //Show intro dialog
                MessageBox.Show(Shane.Church.StirlingBirthday.Strings.Resources.WelcomeText, Shane.Church.StirlingBirthday.Strings.Resources.WelcomeCaption, MessageBoxButton.OK);
            }
        }

        void _model_DataLoaded(object sender, EventArgs e)
        {
            ListBoxUpcoming.ItemsSource = null;
            ListBoxUpcoming.DataVirtualizationMode = Telerik.Windows.Controls.DataVirtualizationMode.OnDemandAutomatic;
            ListBoxUpcoming.ItemsSource = _model.UpcomingContacts;
            ListBoxPast.ItemsSource = null;
            ListBoxPast.DataVirtualizationMode = Telerik.Windows.Controls.DataVirtualizationMode.OnDemandAutomatic;
            ListBoxPast.ItemsSource = _model.PastContacts;
            JumpListAll.ItemsSource = null;
            JumpListAll.DataVirtualizationMode = Telerik.Windows.Controls.DataVirtualizationMode.OnDemandAutomatic;
            JumpListAll.ItemsSource = _model.AllContacts;
        }

		private void ListBoxUpcoming_DataRequested(object sender, EventArgs e)
		{
			BusyIndicatorUpcoming.IsRunning = true;
			_model.LoadNextUpcomingContacts();
			if (_model.TotalUpcomingCount == _model.UpcomingContacts.Count)
				ListBoxUpcoming.DataVirtualizationMode = Telerik.Windows.Controls.DataVirtualizationMode.None;
			BusyIndicatorUpcoming.IsRunning = false;
		}

		private void ListBoxPast_DataRequested(object sender, EventArgs e)
		{
			BusyIndicatorPast.IsRunning = true;
			_model.LoadNextPastContacts();
			if (_model.TotalPastCount == _model.PastContacts.Count)
				ListBoxPast.DataVirtualizationMode = Telerik.Windows.Controls.DataVirtualizationMode.None;
			BusyIndicatorPast.IsRunning = false;
		}

		private void JumpListAll_DataRequested(object sender, EventArgs e)
		{
			BusyIndicatorAll.IsRunning = true;
			_model.LoadNextContacts(30);
			if (_model.TotalCount == _model.AllContacts.Count)
				JumpListAll.DataVirtualizationMode = Telerik.Windows.Controls.DataVirtualizationMode.None;
			BusyIndicatorAll.IsRunning = false;
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
				if (object.Equals(nextKey, ((MonthGroup)group.Key).MonthIndex))
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
