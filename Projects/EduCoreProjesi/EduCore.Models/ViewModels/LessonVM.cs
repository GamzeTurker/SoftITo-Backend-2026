using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduCore.Models.ViewModels
{
    public class LessonVM
    {
        public Lesson Lesson { get; set; }
        public IEnumerable<SelectListItem>? CourseList { get; set; }
    }
}
