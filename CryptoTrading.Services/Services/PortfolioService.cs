using AutoMapper;
using CryptoTrading.DataContext;
using CryptoTrading.DataContext.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Services
{
    public interface IPortfolioService
    {
        Task<PortfolioDto> GetPortfolio(int userId);
    }
    public class PortfolioService : IPortfolioService
    {
        private readonly SQL _context;
        private readonly IMapper _mapper;
        public PortfolioService(SQL context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PortfolioDto> GetPortfolio(int userId)
        {
            var userWallet = await _context.Wallets.Include(w => w.Holdings).ThenInclude(h => h.CryptoCurrency).FirstOrDefaultAsync(w => w.UserId == userId);
            if (userWallet != default)
            {
                var portfolio = _mapper.Map<PortfolioDto>(userWallet);
                portfolio.TotalPrice = userWallet.Holdings.Sum(h => h.CryptoCurrency.CurrentPrice * h.Amount);
                return portfolio;
            }
            throw new Exception("Portfolio not found");
        }
    }
}
