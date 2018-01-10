/**
 * Initial code based on https://msdn.microsoft.com/en-us/magazine/mt793275.aspx
 **/

using Microsoft.Toolkit.Uwp.Services.Twitter;
using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HoloTwit2
{
    public sealed partial class MainPage : Page
    {
        // These values shouldn't be stored here, obviously, but they are for now. Paste in valid key / secret before building.
        private string consumerKey = "PLACEKEYHERE";
        private string consumerSecret = "PLACESECRETHERE";
        private string callbackUri = "http://faketrash.placeholder.cx";

        public MainPage()
        {
            this.InitializeComponent();

            // Check if Twitter account is already active; if so, bypass login.
            if (IsTwitterAccountActive()) { ShowMainInterface(); }
            else { HideMainInterface(); }
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

        private async void Search()
        {
            if (!string.IsNullOrWhiteSpace(SearchField.Text) && !string.IsNullOrEmpty(SearchField.Text))
            {
                SearchResults searchResults = new SearchResults(SearchField.Text);
                await searchResults.DetachFeed();
            }
        }

        private void ShowMainInterface()
        {
            SearchPanel.Visibility = Visibility.Visible;
            TwitterLoginButton.Visibility = Visibility.Collapsed;
        }

        private void HideMainInterface()
        {
            SearchPanel.Visibility = Visibility.Collapsed;
            TwitterLoginButton.Visibility = Visibility.Visible;
        }

        private async void TwitterLoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (consumerKey.Equals("PLACEKEYHERE") || consumerSecret.Equals("PLACESECRETHERE")) // Show error message and fail login if consumer key & secret are wrong.
            {
                var errorMessage = new MessageDialog("Placeholder API keys found, you need to change them to valid ones.", "Error");
                await errorMessage.ShowAsync();
            }
            else
            {
                TwitterService.Instance.Initialize(consumerKey, consumerSecret, callbackUri);
                await TwitterService.Instance.LoginAsync();
                ShowMainInterface();
            }
        }

        private void TwitterLogoutButton_Click(object sender, RoutedEventArgs e)
        {
            TwitterService.Instance.Logout();
            HideMainInterface();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e) { Search(); }

        private void SearchField_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter) { Search(); }
        }
    }
}
