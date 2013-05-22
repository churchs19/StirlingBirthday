using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Shell;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Shane.Church.Utility;
using NLog;
using Shane.Church.StirlingBirthday.ViewModels;

namespace Shane.Church.StirlingBirthday.Controls
{
	public class BirthdayTile
	{
		public static string Path = "/Shared/ShellContent/BirthdayFront.png";
		public static string BackPath = "/Shared/ShellContent/BirthdayBack.png";
		public static string ScheduledTaskName = "StirlingBirthdayUpdater";
		private static Logger _logger = LogManager.GetCurrentClassLogger();

		public BirthdayTile()
		{

		}

		public delegate void UpdateTileComplete(object sender, EventArgs e);
		public event UpdateTileComplete UpdateTileCompleted;

		public void UpdateTile(IEnumerable<ContactViewModel> model)
		{
			var bti = EventAsync.FromEvent<EventArgs>(this, "BuildTileImageCompleted");
			var bbti = EventAsync.FromEvent<EventArgs>(this, "BuildBackTileImageCompleted");
			var utd = EventAsync.FromEvent<EventArgs>(this, "UpdateTileDataCompleted");
			bti.ContinueWith(frontTile =>
			{
				bbti.ContinueWith(backTile =>
				{
					utd.ContinueWith(updateTile =>
					{
						if (UpdateTileCompleted != null)
						{
							UpdateTileCompleted(this, new EventArgs());
						}
					});
					this.UpdateTileData();
				});
				this.BuildBackTileImage(model);
			});
			this.BuildTileImage(model.FirstOrDefault());
		}

		public delegate void BuildTileImageComplete(object sender, EventArgs e);
		public event BuildTileImageComplete BuildTileImageCompleted; 

		private void BuildTileImage(ContactViewModel model)
		{
			Deployment.Current.Dispatcher.BeginInvoke(() =>
			{
				_logger.Trace("BuildTileImage");
				BirthdayTileFrontViewModel viewModel = new BirthdayTileFrontViewModel();
				if (model != null && model.PictureUri != null)
				{
					viewModel.Image = Imaging.LoadImage(model.IsoStorePath);
				    viewModel.Overlay = new BitmapImage(new Uri(@"/Images/BirthdayOverlay.png", UriKind.Relative));
				}
				else
				{
					viewModel.Image = new BitmapImage(new Uri(@"/Background.png", UriKind.Relative));
				}
				BirthdayTileFront front = new BirthdayTileFront(viewModel);
				front.ToTile(Path);
				if (BuildTileImageCompleted != null)
				{
					BuildTileImageCompleted(this, new EventArgs());
				}
			});
		}

		public delegate void BuildBackTileImageComplete(object sender, EventArgs e);
		public event BuildBackTileImageComplete BuildBackTileImageCompleted; 

		private void BuildBackTileImage(IEnumerable<ContactViewModel> model)
		{
			Deployment.Current.Dispatcher.BeginInvoke(() =>
			{
				_logger.Trace("BuildBackTileImage");
				BirthdayTileBackViewModel viewModel = new BirthdayTileBackViewModel();
				if (model.Count() > 0)
				{
					viewModel.Name1 = model.ElementAt(0).DisplayName;
					viewModel.Date1 = model.ElementAt(0).StartTileDateText;
				}
				if (model.Count() > 1)
				{
					viewModel.Name2 = model.ElementAt(1).DisplayName;
					viewModel.Date2 = model.ElementAt(1).StartTileDateText;
				}
				if (model.Count() > 2)
				{
					viewModel.Name3 = model.ElementAt(2).DisplayName;
					viewModel.Date3 = model.ElementAt(2).StartTileDateText;
				}
				BirthdayTileBack back = new BirthdayTileBack(viewModel);
				back.ToTile(BackPath);
				if (BuildBackTileImageCompleted != null)
				{
					BuildBackTileImageCompleted(this, new EventArgs());
				}
			});
		}

		public delegate void UpdateTileDataComplete(object sender, EventArgs e);
		public event UpdateTileDataComplete UpdateTileDataCompleted; 

		private void UpdateTileData()
		{
			Deployment.Current.Dispatcher.BeginInvoke(() =>
			{
				ShellTile mainTile = ShellTile.ActiveTiles.First();

				if (mainTile != null)
				{
					StandardTileData data = new StandardTileData()
					{
						BackgroundImage = new Uri("isostore:" + BirthdayTile.Path, UriKind.Absolute),
						Title = "Stirling Birthday",
						Count = 0,
						BackTitle = "Stirling Birthday",
						BackBackgroundImage = new Uri("isostore:" + BirthdayTile.BackPath, UriKind.Absolute)
					};
					mainTile.Update(data);
				}

				if (UpdateTileDataCompleted != null)
				{
					UpdateTileDataCompleted(this, new EventArgs());
				}
			});
		}
	}
}
