using BookEShop.DataAccess.Repository.IRepository;
using BookEShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookEShop.DataAccess.Repository
{
    public class ShoppingCardRepository : Repository<ShoppingCard>, IShoppingCardRepository
    {
        private ApplicationDbContext _db;

        public ShoppingCardRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
