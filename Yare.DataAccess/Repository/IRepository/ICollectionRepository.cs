using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Yare.Models;

namespace Yare.DataAccess.Repository.IRepository
{
    public interface ICollectionRepository : IRepository<Collection>
    {
        void Update(Collection objCollection);
        void Save();
    }
}
