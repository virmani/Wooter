using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using ProgressSplashScreen;
using WootCaller;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Net.NetworkInformation;
using System.Diagnostics;
using Wooter.Analytics;
using System.Text;

namespace Wooter
{
    public partial class MainPage : PhoneApplicationPage
    {
        private static readonly Dictionary<WootType, PanoramaItem> wootDictionary =
            new Dictionary<WootType, PanoramaItem>();
        private static readonly AutoResetEvent[] eventsArray = new AutoResetEvent[UriDictionary.Count];
        private const short WaitTimeForWoot = 10000;

        private const string SCRIPT = @"var links = document.getElementsByTagName('a');
                                           for( var i = 0; i < links.length; i++  ){ links[i].attachEvent('onclick', overrideLink); }
                                           function preventDefaultAction(evt) {if (evt) {  evt.returnValue = false; } return false; }
                                           function overrideLink(evt) { window.external.Notify(evt.srcElement.href); return preventDefaultAction(evt); }
                                           function lightboxImage(lnk) { window.external.Notify('wooterimage'+lnk); }";
        private readonly Popup splashPopup = new Popup();

        private LocalyticsSession AppSession
        {
            get
            {
                return ((App)Application.Current).appSession;
            }
        }

        private void ShowPopup()
        {
            this.splashPopup.Child = new PopupSplash();
            this.splashPopup.IsOpen = true;
        }

        public MainPage()
        {
            InitializeComponent();
            ShowPopup();
        }

        private void OnMainPageLoaded(object sender, RoutedEventArgs e)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show("Your phone has lost internet connectivity. Please try again later.",
                    "Internet Connectivity Lost",
                    MessageBoxButton.OK);

                AppSession.TagWooterEvent(UserTrackingEvents.NetworkNotAvailable);
                throw new ExitApplicationException("Network not available");
            }

            LittleWatson.CheckForPreviousException();

            for (int i = 0; i < UriDictionary.Count; i++)
            {
                eventsArray[i] = new AutoResetEvent(false);
            }

            try
            {
                AddWootPivotItem(WootType.JustWoot);
                AddWootPivotItem(WootType.ShirtWoot);
                AddWootPivotItem(WootType.KidsWoot);
                AddWootPivotItem(WootType.WineWoot);
                AddWootPivotItem(WootType.SelloutWoot);
                AddWootPivotItem(WootType.HomeWoot);
            }
            catch (Exception ex)
            {
                throw new ApplicationCrashException("Failed in one of the AddWootPivotItems", ex);
            }

