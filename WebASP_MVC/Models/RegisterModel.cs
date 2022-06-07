
using System.ComponentModel.DataAnnotations;
#if !(cookies || role)
using Microsoft.AspNetCore.Authorization;
#endif
namespace WebASP_MVC.Models
{
    [AllowAnonymous]
    public class RegisterModel
    {
#if role || cookies
        [Required(ErrorMessage = "Не указан Email")]
        public string Email { get; set; } = null!;

        [Required]
        [Display(Name = "Псевдоним")]
        public string Alias { get; set; } = null!;

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароль введен неверно")]
        public string ConfirmPassword { get; set; } = null!;
#else
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required]
        [Display(Name = "Псевдоним")]
        public string Alias { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = null!;

        [Required]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль")]
        public string ConfirmPassword { get; set; } = null!;
#endif
    }
}
