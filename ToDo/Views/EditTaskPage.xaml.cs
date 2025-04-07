using Microsoft.Maui.Controls;
using ToDo.Models;

namespace ToDo.Views;

[QueryProperty(nameof(TodoItem), "TodoItem")]
public partial class EditTaskPage : ContentPage
{
    private TodoItem _todoItem;

    public TodoItem TodoItem
    {
        get => _todoItem;
        set
        {
            _todoItem = value;
            OnPropertyChanged();
            LoadTodoItem();
        }
    }

    public EditTaskPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    private void LoadTodoItem()
    {
        if (_todoItem != null)
        {
            TitleEntry.Text = _todoItem.Title;
            DetailsEditor.Text = _todoItem.Details;
            CompletedCheckBox.IsChecked = _todoItem.IsCompleted;
        }
    }

    private async void OnConfirmClicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(TitleEntry.Text))
            {
                await DisplayAlert("Validation Error", "Title is required", "OK");
                return;
            }

            _todoItem.Title = TitleEntry.Text;
            _todoItem.Details = DetailsEditor.Text;
            _todoItem.IsCompleted = CompletedCheckBox.IsChecked;

            await Shell.Current.GoToAsync("todo");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Navigation error: {ex}");
            await DisplayAlert("Error", "Could not save changes. Please try again.", "OK");
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        try
        {
            bool answer = await DisplayAlert("Delete Task", "Are you sure you want to delete this task?", "Yes", "No");
            if (answer)
            {
                await Shell.Current.GoToAsync("todo");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Navigation error: {ex}");
            await DisplayAlert("Error", "Could not delete task. Please try again.", "OK");
        }
    }
} 