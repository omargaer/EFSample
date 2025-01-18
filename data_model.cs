namespace EFSample;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int AuthorId { get; set; } // Внешний ключ на автора
    public Author Author { get; set; }
    public int GenreId { get; set; } // Внешний ключ на жанр
    public Genre Genre { get; set; }
    public int SupplierId { get; set; } // Внешний ключ на поставщика
    public Supplier Supplier { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; } // Сколько книг на складе
}

public class Author
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Book> Books { get; set; } = new List<Book>(); // Книги, написанные автором
}

public class Genre
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Book> Books { get; set; } = new List<Book>(); // Книги, принадлежащие к этому жанру
}

public class Supplier
{
    public int Id { get; set; }
    public string CompanyName { get; set; }
    public List<Book> Books { get; set; } = new List<Book>(); // Книги, предоставляемые поставщиком
}

public class Order
{
    public int Id { get; set; }
    public int ClientId { get; set; } // Внешний ключ на клиента
    public Client Client { get; set; }
    public int EmployeeId { get; set; } // Внешний ключ на сотрудника
    public Employee Employee { get; set; }
    public DateTime OrderDate { get; set; }
    public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>(); // Позиции заказа
}

public class Client
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Order> Orders { get; set; } = new List<Order>(); // Заказы клиента
}

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Order> Orders { get; set; } = new List<Order>(); // Заказы, обработанные сотрудником
}

public class OrderItem
{
    public int Id { get; set; }
    public int BookId { get; set; } // Внешний ключ на книгу
    public Book Book { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; } // Цена за единицу
    public int OrderId { get; set; } // Внешний ключ на заказ
    public Order Order { get; set; }
}