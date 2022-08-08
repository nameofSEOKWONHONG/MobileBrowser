using App1.ViewModel;
using Microsoft.UI.Xaml.Controls;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MobileBrowserPage : Page
    {
        private readonly MobileBrowserViewModel _viewModel;

        public MobileBrowserPage()
        {            
            this.InitializeComponent();
            _viewModel = new MobileBrowserViewModel();
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


}
