
namespace FrontFeedProcessor
{
	public partial class SettingsPage : ContentPage
	{
        public string foo;
		public SettingsPage()
		{
			InitializeComponent();
            LoadPreferences();
		}

        private async void GpgPathButton_Clicked(object sender, EventArgs e)
        {
            FileResult fileResult = await FilePicker.Default.PickAsync();
            if(fileResult != null)
            {
                GpgPathData.Text = fileResult.FullPath;
                Preferences.Default.Set("gpg_path", fileResult.FullPath);
            }
        }

        private async void PriKeyPathButton_Clicked(object sender, EventArgs e)
        {
            FileResult fileResult = await FilePicker.Default.PickAsync();
            if (fileResult != null)
            {
                PriKeyPathData.Text = fileResult.FullPath;
                Preferences.Default.Set("pri_key", fileResult.FullPath);
            }
        }

        private async void PubKeyPathButton_Clicked(object sender, EventArgs e)
        {
            FileResult fileResult = await FilePicker.Default.PickAsync();
            if (fileResult != null)
            {
                PubKeyPathData.Text = fileResult.FullPath;
                Preferences.Default.Set("pub_key", fileResult.FullPath);
            }
        }
        private async void SecretStringButon_Clicked(object sender, EventArgs e)
        {
            string secret = await DisplayPromptAsync("Secret String", "Enter Secret String.", "Save", initialValue: Preferences.Default.Get("secret_string", "Unassigned"));
            if (secret != null && secret != "Unassigned")
            {
                Preferences.Default.Set("secret_string", secret);
                SecretStringData.Text = "**********";
            }
        }
        private void LoadPreferences()
        {
            GpgPathData.Text = Preferences.Get("gpg_path","Unassigned");
            PriKeyPathData.Text = Preferences.Get("pri_key", "Unassigned");
            PubKeyPathData.Text = Preferences.Get("pub_key", "Unassigned");
            string secret = Preferences.Get("secret_string", "Unassigned");
            if (secret == "Unassigned") SecretStringData.Text = "Unassigned";
            else SecretStringData.Text = "**********";
            LogPathData.Text = Preferences.Get("log_path", "Unassigned");
            ArchivePathData.Text = Preferences.Get("archive_path", "Unassigned");
        }

        private async void LogPathButton_Clicked(object sender, EventArgs e)
        {
            string logPath = await DisplayPromptAsync("Log Directory Path", "Enter Log Directory Path.", "Save", initialValue: Preferences.Default.Get("log_path", "Unassigned"));
            if (logPath != null && logPath != "Unassigned")
            {
                Preferences.Default.Set("log_path", logPath);
            }
        }

        private async void ArchivePathButton_Clicked(object sender, EventArgs e)
        {
            string archivePath = await DisplayPromptAsync("Archive Directory Path", "Enter Archive Directory Path.", "Save", initialValue: Preferences.Default.Get("archive_path", "Unassigned"));
            if (archivePath != null && archivePath != "Unassigned")
            {
                Preferences.Default.Set("archive_path", archivePath);
            }
        }
    }
}

