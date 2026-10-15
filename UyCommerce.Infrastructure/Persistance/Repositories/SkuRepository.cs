using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Infrastructure.Persistance.Repositories
{
    public class SkuRepository : GenericRepository<Sku>, ISkuRepository
    {
        public SkuRepository(ShopContext context) : base(context)
        {
        }


    }
}
