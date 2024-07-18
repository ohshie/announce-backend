using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using announce_backend.DAL.AnnounceDbContext;
using announce_backend.DAL.Repository.IRepository;
using announce_backend.DAL.UnitOfWork;
using announce_backend.Models;
using announce_backend.Models.DTOs;
using Microsoft.IdentityModel.Tokens;

namespace announce_backend.Business.Auth.Authorization;

public class AuthManager(IConfiguration configuration, IUserRepository userRepository, IUnitOfWork<AnnounceDbContext> unitOfWork)
{
    public async Task<RegisteredUser?> CreateNewUser(RegisterModel incommingUser)
    {
        var allUsers = await userRepository.GetAll();
        if (allUsers is null)
        {
            return null;
        }
        
        if (allUsers.Any(u => u.Username == incommingUser.Username))
        {
            return null;
        }
        
        CreatePasswordHash(incommingUser.Password, out byte[] passwordHash, out byte[] passwordSalt);

        var newUser = new User()
        {
            Username = incommingUser.Username,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };

        await userRepository.Add(newUser);
        await unitOfWork.Save();

        return new RegisteredUser()
        {
            Token = CreateToken(newUser),
            User = new UserDto { Username = newUser.Username }
        };
    }
    
    public async Task<RegisteredUser?> Login(LoginModel loginUser)
    {
        var user = await userRepository.GetByName(loginUser);
        if (user is null)
        {
            return null;
        }

        if (!VerifyPasswordHash(loginUser.Password, user.PasswordHash, user.PasswordSalt))
        {
            return null;
        }

        return new RegisteredUser()
        {
            Token = CreateToken(user),
            User = new UserDto { Username = user.Username }
        };
    }
    
    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
    
    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512(passwordSalt))
        {
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != passwordHash[i]) return false;
            }
        }
        return true;
    }
    
    private string CreateToken(User user)
    {
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Username),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}