            // make the async call
            ThreadPool.QueueUserWorkItem((state) =>
            {
                WaitForEventsAndDraw();
            });

        }

        private void WaitForEventsAndDraw()
        {
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            int numFired = eventsArray.Length;
            numFired = WaitForEvents(numFired, WaitTimeForWoot);

            if (numFired != 0)
            {
                AppSession.TagWooterEvent(UserTrackingEvents.TimedOutWaitingForWebResponse);
                throw new ExitApplicationException("All woots did not return within the timeout");
            }

            AddPanoramaItems();

            numFired = WaitForEvents(eventsArray.Length, 5000);
            if (numFired != 0)
            {
                AppSession.TagWooterEvent(UserTrackingEvents.TimedOutWaitingForRender);
                throw new ExitApplicationException("All woots did not render within the timeout");
            }

            stopwatch.Stop();
            double totalTime = Math.Ceiling(((double)stopwatch.ElapsedMilliseconds / (double)1000));

            var attributes = new Dictionary<string, string>();
            attributes.Add(UserTrackingEvents.AttributeNames.TotalLoadTime, totalTime.ToString());

            Dispatcher.BeginInvoke(() =>
            {
                splashPopup.IsOpen = false;
                AppSession.TagWooterEvent(UserTrackingEvents.ApplicationLoaded, attributes);
            });
        }

        private static int WaitForEvents(int numFired, int millisecondsToWaitPerEvent)
        {
            for (int i = 0; i < eventsArray.Length; i++)
            {
                if (eventsArray[i].WaitOne(millisecondsToWaitPerEvent))
                {
                    numFired--;
                    eventsArray[i].Reset();
                }
            }
            return numFired;
        }

        private void AddPanoramaItems()
        {
            Dispatcher.BeginInvoke(() => { panorama1.Items.Add(wootDictionary[WootType.JustWoot]); eventsArray[0].Set(); });
            Dispatcher.BeginInvoke(() => { panorama1.Items.Add(wootDictionary[WootType.ShirtWoot]); eventsArray[1].Set(); });
            Dispatcher.BeginInvoke(() => { panorama1.Items.Add(wootDictionary[WootType.WineWoot]); eventsArray[2].Set(); });
            Dispatcher.BeginInvoke(() => { panorama1.Items.Add(wootDictionary[WootType.KidsWoot]); eventsArray[3].Set(); });
            Dispatcher.BeginInvoke(() => { panorama1.Items.Add(wootDictionary[WootType.SelloutWoot]); eventsArray[4].Set(); });
            Dispatcher.BeginInvoke(() => { panorama1.Items.Add(wootDictionary[WootType.HomeWoot]); eventsArray[5].Set(); });
        }

        private void AddWootPivotItem(WootType wootType)
        {
            try
            {
                WebResourceClient.GetResourceFromWeb(UriDictionary.GetUriForWoot(wootType),
                    new AsyncCallback(PopulateWoot), new WootPivotData
                    {
                        WootType = wootType,
                        PanoramaControl = panorama1,
                        ResetEvent = eventsArray[Convert.ToInt32(wootType)]
                    });
            }
            catch (Exception ex)
            {
                throw new ApplicationCrashException("Failed while sending the request using GetResourceFromWeb", ex);
            }
        }

        private void PopulateWoot(IAsyncResult asyncResult)
        {
            WebRequestToken userToken = asyncResult.AsyncState as WebRequestToken;
            WootPivotData wootPivotData = userToken.UserToken as WootPivotData;

            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)userToken.Request.EndGetResponse(asyncResult);
                using (StreamReader streamReader1 = new StreamReader(response.GetResponseStream()))
                {
                    string resultString = streamReader1.ReadToEnd();
                    DrawItemsFromWootData(resultString, wootPivotData.WootType,
                        wootPivotData.PanoramaControl, wootPivotData.ResetEvent);
                }
            }
            catch (WebException ex)
            {
                var responseFormat = "WebException While reading the Http response in the callback. \r\n Response: {0}";
                string resultString = "<empty>";

                using (StreamReader streamReader = new StreamReader(ex.Response.GetResponseStream()))
                {
                    if (streamReader != null)
                    {
                        resultString = streamReader.ReadToEnd();
                    }
                }

                throw new ApplicationCrashException(string.Format(responseFormat, resultString), ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationCrashException("Failed while reading the Http response in the callback", ex);
            }
        }

        private void DrawItemsFromWootData(string result, WootType wootType,
            Panorama panoramaControl, AutoResetEvent resetEvent)
        {
            bool foundDocType;
            WootData wootData = WootAdapter.ExtractAllWootData(result, out foundDocType);
            Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    var productImage = new BitmapImage();
                    productImage.CreateOptions = BitmapCreateOptions.None;
                    productImage.UriSource = wootData.ProductImageUri;

                    var productDetailImage = new BitmapImage(wootData.ProductDetailImageUri);

                    wootData.WootType = wootType.ToString();

                    WootControl wootControl = new WootControl();
                    wootControl.DataContext = wootData;
                    wootData.ProductDescription = FixupDescriptionString(wootData.ProductDescription, wootData.ProductSubtitle, wootData.ProductTeaser,
                        ((SolidColorBrush)panoramaControl.Background).Color.ToString());
                    wootData.ProductImage = productImage;
                    wootData.ProductDetailImage = productDetailImage;

                    PanoramaItem panItem;
                    if (!wootDictionary.TryGetValue(wootType, out panItem))
                    {

                        panItem = new PanoramaItem()
                        {
                            Name = UriDictionary.GetNameForWoot(wootType),
                            Header = new TextBlock
                            {
                                Text = UriDictionary.GetNameForWoot(wootType),
                                Margin = new Thickness(0, 0, 0, -20),
                                FontFamily = new FontFamily("Garamond"),
                            },
                            Content = wootControl,
                        };
                    }
                    else
                    {
                        panItem.Content = wootControl;
                    }
                    wootDictionary[wootType] = panItem;
                }
                catch (Exception ex)
                {
                    throw new ApplicationCrashException("Failed while drawing items from woot data.", ex);
                }
            });
            resetEvent.Set();

            if (foundDocType)
            {
                var attributes = new Dictionary<string, string>();
                attributes.Add(UserTrackingEvents.AttributeNames.WootType, wootType.ToString());
                if (!string.IsNullOrWhiteSpace(result))
                {
                    attributes.Add(UserTrackingEvents.AttributeNames.ResponseLine, 
                        result.Length >= UserTrackingEvents.SubStringLength ? result.Substring(0, UserTrackingEvents.SubStringLength) : result);
                }
                AppSession.TagWooterEvent(UserTrackingEvents.DocTypeFound, attributes);
            }
        }

        public void RefreshWootData(string wootTypeStr)
        {
            OpenSplashPopup();

            WootType wootType = (WootType)Enum.Parse(typeof(WootType), wootTypeStr, true);
            eventsArray[Convert.ToInt32(wootType)].Reset();
            AddWootPivotItem(wootType);
            ThreadPool.QueueUserWorkItem((state) =>
            {
                eventsArray[Convert.ToInt32(wootType)].WaitOne();
                Dispatcher.BeginInvoke(() =>
                {
                    CloseSplashPopup();

                });
            });

            var attributes = new Dictionary<string, string>();
            attributes.Add(UserTrackingEvents.AttributeNames.WootType, wootTypeStr);
            AppSession.TagWooterEvent(UserTrackingEvents.RefreshPressed, attributes);
        }

        private void CloseSplashPopup()
        {
            this.splashPopup.IsOpen = false;
        }

        private void OpenSplashPopup()
        {
            var child = this.splashPopup.Child as PopupSplash;
            child.image1.Visibility = System.Windows.Visibility.Collapsed;
            child.LayoutRoot.Opacity = 0.8;
            child.LayoutRoot.Background = new SolidColorBrush(Color.FromArgb(200, 127, 127, 127));

            this.splashPopup.IsOpen = true;
        }

        private static string FixupDescriptionString(string description, string subtitle, string teaser, string bgcolor)
        {
            var culture = new CultureInfo("en-us");
            string htmlbg = "#91AB62";
            subtitle = subtitle.ToUpper();

            string finaldesc = string.Format(culture, "<html><body bgcolor={0}><b>{1}</b><br/><br/><font size=4 color=\"White\">{2}</font><br/><br/><p>{3}</p><script type='text/javascript'>{4}</script></body></html>",
                htmlbg, subtitle, teaser, description, SCRIPT);
            return finaldesc;
        }

        public class WootPivotData
        {
            public Panorama PanoramaControl;
            public WootType WootType;
            public AutoResetEvent ResetEvent;
        }
    }
}
