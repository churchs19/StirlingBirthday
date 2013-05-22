using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using NLog;

namespace Shane.Church.StirlingBirthday
{
	public partial class Settings : PhoneApplicationPage
	{
		private static Logger _logger = LogManager.GetCurrentClassLogger();

		public Settings()
		{
			InitializeComponent();
			toggleSwitchAgent.DataContext = AgentManagement.IsAgentEnabled;
			checkBoxRestart.DataContext = AgentManagement.AutoRestartAgent;
		}

		private void ToggleSwitch_Checked(object sender, RoutedEventArgs e)
		{
			try
			{
				AgentManagement.StartPeriodicAgent();
			}
			catch (AgentManagementException aex)
			{
				_logger.ErrorException("Error starting periodic agent", aex);
				MessageBox.Show(aex.Message);
			}
		}

		private void ToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
		{
			AgentManagement.RemoveAgent();
		}

		private void checkBoxRestart_Unchecked(object sender, RoutedEventArgs e)
		{
			AgentManagement.AutoRestartAgent = false;
		}

		private void checkBoxRestart_Checked(object sender, RoutedEventArgs e)
		{
			AgentManagement.AutoRestartAgent = true;
		}
	}
}