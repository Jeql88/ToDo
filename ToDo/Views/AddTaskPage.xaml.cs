using Microsoft.Maui.Controls;
using ToDo.Models;

namespace ToDo.Views;

public partial class AddTaskPage : ContentPage
{
    public AddTaskPage()
    {
        InitializeComponent();
    }

    private async void OnCreateClicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(TitleEntry.Text))
            {
                await DisplayAlert("Validation Error", "Title is required", "OK");
                return;
            }

            var newItem = new TodoItem
            {
                Title = TitleEntry.Text,
                Details = DetailsEditor.Text,
                IsCompleted = false
            };

            // Get the ToDoTab page from the navigation stack
            var todoTab = Navigation.NavigationStack
                .FirstOrDefault(p => p is ToDoTab) as ToDoTab;

            if (todoTab != null)
            {
                todoTab.TodoItems.Add(newItem);
            }

            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Navigation error: {ex}");
            await DisplayAlert("Error", "Could not create task. Please try again.", "OK");
        }
    }
} 