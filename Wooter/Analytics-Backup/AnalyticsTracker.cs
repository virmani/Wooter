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
using System.ComponentModel.Composition;
using Microsoft.WebAnalytics;

namespace Wooter.Analytics
{
    public class AnalyticsTracker
    {
        public AnalyticsTracker()
        {
            CompositionInitializer.SatisfyImports(this);
        }

        [Import("Log")]
        public Action<AnalyticsEvent> Log { get; set; }

        public void Track(string category, string name)
        {
            Track(category, name, null);
        }

        public void Track(string category, string name, string actionValue)
        {
            Log(new AnalyticsEvent { Category = category, Name = name, ObjectName = actionValue });
        }
    }
}
