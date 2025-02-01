using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Collections;
using System.Threading.Channels;

namespace EFSample
{
    public static class ObjectPropertyPrinter
    {
        public static string ToDetailedString(this object obj)
        {
            if (obj == null) return "null";

            var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var result = new System.Text.StringBuilder();
            result.AppendLine($"=== {obj.GetType().Name} ===");

            foreach (var prop in properties)
            {
                var value = prop.GetValue(obj);
                
                // Skip navigation properties and collections to avoid circular references
                if (value == null || 
                    (value is IEnumerable enumerable && !(value is string) && !enumerable.Cast<object>().Any()))
                {
                    continue;
                }

                // Format the value based on its type
                string displayValue = value switch
                {
                    DateTime date => date.ToString("yyyy-MM-dd"),
                    decimal price => price.ToString("C"),
                    IEnumerable collection when !(value is string) => 
                        $"Count: {collection.Cast<object>().Count()}",
                    _ => value.ToString()
                };

                result.AppendLine($"{prop.Name}: {displayValue}");
            }

            return result.ToString();
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            using var context = new BookStoreContext();
            var books = context.Books
                .Include(b => b.BookGenres).ToList();
            
            Console.WriteLine("===================первый запрос===================");
            foreach (var book in books)
            {
                Console.WriteLine(book.ToDetailedString());
            }
            
            var book2 = context.Books
                .Include(b => b.BookGenres)
                .ThenInclude(bg => bg.Genre).ToList();
            
            
            
            
            Console.WriteLine("===================второй запрос===================");
            foreach (var book in book2)
            {
                Console.WriteLine(book.ToDetailedString());
            }
            var genreId = 2;
            
            var book3 = context.Books
                .Include(b => b.BookGenres)
                .ThenInclude(bg => bg.Genre)
                .Where(b => b.BookGenres.Any(bg => bg.GenreId == genreId))
                .ToList();
            
            
            
            Console.WriteLine("===================третий запрос===================");
            foreach (var book in book3)
            {
                Console.WriteLine(book.ToDetailedString());
            }
            // using var context = new BookStoreContext();
            //
            // while (true)
            // {
            //     Console.Clear();
            //     Console.WriteLine("=== Книжный магазин ===");
            //     Console.WriteLine("1. Добавить книгу с жанрами");
            //     Console.WriteLine("2. Добавить книгу без жанров");
            //     Console.WriteLine("3. Добавить жанр");
            //     Console.WriteLine("4. Добавить жанр книге");
            //     Console.WriteLine("5. Просмотреть информацию о книге");
            //     Console.WriteLine("6. Просмотреть книги определенного жанра");
            //     Console.WriteLine("7. Добавить автора");
            //     Console.WriteLine("0. Выход");
            //     
            //     Console.Write("\nВыберите действие: ");
            //     var choice = Console.ReadLine();
            //
            //     try
            //     {
            //         switch (choice)
            //         {
            //             case "1":
            //                 AddBookWithGenres(context);
            //                 break;
            //             case "2":
            //                 AddBookWithoutGenres(context);
            //                 break;
            //             case "3":
            //                 AddGenre(context);
            //                 break;
            //             case "4":
            //                 AddGenreToBook(context);
            //                 break;
            //             case "5":
            //                 ViewBookDetails(context);
            //                 break;
            //             case "6":
            //                 ViewBooksByGenre(context);
            //                 break;
            //             case "7":
            //                 AddAuthor(context);
            //                 break;
            //             case "0":
            //                 return;
            //             default:
            //                 Console.WriteLine("Неверный выбор!");
            //                 break;
            //         }
            //     }
            //     catch (Exception ex)
            //     {
            //         Console.WriteLine($"\nОшибка: {ex.Message}");
            //     }
            //
            //     Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            //     Console.ReadKey();
            // }
        }
        
