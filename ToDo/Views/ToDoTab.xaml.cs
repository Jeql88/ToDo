using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using ToDo.Models;

namespace ToDo.Views
{
    public partial class ToDoTab : ContentPage, IQueryAttributable
    {
        public ObservableCollection<TodoItem> TodoItems { get; set; }
        public Command AddTodoCommand { get; }
        public Command<TodoItem> ItemTappedCommand { get; }

        public ToDoTab()
        {
            InitializeComponent();
            
            // Initialize with static data
            TodoItems = new ObservableCollection<TodoItem>
            {
                new TodoItem { Title = "title 1", Details = "Sample details 1" },
                new TodoItem { Title = "title 2", Details = "Sample details 2" },
                new TodoItem { Title = "title 3", Details = "Sample details 3" },
                new TodoItem { Title = "title 4", Details = "Sample details 4" },
                new TodoItem { Title = "title 5", Details = "Sample details 5" }
            };
            
            AddTodoCommand = new Command(OnAddTodoClicked);
            ItemTappedCommand = new Command<TodoItem>(OnItemTapped);
            
            BindingContext = this;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("NewTodoItem") && query["NewTodoItem"] is TodoItem newItem)
            {
                TodoItems.Add(newItem);
            }
        }
        
        private async void OnAddTodoClicked()
        {
            try
            {
                await Navigation.PushAsync(new AddTaskPage());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex}");
                await DisplayAlert("Navigation Error", "Could not navigate to add task page. Please try again.", "OK");
            }
        }
        
        private async void OnItemTapped(TodoItem item)
        {
            if (item != null)
            {
                try
                {
                    var editPage = new EditTaskPage();
                    editPage.TodoItem = item;
                    await Navigation.PushAsync(editPage);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Navigation error: {ex}");
                    await DisplayAlert("Navigation Error", "Could not navigate to edit task page. Please try again.", "OK");
                }
            }
        }
        
        private async void OnCompleteTapped(object? sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new Completed());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex}");
                await DisplayAlert("Navigation Error", "Could not navigate to completed tasks. Please try again.", "OK");
            }
        }
    }
    
    // Enhanced model for TodoItem
    public class TodoItem
    {
        public string Title { get; set; }
        public string Details { get; set; }
        public bool IsCompleted { get; set; }
    }
}