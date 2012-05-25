using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ProgressSplashScreen
{
    public partial class PopupSplash : UserControl
    {
        public PopupSplash()
        {
            InitializeComponent();
            //this.progressBar1.IsIndeterminate = true;
            this.performanceProgressBar1.IsIndeterminate = true;
        }
    }
}
