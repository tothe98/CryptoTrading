using AutoMapper;
using CryptoTrading.DataContext;
using CryptoTrading.DataContext.Dtos;
using CryptoTrading.DataContext.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Services
{
    public interface IAlertService
    {
        Task<AlertDto> CreateAlert(CreateAlertDto alert);
        Task<bool> DeleteAlert(int alertId);
        Task<List<AlertDetailsDto>> GetAlerts(int userid);
        //Task<List<AlertEntity>> GetAlertsByUserIdAsync(int userId);
        //Task<bool> TriggerAlertAsync(int alertId);
    }
    public class AlertService : IAlertService
    {
        private readonly SQL _context;
        private readonly IMapper _mapper;
        public AlertService(SQL context, IMapper mapper)
        {
            this._context = context;
            _mapper = mapper;
        }

        public async Task<AlertDto> CreateAlert(CreateAlertDto alert)
        {
            if (await _context.Alerts.AnyAsync(a => a.UserId == alert.UserId && a.CryptoCurrencyId == alert.CryptoCurrencyId && alert.LimitType == a.LimitType))
            {
                var oldAlert = await _context.Alerts.FirstOrDefaultAsync(a => a.UserId == alert.UserId && a.CryptoCurrencyId == alert.CryptoCurrencyId && alert.LimitType == a.LimitType);
                if (oldAlert != null)
                {
                    oldAlert.Price = alert.Price;
                    oldAlert.IsActive = true;
                    await _context.SaveChangesAsync();
                    return _mapper.Map(oldAlert, new AlertDto());
                }
                else
                {
                    throw new Exception("Server error!");
                }
            }
            else
            {

                var newAlert = _mapper.Map<Alert>(alert);
                await _context.Alerts.AddAsync(newAlert);
                await _context.SaveChangesAsync();
                return _mapper.Map(newAlert, new AlertDto());
            }
        }

        public async Task<bool> DeleteAlert(int alertId)
        {
            var alert = await _context.Alerts.FirstOrDefaultAsync(a => a.Id == alertId);
            if (alert != null)
            {
                alert.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                throw new Exception("Alert not found");
            }

        }

        public async Task<List<AlertDetailsDto>> GetAlerts(int userid)
        {
            return _mapper.Map<List<AlertDetailsDto>>(await _context.Alerts.Include(a => a.CryptoCurrency).Where(x => x.UserId == userid && x.IsActive == true).ToListAsync());
        }
    }
}
