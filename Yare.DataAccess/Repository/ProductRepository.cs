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
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db) :base(db) 
        {
            _db = db;
        }
    
        public void Save()
        {
           _db.SaveChanges();
        }

        public void Update(Product objProduct)
        {
            _db.Products.Update(objProduct);
        }
    }
}
