using System;
using System.Collections.Generic;
using System.Text;
using UyCommerce.Domain.Entities;

namespace UyCommerce.Application.DTOs.Reviews
{
    public class GetReviewsAndProductsResponse
    {
        public List<Review> Reviews { get; set; } = [];
        public List<Producto> Productos { get; set; } = [];
    }
}
