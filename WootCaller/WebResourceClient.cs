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
using System.Text;

namespace WootCaller
{
    public class WebResourceClient
    {
        public static void GetResourceFromWeb(Uri uri, DownloadStringCompletedEventHandler onDownloadStringCompleted, object userToken)
        {
            WebClient client = new WebClient();
            client.DownloadStringCompleted += onDownloadStringCompleted;
            client.DownloadStringAsync(uri, userToken);
        }

        public static void GetResourceFromWeb(Uri uri,
            AsyncCallback requestCompletedCallback,
            object userToken)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.BeginGetResponse(
                requestCompletedCallback, 
                new WebRequestToken
                {
                    UserToken = userToken,
                    Request = request,
                }
            );
        }
    }
}
