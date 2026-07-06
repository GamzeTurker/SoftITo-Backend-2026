using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EduCore.Models
{
    public class Certificate
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int EnrollmentId { get; set; }
        [ForeignKey("EnrollmentId")]
        public virtual Enrollment Enrollment { get; set; }
        [Required]
        [StringLength(512)]
        public string CertificateImageUrl { get; set; }
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    }
}
