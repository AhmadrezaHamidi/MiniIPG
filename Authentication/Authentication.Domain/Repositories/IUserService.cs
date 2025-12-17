using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackOffice.Domain.Entities.Users;

namespace Authentication.Domain.Repositories;

public interface IUserService
{
    Task<User> GetById(int id);
    Task<User> Create(User user, string role);
    Task<bool> Delete(int id);
    Task<bool> Update(User model);
}
