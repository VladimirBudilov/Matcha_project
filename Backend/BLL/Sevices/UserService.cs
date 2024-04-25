using AutoMapper;
using BLL.Helpers;
using BLL.Models;
using DAL.Entities;
using DAL.Repositories;

namespace BLL.Sevices
{
    public class UserService(UserRepository userRepository, IMapper mapper, PasswordManager passwordManager, EmailService emailService)
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

        public async Task<string> RegisterUserAsync(UserModel userModel)
        {
            var user = mapper.Map<UserEntity>(userModel);
            user.Password = passwordManager.HashPassword(user.Password);
            user.ResetToken = emailService.GenerateEmailConfirmationToken();
            user.IsVerified = false;
            await userRepository.AddUserAsync(user);
            return user.ResetToken;
        }
        
        public async Task<bool> AuthenticateUser(string username, string password)
        {
            // Retrieve user from database based on username/email
            var user = await userRepository.GetUserByUserNameAsync(username);
            
            // Verify the password
            bool isValidPassword = passwordManager.VerifyPassword(password, user.Password);

            return isValidPassword;
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

        public async Task<UserModel> GetUserByEmailAsync(string email)
        {
            var user = await userRepository.GetUserByEmailAsync(email);
            var output = mapper.Map<UserModel>(user);
            return output;
        }

        public async Task<bool> ConfirmEmailAsync(int id)
        {
            var user = await userRepository.GetUserByIdAsync(id);
            if (user.IsVerified == true) return false;
            user.IsVerified = true;
            await userRepository.UpdateUserAsync(id, user);
            return true;
        }
    }
}