        static void AddAuthor(BookStoreContext context)
        {
            Console.Clear();
            Console.WriteLine("=== Добавление нового автора ===\n");

            Console.Write("Имя автора: ");
            var name = Console.ReadLine();

            Console.Write("Биография: ");
            var biography = Console.ReadLine();

            Console.Write("Дата рождения (в формате ГГГГ-ММ-ДД, или пустая строка): ");
            var birthDateStr = Console.ReadLine();
            DateTime? dateOfBirth = null;
            if (!string.IsNullOrEmpty(birthDateStr))
            {
                dateOfBirth = DateTime.Parse(birthDateStr);
            }

            Console.Write("Страна происхождения: ");
            var country = Console.ReadLine();

            var author = new Author
            {
                Name = name,
                Biography = biography,
                DateOfBirth = dateOfBirth,
                CountryOfOrigin = country
            };

            context.Authors.Add(author);
            context.SaveChanges();

            Console.WriteLine($"\nАвтор '{author.Name}' успешно добавлен с ID: {author.Id}");
        }
        
        public static void AddBookWithoutGenres(BookStoreContext context)
        {
            Console.Clear();
            Console.WriteLine("=== Добавление новой книги без жанров ===\n");

            try
            {
                // Показываем список доступных авторов
                var authors = context.Authors.ToList();
                Console.WriteLine("Доступные авторы:");
                foreach (var authorItem in authors)
                {
                    Console.WriteLine($"ID: {authorItem.Id}, Имя: {authorItem.Name}");
                }

                // Показываем список доступных поставщиков
                var suppliers = context.Suppliers.ToList();
                Console.WriteLine("\nДоступные поставщики:");
                foreach (var supplierItem in suppliers)  // Изменено имя переменной
                {
                    Console.WriteLine($"ID: {supplierItem.Id}, Название: {supplierItem.CompanyName}");
                }

                // Ввод и проверка автора
                Console.Write("\nID автора: ");
                var authorId = int.Parse(Console.ReadLine());
                var author = authors.FirstOrDefault(a => a.Id == authorId);
                if (author == null)
                {
                    throw new InvalidOperationException($"Автор с ID {authorId} не найден");
                }

                // Ввод и проверка поставщика
                Console.Write("ID поставщика: ");
                var supplierId = int.Parse(Console.ReadLine());
                var supplier = suppliers.FirstOrDefault(s => s.Id == supplierId);
                if (supplier == null)
                {
                    throw new InvalidOperationException($"Поставщик с ID {supplierId} не найден");
                }

                // Ввод основных данных книги
                Console.Write("Название книги: ");
                var title = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(title))
                {
                    throw new InvalidOperationException("Название книги не может быть пустым");
                }

                Console.Write("Описание книги: ");
                var description = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(description))
                {
                    throw new InvalidOperationException("Описание книги не может быть пустым");
                }

                Console.Write("ISBN: ");
                var isbn = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(isbn))
                {
                    throw new InvalidOperationException("ISBN не может быть пустым");
                }

                // Проверка уникальности ISBN
                if (context.Books.Any(b => b.ISBN == isbn))
                {
                    throw new InvalidOperationException("Книга с таким ISBN уже существует");
                }

