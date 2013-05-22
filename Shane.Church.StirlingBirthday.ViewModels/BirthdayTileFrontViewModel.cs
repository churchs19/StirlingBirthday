using System;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace Shane.Church.StirlingBirthday.ViewModels
{
	public class BirthdayTileFrontViewModel : INotifyPropertyChanged
	{

		private BitmapSource _image;
		public BitmapSource Image
		{
			get { return _image; }
			set
			{
				if (_image != value)
				{
					_image = value;
					NotifyPropertyChanged("Image");
				}
			}
		}

		private BitmapSource _overlay;
		public BitmapSource Overlay
		{
			get { return _overlay; }
			set
			{
				if (_overlay != value)
				{
					_overlay = value;
					NotifyPropertyChanged("Overlay");
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(String propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (null != handler)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
