using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using ToDo.Services; // Add this namespace for UserSession

namespace ToDo.Views
{
    public partial class SignInPage : ContentPage
    {
        private const string BaseUrl = "https://todo-list.dcism.org";

        public SignInPage()
        {
            InitializeComponent();
        }

        private async void OnSignInClicked(object sender, EventArgs e)
        {
            var email = EmailEntry.Text;
            var password = PasswordEntry.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Error", "Email and password are required.", "OK");
                return;
            }

            using var client = new HttpClient();
            try
            {
                var response = await client.GetAsync($"{BaseUrl}/signin_action.php?email={email}&password={password}");
                var responseString = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"SignIn Response: {responseString}"); // Log response

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var responseObject = JsonSerializer.Deserialize<SignInResponse>(responseString, options);

                if (responseObject != null && responseObject.Status == 200)
                {
                    // Store user data in the session
                    UserSession.Instance.SetUserData(
                        responseObject.Data.Id,
                        responseObject.Data.Fname,
                        responseObject.Data.Lname,
                        responseObject.Data.Email
                    );

                    await DisplayAlert("Success", $"Welcome back, {responseObject.Data.Fname}!", "OK");

                    // Navigate to the main ToDo page
                    await Navigation.PushAsync(new ToDoTab());
                }
                else
                {
                    await DisplayAlert("Error", responseObject?.Message ?? "An error occurred.", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignIn Error: {ex.Message}"); // Log error
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        private async void OnSignUpTapped(object sender, EventArgs e)
        {
            // Navigate to the SignUpPage
            await Navigation.PushAsync(new SignUpPage());
        }

        private class SignInResponse
        {
            public int Status { get; set; }
            public string Message { get; set; } = string.Empty; // Initialize with default value
            public UserData? Data { get; set; } // Mark as nullable
        }

        private class UserData
        {
            public int Id { get; set; }
            public string Fname { get; set; } = string.Empty; // Initialize with default value
            public string Lname { get; set; } = string.Empty; // Initialize with default value
            public string Email { get; set; } = string.Empty; // Initialize with default value
            public string Timemodified { get; set; } = string.Empty; // Initialize with default value
        }
    }
}