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
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private ApplicationDbContext _db;

        public OrderHeaderRepository(ApplicationDbContext db) :base(db) 
        {
            _db = db;
        }

        public void Update(OrderHeader objOrderHeader)
        {
            _db.OrderHeaders.Update(objOrderHeader);
        }

        public void UpdateOrderStatus(int id, string OrderStatus, string? PaymentStatus = null)
        {
            var orderHeaderFromDb = _db.OrderHeaders.FirstOrDefault(x => x.Id == id);

            if (orderHeaderFromDb != null)
            {
                orderHeaderFromDb.OrderStatus = OrderStatus;
                if (PaymentStatus != null)
                {
                    orderHeaderFromDb.PaymentStatus = PaymentStatus;
                }
            }
        }

        public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
        {
            var orderHeaderFromDb = _db.OrderHeaders.FirstOrDefault(x => x.Id == id);

            orderHeaderFromDb.PaymentDate = DateTime.Now;
            orderHeaderFromDb.SessionId = sessionId;
            orderHeaderFromDb.PaymentIntentId = paymentIntentId;

        }

    }
}
