﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Repositories;

namespace BLL.Sevices
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        
        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        
        public async Task<IEnumerable<string>> GetAllUsersAsync()
        {
            throw new NotImplementedException();
        }
    }
}
