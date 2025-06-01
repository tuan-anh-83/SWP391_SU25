using BOs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IAccountService
    {
        Task<Account?> GetAccountByEmailAsync(string email);
        Task<Account?> GetAccountByIdAsync(int accountId);
        Task<Account?> Login(string email, string password);
        Task<List<Account>> GetAllAccountsAsync();
        Task<List<Account>> SearchAccountsByFullNameAsync(string searchTerm);
        Task<Account?> SearchAccountByIdAsync(int accountId);
        Task<bool> DeleteAccountAsync(int accountId);
        Task<bool> SignUpAsync(Account account);
        Task<bool> UpdateAccountAsync(Account account);
        Task<bool> UpdateAccountStatusAsync(Account account, string status);
        Task<Role?> GetRoleByIdAsync(int roleId);
        Task SavePasswordResetTokenAsync(int accountId, string token, DateTime expiration);
        Task<bool> VerifyPasswordResetTokenAsync(int accountId, string token);
        Task InvalidatePasswordResetTokenAsync(int accountId, string token);
        Task<int> GetParentCountAsync();
        Task<int> GetNurseCountAsync();

        // OTP (cache)
        Task<bool> SaveOtpAsync(string email, string otp, DateTime expiration);
        Task<bool> VerifyOtpAsync(string email, string otp);
        Task InvalidateOtpAsync(string email);
        Task<OtpInfo> GetCurrentOtpAsync(string email);
    }

}
