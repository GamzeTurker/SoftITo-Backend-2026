using System.ComponentModel.DataAnnotations;

namespace WebProject.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Email zorunludur")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Parola zorunludur")]
        [DataType(DataType.Password)]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "Parola en az 8 karakter olmalıdır")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Parola tekrarı zorunludur")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Parolalar eşleşmedi")]
        [Display(Name = "Yeni Parola Tekrarı")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
