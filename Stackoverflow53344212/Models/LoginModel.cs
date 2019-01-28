
using System.ComponentModel.DataAnnotations;

class LoginModel
{
    const string errorMessage = "Password must contain at least 1 number, 1 letter, and 1 special character.";

    [RegularExpression("[0-9]", ErrorMessage= errorMessage)]
    [RegularExpression("[0-9]", ErrorMessage= errorMessage)]
    public string Password { get; set; }
}
