using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace AzureCognitiveServices
{
    public sealed partial class MainPage : Page
    {
        
        public MainPage()
        {
            this.InitializeComponent();
        }

        public void SearchText_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            // If user presses Enter, read the search terms and use them to find an image.
            if(e.Key== Windows.System.VirtualKey.Enter && searchText.Text.Trim().Length > 0)
            {
                string imageURL = FindURLOfImage(searchText.Text);
                foundImage.Source = new BitmapImage(new Uri(imageURL, UriKind.Absolute));
            }
        }

        struct search_result{
            public String jsonResult;
            public Dictionary<String, String> headers_r;
        };

        public string FindURLOfImage(string st){

            // call the method that does the search
            search_result result = PerformBingImageSearch(st);
            JsonObject jsonObj = JsonObject.Parse(result.jsonResult);
            JsonArray results = jsonObj.GetNamedArray("value");
            if (results.Count > 0)
            {
                JsonObject first_result = results.GetObjectAt(0);
                String imageUrl = first_result.GetNamedString("contentUrl");
                return imageUrl;
            }
            else{
                return "https://cdn.techinasia.com/wp-content/uploads/2016/06/why-are-you-so-dumb.jpg";
            }
        }

        static search_result PerformBingImageSearch(string st){

            string uri_key = "5c9869dfcf7a41859de8b2e066543bfe";
            string endpoint = "https://mayank1234.cognitiveservices.azure.com/bing/v7.0/images/search";

            string uriQuery = endpoint + "?q=" + Uri.EscapeDataString(st);
            WebRequest request = WebRequest.Create(uriQuery);
            request.Headers["Ocp-Apim-Subscription-Key"] = uri_key;
            HttpWebResponse response = (HttpWebResponse)request.GetResponseAsync().Result;
            string json = new StreamReader(response.GetResponseStream()).ReadToEnd();
            var searchResult = new search_result()
            {
                jsonResult = json,
                headers_r = new Dictionary<String, String>()
            };

            // Extract the Bing HTTP headers.
            foreach (String header in response.Headers)
            {
                if (header.StartsWith("BingAPIs-") || header.StartsWith("X-MSEdge-"))
                    searchResult.headers_r[header] = response.Headers[header];
            }

            return searchResult;

        }
    }
}
