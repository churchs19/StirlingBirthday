using Microsoft.Phone.Scheduler;
using Ninject;
using Shane.Church.StirlingBirthday.Core.Data;
using Shane.Church.StirlingBirthday.Core.Services;
using Shane.Church.StirlingBirthday.Core.ViewModels;
using Shane.Church.StirlingBirthday.Core.WP;
using Shane.Church.StirlingBirthday.Core.WP.Data;
using Shane.Church.StirlingBirthday.Core.WP.Services;
using Shane.Church.StirlingBirthday.Core.WP.ViewModels;
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

                KernelService.Kernel = new StandardKernel();
                KernelService.Kernel.Bind<INavigationService>().To<PhoneNavigationService>().InSingletonScope();
                KernelService.Kernel.Bind<ISettingsService>().To<PhoneSettingsService>().InSingletonScope();
                KernelService.Kernel.Bind<IWebNavigationService>().To<PhoneWebNavigationService>().InSingletonScope();
                KernelService.Kernel.Bind<IBirthdaySource>().To<PhoneBirthdaySource>().InSingletonScope();
                KernelService.Kernel.Bind<MainViewModel>().To<PhoneMainViewModel>().InSingletonScope();
                KernelService.Kernel.Bind<ContactViewModel>().To<PhoneContactViewModel>();
                KernelService.Kernel.Bind<AboutViewModel>().To<PhoneAboutViewModel>();
                KernelService.Kernel.Bind<SettingsViewModel>().ToSelf();
                KernelService.Kernel.Bind<IAgentManagementService>().To<PhoneAgentManagementService>().InSingletonScope();
                KernelService.Kernel.Bind<ITileUpdateService>().To<PhoneTileUpdateService>().InSingletonScope();

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
            if (reason != AgentExitReason.Completed && reason != AgentExitReason.None)
            {
                DebugUtility.SaveDiagnostics(new Diagnostics() { DeviceUniqueId = DebugUtility.GetDeviceUniqueID(), AgentExitReason = reason.ToString() });
            }
            Debug.WriteLine("Agent Last Exited for Reason: " + reason.ToString());
            var settings = KernelService.Kernel.Get<ISettingsService>();
            settings.SaveSetting<AgentExitReason>(reason, "AgentLastExitReason");
            DebugUtility.DebugStartStopwatch();
            DebugUtility.DebugOutputMemoryUsage("Scheduled Task Initial Memory Snapshot");
#endif

            var service = KernelService.Kernel.Get<ITileUpdateService>();
#if DEBUG
            DebugUtility.DebugOutputMemoryUsage("Scheduled Task - Post Service Injection");
#endif

            service.UpdateTile().Wait();

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