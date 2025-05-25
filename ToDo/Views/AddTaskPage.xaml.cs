using Microsoft.Maui.Controls;
using System.Text.Json;
using ToDo.Models;
using ToDo.Services;

namespace ToDo.Views;

public partial class AddTaskPage : ContentPage
{
    public event EventHandler<TodoItem>? TaskAdded;

    public AddTaskPage()
    {
        InitializeComponent();
    }

    private async void OnCreateClicked(object sender, EventArgs e)
    {
        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(TitleEntry.Text))
            {
                await DisplayAlert("Validation Error", "Title is required", "OK");
                return;
            }

            // Prepare the data to send to the backend
            var newItem = new
            {
                item_name = TitleEntry.Text?.Trim(),
                item_description = DetailsEditor.Text?.Trim(),
                user_id = UserSession.Instance.UserId
            };

            // Log the data being sent
            System.Diagnostics.Debug.WriteLine($"New Item: {JsonSerializer.Serialize(newItem)}");

            // Send the data to the backend
            var apiService = new ApiService();
            var response = await apiService.PostAsync("/addItem_action.php", newItem);

            // Log the response
            System.Diagnostics.Debug.WriteLine($"API Response: {response}");

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var responseObject = JsonSerializer.Deserialize<AddToDoResponse>(response, options);

            // Handle the response
            if (responseObject != null && responseObject.Status == 200 && responseObject.Data != null)
            {
                await DisplayAlert("Success", "Task added successfully!", "OK");

                // Raise the TaskAdded event to notify the parent page
                TaskAdded?.Invoke(this, new TodoItem
                {
                    ItemId = responseObject.Data.ItemId,
                    Title = responseObject.Data.ItemName,
                    Details = responseObject.Data.ItemDescription,
                    IsCompleted = responseObject.Data.Status == "inactive",
                    UserId = responseObject.Data.UserId,
                    Status = responseObject.Data.Status,
                    Timemodified = responseObject.Data.Timemodified
                });

                // Navigate back to the previous page
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", responseObject?.Message ?? "Failed to add task.", "OK");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error: {ex}");
            await DisplayAlert("Error", "Could not create task. Please try again.", "OK");
        }
    }

    private class AddToDoResponse
    {
        public int Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public AddToDoData? Data { get; set; }

        public class AddToDoData
        {
            public int ItemId { get; set; }
            public string ItemName { get; set; } = string.Empty;
            public string ItemDescription { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public int UserId { get; set; }
            public string Timemodified { get; set; } = string.Empty;
        }
    }
}