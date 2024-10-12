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
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private ApplicationDbContext _db;

        public OrderDetailRepository(ApplicationDbContext db) :base(db) 
        {
            _db = db;
        }

        public void Update(OrderDetail objOrderDetail)
        {
           _db.OrderDetails.Update(objOrderDetail);
        }
    }
}
