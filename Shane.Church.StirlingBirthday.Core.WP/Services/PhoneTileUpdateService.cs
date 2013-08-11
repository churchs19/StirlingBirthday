using Shane.Church.StirlingBirthday.Core.Services;
using Shane.Church.StirlingBirthday.Core.ViewModels;
using Shane.Church.StirlingBirthday.Core.WP.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Phone.Shell;
using Ninject;
using Telerik.Windows.Controls;
using Shane.Church.StirlingBirthday.Core.Data;

namespace Shane.Church.StirlingBirthday.Core.WP.Services
{
    public class PhoneTileUpdateService : ITileUpdateService
    {
        public async Task<bool> UpdateTile()
        {
            try
            {
                var source = KernelService.Kernel.Get<IBirthdaySource>();
                var contacts = await source.GetAllEntriesAsync();
                var tileContacts = contacts.OrderBy(it => it.DaysUntil).Take(3);

                var arg = new Ninject.Parameters.ConstructorArgument("contact", tileContacts.FirstOrDefault());
                BirthdayTileFrontViewModel tileModel = KernelService.Kernel.Get<BirthdayTileFrontViewModel>(arg);
                BirthdayTileBackViewModel backTileModel = new BirthdayTileBackViewModel(tileContacts);

                MediumTileUserControl medFrontTile = new MediumTileUserControl();
                medFrontTile.DataContext = tileModel;
                MediumTileBackUserControl medBackTile = new MediumTileBackUserControl();
                medBackTile.DataContext = backTileModel;

                ShellTile mainTile = ShellTile.ActiveTiles.First();
                if (LiveTileHelper.AreNewTilesSupported)
                {
                    SmallTileUserControl smallFrontTile = new SmallTileUserControl();
                    smallFrontTile.DataContext = tileModel;
                    WideTileUserControl wideFrontTile = new WideTileUserControl();
                    wideFrontTile.DataContext = tileModel;
                    WideTileBackUserControl wideBackTile = new WideTileBackUserControl();
                    wideBackTile.DataContext = backTileModel;
                    RadFlipTileData tileData = new RadFlipTileData()
                    {
                        Title = Resources.WPCoreResources.AppTitle,
                        BackTitle = Resources.WPCoreResources.AppTitle,
                        VisualElement = medFrontTile,
                        BackVisualElement = medBackTile,
                        SmallVisualElement = smallFrontTile,
                        WideVisualElement = wideFrontTile,
                        WideBackVisualElement = wideBackTile
                    };
                    LiveTileHelper.UpdateTile(mainTile, tileData);
                }
                else
                {
                    RadExtendedTileData tileData = new RadExtendedTileData()
                    {
                        Title = Resources.WPCoreResources.AppTitle,
                        BackTitle = Resources.WPCoreResources.AppTitle,
                        VisualElement = medFrontTile,
                        BackVisualElement = medBackTile
                    };
                    LiveTileHelper.UpdateTile(mainTile, tileData);
                }
            }
            catch (Exception ex)
            {
                var str = ex.Message;
            }
            return true;
        }
    }
}
