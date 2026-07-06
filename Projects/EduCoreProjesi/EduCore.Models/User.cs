using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EduCore.Models
{
    public class User:IdentityUser<int>
    {
       
        [Required(ErrorMessage = "Ad Soyad alanı zorunludur.")]
        [StringLength(100)]
        public string Name { get; set; }
       
        public string ProfileImageUrl { get; set; }
      
        public ICollection<Enrollment>? Enrollments { get; set; }

        public ICollection<Review>? Reviews { get; set; }

        public ICollection<Certificate>? Certificates { get; set; }
    }
}
