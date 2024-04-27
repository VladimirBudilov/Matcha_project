using AutoMapper;
using DAL.Entities;
using DAL.Repositories;

namespace BLL.Sevices;

public class UserService(UserRepository userRepository, IMapper mapper)
{
        
    public async Task<User?> GetUserByIdAsync(int userId)
    {
        //TODO add validation
        
        var output = await userRepository.GetUserByIdAsync(userId);

        if (output == null)
        {
            //TODO add logging
            return null;
        }

        return output;
    }
        
    public async Task<User?> GetUserByUserNameAsync(string userName)
    {
        //TODO add validation
        
        var output = await userRepository.GetUserByUserNameAsync(userName);
        
        if (output == null)
        {
            //TODO add logging
            return null;
        }

        return output;
    }
        
    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {

        var output = await userRepository.GetAllUsers();
        
        if (output == null)
        {
            //TODO add logging
            return null;
        }

        return output;
    }

    public async Task<User?> UpdateUserAsync(int id, User userModel)
    {
        //TODO add validation
        
        userModel.UpdatedAt = DateTime.Now;
        var output = await userRepository.UpdateUserAsync(id, userModel);
        
        if (output == null)
        {
            //TODO add logging
            return null;
        }
        
        return output;
    }

    public async Task<User?> DeleteUserAsync(int id)
    {
        //TODO add validation
        
        var output = await userRepository.DeleteUserAsync(id);
        
        if (output == null)
        {
            //TODO add logging
            return null;
        }

        return output;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        //TODO add validation
        
        var output = await userRepository.GetUserByEmailAsync(email);
        
        if (output == null)
        {
            //TODO add logging
            return null;
        }

        return output;
    }
    
    public async Task<User> GetFullUserDataAsync(int userId)
    {
        //TODO add validation
        
        var output = await userRepository.GetFullDataAsync(userId);
        
        if (output == null)
        {
            //TODO add logging
            return null;
        }

        return output;
    }
}