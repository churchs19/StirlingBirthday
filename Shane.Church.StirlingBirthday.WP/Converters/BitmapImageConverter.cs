using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Shane.Church.StirlingBirthday.WP.Converters
{
	public class BitmapImageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType,
							  object parameter, CultureInfo culture)
		{
			string stringValue = value as string;
			if (stringValue != null)
			{
				return new BitmapImage(new Uri(stringValue, UriKind.RelativeOrAbsolute));
			}

			Uri uriValue = value as Uri;
			if (uriValue != null)
			{
				return new BitmapImage(uriValue);
			}

			throw new NotSupportedException();
		}

		public object ConvertBack(object value, Type targetType,
								  object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
