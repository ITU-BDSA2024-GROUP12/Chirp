using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Validation.Username;

/// <summary>
/// Class for username validation
/// Has same requirements as github usernames
/// </summary>
public class GitHubUsernameAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var username = value as string;

        if (string.IsNullOrEmpty(username))
        {
            return ValidationResult.Success; // Let [Required] handle empty values
        }

        // Check specific rules
        if (username.StartsWith("-") || username.EndsWith("-"))
        {
            return new ValidationResult("Username cannot start or end with a hyphen.");
        }

        if (username.Contains("--"))
        {
            return new ValidationResult("Username cannot contain consecutive hyphens.");
        }

        if (!Regex.IsMatch(username, @"^[a-zA-Z0-9-]{1,39}$"))
        {
            return new ValidationResult("Username must be 1-39 characters long and contain only letters, numbers, and hyphens.");
        }

        return ValidationResult.Success;
    }
}
