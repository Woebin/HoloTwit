/**
 * Initial code based on https://msdn.microsoft.com/en-us/magazine/mt793275.aspx
 **/

using Microsoft.Toolkit.Uwp.Services.Twitter;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HoloTwit2
{
    public sealed partial class MainPage : Page
    {
        private string ConsumerKey { get; set; } = "PLACEKEYHERE";
        private string ConsumerSecret { get; set; } = "PLACESECRETHERE";
        private string CallbackUri { get; set; } = "http://faketrash.placeholder.cx";

        public MainPage()
        {
            this.InitializeComponent();

            if (IsTwitterAccountActive())
            {
                SearchPanel.Visibility = Visibility.Visible;
                TwitterLoginButton.Visibility = Visibility.Collapsed;
                FeedBladeView.Visibility = Visibility.Visible;
            }
            else
            {
                SearchPanel.Visibility = Visibility.Collapsed;
                TwitterLoginButton.Visibility = Visibility.Visible;
                FeedBladeView.Visibility = Visibility.Collapsed;
            }
        }

        private bool IsTwitterAccountActive()
        {
            try
            {
                var currentScreenName = TwitterService.Instance.UserScreenName;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private async void TwitterLoginButton_Click(object sender, RoutedEventArgs e)
        {
            TwitterService.Instance.Initialize(ConsumerKey, ConsumerSecret, CallbackUri);
            if (await TwitterService.Instance.LoginAsync())
            {
                SearchPanel.Visibility = Visibility.Visible;
                TwitterLoginButton.Visibility = Visibility.Collapsed;
                FeedBladeView.Visibility = Visibility.Visible;
            }
        }

        private void Search()
        {
            if (!string.IsNullOrWhiteSpace(SearchField.Text) && !string.IsNullOrEmpty(SearchField.Text))
            {
                BladeItem bladeItem = new BladeItem
                {
                    Title = SearchField.Text,
                    IsExpanded = false,
                    Content = new SearchResults(SearchField.Text)
                };
                FeedBladeView.Items.Add(bladeItem);
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }


        private void SearchField_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                Search();
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            TwitterService.Instance.Logout();
            SearchPanel.Visibility = Visibility.Collapsed;
            TwitterLoginButton.Visibility = Visibility.Visible;
            FeedBladeView.Visibility = Visibility.Collapsed;
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            SearchPanel.IsPaneOpen = !SearchPanel.IsPaneOpen;
        }
    }

}
