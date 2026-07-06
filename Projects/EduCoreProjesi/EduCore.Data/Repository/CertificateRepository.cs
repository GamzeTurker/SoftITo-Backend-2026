using EduCore.Data.Repository.IRepository;
using EduCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduCore.Data.Repository
{
    public class CertificateRepository:Repository<Certificate>,ICertificateRepository
    {
        private readonly EducationDbContext _context;

        public CertificateRepository(EducationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
