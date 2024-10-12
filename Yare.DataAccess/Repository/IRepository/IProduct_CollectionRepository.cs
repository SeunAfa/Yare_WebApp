using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Yare.Models;

namespace Yare.DataAccess.Repository.IRepository
{
    public interface IProduct_CollectionRepository : IRepository<Product_Collection>
    {
        void Update(Product_Collection objWatch);
        void Save();
    }
}
