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
using Google.WebAnalytics;
using Microsoft.WebAnalytics.Data;
using Microsoft.WebAnalytics;
using System.ComponentModel.Composition.Hosting;
using Microsoft.WebAnalytics.Behaviors;
using System.Collections.ObjectModel;

namespace Wooter.Analytics
{
    public class AnalyticsService : IApplicationService
    {
        private readonly IApplicationService _innerService;
        private readonly GoogleAnalytics _googleAnalytics;

        public AnalyticsService()
        {
            _googleAnalytics = new GoogleAnalytics();
            _googleAnalytics.CustomVariables.Add(new PropertyValue { PropertyName = "Device ID", Value = AnalyticsProperties.DeviceId });
            _googleAnalytics.CustomVariables.Add(new PropertyValue { PropertyName = "Application Version", Value = AnalyticsProperties.ApplicationVersion });
            _googleAnalytics.CustomVariables.Add(new PropertyValue { PropertyName = "Device OS", Value = AnalyticsProperties.OsVersion });
            _googleAnalytics.CustomVariables.Add(new PropertyValue { PropertyName = "Device", Value = AnalyticsProperties.Device });
            _innerService = new WebAnalyticsService
            {
                IsPageTrackingEnabled = false,
                Services = { _googleAnalytics, }
            };
        }

        public string WebPropertyId
        {
            get { return _googleAnalytics.WebPropertyId; }
            set { _googleAnalytics.WebPropertyId = value; }
        }

        #region IApplicationService Members

        public void StartService(ApplicationServiceContext context)
        {
            CompositionHost.Initialize(
                new AssemblyCatalog(
                    Application.Current.GetType().Assembly),
                new AssemblyCatalog(typeof(AnalyticsEvent).Assembly),
                new AssemblyCatalog(typeof(TrackAction).Assembly));
            _innerService.StartService(context);
        }

        public void StopService()
        {
            _innerService.StopService();
        }

        #endregion
    }
}
