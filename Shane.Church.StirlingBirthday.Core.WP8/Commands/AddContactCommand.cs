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
    public class AddContactCommand : ICommand
    {
        SaveContactTask _task;

        public AddContactCommand()
        {
            _task = new SaveContactTask();
            _task.Completed += _task_Completed;
        }

        public event EventHandler ContactAdded;

        private bool _taskInProgress = false;
        protected bool InProgress
        {
            get { return _taskInProgress; }
            set
            {
                if (value != _taskInProgress)
                {
                    _taskInProgress = value;
                    if (CanExecuteChanged != null)
                    {
                        CanExecuteChanged(this, new EventArgs());
                    }
                }
            }
        }

        void _task_Completed(object sender, SaveContactResult e)
        {
            _taskInProgress = false;
            if(e.TaskResult == TaskResult.OK && ContactAdded != null)
            {
                ContactAdded(this, new EventArgs());
            }
            else if(e.TaskResult == TaskResult.None)
            {
                MessageBox.Show("");
            }
        }

        public bool CanExecute(object parameter)
        {
            return !InProgress;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _taskInProgress = true;
            _task.Show();
        }
    }
}
