using System;
using System.IO.IsolatedStorage;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using GNP.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using GNP.Model;
using GNP.ViewModel;

namespace GNP
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
        }


        private void LongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.OriginalSource is CheckBox)
            {
                CategoryList.SelectedItem = null;
                return;
            }
            if (CategoryList.SelectedItem != null)
            {
                var categories = (App.Current.Resources["Locator"] as ViewModelLocator).Main.Categories;
                int catIndex = categories.IndexOf(CategoryList.SelectedItem as FeedData);
                CategoryList.SelectedItem = null;
                NavigationService.Navigate(new Uri("/CategoryPage.xaml?cat=" + catIndex, UriKind.Relative));
            }

        }


        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var list = sender as ListBox;
            if (list.SelectedItem != null)
            {
                var categories = (App.Current.Resources["Locator"] as ViewModelLocator).Main.Categories;
                int catIndex = categories.IndexOf(list.SelectedItem as FeedData);
                list.SelectedItem = null;
                NavigationService.Navigate(new Uri("/CategoryPage.xaml?cat=" + catIndex, UriKind.Relative));
            }
        }

        private void LongList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var list = sender as LongListSelector;
            if (list.SelectedItem != null)
            {
                FeedItem item = list.SelectedItem as FeedItem;
                int catIndex = (App.Current.Resources["Locator"] as ViewModelLocator).Main.Categories.IndexOf(item.Group);
                list.SelectedItem = null;
                NavigationService.Navigate(new Uri("/DetailPage.xaml?cat=" + catIndex + "&article=" + item.Index, UriKind.Relative));
            }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            var check = sender as CheckBox;
            var category = check.DataContext as FeedData;
            var list = settings["Favorites"] as List<string>;
            if(check.IsChecked.HasValue && check.IsChecked.Value)
            {
                if (!list.Contains(category.Title))
                    list.Add(category.Title);
            }
            else 
            {
                if (list.Count == 3) 
                { 
                    check.IsChecked = true;
                    MessageBox.Show(AppResources.ErrorFavDelete);
                }
                else 
                {
                    if (list.Contains(category.Title))
                        list.Remove(category.Title);
                }
            }
            category.IsFavorite = check.IsChecked.Value;
            (App.Current.Resources["Locator"] as ViewModelLocator).Main.NotifyChangeCategories();
            settings.Save();
            //CategoryList.SelectedItem = null;
        }

        private void ButtonSetting_Click(object sender, RoutedEventArgs e)
        {
            MainPanorama.DefaultItem = MainPanorama.Items[2];
        }
        private void ButtonRate_Click(object sender, RoutedEventArgs e)
        {
            var marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }
        private void ButtonAbout_Click(object sender, RoutedEventArgs e)
        {
            MainPanorama.DefaultItem = MainPanorama.Items[3];
        }
        private void ButtonBrowse_Click(object sender, RoutedEventArgs e)
        {
            var wb = new WebBrowserTask();
            wb.Uri = new Uri(AppResources.WebSiteURL, UriKind.Absolute);
            wb.Show();
        }

        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var task = new MarketplaceSearchTask();
            task.SearchTerms = "Yassine Serhane";
            task.Show();
        }
    }
}