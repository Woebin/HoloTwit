/**
 * Initial code based on https://msdn.microsoft.com/en-us/magazine/mt793275.aspx
 **/

using Microsoft.Toolkit.Uwp.Services.Twitter;
using System;
using System.Collections.Generic;
using Windows.UI.Popups;
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

            // Check if Twitter account is already active, if so bypass login.
            if (IsTwitterAccountActive())
            {
                ShowMainInterface();
            }
            else
            {
                HideMainInterface();
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
            TwitterService.Instance.Initialize(ConsumerKey, ConsumerSecret, CallbackUri);
            if (await TwitterService.Instance.LoginAsync())
            {
                ShowMainInterface();
            }
            else
            {
                var msg = new MessageDialog("Login failed!", "Error"); // Terrible and unhelpful error message.
                await msg.ShowAsync();
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

        private void TwitterLogoutButton_Click(object sender, RoutedEventArgs e)
        {
            TwitterService.Instance.Logout();
            HideMainInterface();
        }
    }

}
