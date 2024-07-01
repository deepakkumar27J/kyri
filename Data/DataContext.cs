using kyri.Model;
using Microsoft.EntityFrameworkCore;

namespace kyri.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }
        public DbSet<User> Users { get; set; }
    }
}
