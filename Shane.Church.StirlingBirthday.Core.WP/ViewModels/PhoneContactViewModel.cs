using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Tasks;
using Shane.Church.StirlingBirthday.Core.Data;
using Shane.Church.StirlingBirthday.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shane.Church.StirlingBirthday.Core.WP.ViewModels
{
	public class PhoneContactViewModel : ContactViewModel
	{
		public PhoneContactViewModel()
			: base()
		{
			CallHomeCommand = new RelayCommand(CallHome);
			CallMobileCommand = new RelayCommand(CallMobile);
			CallWorkCommand = new RelayCommand(CallWork);
			SendSMSCommand = new RelayCommand(SendSMS);
			SendEmailCommand = new RelayCommand(SendEmail);
		}

		public PhoneContactViewModel(BirthdayContact contact)
			: base(contact)
		{
			CallHomeCommand = new RelayCommand(CallHome);
			CallMobileCommand = new RelayCommand(CallMobile);
			CallWorkCommand = new RelayCommand(CallWork);
			SendSMSCommand = new RelayCommand(SendSMS);
			SendEmailCommand = new RelayCommand(SendEmail);
		}

		public override string AgeText
		{
			get
			{
				return (Math.Abs(Age) == 1) ? String.Format(Resources.WPCoreResources.YearsSingular, Age) : String.Format(Resources.WPCoreResources.YearsPlural, Age);
			}
		}


		public void CallHome()
		{
			PhoneCallTask call = new PhoneCallTask();
			call.PhoneNumber = HomePhone;
			call.DisplayName = DisplayName;
			call.Show();
		}

		public void CallMobile()
		{
			PhoneCallTask call = new PhoneCallTask();
			call.PhoneNumber = MobilePhone;
			call.DisplayName = DisplayName;
			call.Show();
		}

		public void SendSMS()
		{
			SmsComposeTask text = new SmsComposeTask();
			text.To = MobilePhone;
			text.Body = Resources.WPCoreResources.HappyBirthdayText;
			text.Show();
		}

		public void CallWork()
		{
			PhoneCallTask call = new PhoneCallTask();
			call.PhoneNumber = WorkPhone;
			call.DisplayName = DisplayName;
			call.Show();
		}

		public void SendEmail()
		{
			EmailComposeTask email = new EmailComposeTask();
			email.To = Email;
			email.Subject = Resources.WPCoreResources.HappyBirthdayText;
			email.Body = Resources.WPCoreResources.EmailBodyText;
			email.Show();
		}
	}
}
