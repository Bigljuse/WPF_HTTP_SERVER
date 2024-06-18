using Microsoft.EntityFrameworkCore;
using WPF_HTTP_SERVER.DataBase.Models;

namespace WPF_HTTP_SERVER.DataBase
{
    public class DataBaseConfiguration : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source = DataBase.db");
        }

        public DbSet<UserModel> Users { get; set; }

        public DbSet<PredictionModel> Predictions { get; set; }
    }
}