                Console.Write("Цена: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal price) || price < 0)
                {
                    throw new InvalidOperationException("Некорректная цена");
                }

                Console.Write("Год публикации: ");
                if (!int.TryParse(Console.ReadLine(), out int year) || year < 1000 || year > DateTime.Now.Year)
                {
                    throw new InvalidOperationException("Некорректный год публикации");
                }

                // Используем транзакцию
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    var book = new Book
                    {
                        Title = title,
                        Description = description,
                        AuthorId = authorId,
                        SupplierId = supplierId,
                        ISBN = isbn,
                        CurrentPrice = price,
                        PublicationYear = year,
                        BookGenres = new List<BookGenre>()
                    };

                    context.Books.Add(book);
                    context.SaveChanges();
                    
                    transaction.Commit();
                    Console.WriteLine($"\nКнига '{book.Title}' успешно добавлена с ID: {book.Id}");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new InvalidOperationException($"Ошибка при сохранении книги: {ex.Message}");
                }
            }
            catch (FormatException)
            {
                throw new InvalidOperationException("Некорректный формат введенных данных");
            }
        }

        static void AddGenreToBook(BookStoreContext context)
        {
            Console.Clear();
            Console.WriteLine("=== Добавление жанра книге ===\n");

            Console.Write("Введите ID книги: ");
            var bookId = int.Parse(Console.ReadLine());

            var book = context.Books
                .Include(b => b.BookGenres)
                .FirstOrDefault(b => b.Id == bookId);

            if (book == null)
            {
                Console.WriteLine("Книга не найдена!");
                return;
            }

            Console.Write("Введите ID жанра: ");
            var genreId = int.Parse(Console.ReadLine());

            var genre = context.Genres.Find(genreId);
            if (genre == null)
            {
                Console.WriteLine("Жанр не найден!");
                return;
            }

            // Проверяем, не добавлен ли уже этот жанр
            if (book.BookGenres.Any(bg => bg.GenreId == genreId))
            {
                Console.WriteLine("Этот жанр уже добавлен к книге!");
                return;
            }

            // Добавляем связь книга-жанр
            var bookGenre = new BookGenre
            {
                BookId = bookId,
                GenreId = genreId
            };

            context.Add(bookGenre);
            context.SaveChanges();

            Console.WriteLine($"\nЖанр '{genre.Name}' успешно добавлен к книге '{book.Title}'");
        }
        public static void AddBookWithGenres(BookStoreContext context)
        {
            Console.Clear();
            Console.WriteLine("=== Добавление новой книги с жанрами ===\n");

            try 
            {
                // Показываем список доступных авторов
                var authors = context.Authors.ToList();
                Console.WriteLine("Доступные авторы:");
                foreach (var authorItem in authors)
                {
                    Console.WriteLine($"ID: {authorItem.Id}, Имя: {authorItem.Name}");
                }

                // Показываем список доступных поставщиков
                var suppliers = context.Suppliers.ToList();
                Console.WriteLine("\nДоступные поставщики:");
                foreach (var supplierItem in suppliers)  // Изменено имя переменной
                {
                    Console.WriteLine($"ID: {supplierItem.Id}, Название: {supplierItem.CompanyName}");
                }

                // Показываем список доступных жанров
                var genres = context.Genres.ToList();
                Console.WriteLine("\nДоступные жанры:");
                foreach (var genre in genres)
                {
                    Console.WriteLine($"ID: {genre.Id}, Название: {genre.Name}");
                }

                // Ввод и проверка автора
                Console.Write("\nID автора: ");
                var authorId = int.Parse(Console.ReadLine());
                var author = authors.FirstOrDefault(a => a.Id == authorId);
                if (author == null)
                {
                    throw new InvalidOperationException($"Автор с ID {authorId} не найден");
                }

                // Ввод и проверка поставщика
                Console.Write("ID поставщика: ");
                var supplierId = int.Parse(Console.ReadLine());
                var supplier = suppliers.FirstOrDefault(s => s.Id == supplierId);
                if (supplier == null)
                {
                    throw new InvalidOperationException($"Поставщик с ID {supplierId} не найден");
                }

                // Ввод основных данных книги
                Console.Write("Название книги: ");
                var title = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(title))
                {
                    throw new InvalidOperationException("Название книги не может быть пустым");
                }

                Console.Write("Описание книги: ");
                var description = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(description))
                {
                    throw new InvalidOperationException("Описание книги не может быть пустым");
                }

                Console.Write("ISBN: ");
                var isbn = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(isbn))
                {
                    throw new InvalidOperationException("ISBN не может быть пустым");
                }

                // Проверка уникальности ISBN
                if (context.Books.Any(b => b.ISBN == isbn))
                {
                    throw new InvalidOperationException("Книга с таким ISBN уже существует");
                }

                Console.Write("Цена: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal price) || price < 0)
                {
                    throw new InvalidOperationException("Некорректная цена");
                }

                Console.Write("Год публикации: ");
                if (!int.TryParse(Console.ReadLine(), out int year) || year < 1000 || year > DateTime.Now.Year)
                {
                    throw new InvalidOperationException("Некорректный год публикации");
                }

                // Ввод и проверка жанров
                Console.Write("\nВведите ID жанров (через запятую): ");
                var genreIdsInput = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(genreIdsInput))
                {
                    throw new InvalidOperationException("Необходимо указать хотя бы один жанр");
                }

                var genreIds = genreIdsInput.Split(',')
                    .Select(id => int.Parse(id.Trim()))
                    .ToList();

                var selectedGenres = genres.Where(g => genreIds.Contains(g.Id)).ToList();
                if (selectedGenres.Count != genreIds.Count)
                {
                    throw new InvalidOperationException("Некоторые жанры не найдены");
                }

                // Используем транзакцию для атомарности операции
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    var book = new Book
                    {
                        Title = title,
                        Description = description,
                        AuthorId = authorId,
                        SupplierId = supplierId,
                        ISBN = isbn,
                        CurrentPrice = price,
                        PublicationYear = year,
                        BookGenres = new List<BookGenre>()
                    };

                    // Добавляем книгу
                    context.Books.Add(book);
                    context.SaveChanges();

                    // Добавляем связи с жанрами
                    foreach (var genre in selectedGenres)
                    {
                        var bookGenre = new BookGenre
                        {
                            BookId = book.Id,
                            GenreId = genre.Id
                        };
                        context.Add(bookGenre);
                    }
                    context.SaveChanges();

                    transaction.Commit();
                    Console.WriteLine($"\nКнига '{book.Title}' успешно добавлена с ID: {book.Id}");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new InvalidOperationException($"Ошибка при сохранении книги: {ex.Message}");
                }
            }
            catch (FormatException)
            {
                throw new InvalidOperationException("Некорректный формат введенных данных");
            }
        }

        static void AddGenre(BookStoreContext context)
        {
            Console.Clear();
            Console.WriteLine("=== Добавление нового жанра ===\n");

            Console.Write("Название жанра: ");
            var name = Console.ReadLine();

            Console.Write("Описание жанра: ");
            var description = Console.ReadLine();

            var genre = new Genre
            {
                Name = name,
                Description = description
            };

            context.Genres.Add(genre);
            context.SaveChanges();

            Console.WriteLine($"\nЖанр '{genre.Name}' успешно добавлен с ID: {genre.Id}");
        }

        static void ViewBookDetails(BookStoreContext context)
        {
            Console.Clear();
            Console.WriteLine("=== Просмотр информации о книге ===\n");

            Console.Write("Введите ID книги: ");
            var bookId = int.Parse(Console.ReadLine());

            var book = context.Books
                .Include(b => b.BookGenres)
                    .ThenInclude(bg => bg.Genre)
                .Include(b => b.Author)
                .Include(b => b.Supplier)
                .FirstOrDefault(b => b.Id == bookId);

            if (book == null)
            {
                Console.WriteLine("Книга не найдена!");
                return;
            }

            Console.WriteLine($"\nНазвание: {book.Title}");
            Console.WriteLine($"Автор: {book.Author.Name}");
            Console.WriteLine($"Поставщик: {book.Supplier.CompanyName}");
            Console.WriteLine($"ISBN: {book.ISBN}");
            Console.WriteLine($"Цена: {book.CurrentPrice:C}");
            Console.WriteLine($"Год публикации: {book.PublicationYear}");
            
            Console.WriteLine("\nЖанры:");
            foreach (var bookGenre in book.BookGenres)
            {
                Console.WriteLine($"- {bookGenre.Genre.Name}");
            }
        }

        static void ViewBooksByGenre(BookStoreContext context)
        {
            Console.Clear();
            Console.WriteLine("=== Просмотр книг по жанру ===\n");

            Console.Write("Введите ID жанра: ");
            var genreId = int.Parse(Console.ReadLine());

            var genre = context.Genres.Find(genreId);
            if (genre == null)
            {
                Console.WriteLine("Жанр не найден!");
                return;
            }

            var books = context.Books
                .Include(b => b.BookGenres)
                    .ThenInclude(bg => bg.Genre)
                .Include(b => b.Author)
                .Where(b => b.BookGenres.Any(bg => bg.GenreId == genreId))
                .ToList();

            Console.WriteLine($"\nКниги жанра '{genre.Name}':");
            foreach (var book in books)
            {
                Console.WriteLine($"- {book.Title} (Автор: {book.Author.Name})");
            }
        }
    }
}

