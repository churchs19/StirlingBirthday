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


		public void CallHome()
		{
			PhoneCallTask call = new PhoneCallTask();
			call.PhoneNumber = HomePhone;
			call.DisplayName = DisplayName;
#if WP8
			FlurryWP8SDK.Api.LogEvent("Call_Home");
#endif
			call.Show();
		}

		public void CallMobile()
		{
			PhoneCallTask call = new PhoneCallTask();
			call.PhoneNumber = MobilePhone;
			call.DisplayName = DisplayName;
#if WP8
			FlurryWP8SDK.Api.LogEvent("Call_Mobile");
#endif
			call.Show();
		}

		public void SendSMS()
		{
			SmsComposeTask text = new SmsComposeTask();
			text.To = MobilePhone;
			text.Body = Properties.Resources.HappyBirthdayText;
#if WP8
			FlurryWP8SDK.Api.LogEvent("Send_SMS");
#endif
			text.Show();
		}

		public void CallWork()
		{
			PhoneCallTask call = new PhoneCallTask();
			call.PhoneNumber = WorkPhone;
			call.DisplayName = DisplayName;
#if WP8
			FlurryWP8SDK.Api.LogEvent("Call_Work");
#endif
			call.Show();
		}

		public void SendEmail()
		{
			EmailComposeTask email = new EmailComposeTask();
			email.To = Email;
			email.Subject = Properties.Resources.HappyBirthdayText;
			email.Body = Properties.Resources.EmailBodyText;
#if WP8
			FlurryWP8SDK.Api.LogEvent("Send_Email");
#endif
			email.Show();
		}

		public void Share()
		{
			ShareStatusTask shareStatusTask = new ShareStatusTask();
			shareStatusTask.Status = String.Format(Properties.Resources.HappyBirthdayShare, this.DisplayName, this.FirstName);
#if WP8
			FlurryWP8SDK.Api.LogEvent("Status_Share");
#endif
			shareStatusTask.Show();
		}
	}
}
