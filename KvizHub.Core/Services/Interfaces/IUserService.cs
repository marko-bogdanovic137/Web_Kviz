using KvizHub.Models.DTOs;
using KvizHub.Models.DTOs.KvizHub.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KvizHub.Core.Services.Interfaces
{
	public interface IUserService
	{
		Task<UserDTO> GetUserByIdAsync(int userId);
		Task<UserDTO> UpdateUserAsync(int userId, UserUpdateDTO updateDto);
		Task<bool> DeleteUserAsync(int userId);
	}
}
