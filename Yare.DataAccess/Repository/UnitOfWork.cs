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
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            product = new ProductRepository(_db);
            Watch = new WatchRepository(_db);
            Jewellery = new JewelleryRepository(_db);
            Accessory = new AccessoryRepository(_db);
            Collection = new CollectionRepository(_db);
            Product_Collection = new Product_CollectionRepository(_db);

            ApplicationUser = new ApplicationUserRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db);
            OrderHeader = new OrderHeaderRepository(_db);
            OrderDetail = new OrderDetailRepository(_db);

        }

        public IProductRepository product { get; private set; }
        public IWatchRepository Watch { get; private set; }
        public IJewelleryRepository Jewellery { get; private set; }
        public IAccessoryRepository Accessory { get; private set; }
        public ICollectionRepository Collection { get; private set; }
        public IProduct_CollectionRepository Product_Collection { get; private set; }


        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
