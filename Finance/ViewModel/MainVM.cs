using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using Finance.Model;
using Microsoft.AppCenter.Crashes;

namespace Finance.ViewModel
{
    public class MainVM : INotifyPropertyChanged
    {
        public Posts Blog
        {
            get;
            set;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainVM()
        {
            ReadRss();
        }

        public void ReadRss()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Posts));

                using (WebClient client = new WebClient())
                {
                    string xml = Encoding.Default.GetString(client.DownloadData("https://www.finzen.mx/blog-feed.xml"));
                    using (Stream reader = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
                    {
                        Blog = (Posts)serializer.Deserialize(reader);
                    }
                }
            }
            catch(Exception ex)
            {
                var infoDictionary = new Dictionary<string, string>
                {
                    {"RSS", "Reading RSS"}
                };
                Crashes.TrackError(ex, infoDictionary);
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
