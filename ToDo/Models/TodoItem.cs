namespace ToDo.Models
{
    public class TodoItem
    {
        public int ItemId { get; set; } // Unique identifier for the task
        public string Title { get; set; } = string.Empty; // Task title
        public string Details { get; set; } = string.Empty; // Task description
        public bool IsCompleted { get; set; } // Indicates if the task is completed
        public int UserId { get; set; } // ID of the user who created the task
        public string Status { get; set; } = "active"; // Task status (active/inactive)
        public string Timemodified { get; set; } = string.Empty; // Last modified timestamp
    }
}