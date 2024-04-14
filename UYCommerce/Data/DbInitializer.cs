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

        public async static void Initialize(IServiceProvider serviceProvider)
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
                            ImagenNombre = "cat_deporte.jpg",
                            MostrarEnInicio= true,
                            Atributos = new Atributo[]
                            {
                                atributos.FirstOrDefault(a=>a.Nombre == "Color")!,
                                atributos.FirstOrDefault(a=>a.Nombre == "Rodado")!,
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
                        new Categoria{Nombre = "Joggers", CategoriaPadre = categorias[0]},
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
                        new AtributoValor{Valor = "R26", Atributo = atributos.FirstOrDefault(a=>a.Nombre == "Rodado")},
                        new AtributoValor{Valor = "R29", Atributo = atributos.FirstOrDefault(a=>a.Nombre == "Rodado")},
                        new AtributoValor{Valor = "8gb", Atributo = atributos.FirstOrDefault(a=>a.Nombre == "Memoria RAM")},
                        new AtributoValor{Valor = "16gb", Atributo = atributos.FirstOrDefault(a=>a.Nombre == "Memoria RAM")},
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
                            VecesComprado = 1,
                            Imagenes = new ProductoImagen[]
                            {
                                new ProductoImagen{ImagenNombre = "iphone11_1.png"},
                                new ProductoImagen{ImagenNombre = "iphone11_2.png"},
                                new ProductoImagen{ImagenNombre = "iphone11_3.png"},
                            },
                            Atributos= new Atributo[]{

                                atributos.First(a => a.Nombre == "Color")!,
                                atributos.First(a => a.Nombre == "Almacenamiento")!,
                            },
                            Skus = new Sku[]
                            {
                                new Sku{
                                    Key="iphone-11-azul-64gb",
                                    Nombre = "Iphone 11 Azul 64gb",
                                    Stock = 4,
                                    Precio = 759.99,
                                    PrecioAnterior= 800,
                                    Imagenes = new SkuImagen[]
                                    {
                                       new SkuImagen{ImagenNombre = "iphone11_1.png"},
                                       new SkuImagen{ImagenNombre = "iphone11_2.png"},
                                       new SkuImagen{ImagenNombre = "iphone11_3.png"},
                                    },
                                    AtributosValores = new AtributoValor[]
                                    {
                                        atributoValores.FirstOrDefault(a=>a.Valor == "Azul")!,
                                        atributoValores.FirstOrDefault(a=>a.Valor == "64gb")!,
                                    }
                                },
                                new Sku{
                                    Key="iphone-11-rojo-64gb",
                                    Nombre = "Iphone 11 Rojo 64gb",
                                    Stock = 4,
                                    Precio = 800,
                                    Imagenes = new SkuImagen[]
                                    {
                                        new SkuImagen{ImagenNombre="iphonerojo1.png"},
                                        new SkuImagen{ImagenNombre="iphonerojo2.png"},
                                    },
                                    AtributosValores = new AtributoValor[]
                                    {
                                        atributoValores.FirstOrDefault(a=>a.Valor == "Rojo")!,
                                        atributoValores.FirstOrDefault(a=>a.Valor == "64gb")!,
                                    }
                                },
                                new Sku{
                                    Key="iphone-11-rojo-128gb",
                                    Nombre = "Iphone 11 Rojo 128gb",
                                    Stock = 10,
                                    Precio = 999,
                                    PrecioAnterior= 1020,
                                    Imagenes = new SkuImagen[]
                                    {
                                        new SkuImagen{ImagenNombre="iphonerojo1.png"},
                                        new SkuImagen{ImagenNombre="iphonerojo2.png"},
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
                            VecesComprado = 4,
                            Imagenes = new ProductoImagen[]
                            {
                                new ProductoImagen{ImagenNombre = "remera-vlone1.jpg"},
                            },
                            Atributos= new Atributo[]{

                                atributos.First(a => a.Nombre == "Color")!,
                            },
                            Skus = new Sku[]
                            {
                                new Sku{
                                    Key="remera-vlone-violeta",
                                    Nombre = "Remera Vlone Violeta",
                                    Stock = 4,
                                    Precio = 135,
                                    PrecioAnterior= 200,
                                    Imagenes = new SkuImagen[]
                                    {
                                        new SkuImagen{ImagenNombre = "remera-vlone1.jpg"},
                                    },
                                    AtributosValores = new AtributoValor[]
                                    {
                                        atributoValores.FirstOrDefault(a=>a.Valor == "Violeta")!,
                                    },
                                },
                                new Sku{
                                    Key="remera-vlone-azul",
                                    Nombre = "Remera Vlone Azul",
                                    Stock = 4,
                                    Precio = 140,
                                    AtributosValores = new AtributoValor[]
                                    {
                                        atributoValores.FirstOrDefault(a=>a.Valor == "Azul")!,
                                    },
                                    Imagenes = new SkuImagen[]
                                    {
                                        new SkuImagen{ImagenNombre = "remera-vlone2.jpg"}
                                    }
                                },
                                new Sku{
                                    Key="remera-vlone-rojo",
                                    Nombre = "Remera Vlone Rojo",
                                    Stock = 4,
                                    Precio = 300,
                                    AtributosValores = new AtributoValor[]
                                    {
                                        atributoValores.FirstOrDefault(a=>a.Valor == "Rojo")!,
                                    },
                                    Imagenes = new SkuImagen[]
                                    {
                                        new SkuImagen{ImagenNombre = "remera-vlone3.jpg"}
                                    }
                                },
                                new Sku{
                                    Key="remera-vlone-naranja",
                                    Nombre = "Remera Vlone Naranja",
                                    Stock = 4,
                                    Precio = 399.99,
                                    AtributosValores = new AtributoValor[]
                                    {
                                        atributoValores.FirstOrDefault(a=>a.Valor == "Naranja")!,
                                    },
                                    Imagenes = new SkuImagen[]
                                    {
                                        new SkuImagen{ImagenNombre = "remera-vlone4.jpg"}
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
                            VecesComprado = 1,
                            Imagenes = new ProductoImagen[]
                            {
                                new ProductoImagen{ImagenNombre = "macbook-1.jpg"},
                                new ProductoImagen{ImagenNombre = "macbook-2.jpg"},
                                new ProductoImagen{ImagenNombre = "macbook-3.jpg"},
                            },
                            Atributos= new Atributo[]{

                                atributos.First(a => a.Nombre == "Memoria RAM")!,
                            },
                            Skus = new Sku[]
                            {
                                new Sku{
                                    Key="macbook-m2-8gb",
                                    Nombre = "Macbook m2 8gb",
                                    Stock = 1,
                                    Precio = 1250,
                                    PrecioAnterior= 1490,
                                    AtributosValores = new AtributoValor[]
                                    {
                                        atributoValores.FirstOrDefault(a=>a.Valor == "8gb")!,
                                    },
                                    Imagenes = new SkuImagen[]
                                    {
                                        new SkuImagen{ImagenNombre = "macbook-1.jpg"},
                                        new SkuImagen{ImagenNombre = "macbook-2.jpg"},
                                        new SkuImagen{ImagenNombre = "macbook-3.jpg"},
                                    },
                                },
                                new Sku{
                                    Key="macbook-m2-16gb",
                                    Nombre = "Macbook m2 16gb",
                                    Stock = 5,
                                    Precio = 1689,
                                    PrecioAnterior= 1900,
                                    AtributosValores = new AtributoValor[]
                                    {
                                        atributoValores.FirstOrDefault(a=>a.Valor == "16gb")!,
                                    },
                                    Imagenes = new SkuImagen[]
                                    {
                                        new SkuImagen{ImagenNombre = "macbook-1.jpg"},
                                        new SkuImagen{ImagenNombre = "macbook-2.jpg"},
                                        new SkuImagen{ImagenNombre = "macbook-3.jpg"},
                                    },
                                },
                            }
                        },
                        new Producto{
                            Nombre="Bicicleta Scott Negra",
                            NombreClave = "bicicleta-scott-negra",
                            Descripcion = "Desde hace 60 años, la filosofía de Scott es \"sin atajos\". Esta compañía internacional se destaca por la pasión, la tecnología, la innovación y el diseño en cada uno de sus productos.\n\nDesafiá tus capacidades\nLas mountain bikes, o bicicletas de montaña son el medio de transporte perfecto para tus aventuras y alcanzar aquellos lugares recónditos que querés conocer. Sus materiales y diseños están pensados para la acción, son resistentes y cuentan con mejor maniobrabilidad que otros modelos brindándote mayor tracción. Además, sus llantas con dibujos marcados favorecen el agarre en terrenos difíciles.\n\nDiseñada para vos\nSu sistema de suspensión delantera la hace más liviana que aquellas que tienen doble suspensión. Esto se traduce en más velocidad, mejor nivel y mantenimiento sencillo.\n\nSeguridad y calidad\nGracias a sus frenos de disco hidráulico podés detener la bicicleta con total precisión cuando lo necesites, ya que tiene una gran potencia; además, su funcionamiento no se ve afectado por cuestiones como la corrosión, las condiciones climáticas o del suelo, ni el estado del aro.",
                            Categoria = subCategorias.FirstOrDefault(c => c.Nombre == "Bicicletas"),
                            Marca = marcas.FirstOrDefault(m=>m.Nombre == "Scott"),
                            VecesComprado = 0,
                            Imagenes = new ProductoImagen[]
                            {
                                new ProductoImagen{ImagenNombre = "bici-scott-negra-1.jpg"},
                                new ProductoImagen{ImagenNombre = "bici-scott-negra-2.jpg"},
                                new ProductoImagen{ImagenNombre = "bici-scott-negra-3.jpg"},
                            },
                            Atributos= new Atributo[]{

                                atributos.First(a => a.Nombre == "Rodado")!,
                            },
                            Skus= new Sku[]{

                                new Sku {
                                    Key = "bicicleta-scott-negra-R26",
                                    Nombre = "Bicicleta Scott Negra R26",
                                    Stock = 10,
                                    Precio=839,
                                    PrecioAnterior=939,
                                    Imagenes = new SkuImagen[]{
                                        new SkuImagen{ImagenNombre = "bici-scott-negra-1.jpg"},
                                        new SkuImagen{ImagenNombre = "bici-scott-negra-2.jpg"},
                                        new SkuImagen{ImagenNombre = "bici-scott-negra-3.jpg"},
                                    },
                                    AtributosValores= new AtributoValor[]{
                                        atributoValores.FirstOrDefault(a=>a.Valor == "R26")!,
                                    }

                                },
                                new Sku {
                                    Key = "bicicleta-scott-negra-R29",
                                    Nombre = "Bicicleta Scott Negra R29",
                                    Stock = 5,
                                    Precio=949,
                                    Imagenes = new SkuImagen[]{
                                        new SkuImagen{ImagenNombre = "bici-scott-negra-1.jpg"},
                                        new SkuImagen{ImagenNombre = "bici-scott-negra-2.jpg"},
                                        new SkuImagen{ImagenNombre = "bici-scott-negra-3.jpg"},
                                    },
                                    AtributosValores= new AtributoValor[]{
                                        atributoValores.FirstOrDefault(a=>a.Valor == "R29")!,
                                    }

                                }
                            }
                        },
                        new Producto{
                            Nombre="Samsung Galaxy Flip",
                            NombreClave = "samsung-galaxy-flip",
                            Descripcion = "Doble cámara y más detalles\nSus 2 cámaras traseras de 12 Mpx/12 Mpx te permitirán tomar imágenes con más detalles y lograr efectos únicos como el famoso modo retrato de poca profundidad de campo.\n\nAdemás, el dispositivo cuenta con cámara frontal de 10 Mpx para que puedas sacarte divertidas selfies o hacer videollamadas.\n\nDescubrí la pantalla plegable\nAl plegarse, adopta un tamaño pequeño y compacto que te resultará muy cómodo para transportar. Cuando quieras disfrutar de tus contenidos favoritos se transforma desplegando su pantalla de 6.7\".\n\nDesbloqueo veloz con tu huella dactilar\nCon el sensor de huella dactilar, el acceso es seguro y podrás desbloquearlo automáticamente con un toque.\n\nBatería para todo el día\nSu batería de 3300 mAh se adapta a tu ritmo de vida y te garantiza energía para toda una jornada con una sola carga.\n\nGran capacidad de almacenamiento\nCon su memoria interna de 128 GB podrás almacenar archivos y aplicaciones de gran tamaño sin necesidad de subirlos a la nube y aprovechar tus momentos sin conexión para darles el máximo uso.",
                            Categoria = subCategorias.FirstOrDefault(c => c.Nombre == "Celulares"),
                            Marca = marcas.FirstOrDefault(m=>m.Nombre == "Samsung"),
                            VecesComprado = 1,
                            Imagenes = new ProductoImagen[]{
                                new ProductoImagen{ImagenNombre = "galaxyflip1.png"},
                                new ProductoImagen{ImagenNombre = "galaxyflip2.png"},
                            },
                            Skus= new Sku[]{

                                new Sku{
                                    Key = "samsung-galaxy-flip",
                                    Nombre = "Samsung Galaxy Flip",
                                    Stock = 1,
                                    Precio=399,
                                    Imagenes = new SkuImagen[]{
                                        new SkuImagen{ImagenNombre = "galaxyflip1.png"},
                                        new SkuImagen{ImagenNombre = "galaxyflip2.png"},
                                    },
                                }
                            }
                        },
                        new Producto{
                            Nombre="Iphone 13",
                            NombreClave = "iphone-13",
                            Descripcion = "iPhone 13. El sistema de dos cámaras más avanzado en un iPhone. El superrápido chip A15 Bionic. Un gran\nsalto en duración de batería. Un diseño resistente. Y una pantalla Super Retina XDR más brillante.\n\nAvisos Legales\n1La pantalla tiene esquinas redondeadas que siguen el elegante diseño curvo del teléfono, y las esquinas se encuentran dentro de un rectángulo estándar. Si se mide en forma de rectángulo estándar, la pantalla tiene 6.06 pulgadas en diagonal. El área real de visualización es menor.\n2La duración de la batería varía según el uso y la configuración. Para obtener más información, visita apple.com/mx/batteries.\n3El iPhone 13 es resistente a los derrames, a las salpicaduras y al polvo, y fue probado en condiciones de laboratorio controladas, con una clasificación IP68 según la norma IEC 60529. La resistencia a los derrames, a las salpicaduras y al polvo no es una condición permanente, y podría disminuir como consecuencia del uso normal. No intentes cargar un iPhone mojado; consulta el manual del usuario para ver las instrucciones de limpieza y secado. La garantía no cubre daños producidos por líquidos.\n4Algunas funcionalidades podrían no estar disponibles en todos los países o áreas.\n5Los accesorios se venden por separado.",
                            Categoria = subCategorias.FirstOrDefault(c => c.Nombre == "Celulares"),
                            Marca = marcas.FirstOrDefault(m=>m.Nombre == "Iphone"),
                            VecesComprado = 1,
                            Imagenes = new ProductoImagen[]{
                                new ProductoImagen{ImagenNombre = "iphone13-1.png"},
                                new ProductoImagen{ImagenNombre = "iphone13-2.png"},
                            },
                            Atributos = new Atributo[]{

                                atributos.First(a => a.Nombre == "Color")
                            },
                            Skus= new Sku[]{

                                new Sku{
                                    Key = "iphone-13-azul",
                                    Nombre = "Iphone 13 Azul",
                                    Stock = 3,
                                    Precio=969,
                                    Imagenes = new SkuImagen[]{
                                        new SkuImagen{ImagenNombre = "iphone13-1.png"},
                                        new SkuImagen{ImagenNombre = "iphone13-2.png"},
                                    },
                                    AtributosValores = new AtributoValor[]{
                                        atributoValores.FirstOrDefault(a=>a.Valor == "Azul")!,
                                    }
                                },
                                new Sku{
                                    Key = "iphone-13-negro",
                                    Nombre = "Iphone 13 Negro",
                                    Stock = 0,
                                    Precio=969,
                                    Imagenes = new SkuImagen[]{
                                        new SkuImagen{ImagenNombre = "iphone13negro-1.webp"},
                                    },
                                    AtributosValores = new AtributoValor[]{
                                        atributoValores.FirstOrDefault(a=>a.Valor == "Negro")!,
                                    }
                                }
                            }
                        },
                        new Producto{
                            Nombre="Joggers Adidas",
                            NombreClave = "joggers-adidas",
                            Descripcion = "These loose-fitting adidas joggers are made for comfort and retro style. The open hem and relaxed silhouette channel vintage track pant inspiration, while 3-Stripes down the sides keep you connected to adidas history. Paired with a crop hoodie, they're ready for adventures in the city or cozy nights in.\n\nThe cotton in this product has been sourced through Better Cotton. Better Cotton is sourced via a chain of custody model called mass balance. This means that Better Cotton is not physically traceable to end products.",
                            Categoria = subCategorias.FirstOrDefault(c => c.Nombre == "Joggers"),
                            Marca = marcas.FirstOrDefault(m=>m.Nombre == "Adidas"),
                            VecesComprado = 0,
                            Imagenes = new ProductoImagen[]{
                                new ProductoImagen{ImagenNombre = "joggings-adidas-blanco.jpg"},
                                new ProductoImagen{ImagenNombre = "joggings-adidas-negro.jpg"},
                            },
                            Atributos= new Atributo[]{

                                atributos.First(a => a.Nombre == "Color")!,
                            },
                            Skus= new Sku[]{

                                new Sku{
                                    Key = "joggers-adidas-blanco",
                                    Nombre = "Joggers Adidas Blanco",
                                    Stock = 10,
                                    Precio=46,
                                    Imagenes = new SkuImagen[]{
                                        new SkuImagen{ImagenNombre = "joggings-adidas-blanco.jpg"},
                                    },
                                    AtributosValores = new AtributoValor[]{
                                        atributoValores.FirstOrDefault(a=>a.Valor == "Blanco")!,
                                    }
                                },
                                new Sku{
                                    Key = "joggers-adidas-negro",
                                    Nombre = "Joggers Adidas Negro",
                                    Stock = 3,
                                    Precio=52,
                                    Imagenes = new SkuImagen[]{
                                        new SkuImagen{ImagenNombre = "joggings-adidas-negro.jpg"},
                                    },
                                    AtributosValores = new AtributoValor[]{
                                        atributoValores.FirstOrDefault(a=>a.Valor == "Negro")!,
                                    }
                                },
                            }
                        },
                        new Producto{
                            Nombre="Remera Nike",
                            NombreClave = "remera-nike",
                            Descripcion = "La remera Nike Sportswear Club es un clásico. Está hecha de 100% algodón y se siente suave. Con solo el logotipo de Nike bordado en el pecho, es un diseño básico pero moderno.",
                            Categoria = subCategorias.FirstOrDefault(c => c.Nombre == "Remeras"),
                            Marca = marcas.FirstOrDefault(m=>m.Nombre == "Nike"),
                            VecesComprado = 0,
                            Imagenes = new ProductoImagen[]{
                                new ProductoImagen{ImagenNombre = "remera-nike-comun-blanca.jpg"},
                                new ProductoImagen{ImagenNombre = "remera-nike-comun-negra.jpg"},
                            },
                            Atributos= new Atributo[]{

                                atributos.First(a => a.Nombre == "Color")!,
                                atributos.First(a => a.Nombre == "Talle")!,
                            },
                            Skus= new Sku[]{

                                new Sku{
                                    Key = "remera-nike-blanco-m",
                                    Nombre = "Remera Nike Blanca",
                                    Stock = 3,
                                    Precio=43,
                                    Imagenes = new SkuImagen[]{
                                        new SkuImagen{ImagenNombre = "remera-nike-comun-blanca.jpg"},
                                    },
                                    AtributosValores = new AtributoValor[]{
                                        atributoValores.FirstOrDefault(a=>a.Valor == "Blanco")!,
                                        atributoValores.FirstOrDefault(a=>a.Valor == "M")!,
                                    }
                                },
                                new Sku{
                                    Key = "remera-nike-blanco-l",
                                    Nombre = "Remera Nike Blanca",
                                    Stock = 3,
                                    Precio=43,
                                    Imagenes = new SkuImagen[]{
                                        new SkuImagen{ImagenNombre = "remera-nike-comun-blanca.jpg"},
                                    },
                                    AtributosValores = new AtributoValor[]{
                                        atributoValores.FirstOrDefault(a=>a.Valor == "Blanco")!,
                                        atributoValores.FirstOrDefault(a=>a.Valor == "L")!,
                                    }
                                },
                                new Sku{
                                    Key = "remera-nike-negro-xl",
                                    Nombre = "Remera Nike Negro",
                                    Stock = 3,
                                    Precio=46,
                                    Imagenes = new SkuImagen[]{
                                        new SkuImagen{ImagenNombre = "remera-nike-comun-negra.jpg"},
                                    },
                                    AtributosValores = new AtributoValor[]{
                                        atributoValores.FirstOrDefault(a=>a.Valor == "Negro")!,
                                        atributoValores.FirstOrDefault(a=>a.Valor == "XL")!,
                                    }
                                },
                            }
                        },

                    };


                //ROLES
                Rol[] roles = new Rol[] {

                    new Rol{Nombre = "User" },
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
                            productos[0].Skus!.ElementAt(1),
                            productos[1].Skus!.ElementAt(1),
                        },
                    },
                    new Usuario
                    {
                        Id = 2,
                        Nombre = "Maria",
                        Email = "maria@gmail.com",
                        Rol = roles[0],
                        Password = "123",
                        Favoritos = new Sku[]
                        {
                            productos[1].Skus!.ElementAt(1),
                            productos[2].Skus!.ElementAt(1),
                        }
                    },
                    new Usuario
                    {
                        Id = 3,
                        Nombre = "Admin",
                        Email = "admin@gmail.com",
                        Rol = roles[1],
                        Password = "123"
                    },
                };

                //ORDENES
                Orden[] ordenes = new Orden[] {

                    new Orden
                    {
                        Id = "1",
                        NombreComprador = "Juan",
                        UsuarioId = 1,
                        Estado = "Entregado",
                        Direccion = new OrdenDireccion{
                            address_line_1 = "Calle 1",
                            address_line_2 = "apt 23",
                            admin_area_1 = "CL",
                            admin_area_2 = "Las Toscas",
                            postal_code = "16002",
                            country_code = "UY"
                        },
                        FechaDeCompra = DateTime.Now.AddDays(-7),
                        FechaDeEntrega = DateTime.Now.AddDays(-1).AddHours(2),
                        Productos = new ProductoOrden[]
                        {
                            new ProductoOrden
                            {
                                OrdenId = "1",
                                SkuId = 2,
                                Sku = productos[0].Skus!.ElementAt(1),
                                Nombre = "Iphone 11 Azul 64gb",
                                Precio = 759.99,
                                Cantidad = 1,
                            }

                        },
                        Total = 800,
                        MetodoDePago = "Tarjeta"
                    },
                    new Orden
                    {
                        Id = "2",
                        NombreComprador = "Juan",
                        UsuarioId = 1,
                        Estado = "En camino",
                        Direccion = new OrdenDireccion{
                            address_line_1 = "Calle 1",
                            address_line_2 = "apt 23",
                            admin_area_1 = "CL",
                            admin_area_2 = "Las Toscas",
                            postal_code = "16002",
                            country_code = "UY"
                        },
                        FechaDeCompra = DateTime.Now.AddDays(-1).AddHours(5),
                        Productos = new ProductoOrden[]
                        {
                            new ProductoOrden
                            {
                                OrdenId = "2",
                                SkuId = 6,
                                Sku = productos[1].Skus!.ElementAt(3),
                                Nombre = "Remera Vlone Rojo",
                                Precio = 300,
                                Cantidad = 2,
                            },
                            new ProductoOrden
                            {
                                OrdenId = "2",
                                SkuId = 8,
                                Sku = productos[2].Skus!.ElementAt(1),
                                Nombre = "Macbook m2 8gb",
                                Precio = 1250,
                                Cantidad = 1,
                            }

                        },
                        Total = 1850,
                        MetodoDePago = "Paypal"
                    },
                    new Orden
                    {
                        Id = "3",
                        NombreComprador = "Juan",
                        UsuarioId = 1,
                        Estado = "En camino",
                        Direccion = new OrdenDireccion{
                            address_line_1 = "Calle 1",
                            address_line_2 = "apt 23",
                            admin_area_1 = "CL",
                            admin_area_2 = "Las Toscas",
                            postal_code = "16002",
                            country_code = "UY"
                        },
                        FechaDeCompra = DateTime.Now.AddMonths(-1),
                        Productos = new ProductoOrden[]
                        {
                            new ProductoOrden
                            {
                                OrdenId = "3",
                                SkuId = 6,
                                Sku = productos[1].Skus!.ElementAt(3),
                                Nombre = "Remera Vlone Rojo",
                                Precio = 300,
                                Cantidad = 2,
                            },
                        },
                        Total = 600,
                        MetodoDePago = "Paypal"
                    },
                    new Orden
                    {
                        Id = "4",
                        NombreComprador = "Juan",
                        UsuarioId = 1,
                        Estado = "En camino",
                        Direccion = new OrdenDireccion{
                            address_line_1 = "Calle 1",
                            address_line_2 = "apt 23",
                            admin_area_1 = "CL",
                            admin_area_2 = "Las Toscas",
                            postal_code = "16002",
                            country_code = "UY"
                        },
                        FechaDeCompra = DateTime.Now.AddMonths(-2),
                        Productos = new ProductoOrden[]
                        {
                            new ProductoOrden
                            {
                                OrdenId = "4",
                                SkuId = 6,
                                Sku = productos[1].Skus!.ElementAt(3),
                                Nombre = "Remera Vlone Rojo",
                                Precio = 300,
                                Cantidad = 2,
                            },
                        },
                        Total = 600,
                        MetodoDePago = "Paypal"
                    }
                    ,
                    new Orden
                    {
                        Id = "5",
                        NombreComprador = "Juan",
                        UsuarioId = 1,
                        Estado = "En camino",
                        Direccion = new OrdenDireccion{
                            address_line_1 = "Calle 1",
                            address_line_2 = "apt 23",
                            admin_area_1 = "CL",
                            admin_area_2 = "Las Toscas",
                            postal_code = "16002",
                            country_code = "UY"
                        },
                        FechaDeCompra = DateTime.Now.AddMonths(-3),
                        Productos = new ProductoOrden[]
                        {
                            new ProductoOrden
                            {
                                OrdenId = "5",
                                SkuId = 6,
                                Sku = productos[1].Skus!.ElementAt(3),
                                Nombre = "Remera Vlone Rojo",
                                Precio = 300,
                                Cantidad = 2,
                            },
                        },
                        Total = 600,
                        MetodoDePago = "Paypal"
                    },
                    new Orden
                    {
                        Id = "6",
                        NombreComprador = "Juan",
                        UsuarioId = 1,
                        Estado = "Entregado",
                        Direccion = new OrdenDireccion{
                            address_line_1 = "Calle 1",
                            address_line_2 = "apt 23",
                            admin_area_1 = "CL",
                            admin_area_2 = "Las Toscas",
                            postal_code = "16002",
                            country_code = "UY"
                        },
                        FechaDeCompra = DateTime.Now.AddMonths(-3),
                        FechaDeEntrega = DateTime.Now.AddDays(-7),
                        Productos = new ProductoOrden[]
                        {
                            new ProductoOrden
                            {
                                OrdenId = "6",
                                SkuId = productos[5].Skus!.ElementAt(1).Id,
                                Sku = productos[5].Skus!.ElementAt(1),
                                Nombre = productos[5].Skus!.ElementAt(1).Nombre,
                                Precio = productos[5].Skus!.ElementAt(1).Precio,
                                Cantidad = 2,
                            },
                        },
                        Total = productos[5].Skus!.ElementAt(1).Precio * 2,
                        MetodoDePago = "Paypal"
                    },
                    new Orden
                    {
                        Id = "7",
                        NombreComprador = "Juan",
                        UsuarioId = 1,
                        Estado = "Entregado",
                        Direccion = new OrdenDireccion{
                            address_line_1 = "Calle 1",
                            address_line_2 = "apt 23",
                            admin_area_1 = "CL",
                            admin_area_2 = "Las Toscas",
                            postal_code = "16002",
                            country_code = "UY"
                        },
                        FechaDeCompra = DateTime.Now.AddMonths(-2),
                        FechaDeEntrega = DateTime.Now.AddDays(-5),
                        Productos = new ProductoOrden[]
                        {
                            new ProductoOrden
                            {
                                OrdenId = "7",
                                SkuId = productos[4].Skus!.ElementAt(0).Id,
                                Sku = productos[4].Skus!.ElementAt(0),
                                Nombre = productos[4].Skus!.ElementAt(0).Nombre,
                                Precio = productos[4].Skus!.ElementAt(0).Precio,
                                Cantidad = 1,
                            },
                        },
                        Total = productos[4].Skus!.ElementAt(0).Precio,
                        MetodoDePago = "Paypal"
                    },
                };

                //Reviews

                Review[] reviews = new Review[] {

                    new Review {
                        ProductoId = 1,
                        Puntuacion = 5,
                        Texto = "Excelente!",
                        UsuarioId = 1
                    },
                    new Review {
                        ProductoId = 1,
                        Puntuacion = 5,
                        Texto = "Buenisimo",
                        UsuarioId = 2
                    },
                    new Review {
                        ProductoId = 2,
                        Puntuacion = 4,
                        Texto = "Bien",
                        UsuarioId = 1
                    },
                    new Review {
                        ProductoId = 2,
                        Puntuacion = 3,
                        Texto = "Talle equivocado",
                        UsuarioId = 2
                    },
                    new Review {
                        ProductoId = 3,
                        Puntuacion = 4,
                        Texto = "No tan rapida pero bien",
                        UsuarioId = 1
                    },
                    new Review {
                        ProductoId = 4,
                        Puntuacion = 5,
                        Texto = "Genial para mtb",
                        UsuarioId = 1
                    },
                };


                ProductoCarrito[] productosCarrito = new ProductoCarrito[]
                {
                    new ProductoCarrito{
                        SkuId=1 ,
                        CarritoId=1,
                        Cantidad=1,
                        Sku= productos[0].Skus!.ElementAt(0)
                    },
                    new ProductoCarrito{
                        SkuId= productos[1].Skus!.ElementAt(2).Id ,
                        CarritoId=1,
                        Cantidad=3,
                        Sku= productos[1].Skus!.ElementAt(2)
                    },
                    new ProductoCarrito{
                        SkuId= productos[2].Skus!.ElementAt(1).Id ,
                        CarritoId=2,
                        Cantidad=1,
                        Sku= productos[2].Skus!.ElementAt(1)
                    }
                };


                Pregunta[] preguntas = new Pregunta[]
                {
                    new Pregunta{ Contenido= "Tienen stock?", Fecha = DateTime.Now, Usuario = usuarios[0], Producto = productos[0]}
                };


                await context.AddRangeAsync(categorias);
                await context.AddRangeAsync(subCategorias);
                await context.AddRangeAsync(marcas);
                await context.AddRangeAsync(atributos);
                await context.AddRangeAsync(atributoValores);
                await context.AddRangeAsync(productos);
                await context.AddRangeAsync(roles);
                await context.AddRangeAsync(usuarios);
                await context.AddRangeAsync(ordenes);
                await context.AddRangeAsync(reviews);
                await context.AddRangeAsync(productosCarrito);
                await context.AddRangeAsync(preguntas);

                await context.SaveChangesAsync();
            }

        }
    }
}


