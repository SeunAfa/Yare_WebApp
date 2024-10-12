using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yare.Models;

namespace Yare.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IProductRepository product { get; }
        IWatchRepository Watch { get; }
        IJewelleryRepository Jewellery { get; }
        IAccessoryRepository Accessory { get; }
        ICollectionRepository Collection { get; }
        IProduct_CollectionRepository Product_Collection { get; }

        IApplicationUserRepository ApplicationUser { get; }
        IShoppingCartRepository ShoppingCart { get; }
        IOrderHeaderRepository OrderHeader { get; }
        IOrderDetailRepository OrderDetail { get; }
        void Save();
    }
}
