using System.Text.Json;
using ToDo.Services;
using ToDo.Models;

namespace ToDo.Views;

public partial class EditTaskPage : ContentPage
{
    private TodoItem _todoItem;
    public static Action? TaskChanged;

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

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is TodoItem item)
        {
            _todoItem = item;
            LoadTodoItem();
        }
        else
        {
            DisplayAlert("Error", "Task data is not loaded. Please try again.", "OK");
        }
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

    private void ShowLoading()
    {
        MainThread.BeginInvokeOnMainThread(() => LoadingOverlay.IsVisible = true);
    }

    private void HideLoading()
    {
        MainThread.BeginInvokeOnMainThread(() => LoadingOverlay.IsVisible = false);
    }

    private async void OnConfirmClicked(object sender, EventArgs e)
    {
        ShowLoading();
        try
        {
            if (string.IsNullOrWhiteSpace(TitleEntry.Text))
            {
                await DisplayAlert("Validation Error", "Title is required", "OK");
                return;
            }

            _todoItem.Title = TitleEntry.Text?.Trim();
            _todoItem.Details = DetailsEditor?.Text?.Trim();
            bool newIsCompleted = CompletedCheckBox?.IsChecked ?? false;

            var apiService = new ApiService();

            // 1. Always update title/details
            var editData = new
            {
                item_id = _todoItem.ItemId,
                item_name = _todoItem.Title,
                item_description = _todoItem.Details,
                user_id = _todoItem.UserId
            };
            await apiService.PutAsync("/editItem_action.php", editData);

            // 2. Always update status (ensure correct key and value)
            string newStatus = newIsCompleted ? "inactive" : "active";
            var statusData = new
            {
                status = newStatus,
                item_id = _todoItem.ItemId
            };
            await apiService.PutAsync("/statusItem_action.php", statusData);

            _todoItem.IsCompleted = newIsCompleted;
            _todoItem.Status = newStatus;

            TaskChanged?.Invoke();
            await DisplayAlert("Success", "Task updated successfully!", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error: {ex}");
            await DisplayAlert("Error", "Could not save changes. Please try again.", "OK");
        }
        finally
        {
            HideLoading();
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        ShowLoading();
        try
        {
            bool answer = await DisplayAlert("Delete Task", "Are you sure you want to delete this task?", "Yes", "No");
            if (answer)
            {
                var apiService = new ApiService();
                var response = await apiService.DeleteAsync($"/deleteItem_action.php?item_id={_todoItem.ItemId}");
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var responseObject = JsonSerializer.Deserialize<DeleteResponse>(response, options);

                if (responseObject != null && responseObject.Status == 200)
                {
                    TaskChanged?.Invoke();
                    await DisplayAlert("Success", "Task deleted successfully!", "OK");
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Error", responseObject?.Message ?? "Failed to delete task.", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error: {ex}");
            await DisplayAlert("Error", "Could not delete task. Please try again.", "OK");
        }
        finally
        {
            HideLoading();
        }
    }

    private class DeleteResponse
    {
        public int Status { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}