using Api_Rest.Controllers.Models;
using Api_Rest.Models;
using Microsoft.EntityFrameworkCore;


namespace Api_Rest.Data
{
    public class ApplicationDbContext : DbContext 
    {
        public DbSet<Produto> Produtos {get; set;}
        public DbSet<Usuario> Usuarios {get; set;}
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }
}