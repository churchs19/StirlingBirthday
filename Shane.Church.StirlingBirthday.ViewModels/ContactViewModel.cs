using System;
using System.Net;
using System.Linq;
using System.IO;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using Microsoft.Phone;
using Microsoft.Phone.UserData;
using System.Windows;
using System.Windows.Media;
using Shane.Church.Utility;
using System.IO.IsolatedStorage;
using NLog;

namespace Shane.Church.StirlingBirthday.ViewModels
{
    public class ContactViewModel : INotifyPropertyChanged, IDisposable
    {
		private Contact _contact = null;
		private static Logger _logger = LogManager.GetCurrentClassLogger();

		private Contact Contact
		{
			get { return _contact; }
			set
			{
				if (_contact != value)
				{
					_contact = value;
					NotifyPropertyChanged("FirstName");
					NotifyPropertyChanged("LastName");
					NotifyPropertyChanged("DisplayName");
					NotifyPropertyChanged("Date");
					NotifyPropertyChanged("DayText");
					NotifyPropertyChanged("MonthText");
					NotifyPropertyChanged("MonthFullText");
					NotifyPropertyChanged("YearText");
					NotifyPropertyChanged("YearText");
					NotifyPropertyChanged("StartTileDateText");
					NotifyPropertyChanged("Picture");
					NotifyPropertyChanged("HomePhone");
					NotifyPropertyChanged("MobilePhone");
					NotifyPropertyChanged("WorkPhone");
					NotifyPropertyChanged("Email");
					NotifyPropertyChanged("Phone");
					NotifyPropertyChanged("PhoneNumber");
					NotifyPropertyChanged("MobilePhoneNumber");
					NotifyPropertyChanged("HomePhoneNumber");
					NotifyPropertyChanged("WorkPhoneNumber");
					NotifyPropertyChanged("EmailPhoneNumber");
					NotifyPropertyChanged("TextVisibility");
					NotifyPropertyChanged("HomeMenuVisibility");
					NotifyPropertyChanged("MobileMenuVisibility");
					NotifyPropertyChanged("WorkMenuVisibility");
					NotifyPropertyChanged("EmailVisibility");
					NotifyPropertyChanged("HomeMenuText");
					NotifyPropertyChanged("MobileMenuText");
					NotifyPropertyChanged("WorkMenuText");
					NotifyPropertyChanged("ForegroundBrush");
					NotifyPropertyChanged("BackgroundBrush");
					NotifyPropertyChanged("SMSMenuText");
					NotifyPropertyChanged("EmailMenuText");
				}
			}
		}

		public ContactViewModel()
		{
			NumDaysSinceDate = int.MaxValue;
			NumDaysUntilDate = int.MaxValue;
			Years = 0;
        }

		public ContactViewModel(Contact c)
		{
			Contact = c;
			
			NumDaysSinceDate = 0;

			DateTime compareDate = DateTime.MinValue;
			if (Date.Day == 29 && Date.Month == 2)
			{
				compareDate = new DateTime(DateTime.Today.Year, 2, 28);
			}
			else
			{
				compareDate = new DateTime(DateTime.Today.Year, Date.Month, Date.Day);
			}
			//            NumDaysUntilDate = compareDate.DayOfYear - DateTime.Today.DayOfYear;
			TimeSpan span = compareDate - DateTime.Today;
			if (span.TotalDays < 0)
			{
				NumDaysSinceDate = Convert.ToInt32(-span.TotalDays);
				span = compareDate.AddYears(1) - DateTime.Today;
			}
			else
			{
				TimeSpan daysSince = compareDate.AddYears(-1) - DateTime.Today;
				NumDaysSinceDate = Convert.ToInt32(daysSince.TotalDays);
			}
			NumDaysUntilDate = Convert.ToInt32(span.TotalDays);

			//            DateTime nextDate = DateTime.Today.AddDays(NumDaysUntilDate);
			Years = compareDate.Year - Date.Year;
		}

		public string FirstName
		{
			get { return _contact.CompleteName.FirstName; }
		}

		public string LastName
		{
			get { return _contact.CompleteName.LastName; }
		}

		public string DisplayName
		{
			get { return _contact.DisplayName; }
		}

		public DateTime Date
		{
			get 
			{ 
				var birthday = _contact.Birthdays.FirstOrDefault();
				return birthday.ToUniversalTime() + DateTimeOffset.Now.Offset;
			}
		}

		public string DayText
		{
			get
			{
				return Date.Day.ToString();
			}
		}

		public string MonthText
		{
			get
			{
				return Date.ToString("MMM");
			}
		}

		public string MonthFullText
		{
			get
			{
				return Date.ToString("MMMM");
			}
		}

		public string YearText
		{
			get
			{
				return Date.ToString("yyyy");
			}
		}

		public string StartTileDateText
		{
			get
			{
				if (NumDaysUntilDate == 0)
				{
					return "Today";
				}
				else if (NumDaysUntilDate == 1)
				{
					return "Tomorrow";
				}
				else
				{
					return MonthText + " " + DayText;
				}
			}
		}

		public WriteableBitmap Picture
		{
			get 
			{
				try
				{
					return PictureDecoder.DecodeJpeg(_contact.GetPicture(), 173, 173);
				}
				catch
				{
					return null;
				}
			}
		}

		private int _numDaysUntilDate;
		public int NumDaysUntilDate
		{
			get { return _numDaysUntilDate; }
			set
			{
				if (_numDaysUntilDate != value)
				{
					_numDaysUntilDate = value;
					NotifyPropertyChanged("NumDaysUntilDate");
				}
			}
		}

