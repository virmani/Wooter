using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Linq;
using System.Collections;
using System.Windows.Media.Imaging;

namespace WootCaller
{
    public static class WootAdapter
    {
        private static string RemoveDocTypeDecl(string xml, out bool foundDocType)
        {
            foundDocType = false;
            int docTypeIndex = xml.IndexOf("<!DOCTYPE");
            if (docTypeIndex >= 0)
            {
                foundDocType = true;
                int docTypeEnd = xml.IndexOf(">", docTypeIndex);

                if (docTypeEnd >= 0)
                {
                    xml = xml.Substring(0, docTypeIndex) + xml.Substring(docTypeEnd + 1);
                }
            }
            return xml;
        }

        public static WootData ExtractAllWootData(string rssFeedStr, out bool foundDocType)
        {
            rssFeedStr = RemoveDocTypeDecl(rssFeedStr, out foundDocType);

            XElement rssFeed = XElement.Parse(rssFeedStr);
            XNamespace wootNamespace = "http://www.woot.com/";

            IEnumerable listOfNames = from item in rssFeed.Descendants("item")
                                      let teaser = item.Element(wootNamespace + "teaser")
                                      let subtitle = item.Element(wootNamespace + "subtitle")
                                      select new WootData
                                      {
                                          ProductTitle = HttpUtility.HtmlDecode(item.Element("title").Value),
                                          ProductDescription = item.Element("description").Value,
                                          ProductImageUri = new Uri(item.Element(wootNamespace + "standardimage").Value),
                                          ProductDetailImageUri = new Uri(item.Element(wootNamespace + "detailimage").Value),
                                          ProductPrice = HttpUtility.HtmlDecode(item.Element(wootNamespace + "price").Value.Trim().Trim()),
                                          ProductShippingString = HttpUtility.HtmlDecode(item.Element(wootNamespace + "shipping").Value.Trim().Trim()),
                                          ProductCondition = HttpUtility.HtmlDecode(item.Element(wootNamespace + "condition").Value.Trim()),
                                          ProductSoldOutPercentage = HttpUtility.HtmlDecode(item.Element(wootNamespace + "soldoutpercentage").Value.Trim()),
                                          ProductPurchaseUri = item.Element(wootNamespace + "purchaseurl").Value,
                                          ProductPriceWithShipping = string.Format("{0} + {1}", HttpUtility.HtmlDecode(item.Element(wootNamespace + "price").Value.Trim().Trim()),
                                            HttpUtility.HtmlDecode(item.Element(wootNamespace + "shipping").Value.Trim().Trim())),
                                          ProductSubtitle = (subtitle == null ? string.Empty : subtitle.Value.Trim()),
                                          ProductTeaser = (teaser == null ? string.Empty : teaser.Value.Trim()),
                                          IsWootOff = HttpUtility.HtmlDecode(item.Element(wootNamespace + "wootoff").Value.Trim()),
                                      };
            int i = 0;
            //TODO: Fix the null
            WootData nameToReturn = null;
            foreach (WootData name in listOfNames)
            {
                if (++i > 1)
                {
                    throw new ArgumentOutOfRangeException("There should not be more than one items in the feed");
                }
                nameToReturn = name;
            }
            nameToReturn.WootRssString = rssFeedStr;
            return nameToReturn;
        }
    }

    public class WootData
    {
        public string WootType { get; set; }
        public string ProductTitle { get; set; }
        public Uri ProductImageUri { get; set; }
        public Uri ProductDetailImageUri { get; set; }
        public string ProductCondition { get; set; }
        public string ProductPrice { get; set; }
        public string ProductShippingString { get; set; }
        public string ProductPriceWithShipping { get; set; }
        public string ProductDescription { get; set; }
        public string ProductSubtitle { get; set; }
        public string ProductTeaser { get; set; }
        public string ProductSoldOutPercentage { get; set; }
        public string ProductPurchaseUri { get; set; }
        public string WootRssString { get; set; }
        public string IsWootOff { get; set; }
        public BitmapImage ProductImage { get; set; }
        public BitmapImage ProductDetailImage { get; set; }
    }
}
