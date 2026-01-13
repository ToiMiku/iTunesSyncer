using System;
using System.Windows;

namespace iTunesSyncer.UI.ExportSettingWindow
{
    /// <summary>
    /// ExportSettingWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ExportSettingWindow : Window
    {
        public ExportSettingWindow()
        {
            var viewModel = new ExportViewModel(this);
            this.DataContext = viewModel;

            InitializeComponent();
        }

        public new void Show()
        {
           throw new NotSupportedException();
        }
    }
}
