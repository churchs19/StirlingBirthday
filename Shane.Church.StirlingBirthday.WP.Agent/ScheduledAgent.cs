using Microsoft.Phone.Scheduler;
using Shane.Church.StirlingBirthday.Core.WP;
using Shane.Church.StirlingBirthday.Core.WP.Data;
using Shane.Church.StirlingBirthday.Core.WP.Services;
using System;
using System.Diagnostics;
using System.Windows;

namespace Shane.Church.StirlingBirthday.WP.Agent
{
	public class ScheduledAgent : ScheduledTaskAgent
	{
		private static volatile bool _classInitialized;

		/// <remarks>
		/// ScheduledAgent constructor, initializes the UnhandledException handler
		/// </remarks>
		public ScheduledAgent()
		{
			if (!_classInitialized)
			{
				_classInitialized = true;

				// Subscribe to the managed exception handler
				Deployment.Current.Dispatcher.BeginInvoke(delegate
				{
					Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
				});
			}
		}

		/// Code to execute on Unhandled Exceptions
		private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
		{
			DebugUtility.SaveDiagnosticException(e.ExceptionObject);
			if (System.Diagnostics.Debugger.IsAttached)
			{
				// An unhandled exception has occurred; break into the debugger
				System.Diagnostics.Debugger.Break();
			}
		}

		/// <summary>
		/// Agent that runs a scheduled task
		/// </summary>
		/// <param name="task">
		/// The invoked task
		/// </param>
		/// <remarks>
		/// This method is called when a periodic or resource intensive task is invoked
		/// </remarks>
		protected override void OnInvoke(ScheduledTask task)
		{
#if DEBUG
			AgentExitReason reason = task.LastExitReason;
			DebugUtility.SaveDiagnostics(new Diagnostics() { DeviceUniqueId = DebugUtility.GetDeviceUniqueID(), AgentExitReason = reason.ToString() });
			Debug.WriteLine("Agent Last Exited for Reason: " + reason.ToString());
			DebugUtility.DebugStartStopwatch();
			DebugUtility.DebugOutputMemoryUsage("Scheduled Task Initial Memory Snapshot");
#endif

			var source = new PhoneBirthdaySource();
			source.GetEntriesCompleted += source_GetEntriesCompleted;
			source.BeginGetAllEntries();
		}

		void source_GetEntriesCompleted(object sender, GetEntriesCompletedEventArgs args)
		{
			var service = new PhoneTileUpdateService();

			service.UpdateTileSynchronous(args.Contacts, UpdateTileCompleted);
		}

		void UpdateTileCompleted()
		{
#if DEBUG
			DebugUtility.DebugOutputElapsedTime("Scheduled Task Final Time Snapshot:");
			DebugUtility.DebugOutputMemoryUsage("Scheduled Task Final Memory Snapshot");
#endif

#if(DEBUG_AGENT)
			ScheduledActionService.LaunchForTest("StirlingBirthdayTileUpdateTask", TimeSpan.FromSeconds(60));
#endif

			NotifyComplete();
		}
	}
}