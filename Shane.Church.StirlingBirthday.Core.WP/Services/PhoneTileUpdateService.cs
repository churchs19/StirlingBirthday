using Microsoft.Phone.Shell;
using Ninject;
using Shane.Church.StirlingBirthday.Core.Data;
using Shane.Church.StirlingBirthday.Core.Services;
using Shane.Church.StirlingBirthday.Core.ViewModels;
using Shane.Church.StirlingBirthday.Core.WP.Controls;
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
        public async Task UpdateTile()
        {
            try
            {
                var source = KernelService.Kernel.Get<IBirthdaySource>();
                var contacts = await source.GetAllEntriesAsync();
                var tileContacts = contacts.OrderBy(it => it.DaysUntil).Take(3);
                var isoStoreUri = "isostore:/shared/shellcontent/{0}.{1}.png";
                var displayName = tileContacts.First().DisplayName;
                var todayCount = contacts.Where(it => it.DaysUntil == 0).Count();

                await Deployment.Current.Dispatcher.InvokeAsync(() =>
                {

                    BirthdayTileBackViewModel backTileModel = new BirthdayTileBackViewModel(tileContacts);

                    MediumTileBackUserControl medBackTile = new MediumTileBackUserControl() { DataContext = backTileModel };

                    ShellTile mainTile = ShellTile.ActiveTiles.First();
#if !WP8
                    if (LiveTileHelper.AreNewTilesSupported)
                    {
#endif
                        WideTileBackUserControl wideBackTile = new WideTileBackUserControl();
                        wideBackTile.DataContext = backTileModel;
                        RadFlipTileData tileData = new RadFlipTileData()
                        {
                            Title = Resources.WPCoreResources.AppTitle,
                            Count = todayCount,
                            BackTitle = Resources.WPCoreResources.AppTitle,
                            BackgroundImage = new Uri(string.Format(isoStoreUri, displayName, "m"), UriKind.RelativeOrAbsolute),
                            BackVisualElement = medBackTile,
                            SmallBackgroundImage = new Uri(string.Format(isoStoreUri, displayName, "s"), UriKind.RelativeOrAbsolute),
                            WideBackgroundImage = new Uri(string.Format(isoStoreUri, displayName, "w"), UriKind.RelativeOrAbsolute),
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
                            BackgroundImage = new Uri(string.Format(isoStoreUri, displayName, "m"), UriKind.RelativeOrAbsolute),
                            BackVisualElement = medBackTile
                        };
                        LiveTileHelper.UpdateTile(mainTile, tileData);
                    }
#endif
                });
            }
            catch (Exception ex)
            {
                var str = ex.Message;
            }
        }

        public async Task SaveUpcomingImages()
        {
            var source = KernelService.Kernel.Get<IBirthdaySource>();
            var contacts = await source.GetFilteredEntriesAsync(c => c.DaysUntil <= 31, false);
            var isostorePath = "/shared/shellcontent/{0}.{1}.png";

            try
            {
                List<string> filenames = new List<string>();
                foreach (var c in contacts)
                {
                    var picture = await source.GetContactPicture(c.DisplayName);
                    Ninject.Parameters.ConstructorArgument arg = new Ninject.Parameters.ConstructorArgument("contact", c);
                    Ninject.Parameters.ConstructorArgument picArg = new Ninject.Parameters.ConstructorArgument("picture", picture);
                    BirthdayTileFrontViewModel model = KernelService.Kernel.Get<BirthdayTileFrontViewModel>(arg, picArg);
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
