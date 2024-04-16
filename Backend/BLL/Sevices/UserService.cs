using AutoMapper;
using BLL.Models;
using DAL.Entities;
using DAL.Repositories;

namespace BLL.Sevices
{
    public class UserService(UserRepository userRepository, IMapper mapper)
    {
        
        public async Task<UserModel> GetUserByIdAsync(int userId)
        {
            var user = await userRepository.GetUserByIdAsync(userId);
            var output = mapper.Map<UserModel>(user);
            return output;
        }
        
        public async Task<IEnumerable<UserModel>> GetAllUsersAsync()
        {
            var allUsers = await userRepository.GetAllUsers();
            var output = new List<UserModel>();
            return output;
        }

        public async Task CreateUserAsync(UserModel userModel)
        {
            var User = mapper.Map<UserEntity>(userModel);
            await userRepository.AddUserAsync(User);
        }
    }
}
