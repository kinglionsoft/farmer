using Microsoft.EntityFrameworkCore;

namespace Test.EF
{
    public class TestContext : DbContext
    {
        public TestContext(DbContextOptions options) : base(options)
        {
        }

        protected TestContext()
        {
        }

        public virtual DbSet<User> Users { get; set; }
    }
}