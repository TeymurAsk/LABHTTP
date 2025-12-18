using LABHTTP.Data;

namespace LABHTTP.Repository
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        void AddAsync(User user);
    }
}
