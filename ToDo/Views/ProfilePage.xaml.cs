using ToDo.Services;

namespace ToDo.Views
{
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            NavigationPage.SetHasBackButton(this, false);

            ShowLoading();
            try
            {
                // Get user info from session (set at sign-in)
                NameLabel.Text = $"{UserSession.Instance.FirstName} {UserSession.Instance.LastName}";
                EmailLabel.Text = UserSession.Instance.Email;
            }
            catch
            {
                NameLabel.Text = "Unknown";
                EmailLabel.Text = "Unknown";
            }
            finally
            {
                HideLoading();
            }
        }

        private void ShowLoading()
        {
            MainThread.BeginInvokeOnMainThread(() => LoadingOverlay.IsVisible = true);
        }

        private void HideLoading()
        {
            MainThread.BeginInvokeOnMainThread(() => LoadingOverlay.IsVisible = false);
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Logout", "You have been logged out.", "OK");
            await Navigation.PopToRootAsync();
        }

        private async void OnCompleteTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Completed());
        }

        private async void OnToDoTapped(object? sender, EventArgs e)
        {
            await Navigation.PushAsync(new ToDoTab());
        }
    }
}