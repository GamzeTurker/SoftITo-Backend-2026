using EduCore.Data.Repository.IRepository;
using EduCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduCore.Data.Repository
{
    public class ReviewRepository:Repository<Review>,IReviewRepository
    {
        private readonly EducationDbContext _context;

        public ReviewRepository(EducationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
