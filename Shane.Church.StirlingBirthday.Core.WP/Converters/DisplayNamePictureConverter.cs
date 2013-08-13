using Ninject;
using Shane.Church.StirlingBirthday.Core.Data;
using Shane.Church.StirlingBirthday.Core.Services;
using Shane.Church.Utility.Core.WP;
using System;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Shane.Church.StirlingBirthday.Core.WP.Converters
{
    public class DisplayNamePictureConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                var source = KernelService.Kernel.Get<IBirthdaySource>();
                var picture = AsyncHelpers.RunSync<byte[]>(() => source.GetContactPicture(value.ToString()));
                if (picture != null)
                {
                    MemoryStream s = new MemoryStream(picture, true);

                    BitmapImage imgSource = new BitmapImage();
                    imgSource.SetSource(s);

                    return imgSource;
                }
            }
            catch { }
            return new BitmapImage();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
