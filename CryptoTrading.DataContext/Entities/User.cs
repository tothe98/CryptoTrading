using CryptoTrading.DataContext.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.DataContext.Entities
{
    [Index(nameof(Email), IsUnique = true)]
    [Table("Users")]
    public class User :AbstractEntity
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public ERole Role { get; set; } = ERole.USER;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [NotMapped]
        public Wallet Wallet { get; set; }
    }
}
