using AutoMapper;
using CryptoTrading.DataContext.Dtos;
using CryptoTrading.DataContext.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Services
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {

            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, UserDataDto>().ReverseMap();
            CreateMap<UserCreateDto, User>();
            CreateMap<UserLoginDto, User>();
            CreateMap<UserUpdateDto, User>();

            CreateMap<Wallet, WalletDto>().ReverseMap();
            CreateMap<Wallet, WalletDetailDto>().ReverseMap();
            CreateMap<WalletHolding, WalletHoldingDto>().ReverseMap();

            CreateMap<CryptoCurrency, CryptoDto>().ReverseMap();
            CreateMap<CryptoCreateDto, CryptoCurrency>()
                .ForMember(dest => dest.CurrentPrice, act => act.MapFrom(a => a.InitialPrice));
            CreateMap<Transaction, TransactionDto>()
                .ForMember(dest => dest.TransactionType, act=>act.MapFrom(t=>t.TransactionType.ToString()));

            CreateMap<Wallet, PortfolioDto>().ReverseMap();
            CreateMap<WalletHolding, WalletHoldingAmountDto>().ReverseMap();

            CreateMap<CryptoPriceFluctuation, CryptoFluctuationDto>()
                .ForMember(dest => dest.Name, act => act.MapFrom(c => c.CryptoCurrency.Name));

            CreateMap<Transaction, TransactionDetailDto>()
                .ForMember(dest => dest.CryptoCurrency, act => act.MapFrom(t => t.CryptoCurrency))
                .ForMember(dest => dest.TransactionType, act => act.MapFrom(t => t.TransactionType.ToString()));

        }
    }
}
