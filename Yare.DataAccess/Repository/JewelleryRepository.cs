using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yare.DataAccess.Repository.IRepository;
using Yare.DataAccess.Repository;
using Yare.Models;
using Yare.Data;

namespace Yare.DataAccess.Repository
{
    public class JewelleryRepository : Repository<Jewellery>, IJewelleryRepository
    {
        private ApplicationDbContext _db;

        public JewelleryRepository(ApplicationDbContext db) :base(db) 
        {
            _db = db;
        }
    
        public void Save()
        {
           _db.SaveChanges();
        }

        public void Update(Jewellery objJewellery)
        {
            _db.Jewellerys.Update(objJewellery);
        }
    }
}
