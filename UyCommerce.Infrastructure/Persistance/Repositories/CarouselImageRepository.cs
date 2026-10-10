using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Application.Contracts.Persistance;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Infrastructure.Persistance.Repositories
{
    public class CarouselImageRepository : GenericRepository<CarouselImage>, ICarouselImageRepository
    {
        public CarouselImageRepository(ShopContext context) : base(context)
        { }

    }
}
