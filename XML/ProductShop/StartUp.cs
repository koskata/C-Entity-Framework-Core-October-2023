using System.Text;
using System.Xml.Serialization;

using ProductShop.Data;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            var db = new ProductShopContext();

            //string xmlUsers = File.ReadAllText(@"../../../Datasets/users.xml");

            //string xmlProducts = File.ReadAllText(@"../../../Datasets/products.xml");

            //string xmlCategories = File.ReadAllText(@"../../../Datasets/categories.xml");

            //string xmlCategorieProducts = File.ReadAllText(@"../../../Datasets/categories-products.xml");

            Console.WriteLine(GetUsersWithProducts(db));
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(UserDTO[]), new XmlRootAttribute("Users"));

            var usersDtos = (UserDTO[])serializer.Deserialize(new StringReader(inputXml));

            List<User> users = new List<User>();

            foreach (var userDto in usersDtos)
            {
                var user = new User()
                {
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    Age = userDto.Age
                };
                users.Add(user);
            }

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count()}";
        }
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ProductDTO[]), new XmlRootAttribute("Products"));

            var productsDTOS = (ProductDTO[])serializer.Deserialize(new StringReader(inputXml));

            Product[] products = productsDTOS
                .Select(p => new Product()
                {
                    Name = p.Name,
                    Price = p.Price,
                    SellerId = p.SellerId,
                    BuyerId = p.BuyerId
                })
                .ToArray();

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count()}";
        }
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(CategoryDTO[]), new XmlRootAttribute("Categories"));

            var categoriesDTOS = (CategoryDTO[])serializer.Deserialize(new StringReader(inputXml));

            var categories = categoriesDTOS
                .Select(c => new Category()
                {
                    Name = c.Name
                })
                .ToArray();

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count()}";
        }
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(CategoryProductDTO[]), new XmlRootAttribute("CategoryProducts"));

            var categoryProductsDTOS = (CategoryProductDTO[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var categoryProducts = categoryProductsDTOS
                .Select(c => new CategoryProduct()
                {
                    CategoryId = c.CategoryId,
                    ProductId = c.ProductId
                })
                .ToList();

            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                            .Select(c => new ProductExportDTO
                            {
                                Name = c.Name,
                                Price = c.Price,
                                FullName = c.Buyer.FirstName + " " + c.Buyer.LastName
                            })
                            .Where(x => x.Price >= 500 && x.Price <= 1000)
                            .OrderBy(x => x.Price)
                            .Take(10)
                            .ToArray();

            var serializer = new XmlSerializer(typeof(ProductExportDTO[]), new XmlRootAttribute("Products"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            StringBuilder sb = new StringBuilder();
            using StringWriter writer = new StringWriter(sb);

            serializer.Serialize(writer, products, namespaces);

            return sb.ToString().TrimEnd();
        }

        //public static string GetSoldProducts(ProductShopContext context)
        //{
        //    var users = context.Users
        //                .Where(u => u.ProductsSold.Count >= 1)
        //                .OrderBy(u => u.LastName)
        //                .ThenBy(u => u.FirstName)
        //                .Select(x => new UserExportDTO
        //                {
        //                    FirstName = x.FirstName,
        //                    LastName = x.LastName,
        //                    SoldProducts = x.ProductsSold.Select(p => new UserSoldProductsDTO()
        //                    {
        //                        Name = p.Name,
        //                        Price = p.Price
        //                    }).ToArray()

        //                }).Take(5).ToArray();

        //    var serializer = new XmlSerializer(typeof(UserExportDTO[]), new XmlRootAttribute("Users"));

        //    var namespaces = new XmlSerializerNamespaces();
        //    namespaces.Add(string.Empty, string.Empty);

        //    StringBuilder sb = new StringBuilder();
        //    using StringWriter writer = new StringWriter(sb);

        //    serializer.Serialize(writer, users, namespaces);

        //    return sb.ToString().TrimEnd();
        //}

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                                .Select(x => new CategoryExportDTO
                                {
                                    Name = x.Name,
                                    Count = x.CategoryProducts.Count,
                                    AveragePrice = x.CategoryProducts.Average(x => x.Product.Price),
                                    TotalRevenue = x.CategoryProducts.Sum(x => x.Product.Price)
                                }).OrderByDescending(x => x.Count).ThenBy(x => x.TotalRevenue).ToArray();

            var serializer = new XmlSerializer(typeof(CategoryExportDTO[]), new XmlRootAttribute("Categories"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            StringBuilder sb = new StringBuilder();
            using StringWriter writer = new StringWriter(sb);

            serializer.Serialize(writer, categories, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Count >= 1)
                .OrderByDescending(u => u.ProductsSold.Count)
                .Select(u => new UserExportDTO()
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = new SoldProductsContainerDTO()
                    {
                        Count = u.ProductsSold.Count,
                        Products = u.ProductsSold.Select(ps => new UserSoldProductsDTO()
                        {
                            Name = ps.Name,
                            Price = ps.Price,
                        })
                        .OrderByDescending(ps => ps.Price).ToArray()
                    }
                })
                .ToArray();

            ExportUsersWithCountDto result = new()
            {
                Count = users.Length,
                Users = users.Take(10).ToArray()
            };

            var serializer = new XmlSerializer(typeof(ExportUsersWithCountDto), new XmlRootAttribute("Users"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            StringBuilder sb = new StringBuilder();
            using StringWriter writer = new StringWriter(sb);

            serializer.Serialize(writer, result, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}