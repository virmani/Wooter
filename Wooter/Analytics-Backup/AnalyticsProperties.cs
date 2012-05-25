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
using Coding4Fun.Phone.Controls.Data;
using Microsoft.Phone.Info;

namespace Wooter.Analytics
{
    public static class AnalyticsProperties
    {
        public static string DeviceId
        {
            get
            {
                var value = (byte[])DeviceExtendedProperties.GetValue("DeviceUniqueId");
                return Convert.ToBase64String(value);
            }
        }

        public static string DeviceManufacturer
        {
            get { return DeviceExtendedProperties.GetValue("DeviceManufacturer").ToString(); }
        }

        public static string DeviceType
        {
            get { return DeviceExtendedProperties.GetValue("DeviceName").ToString(); }
        }

        public static string Device
        {
            get { return string.Format("{0} - {1}", DeviceManufacturer, DeviceType); }
        }

        public static string OsVersion
        {
            get { return string.Format("WP {0}", Environment.OSVersion.Version); }
        }

        public static string ApplicationVersion
        {
            get { return PhoneHelper.GetAppAttribute("Version").Replace(".0.0", ""); }
        }
    }
}
