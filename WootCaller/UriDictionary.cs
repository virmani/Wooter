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
using System.Collections.Generic;

namespace WootCaller
{
    public enum WootType : uint
    {
        JustWoot = 0,
        KidsWoot = 1,
        WineWoot = 2,
        ShirtWoot = 3,
        SelloutWoot = 4,
        HomeWoot = 5,
    }

    public static class UriDictionary
    {
        private static Dictionary<WootType, WootTypeData> uriDict;

        static UriDictionary()
        {
            uriDict = new Dictionary<WootType, WootTypeData>();
            uriDict.Add(WootType.JustWoot, new WootTypeData { WootUri = new Uri("http://api.woot.com/1/sales/current.rss/www.woot.com"), WootName = "Woot.com" });
            uriDict.Add(WootType.KidsWoot, new WootTypeData { WootUri = new Uri("http://api.woot.com/1/sales/current.rss/kids.woot.com"), WootName = "Kids Woot" });
            uriDict.Add(WootType.WineWoot, new WootTypeData { WootUri = new Uri("http://api.woot.com/1/sales/current.rss/wine.woot.com"), WootName = "Wine Woot" });
            uriDict.Add(WootType.ShirtWoot, new WootTypeData { WootUri = new Uri("http://api.woot.com/1/sales/current.rss/shirt.woot.com"), WootName = "Shirt Woot" });
            uriDict.Add(WootType.SelloutWoot, new WootTypeData { WootUri = new Uri("http://api.woot.com/1/sales/current.rss/sellout.woot.com"), WootName = "Sellout Woot" });
            uriDict.Add(WootType.HomeWoot, new WootTypeData { WootUri = new Uri("http://api.woot.com/1/sales/current.rss/home.woot.com"), WootName = "Home Woot" });
        }

        public static int Count
        {
            get
            {
                return uriDict.Count;
            }
        }
        public static Uri GetUriForWoot(WootType wootType)
        {
            return uriDict[wootType].WootUri;
        }

        public static string GetNameForWoot(WootType wootType)
        {
            return uriDict[wootType].WootName;
        }
    }

    public struct WootTypeData
    {
        public string WootName { get; set; }
        public Uri WootUri { get; set; }
    }
}
