using EduCore.Data.Repository.IRepository;
using EduCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduCore.Data.Repository
{
    public class CategoryRepository:Repository<Category>,ICategoryRepository
    {
        private readonly EducationDbContext _context;

        public CategoryRepository(EducationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
