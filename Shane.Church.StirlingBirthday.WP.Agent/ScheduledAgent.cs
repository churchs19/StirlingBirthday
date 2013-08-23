using Microsoft.Phone.Scheduler;
using Shane.Church.StirlingBirthday.Core.WP;
using Shane.Church.StirlingBirthday.Core.WP.Data;
using Shane.Church.StirlingBirthday.Core.WP.Services;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace Shane.Church.StirlingBirthday.WP.Agent
{
	public class ScheduledAgent : ScheduledTaskAgent
	{
		private static volatile bool _classInitialized;
		private PhoneBirthdaySource source;
		private PhoneTileUpdateService service;

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
#if DEBUG
					Application.Current.Exit += Current_Exit;
#endif
				});
			}
		}

#if DEBUG
		void Current_Exit(object sender, System.EventArgs e)
		{
			DebugUtility.DebugOutputMemoryUsage("Agent Exit");
		}
#endif
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
			source = new PhoneBirthdaySource();
			source.GetEntriesCompleted += source_GetEntriesCompleted;
			source.BeginGetAllEntries();
		}

		void source_GetEntriesCompleted(object sender, GetEntriesCompletedEventArgs args)
		{
#if DEBUG
			DebugUtility.DebugOutputMemoryUsage("Beginning Entries Completed Handler");
#endif
			source = null;
			System.GC.Collect();
			System.GC.WaitForPendingFinalizers();
			System.GC.Collect();
			service = new PhoneTileUpdateService();

			var tileContacts = args.Contacts.OrderBy(it => it.DaysUntil).Take(3).ToList();
			var count = args.Contacts.Where(it => it.DaysUntil == 0).Count();

			args.Contacts = null;
			System.GC.Collect();
			System.GC.WaitForPendingFinalizers();
			System.GC.Collect();

			service.UpdateTileSynchronous(tileContacts, count, UpdateTileCompleted);
		}

		void UpdateTileCompleted()
		{
			service = null;
			System.GC.Collect();
			System.GC.WaitForPendingFinalizers();
			System.GC.Collect();
#if DEBUG
			DebugUtility.DebugOutputElapsedTime("Scheduled Task Final Time Snapshot:");
			DebugUtility.DebugOutputMemoryUsage("Scheduled Task Final Memory Snapshot");
#endif

#if(DEBUG_AGENT)
			//			ScheduledActionService.LaunchForTest("StirlingBirthdayTileUpdateTask", TimeSpan.FromSeconds(60));
#endif

			NotifyComplete();
		}
	}
}