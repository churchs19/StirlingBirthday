using GalaSoft.MvvmLight.Command;
using Shane.Church.StirlingBirthday.Core.Data;
using Shane.Church.StirlingBirthday.Core.Services;
using Shane.Church.StirlingBirthday.Core.ViewModels;
using Shane.Church.StirlingBirthday.Core.WP.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Shane.Church.StirlingBirthday.Core.WP.ViewModels
{
    public class PhoneMainViewModel : MainViewModel
    {
        public PhoneMainViewModel(IBirthdaySource source, ISettingsService settings, INavigationService navigation)
            : base(source, settings, navigation)
        {
            AboutCommand = new RelayCommand(About);
            SettingsCommand = new RelayCommand(Settings);
            RateCommand = new RateThisAppCommand();
            PinCommand = new RelayCommand(PinToStart);
            var addContactCmd = new AddContactCommand();
            addContactCmd.ContactAdded += addContactCmd_ContactAdded;
            AddContactCommand = addContactCmd;
        }

        async void addContactCmd_ContactAdded(object sender, EventArgs e)
        {
            await LoadData(true);
        }

        public void About()
        {
            _navigation.NavigateTo(new Uri("/About.xaml", UriKind.Relative));
        }

        public void Settings()
        {
            _navigation.NavigateTo(new Uri("/Settings.xaml", UriKind.Relative));
        }

        public void PinToStart()
        {

        }
    }
}
