using Microsoft.Toolkit.Uwp.Services.Twitter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace HoloTwit2
{
    public sealed partial class SearchResults : UserControl
    {
        public string SearchTerm { get; private set; }
        public bool AutoRefreshing { get; private set; } = false;

        public SearchResults(string searchTerm)
        {
            this.InitializeComponent();
            this.SearchTerm = searchTerm;
            Search();
        }

        private async void Search()
        {
            SearchResultsListView.ItemsSource = await TwitterService.Instance.SearchAsync(this.SearchTerm, 50);
        }

        private void RefreshFeedButton_OnClick(object sender, RoutedEventArgs e)
        {
            Search();
        }

        // Toggle automatic refresh of search results, done every X seconds (20 as of this comment).
        private async void AutoRefreshToggle_OnClick(object sender, RoutedEventArgs e)
        {
            AutoRefreshing = !AutoRefreshing;
            while (AutoRefreshing)
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
        private void CopyTweet_OnClick(object sender, RoutedEventArgs e)
        {
            var menuFlyoutItemSender = (MenuFlyoutItem)sender;
            var tweet = menuFlyoutItemSender.Tag as Tweet;
            DataPackage dataPackage = new DataPackage
            {
                RequestedOperation = DataPackageOperation.Copy
            };
            dataPackage.SetText($"@{tweet.User.ScreenName}: {tweet.Text} ");
            Clipboard.SetContent(dataPackage);
        }

        private void DetachButton_OnClick(object sender, RoutedEventArgs e)
        {
            return;
        }
    }
}
