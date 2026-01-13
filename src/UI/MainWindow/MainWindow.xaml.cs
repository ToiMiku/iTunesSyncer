using System.Reflection;
using System.Windows;

namespace iTunesSyncer.UI.MainWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            Title = GetTitle();
        }

        private string GetTitle()
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;

            var fullname = typeof(App).Assembly.Location;
            var info = System.Diagnostics.FileVersionInfo.GetVersionInfo(fullname);
            var ver = info.FileVersion;

            return assemblyName + " " + ver;
        }
    }
}
