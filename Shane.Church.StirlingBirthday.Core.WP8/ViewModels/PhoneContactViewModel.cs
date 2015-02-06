using GalaSoft.MvvmLight.Command;
using Ninject;
using Microsoft.Phone.Tasks;
using Shane.Church.StirlingBirthday.Core.Data;
using Shane.Church.StirlingBirthday.Core.Services;
using Shane.Church.StirlingBirthday.Core.ViewModels;
using Shane.Church.StirlingBirthday.Core.WP.Commands;
using Shane.Church.StirlingBirthday.Strings;
using Shane.Church.Utility.Core.Command;
using System;

namespace Shane.Church.StirlingBirthday.Core.WP.ViewModels
{
	public class PhoneContactViewModel : ContactViewModel
	{
		private ILoggingService _log;

		public PhoneContactViewModel()
			: base()
		{
			_log = KernelService.Kernel.Get<ILoggingService>();
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
			_log = KernelService.Kernel.Get<ILoggingService>();
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
				return (Math.Abs(Age) == 1) ? String.Format(Resources.YearsSingular, Age) : String.Format(Resources.YearsPlural, Age);
			}
		}

		public override string DaysUntilText
		{
			get
			{
				if (DaysUntil == 0) return Resources.TodayText;

				return (Math.Abs(DaysUntil) == 1) ? String.Format(Resources.DaysTextSingular, DaysUntil) : String.Format(Resources.DaysTextPlural, DaysUntil);
			}
		}


		public void CallHome()
		{
			PhoneCallTask call = new PhoneCallTask();
			call.PhoneNumber = HomePhone;
			call.DisplayName = DisplayName;
			_log.LogMessage("Call_Home");
			call.Show();
		}

		public void CallMobile()
		{
			PhoneCallTask call = new PhoneCallTask();
			call.PhoneNumber = MobilePhone;
			call.DisplayName = DisplayName;
			_log.LogMessage("Call_Mobile");
			call.Show();
		}

		public void SendSMS()
		{
			SmsComposeTask text = new SmsComposeTask();
			text.To = MobilePhone;
			text.Body = Resources.HappyBirthdayText;
			_log.LogMessage("Send_SMS");
			text.Show();
		}

		public void CallWork()
		{
			PhoneCallTask call = new PhoneCallTask();
			call.PhoneNumber = WorkPhone;
			call.DisplayName = DisplayName;
			_log.LogMessage("Call_Work");
			call.Show();
		}

		public void SendEmail()
		{
			EmailComposeTask email = new EmailComposeTask();
			email.To = Email;
			email.Subject = Resources.HappyBirthdayText;
			email.Body = Resources.EmailBodyText;
			_log.LogMessage("Send_Email");
			email.Show();
		}

		public void Share()
		{
			ShareStatusTask shareStatusTask = new ShareStatusTask();
			shareStatusTask.Status = String.Format(Resources.HappyBirthdayShare, this.DisplayName, this.FirstName);
			_log.LogMessage("Status_Share");
			shareStatusTask.Show();
		}
	}
}
