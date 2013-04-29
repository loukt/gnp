using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using GNP.ViewModel;

namespace GNP
{
    public partial class CategoryPage : PhoneApplicationPage
    {
        private int _catIndex = 0;
        public CategoryPage()
        {
            InitializeComponent();
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            try
            {
                _catIndex = Int32.Parse(NavigationContext.QueryString["cat"]);
            }
            catch (Exception)
            {
                
            }

            if (!(App.Current.Resources["Locator"] as ViewModelLocator).Main.IsInDesignMode)
            DataContext = (App.Current.Resources["Locator"] as ViewModelLocator).Main.Categories[_catIndex];
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var list = sender as ListBox;
            if(list.SelectedIndex!=-1)
            {
                int selectedIndex = list.SelectedIndex;
                list.SelectedIndex = -1;
                NavigationService.Navigate(new Uri("/DetailPage.xaml?cat=" + _catIndex+ "&article=" + selectedIndex, UriKind.Relative));
            }
        }
    }
}