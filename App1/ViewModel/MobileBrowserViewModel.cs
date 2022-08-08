using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json.Linq;
using SettingsUI.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace App1.ViewModel
{
    public class MobileBrowserViewModel : BindableBase
    {
        #region [private]
        private readonly string _mobileHeader = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_2_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1 Edg/103.0.5060.134";
        private readonly string _desktopHeader = "Chrome";
        private readonly string _home = "https://google.com";

        private bool _cangoback;
        private bool _cangoforward;        
        private string _searchUrl = "https://google.com";
        #endregion

        #region [public]        
        public bool CanGoBack { get => _cangoback; set => this.SetProperty(ref this._cangoback, value); }
        public bool CanGoForward { get => _cangoforward; set => this.SetProperty(ref this._cangoforward, value); }
        public string SearchUrl { get => _searchUrl; set => this.SetProperty(ref this._searchUrl, value); }
        #endregion

        public MobileBrowserViewModel()
        {
        }

        #region [command and event]

        public ICommand BackCommand => new RelayCommand<WebView2>(BackCommand_Execute);
        private void BackCommand_Execute(WebView2 webView)
        {
            webView.GoBack();
        }

        public ICommand ForwardCommand => new RelayCommand<WebView2>(ForwardCommand_Execute);
        private void ForwardCommand_Execute(WebView2 webView)
        {
            webView.GoForward();
        }

        public ICommand HomeCommand => new RelayCommand<WebView2>(HomeCommand_Execute);
        private void HomeCommand_Execute(WebView2 webView)
        {
            webView.Source = new Uri(_home);
        }

        public ICommand SearchCommand => new RelayCommand<WebView2>(SearchCommand_Execute);
        private void SearchCommand_Execute(WebView2 webView)
        {
            if (AllowDesktopUrl(this._searchUrl))
            {
                webView.CoreWebView2.Settings.UserAgent = _desktopHeader;
            }
            else
            {
                webView.CoreWebView2.Settings.UserAgent = _mobileHeader;
            }

            if (!HasHttpKeyword(this._searchUrl, out string convertedSearchUrl))
            {
                webView.Source = new Uri(convertedSearchUrl);
                return;
            }
            webView.Source = new Uri(this._searchUrl);
        }

        public ICommand PrintCommand => new RelayCommand<WebView2>(PrintCommand_Execute);
        private async void PrintCommand_Execute(WebView2 webView)
        {
            await webView.CoreWebView2.ExecuteScriptAsync("window.print()");
        }

        public void SourceChanged(Microsoft.Web.WebView2.Core.CoreWebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2SourceChangedEventArgs args)
        {
            this.CanGoBack = sender.CanGoBack;
            this.CanGoForward = sender.CanGoForward;
        }

        public void NavigationStarting(Microsoft.Web.WebView2.Core.CoreWebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs args)
        {
            //cause google login
            if (new Uri(args.Uri).Host.Contains("accounts.google.com"))
            {
                var setting = sender.Settings;
                setting.UserAgent = _desktopHeader;
            }
        }
        #endregion

        #region [private method]

        private bool HasHttpKeyword(string url, out string completedUrl)
        {
            completedUrl = string.Empty;
            if (!url.Contains("http"))
            {
                completedUrl = $"http://{url}";
                return false;
            }
            return true;
        }

        /// <summary>
        /// TODO : 설정화 해야 함.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private bool AllowDesktopUrl(string url)
        {
            var allowPCUrls = new[]
            {
                "youtube"
            };

            var exists = allowPCUrls.FirstOrDefault(m => url.Contains(m));
            return exists != null;
        }

        private async void Capture(WebView2 webView)
        {
            string pic = await webView.CoreWebView2.CallDevToolsProtocolMethodAsync("Page.captureScreenshot", "{}");
            JObject o3 = JObject.Parse(pic);
            JToken data = o3["data"]!;

            byte[] bytes = Convert.FromBase64String(data.ToString());
            Image image = new Image();
            double picHeight = 0d;
            double picWidth = 0d;

            //using (MemoryStream stream = new MemoryStream(bytes))
            //{
            //    BitmapFrame bitmap = BitmapDecoder.CreateAsync().Frames[0];
            //    picHeight = bitmap.Height;
            //    picWidth = bitmap.Width;
            //    image.Source = bitmap;
            //}
            //TODO : 이미지 생성 후 저장 폴더 지정해야 함.
        }

        #endregion

    }
}
