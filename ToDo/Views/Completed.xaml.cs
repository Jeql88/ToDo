using System.Collections.ObjectModel;
using System.Text.Json;
using ToDo.Models;
using ToDo.Services;
using Microsoft.Maui.Controls;

namespace ToDo.Views;

public partial class Completed : ContentPage
{
    public ObservableCollection<TodoItem> CompletedItems { get; set; }
    public Command<TodoItem> EditCommand { get; }
    public Command<TodoItem> DeleteCommand { get; }
    public Command<TodoItem> ItemTappedCommand { get; }

    public Completed()
    {
        InitializeComponent();
        CompletedItems = new ObservableCollection<TodoItem>();
        EditCommand = new Command<TodoItem>(OnEditTapped);
        DeleteCommand = new Command<TodoItem>(OnDeleteTapped);
        ItemTappedCommand = new Command<TodoItem>(OnItemTapped);
        BindingContext = this;
        NavigationPage.SetHasBackButton(this, false);
        NavigationPage.SetHasNavigationBar(this, false);
        EditTaskPage.TaskChanged += async () => await LoadCompletedItems();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadCompletedItems();
    }

    private void ShowLoading()
    {
        MainThread.BeginInvokeOnMainThread(() => LoadingOverlay.IsVisible = true);
    }

    private void HideLoading()
    {
        MainThread.BeginInvokeOnMainThread(() => LoadingOverlay.IsVisible = false);
    }

    public static event Action? TasksChanged; // For cross-page refresh

    private async Task LoadCompletedItems()
    {
        ShowLoading();
        try
        {
            var apiService = new ApiService();
            var userId = UserSession.Instance.UserId;
            var response = await apiService.GetAsync($"/getItems_action.php?status=inactive&user_id={userId}");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var responseObject = JsonSerializer.Deserialize<GetToDoResponse>(response, options);

            CompletedItems.Clear();
            if (responseObject != null && responseObject.Status == 200 && responseObject.Data != null)
            {
                foreach (var item in responseObject.Data.Values)
                {
                    CompletedItems.Add(new TodoItem
                    {
                        ItemId = item.item_id,
                        Title = item.item_name,
                        Details = item.item_description,
                        IsCompleted = item.status == "inactive",
                        UserId = item.user_id,
                        Status = item.status,
                        Timemodified = item.dateTime_created
                    });
                }
            }
            else
            {
                await DisplayAlert("Error", responseObject?.Message ?? "Failed to load completed tasks.", "OK");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error: {ex}");
            await DisplayAlert("Error", "Failed to load completed tasks.", "OK");
        }
        finally
        {
            HideLoading();
        }
    }

    private async void OnEditTapped(TodoItem item) => await EditTask(item);
    private async void OnItemTapped(TodoItem item) => await EditTask(item);

    private async Task EditTask(TodoItem item)
    {
        if (item == null) return;
        var editPage = new EditTaskPage { BindingContext = item };
        EditTaskPage.TaskChanged = async () =>
        {
            await LoadCompletedItems();
            TasksChanged?.Invoke(); // Notify ToDoTab to refresh too
        };
        await Navigation.PushAsync(editPage);
    }

    private async void OnDeleteTapped(TodoItem item)
    {
        if (item == null) return;
        bool confirm = await DisplayAlert("Delete Task", "Are you sure you want to delete this task?", "Yes", "No");
        if (!confirm) return;

        ShowLoading();
        try
        {
            var apiService = new ApiService();
            var response = await apiService.DeleteAsync($"/deleteItem_action.php?item_id={item.ItemId}");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var responseObject = JsonSerializer.Deserialize<DeleteResponse>(response, options);

            if (responseObject != null && responseObject.Status == 200)
            {
                CompletedItems.Remove(item);
                await DisplayAlert("Success", responseObject.Message, "OK");
                TasksChanged?.Invoke(); // Notify ToDoTab to refresh
            }
            else
            {
                await DisplayAlert("Error", responseObject?.Message ?? "Failed to delete task.", "OK");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error: {ex}");
            await DisplayAlert("Error", "Failed to delete task.", "OK");
        }
        finally
        {
            HideLoading();
        }
    }

    private async void OnToDoTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ToDoTab());
    }

    private async void OnProfileTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ProfilePage());
    }

    private class GetToDoResponse
    {
        public int Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public Dictionary<string, ToDoItemData>? Data { get; set; }
    }

    private class ToDoItemData
    {
        public int item_id { get; set; }
        public string item_name { get; set; } = string.Empty;
        public string item_description { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public int user_id { get; set; }
        public string dateTime_created { get; set; } = string.Empty;
    }

    private class DeleteResponse
    {
        public int Status { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}