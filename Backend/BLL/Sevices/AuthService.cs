using AutoMapper;
using BLL.Helpers;
using DAL.Entities;
using DAL.Repositories;
using Profile = DAL.Entities.Profile;

namespace BLL.Sevices;

public class AuthService(UsersRepository usersRepository,
      ProfilesRepository profilesRepository,  IMapper mapper,
    PasswordManager passwordManager, EmailService emailService)
{
    public async Task<string?> RegisterUserAsync(User user)
    {
        var userByEmail = await usersRepository.GetUserByEmailAsync(user.Email);
        var userByUserName = await usersRepository.GetUserByUserNameAsync(user.UserName);
        if (userByEmail != null || userByUserName != null) return null;
        user.Password = passwordManager.HashPassword(user.Password);
        user.EmailResetToken = emailService.GenerateEmailConfirmationToken();
        
        //TODO change when email service is ready
        user.IsVerified = true;
        
        var res = await usersRepository.CreateUserAsync(user);
        userByUserName = await usersRepository.GetUserByUserNameAsync(user.UserName);
        var profile = new Profile()
        {
            Id = userByUserName.Id,
            Gender = "male",
            Age = 18,
            ProfilePictureId = 501,
        };
        await profilesRepository.CreateProfileAsync(profile);
        return res == null ? null : user.EmailResetToken;
    }
    
    public async Task<bool> AuthenticateUser(string username, string password)
    {
        var user = await usersRepository.GetUserByUserNameAsync(username);
        if (user == null) return false;
        var isValidPassword = passwordManager.VerifyPassword(password, user!.Password);
        return isValidPassword;
    }
    
    public async Task<bool> ConfirmEmailAsync(int id)
    {
        //TODO add validation
        
        var user = await usersRepository.GetUserByIdAsync(id);
        if (user.IsVerified) return false;
        user.IsVerified = false;
        await usersRepository.UpdateUserAsync(id, user);
        return true;
    }
    
}