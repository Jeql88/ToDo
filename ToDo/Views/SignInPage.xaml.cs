using System;
using Microsoft.Maui.Controls;

namespace ToDo.Views
{
    public partial class SignInPage : ContentPage
    {
        public SignInPage()
        {
            InitializeComponent();
        }

        private async void OnSignUpTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SignUpPage());
        }
        
        private async void OnSignInClicked(object sender, EventArgs e)
        {
            Console.WriteLine("Sign-In button clicked");
            await Navigation.PushAsync(new ToDoTab());
        }
    }
}