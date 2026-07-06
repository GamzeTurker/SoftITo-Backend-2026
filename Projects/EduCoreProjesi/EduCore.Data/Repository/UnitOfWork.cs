
using EduCore.Data.Repository.IRepository;
using System;

namespace EduCore.Data.Repository
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly EducationDbContext _context;

        public UnitOfWork(EducationDbContext context)
        {
            _context = context;

           
        }
        public ICategoryRepository Category => new CategoryRepository(_context);
        public ICertificateRepository Certificate => new CertificateRepository(_context);
        public ILessonRepository Lesson => new LessonRepository(_context);
        public IEnrollmentRepository Enrollment => new EnrollmentRepository(_context);
        public IReviewRepository Review => new ReviewRepository(_context);
        public ICourseRepository Course => new CourseRepository(_context); 





        public void Save()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}