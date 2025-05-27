using Microsoft.Maui.Controls;

namespace CogtiveDevAssignment
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }
    }
}