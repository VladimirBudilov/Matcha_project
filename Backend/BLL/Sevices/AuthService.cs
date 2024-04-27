using AutoMapper;
using BLL.Helpers;
using DAL.Entities;
using DAL.Repositories;

namespace BLL.Sevices;

public class AuthService(UserRepository userRepository, IMapper mapper, PasswordManager passwordManager, EmailService emailService)
{
    public async Task<string?> RegisterUserAsync(User user)
    {
        //TODO add validation
        
        user.Password = passwordManager.HashPassword(user.Password);
        user.ResetToken = emailService.GenerateEmailConfirmationToken();
        user.IsVerified = false;
        user.CreatedAt = DateTime.Now;
        user.UpdatedAt = DateTime.Now;
        var res = await userRepository.AddUserAsync(user);
        return res == null ? null : user.ResetToken;
    }
    
    public async Task<bool> AuthenticateUser(string username, string password)
    {
        //TODO add validation
        
        var user = await userRepository.GetUserByUserNameAsync(username);
        var isValidPassword = passwordManager.VerifyPassword(password, user!.Password);
        return isValidPassword;
    }
    
    public async Task<bool> ConfirmEmailAsync(long id)
    {
        //TODO add validation
        
        var user = await userRepository.GetUserByIdAsync(id);
        if (user.IsVerified == true) return false;
        user.IsVerified = false;
        await userRepository.UpdateUserAsync(id, user);
        return true;
    }
}