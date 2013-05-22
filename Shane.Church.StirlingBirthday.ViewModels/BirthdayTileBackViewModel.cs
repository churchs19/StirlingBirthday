using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Shane.Church.StirlingBirthday.ViewModels
{
	public class BirthdayTileBackViewModel : INotifyPropertyChanged
	{

		private string _name1;
		public string Name1
		{
			get { return _name1; }
			set
			{
				if (_name1 != value)
				{
					_name1 = value;
					NotifyPropertyChanged("Name1");
				}
			}
		}

		private string _name2;
		public string Name2
		{
			get { return _name2; }
			set
			{
				if (_name2 != value)
				{
					_name2 = value;
					NotifyPropertyChanged("Name2");
				}
			}
		}

		private string _name3;
		public string Name3
		{
			get { return _name3; }
			set
			{
				if (_name3 != value)
				{
					_name3 = value;
					NotifyPropertyChanged("Name3");
				}
			}
		}

		private string _date1;
		public string Date1
		{
			get { return _date1; }
			set
			{
				if (_date1 != value)
				{
					_date1 = value;
					NotifyPropertyChanged("Date1");
				}
			}
		}

		private string _date2;
		public string Date2
		{
			get { return _date2; }
			set
			{
				if (_date2 != value)
				{
					_date2 = value;
					NotifyPropertyChanged("Date2");
				}
			}
		}

		private string _date3;
		public string Date3
		{
			get { return _date3; }
			set
			{
				if (_date3 != value)
				{
					_date3 = value;
					NotifyPropertyChanged("Date3");
				}
			}
		}

		public Visibility Field1Visibility
		{
			get
			{
				return string.IsNullOrWhiteSpace(Name1) ? Visibility.Collapsed : Visibility.Visible;
			}
		}

		public Visibility Field2Visibility
		{
			get
			{
				return string.IsNullOrWhiteSpace(Name2) ? Visibility.Collapsed : Visibility.Visible;
			}
		}

		public Visibility Field3Visibility
		{
			get
			{
				return string.IsNullOrWhiteSpace(Name3) ? Visibility.Collapsed : Visibility.Visible;
			}
		}

		public Visibility NoBirthdaysVisibility
		{
			get
			{
				return Field1Visibility == Visibility.Collapsed && Field2Visibility == Visibility.Collapsed && Field3Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
			}
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
