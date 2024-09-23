using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Order.API.Models.Context
{
    public class OrderApiDbContext : DbContext
    {
        public OrderApiDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderOutbox> OrderOutboxes { get; set; }

    }
}
