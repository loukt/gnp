using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using GalaSoft.MvvmLight;
using GNP.Model;
using GNP.Resources;

namespace GNP.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                var list = new List<FeedItem>();
                var categories = new ObservableCollection<FeedData>();
                Categories = categories;
                for (int i = 0; i < 6; i++)
                {
                    list.Add(new FeedItem() { Author = "Yassine Serhane", Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer nec odio. Praesent libero. Sed cursus ante dapibus diam. Sed nisi. Nulla quis sem at nibh elementum imperdiet. Duis sagittis ipsum. Praesent mauris. Fusce nec tellus sed augue semper porta. Mauris massa. Vestibulum lacinia arcu eget nulla. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Curabitur sodales ligula in libero. Sed dignissim lacinia nunc. Curabitur tortor. Pellentesque nibh. Aenean quam. In scelerisque sem at dolor. Maecenas mattis. Sed convallis tristique sem. Proin ut ligula vel nunc egestas porttitor. Morbi lectus risus, iaculis vel, suscipit quis, luctus non, massa. Fusce ac turpis quis ligula lacinia aliquet. Mauris ipsum. Nulla metus metus, ullamcorper vel, tincidunt sed, euismod in, nibh. Quisque volutpat condimentum velit. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Nam nec ante. Sed lacinia, urna non tincidunt mattis, tortor neque adipiscing diam, a cursus ipsum ante quis turpis. Nulla facilisi. Ut fringilla. Suspendisse potenti. Nunc feugiat mi a tellus consequat imperdiet. Vestibulum sapien. Proin quam. Etiam ultrices. Suspendisse in justo eu magna luctus suscipit. Sed lectus. Integer euismod lacus luctus magna. Quisque cursus, metus vitae pharetra auctor, sem massa mattis sem, at interdum magna augue eget diam. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Morbi lacinia molestie dui. Praesent blandit dolor. Sed non quam. In vel mi sit amet augue congue elementum. Morbi in ipsum sit amet pede facilisis laoreet. Donec lacus nunc, viverra nec", Index = i, Title = "Titre N°" + i, ImageLink = "Assets/Default.jpg", Group = null, PubDate = DateTime.Now });
                    categories.Add(new FeedData() { Description = "Ceci est une description", PubDate = DateTime.Now, Title = "Categorie" + i });
                    foreach (var category in categories)
                    {
                        foreach (var item in list)
                        {
                            category.Items.Add(item);
                        }
                    }
                }
                MixedItems = list;
                FavCat = Categories;
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(LoadData);
            }
        }

        private ObservableCollection<FeedData> _categories = null;
        public ObservableCollection<FeedData> Categories { get { return _categories; } set { _categories = value; RaisePropertyChanged("Category"); } }

        private List<FeedItem> _mixedItems = null;
        public List<FeedItem> MixedItems { get { return _mixedItems; } set { _mixedItems = value; RaisePropertyChanged("MixedItems"); RaisePropertyChanged("NextHighlights"); } }

        private ObservableCollection<FeedData> _favCat = null;
        public ObservableCollection<FeedData> FavCat { get { return _favCat; } set { _favCat = value; RaisePropertyChanged("FavCat"); } }

        private Boolean _isLoadingData = false;
        public Boolean IsLoadingData { get { return _isLoadingData; } set { _isLoadingData = value; RaisePropertyChanged("IsLoadingData"); } }

        private void LoadData()
        {
            var service = new FeedDataService();
            IsLoadingData = true;
            service.MyEvent += service_MyEvent;
            Categories = service.LoadData();

            //Check for favorites categories
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (!settings.Contains("Favorites"))
            {
                var list = new List<string>();
                list.Add("Sport");
                list.Add("Maroc");
                list.Add("Economie");
                settings["Favorites"] = list;
                settings.Save();
            }

            var fav = settings["Favorites"] as List<string>;
            foreach (var category in Categories)
            {
                if (fav.Contains(category.Title))
                {
                    category.IsFavorite = true;
                }
            }


            //Select Favorites Categories
            FavCat = new ObservableCollection<FeedData>(Categories.Where(o => o.IsFavorite));
        }

        void service_MyEvent(object sender, EventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                MakeAMix();
                IsLoadingData = false;
            });
        }

        private void MakeAMix()
        {
            var mix = new List<FeedItem>();
            foreach (var category in Categories.Where(o => o.IsFavorite))
            {
                mix.AddRange(category.Items);
            }
            //mix all the items.
            Random rand = new Random();
            int num = mix.Count - 1;
            for (int i = 0; i <= num; i++)
            {
                var item = mix[i];
                mix.RemoveAt(i);
                int newIndex = rand.Next(num);
                mix.Insert(newIndex, item);
            }
            mix.TrimExcess();
            MixedItems = (mix.Count <= 6) ? mix : mix.GetRange(0, 6);
        }

        public void NotifyChangeCategories()
        {
            FavCat = new ObservableCollection<FeedData>(Categories.Where(o => o.IsFavorite));
        }
    }
}