using Microsoft.Phone.Shell;
using Shane.Church.StirlingBirthday.Core.Data;
using Shane.Church.StirlingBirthday.Core.Services;
using Shane.Church.StirlingBirthday.Core.ViewModels;
using Shane.Church.StirlingBirthday.Core.WP.Controls;
using Shane.Church.StirlingBirthday.Strings;
using Shane.Church.Utility.Core.WP.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Telerik.Windows.Controls;

namespace Shane.Church.StirlingBirthday.Core.WP.Services
{
	public class PhoneTileUpdateService : ITileUpdateService
	{
		public const int DaysToBuild = 16;
		public const string isoStorePath = "/shared/shellcontent/{0}.{1}.png";
		public const string isoStoreUri = "isostore:" + isoStorePath;

		public async Task UpdateTile(Action callback = null)
		{
			try
			{
				var source = new Shane.Church.StirlingBirthday.Core.WP.Data.PhoneBirthdaySource();
				var contacts = await source.GetAllEntriesAsync();

				var tileContacts = new List<BirthdayContact>();
				if (contacts.Any())
					tileContacts = contacts.OrderBy(it => it.DaysUntil).Take(3).ToList();
				var count = 0;
				contacts = null;
				source = null;
				System.GC.Collect();
				System.GC.WaitForPendingFinalizers();
				System.GC.Collect();

				UpdateTileSynchronous(tileContacts, count, callback);
			}
			catch (Exception ex)
			{
				DebugUtility.SaveDiagnosticException(ex);
			}
		}

		public void UpdateTileSynchronous(List<BirthdayContact> tileContacts, int count = 0, Action callback = null)
		{
#if DEBUG
			DebugUtility.DebugOutputMemoryUsage("Beginning UpdateTileSynchronous");
#endif
			var displayName = "";
#if DEBUG
			DebugUtility.DebugOutputMemoryUsage("UpdateTileSynchronous - Loaded Data");
#endif

			Deployment.Current.Dispatcher.BeginInvoke(() =>
			{
#if DEBUG
				DebugUtility.DebugOutputMemoryUsage("UpdateTileSynchronous - Medium Tile Control Created");
#endif
				ShellTile mainTile = ShellTile.ActiveTiles.First();
				using (IsolatedStorageFile appStorage = IsolatedStorageFile.GetUserStoreForApplication())
				{
#if WP8
					RadFlipTileData tileData = null;
					if (tileContacts.Count > 0)
					{
						displayName = tileContacts.First().DisplayName;
						BirthdayTileBackViewModel backTileModel = new BirthdayTileBackViewModel(tileContacts);
						MediumTileBackUserControl medBackTile = new MediumTileBackUserControl() { DataContext = backTileModel };
						WideTileBackUserControl wideBackTile = new WideTileBackUserControl() { DataContext = backTileModel };
						tileData = new RadFlipTileData()
						{
							Title = Resources.AppTitle.ToTitleCase(),
							BackTitle = Resources.AppTitle.ToTitleCase(),
							BackgroundImage = appStorage.FileExists(string.Format(isoStorePath, displayName, "m")) ?
													new Uri(string.Format(isoStoreUri, displayName, "m"), UriKind.RelativeOrAbsolute) :
													new Uri("/Assets/Tiles/BirthdayTileMedium.png", UriKind.Relative),
							BackVisualElement = medBackTile,
							SmallBackgroundImage = appStorage.FileExists(string.Format(isoStorePath, displayName, "s")) ?
													new Uri(string.Format(isoStoreUri, displayName, "s"), UriKind.RelativeOrAbsolute) :
													new Uri("/Assets/Tiles/BirthdayTileSmall.png", UriKind.RelativeOrAbsolute),
							WideBackgroundImage = appStorage.FileExists(string.Format(isoStorePath, displayName, "w")) ?
													new Uri(string.Format(isoStoreUri, displayName, "w"), UriKind.RelativeOrAbsolute) :
													new Uri("/Assets/Tiles/BirthdayTileWide.png", UriKind.RelativeOrAbsolute),
							WideBackVisualElement = wideBackTile
						};
					}
					else
					{
						tileData = new RadFlipTileData()
						{
							Title = Resources.AppTitle.ToTitleCase(),
							BackgroundImage = new Uri("/Assets/Tiles/BirthdayTileMedium.png", UriKind.Relative),
							SmallBackgroundImage = new Uri("/Assets/Tiles/BirthdayTileSmall.png", UriKind.RelativeOrAbsolute),
							WideBackgroundImage = new Uri("/Assets/Tiles/BirthdayTileWide.png", UriKind.RelativeOrAbsolute),
						};
					}
#if DEBUG
					DebugUtility.DebugOutputMemoryUsage("UpdateTileSynchronous - RadFlipTileData created");
#endif
#else
					RadExtendedTileData tileData = null;
					if (tileContacts.Count > 0)
					{
						displayName = tileContacts.First().DisplayName;
						BirthdayTileBackViewModel backTileModel = new BirthdayTileBackViewModel(tileContacts);
						MediumTileBackUserControlWP7 medBackTile = new MediumTileBackUserControlWP7() { DataContext = backTileModel };
						tileData = new RadExtendedTileData()
						{
							Title = Resources.AppTitle.ToTitleCase(),
							BackTitle = Resources.AppTitle,
							BackgroundImage = appStorage.FileExists(string.Format(isoStorePath, displayName, "m")) ?
												new Uri(string.Format(isoStoreUri, displayName, "m"), UriKind.RelativeOrAbsolute) :
												new Uri("/Assets/Tiles/BirthdayTileMedium.png", UriKind.RelativeOrAbsolute),
							BackVisualElement = medBackTile
						};
					}
					else
					{
						tileData = new RadExtendedTileData()
						{
							Title = Resources.AppTitle.ToTitleCase(),
							BackgroundImage = new Uri("/Assets/Tiles/BirthdayTileMedium.png", UriKind.RelativeOrAbsolute)
						};

					}
#if DEBUG
					DebugUtility.DebugOutputMemoryUsage("UpdateTileSynchronous - RadExtendedTileData created");
#endif
#endif
					try
					{
						LiveTileHelper.UpdateTile(mainTile, tileData);
					}
					catch (Exception ex)
					{
						DebugUtility.SaveDiagnosticException(ex);
						throw ex;
					}
				}
#if DEBUG
				DebugUtility.SaveDiagnosticMessage("Completed tile update");
#endif
				if (callback != null) callback.Invoke();
			});
		}

