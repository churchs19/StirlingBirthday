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
			ShareCommand = new RelayCommand(Share);
		}

		public PhoneContactViewModel(BirthdayContact contact)
			: base(contact)
		{
			CallHomeCommand = new RelayCommand(CallHome);
			CallMobileCommand = new RelayCommand(CallMobile);
			CallWorkCommand = new RelayCommand(CallWork);
			SendSMSCommand = new RelayCommand(SendSMS);
			SendEmailCommand = new RelayCommand(SendEmail);
			ShareCommand = new RelayCommand(Share);
		}

		public override string AgeText
		{
			get
			{
				return (Math.Abs(Age) == 1) ? String.Format(Properties.Resources.YearsSingular, Age) : String.Format(Properties.Resources.YearsPlural, Age);
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
			text.Body = Properties.Resources.HappyBirthdayText;
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
			email.Subject = Properties.Resources.HappyBirthdayText;
			email.Body = Properties.Resources.EmailBodyText;
			email.Show();
		}

		public void Share()
		{
			ShareStatusTask shareStatusTask = new ShareStatusTask();
			shareStatusTask.Status = String.Format(Properties.Resources.HappyBirthdayShare, this.DisplayName, this.FirstName);
			shareStatusTask.Show();
		}
	}
}
