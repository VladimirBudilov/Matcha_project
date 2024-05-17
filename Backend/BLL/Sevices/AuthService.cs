using AutoMapper;
using BLL.Helpers;
using DAL.Entities;
using DAL.Repositories;
using Profile = DAL.Entities.Profile;

namespace BLL.Sevices;

public class AuthService(UserRepository userRepository,
      ProfileRepository profileRepository,  IMapper mapper,
    PasswordManager passwordManager, EmailService emailService)
{
    public async Task<string?> RegisterUserAsync(User user)
    {
        //TODO add validation
        var userByEmail = await userRepository.GetUserByEmailAsync(user.Email);
        var userByUserName = await userRepository.GetUserByUserNameAsync(user.UserName);
        if (userByEmail != null || userByUserName != null) return null;
        user.Password = passwordManager.HashPassword(user.Password);
        user.EmailResetToken = emailService.GenerateEmailConfirmationToken();
        user.IsVerified = false;
        var res = await userRepository.CreateUserAsync(user);
        userByUserName = await userRepository.GetUserByUserNameAsync(user.UserName);
        var profile = new Profile()
        {
            ProfileId = userByUserName.UserId,
            Gender = "male",
            Age = 18,
        };
        await profileRepository.CreateProfileAsync(profile);
        return res == null ? null : user.EmailResetToken;
    }
    
    public async Task<bool> AuthenticateUser(string username, string password)
    {
        var user = await userRepository.GetUserByUserNameAsync(username);
        if (user == null) return false;
        var isValidPassword = passwordManager.VerifyPassword(password, user!.Password);
        return isValidPassword;
    }
    
    public async Task<bool> ConfirmEmailAsync(int id)
    {
        //TODO add validation
        
        var user = await userRepository.GetUserByIdAsync(id);
        if (user.IsVerified == true) return false;
        user.IsVerified = false;
        await userRepository.UpdateUserAsync(id, user);
        return true;
    }
    
}