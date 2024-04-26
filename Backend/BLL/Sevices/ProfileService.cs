using AutoMapper;
using BLL.Models;
using DAL.Repositories;

namespace BLL.Sevices;

public class ProfileService(UserRepository userRepository, IMapper mapper)
{
    private readonly UserRepository _userRepository = userRepository;
    private readonly IMapper _mapper = mapper;
        
    public async Task<UserModel> GetFullDataAsync(int id)
    {
        var entity = await _userRepository.GetFullDataAsync(id);
        var model = _mapper.Map<UserModel>(entity);
        return model;
    }

    public async Task<IEnumerable<string>> GetAllUsersAsync()
    {
        throw new NotImplementedException();
    }
}