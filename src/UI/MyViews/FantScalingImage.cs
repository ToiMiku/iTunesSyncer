
namespace iTunesSyncer.UI.MyViews
{
    public class FantScalingImage : System.Windows.Controls.Image
    {
        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
            this.VisualBitmapScalingMode = System.Windows.Media.BitmapScalingMode.Fant;
            base.OnRender(dc);
        }
    }
}
