using System;
using Microsoft.Maui.Controls;

namespace ToDo.Views
{
    public partial class SignUpPage : ContentPage
    {
        public SignUpPage()
        {
            InitializeComponent();
        }

        private async void OnSignInTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SignInPage());
        }
    }
}