		public async Task SaveUpcomingImages()
		{
			var source = new Shane.Church.StirlingBirthday.Core.WP.Data.PhoneBirthdaySource();
			var contacts = await source.GetFilteredEntriesAsync(c => c.DaysUntil <= DaysToBuild, false);

			try
			{
				List<string> filenames = new List<string>();
				foreach (var c in contacts)
				{
					var picture = await source.GetContactPictureAsync(c.DisplayName);
					BirthdayTileFrontViewModel model = new BirthdayTileFrontViewModel(c, picture);
					MediumTileUserControl mediumTile = new MediumTileUserControl() { DataContext = model };
#if WP8
					SmallTileUserControl smallTile = new SmallTileUserControl() { DataContext = model };
					WideTileUserControl wideTile = new WideTileUserControl() { DataContext = model };
#endif

					var mediumPath = string.Format(isoStorePath, model.Name, "m");
					filenames.Add(mediumPath);
#if WP8
					var smallPath = string.Format(isoStorePath, model.Name, "s");
					filenames.Add(smallPath);
					var widePath = string.Format(isoStorePath, model.Name, "w");
					filenames.Add(widePath);
#endif
					await Deployment.Current.Dispatcher.InvokeAsync(async () =>
					{
						await mediumTile.ToTileAsync(mediumPath);
#if WP8
						await smallTile.ToTileAsync(smallPath);
						await wideTile.ToTileAsync(widePath);
#endif
					});
				}

				for (int i = 0; i < DaysToBuild; i++)
				{

				}
				CleanUnusedFiles(filenames);
			}
			catch (Exception)
			{

			}
		}

		private void CleanUnusedFiles(IEnumerable<string> filenames)
		{
			using (var store = IsolatedStorageFile.GetUserStoreForApplication())
			{
				try
				{
					foreach (string fileName in store.GetFileNames("/shared/shellcontent/*.png"))
					{
						if (!filenames.Contains("/shared/shellcontent/" + fileName))
							store.DeleteFile(fileName);
					}
				}
				catch { }
			}
		}

	}
}
