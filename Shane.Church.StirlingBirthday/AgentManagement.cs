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
using Microsoft.Phone.Scheduler;
using Shane.Church.StirlingBirthday.ViewModels;
using Microsoft.Phone.Info;
using System.IO.IsolatedStorage;
using Shane.Church.Utility;
using Shane.Church.StirlingBirthday.Controls;
using NLog;

namespace Shane.Church.StirlingBirthday
{
	public static class AgentManagement
	{
		private static Logger _logger = LogManager.GetCurrentClassLogger();

		private static AppSettings settings = new AppSettings();

		public static void StartPeriodicAgent()
		{
			// Obtain a reference to the period task, if one exists
			var periodicTask = ScheduledActionService.Find(BirthdayTile.ScheduledTaskName) as PeriodicTask;

			// If the task already exists and background agents are enabled for the
			// application, you must remove the task and then add it again to update 
			// the schedule
			if (periodicTask != null)
			{
				AgentExitReason reason = periodicTask.LastExitReason;
				_logger.Debug("Agent Last Exited for Reason: " + reason.ToString());
				RemoveAgent();
			}

			periodicTask = new PeriodicTask(BirthdayTile.ScheduledTaskName);

			// The description is required for periodic agents. This is the string that the user
			// will see in the background services Settings page on the device.
			periodicTask.Description = "This task updates the Stirling Birthday tile with upcoming birthday information.";
			periodicTask.ExpirationTime = DateTime.Now.AddDays(14);

			// Place the call to Add in a try block in case the user has disabled agents.
			try
			{
				ScheduledActionService.Add(periodicTask);
				// If debugging is enabled, use LaunchForTest to launch the agent in one minute.
#if(DEBUG_AGENT)
				ScheduledActionService.LaunchForTest(BirthdayTile.ScheduledTaskName, TimeSpan.FromSeconds(30));
#endif
			}
			catch (InvalidOperationException exception)
			{
				if (exception.Message.Contains("BNS Error: The action is disabled"))
				{
					throw new AgentManagementException("Background agents for this application have been disabled by the user.");
				}
				if (exception.Message.Contains("BNS Error: The maximum number of ScheduledActions of this type have already been added."))
				{
					if (IsLowMemDevice)
					{
						throw new AgentManagementException("This device does not support background agents, so this feature will be disabled.");
					}
					else
					{
						throw new AgentManagementException("Too many background agents have been enabled. Please go to the settings application to disable other agents before enabling Stirling Money to update.");
					}
				}
			}
		}

		public static void RemoveAgent()
		{
			try
			{
				ScheduledActionService.Remove(BirthdayTile.ScheduledTaskName);
			}
			catch { }
		}

		public static bool IsAgentEnabled
		{
			get
			{
				var periodicTask = ScheduledActionService.Find(BirthdayTile.ScheduledTaskName) as PeriodicTask;
				return periodicTask != null;
			}
		}

		private static bool IsLowMemDevice
		{
			get
			{
				try
				{
					// check the working set limit    
					var result = (Int64)DeviceExtendedProperties.GetValue("ApplicationWorkingSetLimit");
					return result < 94371840L;
				}
				catch (ArgumentOutOfRangeException)
				{
					// OS does not support this call => indicates a 512 MB device   
					return false;
				}
			}
		}

		public static bool AutoRestartAgent
		{
			get
			{
				return settings.AutoRestartBackgroundAgent;
			}
			set
			{
				settings.AutoRestartBackgroundAgent = value;
			}
		}
	}
}
