using Shane.Church.StirlingBirthday.Core.Data;
using Shane.Church.StirlingBirthday.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shane.Church.StirlingBirthday.Core.ViewModels
{
	public abstract class MainViewModel
	{
		protected IRepository<IContact> _repository;
		protected ISettingsService _settings;

		public MainViewModel(IRepository<IContact> repository, ISettingsService settings)
		{
			if (repository == null)
				throw new ArgumentNullException("repository");
			_repository = repository;
			if (settings == null)
				throw new ArgumentNullException("settings");
			_settings = settings;

		}


	}
}
