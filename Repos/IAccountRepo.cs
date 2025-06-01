using BOs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repos
{
    public interface IAccountRepo
    {
        Task<Account?> GetAccountByEmailAsync(string email);
        Task<Role?> GetRoleByIdAsync(int roleId);
        Task<List<Account>> GetAllAccountsAsync();
        Task<bool> DeleteAccountAsync(int accountId);
        Task<bool> SignUpAsync(Account account);
        Task<bool> UpdateAccountAsync(Account account);
        Task<Account?> Login(string email, string password);
        Task<Account?> GetAccountByIdAsync(int accountId);
        Task<Account?> SearchAccountByIdAsync(int accountId);
        Task<List<Account>> SearchAccountsByFullNameAsync(string searchTerm);
        Task<bool> UpdateAccountStatusAsync(Account account, string status);

        Task SavePasswordResetTokenAsync(int accountId, string token, DateTime expiration);
        Task<bool> VerifyPasswordResetTokenAsync(int accountId, string token);
        Task InvalidatePasswordResetTokenAsync(int accountId, string token);

        Task<int> GetParentCountAsync();
        Task<int> GetNurseCountAsync();
    }

}
