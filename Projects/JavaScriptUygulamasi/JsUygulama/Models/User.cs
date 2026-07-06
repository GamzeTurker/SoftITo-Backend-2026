using System.ComponentModel.DataAnnotations;

namespace JsUygulama.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı Adı Giriniz")]
        [StringLength(50)]
        public string Username { get; set; }

        [Required(ErrorMessage = "İsim Giriniz")]
        [StringLength(50)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Şifre Giriniz")]
        [StringLength(50)]
        public string Password { get; set; }
        [Required(ErrorMessage ="Email Giriniz")]
        [StringLength(100)]
        public string Email { get; set; }

    }
}
