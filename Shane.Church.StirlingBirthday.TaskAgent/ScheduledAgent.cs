using System.Windows;
using System.Linq;
using Microsoft.Phone.Scheduler;
using Shane.Church.StirlingBirthday.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Phone.Info;
using System.Threading.Tasks;
using Shane.Church.Utility;
using Microsoft.Phone.Shell;
using NLog;
using Shane.Church.StirlingBirthday.Controls;

namespace Shane.Church.StirlingBirthday.TaskAgent
{
	public class ScheduledAgent : ScheduledTaskAgent
	{
		private static volatile bool _classInitialized;
		private static volatile MainViewModel _model;
		private static Logger _logger = LogManager.GetCurrentClassLogger();

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
			_logger.ErrorException("Scheduled Agent Unhandled Exception", e.ExceptionObject);
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
			_logger.Debug("Launching Scheduled Task");
			AgentExitReason reason = task.LastExitReason;
			_logger.Debug("Agent Last Exited for Reason: " + reason.ToString());
#if DEBUG
            DebugUtility.DebugStartStopwatch();
            DebugUtility.DebugOutputMemoryUsage("Scheduled Task Initial Memory Snapshot");
#endif
			Deployment.Current.Dispatcher.BeginInvoke(() =>
			{
				try
				{
					if (_model == null)
					{
						_model = new MainViewModel();
					}
					if (!_model.LastUpdated.Date.Equals(DateTime.Today))
					{
						BirthdayTile tile = new BirthdayTile();
						tile.UpdateTileCompleted += new BirthdayTile.UpdateTileComplete(tile_UpdateTileCompleted);

						_model.Begin_LoadData(true).ContinueWith(t =>
						{
							if (t.IsCompleted)
							{
								tile.UpdateTile(_model.Upcoming.Take(3));
							}
						});
					}
					else
					{
						_logger.Debug("Completed Scheduled Task - No Update Needed");
						NotifyComplete();
					}
				}
				catch (Exception ex)
				{
					_logger.ErrorException("Scheduled Task Error", ex);
				}
			});
		}

		void tile_UpdateTileCompleted(object sender, EventArgs e)
		{
#if DEBUG
			DebugUtility.DebugOutputElapsedTime("Scheduled Task Final Time Snapshot:");
			DebugUtility.DebugOutputMemoryUsage("Scheduled Task Final Memory Snapshot");
#endif
			Cleaner.CleanTempFiles();
			_logger.Debug("Completed Scheduled Task");
			NotifyComplete();
		}
	}
}