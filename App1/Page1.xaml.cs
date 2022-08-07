using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Newtonsoft.Json.Linq;
using SettingsUI.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using WinRT;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Page1 : Page
    {
        private readonly Page1ViewModel _viewModel;

        public Page1()
        {            
            this.InitializeComponent();
            _viewModel = new Page1ViewModel();
            this.DataContext = _viewModel;
            this.webView.Loaded += async (s, e) =>
            {
                var webView = s as WebView2;
                await webView.EnsureCoreWebView2Async();
                var settings = webView.CoreWebView2.Settings;
                settings.AreDefaultScriptDialogsEnabled = true;
                settings.AreDefaultContextMenusEnabled = true;
                settings.AreDevToolsEnabled = true;
                settings.IsScriptEnabled = true;

                this.webView.CoreWebView2.SourceChanged += _viewModel.SourceChanged;
                this.webView.CoreWebView2.NavigationStarting += _viewModel.NavigationStarting;
                this.SearchUrl.KeyUp += (s, e) =>
                {
                    if(e.Key == Windows.System.VirtualKey.Enter)
                    {
                        this._viewModel.SearchCommand.Execute(this.webView);
                    }
                };
            };            
        }

        //private void Button_PointerEntered(object sender, PointerRoutedEventArgs e)
        //{
        //    AnimatedIcon.SetState(this.SearchAnimatedIcon, "PointerOver");
        //}

        //private void Button_PointerExited(object sender, PointerRoutedEventArgs e)
        //{
        //    AnimatedIcon.SetState(this.SearchAnimatedIcon, "Normal");
        //}
    }

    public class Page1ViewModel : BindableBase
    {
        public string RootDomain { get => "https://google.com"; } 
        private readonly string _mobileHeader = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_2_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1 Edg/103.0.5060.134";
        private readonly string _desktopHeader = "Chrome";
        private bool _cangoback;
        private bool _cangoforward;
        public bool CanGoBack { get => _cangoback; set => this.SetProperty(ref this._cangoback, value); }
        public bool CanGoForward { get => _cangoforward; set => this.SetProperty(ref this._cangoforward, value); }
        private string _searchUrl = "https://google.com";
        public string SearchUrl { get => _searchUrl; set => this.SetProperty(ref this._searchUrl, value); }

        public Page1ViewModel()
        {
        }

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
            webView.Source = new Uri(RootDomain);
        }

        public ICommand SearchCommand => new RelayCommand<WebView2>(SearchCommand_Execute);
        private void SearchCommand_Execute(WebView2 webView)
        {
            if(AllowDesktopUrl(this._searchUrl))
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
    }

    public class WebViewSearchModel
    {
        public WebView2 WebView { get; set; }
        public string SearchUrl { get; set; }
    }
}
