using EduCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduCore.Data
{
    public class EducationDbContext: IdentityDbContext<User, IdentityRole<int>, int>
    {
        public EducationDbContext(DbContextOptions<EducationDbContext> options) : base(options)
        {
        }
        // NOT: Users tablosunu buraya yazmıyoruz, IdentityDbContext bunu otomatik ekliyor.
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<Review> Reviews { get; set; }
    }
}
