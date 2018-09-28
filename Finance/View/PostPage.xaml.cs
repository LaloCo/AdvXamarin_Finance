using System;
using System.Collections.Generic;
using Finance.Model;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;

namespace Finance.View
{
    public partial class PostPage : ContentPage
    {
        public PostPage()
        {
            InitializeComponent();
            Xamarin.Forms.PlatformConfiguration.iOSSpecific.Page.SetUseSafeArea(this, true);
        }

        public PostPage(Item item)
        {
            InitializeComponent();
            Xamarin.Forms.PlatformConfiguration.iOSSpecific.Page.SetUseSafeArea(this, true);

            try
            {
                throw (new Exception("Unable to load blog"));

                webView.Source = item.ItemLink;
            }
            catch(Exception ex)
            {
                var properties = new Dictionary<string, string>
                {
                    {"Blog_Post", $"{item.Title}"}
                };
                Crashes.TrackError(ex, properties);
            }
        }
    }
}