		private int _numDaysSinceDate;
		public int NumDaysSinceDate
		{
			get { return _numDaysSinceDate; }
			set
			{
				if (_numDaysSinceDate != value)
				{
					_numDaysSinceDate = value;
					NotifyPropertyChanged("NumDaysSinceDate");
				}
			}
		}

		private int _years;
		public int Years
		{
			get { return _years; }
			set
			{
				if (_years != value)
				{
					_years = value;
					NotifyPropertyChanged("Years");
					NotifyPropertyChanged("YearsText");
				}
			}
		}

		public string YearsText
		{
			get
			{
				if (Years == 1)
				{
					return Years.ToString() + " year";
				}
				else
				{
					return Years.ToString() + " years";
				}
			}
		}

		public string HomePhone
		{
			get
			{
				try
				{
					return _contact.PhoneNumbers.Where(m => (m.Kind == PhoneNumberKind.Home)).FirstOrDefault().PhoneNumber;
				}
				catch { return null; }
			}
		}

		public string MobilePhone
		{
			get
			{
				try
				{
					return _contact.PhoneNumbers.Where(m => (m.Kind == PhoneNumberKind.Mobile)).FirstOrDefault().PhoneNumber;
				}
				catch { return null; }
			}
		}

		public string WorkPhone
		{
			get
			{
				try
				{
					return _contact.PhoneNumbers.Where(m => (m.Kind == PhoneNumberKind.Work)).FirstOrDefault().PhoneNumber;
				}
				catch { return null; }
			}
		}

		public string Email
		{
			get
			{
				try
				{
					return _contact.EmailAddresses.FirstOrDefault().EmailAddress;
				}
				catch { return null; }
			}
		}

		public string Phone
		{
			get
			{
				if (!string.IsNullOrWhiteSpace(MobilePhone))
					return MobilePhone;
				else if (!string.IsNullOrWhiteSpace(HomePhone))
					return HomePhone;
				else
					return WorkPhone;
			}
		}

		public PhoneNumber PhoneNumber
		{
			get
			{
				return new PhoneNumber() { DisplayName = DisplayName, Number = Phone };
			}
		}

		public PhoneNumber MobilePhoneNumber
		{
			get
			{
				return new PhoneNumber() { DisplayName = DisplayName, Number = MobilePhone };
			}
		}

		public PhoneNumber HomePhoneNumber
		{
			get
			{
				return new PhoneNumber() { DisplayName = DisplayName, Number = HomePhone };
			}
		}

		public PhoneNumber WorkPhoneNumber
		{
			get
			{
				return new PhoneNumber() { DisplayName = DisplayName, Number = WorkPhone };
			}
		}

		public PhoneNumber EmailPhoneNumber
		{
			get
			{
				return new PhoneNumber() { DisplayName = DisplayName, Number = Email };
			}
		}

		public Visibility TextVisibility
		{
			get
			{
				return (!string.IsNullOrWhiteSpace(MobilePhone)) ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		public Visibility HomeMenuVisibility
		{
			get
			{
				return (!string.IsNullOrWhiteSpace(HomePhone)) ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		public Visibility MobileMenuVisibility
		{
			get
			{
				return TextVisibility;
			}
		}

		public Visibility WorkMenuVisibility
		{
			get
			{
				return (!string.IsNullOrWhiteSpace(WorkPhone)) ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		public Visibility EmailVisibility
		{
			get
			{
				return (!string.IsNullOrWhiteSpace(Email)) ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		public string HomeMenuText
		{
			get
			{
				return "Call Home";
			}
		}

		public string MobileMenuText
		{
			get
			{
				return "Call Mobile";
			}
		}

		public string WorkMenuText
		{
			get
			{
				return "Call Work";
			}
		}

		public string SMSMenuText
		{
			get
			{
				return "Send Text";
			}
		}
		
		public string EmailMenuText
		{
			get
			{
				return "Send Email";
			}
		}

		public Brush ForegroundBrush
		{
			get
			{
				return (NumDaysUntilDate == 0) ? (Brush)Application.Current.Resources["PhoneContrastForegroundBrush"] : (Brush)Application.Current.Resources["PhoneForegroundBrush"];
			}
		}

		public Brush BackgroundBrush
		{
			get
			{
				return (NumDaysUntilDate == 0) ? (Brush)Application.Current.Resources["PhoneContrastBackgroundBrush"] : (Brush)Application.Current.Resources["TransparentBrush"];
			}
		}

		public Uri PictureUri
		{
		    get
		    {
				try
				{
					using (IsolatedStorageFile appStorage = IsolatedStorageFile.GetUserStoreForApplication())
					{
						if (appStorage.FileExists(IsoStorePath))
							return new Uri("isostore:" + IsoStorePath, UriKind.Absolute);
					}
					return null;
				}
				catch { return null; }
		    }
		}

		public string IsoStorePath
		{
			get
			{
				return "/Contacts/" + FirstName + LastName + ".jpg";
			}
		}

		public void SavePicture()
		{
			try
			{
				Imaging.SaveImage(this.Picture, IsoStorePath, Imaging.ImageType.Jpeg);
			}
			catch(Exception ex) 
			{
				_logger.DebugException("Error saving picture", ex);
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

		public void Dispose()
		{
			try
			{
				using (IsolatedStorageFile appStorage = IsolatedStorageFile.GetUserStoreForApplication())
				{
					if (appStorage.FileExists(IsoStorePath))
					{
						appStorage.DeleteFile(IsoStorePath);
					}
				}
			}
			catch { }
		}
	}
}
