using Shane.Church.Utility.Core.WP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Shane.Church.StirlingBirthday.Core.WP.Controls
{
    public partial class WideTileUserControl : UserControl
    {
        public WideTileUserControl()
        {
            InitializeComponent();
        }

        public async Task ToTile(string Path)
        {
            // Need to call these, otherwise the contents aren't rendered correctly.
            this.Measure(new Size(691, 336));
            this.Arrange(new Rect(0, 0, 691, 336));

            WriteableBitmap bitmap = new WriteableBitmap(this, new TranslateTransform());

            await Imaging.SaveImageAsync(bitmap, Path, Imaging.ImageType.Png);
        }
    }
}
