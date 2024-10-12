using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yare.DataAccess.Repository.IRepository;
using Yare.DataAccess.Repository;
using Yare.Models;
using Yare.Data;
using System.Security.Cryptography;

namespace Yare.DataAccess.Repository
{
    public class CollectionRepository : Repository<Collection>, ICollectionRepository
    {
        private ApplicationDbContext _db;

        public CollectionRepository(ApplicationDbContext db) :base(db) 
        {
            _db = db;
        }
    
        public void Save()
        {
           _db.SaveChanges();
        }

        public void Update(Collection objCollection)
        {
            _db.Collections.Update(objCollection);
        }
    }
}
