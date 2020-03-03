using Api_Rest.Controllers.Models;
using Microsoft.EntityFrameworkCore;


namespace Api_Rest.Data
{
    public class ApplicationDbContext : DbContext 
    {
        public DbSet<Produto> Produtos {get; set;}
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }
}