using Azure.Core;
using BOs.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Services;
using System.Security.Claims;
using System;
using SWP391_BE.DTO;
using Services.Token;
using Services.Email;

namespace SWP391_BE.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IPasswordHasher<Account> _passwordHasher;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountController> _logger;
        private readonly TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");


        public AccountController(
            IAccountService accountService,
            ITokenService tokenService,
            IEmailService emailService,
            IPasswordHasher<Account> passwordHasher,
            ILogger<AccountController> logger,
            IConfiguration configuration)

        {
            _accountService = accountService;
            _tokenService = tokenService;
            _emailService = emailService;
            _passwordHasher = passwordHasher;
            _logger = logger;
            _configuration = configuration;

        }

        #region Registration Endpoints

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AccountRequestDTO accountRequest)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return BadRequest(new { errors });
            }

            var account = new Account
            {
                Email = accountRequest.Email,
                Password = accountRequest.Password,
                Fullname = accountRequest.Fullname,
                Address = accountRequest.Address,
                PhoneNumber = accountRequest.PhoneNumber,
                DateOfBirth = (DateTime)accountRequest.DateOfBirth,
                RoleID = 3,
                Status = "Active",
                CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone),
                UpdateAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone)
            };

            bool result = await _accountService.SignUpAsync(account);
            if (!result)
                return Conflict("Username or Email already exists.");

            return Ok(new
            {
                message = "Account successfully registered.",
                account = new
                {
                    account.AccountID,
                    account.Email,
                    account.Fullname
                }
            });
        }



        [HttpPost("otp/register")]
        public async Task<IActionResult> OtpRegister([FromBody] OtpRegistrationRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return BadRequest(new { errors });
            }

            if (request.Password != request.ConfirmPassword)
                return BadRequest(new { error = "Password and Confirm Password do not match." });

            var account = new Account
            {
                Email = request.Email,
                Password = request.Password,
                Fullname = request.Fullname,
                Address = request.Address,
                PhoneNumber = request.PhoneNumber,
                DateOfBirth = (DateTime)request.DateOfBirth,
                RoleID = 3,
                Status = "Pending",
                CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone),
                UpdateAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone)
            };

            bool result = await _accountService.SignUpAsync(account);
            if (!result)
                return Conflict("Username or Email already exists.");

            var otpCode = new Random().Next(100000, 999999).ToString();
            var expiration = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone).AddMinutes(5);

            bool otpSaved = await _accountService.SaveOtpAsync(request.Email, otpCode, expiration);
            if (!otpSaved)
                return StatusCode(500, "Failed to generate OTP.");

            try
            {
                await _emailService.SendOtpEmailAsync(request.Email, otpCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send OTP email to {Email}", request.Email);
                return StatusCode(500, "Failed to send OTP email.");
            }

            return Ok(new
            {
                message = "Registration successful. An OTP has been sent to your email. Please verify to activate your account."
            });
        }



        [HttpPost("otp/verify")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return BadRequest(new { errors });
            }

            var account = await _accountService.GetAccountByEmailAsync(request.Email);
            if (account == null)
                return NotFound("Account not found.");
            if (account.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Account is already active.");

            bool isValid = await _accountService.VerifyOtpAsync(request.Email, request.OtpCode);
            if (!isValid)
                return BadRequest("Invalid or expired OTP.");
            account.Status = "Active";
            bool updateResult = await _accountService.UpdateAccountAsync(account);
            if (!updateResult)
            {
                _logger.LogInformation("No changes detected during OTP verification; treating as success.");
            }
            await _accountService.InvalidateOtpAsync(request.Email);

            return Ok(new { message = "Account verified successfully." });
        }

        [HttpPost("otp/resend")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return BadRequest(new { errors });
            }

            var account = await _accountService.GetAccountByEmailAsync(request.Email);
            if (account == null)
                return NotFound("Account not found.");

            if (!account.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                return Ok(new { message = "Account is already verified." });

            var currentOtp = await _accountService.GetCurrentOtpAsync(request.Email);
            if (currentOtp != null && currentOtp.Expiration > TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone))
            {
                return Ok(new { message = "Your OTP is still active. Please use the existing OTP." });
            }

            var otpCode = new Random().Next(100000, 999999).ToString();
            var expiration = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone).AddMinutes(5);

            bool otpSaved = await _accountService.SaveOtpAsync(request.Email, otpCode, expiration);
            if (!otpSaved)
                return StatusCode(500, "Failed to generate OTP.");

            try
            {
                await _emailService.SendOtpEmailAsync(request.Email, otpCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to resend OTP email to {Email}", request.Email);
                return StatusCode(500, "Failed to resend OTP email.");
            }

            return Ok(new { message = "A new OTP has been sent to your email." });
        }

        #endregion

        #region Login & External Authentication

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequest)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return BadRequest(new { errors });
            }

            var account = await _accountService.Login(loginRequest.Email, loginRequest.Password);
            if (account == null)
                return Unauthorized("Invalid login information or account is inactive.");
            if (!account.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new
                {
                    message = "Tài khoản của bạn đã bị khóa, nếu bạn nghĩ đây là một sự hiểu lầm, hãy gửi yêu cầu mở khóa cho chúng tôi.",
                    url = "mailto:admin@example.com?subject=Yêu cầu hỗ trợ mở tài khoản&body=Chào bạn,%0ATôi cần hỗ trợ về vấn đề mở tài khoản với lí do:"
                });
            if (account.RoleID == 0)
                return Unauthorized("Account is not permitted to login due to invalid role.");
            var token = _tokenService.GenerateToken(account);
            return Ok(new
            {
                message = "Login successful.",
                token,
                account = new
                {
                    account.AccountID,
                    account.Email,
                    account.Fullname,
                    RoleName = account.Role?.RoleName
                }
            });
        }

        /*[HttpGet("google-login")]
        public IActionResult GoogleLogin(string returnUrl = "/")
        {
            var state = Guid.NewGuid().ToString(); // Generate unique state
            HttpContext.Session.SetString("OAuthState", state);

            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse", new { returnUrl }),
                Items = { { "LoginProvider", "Google" }, { "state", state } }
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }*/

        /*[HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse(string returnUrl = "/")
        {
            var savedState = HttpContext.Session.GetString("OAuthState");
            var returnedState = Request.Query["state"];

            if (savedState != returnedState)
            {
                _logger.LogError("OAuth state mismatch!");
                return BadRequest("Invalid OAuth state.");
            }

            var result = await HttpContext.AuthenticateAsync("ExternalCookie");
            if (!result.Succeeded)
            {
                _logger.LogWarning("External authentication failed.");
                return BadRequest("External authentication error.");
            }

            var externalUser = result.Principal;
            var email = externalUser.FindFirst(ClaimTypes.Email)?.Value;
            var name = externalUser.FindFirst(ClaimTypes.Name)?.Value;
            var googleId = externalUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(googleId))
                return BadRequest("Necessary claims not received from Google.");

            var account = await _accountService.Login(email, string.Empty);
            if (account != null)
            {
                if (string.IsNullOrEmpty(account.ExternalProvider))
                {
                    account.ExternalProvider = "Google";
                    account.ExternalProviderKey = googleId;
                    await _accountService.UpdateAccountAsync(account);
                }
            }
            else
            {
                var newAccount = new Account
                {
                    Username = email,
                    Email = email,
                    Fullname = name,
                    Password = string.Empty,
                    RoleID = 1,
                    Status = "Active",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    ExternalProvider = "Google",
                    ExternalProviderKey = googleId
                };
                bool created = await _accountService.SignUpAsync(newAccount);
                if (!created)
                    return Conflict("Unable to create account.");

                account = newAccount;
            }

            var token = _tokenService.GenerateToken(account);
            await HttpContext.SignOutAsync("ExternalCookie");
            return Ok(new
            {
                message = "Google login successful.",
                token,
                account = new
                {
                    account.AccountID,
                    account.Username,
                    account.Email,
                    account.Fullname,
                    RoleName = account.Role?.RoleName
                }
            });
        }*/

        #endregion

        #region Account Management

        [HttpGet("info")]
        [Authorize(Roles = "Admin,Nurse,Parent")]
        public async Task<IActionResult> GetAccountInformation()
        {
            try
            {
                var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int accountId))
                    return Unauthorized("Invalid or missing token.");
                var account = await _accountService.GetAccountByIdAsync(accountId);
                if (account == null)
                    return NotFound("Account not found.");
                if (!account.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                    return Unauthorized("Account is not active.");

                var accountInfo = new
                {
                    account.AccountID,
                    account.Email,
                    account.Fullname,
                    account.Address,
                    account.PhoneNumber,
                    // Trả về image dạng base64 string (giống Blog)
                    Image = account.Image != null ? Convert.ToBase64String(account.Image) : null,
                    Students = account.Students?.Select(ap => new
                    {
                        ap.StudentId,
                        ap.Fullname,
                        ap.ClassId,
                        ap.StudentCode,
                        ap.Gender,
                        ap.DateOfBirth,
                        ClassName = ap.Class?.ClassName
                    }).ToList()
                };
                return Ok(accountInfo);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPatch("update")]
        [Authorize(Roles = "Admin,Nurse,Parent")]
        public async Task<IActionResult> UpdateAccount([FromForm] PartialAccountUpdateRequest updateRequest)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return BadRequest(new { errors });
            }

            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int accountId))
                return Unauthorized("Invalid or missing token.");
            var account = await _accountService.GetAccountByIdAsync(accountId);
            if (account == null)
                return NotFound("Account not found.");

            if (!string.IsNullOrEmpty(updateRequest.Email))
            {
                var existingByEmail = await _accountService.GetAccountByEmailAsync(updateRequest.Email);
                if (existingByEmail != null && existingByEmail.AccountID != accountId)
                    return Conflict("Email already exists.");
                account.Email = updateRequest.Email;
            }
            if (!string.IsNullOrEmpty(updateRequest.Fullname))
                account.Fullname = updateRequest.Fullname;
            if (updateRequest.Address != null)
                account.Address = updateRequest.Address;
            if (!string.IsNullOrEmpty(updateRequest.PhoneNumber))
                account.PhoneNumber = updateRequest.PhoneNumber;

            // Xử lý ảnh đại diện nếu có upload
            if (updateRequest.Image != null && updateRequest.Image.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await updateRequest.Image.CopyToAsync(ms);
                    account.Image = ms.ToArray();
                }
            }

            // Update the UpdatedAt timestamp
            account.UpdateAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

            bool updateResult = await _accountService.UpdateAccountAsync(account);
            if (!updateResult)
            {
                _logger.LogError("Failed to update account for AccountID: {AccountId}", accountId);
                return StatusCode(500, "A problem occurred while processing your request.");
            }

            // Trả về thông tin account mới nhất, bao gồm ảnh base64
            return Ok(new
            {
                message = "Account updated successfully.",
                account = new
                {
                    account.AccountID,
                    account.Email,
                    account.Fullname,
                    account.Address,
                    account.PhoneNumber,
                    Image = account.Image != null ? Convert.ToBase64String(account.Image) : null
                }
            });
        }

        [HttpPatch("change-password")]
        [Authorize(Roles = "Admin,Nurse,Parent")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDTO changePasswordRequest)
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int accountId))
                return Unauthorized("Invalid or missing token.");
            var account = await _accountService.GetAccountByIdAsync(accountId);
            if (account == null)
                return NotFound("Account not found.");
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return BadRequest(new { errors });
            }
            bool isCurrentPasswordValid = BCrypt.Net.BCrypt.Verify(changePasswordRequest.CurrentPassword, account.Password);
            if (!isCurrentPasswordValid)
                return BadRequest("Current password is incorrect.");
            if (BCrypt.Net.BCrypt.Verify(changePasswordRequest.NewPassword, account.Password))
                return BadRequest("New password cannot be the same as the current password.");

            // Set the new password (will be hashed in DAO)
            account.Password = changePasswordRequest.NewPassword;
            account.UpdateAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
            // Ensure Status remains unchanged
            // account.Status is already loaded from database, so no need to change it

            bool updateResult = await _accountService.UpdateAccountAsync(account);
            if (!updateResult)
            {
                _logger.LogError("Failed to change password for AccountID: {AccountId}", accountId);
                return StatusCode(500, "A problem occurred while processing your request.");
            }
            return Ok(new { message = "Password successfully changed." });
        }

        #endregion

        #region Password Management

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var account = await _accountService.GetAccountByEmailAsync(request.Email);
            if (account == null)
            {
                return Ok(new { message = "If an account with that email exists, you will receive a password reset email." });
            }

            var token = Guid.NewGuid().ToString();
            var expiration = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone).AddHours(1);
            await _accountService.SavePasswordResetTokenAsync(account.AccountID, token, expiration);

            await _emailService.SendPasswordResetEmailAsync(request.Email, token);

            return Ok(new { message = "If an account with that email exists, you will receive a password reset email." });
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var account = await _accountService.GetAccountByEmailAsync(request.Email);
            if (account == null)
                return BadRequest("Invalid request.");
            var tokenValid = await _accountService.VerifyPasswordResetTokenAsync(account.AccountID, request.Token);
            if (!tokenValid)
                return BadRequest("Invalid or expired token.");

            // Set the new password (will be hashed in DAO)
            account.Password = request.NewPassword;
            account.UpdateAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

            await _accountService.UpdateAccountAsync(account);
            await _accountService.InvalidatePasswordResetTokenAsync(account.AccountID, request.Token);
            return Ok(new { message = "Password reset successfully." });
        }

        #endregion

        #region Admin Endpoints

        [Authorize(Roles = "Admin")]
        [HttpPatch("admin/update-status/{id}")]
        public async Task<IActionResult> UpdateAccountStatus(int id, [FromBody] StatusUpdateRequestDTO request)
        {
            if (id <= 0)
                return BadRequest("Invalid account ID.");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return BadRequest(new { errors });
            }

            var account = await _accountService.GetAccountByIdAsync(id);
            if (account == null)
                return NotFound("Account not found.");

            var allowedStatuses = new[] { "Active", "InActive" };
            if (!allowedStatuses.Contains(request.Status, StringComparer.OrdinalIgnoreCase))
                return BadRequest("Invalid status. Only 'Active' and 'InActive' are allowed.");

            bool result = await _accountService.UpdateAccountStatusAsync(account, request.Status);
            if (!result)
                return StatusCode(500, "Failed to update account status.");

            await _emailService.SendStatusUserAsync(account.Email, request.Status);

            return Ok(new { message = "Account status updated successfully." });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/statistics/signup-count")]
        public async Task<IActionResult> GetSignUpCounts()
        {
            int userCount = await _accountService.GetParentCountAsync();
            int schoolOwnerCount = await _accountService.GetNurseCountAsync();
            return Ok(new { userCount, schoolOwnerCount });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/all")]
        public async Task<IActionResult> GetAllAccounts()
        {
            var accounts = await _accountService.GetAllAccountsAsync();
            return Ok(accounts);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("admin/{id}")]
        public async Task<IActionResult> GetAccountById(int id)
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            return account != null ? Ok(account) : NotFound("Account not found.");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("admin/create-nurse")]
        public async Task<IActionResult> CreateNurseAccount([FromBody] NurseAccountCreateDTO nurseRequest)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return BadRequest(new { errors });
            }

            // Check if email already exists
            var existingAccount = await _accountService.GetAccountByEmailAsync(nurseRequest.Email);
            if (existingAccount != null)
                return Conflict("Email already exists.");

            var nurseAccount = new Account
            {
                Email = nurseRequest.Email,
                Password = nurseRequest.Password,
                Fullname = nurseRequest.Fullname,
                Address = nurseRequest.Address,
                PhoneNumber = nurseRequest.PhoneNumber,
                DateOfBirth = nurseRequest.DateOfBirth,
                RoleID = 2, // Nurse role
                Status = "Active", // Admin-created accounts are immediately active
                CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone),
                UpdateAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone)
            };

            bool result = await _accountService.SignUpAsync(nurseAccount);
            if (!result)
                return StatusCode(500, "Failed to create nurse account.");

            return Ok(new
            {
                message = "Nurse account created successfully.",
                account = new
                {
                    nurseAccount.AccountID,
                    nurseAccount.Email,
                    nurseAccount.Fullname,
                    nurseAccount.Address,
                    nurseAccount.PhoneNumber,
                    nurseAccount.DateOfBirth,
                    Role = "Nurse",
                    nurseAccount.Status
                }
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("admin/delete/{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            bool result = await _accountService.DeleteAccountAsync(id);
            return NotFound("Account not found.");
        }

        #endregion

        #region Search Endpoint

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string name)
        {
            if (string.IsNullOrEmpty(name))
                return BadRequest("The 'name' query parameter is required.");
            var accounts = await _accountService.SearchAccountsByFullNameAsync(name);
            if (accounts == null || accounts.Count == 0)
                return NotFound("No accounts found matching the provided name.");
            return Ok(accounts);
        }

        #endregion
    }
}
