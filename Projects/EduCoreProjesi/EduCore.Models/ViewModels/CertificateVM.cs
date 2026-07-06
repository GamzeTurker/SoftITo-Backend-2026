using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduCore.Models.ViewModels
{
    public class CertificateVM
    {
        public Certificate Certificate { get; set; }
        // Hangi kayda (Enrollment) sertifika verileceğini seçmek için liste
        public IEnumerable<SelectListItem>? EnrollmentList { get; set; }
    }
}
