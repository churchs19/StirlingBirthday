using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shane.Church.StirlingBirthday.Strings;

namespace Shane.Church.StirlingBirthday.Strings
{
    public class LocalizedStrings
    {
        public LocalizedStrings()
        {
        }

        private static Resources localizedResources = new Resources();

        public Resources LocalizedResources { get { return localizedResources; } }
    }
}
