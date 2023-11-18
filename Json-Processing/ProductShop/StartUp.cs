using Newtonsoft.Json;

using ProductShop.Data;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            ProductShopContext context = new ProductShopContext();

            //1.
            //string users = File.ReadAllText("../../../Datasets/users.json");

            //2.
            //string products = File.ReadAllText("../../../Datasets/products.json");

            //3.
            //string categories = File.ReadAllText("../../../Datasets/categories.json");

            //4.
            //string categoriesProducts = File.ReadAllText("../../../Datasets/categories-products.json");

            Console.WriteLine(GetUsersWithProducts(context));


        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var users = JsonConvert.DeserializeObject<User[]>(inputJson).ToList();

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var products = JsonConvert.DeserializeObject<Product[]>(inputJson).ToList();

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var categories = JsonConvert.DeserializeObject<Category[]>(inputJson).Where(x => x.Name is not null).ToList();

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var categoryProducts = JsonConvert.DeserializeObject<CategoryProduct[]>(inputJson).ToList();

            context.CategoriesProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                            .Where(p => p.Price >= 500 && p.Price <= 1000)
                            .Select(p => new
                            {
                                name = p.Name,
                                price = p.Price,
                                seller = $"{p.Seller.FirstName} {p.Seller.LastName}"
                            }).OrderBy(p => p.price).ToList();

            string json = JsonConvert.SerializeObject(products, Formatting.Indented);
            return json;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                        .Where(x => x.ProductsSold.Any(x => x.BuyerId != null))
                        .Select(x => new
                        {
                            firstName = x.FirstName,
                            lastName = x.LastName,
                            soldProducts = x.ProductsSold.Select(ps => new
                            {
                                name = ps.Name,
                                price = ps.Price,
                                buyerFirstName = ps.Buyer.FirstName,
                                buyerLastName = ps.Buyer.LastName
                            }).ToList()
                        }).OrderBy(x => x.lastName).ThenBy(x => x.firstName).ToList();

            return JsonConvert.SerializeObject(users, Formatting.Indented);
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                                .Select(x => new
                                {
                                    category = x.Name,
                                    productsCount = x.CategoriesProducts.Count(),
                                    averagePrice = x.CategoriesProducts.Average(x => x.Product.Price).ToString("f2"),
                                    totalRevenue = x.CategoriesProducts.Sum(x => x.Product.Price).ToString("f2")
                                }).OrderByDescending(x => x.productsCount).ToList();

            return JsonConvert.SerializeObject(categories, Formatting.Indented);
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                        .Where(x => x.ProductsSold.Any(x => x.BuyerId != null))
                        .Select(x => new
                        {
                            firstName = x.FirstName,
                            lastName = x.LastName,
                            age = x.Age,
                            soldProducts = x.ProductsSold.Where(x => x.BuyerId != null).Select(ps => new
                            {
                                name = ps.Name,
                                price = ps.Price
                            }).ToList()
                        }).OrderByDescending(x => x.soldProducts.Count).ToList();

            var output = new
            {
                usersCount = users.Count,
                users = users.Select(x => new
                {
                    x.firstName,
                    x.lastName,
                    x.age,
                    soldProducts = new
                    {
                        count = x.soldProducts.Count,
                        products = x.soldProducts
                    }
                })
            };

            string json = JsonConvert.SerializeObject(output, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            });

            
            return json;
        }
    }
}