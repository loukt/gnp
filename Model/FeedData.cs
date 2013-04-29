using System;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using GalaSoft.MvvmLight;
using System.ServiceModel.Syndication;
using System.Xml;


namespace GNP.Model
{
    // Holds info for a single news post
    public class FeedItem : ObservableObject
    {
        public FeedData Group { get; set; }
        public int Index { get; set; }
        private string _title;
        public string Title { get { return _title; } set { _title = value; RaisePropertyChanged("Content"); } }
        public string Author { get; set; }
        private string _content;
        public string Content { get { return _content; } set { _content = value; RaisePropertyChanged("Content"); } }
        private string _imageLink;
        public string ImageLink { get { return _imageLink; } set { _imageLink = value; RaisePropertyChanged("ImageLink"); } }
        public string[] VideoLink { get; set; }
        public string[] MediaLinks { get; set; }
        public DateTime PubDate { get; set; }
        public Uri Link { get; set; }
        private bool _isDataLoading;
        public bool IsDataLoading { get { return _isDataLoading; } set { _isDataLoading = value; RaisePropertyChanged("IsDataLoading"); } }
    }

    // Holds info for a single category, which contains a lot of blog posts(Items)
    public class FeedData : ObservableObject
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }

        private bool _isFavorite;
        public bool IsFavorite { get { return _isFavorite; } set { _isFavorite = value; RaisePropertyChanged("IsFavorite"); } }
        public string Link { get; set; }
        public DateTime PubDate { get; set; }
        private ObservableCollection<FeedItem> _items = new ObservableCollection<FeedItem>();
        public ObservableCollection<FeedItem> Items
        {
            get { return this._items; }
        }

        public async Task<string> getFeedString(string url)
        {
            var webClient = new WebClient();
            string feed = await DownloadStringTask(webClient, new Uri(url));
            return feed;
        }

        //function to call for parsing
        public async void GetFeedAsync(FeedDataService service)
        {
            try
            {
                string url = Link;
                string color = null;
                string sfeed = await getFeedString(url);
                StringReader stringReader = new StringReader(sfeed);
                XmlReader xmlReader = XmlReader.Create(stringReader);
                SyndicationFeed feed = SyndicationFeed.Load(xmlReader);

                // Process the feed and copy the data you want into the FeedData and FeedItem classes.

                this.Color = color;
                int index = 0;
                /*if (feed.Title != null && feed.Title.Text != null)
                {
                    this.Title = feed.Title;
                       
                }*/
                if (feed.Description != null && feed.Description.Text != null)
                {
                    this.Description = feed.Description.Text;
                }
                if (feed.Items.Any())
                {
                    // Use the date of the latest post as the last updated date.
                    this.PubDate = feed.Items.ElementAt(0).PublishDate.DateTime;

                    foreach (SyndicationItem item in feed.Items)
                    {
                        FeedItem feedItem = new FeedItem();
                        if (item.Title != null && item.Title.Text != null)
                        {
                            feedItem.Title = item.Title.Text;
                        }
                        if (item.PublishDate != null)
                        {
                            feedItem.PubDate = item.PublishDate.DateTime;
                        }
                        if (item.Authors != null && item.Authors.Count > 0)
                        {
                            feedItem.Author = item.Authors[0].Name;
                        }

                        if (item.Summary != null && item.Summary.Text != null)
                        {
                            string fixedString = Regex.Replace(item.Summary.Text, "<[^>]+>", string.Empty);
                            feedItem.Content = HttpUtility.HtmlDecode(fixedString);
                        }

                        //retrieve links or image
                        if (item.Links != null && item.Links.Count > 0)
                        {
                            feedItem.Link = item.Links.First().Uri;
                            if (item.Links.Count > 1)
                                feedItem.ImageLink = item.Links.Last().Uri.ToString();
                            else
                                feedItem.ImageLink = "Assets/Default.jpg";
                        }

                        feedItem.Group = this;
                        feedItem.Index = index;
                        index++;
                        this.Items.Add(feedItem);
                    }
                }
                service.NumFeed--;
            }
            catch (Exception)
            {
                service.NumFeed--;
            }
        }

        public async Task<string> DownloadStringTask(WebClient webClient, Uri uri)
        {
            var tcs = new TaskCompletionSource<string>();
            webClient.DownloadStringCompleted += (s, e) =>
            {
                if (e.Error != null)
                {
                    tcs.SetException(e.Error);
                }
                else
                {
                    tcs.SetResult(e.Result);
                }
            };
            webClient.DownloadStringAsync(uri);


            string result = await tcs.Task;
            return result;
        }
    }

    //Holds all the Categories, listed in BaseLinks file.
    public class FeedDataService
    {
        public event EventHandler MyEvent;
        private int _numFeed = 0;
        public int NumFeed
        {
            get { return _numFeed; }
            set
            {
                _numFeed = value;
                if (_numFeed == 0) MyEvent.Invoke(this, null);
            }
        }

        public ObservableCollection<FeedData> LoadData()
        {
            var feedElements = XDocument.Load("Resources/BaseLinks.xml");

            var result = (from entry in feedElements.Descendants("feedlink")
                          select new FeedData()
                          {
                              Title = entry.Element("name") != null ? entry.Element("name").Value : "No Name",
                              Link = entry.Element("link") != null ? entry.Element("link").Value : ""
                          }).ToList<FeedData>();
            NumFeed = result.Count;

            foreach (var feedData in result)
            {
                feedData.GetFeedAsync(this);
            }
            return new ObservableCollection<FeedData>(result);
        }
    }
}
