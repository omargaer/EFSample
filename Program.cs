using System.Data;
using Microsoft.EntityFrameworkCore;

namespace EFSample
{
    // Microsoft.EntityFrameworkCore
    // Microsoft.EntityFrameworkCore.Design
    // Microsoft.EntityFrameworkCore. Sqlite
    // Microsoft.EntityFrameworkCore.Tools 
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new BookStoreContext())
            {
                // Создание и добавление данных
                var author = new Author { Name = "Лев Толстой" };
                var genre = new Genre { Name = "Роман" };
                var supplier = new Supplier { CompanyName = "Книжный дом" };

                context.Authors.Add(author);
                context.Genres.Add(genre);
                context.Suppliers.Add(supplier);
                context.SaveChanges();

                var book = new Book
                {
                    Title = "Война и мир",
                    AuthorId = author.Id,
                    GenreId = genre.Id,
                    SupplierId = supplier.Id,
                    Price = 999.99m,
                    StockQuantity = 10
                };
                context.Books.Add(book);
                context.SaveChanges();

                // Создание заказа
                var client = new Client { Name = "Иван Петров" };
                var employee = new Employee { Name = "Мария Сидорова" };
                context.Clients.Add(client);
                context.Employees.Add(employee);
                context.SaveChanges();

                var order = new Order
                {
                    ClientId = client.Id,
                    EmployeeId = employee.Id,
                    OrderDate = DateTime.Now,
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem
                        {
                            BookId = book.Id,
                            Quantity = 2,
                            Price = book.Price
                        }
                    }
                };
                context.Orders.Add(order);
                context.SaveChanges();

                // Примеры выборки данных

                // Получение книги с автором и жанром (один-к-одному)
                var bookWithDetails = context.Books
                    .Include(b => b.Author)
                    .Include(b => b.Genre)
                    .FirstOrDefault(b => b.Id == book.Id);

                Console.WriteLine($"Книга: {bookWithDetails.Title}");
                Console.WriteLine($"Автор: {bookWithDetails.Author.Name}");
                Console.WriteLine($"Жанр: {bookWithDetails.Genre.Name}");

                // Получение всех книг автора (один-ко-многим)
                var authorWithBooks = context.Authors
                    .Include(a => a.Books)
                    .FirstOrDefault(a => a.Id == author.Id);

                Console.WriteLine($"\nКниги автора {authorWithBooks.Name}:");
                foreach (var authorBook in authorWithBooks.Books)
                {
                    Console.WriteLine($"- {authorBook.Title}");
                }

                // Получение заказов клиента с деталями (один-ко-многим)
                var clientWithOrders = context.Clients
                    .Include(c => c.Orders)
                    .ThenInclude(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                    .FirstOrDefault(c => c.Id == client.Id);

                Console.WriteLine($"\nЗаказы клиента {clientWithOrders.Name}:");
                foreach (var clientOrder in clientWithOrders.Orders)
                {
                    Console.WriteLine($"Заказ от {clientOrder.OrderDate:d}:");
                    foreach (var item in clientOrder.OrderItems)
                    {
                        Console.WriteLine($"- {item.Book.Title} x{item.Quantity}");
                    }
                }

                // Пример обновления данных
                book.Price = 899.99m;
                context.SaveChanges();

                // Пример удаления данных
                var oldOrder = context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefault(o => o.Id == order.Id);

                if (oldOrder != null)
                {
                    context.OrderItems.RemoveRange(oldOrder.OrderItems);
                    context.Orders.Remove(oldOrder);
                    context.SaveChanges();
                }
            }
        }
    }
}

