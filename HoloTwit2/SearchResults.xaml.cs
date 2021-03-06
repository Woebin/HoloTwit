﻿using Microsoft.Toolkit.Uwp.Services.Twitter;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

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

        private void RefreshFeedButton_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }

        // Toggle automatic refresh of search results, done every X seconds (20 as of this comment).
        private async void AutoRefreshToggle_Click(object sender, RoutedEventArgs e)
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
        private void CopyTweet_Click(object sender, RoutedEventArgs e)
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

        // Open selected feed in new window.
        // Based on https://social.msdn.microsoft.com/Forums/sqlserver/en-US/f1328991-b5e5-48e1-b4ff-536a0013ef9f/uwpis-it-possible-to-open-a-new-window-in-uwp-apps?forum=wpdevelop
        private async void DetachButton_Click(object sender, RoutedEventArgs e)
        {
            var currentApplicationView = ApplicationView.GetForCurrentView();
            var newApplicationView = CoreApplication.CreateNewView();
            await newApplicationView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                            {
                                var newWindow = Window.Current;
                                var newAppView = ApplicationView.GetForCurrentView();
                                newAppView.Title = SearchTerm;

                                newWindow.Content = new SearchResults(SearchTerm);
                                newWindow.Activate();

                                await ApplicationViewSwitcher.TryShowAsStandaloneAsync(
                                    newAppView.Id,
                                    ViewSizePreference.UseMinimum,
                                    currentApplicationView.Id,
                                    ViewSizePreference.UseMinimum);
                            });
        }
    }
}
