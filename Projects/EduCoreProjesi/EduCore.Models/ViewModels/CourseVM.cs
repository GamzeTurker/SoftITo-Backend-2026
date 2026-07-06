using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduCore.Models.ViewModels
{
    public class CourseVM
    {
        public Course Course { get; set; } // Formdan gelecek yeni Kurs bilgileri (Adı, Açıklaması vb.)

        // Ekranda gösterilecek Kategorilerin listesi (Açılır Liste/Dropdown için)
        public IEnumerable<SelectListItem> CategoryList { get; set; }
    }
}

