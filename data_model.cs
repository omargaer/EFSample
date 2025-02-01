namespace EFSample;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

public class Book
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; }
    
    [Required]
    public int AuthorId { get; set; }
    public Author Author { get; set; }
    
    
    public List<BookGenre> BookGenres { get; set; } = new();
    
    
    [Required]
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal CurrentPrice { get; set; }
    
    public int StockQuantity { get; set; }
    
    [MaxLength(1000)]
    public string Description { get; set; }
    
    public string ISBN { get; set; }
    public int PublicationYear { get; set; }
    public List<PriceHistory> PriceHistory { get; set; } = new();
    public List<BookReview> Reviews { get; set; } = new();
}

public class BookGenre
{
    public int BookId { get; set; }
    public Book Book { get; set; }
    
    public int GenreId { get; set; }
    public Genre Genre { get; set; }
}

public class Genre
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
    
    [MaxLength(500)]
    public string Description { get; set; }
    public List<BookGenre> BookGenres { get; set; } = new();
}

public class Author
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
    
    [MaxLength(1000)]
    public string Biography { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string CountryOfOrigin { get; set; }
    public List<Book> Books { get; set; } = new();
}

public class PriceHistory
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public Book Book { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}

public class Supplier
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string CompanyName { get; set; }
    
    [MaxLength(200)]
    public string Address { get; set; }
    public string ContactPerson { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public List<Book> Books { get; set; } = new();
}

public class Order
{
    public int Id { get; set; }
    
    [Required]
    public int ClientId { get; set; }
    public Client Client { get; set; }
    
    [Required]
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; }
    
    public DateTime OrderDate { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }
    
    public OrderStatus Status { get; set; }
    public string DeliveryAddress { get; set; }
    public List<OrderItem> OrderItems { get; set; } = new();
    public List<OrderStatusHistory> StatusHistory { get; set; } = new();
}

public enum OrderStatus
{
    Created,
    Confirmed,
    Processing,
    Shipped,
    Delivered,
    Cancelled
}

public class OrderStatusHistory
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; }
    public OrderStatus Status { get; set; }
    public string Comment { get; set; }
    public DateTime ChangedAt { get; set; }
}

public class Client
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public List<Order> Orders { get; set; } = new();
    public List<BookReview> Reviews { get; set; } = new();
}

public class Employee
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public DateTime HireDate { get; set; }
    public string Position { get; set; }
    public List<Order> Orders { get; set; } = new();
}

public class OrderItem
{
    public int Id { get; set; }
    
    [Required]
    public int BookId { get; set; }
    public Book Book { get; set; }
    
    [Required]
    public int Quantity { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }
    
    [Required]
    public int OrderId { get; set; }
    public Order Order { get; set; }
}

public class BookReview
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public Book Book { get; set; }
    public int ClientId { get; set; }
    public Client Client { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}
