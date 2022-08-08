using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App1
{
    /*
     * TODO LIST
     * 0. User setting start url.
     * 1. Screen Capture.
     * 2. Hide/Show url input & button.
     * 3. Share capture to sns, messenger.
     * 4. Share url to friends.
     */


    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {   
            this.InitializeComponent();
            // C# code to set AppTitleBar uielement as titlebar            
            this.ExtendsContentIntoTitleBar = true;  // enable custom titlebar
            this.SetTitleBar(AppTitleBar);      // set user ui element as titlebar
            this.SizeChanged += MainWindow_SizeChanged;
        }

        private void MainWindow_SizeChanged(object sender, WindowSizeChangedEventArgs args)
        {
            throw new System.NotImplementedException();
        }
    }


}
