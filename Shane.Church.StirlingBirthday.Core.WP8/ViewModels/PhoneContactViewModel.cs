using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Tasks;
using Shane.Church.StirlingBirthday.Core.Data;
using Shane.Church.StirlingBirthday.Core.ViewModels;
using System;

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

		public override string DaysUntilText
		{
			get
			{
				if (DaysUntil == 0) return Core.Properties.Resources.TodayText;

				return (Math.Abs(DaysUntil) == 1) ? String.Format(Properties.Resources.DaysTextSingular, DaysUntil) : String.Format(Properties.Resources.DaysTextPlural, DaysUntil);
			}
		}


		public void CallHome()
		{
			PhoneCallTask call = new PhoneCallTask();
			call.PhoneNumber = HomePhone;
			call.DisplayName = DisplayName;
			FlurryWP8SDK.Api.LogEvent("Call_Home");
			call.Show();
		}

		public void CallMobile()
		{
			PhoneCallTask call = new PhoneCallTask();
			call.PhoneNumber = MobilePhone;
			call.DisplayName = DisplayName;
			FlurryWP8SDK.Api.LogEvent("Call_Mobile");
			call.Show();
		}

		public void SendSMS()
		{
			SmsComposeTask text = new SmsComposeTask();
			text.To = MobilePhone;
			text.Body = Properties.Resources.HappyBirthdayText;
			FlurryWP8SDK.Api.LogEvent("Send_SMS");
			text.Show();
		}

		public void CallWork()
		{
			PhoneCallTask call = new PhoneCallTask();
			call.PhoneNumber = WorkPhone;
			call.DisplayName = DisplayName;
			FlurryWP8SDK.Api.LogEvent("Call_Work");
			call.Show();
		}

		public void SendEmail()
		{
			EmailComposeTask email = new EmailComposeTask();
			email.To = Email;
			email.Subject = Properties.Resources.HappyBirthdayText;
			email.Body = Properties.Resources.EmailBodyText;
			FlurryWP8SDK.Api.LogEvent("Send_Email");
			email.Show();
		}

		public void Share()
		{
			ShareStatusTask shareStatusTask = new ShareStatusTask();
			shareStatusTask.Status = String.Format(Properties.Resources.HappyBirthdayShare, this.DisplayName, this.FirstName);
			FlurryWP8SDK.Api.LogEvent("Status_Share");
			shareStatusTask.Show();
		}
	}
}
