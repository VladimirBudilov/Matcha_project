using AutoMapper;
using BLL.Helpers;
using BLL.Models;
using DAL.Entities;
using DAL.Repositories;

namespace BLL.Sevices;

public class AuthService(UserRepository userRepository, IMapper mapper, PasswordManager passwordManager, EmailService emailService)
{
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
        var user = await userRepository.GetUserByUserNameAsync(username);
        bool isValidPassword = passwordManager.VerifyPassword(password, user.Password);
        return isValidPassword;
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