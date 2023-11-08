namespace BookShop
{
    using System.Globalization;
    using System.Text;

    using BookShop.Models.Enums;

    using Data;

    using Initializer;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            //int length = int.Parse(Console.ReadLine());
            Console.WriteLine(RemoveBooks(db));
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            if (!Enum.TryParse<AgeRestriction>(command, true, out var ageRestriction))
            {
                return $"{command} is not a valid age restriction";
            }

            var books = context.Books
                            .Select(x => new
                            {
                                x.Title,
                                x.AgeRestriction
                            })
                            .Where(x => x.AgeRestriction == ageRestriction)
                            .OrderBy(x => x.Title).ToList();

            string result = string.Join(Environment.NewLine, books.Select(x => x.Title));

            return result;
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            var books = context.Books
                        .Select(x => new
                        {
                            x.Title,
                            x.EditionType,
                            x.Copies
                        }).Where(x => x.EditionType == EditionType.Gold && x.Copies < 5000);

            return string.Join(Environment.NewLine, books.Select(x => x.Title));
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
                        .Select(x => new
                        {
                            x.Title,
                            x.Price
                        }).Where(x => x.Price > 40).OrderByDescending(x => x.Price);

            return string.Join(Environment.NewLine, books.Select(x => $"{x.Title} - ${x.Price:f2}"));
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context.Books
                        .Select(x => new
                        {
                            x.BookId,
                            x.Title,
                            x.ReleaseDate
                        }).Where(x => x.ReleaseDate.Value.Year != year).OrderBy(x => x.BookId);

            return string.Join(Environment.NewLine, books.Select(x => x.Title));
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            string[] cmdArgs = input.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(x => x.ToLower()).ToArray();


            var books = context.Books
                        .Select(x => new
                        {
                            x.Title,
                            x.BookCategories
                        }).Where(x => x.BookCategories.Any(bc => cmdArgs.Contains(bc.Category.Name.ToLower())))
                        .OrderBy(x => x.Title).ToList();

            return string.Join(Environment.NewLine, books.Select(x => x.Title));
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;

            DateTime dateTime = DateTime.ParseExact(date, "dd-MM-yyyy", provider);

            var books = context.Books
                        .Select(x => new
                        {
                            x.Title,
                            x.EditionType,
                            x.Price,
                            x.ReleaseDate
                        }).Where(x => x.ReleaseDate.Value.Date < dateTime).OrderByDescending(x => x.ReleaseDate).ToList();

            return string.Join(Environment.NewLine, books.Select(x => $"{x.Title} - {x.EditionType} - ${x.Price:f2}"));
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors
                            .Select(x => new
                            {
                                x.FirstName,
                                x.LastName
                            }).Where(x => x.FirstName.EndsWith(input)).OrderBy(x => x.FirstName).ThenBy(x => x.LastName);

            return string.Join(Environment.NewLine, authors.Select(x => $"{x.FirstName} {x.LastName}"));
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var books = context.Books
                        .Select(x => new
                        {
                            x.Title
                        }).Where(x => x.Title.ToLower().Contains(input.ToLower())).OrderBy(x => x.Title).ToList();

            return string.Join(Environment.NewLine, books.Select(x => x.Title));
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var books = context.Books
                        .Select(x => new
                        {
                            x.Title,
                            x.Author.FirstName,
                            x.Author.LastName
                        }).Where(x => x.LastName.ToLower().StartsWith(input.ToLower())).ToList();

            return string.Join(Environment.NewLine, books.Select(x => $"{x.Title} ({x.FirstName} {x.LastName})"));
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var books = context.Books
                        .Where(x => x.Title.Length > lengthCheck).ToList();

            return books.Count();
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authors = context.Authors
                            .Select(x => new
                            {
                                x.FirstName,
                                x.LastName,
                                Total = x.Books.Sum(x => x.Copies)
                            }).OrderByDescending(x => x.Total).ToList();


            return string.Join(Environment.NewLine, authors.Select(x => $"{x.FirstName} {x.LastName} - {x.Total}"));
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categories = context.Categories
                                .Select(x => new
                                {
                                    x.Name,
                                    Total = x.CategoryBooks.Sum(x => x.Book.Copies * x.Book.Price)
                                }).OrderByDescending(x => x.Total).ThenBy(x => x.Name).ToList();

            return string.Join(Environment.NewLine, categories.Select(x => $"{x.Name} ${x.Total}"));
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categories = context.Categories
                            .Select(x => new
                            {
                                x.Name,
                                Books = x.CategoryBooks.Select(x => new
                                {
                                    x.Book.Title,
                                    x.Book.ReleaseDate,
                                }).OrderByDescending(x => x.ReleaseDate).Take(3).ToList()
                            }).OrderBy(x => x.Name).ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var cat in categories)
            {
                sb.AppendLine($"--{cat.Name}");
                foreach (var book in cat.Books)
                {
                    sb.AppendLine($"{book.Title} ({book.ReleaseDate.Value.Year})");
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static void IncreasePrices(BookShopContext context)
        {
            var books = context.Books.Where(x => x.ReleaseDate.Value.Year < 2010).ToList();

            foreach (var book in books)
            {
                book.Price += 5;
            }

            context.SaveChanges();
        }

        public static int RemoveBooks(BookShopContext context)
        {
            var books = context.Books
                        .Where(x => x.Copies < 4200).ToList();

            context.RemoveRange(books);

            context.SaveChanges();

            int removed = books.Count;

            return removed;
        }
    }
}


