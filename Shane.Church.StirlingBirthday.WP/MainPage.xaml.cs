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

namespace Shane.Church.StirlingBirthday.WP
{
	public partial class MainPage : PhoneApplicationPage
	{
		MainViewModel _model;

		// Constructor
		public MainPage()
		{
			InitializeComponent();

			//Shows the trial reminder message, according to the settings of the TrialReminder.
			//(App.Current as App).trialReminder.Notify();

			//Shows the rate reminder message, according to the settings of the RateReminder.
			(App.Current as App).rateReminder.Notify();

			_model = KernelService.Kernel.Get<MainViewModel>();

			PropertyGroupDescriptor grouping = new PropertyGroupDescriptor("Group");
			grouping.SortMode = ListSortMode.Descending;
			JumpListAll.GroupDescriptors.Add(grouping);
			//GenericGroupDescriptor<ContactViewModel, MonthGroup> groupByMonth = new GenericGroupDescriptor<ContactViewModel, MonthGroup>();
			//groupByMonth.KeySelector = it =>
			//{
			//	return new MonthGroup(it.Date);
			//};
			//groupByMonth.SortMode = ListSortMode.Ascending;
			//JumpListAll.GroupDescriptors.Add(groupByMonth);
			JumpListAll.GroupPickerItemsSource = null;
			JumpListAll.GroupPickerItemsSource = _model.MonthNames;
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
