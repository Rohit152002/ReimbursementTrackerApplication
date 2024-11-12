using AutoMapper;
using ReimbursementTrackingApplication.Models;
using ReimbursementTrackingApplication.Models.DTOs;

namespace ReimbursementTrackingApplication.Mapper
{
    public class BankProfile:Profile
    {
        public BankProfile()
        {
            CreateMap<BankDTO, BankAccount>();
            CreateMap<BankDTO, ResponseBankDTO>();
            CreateMap<BankAccount, BankDTO>();
            CreateMap<ResponseBankDTO, BankDTO>();

        }
    }
}
