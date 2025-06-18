using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AuthTest_RoleBased.Models
{
    public class AppUser
    {

        public int Id { get; set; }
        [Required, StringLength(50), Display(Name = "User Name")]
        public string UserName { get; set; } = default!;
        [Required, StringLength(50)]
        public string Password { get; set; } = default!;
        public int Role { get; set; }
        public bool IsActive { get; set; }

    }
    public class Product
    {
        public int Id { get; set; }
        [Required, StringLength(50)]
        public string Name { get; set; } = default!;
        public string? Photo { get; set; }
        public string Unit { get; set; } = default!;
        public double Price { get; set; }
        public double Quantity { get; set; }

        public virtual ICollection<SalesItem> SalesItems { get; set; } = new List<SalesItem>();

    }
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Mobile { get; set; } = default!;
        public string Address { get; set; } = default!;
        public virtual ICollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();

    }
    public class SalesOrder
    {
        public int Id { get; set; }
        public string OrderNo { get; set; } = default!;
        public DateTime OrderDate { get; set; }
        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual ICollection<SalesItem> SalesItems { get; set; } = new List<SalesItem>();
    }
    public class SalesItem
    {
        public int Id { get; set; }
        [ForeignKey("SalesOrder")]
        public int SalesOrderId { get; set; }

        public double UnitPrice { get; set; }
        public double Quantity { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        //nev
        public virtual SalesOrder? SalesOrder { get; set; }
        public virtual Product? Product { get; set; }
    }
    //public class ApplicationDbContext : DbContext
    //{
    //    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    //    {

    //    }
        
    //}
}
