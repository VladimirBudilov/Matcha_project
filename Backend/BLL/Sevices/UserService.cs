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
        
        public async Task<UserModel> GetUserByUserNameAsync(string userName)
        {
            var user = await userRepository.GetUserByUserNameAsync(userName);
            var output = mapper.Map<UserModel>(user);
            return output;
        }
        
        public async Task<IEnumerable<UserModel>> GetAllUsersAsync()
        {
            var allUsers = await userRepository.GetAllUsers();
            var output = mapper.Map<IEnumerable<UserModel>>(allUsers);
            return output;
        }

        public async Task CreateUserAsync(UserModel userModel)
        {
            var User = mapper.Map<UserEntity>(userModel);
            await userRepository.AddUserAsync(User);
        }

        public async Task UpdateUserAsync(int id, UserModel userModel)
        {
            var User = mapper.Map<UserEntity>(userModel);
            await userRepository.UpdateUserAsync(id, User);
        }

        public async Task DeleteUserAsync(int id)
        {
            await userRepository.DeleteUserAsync(id);
        }
    }
}
