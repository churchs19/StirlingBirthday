using GalaSoft.MvvmLight;
using Ninject;
using Shane.Church.StirlingBirthday.Core.Data;
using Shane.Church.StirlingBirthday.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Shane.Church.StirlingBirthday.Core.ViewModels
{
	public abstract class ContactViewModel : ObservableObject
	{
        private BirthdayContact _contact;
		public ContactViewModel()
		{

		}

		public ContactViewModel(BirthdayContact contact)
		{
            _contact = contact;
			FirstName = contact.FirstName;
			LastName = contact.LastName;
			DisplayName = contact.DisplayName;
			Date = contact.Date;
			HomePhone = contact.HomePhone;
			MobilePhone = contact.MobilePhone;
			WorkPhone = contact.WorkPhone;
			Email = contact.Email;
			DaysUntil = contact.DaysUntil;
			DaysSince = contact.DaysSince;
			Age = contact.Age;
		}

		private string _firstName;
		public string FirstName
		{
			get { return _firstName; }
			set
			{
				Set(() => FirstName, ref _firstName, value);
			}
		}
		private string _lastName;
		public string LastName
		{
			get { return _lastName; }
			set
			{
				Set(() => LastName, ref _lastName, value);
			}
		}
		private string _displayName;
		public string DisplayName
		{
			get { return _displayName; }
			set
			{
				Set(() => DisplayName, ref _displayName, value);
			}
		}
		private DateTime _date;
		public DateTime Date
		{
			get { return _date; }
			set
			{
				Set(() => Date, ref _date, value);
			}
		}
		private string _homePhone;
		public string HomePhone
		{
			get { return _homePhone; }
			set
			{
				if (Set(() => HomePhone, ref _homePhone, value))
					RaisePropertyChanged(() => HasHomePhone);
			}
		}
		private string _mobilePhone;
		public string MobilePhone
		{
			get { return _mobilePhone; }
			set
			{
				if (Set(() => MobilePhone, ref _mobilePhone, value))
					RaisePropertyChanged(() => HasMobilePhone);
			}
		}
		private string _workPhone;
		public string WorkPhone
		{
			get { return _workPhone; }
			set
			{
				if (Set(() => WorkPhone, ref _workPhone, value))
					RaisePropertyChanged(() => HasWorkPhone);
			}
		}
		private string _email;
		public string Email
		{
			get { return _email; }
			set
			{
				if (Set(() => Email, ref _email, value))
					RaisePropertyChanged(() => HasEmail);
			}
		}
		private int _daysUntil;
		public int DaysUntil
		{
			get { return _daysUntil; }
			set
			{
				Set(() => DaysUntil, ref _daysUntil, value);
			}
		}
		private int _daysSince;
		public int DaysSince
		{
			get { return _daysSince; }
			set
			{
				Set(() => DaysSince, ref _daysSince, value);
			}
		}
		private int _age;
		public int Age
		{
			get { return _age; }
			set
			{
				Set(() => Age, ref _age, value);
			}
		}

		public bool HasHomePhone
		{
			get { return !string.IsNullOrWhiteSpace(HomePhone); }
		}

		public bool HasMobilePhone
		{
			get { return !string.IsNullOrWhiteSpace(MobilePhone); }
		}

		public bool HasWorkPhone
		{
			get { return !string.IsNullOrWhiteSpace(WorkPhone); }
		}

		public bool HasEmail
		{
			get { return !string.IsNullOrWhiteSpace(Email); }
		}

		public string MonthText
		{
			get { return Date.ToString("MMM"); }
		}

		public string YearText
		{
			get { return Date.ToString("yyyy"); }
		}

		public string DayText
		{
			get { return Date.Day.ToString(); }
		}

		public virtual string AgeText
		{
			get
			{
				return Age.ToString();
			}
		}

		public MonthGroup Group
		{
			get
			{
				return new MonthGroup(this.Date);
			}
		}

		private ICommand _callHomeCommand;
		public ICommand CallHomeCommand
		{
			get { return _callHomeCommand; }
			set
			{
				Set(() => CallHomeCommand, ref _callHomeCommand, value);
			}
		}

		private ICommand _callMobileCommand;
		public ICommand CallMobileCommand
		{
			get { return _callMobileCommand; }
			set
			{
				Set(() => SendSMSCommand, ref _callMobileCommand, value);
			}
		}

		private ICommand _sendSMSCommand;
		public ICommand SendSMSCommand
		{
			get { return _sendSMSCommand; }
			set
			{
				Set(() => SendSMSCommand, ref _sendSMSCommand, value);
			}
		}

		private ICommand _callWorkCommand;
		public ICommand CallWorkCommand
		{
			get { return _callWorkCommand; }
			set
			{
				Set(() => CallWorkCommand, ref _callWorkCommand, value);
			}
		}

		private ICommand _sendEmailCommand;
		public ICommand SendEmailCommand
		{
			get { return _sendEmailCommand; }
			set
			{
				Set(() => SendEmailCommand, ref _sendEmailCommand, value);
			}
		}
	}
}
