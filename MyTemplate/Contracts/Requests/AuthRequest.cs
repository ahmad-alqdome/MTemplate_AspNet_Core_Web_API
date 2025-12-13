using System.ComponentModel.DataAnnotations;

namespace MyTemplate.Contracts.Requests;

public class AuthRequest
{
    [EmailAddress(ErrorMessage = "Email is not Valid")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Password is Required")]
    public string Password { get; set; }
}