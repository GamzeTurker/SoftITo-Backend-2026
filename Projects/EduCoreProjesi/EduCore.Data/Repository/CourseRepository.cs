
using EduCore.Data;
using EduCore.Data.Repository;
using EduCore.Data.Repository.IRepository;
using EduCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduCore.Data.Repository
{
    public class CourseRepository : Repository<Course>,ICourseRepository
    {
        private readonly EducationDbContext _context;

        public CourseRepository(EducationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
