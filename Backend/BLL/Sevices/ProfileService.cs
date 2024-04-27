using AutoMapper;
using DAL.Entities;
using DAL.Repositories;

namespace BLL.Sevices;

public class ProfileService(UserRepository userRepository, IMapper mapper)
{
    private readonly UserRepository _userRepository = userRepository;
    private readonly IMapper _mapper = mapper;
        
    public async Task<User> GetFullDataAsync(int id)
    {
        return await _userRepository.GetFullDataAsync(id);
    }

    public async Task<IEnumerable<string>> GetAllUsersAsync()
    {
        throw new NotImplementedException();
    }
}