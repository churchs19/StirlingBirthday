using Microsoft.Phone.Shell;
#if AGENT
#else
using Ninject;
#endif
using Shane.Church.StirlingBirthday.Core.Data;
using Shane.Church.StirlingBirthday.Core.Services;
using Shane.Church.StirlingBirthday.Core.ViewModels;
using Shane.Church.StirlingBirthday.Core.WP.Controls;
using Shane.Church.StirlingBirthday.Core.WP.Resources;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
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
#if !AGENT
				var source = KernelService.Kernel.Get<IBirthdaySource>();
#else
				var source = new Shane.Church.StirlingBirthday.Core.WP.Data.PhoneBirthdaySource();
#endif
				var contacts = await source.GetAllEntriesAsync();

				UpdateTileSynchronous(contacts, callback);
			}
			catch (Exception ex)
			{
				DebugUtility.SaveDiagnosticException(ex);
			}
		}

		public void UpdateTileSynchronous(IQueryable<BirthdayContact> contacts, Action callback = null)
		{
			var tileContacts = contacts.OrderBy(it => it.DaysUntil).Take(3);
			var displayName = tileContacts.First().DisplayName;
			var todayCount = contacts.Where(it => it.DaysUntil == 0).Count();

			Deployment.Current.Dispatcher.BeginInvoke(() =>
			{

				BirthdayTileBackViewModel backTileModel = new BirthdayTileBackViewModel(tileContacts);

				MediumTileBackUserControl medBackTile = new MediumTileBackUserControl() { DataContext = backTileModel };

				ShellTile mainTile = ShellTile.ActiveTiles.First();
				using (IsolatedStorageFile appStorage = IsolatedStorageFile.GetUserStoreForApplication())
				{
#if !WP8
					if (LiveTileHelper.AreNewTilesSupported)
					{
#endif
						WideTileBackUserControl wideBackTile = new WideTileBackUserControl();
						wideBackTile.DataContext = backTileModel;
						RadFlipTileData tileData = new RadFlipTileData()
						{
							Title = WPCoreResources.AppTitle,
							Count = todayCount,
							BackTitle = WPCoreResources.AppTitle,
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
						LiveTileHelper.UpdateTile(mainTile, tileData);
#if !WP8
					}
					else
					{
						RadExtendedTileData tileData = new RadExtendedTileData()
						{
							Title = Resources.WPCoreResources.AppTitle,
							Count = todayCount,
							BackTitle = Resources.WPCoreResources.AppTitle,
							BackgroundImage = appStorage.FileExists(string.Format(isoStorePath, displayName, "m")) ?
												new Uri(string.Format(isoStoreUri, displayName, "m"), UriKind.RelativeOrAbsolute) :
												new Uri("/Assets/Tiles/BirthdayTileMedium.png", UriKind.RelativeOrAbsolute),
							BackVisualElement = medBackTile
						};
						LiveTileHelper.UpdateTile(mainTile, tileData);
					}
#endif
				}
#if DEBUG
				DebugUtility.SaveDiagnosticMessage("Completed tile update");
#endif
				if (callback != null) callback.Invoke();
			});
		}

		public async Task SaveUpcomingImages()
		{
#if !AGENT
			var source = KernelService.Kernel.Get<IBirthdaySource>();
#else
				var source = new Shane.Church.StirlingBirthday.Core.WP.Data.PhoneBirthdaySource();
#endif
			var contacts = await source.GetFilteredEntriesAsync(c => c.DaysUntil <= DaysToBuild, false);
			var isostorePath = "/shared/shellcontent/{0}.{1}.png";

			try
			{
				List<string> filenames = new List<string>();
				foreach (var c in contacts)
				{
					var picture = await source.GetContactPicture(c.DisplayName);
#if !AGENT
					Ninject.Parameters.ConstructorArgument arg = new Ninject.Parameters.ConstructorArgument("contact", c);
					Ninject.Parameters.ConstructorArgument picArg = new Ninject.Parameters.ConstructorArgument("picture", picture);
					BirthdayTileFrontViewModel model = KernelService.Kernel.Get<BirthdayTileFrontViewModel>(arg, picArg);
#else
					BirthdayTileFrontViewModel model = new BirthdayTileFrontViewModel(c, picture);					
#endif
					SmallTileUserControl smallTile = new SmallTileUserControl() { DataContext = model };
					MediumTileUserControl mediumTile = new MediumTileUserControl() { DataContext = model };
					WideTileUserControl wideTile = new WideTileUserControl() { DataContext = model };

					var smallPath = string.Format(isostorePath, model.Name, "s");
					filenames.Add(smallPath);
					var mediumPath = string.Format(isostorePath, model.Name, "m");
					filenames.Add(mediumPath);
					var widePath = string.Format(isostorePath, model.Name, "w");
					filenames.Add(widePath);

					await Deployment.Current.Dispatcher.InvokeAsync(async () =>
					{
						await smallTile.ToTile(smallPath);
						await mediumTile.ToTile(mediumPath);
						await wideTile.ToTile(widePath);
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
				//try
				//{
				//    foreach (string fileName in store.GetFileNames("*.tmp"))
				//    {
				//        store.DeleteFile(fileName);
				//    }
				//}
				//catch { }
			}
		}

	}
}
