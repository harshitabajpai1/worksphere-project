namespace AuthService.DTOs
{
    // This DTO contains the information required
    // when a logged-in user wants to change their password.
    public class ChangePasswordDto
    {
        // The user's current password.
        public string CurrentPassword { get; set; } = string.Empty;

        // The new password that the user wants to use.
        public string NewPassword { get; set; } = string.Empty;
    }
}