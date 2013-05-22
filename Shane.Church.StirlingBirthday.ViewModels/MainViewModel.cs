using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using Microsoft.Phone.UserData;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Shane.Church.Utility;

namespace Shane.Church.StirlingBirthday.ViewModels
{
	public class MainViewModel : INotifyPropertyChanged
	{
		public MainViewModel()
		{
			All = new List<ContactViewModel>();
		}

		private DateTime _lastUpdated;
		public DateTime LastUpdated
		{
			get { return _lastUpdated; }
			set
			{
				if (_lastUpdated != value)
				{
					_lastUpdated = value;
					NotifyPropertyChanged("LastUpdated");
				}
			}
		}

		public IEnumerable<ContactViewModel> Upcoming
		{
			get 
			{
				var upcoming = (from cvm in All
								where cvm.NumDaysUntilDate <= 30
								orderby cvm.NumDaysUntilDate
								select cvm);
				return upcoming;
			}
		}

		public Visibility NoUpcomingVisibility
		{
			get
			{
				return IsDataLoaded && Upcoming.Any() ? Visibility.Collapsed : Visibility.Visible;
			}
		}

		List<ContactViewModel> _all;
		public List<ContactViewModel> All
		{
			get { return _all; }
			set
			{
				if (_all != value)
				{
					_all = value;
					NotifyPropertyChanged("All");
					NotifyPropertyChanged("AllDisplay");
					NotifyPropertyChanged("Upcoming");
					NotifyPropertyChanged("NoUpcomingVisibility");
					NotifyPropertyChanged("Past");
				}
			}
		}

		public IEnumerable<Group<ContactViewModel>> AllDisplay
		{
			get
			{
				var all = (from cvm in All
						   group cvm by cvm.MonthFullText into g
							select new Group<ContactViewModel>(g.Key, DateTime.ParseExact(g.Key, "MMMM", CultureInfo.CurrentCulture).ToString("MMM"), g.OrderBy(a=>a.NumDaysUntilDate)));
				return all;
			}
		}

		public IEnumerable<ContactViewModel> Past
		{
			get 
			{
				var past = (from cvm in All
							where cvm.NumDaysSinceDate > 0 && cvm.NumDaysSinceDate <= 30
							orderby cvm.NumDaysSinceDate
							select cvm);
				return past;
			}
		}

		public Visibility NoPastVisibility
		{
			get
			{
				return IsDataLoaded && Past.Any() ? Visibility.Collapsed : Visibility.Visible;
			}
		}

		private bool _isDataLoaded;
		private bool IsDataLoaded
		{
			get
			{
				return _isDataLoaded;
			}
			set
			{
				if (_isDataLoaded != value)
				{
					_isDataLoaded = value;
					NotifyPropertyChanged("IsDataLoaded");
					NotifyPropertyChanged("LoadingVisibility");
				}
			}
		}

		public Visibility LoadingVisibility
		{
			get
			{
				return IsDataLoaded ? Visibility.Collapsed : Visibility.Visible;
			}
		}

		public Task<bool> Begin_LoadData(bool force = false)
		{
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>(TaskCreationOptions.AttachedToParent);
	
			Contacts contacts = new Contacts();
			var eventTask = EventAsync.FromEvent<ContactsSearchEventArgs>(contacts, "SearchCompleted").ContinueWith(t =>
			{
				var items = (from r in t.Result.Results
							   where r.Birthdays.Any()
							   select new ContactViewModel(r)).ToList();
				var sorted = items.OrderBy(m => m.NumDaysUntilDate);
				All.AddRange(sorted);
				IsDataLoaded = true;
				LastUpdated = DateTime.Now;
				var firstContact = Upcoming.FirstOrDefault();
				Deployment.Current.Dispatcher.BeginInvoke(() => { try { firstContact.SavePicture(); } catch { } });
				tcs.TrySetResult(true);
			});
			if (!IsDataLoaded || force)
			{
				contacts.SearchAsync(String.Empty, FilterKind.None, "Load Contacts");
			}
			else
			{
				tcs.TrySetResult(true);
			}

			return tcs.Task;
		}
		
		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(String propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (null != handler)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
