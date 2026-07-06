using System;
using System.Collections.Generic;
using System.Text;

namespace EduCore.Data.Repository.IRepository
{
    public interface IUnitOfWork:IDisposable
    {

       ICertificateRepository Certificate { get; }
        ICourseRepository Course { get; }
        ILessonRepository Lesson { get; }
        ICategoryRepository Category { get; }
         IReviewRepository Review { get; }
        IEnrollmentRepository Enrollment { get; }

        void Save();
    }
}
