using EduCore.Data.Repository.IRepository;
using EduCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduCore.Data.Repository
{
    public class EnrollmentRepository:Repository<Enrollment>,IEnrollmentRepository
    {
        private readonly EducationDbContext _context;

        public EnrollmentRepository(EducationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
