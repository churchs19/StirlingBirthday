using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Ninject;
using Ninject.Parameters;
using Shane.Church.StirlingBirthday.Core.Data;
using Shane.Church.StirlingBirthday.Core.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Shane.Church.StirlingBirthday.Core.ViewModels
{
    public abstract class MainViewModel : ObservableObject
    {
        protected IBirthdaySource _source;
        protected ISettingsService _settings;
        protected INavigationService _navigation;
        private List<BirthdayContact> _allBirthdays;
        private ObservableCollection<ContactViewModel> _allContacts;
        private ObservableCollection<ContactViewModel> _upcomingContacts;
        private ObservableCollection<ContactViewModel> _pastContacts;

        public MainViewModel(IBirthdaySource source, ISettingsService settings, INavigationService navigation)
        {
            if (source == null)
                throw new ArgumentNullException("repository");
            _source = source;
            if (settings == null)
                throw new ArgumentNullException("settings");
            _settings = settings;
            if (navigation == null)
                throw new ArgumentNullException("navigation");
            _navigation = navigation;

            _allBirthdays = new List<BirthdayContact>();
            _allContacts = new ObservableCollection<ContactViewModel>();
            _allContacts.CollectionChanged += (s, e) =>
            {
                RaisePropertyChanged(() => AllContacts);
            };
            _upcomingContacts = new ObservableCollection<ContactViewModel>();
            _upcomingContacts.CollectionChanged += (s, e) =>
            {
                RaisePropertyChanged(() => UpcomingContacts);
            };
            _pastContacts = new ObservableCollection<ContactViewModel>();
            _pastContacts.CollectionChanged += (s, e) =>
            {
                RaisePropertyChanged(() => PastContacts);
            };

            _monthNames = new List<MonthGroup>();
            for (int i = 0; i < 12; i++)
            {
                _monthNames.Add(new MonthGroup(new DateTime(DateTime.Today.Year, i + 1, 1, 0, 0, 0, DateTimeKind.Utc)));
            }

            _contactGroups = new List<MonthGroup>();
        }

        private List<MonthGroup> _monthNames;
        public List<MonthGroup> MonthNames
        {
            get { return _monthNames; }
        }

        private List<MonthGroup> _contactGroups;
        public List<MonthGroup> ContactGroups
        {
            get { return _contactGroups; }
        }

        private int _totalUpcomingCount;
        public int TotalUpcomingCount
        {
            get { return _totalUpcomingCount; }
            set
            {
                Set(() => TotalUpcomingCount, ref _totalUpcomingCount, value);
            }
        }

        private int _totalCount;
        public int TotalCount
        {
            get { return _totalUpcomingCount; }
            set
            {
                Set(() => TotalCount, ref _totalCount, value);
            }
        }

        private int _totalPastCount;
        public int TotalPastCount
        {
            get { return _totalPastCount; }
            set
            {
                Set(() => TotalPastCount, ref _totalPastCount, value);
            }
        }

        public ObservableCollection<ContactViewModel> UpcomingContacts
        {
            get
            {
                return _upcomingContacts;
            }
        }

        public ObservableCollection<ContactViewModel> AllContacts
        {
            get
            {
                return _allContacts;
            }
        }

        public ObservableCollection<ContactViewModel> PastContacts
        {
            get
            {
                return _pastContacts;
            }
        }

        private ICommand _aboutCommand;
        public ICommand AboutCommand
        {
            get { return _aboutCommand; }
            set
            {
                Set(() => AboutCommand, ref _aboutCommand, value);
            }
        }

        private ICommand _settingsCommand;
        public ICommand SettingsCommand
        {
            get { return _settingsCommand; }
            set
            {
                Set(() => SettingsCommand, ref _settingsCommand, value);
            }
        }

        private ICommand _rateCommand;
        public ICommand RateCommand
        {
            get { return _rateCommand; }
            set
            {
                Set(() => RateCommand, ref _rateCommand, value);
            }
        }

        private ICommand _pinCommand;
        public ICommand PinCommand
        {
            get { return _pinCommand; }
            set
            {
                Set(() => PinCommand, ref _pinCommand, value);
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                Set(() => IsLoading, ref _isLoading, value);
            }
        }

        public void LoadNextUpcomingContacts(int count = 10)
        {
            IsLoading = false;
            var upcoming = _allBirthdays.Where(it => it.DaysUntil <= 30).OrderBy(it => it.DaysUntil).ThenBy(it => it.Age).Skip(_upcomingContacts.Count).Take(count);
            foreach (var u in upcoming)
            {
                ConstructorArgument arg = new ConstructorArgument("contact", u);
                var contactModel = KernelService.Kernel.Get<ContactViewModel>(arg);
                _upcomingContacts.Add(contactModel);
            }
            IsLoading = false;
        }

        public void LoadNextPastContacts(int count = 10)
        {
            IsLoading = true;
            var past = _allBirthdays.Where(it => it.DaysSince <= 30 && it.DaysSince > 0).OrderBy(it => it.DaysSince).ThenBy(it => it.Age).Skip(_pastContacts.Count).Take(count);
            foreach (var u in past)
            {
                ConstructorArgument arg = new ConstructorArgument("contact", u);
                var contactModel = KernelService.Kernel.Get<ContactViewModel>(arg);
                _pastContacts.Add(contactModel);
            }
            IsLoading = false;
        }

        public void LoadNextContacts(int count = 10)
        {
            IsLoading = true;
            var currentMonth = DateTime.Today.Month;
            var currentDay = DateTime.Today.Day;
            var all = _allBirthdays.OrderBy(it => it.Date.Month == currentMonth ? (it.Date.Day < currentDay ? it.DaysUntil - 365 : it.DaysUntil) : it.DaysUntil).ThenBy(it => it.Age).Skip(_allContacts.Count).Take(count);
            foreach (var u in all)
            {
                ConstructorArgument arg = new ConstructorArgument("contact", u);
                var contactModel = KernelService.Kernel.Get<ContactViewModel>(arg);
                _allContacts.Add(contactModel);
            }
            IsLoading = false;
        }

        public async Task LoadData(bool forceRefresh = false)
        {
            if (forceRefresh || _allBirthdays.Count == 0)
            {
                IsLoading = true;
                _allBirthdays.Clear();
                IQueryable<BirthdayContact> contacts = await _source.GetAllEntriesAsync(forceRefresh);
                _allBirthdays.AddRange(contacts.OrderBy(it => it.DaysUntil));
                TotalUpcomingCount = _allBirthdays.Where(it => it.DaysUntil <= 30).Count();
                TotalCount = _allBirthdays.Count();
                TotalPastCount = _allBirthdays.Where(it => it.DaysSince <= 30 && it.DaysSince > 0).Count();

                _contactGroups.AddRange(_allBirthdays.Select(it => new MonthGroup(it.Date)).Distinct().OrderBy(it => it.MonthIndex));
                IsLoading = false;
            }
        }
    }
}
