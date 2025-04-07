using System.Collections.ObjectModel;

namespace ToDo.Views;

public partial class Completed : ContentPage
{
    public ObservableCollection<CompletedItem> CompletedItems { get; set; }

    public Completed()
    {
        InitializeComponent();
            
        // Initialize with static completed items
        CompletedItems = new ObservableCollection<CompletedItem>
        {
            new CompletedItem { Title = "Completed 1" },
            new CompletedItem { Title = "Completed 2" },
            new CompletedItem { Title = "Completed 3" },
            new CompletedItem { Title = "Completed 4" },
            new CompletedItem { Title = "Completed 5" }
        };
            
        BindingContext = this;
        
    }
    
    // Simple model for CompletedItem
    public class CompletedItem
    {
        public string Title { get; set; }
    }
}