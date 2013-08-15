using Shane.Church.StirlingBirthday.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Telerik.Windows.Controls;

namespace Shane.Church.StirlingBirthday.WP.Helpers
{
    public class BirthdayItemTemplateSelector : DataTemplateSelector
    {
        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            var visualContainer = container as RadDataBoundListBoxItem;
            if (visualContainer != null)
            {
                if (item is ContactViewModel && ((ContactViewModel)item).DaysUntil == 0)
                    return TodayTemplate;
            }
            return DefaultTemplate;
        }

        public DataTemplate TodayTemplate { get; set; }
        public DataTemplate DefaultTemplate { get; set; }
    }     
}
