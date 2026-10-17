using Microsoft.EntityFrameworkCore;
using CollegeManagementApi.Models;


namespace CollegeManagementApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {}
        public DbSet<College> StudentsDetails {get; set;}
         public DbSet<User> Users { get; set; }
        
    }
}