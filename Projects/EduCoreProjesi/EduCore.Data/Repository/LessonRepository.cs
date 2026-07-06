using EduCore.Data.Repository.IRepository;
using EduCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduCore.Data.Repository
{
    public class LessonRepository:Repository<Lesson>,ILessonRepository
    {
        private readonly EducationDbContext _context;

    public LessonRepository(EducationDbContext context) : base(context)
    {
        _context = context;
    }
}
}

