namespace FrontFeedProcessor
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            if (Preferences.Default.Get("log_path", "Unassigned") == "Unassigned")
                Preferences.Default.Set("log_path", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "FrontFeedProcessor", "Logs"));
            if (Preferences.Default.Get("archive_path", "Unassigned") == "Unassigned")
                Preferences.Default.Set("archive_path", @"\\ZengerFTP01\FTP_ROOT\GoodRx\_incoming_data\Direct_Mail\Archive");
        }
    }
}
