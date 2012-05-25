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
using System.Collections.Generic;

namespace Wooter.Analytics
{
    public static class AnalyticsProperties
    {
        private static Dictionary<String, String> deviceAppAttributes;

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

        public static Dictionary<String, String> DeviceAppAttributes
        {
            get
            {
                if (deviceAppAttributes == null)
                {
                    deviceAppAttributes = new Dictionary<string, string>();
                    deviceAppAttributes.Add(UserTrackingEvents.AttributeNames.Device, Device);
                    deviceAppAttributes.Add(UserTrackingEvents.AttributeNames.DeviceType, DeviceType);
                    deviceAppAttributes.Add(UserTrackingEvents.AttributeNames.DeviceManufacturer, DeviceManufacturer);
                    deviceAppAttributes.Add(UserTrackingEvents.AttributeNames.OsVersion, OsVersion);
                    deviceAppAttributes.Add(UserTrackingEvents.AttributeNames.ApplicationVersion, ApplicationVersion);
                    deviceAppAttributes.Add(UserTrackingEvents.AttributeNames.DeviceId, DeviceId);
                }

                return deviceAppAttributes;
            }
        }
    }
}
