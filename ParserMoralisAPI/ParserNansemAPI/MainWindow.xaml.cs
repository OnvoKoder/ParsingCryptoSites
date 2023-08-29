using ParserNansemAPI.Pages;
using System.Windows;

namespace ParserNansemAPI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            mainFrame.Navigate(new mainPage());
            mainFrame.NavigationUIVisibility = System.Windows.Navigation.NavigationUIVisibility.Hidden;
            mainFrame.ContentRendered += (sender, e) => btnBack.Visibility = mainFrame.CanGoBack ? Visibility.Visible : Visibility.Hidden;
            btnBack.Click += (sender, e) => mainFrame.GoBack();
            btnSettings.Click += (sender, e) => mainFrame.Navigate(new settingsPage());
            btnParsing.Click += (sender, e) => mainFrame.Navigate(new parsingPage());
        }
    }
}
