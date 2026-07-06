using System.ComponentModel.DataAnnotations;

namespace WebProject.ViewModels
{
    public class VerifyEmailViewModel
    {
        [Required(ErrorMessage = "Email zorunludur")]
        [EmailAddress(ErrorMessage = "Geçersiz email adresi")]
        public string Email { get; set; } = string.Empty;
    }
}
