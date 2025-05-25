using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace ToDo.Views
{
    public partial class SignUpPage : ContentPage
    {
        private const string BaseUrl = "https://todo-list.dcism.org";

        public SignUpPage()
        {
            InitializeComponent();
        }

        private async void OnSignUpClicked(object sender, EventArgs e)
        {
            var firstName = FirstNameEntry.Text;
            var lastName = LastNameEntry.Text;
            var email = EmailEntry.Text;
            var password = PasswordEntry.Text;
            var confirmPassword = ConfirmPasswordEntry.Text;

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                await DisplayAlert("Error", "All fields are required.", "OK");
                return;
            }

            if (password != confirmPassword)
            {
                await DisplayAlert("Error", "Passwords do not match.", "OK");
                return;
            }

            var signUpData = new
            {
                first_name = firstName,
                last_name = lastName,
                email = email,
                password = password,
                confirm_password = confirmPassword
            };

            var json = JsonSerializer.Serialize(signUpData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var client = new HttpClient();
            try
            {
                var response = await client.PostAsync($"{BaseUrl}/signup_action.php", content);
                var responseString = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"SignUp Response: {responseString}"); // Log response

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var responseObject = JsonSerializer.Deserialize<ApiResponse>(responseString, options);

                if (responseObject != null && responseObject.Status == 200)
                {
                    await DisplayAlert("Success", responseObject.Message, "OK");
                    await Navigation.PushAsync(new SignInPage());
                }
                else
                {
                    await DisplayAlert("Error", responseObject?.Message ?? "An error occurred.", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignUp Error: {ex.Message}"); // Log error
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        private async void OnSignInTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SignInPage());
        }

        private class ApiResponse
        {
            public int Status { get; set; }
            public string Message { get; set; }
        }
    }
}