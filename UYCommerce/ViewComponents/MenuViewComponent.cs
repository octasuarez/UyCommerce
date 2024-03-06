using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UYCommerce.Data;

namespace UYCommerce.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly ShopContext _context;

        public MenuViewComponent(ShopContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retorna todas las categorias para armar el menu principal
        /// </summary>
        /// <returns></returns>
        public IViewComponentResult Invoke()
        {
            var result = _context.Categorias
                .Include(c => c.CategoriaPadre)
                .Include(c => c.SubCategorias)
                .ToList();

            return View(result);
        }
    }
}

