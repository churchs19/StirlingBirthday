using Shane.Church.StirlingBirthday.Core.Data;
using Shane.Church.StirlingBirthday.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shane.Church.StirlingBirthday.Core.WP.ViewModels
{
    public class PhoneBirthdayTileFrontViewModel : BirthdayTileFrontViewModel
    {
        public PhoneBirthdayTileFrontViewModel(BirthdayContact contact, byte[] picture)
            : base(contact, picture)
        {

        }

        public override string AgeText
        {
            get
            {
                return (Math.Abs(Age) == 1) ? String.Format(Resources.WPCoreResources.YearsSingular, Age) : String.Format(Resources.WPCoreResources.YearsPlural, Age);
            }
        }
    }
}
