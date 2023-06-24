using Microsoft.UI.Xaml;
using System;
using System.Runtime.InteropServices;
using WinRT;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App1
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private readonly int _width = 420;
        private readonly int _height = 820;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            var windowNative = m_window.As<IWindowNative>();
            m_windowHandle = windowNative.WindowHandle;
            AppNativeInfo.Instance.MainWindowHandle = m_windowHandle;
            m_window.Activate();            
            SetWindowSize(m_windowHandle, _width, _height);
            //WindowHelper.RegisterWindowMinMax(m_window);            
            //WindowHelper.MinWindowWidth = _width;
            //WindowHelper.MinWindowHeight = _height;            
            //m_window.SetWindowSize(400, 800);
        }

        private Window m_window;
        private IntPtr m_windowHandle;
        public IntPtr WindowHandle { get { return m_windowHandle; } }

        private void SetWindowSize(IntPtr hwnd, int width, int height)
        {
            var dpi = PInvoke.User32.GetDpiForWindow(hwnd);
            float scalingFactor = (float)dpi / 96;
            width = (int)(width * scalingFactor);
            height = (int)(height * scalingFactor);

            PInvoke.User32.SetWindowPos(hwnd, PInvoke.User32.SpecialWindowHandles.HWND_TOP,
                                        0, 0, width, height,
                                        PInvoke.User32.SetWindowPosFlags.SWP_NOMOVE);
        }



        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("EECDBF0E-BAE9-4CB6-A68E-9598E1CB57BB")]
        internal interface IWindowNative
        {
            IntPtr WindowHandle { get; }
        }
    }

    public class AppNativeInfo
    {
        public static AppNativeInfo Instance => _instance.Value;
        private static Lazy<AppNativeInfo> _instance = new Lazy<AppNativeInfo>(() => new AppNativeInfo());

        public IntPtr MainWindowHandle { get; set; }
        private AppNativeInfo() { }
    }
}
