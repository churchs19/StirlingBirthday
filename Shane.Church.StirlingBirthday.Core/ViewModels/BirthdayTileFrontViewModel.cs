using GalaSoft.MvvmLight;
using System;
using System.ComponentModel;

namespace Shane.Church.StirlingBirthday.ViewModels
{
	public class BirthdayTileFrontViewModel : ObservableObject
	{
		private Uri _imageUri;
		public Uri ImageUri
		{
			get { return _imageUri; }
			set
			{
				Set(() => ImageUri, ref _imageUri, value);
			}
		}

		private Uri _overlayUri;
		public Uri OverlayUri
		{
			get { return _overlayUri; }
			set
			{
				Set(() => OverlayUri, ref _overlayUri, value);
			}
		}
	}
}
