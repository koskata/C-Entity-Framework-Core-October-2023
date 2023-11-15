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
            string users = File.ReadAllText("../../../Datasets/users.json");

            //2.
            string products = File.ReadAllText("../../../Datasets/products.json");

            //3.
            string categories = File.ReadAllText("../../../Datasets/categories.json");

            //4.
            string categoriesProducts = File.ReadAllText("../../../Datasets/categories-products.json");

            Console.WriteLine(GetProductsInRange(context));


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
    }
}