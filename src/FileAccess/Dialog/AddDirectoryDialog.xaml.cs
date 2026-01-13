using System;
using System.Windows;

namespace iTunesSyncer.FileAccess.Dialog
{
    /// <summary>
    /// AddDirectoryDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class AddDirectoryDialog : Window
    {
        public string DirectoryName { get; private set; }


        public AddDirectoryDialog()
        {
        }

        public new void Show()
        {
            throw new NotSupportedException();
        }

        public new bool? ShowDialog()
        {
            InitializeComponent();

            var ret = base.ShowDialog();

            return ret;
        }

        private void Button_ClickOK(object sender, RoutedEventArgs e)
        {
            DirectoryName = textbox.Text;

            DialogResult = true;
            Close();
        }

        private void Button_ClickCancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
