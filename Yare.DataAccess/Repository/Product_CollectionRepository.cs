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
    public class Product_CollectionRepository : Repository<Product_Collection>, IProduct_CollectionRepository
    {
        private ApplicationDbContext _db;

        public Product_CollectionRepository(ApplicationDbContext db) :base(db) 
        {
            _db = db;
        }
    
        public void Save()
        {
           _db.SaveChanges();
        }

        public void Update(Product_Collection objWatch)
        {
            _db.Product_Collections.Update(objWatch);
        }
    }
}
