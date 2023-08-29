using ParserNansemAPI.Class.Patterns;
using ParserNansemAPI.Class.settings;
using System.Windows.Controls;
using System.Windows;

namespace ParserNansemAPI.Pages
{
    public partial class settingsPage : Page
    {
        private ConfigurationManager manager = new ConfigurationManager();
        public settingsPage()
        {
            InitializeComponent();
            cmbFormat.ItemsSource = new string[]{ "Excel", "Json"};
            CurrentSettings();
            tbxPort.PreviewTextInput += (sender, e) => e.Handled =!char.IsDigit(e.Text, 0) ? true : false;
            cbxProxy.Checked += (sender, e) => proxy.Visibility = Visibility.Visible;
            cbxProxy.Unchecked += (sender, e) => proxy.Visibility = Visibility.Hidden;
            btnSave.Click += (sender, e) =>
            {
                if (tbxAPI.Text != string.Empty && cbxProxy.IsChecked != true && cmbFormat.SelectedIndex != -1 && tbxSite.Text != string.Empty)
                {
                    manager.SaveCurrentSettings(new Settings(tbxAPI.Text.Trim(), new Proxy(false, tbxAddress.Text.Trim(), tbxPort.Text.Trim()), cmbFormat.SelectedValue.ToString(), tbxSite.Text.Trim()));
                    MessageBox.Show("Save", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (cmbFormat.SelectedIndex != -1 && tbxAddress.Text != string.Empty && tbxPort.Text != string.Empty && tbxSite.Text != string.Empty && tbxAPI.Text != string.Empty && cbxProxy.IsChecked == true)
                {
                    if (!manager.IsIPAddress(tbxAddress.Text))
                        MessageBox.Show("This is not IP address", "Notification", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    else
                    {
                        manager.SaveCurrentSettings(new Settings(tbxAPI.Text.Trim(), new Proxy(true, tbxAddress.Text.Trim(), tbxPort.Text.Trim()), cmbFormat.SelectedValue.ToString(), tbxSite.Text.Trim()));
                        MessageBox.Show("Save", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                    MessageBox.Show("Input textbox", "Notification", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            };
        }
        private void CurrentSettings()
        {
            Settings settings = manager.GetCurrentSettings();
            if (settings.Key != null)
            {
                tbxAPI.Text = settings.Key;
                if (settings.Proxy != null)
                {
                    cbxProxy.IsChecked = true;
                    tbxAddress.Text = settings.Proxy.Address;
                    tbxPort.Text = settings.Proxy.Port.ToString();
                }
                cmbFormat.SelectedIndex = settings.Format == "Excel"? 0 : 1;
                tbxSite.Text = settings.Site;
            }

        }
    }
}
