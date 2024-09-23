using Microsoft.EntityFrameworkCore;
using Stock.Service.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Service.Models.Contexts
{
    public class StockApiDbContext : DbContext
    {
        public StockApiDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<OrderInbox> OrderInboxes { get; set; }
    }
}
