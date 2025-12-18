using LABHTTP.Data;
using Microsoft.EntityFrameworkCore;

namespace LABHTTP.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public void AddAsync(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

    }
}
