using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Wooter.Analytics;

namespace Wooter
{
    public partial class App : Application
    {
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        private AutoResetEvent messageBoxEvent = new AutoResetEvent(false);

        private bool messageBoxShown;

        private object messageBoxLock = new object();

        public LocalyticsSession appSession;

        //public AnalyticsTracker Tracker;

        private const string LocalyticsKey = "f7a6e5f4b903056b85fe311-ce8f6ae0-56db-11e1-aa62-008545fe83d2";

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            // Note that exceptions thrown by ApplicationBarItem.Click will not get caught here.
            UnhandledException += Application_UnhandledException;

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are being GPU accelerated with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;
            }

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();
        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            InitializeAndUploadTracking();
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            InitializeAndUploadTracking();
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            CloseTracking();
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            CloseTracking();
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }

            ShowMessageBoxIfNeeded(e.Exception);
            if (ShouldWatsonException(e.Exception))
            {
                LittleWatson.ReportException(e.Exception, "An Exception was thrown from RootFrame_NavigationFailed");
            }

            messageBoxEvent.WaitOne(2000);
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }

            ShowMessageBoxIfNeeded(e.ExceptionObject);
            if (ShouldWatsonException(e.ExceptionObject))
            {
                LittleWatson.ReportException(e.ExceptionObject, "An Exception was thrown from Application_UnhandledException");
            }

            messageBoxEvent.WaitOne(2000);
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        private void ShowMessageBoxIfNeeded(Exception e)
        {
            if (!messageBoxShown)
            {
                lock (messageBoxLock)
                {
                    if (!messageBoxShown)
                    {
                        messageBoxEvent.Reset();
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            MessageBox.Show("Either Woot is not behaving or your phone has lost internet connectivity. Please try again later.",
                                "Woot Not Responding",
                                MessageBoxButton.OK);
                            messageBoxEvent.Set();
                        });
                        messageBoxShown = true;
                    }
                }
            }

            SendExceptionToTracker(e);
        }

        private bool ShouldWatsonException(Exception e)
        {
            if (e != null && !(e is ExitApplicationException))
            {
                return true;
            }

            return false;
        }

        private void SendExceptionToTracker(Exception e)
        {
            var eventName = UserTrackingEvents.UnexpectedException;

            if (e is ApplicationCrashException)
            {
                eventName = UserTrackingEvents.ApplicationCrashException;
            }

            if (e is ExitApplicationException)
            {
                eventName = UserTrackingEvents.ExitApplicationException;
            }

            var attributes = CreateAttributesFromException(e);

            appSession.TagWooterEvent(eventName, attributes);

        }

        private Dictionary<String, String> CreateAttributesFromException(Exception e)
        {
            Dictionary<String, String> attributes = null;

            if (e != null)
            {
                attributes = new Dictionary<String, String>();

                attributes.Add(UserTrackingEvents.AttributeNames.ExceptionType, e.GetType().ToString());
                attributes.Add(UserTrackingEvents.AttributeNames.ExceptionMessage, e.Message);

                var inner = e.InnerException;
                if (inner != null)
                {
                    attributes.Add(UserTrackingEvents.AttributeNames.InnerExceptionType, inner.GetType().ToString());
                    attributes.Add(UserTrackingEvents.AttributeNames.InnerExceptionMessage, inner.Message);
                }
            }

            return attributes;
        }

        private void InitializeAndUploadTracking()
        {
            //Tracker = new AnalyticsTracker(); 

            appSession = new LocalyticsSession(LocalyticsKey);
            appSession.open();
            appSession.upload();
        }

        private void CloseTracking()
        {
            appSession.close();
        }

        #endregion
    }
}