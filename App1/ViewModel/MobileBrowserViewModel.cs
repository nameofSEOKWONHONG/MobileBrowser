using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Newtonsoft.Json.Linq;
using SettingsUI.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using WinRT.Interop;
using static App1.App;

namespace App1.ViewModel
{
    public class AgentConst
    {
        public static readonly string MOBILE_AGENT = "Mozilla/5.0 (Linux; Android 13) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.5735.130 Mobile Safari/537.36";
        public static readonly string DESKTOP_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36 Edg/114.0.1823.51";
    }

    public class MobileBrowserViewModel : BindableBase
    {
        #region [private]

        private readonly string _home = "https://google.com";

        private bool _cangoback;
        private bool _cangoforward;
        private string _searchUrl = "https://google.com";

        #endregion [private]

        #region [public]

        public bool CanGoBack { get => _cangoback; set => this.SetProperty(ref this._cangoback, value); }
        public bool CanGoForward { get => _cangoforward; set => this.SetProperty(ref this._cangoforward, value); }
        public string SearchUrl { get => _searchUrl; set => this.SetProperty(ref this._searchUrl, value); }

        #endregion [public]

        public MobileBrowserViewModel()
        {
        }

        #region [command and event]

        public ICommand BackCommand => new RelayCommand<WebView2>(BackCommand_Execute);

        private void BackCommand_Execute(WebView2 webView) => webView.GoBack();

        public ICommand ForwardCommand => new RelayCommand<WebView2>(ForwardCommand_Execute);

        private void ForwardCommand_Execute(WebView2 webView) => webView.GoForward();

        public ICommand HomeCommand => new RelayCommand<WebView2>(HomeCommand_Execute);

        private void HomeCommand_Execute(WebView2 webView) => webView.Source = new Uri(_home);

        public ICommand SearchCommand => new RelayCommand<WebView2>(SearchCommand_Execute);

        private void SearchCommand_Execute(WebView2 webView)
        {
            if (AllowDesktopUrl(this._searchUrl))
            {
                webView.CoreWebView2.Settings.UserAgent = AgentConst.DESKTOP_AGENT;
            }
            else
            {
                webView.CoreWebView2.Settings.UserAgent = AgentConst.MOBILE_AGENT;
            }

            if (!HasHttpKeyword(this._searchUrl, out string convertedSearchUrl))
            {
                webView.Source = new Uri(convertedSearchUrl);
                return;
            }
            webView.Source = new Uri(this._searchUrl);
        }

        public ICommand PrintCommand => new RelayCommand<WebView2>(PrintCommand_Execute);

        private async void PrintCommand_Execute(WebView2 webView) => await webView.CoreWebView2.ExecuteScriptAsync("window.print()");

        public ICommand CaptureCommand => new RelayCommand<WebView2>(CaptureCommand_Execute);

        private async void CaptureCommand_Execute(WebView2 webView) => await this.CaptureAsync(webView);

        public ICommand SwitchAgentCommand => new RelayCommand<WebView2>(SwitchAgentCommand_Execute);

        private void SwitchAgentCommand_Execute(WebView2 webView) => this.SwitchAgent(webView);

        private void SwitchAgent(WebView2 webView)
        {
            if (webView.CoreWebView2.Settings.UserAgent == AgentConst.MOBILE_AGENT)
            {
                webView.CoreWebView2.Settings.UserAgent = AgentConst.DESKTOP_AGENT;
            }
            else
            {
                webView.CoreWebView2.Settings.UserAgent = AgentConst.MOBILE_AGENT;
            }
            webView.CoreWebView2.Reload();
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
                setting.UserAgent = AgentConst.DESKTOP_AGENT;
            }
        }

        #endregion [command and event]

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

        private async Task CaptureAsync(WebView2 webView)
        {
            var savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            savePicker.FileTypeChoices.Add("Image", new List<string>() { ".png" });
            savePicker.SuggestedFileName = "Card" + DateTime.Now.ToString("yyyyMMddhhmmss");

            InitializeWithWindow.Initialize(savePicker, AppNativeInfo.Instance.MainWindowHandle);
            StorageFile savefile = await savePicker.PickSaveFileAsync();
            if (savefile == null)
                return;

            using (IRandomAccessStream randomAccessStream = await savefile.OpenAsync(FileAccessMode.ReadWrite))
            {
                await webView.CoreWebView2.CapturePreviewAsync(Microsoft.Web.WebView2.Core.CoreWebView2CapturePreviewImageFormat.Png, randomAccessStream);
            }
        }

        #endregion [private method]
    }
}