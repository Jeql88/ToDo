using System.Collections.ObjectModel;
using System.Text.Json;
using Microsoft.Maui.Controls;
using ToDo.Models;
using ToDo.Services;

namespace ToDo.Views
{
    public partial class ToDoTab : ContentPage
    {
        public ObservableCollection<TodoItem> TodoItems { get; set; }
        public Command AddTodoCommand { get; }
        public Command<TodoItem> ItemTappedCommand { get; }
        public Command<TodoItem> ChangeStatusCommand { get; }
        public Command<TodoItem> DeleteTodoCommand { get; }

        public ToDoTab()
        {
            InitializeComponent();

            TodoItems = new ObservableCollection<TodoItem>();

            AddTodoCommand = new Command<object>(OnAddTodoClicked);
            ItemTappedCommand = new Command<TodoItem>(OnItemTapped);
            ChangeStatusCommand = new Command<TodoItem>(OnChangeStatusClicked);
            DeleteTodoCommand = new Command<TodoItem>(OnDeleteTodoClicked);
            Completed.TasksChanged += async () => await LoadToDoItems("active");
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            NavigationPage.SetHasNavigationBar(this, false); // Hide the navigation bar
            await LoadToDoItems("active");
        }

        private void ShowLoading()
        {
            MainThread.BeginInvokeOnMainThread(() => LoadingOverlay.IsVisible = true);
        }

        private void HideLoading()
        {
            MainThread.BeginInvokeOnMainThread(() => LoadingOverlay.IsVisible = false);
        }

        private async Task LoadToDoItems(string status)
        {
            ShowLoading();
            try
            {
                var apiService = new ApiService();
                var userId = UserSession.Instance.UserId; // Get user_id from session
                var response = await apiService.GetAsync($"/getItems_action.php?status={status}&user_id={userId}");
                System.Diagnostics.Debug.WriteLine($"API Response: {response}"); // Log the API response

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var responseObject = JsonSerializer.Deserialize<GetToDoResponse>(response, options);

                if (responseObject != null && responseObject.Status == 200 && responseObject.Data != null)
                {
                    TodoItems.Clear();
                    foreach (var item in responseObject.Data.Values) // Iterate over the dictionary values
                    {
                        System.Diagnostics.Debug.WriteLine($"Mapping Item: ID={item.item_id}, Name={item.item_name}, Description={item.item_description}");
                        TodoItems.Add(new TodoItem
                        {
                            ItemId = item.item_id, // Correctly map "item_id"
                            Title = item.item_name, // Correctly map "item_name"
                            Details = item.item_description, // Correctly map "item_description"
                            IsCompleted = item.status == "inactive",
                            UserId = item.user_id,
                            Status = item.status,
                            Timemodified = item.dateTime_created // Correctly map "dateTime_created"
                        });
                    }
                }
                else
                {
                    await DisplayAlert("Error", responseObject?.Message ?? "Failed to load tasks.", "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex}");
                await DisplayAlert("Error", "Failed to load tasks.", "OK");
            }
            finally
            {
                HideLoading();
            }
        }

        private async void OnAddTodoClicked(object sender)
        {
            try
            {
                var addTaskPage = new AddTaskPage();
                addTaskPage.TaskAdded += (s, newItem) =>
                {
                    TodoItems.Add(newItem); // Add the new task to the list
                };

                await Navigation.PushAsync(addTaskPage);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex}");
                await DisplayAlert("Navigation Error", "Could not navigate to Add Task page. Please try again.", "OK");
            }
        }

        private async void OnItemTapped(TodoItem item)
        {
            if (item != null)
            {
                try
                {
                    // Navigate to EditTaskPage and pass the selected task
                    await Navigation.PushAsync(new EditTaskPage
                    {
                        BindingContext = item // Pass the selected task as the BindingContext
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Navigation error: {ex}");
                    await DisplayAlert("Error", "Could not navigate to the Edit Task page. Please try again.", "OK");
                }
            }
        }

        private async void OnChangeStatusClicked(TodoItem item)
        {
            ShowLoading();
            try
            {
                var apiService = new ApiService();
                var data = new
                {
                    status = item.IsCompleted ? "active" : "inactive",
                    item_id = item.ItemId
                };

                var response = await apiService.PutAsync("/statusItem_action.php", data);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var responseObject = JsonSerializer.Deserialize<ChangeStatusResponse>(response, options);

                if (responseObject != null && responseObject.Status == 200)
                {
                    TodoItems.Remove(item);
                    await DisplayAlert("Success", responseObject.Message, "OK");
                }
                else
                {
                    await DisplayAlert("Error", responseObject?.Message ?? "Failed to change status.", "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex}");
                await DisplayAlert("Error", "Failed to change status.", "OK");
            }
            finally
            {
                HideLoading();
            }
        }

        private async void OnDeleteTodoClicked(TodoItem item)
        {
            ShowLoading();
            try
            {
                bool confirm = await DisplayAlert("Delete Task", "Are you sure you want to delete this task?", "Yes", "No");
                if (!confirm) return;

                var apiService = new ApiService();
                var response = await apiService.DeleteAsync($"/deleteItem_action.php?item_id={item.ItemId}");
                System.Diagnostics.Debug.WriteLine($"Delete Response: {response}"); // Log the response

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var responseObject = JsonSerializer.Deserialize<DeleteResponse>(response, options);

                if (responseObject != null && responseObject.Status == 200)
                {
                    TodoItems.Remove(item);
                    await DisplayAlert("Success", responseObject.Message, "OK");
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

        private async void OnProfileTapped(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new ProfilePage());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex}");
                await DisplayAlert("Navigation Error", "Could not navigate to the Profile page. Please try again.", "OK");
            }
        }

        private async void OnCompleteTapped(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new Completed());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex}");
                await DisplayAlert("Navigation Error", "Could not navigate to the Completed page. Please try again.", "OK");
            }
        }

        private class DeleteResponse
        {
            public int Status { get; set; }
            public string Message { get; set; } = string.Empty;
        }

        private class GetToDoResponse
        {
            public int Status { get; set; }
            public string Message { get; set; } = string.Empty;
            public Dictionary<string, ToDoItemData>? Data { get; set; } // Dictionary with string keys
        }

        private class ToDoItemData
        {
            public int item_id { get; set; } // Matches "item_id" from the API
            public string item_name { get; set; } = string.Empty; // Matches "item_name" from the API
            public string item_description { get; set; } = string.Empty; // Matches "item_description" from the API
            public string status { get; set; } = string.Empty; // Matches "status" from the API
            public int user_id { get; set; } // Matches "user_id" from the API
            public string dateTime_created { get; set; } = string.Empty; // Matches "dateTime_created" from the API
        }

        private class ChangeStatusResponse
        {
            public int Status { get; set; }
            public string Message { get; set; } = string.Empty;
        }
    }
}