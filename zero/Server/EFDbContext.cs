using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using zero.Models.DbContext;

namespace zero
{
    class EFDbContext : DbContext
    {
        public EFDbContext() : base("ZeroContext") { }        
        public DbSet<Quotation> Quotations { get; set; }
    }
}
