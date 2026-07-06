using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EduCore.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        [StringLength(50)]
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Course>? Courses { get; set; }
    }
}
