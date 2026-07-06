using System.ComponentModel.DataAnnotations;

namespace EduCore.Models.ViewModels
{
    public class VerifyEmailViewModel
    {
        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçersiz e-posta adresi.")]
        [Display(Name = "E-posta Adresi")]
        public string Email { get; set; }
    }
}