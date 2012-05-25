using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Wooter.Analytics;
using System.Collections.Generic;


namespace Wooter
{
    internal enum ImageOnDisplay : uint
    {
        ProductImage = 0,
        DetailedImage = 1,
        AdditionalImage = 2,
    }

    public partial class WootControl : UserControl
    {
        private LocalyticsSession AppSession
        {
            get
            {
                return ((App)Application.Current).appSession;
            }
        }

        private readonly WebBrowserTask task = new WebBrowserTask();

        private ImageOnDisplay imageOnDisplay = ImageOnDisplay.ProductImage;

        private Regex regex = new Regex("javascript:lightboxImage[(]'(.*)'[)]", RegexOptions.Compiled);

        public WootControl()
        {
            // Required to initialize variables
            InitializeComponent();
            //DescriptionBrowser.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void BrowserLoaded(object sender, RoutedEventArgs e)
        {
            DescriptionBrowser.NavigateToString(DescriptionHtml.Text);
            //DescriptionBrowser.Visibility = System.Windows.Visibility.Visible;
        }

        private void OnWantOneButtonClick(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                if (this.PurchaseUri != null && !string.IsNullOrEmpty(this.PurchaseUri.Text))
                {
                    var uri = new Uri(PurchaseUri.Text);
                    task.Uri = uri;
                    task.Show();
                }
            }
            catch
            {
            }
        }

        private void BrowserScriptNotify(object sender, NotifyEventArgs e)
        {
            string wooterMarker = "wooterimage";
            var url = e.Value;
            Match match = this.regex.Match(url);

            if (url.StartsWith(wooterMarker))
            {
                url = url.Substring(wooterMarker.Length);
                DisplayAdditionalImage(url);
            }
            else if (match.Success)
            {
                url = match.Groups[1].Value;
                DisplayAdditionalImage(url);
            }
            else
            {
                try
                {
                    task.Uri = new Uri(url);
                    task.Show();
                }
                catch
                {
                }
            }
        }

        private void DisplayAdditionalImage(string url)
        {
            this.ProductImage.Source = new BitmapImage(new Uri(url));
            this.imageOnDisplay = ImageOnDisplay.AdditionalImage;
            this.MainScroller.ScrollToVerticalOffset(0);
        }

        private void OnImageTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (imageOnDisplay == ImageOnDisplay.ProductImage)
            {
                ProductImage.Source = DetailedImageTextBlock.Source;
                imageOnDisplay = ImageOnDisplay.DetailedImage;
            }
            else
            {
                ProductImage.Source = ProductImageTextBlock.Source;
                imageOnDisplay = ImageOnDisplay.ProductImage;
            }
        }

        private void OnWootControlLoaded(object sender, RoutedEventArgs e)
        {
            if (this.IsWootOff.Text.Equals("True", StringComparison.OrdinalIgnoreCase))
            {
                this.WootOffImage1.Visibility = System.Windows.Visibility.Visible;
                this.WootOffImage2.Visibility = System.Windows.Visibility.Visible;
                this.SoldOutTextBlock.Visibility = System.Windows.Visibility.Visible;
                this.SoldOutRectangle.Visibility = System.Windows.Visibility.Visible;
                this.OuterRectangle.Visibility = System.Windows.Visibility.Visible;
                this.RefreshButton.Visibility = System.Windows.Visibility.Visible;

                double soldoutFraction = Convert.ToDouble(this.RawSoldOutTextBlock.Text);
                this.SoldOutRectangle.Width = (336 * (1 - soldoutFraction));
                this.SoldOutTextBlock.Text = soldoutFraction != 1 ? string.Format("Woot Off! {0}% Left", (100 * (1-soldoutFraction)).ToString("#.00")) : "Sold Out!";
            }
        }

        private void OnRefreshTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var panoramaItem = this.Parent as PanoramaItem;
            var panorama = panoramaItem.Parent as Panorama;
            var stackpanel = panorama.Parent as StackPanel;
            var mainPage = stackpanel.Parent as MainPage;
            mainPage.RefreshWootData(this.WootType.Text);
        }
    }
}