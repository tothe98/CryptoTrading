using AutoMapper;
using CryptoTrading.DataContext;
using CryptoTrading.DataContext.Dtos;
using CryptoTrading.DataContext.Entities;
using CryptoTrading.Services.Security;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Services
{
    public interface IUserService
    {
        Task<bool> RegisterUser(UserCreateDto user);
        Task<string> LoginUser(UserLoginDto user);
        Task<UserDataDto> GetUser(int id);
        Task<UserDataDto> UpdateUser(UserUpdateDto userDto);
        Task<bool> DeleteUser(int id);
    }
    public class UserService : IUserService
    {
        private readonly SQL _context;
        private readonly IMapper _mapper;
        private readonly TokenHandlerService _tokenHandlerService;
        public UserService(SQL context, IMapper mapper, TokenHandlerService tokenHandlerService)
        {
            _context = context;
            _mapper = mapper;
            _tokenHandlerService = tokenHandlerService;
        }

        public async Task<bool> DeleteUser(int id)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user != default)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<UserDataDto> GetUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user != default)
                return _mapper.Map<UserDataDto>(user);
            throw new Exception("User not found");
        }

        public async Task<string> LoginUser(UserLoginDto user)
        {
            if (user.Email != null && user.Password != null)
            {

                var getUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
                if (getUser != default)
                {
                    Encryption enc = Encryption.Initialize(getUser.Password);
                    if (enc.Validate(user.Password))
                    {
                        string token = _tokenHandlerService.GenerateToken(getUser);
                        return token;
                    }
                    throw new Exception("Username and Password are not correct!");

                }
                throw new Exception("Username and Password are not correct!");
            }
            else
            {
                throw new Exception("All fields are required!");
            }
        }

        public async Task<bool> RegisterUser(UserCreateDto user)
        {
            User newUser = _mapper.Map<User>(user);
            if (user.Password.Length < 8)
            {
                throw new Exception("Password must be at least 8 characters long");
            }
            if (user.Password != user.PasswordConfirm)
            {
                throw new Exception("Passwords do not match");
            }
            if (UserExists(newUser.Email))
            {
                throw new Exception("User already exists");
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    Encryption enc = Encryption.Initialize();
                    newUser.Password = enc.EncyptPassword(newUser.Password);
                    await _context.Users.AddAsync(newUser);

                    await _context.SaveChangesAsync();

                    Wallet wallet = new Wallet { UserId = newUser.Id, Balance = 200};
                    await _context.Wallets.AddAsync(wallet);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception($"Server error: {ex.Message}");
                }
            }

        }

        public async Task<UserDataDto> UpdateUser(UserUpdateDto userUpdateDto)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userUpdateDto.Id);
            if (user != default)
            {
                if (user.Email != userUpdateDto.Email && UserExists(userUpdateDto.Email))
                {
                    throw new Exception("Email exists!");
                }

                try
                {
                    _mapper.Map(userUpdateDto, user);
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();

                    return _mapper.Map<UserDataDto>(user);

                }
                catch (Exception ex)
                {
                    throw new Exception($"Server error: {ex.Message}");
                }
            }
            throw new Exception("User not found");
        }

        private bool UserExists(string email)
        {
            return _context.Users.Any(e => e.Email == email);
        }
        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
