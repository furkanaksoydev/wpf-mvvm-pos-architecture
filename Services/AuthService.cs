using Lavira.AkyaPOS.Core.Models;
using Lavira.AkyaPOS.Core.Security;
using Lavira.AkyaPOS.Repositories;
using Lavira.AkyaPOS.Views;

namespace Lavira.AkyaPOS.Services
{
    public class AuthService
    {
        private readonly UserRepository _userRepository = new();

        public User Login(string username, string password)
        {
            var user = _userRepository.GetByUsername(username);
            if (user == null)
                return null;

            var hash = PasswordHasher.Hash(password);
            return user.PasswordHash == hash ? user : null;
        }
    }
}