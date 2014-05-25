using Shane.Church.StirlingBirthday.Core.ViewModels;
using Shane.Church.StirlingBirthday.Core.WP.Commands;
using System.Reflection;

namespace Shane.Church.StirlingBirthday.Core.WP.ViewModels
{
    public class PhoneAboutViewModel : AboutViewModel
    {
        public override void Initialize()
        {
            RateThisAppCommand = new RateThisAppCommand();
            SendAnEmailCommand = new SendAnEmailCommand();
            OtherAppsCommand = new OtherAppsCommand();

            var versionAttrib = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
            Version = versionAttrib.Version.ToString();
        }
    }
}
