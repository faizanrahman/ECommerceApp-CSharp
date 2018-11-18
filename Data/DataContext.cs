using ECommerceApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<User> Users {get;set;}
        public DbSet<Product> Products {get;set;}
    }
}