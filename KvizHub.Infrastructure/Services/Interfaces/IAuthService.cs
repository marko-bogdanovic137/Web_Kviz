using KvizHub.Models.DTOs;
using KvizHub.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KvizHub.Infrastructure.Services.Interfaces
{
	public interface IAuthService
	{
		Task<LoginResponseDTO> RegisterAsync(RegisterRequestDTO registerDto);
		Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginDto);
		Task<bool> UserExistsAsync(string username, string email);
	}
}
