using ReimbursementTrackingApplication.Models;
using ReimbursementTrackingApplication.Models.DTOs;

namespace ReimbursementTrackingApplication.Interfaces
{
    public interface IBankService
    {
        Task<SuccessResponseDTO<ResponseBankDTO>> AddBankAccountAsync(BankDTO bankAccount);
        Task<SuccessResponseDTO<ResponseBankDTO>> UpdateBankAccountAsync(int id, BankDTO bankAccount);
        Task<SuccessResponseDTO<int>> DeleteBankAccountAsync(int id);
        Task<SuccessResponseDTO<ResponseBankDTO>> GetBankAccountByIdAsync(int id);
        Task<PaginatedResultDTO<ResponseBankDTO>> GetAllBankAccountsAsync(int pageNumber, int pageSize);
        Task<SuccessResponseDTO<ResponseBankDTO>> GetBankAccountByUserIdAsync(int userId);
    }
}
