using Microsoft.Extensions.DependencyInjection;

namespace FrontFeedProcessor
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new AppShell());
            window.Height = 600;
            window.Width = 1000;
            return window;
            //return new Window(new AppShell());
        }
    }
}