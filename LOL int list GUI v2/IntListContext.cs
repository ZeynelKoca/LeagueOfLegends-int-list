using Microsoft.EntityFrameworkCore;

namespace LOL_int_list_GUI_v2
{
    class IntListContext : DbContext
    {
        public DbSet<Summoner> Summoners { get; set; }
        public DbSet<LockFile> LockFileLocation { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsbuilder)
        {
            optionsbuilder.UseSqlServer(
                    @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=My_int_list_DB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }
    }
}
