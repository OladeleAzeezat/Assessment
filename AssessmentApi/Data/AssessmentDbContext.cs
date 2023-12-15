using AssessmentApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AssessmentApi.Data
{
    public class AssessmentDbContext : DbContext 
    {
        public AssessmentDbContext(DbContextOptions<AssessmentDbContext> options) : base(options) 
        { 
        }

        public DbSet<Employee> Employees { get; set; }
    }
}
