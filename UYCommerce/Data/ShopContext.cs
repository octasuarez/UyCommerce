using System;
using Microsoft.EntityFrameworkCore;
using UYCommerce.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace UYCommerce.Data
{
    public class ShopContext : DbContext
    {
        public ShopContext(DbContextOptions<ShopContext> options) : base(options)
        {
        }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Marca> Marcas { get; set; }
        public DbSet<Atributo> Atributos { get; set; }
        public DbSet<AtributoValor> AtributoValores { get; set; }
        public DbSet<Sku> Skus { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<OrdenDireccion> Direcciones { get; set; }
        public DbSet<Orden> Ordenes { get; set; }
        public DbSet<Carrito> Carritos { get; set; }
        public DbSet<ProductoCarrito> ProductosCarritos { get; set; }
        public DbSet<Pregunta> Preguntas { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Codigo> Codigos { get; set; }
        public DbSet<CarouselImage> CarouselImages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Producto>()
                .HasIndex(p => p.NombreClave)
                .IsUnique();

            builder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}

