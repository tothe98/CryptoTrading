using CryptoTrading.Services.Services;

namespace CryptoTrading.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddLocalServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AutoMapperProfile));
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IWalletService, WalletService>();
        }
    }
}
