#if !AGENT
using Ninject;
#else
using Shane.Church.StirlingBirthday.Core.WP.Data;
#endif
using Shane.Church.StirlingBirthday.Core.Data;
using Shane.Church.StirlingBirthday.Core.Services;
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
#if !AGENT
				var source = KernelService.Kernel.Get<IBirthdaySource>();
#else
				var source = new PhoneBirthdaySource();
#endif
				var picture = source.GetContactPicture(value.ToString());

				if (picture != null)
				{
					MemoryStream s = new MemoryStream(picture, true);

					BitmapImage imgSource = new BitmapImage();
					imgSource.SetSource(s);

					return imgSource;
				}
			}
			catch (Exception ex)
			{
				var msg = ex.Message;
			}
			return new BitmapImage();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

}
