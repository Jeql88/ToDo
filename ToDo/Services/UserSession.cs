public class UserSession
{
    private static UserSession? _instance; // Mark as nullable

    public static UserSession Instance => _instance ??= new UserSession();

    public int UserId { get; private set; }
    public string FirstName { get; private set; } = string.Empty; // Initialize with default value
    public string LastName { get; private set; } = string.Empty; // Initialize with default value
    public string Email { get; private set; } = string.Empty; // Initialize with default value

    private UserSession() { }

    public void SetUserData(int userId, string firstName, string lastName, string email)
    {
        UserId = userId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    public void ClearSession()
    {
        UserId = 0;
        FirstName = string.Empty; // Reset to default value
        LastName = string.Empty; // Reset to default value
        Email = string.Empty; // Reset to default value
    }
}