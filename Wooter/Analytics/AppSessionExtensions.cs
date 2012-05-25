using System.Collections.Generic;
using System.Linq;

namespace Wooter.Analytics
{
    public static class AppSessionExtensions
    {
        public static void TagWooterEvent(this LocalyticsSession appSession, string eventName, Dictionary<string, string> attributes = null)
        {
            var deviceAttributes = AnalyticsProperties.DeviceAppAttributes;
            var union = deviceAttributes;
            if (attributes != null)
            {
                union = deviceAttributes.Union(attributes).ToDictionary(pair => pair.Key, pair => pair.Value);
            }

            appSession.tagEvent(eventName, union);
        }
    }
}
