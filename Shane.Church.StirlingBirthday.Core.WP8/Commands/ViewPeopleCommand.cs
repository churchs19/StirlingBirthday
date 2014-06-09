using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Shane.Church.StirlingBirthday.Core.WP.Commands
{
    public class ViewPeopleCommand : ICommand
    {
        public ViewPeopleCommand()
        {
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

#pragma warning disable 0067 
        public event EventHandler CanExecuteChanged;
#pragma warning restore 0067

        public void Execute(object parameter)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri("app://5B04B775-356B-4AA0-AAF8-6491FFEA5615/Default", UriKind.Absolute);
            task.Show();
            //var success = await Windows.System.Launcher.LaunchUriAsync(new Uri("app://5B04B775-356B-4AA0-AAF8-6491FFEA5615/Default", UriKind.Absolute));
            //if(success)
            //{

            //}
            //else
            //{

            //}
        }
    }
}
