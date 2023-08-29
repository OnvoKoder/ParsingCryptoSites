using ParserNansemAPI.Class.Patterns;
using ParserNansemAPI.Class.QueryHtttp;
using ParserNansemAPI.Class.Selenium;
using ParserNansemAPI.Class.settings;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ParserNansemAPI.Pages
{
    /// <summary>
    /// Interaction logic for parsingPage.xaml
    /// </summary>
    public partial class parsingPage : Page
    {
        private ParsingAPI api = new ParsingAPI();
        private ParsingSite site = new ParsingSite();
        private ConfigurationManager manager = new ConfigurationManager();
        private static bool flag = false;
        public parsingPage()
        {
            InitializeComponent();
            api.ProccessCompleted += (sender, e) =>
            {
                Dispatcher.Invoke((Action)(async () =>
                {
                    tbxStatus.Text = $"Status: {Environment.NewLine}{e.Message}";
                    if(e.Message == "Complete")
                    {
                        btnParsing.IsEnabled = true;
                        Settings settings = manager.GetCurrentSettings();
                        if(flag == false)
                        {
                            await Task.Run(() => site.Parsing(settings.Site));
                            flag = true;
                        }
                    }
                    else
                        btnParsing.IsEnabled = false;
                }));
            };
            site.ProccessCompleted += (sender, e) =>
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    tbxStatus.Text = $"Status: {Environment.NewLine}{e.Message}";
                    if (e.Message == "Complete")
                        btnParsing.IsEnabled = true;
                    else
                        btnParsing.IsEnabled = false;
                }));
            };
            btnParsing.Click += async (sender, e) =>
            {
                string label = tbxAddress.Text.Trim();
                if (label != string.Empty)
                {
                    if (manager.InternetAvalible())
                    {
                        btnParsing.IsEnabled = false;
                        await Task.Run(() => api.Parsing(label));
                    }
                    else
                        MessageBox.Show("Internet unavailable", "Notification", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                    MessageBox.Show("Input textbox", "Notification", MessageBoxButton.OK, MessageBoxImage.Warning);
            };
        }
    }
}
