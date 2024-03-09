using System;
using Microsoft.EntityFrameworkCore;
using UYCommerce.Models;

namespace UYCommerce.Data
{
    public class DbInitializer
    {
        public DbInitializer()
        {
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {


            using (var context = new ShopContext(
                serviceProvider.GetRequiredService<DbContextOptions<ShopContext>>()))
            {

                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();


                Atributo[] atributos = new Atributo[] {

                        new Atributo{Nombre = "Color"},
                        new Atributo{Nombre = "Talle"},
                        new Atributo{Nombre = "Almacenamiento"},
                        new Atributo{Nombre = "Rodado"},
                        new Atributo{Nombre = "Memoria RAM"},
                        new Atributo{Nombre = "Woofer Tamaño"},
                    };

                Categoria[] categorias = new Categoria[] {

                        new Categoria
                        {
                            Nombre = "Ropa",
                            ImagenNombre = "cat_ropa.png",
                            MostrarEnInicio= true
                        },
                        new Categoria
                        {
                            Nombre = "Tecnologia",
                            ImagenNombre = "cat_tecnologia.png",
                            MostrarEnInicio= true
                        },
                        new Categoria
                        {
                            Nombre = "Deporte",
                            ImagenNombre = "cat_deporte.png",
                            MostrarEnInicio= true,
                            Atributos = new Atributo[]
                            {
                                atributos.FirstOrDefault(a=>a.Nombre == "Color")!,
                                atributos.FirstOrDefault(a=>a.Nombre == "Rodado")!,
                            }
                        },
                        new Categoria
                        {
                            Nombre = "Digitales",
                            ImagenNombre = "cat_digitales.png",
                            MostrarEnInicio= true,
                        },
                        new Categoria
                        {
                            Nombre = "Musica",
                            ImagenNombre = "cat_musica.png",
                            MostrarEnInicio= true,
                            Atributos = new Atributo[]
                            {
                                atributos.FirstOrDefault(a=>a.Nombre == "Color")!,
                                atributos.FirstOrDefault(a=>a.Nombre == "Woofer Tamaño")!,
                            }
                        },

                    };

                //subcategorias
                Categoria[] subCategorias = new Categoria[] {

                        new Categoria{Nombre = "Remeras", CategoriaPadre = categorias[0],
                        Atributos = new Atributo[]
                        {
                            atributos.FirstOrDefault(a=>a.Nombre == "Color")!,
                            atributos.FirstOrDefault(a=>a.Nombre == "Talle")!
                        }},
                        new Categoria{Nombre = "Joggins", CategoriaPadre = categorias[0]},
                        new Categoria{Nombre = "Camperas", CategoriaPadre = categorias[0]},
                        new Categoria{Nombre = "Celulares", CategoriaPadre = categorias[1],
                        Atributos = new Atributo[]
                        {
                            atributos.FirstOrDefault(a=>a.Nombre == "Almacenamiento")!,
                            atributos.FirstOrDefault(a=>a.Nombre == "Color")!
                        }},
                        new Categoria{Nombre = "Computadoras", CategoriaPadre = categorias[1],
                        Atributos = new Atributo[]
                        {
                            atributos.FirstOrDefault(a=>a.Nombre == "Almacenamiento")!,
                            atributos.FirstOrDefault(a=>a.Nombre == "Memoria RAM")!
                        }},
                        new Categoria{Nombre = "Bicicletas", CategoriaPadre = categorias[2]},
                        new Categoria{Nombre = "Juegos", CategoriaPadre = categorias[3]},
                        new Categoria{Nombre = "Parlantes", CategoriaPadre = categorias[4]},
                        new Categoria{Nombre = "Auriculares", CategoriaPadre = categorias[4]},
                        new Categoria{Nombre = "Teclados Midi", CategoriaPadre = categorias[4]},

                    };




                AtributoValor[] atributoValores = new AtributoValor[] {

                        new AtributoValor{Valor = "Blanco", Atributo = atributos.FirstOrDefault(a=>a.Nombre == "Color")},
                        new AtributoValor{Valor = "Negro", Atributo = atributos.FirstOrDefault(a=>a.Nombre == "Color")},
                        new AtributoValor{Valor = "Gris", Atributo = atributos.FirstOrDefault(a=>a.Nombre == "Color")},
                        new AtributoValor{Valor = "Marron", Atributo = atributos.FirstOrDefault(a=>a.Nombre == "Color")},
                        new AtributoValor{Valor = "Rojo", Atributo = atributos.FirstOrDefault(a=>a.Nombre == "Color")},
                        new AtributoValor{Valor = "Azul", Atributo = atributos.FirstOrDefault(a=>a.Nombre == "Color")},
                        new AtributoValor{Valor = "Verde", Atributo = atributos.FirstOrDefault(a=>a.Nombre == "Color")},
                        new AtributoValor{Valor = "Amarillo", Atributo = atributos.FirstOrDefault(a=>a.Nombre == "Color")},
                        new AtributoValor{Valor = "Naranja", Atributo = atributos.FirstOrDefault(a=>a.Nombre == "Color")},
                        new AtributoValor{Valor = "Violeta", Atributo = atributos.FirstOrDefault(a=>a.Nombre == "Color")},
                        new AtributoValor{Valor = "64gb", Atributo = atributos.FirstOrDefault(a=>a.Nombre == "Almacenamiento")},
                        new AtributoValor{Valor = "128gb", Atributo = atributos.FirstOrDefault(a=>a.Nombre == "Almacenamiento")},
                        new AtributoValor{Valor = "256gb", Atributo = atributos.FirstOrDefault(a=>a.Nombre == "Almacenamiento")},
                        new AtributoValor{Valor = "512gb", Atributo = atributos.FirstOrDefault(a=>a.Nombre == "Almacenamiento")},
                        new AtributoValor{Valor = "S", Atributo = atributos.FirstOrDefault(a=>a.Nombre == "Talle")},
                        new AtributoValor{Valor = "M", Atributo = atributos.FirstOrDefault(a=>a.Nombre == "Talle")},
                        new AtributoValor{Valor = "L", Atributo = atributos.FirstOrDefault(a=>a.Nombre == "Talle")},
                        new AtributoValor{Valor = "XL", Atributo = atributos.FirstOrDefault(a=>a.Nombre == "Talle")},
                        new AtributoValor{Valor = "19", Atributo = atributos.FirstOrDefault(a=>a.Nombre == "Rodado")},
                        new AtributoValor{Valor = "20", Atributo = atributos.FirstOrDefault(a=>a.Nombre == "Rodado")},
                        new AtributoValor{Valor = "8gb", Atributo = atributos.FirstOrDefault(a=>a.Nombre == "Memoria RAM")},
                        new AtributoValor{Valor = "16gb", Atributo = atributos.FirstOrDefault(a=>a.Nombre == "Memoria RAM")},
                        new AtributoValor{Valor = "5'", Atributo = atributos.FirstOrDefault(a=>a.Nombre == "Woofer Tamaño")},
                        new AtributoValor{Valor = "7'", Atributo = atributos.FirstOrDefault(a=>a.Nombre == "Woofer Tamaño")},
                    };

                Marca[] marcas = new Marca[] {

                        new Marca{Nombre = "Nike"},
                        new Marca{Nombre = "Adidas"},
                        new Marca{Nombre = "Apple"},
                        new Marca{Nombre = "Samsung"},
                        new Marca{Nombre = "HP"},
                        new Marca{Nombre = "Dell"},
                        new Marca{Nombre = "Scott"},
                        new Marca{Nombre = "Gt"},
                        new Marca{Nombre = "Rockstar"},
                        new Marca{Nombre = "Krk"},
                        new Marca{Nombre = "PreSounus"},
                        new Marca{Nombre = "ATH"},
                        new Marca{Nombre = "M-Audio"},
                        new Marca{Nombre = "Vlone"},

                     };




                Producto[] productos = new Producto[] {
                        //Iphone 11
                        new Producto
                        {
                            Nombre="Iphone 11",
                            NombreClave = "iphone-11",
                            Descripcion = "Grabe videos en 4K, tome hermosos retratos y capture paisajes completos con el nuevo sistema de cámara dual. Toma fotos increíbles con poca luz utilizando el modo Noche. Vea colores reales en fotos, videos y juegos en la pantalla Liquid Retina de 6.1 pulgadas. Lleve el rendimiento sin precedentes del chip A13 Bionic a sus juegos, realidad aumentada y fotografías. Una batería para todo el día: haz mucho y recarga poco. Y cuente con una resistencia al agua de hasta dos metros por hasta 30 minutos. \n\nEl iPhone 11 tiene un nuevo sistema de cámara dual para capturar más de lo que ves y amas. Viene con el chip más rápido en un celular inteligente.También tiene la mejor calidad de video en un teléfono móvil. De esa forma, sus recuerdos serán verdaderamente inolvidables. \n\nLa cámara gran angular de 12 MP tiene Focus Pixeles 100% para un enfoque automático hasta tres veces más rápido en ambiente con poca luz. La cámara ultra angular de 12 MP captura cuatro veces más de la imagen que la cámara gran angular. Perfecto para paisajes, viajes, grupos grandes, grandes espacios y objetos en movimiento.\n \nAmbas cámaras del iPhone 11 graban videos 4K nítidos a 60 fps. La nueva cámara ultra angular captura 4 veces más de cualquier escena y es ideal para sujetos en movimiento. Y con el Zoom de audio, el sonido se acerca junto con la imagen. Además, editar videos es tan simple como editar fotos. El Modo Noche es una nueva función automática para capturar imágenes en ambientes con poca luz. Los colores se ven más naturales, las fotos se ven más brillantes y ni siquiera necesitas usar el flash. \n\nEl vidrio de la parte delantera y trasera del iPhone 11 se ha reforzado mediante un proceso de intercambio iónico. El iPhone 11 es resistente al agua hasta una profundidad de dos metros durante 30 minutos, el doble de la profundidad del iPhone XR. ",
                            Categoria = subCategorias.FirstOrDefault(c => c.Nombre == "Celulares"),
                            Marca = marcas.FirstOrDefault(m=>m.Nombre == "Apple"),
                            Puntaje = 7.5,
                            VecesComprado = 5,
                            Imagenes = new ProductoImagen[]
                            {
                                new ProductoImagen{ImagenNombre = "iphone11_1.png",Orden = 1},
                                new ProductoImagen{ImagenNombre = "iphone11_2.png",Orden = 2},
                                new ProductoImagen{ImagenNombre = "iphone11_3.png",Orden = 3},
                            },
                            Skus = new Sku[]
                            {
                                new Sku{
                                    Nombre="iphone-11-azul-64gb",
                                    Stock = 4,
                                    Precio = 759.99,
                                    Imagenes = new SkuImagen[]
                                    {
                                       new SkuImagen{ImagenNombre = "iphone11_1.png",Orden = 1},
                                       new SkuImagen{ImagenNombre = "iphone11_2.png",Orden = 2},
                                       new SkuImagen{ImagenNombre = "iphone11_3.png",Orden = 3},
                                    },
                                    AtributosValores = new AtributoValor[]
                                    {
                                        atributoValores.FirstOrDefault(a=>a.Valor == "Azul")!,
                                        atributoValores.FirstOrDefault(a=>a.Valor == "64gb")!,
                                    }
                                },
                                new Sku{
                                    Nombre="iphone-11-rojo-64gb",
                                    Stock = 4,
                                    Precio = 800,
                                    Imagenes = new SkuImagen[]
                                    {
                                        new SkuImagen{ImagenNombre="iphonerojo1.png",Orden=1},
                                        new SkuImagen{ImagenNombre="iphonerojo2.png",Orden=2},
                                    },
                                    AtributosValores = new AtributoValor[]
                                    {
                                        atributoValores.FirstOrDefault(a=>a.Valor == "Rojo")!,
                                        atributoValores.FirstOrDefault(a=>a.Valor == "64gb")!,
                                    }
                                },
                                new Sku{
                                    Nombre="iphone-11-rojo-128gb",
                                    Stock = 10,
                                    Precio = 999,
                                    Imagenes = new SkuImagen[]
                                    {
                                        new SkuImagen{ImagenNombre="iphonerojo1.png",Orden=1},
                                        new SkuImagen{ImagenNombre="iphonerojo2.png",Orden=2},
                                    },
                                    AtributosValores = new AtributoValor[]
                                    {
                                        atributoValores.FirstOrDefault(a=>a.Valor == "Rojo")!,
                                        atributoValores.FirstOrDefault(a=>a.Valor == "128gb")!,
                                    }
                                },
                            }
                        },
                        new Producto
                        {
                            Nombre="Remera Vlone",
                            NombreClave = "remera-vlone",
                            Descripcion = "",
                            Categoria = subCategorias.FirstOrDefault(c => c.Nombre == "Remeras"),
                            Marca = marcas.FirstOrDefault(m=>m.Nombre == "Vlone"),
                            Puntaje = 8,
                            VecesComprado = 100,
                            Imagenes = new ProductoImagen[]
                            {
                                new ProductoImagen{ImagenNombre = "remera-vlone1.jpg",Orden = 1},
                            },
                            Skus = new Sku[]
                            {
                                new Sku{
                                    Nombre="remera-vlone-violeta",
                                    Stock = 4,
                                    Precio = 135,
                                    Imagenes = new SkuImagen[]
                                    {
                                        new SkuImagen{ImagenNombre = "remera-vlone1.jpg",Orden = 1},
                                    },
                                    AtributosValores = new AtributoValor[]
                                    {
                                        atributoValores.FirstOrDefault(a=>a.Valor == "Violeta")!,
                                    },
                                },
                                new Sku{
                                    Nombre="remera-vlone-azul",
                                    Stock = 4,
                                    Precio = 140,
                                    AtributosValores = new AtributoValor[]
                                    {
                                        atributoValores.FirstOrDefault(a=>a.Valor == "Azul")!,
                                    },
                                    Imagenes = new SkuImagen[]
                                    {
                                        new SkuImagen{ImagenNombre = "remera-vlone2.jpg",Orden = 1}
                                    }
                                },
                                new Sku{
                                    Nombre="remera-vlone-rojo",
                                    Stock = 4,
                                    Precio = 300,
                                    AtributosValores = new AtributoValor[]
                                    {
                                        atributoValores.FirstOrDefault(a=>a.Valor == "Rojo")!,
                                    },
                                    Imagenes = new SkuImagen[]
                                    {
                                        new SkuImagen{ImagenNombre = "remera-vlone3.jpg",Orden = 1}
                                    }
                                },
                                new Sku{
                                    Nombre="remera-vlone-naranja",
                                    Stock = 4,
                                    Precio = 399.99,
                                    AtributosValores = new AtributoValor[]
                                    {
                                        atributoValores.FirstOrDefault(a=>a.Valor == "Naranja")!,
                                    },
                                    Imagenes = new SkuImagen[]
                                    {
                                        new SkuImagen{ImagenNombre = "remera-vlone4.jpg",Orden = 1}
                                    }
                                },
                            }
                        },
                        new Producto
                        {
                            Nombre="Macbook m2",
                            NombreClave = "macbook-m2",
                            Descripcion = "En su interior, el chip M2 llama a ser un sucesor más bien lineal del M1 añadiendo más núcleos gráficos (9 o hasta 10) con un poco más de eficiencia en los núcleos de la CPU. El rendimiento final es un 20% más que en el chip M1 y seguimos sin ventilador. Por lo tanto, el silencio del MacBook Air va a ser total.A nivel de almacenamiento partimos de 256 GB y podemos llegar a los 2 TB de almacenamiento, incluyendo una memoria RAM de hasta 24 GB. El mínimo en ese aspecto sigue siendo de 8 GB.La pantalla es de 13.6 pulgadas y tiene 500 nits de brillo, capaces de plasmar más de mil millones de colores. Hay una cámara FaceTime (sí, con notch) de 1080p capaz de detectar mucho mejor los ambientes de luminosidad baja. Cuatro altavoces repartidos por la carcasa se encargan de aportar audio espacial.",
                            Categoria = subCategorias.FirstOrDefault(c => c.Nombre == "Computadoras"),
                            Marca = marcas.FirstOrDefault(m=>m.Nombre == "Apple"),
                            Puntaje = 9.3,
                            VecesComprado = 1000,
                            Imagenes = new ProductoImagen[]
                            {
                                new ProductoImagen{ImagenNombre = "macbook-1.jpg",Orden = 1},
                                new ProductoImagen{ImagenNombre = "macbook-2.jpg",Orden = 2},
                                new ProductoImagen{ImagenNombre = "macbook-3.jpg",Orden = 3},
                            },
                            Skus = new Sku[]
                            {
                                new Sku{
                                    Nombre="macbook-m2-8gb",
                                    Stock = 1,
                                    Precio = 1250,
                                    AtributosValores = new AtributoValor[]
                                    {
                                        atributoValores.FirstOrDefault(a=>a.Valor == "8gb")!,
                                    },
                                    Imagenes = new SkuImagen[]
                                    {
                                        new SkuImagen{ImagenNombre = "macbook-1.jpg",Orden = 1},
                                        new SkuImagen{ImagenNombre = "macbook-2.jpg",Orden = 2},
                                        new SkuImagen{ImagenNombre = "macbook-3.jpg",Orden = 3},
                                    },
                                },
                                new Sku{
                                    Nombre="macbook-m2-16gb",
                                    Stock = 5,
                                    Precio = 1689,
                                    AtributosValores = new AtributoValor[]
                                    {
                                        atributoValores.FirstOrDefault(a=>a.Valor == "16gb")!,
                                    },
                                    Imagenes = new SkuImagen[]
                                    {
                                        new SkuImagen{ImagenNombre = "macbook-1.jpg",Orden = 1},
                                        new SkuImagen{ImagenNombre = "macbook-2.jpg",Orden = 2},
                                        new SkuImagen{ImagenNombre = "macbook-3.jpg",Orden = 3},
                                    },
                                },
                            }
                        },
                    };


                //ROLES
                Rol[] roles = new Rol[] {

                    new Rol{Nombre = "Cliente" },
                    new Rol{Nombre = "Admin" }
                };

                //USUARIOS
                Usuario[] usuarios = new Usuario[] {

                    new Usuario
                    {
                        Id = 1,
                        Nombre = "Juan",
                        Email = "juan@gmail.com",
                        Rol = roles[0],
                        Password = "123",
                        Favoritos = new Sku[]
                        {
                            productos[0].Skus.ElementAt(1),
                            productos[1].Skus.ElementAt(1),
                        }
                    }
                };

                //ORDENES
                Orden[] ordenes = new Orden[] {

                    new Orden
                    {
                        Id = "1",
                        NombreComprador = "Juan",
                        UsuarioId = 1,
                        Estado = "Entregado",
                        Direccion = "Calle 1 Esq 1",
                        FechaDeCompra = DateTime.Now.AddDays(-7),
                        FechaDeEntrega = DateTime.Now.AddDays(-1),
                        Productos = new ProductoOrden[]
                        {
                            new ProductoOrden
                            {
                                OrdenId = "1",
                                SkuId = 2,
                                Sku = productos[0].Skus!.ElementAt(1),
                                Nombre = "Iphone 11",
                                Precio = 800,
                                Cantidad = 1,
                            }

                        }
                    }
                };



                context.AddRangeAsync(categorias);
                context.AddRangeAsync(subCategorias);
                context.AddRangeAsync(marcas);
                context.AddRangeAsync(atributos);
                context.AddRangeAsync(atributoValores);
                context.AddRangeAsync(productos);
                context.AddRangeAsync(roles);
                context.AddRangeAsync(usuarios);
                context.AddRangeAsync(ordenes);

                context.SaveChangesAsync();

            }

        }
    }
}


