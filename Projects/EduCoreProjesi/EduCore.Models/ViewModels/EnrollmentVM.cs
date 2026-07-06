using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduCore.Models.ViewModels
{
    public class EnrollmentVM
    {
        public Enrollment Enrollment { get; set; }
        public IEnumerable<SelectListItem>? StudentList { get; set; }
        public IEnumerable<SelectListItem>? CourseList { get; set; }
    }
}
