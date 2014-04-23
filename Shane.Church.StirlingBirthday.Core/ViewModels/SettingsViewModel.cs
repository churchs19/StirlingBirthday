using GalaSoft.MvvmLight;
using Shane.Church.StirlingBirthday.Core.Exceptions;
using Shane.Church.StirlingBirthday.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Shane.Church.StirlingBirthday.Core.ViewModels
{
	public class SettingsViewModel : ObservableObject
	{
		private IAgentManagementService _service;

		public SettingsViewModel(IAgentManagementService service)
		{
			if (service == null)
				throw new ArgumentNullException("service");
			_service = service;
		}

		public bool IsAgentToggleEnabled
		{
			get
			{
				return _service.AreAgentsSupported;
			}
		}

		public bool IsAgentEnabled
		{
			get
			{
				return _service.IsAgentEnabled;
			}
		}

		private ICommand _changeAgentEnabledCommand;
		public ICommand ChangeAgentEnabledCommand
		{
			get { return _changeAgentEnabledCommand; }
			set
			{
				Set(() => ChangeAgentEnabledCommand, ref _changeAgentEnabledCommand, value);
			}
		}

		private Action<AgentManagementException> _handleChangeAgentError;
		public Action<AgentManagementException> HandleChangeAgentError
		{
			get { return _handleChangeAgentError; }
			set
			{
				Set(() => HandleChangeAgentError, ref _handleChangeAgentError, value);
			}
		}

		public void ChangeAgentEnabled()
		{
			try
			{
				if (IsAgentEnabled)
					_service.RemoveAgent();
				else
					_service.StartAgent();
			}
			catch (AgentManagementException ame)
			{
				if (HandleChangeAgentError != null)
					HandleChangeAgentError.Invoke(ame);
			}
		}
	}
}
