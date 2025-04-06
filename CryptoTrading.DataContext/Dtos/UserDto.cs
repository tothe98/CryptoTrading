using CryptoTrading.DataContext.Entities;
using CryptoTrading.DataContext.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.DataContext.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public WalletDto Wallet { get; set; }
    }

    public class UserDataDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public ERole Role { get; set; }
    }

    public class UserCreateDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
        public ERole Role { get; set; }
    }

    public class UserLoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserUpdateDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public ERole Role { get; set; }
    }

    public class UserPasswordUpdateDto
    {
        public int Id { get; set; }
        public string OldPassword { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
    }


}
