using System;
using Test.EF;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Test
{
    public interface IUserService : IDisposable
    {
        User Create(int id);

        User Get(int id);

        Task<User> GetAsync(int id);
    }

    public class UserService : IUserService
    {
        private readonly TestContext _db;

        public UserService(TestContext db)
        {
            _db = db;
        }

        public User Create(int id)
        {
            var user = new User(id, string.Empty);
            _db.Users.Add(user);
            _db.SaveChanges();
            return user;
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        public User Get(int id)
        {
            return _db.Users.FirstOrDefault(x => x.Id == id);
        }

         public Task<User> GetAsync(int id)
        {
            return _db.Users.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}