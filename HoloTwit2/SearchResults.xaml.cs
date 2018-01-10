using Microsoft.Toolkit.Uwp.Services.Twitter;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace HoloTwit2
{
    public sealed partial class SearchResults : UserControl
    {
        private string searchTerm;
        private bool autoRefreshing = false;
        private int minColor, maxColor;
        private Random random = new Random();

        public SearchResults(string searchTerm)
        {
            this.InitializeComponent();
            this.searchTerm = searchTerm;
            if (Application.Current.RequestedTheme == ApplicationTheme.Dark) // Set background color range to match dark theme.
            {
                minColor = 0;
                maxColor = 127;
            }
            else // Set background color range to match light theme.
            {
                minColor = 128;
                maxColor = 255;
            }
            RandomizeBackground();
            Search();
        }

        // Set background color for search feed.
        private void RandomizeBackground()
        {
            SearchResultsListView.Background = new SolidColorBrush(Color.FromArgb(255,
                (byte)random.Next(minColor, maxColor),
                (byte)random.Next(minColor, maxColor),
                (byte)random.Next(minColor, maxColor)));
        }

        private async void Search()
        {
            SearchResultsListView.ItemsSource = await TwitterService.Instance.SearchAsync(this.searchTerm, 50);
        }

        // Open selected feed in new window.
        // Based on https://social.msdn.microsoft.com/Forums/sqlserver/en-US/f1328991-b5e5-48e1-b4ff-536a0013ef9f/uwpis-it-possible-to-open-a-new-window-in-uwp-apps?forum=wpdevelop
        public async Task DetachFeed()
        {
            var currentApplicationView = ApplicationView.GetForCurrentView();
            var newApplicationView = CoreApplication.CreateNewView();
            await newApplicationView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var newWindow = Window.Current;
                var newAppView = ApplicationView.GetForCurrentView();
                newAppView.Title = searchTerm;

                newWindow.Content = new SearchResults(searchTerm);
                newWindow.Activate();

                await ApplicationViewSwitcher.TryShowAsStandaloneAsync(
                    newAppView.Id,
                    ViewSizePreference.UseMinimum,
                    currentApplicationView.Id,
                    ViewSizePreference.UseMinimum);
            });
        }

        private void RefreshFeedButton_Click(object sender, RoutedEventArgs e) { Search(); }
        private void RandomizeBackgroundButton_Click(object sender, RoutedEventArgs e) { RandomizeBackground(); }

        // Toggle automatic refresh of search results.
        // When enabled, refresh every X seconds (20 as of this comment).
        private async void AutoRefreshToggle_Click(object sender, RoutedEventArgs e)
        {
            autoRefreshing = !autoRefreshing;
            while (autoRefreshing)
            {
                await Task.Delay(20000);
                Search();
            }
        }


        // Bring out context menu when right-clicking on a tweet. From https://msdn.microsoft.com/en-us/magazine/mt793275.aspx
        private void SearchResultsListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var tweet = ((FrameworkElement)e.OriginalSource).DataContext;
            CopyTweet.Tag = tweet;
            CopyMenu.ShowAt(SearchResultsListView, e.GetPosition(SearchResultsListView));
        }

        // Copy tweet to clipboard. Also from https://msdn.microsoft.com/en-us/magazine/mt793275.aspx
        private void CopyTweet_Click(object sender, RoutedEventArgs e)
        {
            var menuFlyoutItemSender = (MenuFlyoutItem)sender;
            var tweet = menuFlyoutItemSender.Tag as Tweet;
            DataPackage dataPackage = new DataPackage
            {
                RequestedOperation = DataPackageOperation.Copy
            };
            dataPackage.SetText($"@{tweet.User.ScreenName}: {tweet.Text} ({tweet.CreationDate})");
            Clipboard.SetContent(dataPackage);
        }
    }
}
