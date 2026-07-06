using System.ComponentModel.DataAnnotations;

namespace WebProject.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ad Soyad zorunludur")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email zorunludur")]
        [EmailAddress(ErrorMessage = "Geçersiz email adresi")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Parola zorunludur")]
        [DataType(DataType.Password)]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "Parola en az 8 karakter olmalıdır")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Parola tekrarı zorunludur")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Parolalar eşleşmedi")]
        [Display(Name = "Parola Tekrarı")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
