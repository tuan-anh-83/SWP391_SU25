using BOs.Models;
using Microsoft.Extensions.Caching.Distributed;
using Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepo _accountRepo;
        private readonly IDistributedCache _cache;

        public AccountService(IAccountRepo accountRepo, IDistributedCache cache)
        {
            _accountRepo = accountRepo;
            _cache = cache;
        }

        private string GetOtpCacheKey(string email) => $"OTP_{email}";

        public async Task<Account?> GetAccountByEmailAsync(string email)
        {
            return await _accountRepo.GetAccountByEmailAsync(email);
        }

        public async Task<Account?> GetAccountByIdAsync(int accountId)
        {
            return await _accountRepo.GetAccountByIdAsync(accountId);
        }

        public async Task<Account?> Login(string email, string password)
        {
            return await _accountRepo.Login(email, password);
        }

        public async Task<List<Account>> GetAllAccountsAsync()
        {
            return await _accountRepo.GetAllAccountsAsync();
        }

        public async Task<List<Account>> SearchAccountsByFullNameAsync(string searchTerm)
        {
            return await _accountRepo.SearchAccountsByFullNameAsync(searchTerm);
        }

        public async Task<Account?> SearchAccountByIdAsync(int accountId)
        {
            return await _accountRepo.SearchAccountByIdAsync(accountId);
        }

        public async Task<bool> DeleteAccountAsync(int accountId)
        {
            return await _accountRepo.DeleteAccountAsync(accountId);
        }

        public async Task<bool> SignUpAsync(Account account)
        {
            return await _accountRepo.SignUpAsync(account);
        }

        public async Task<bool> UpdateAccountAsync(Account account)
        {
            return await _accountRepo.UpdateAccountAsync(account);
        }

        public async Task<bool> UpdateAccountStatusAsync(Account account, string status)
        {
            return await _accountRepo.UpdateAccountStatusAsync(account, status);
        }

        public async Task<Role?> GetRoleByIdAsync(int roleId)
        {
            return await _accountRepo.GetRoleByIdAsync(roleId);
        }

        public Task SavePasswordResetTokenAsync(int accountId, string token, DateTime expiration)
        {
            return _accountRepo.SavePasswordResetTokenAsync(accountId, token, expiration);
        }

        public Task<bool> VerifyPasswordResetTokenAsync(int accountId, string token)
        {
            return _accountRepo.VerifyPasswordResetTokenAsync(accountId, token);
        }

        public Task InvalidatePasswordResetTokenAsync(int accountId, string token)
        {
            return _accountRepo.InvalidatePasswordResetTokenAsync(accountId, token);
        }

        public async Task<int> GetParentCountAsync()
        {
            return await _accountRepo.GetParentCountAsync();
        }

        public async Task<int> GetNurseCountAsync()
        {
            return await _accountRepo.GetNurseCountAsync();
        }

        public async Task<List<Account>> GetActiveNursesAsync()
        {
            return await _accountRepo.GetActiveNursesAsync();
        }

        // =================== OTP Cache ===================

        public async Task<bool> SaveOtpAsync(string email, string otp, DateTime expiration)
        {
            var otpInfo = new OtpInfo { Code = otp, Expiration = expiration };
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = expiration
            };
            var json = JsonSerializer.Serialize(otpInfo);
            try
            {
                await _cache.SetStringAsync(GetOtpCacheKey(email), json, options);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> VerifyOtpAsync(string email, string otp)
        {
            var json = await _cache.GetStringAsync(GetOtpCacheKey(email));
            if (string.IsNullOrEmpty(json)) return false;

            var otpInfo = JsonSerializer.Deserialize<OtpInfo>(json);
            return otpInfo != null && otpInfo.Code == otp;
        }

        public async Task InvalidateOtpAsync(string email)
        {
            await _cache.RemoveAsync(GetOtpCacheKey(email));
        }

        public async Task<OtpInfo?> GetCurrentOtpAsync(string email)
        {
            var json = await _cache.GetStringAsync(GetOtpCacheKey(email));
            if (string.IsNullOrEmpty(json)) return null;

            return JsonSerializer.Deserialize<OtpInfo>(json);
        }
    }

}
