using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
using Shane.Church.StirlingBirthday.ViewModels;
using Shane.Church.Utility;

namespace Shane.Church.StirlingBirthday.Controls
{
	public partial class BirthdayTileFront : UserControl
	{
		public BirthdayTileFront(BirthdayTileFrontViewModel model)
		{
			InitializeComponent();
			this.DataContext = model;
		}

		public void ToTile(string Path)
		{
			// Need to call these, otherwise the contents aren't rendered correctly.
			this.Measure(new Size(173, 173));
			this.Arrange(new Rect(0, 0, 173, 173));

			WriteableBitmap bitmap = new WriteableBitmap(this, new TranslateTransform());

			Imaging.SaveImage(bitmap, Path, Imaging.ImageType.Png);
		}

	}
}
