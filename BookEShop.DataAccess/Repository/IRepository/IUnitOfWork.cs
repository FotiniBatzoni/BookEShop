using BookEShop.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookEShop.DataAccess.Repository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category{ get; }

        void Save();
    }
}
