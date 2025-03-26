using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.DataContext
{
    public class SQL: DbContext
    {
        public SQL(DbContextOptions options) : base(options) { }
    }
}
