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
    public partial class DetailPage : PhoneApplicationPage
    {
        private int _index;
        private int _catIndex;

        public DetailPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            try
            {
                _catIndex = Int32.Parse(NavigationContext.QueryString["cat"]);
                _index = Int32.Parse(NavigationContext.QueryString["article"]);
            }
            catch (Exception)
            {

            }
            var category = (App.Current.Resources["Locator"] as ViewModelLocator).Main.Categories[_catIndex];
            if (_index == category.Items.Count) _index = 0;
            if (_index == -1) _index = category.Items.Count - 1;
            
            if(!(App.Current.Resources["Locator"] as ViewModelLocator).Main.IsInDesignMode)
            DataContext=category.Items[_index];
        }

        private void ApplicationBarIconButton_RightClick(object sender, EventArgs e)
        {
            _index++;
            var category = (App.Current.Resources["Locator"] as ViewModelLocator).Main.Categories[_catIndex];
            if (_index == category.Items.Count) _index = 0;
            if (_index == -1) _index = category.Items.Count - 1;
            DataContext=category.Items[_index];
        }
        private void ApplicationBarIconButton_LeftClick(object sender, EventArgs e)
        {
            _index --;
            var category = (App.Current.Resources["Locator"] as ViewModelLocator).Main.Categories[_catIndex];
            if (_index == category.Items.Count) _index = 0;
            if (_index == -1) _index = category.Items.Count - 1;
            DataContext=category.Items[_index];
        }
    }
}