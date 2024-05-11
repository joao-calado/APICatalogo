using System.ComponentModel.DataAnnotations;

namespace APICatalogo.DTOs;

public class RegisterModel
{
    [Required(ErrorMessage ="User name is required")]
    public string? UserName { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [Required(ErrorMessage ="Password is required")]
    public string? Password { get; set; }
}
