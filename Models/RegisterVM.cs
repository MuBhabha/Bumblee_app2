using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BumbleBee.Models;

public class RegisterVM
{
    [Required]
    public string? FirstName { get; set; }
    [Required]
    public string? LastName { get; set; }
    
    [Required]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    public string? Password { get; set; }
    
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    [DisplayName("Confirm Password")]
    public string? ConfirmPassword { get; set; }
}