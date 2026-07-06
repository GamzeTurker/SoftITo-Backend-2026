using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace EduCore.Models.ViewModels
{
    public class ReviewVM
    {
        // Formdan gelecek veya formda gösterilecek Değerlendirme nesnesi
        public Review Review { get; set; }

        // Arayüzde Kurs seçimi yapabilmek için açılır liste (Dropdown)
        public IEnumerable<SelectListItem> CourseList { get; set; }

        // Arayüzde Öğrenci seçimi yapabilmek için açılır liste (Dropdown)
        public IEnumerable<SelectListItem> StudentList { get; set; }
    }
}