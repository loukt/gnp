using System.Windows;
using GNP.Model;

namespace GNP.Common
{
    /// <summary>
    /// Selects between different templates
    /// </summary>
    public class HighlightTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// The template to use when it's the first Item
        /// </summary>
        /// 

        public DataTemplate FirstItem
        {
            get;
            set;
        }

        /// <summary>
        /// The default template
        /// </summary>
        public DataTemplate OtherItems
        {
            get;
            set;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return (item as FeedItem).Index == 0 ? FirstItem : OtherItems;
        }
    }
}
