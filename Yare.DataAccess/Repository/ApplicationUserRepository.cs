using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yare.Data;
using Yare.DataAccess.Repository.IRepository;
using Yare.Models;

namespace Yare.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private ApplicationDbContext _db;

        public ApplicationUserRepository(ApplicationDbContext db):base(db)
        {
            _db = db;   
        }

        //public int Count()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
