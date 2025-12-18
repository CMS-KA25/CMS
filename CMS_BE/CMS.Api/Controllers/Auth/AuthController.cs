using CMS.Application.Auth.DTOs.Requests;
using CMS.Application.Auth.DTOs.Responses;
using CMS.Application.Auth.Interfaces;
using CMS.Application.Shared.DTOs;
using System.Linq;
using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CMS.Api.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        public class BulkInviteForm
        {
            public IFormFile File { get; set; }
            public CMS.Domain.Auth.Enums.RoleType Role { get; set; }
        }

        [HttpPost("resend-signup-otp")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<object>>> ResendSignUpOtp([FromBody] SendOtpRequest request)
        {
            try
            {
                await _authService.SendSignUpOtpAsync(request.Email);
                return Ok(ApiResponse<object>.SuccessResponse(null, "OTP sent"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending signup OTP for {Email}", request.Email);
                throw;
            }
        }

        [HttpPost("accept-invite")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<object>>> AcceptInvite([FromBody] CMS.Application.Auth.DTOs.Requests.AcceptInviteRequest request)
        {
            try
            {
                await _authService.AcceptInvitationAsync(request.Token, request.NewPassword);
                return Ok(ApiResponse<object>.SuccessResponse(null, "Invitation accepted. You can now login."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accepting invitation for token: {Token}", request.Token);
                throw;
            }
        }

        [HttpPost("bulk-invite")]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> BulkInvite([FromForm] BulkInviteForm form)
        {
            // Parse uploaded file (CSV or XLSX) to extract emails and invite each
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var adminUserId))
                {
                    return Unauthorized(ApiResponse<object>.ErrorResponse("Invalid user token"));
                }

                if (form?.File == null || form.File.Length == 0) return BadRequest(ApiResponse<object>.ErrorResponse("File is required"));

                var emails = new System.Collections.Generic.List<string>();
                var fileExt = System.IO.Path.GetExtension(form.File.FileName).ToLowerInvariant();
                using (var ms = new System.IO.MemoryStream())
                {
                    await form.File.CopyToAsync(ms);
                    ms.Position = 0;

                    if (fileExt == ".csv" || fileExt == ".txt")
                    {
                        using var sr = new System.IO.StreamReader(ms);
                        while (!sr.EndOfStream)
                        {
                            var line = (await sr.ReadLineAsync())?.Trim();
                            if (string.IsNullOrEmpty(line)) continue;
                            // split by commas or whitespace
                            foreach (var part in line.Split(new[] { ',', ';', '\t', ' ' }, System.StringSplitOptions.RemoveEmptyEntries))
                            {
                                if (System.Text.RegularExpressions.Regex.IsMatch(part, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) emails.Add(part);
                            }
                        }
                    }
                    else if (fileExt == ".xlsx" || fileExt == ".xls")
                    {
                        // Excel parsing requires ExcelDataReader (ExcelDataReader and ExcelDataReader.DataSet)
                        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                        try
                        {
                            using var reader = ExcelDataReader.ExcelReaderFactory.CreateReader(ms);
                            var result = reader.AsDataSet();
                            foreach (System.Data.DataTable table in result.Tables)
                            {
                                foreach (System.Data.DataRow row in table.Rows)
                                {
                                    foreach (var cell in row.ItemArray)
                                    {
                                        var s = cell?.ToString()?.Trim();
                                        if (!string.IsNullOrEmpty(s) && System.Text.RegularExpressions.Regex.IsMatch(s, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) emails.Add(s);
                                    }
                                }
                            }
                        }
                        catch (System.IO.FileNotFoundException) { throw; }
                        catch (System.Exception ex)
                        {
                            throw new System.InvalidOperationException("Excel parsing failed. Ensure ExcelDataReader packages are installed.", ex);
                        }
                    }
                    else
                    {
                        return BadRequest(ApiResponse<object>.ErrorResponse("Unsupported file type. Use .csv, .txt, .xlsx"));
                    }
                }

                emails = emails.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                var results = new System.Collections.Generic.List<string>();
                foreach (var email in emails)
                {
                    try
                    {
                        // Provide fallback name and phone so DTO validation passes for bulk imports
                        var fallbackName = email?.Split('@')[0] ?? "Invited";
                        var inviteReq = new CMS.Application.Auth.DTOs.Requests.InviteUserRequest
                        {
                            Email = email,
                            Name = string.IsNullOrWhiteSpace(fallbackName) ? "Invited" : fallbackName,
                            PhoneNumber = "0000000000",
                            Role = form.Role
                        };
                        var inviteResult = await _authService.InviteUserAsync(inviteReq, adminUserId);
                        if (inviteResult != null && !string.IsNullOrEmpty(inviteResult.Message) && inviteResult.Message.StartsWith("Invitation already sent"))
                        {
                            results.Add($"AlreadySent: {email} -> {inviteResult.Message}");
                        }
                        else if (inviteResult != null && !string.IsNullOrEmpty(inviteResult.Message) && inviteResult.Message.StartsWith("Invitation re-sent"))
                        {
                            results.Add($"ReSent: {email}");
                        }
                        else
                        {
                            results.Add($"Invited: {email}");
                        }
                    }
                    catch (Exception ex)
                    {
                        results.Add($"Failed: {email} -> {ex.Message}");
                    }
                }

                return Ok(ApiResponse<object>.SuccessResponse(results, $"Processed {emails.Count} emails"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during bulk invite");
                throw;
            }
        }

        [HttpGet("bulk-invite/template")]
        [Authorize(Roles = "Admin")]
        public ActionResult DownloadBulkInviteTemplate([FromQuery] string format = "csv")
        {
            try
            {
                // Currently support CSV template only
                if (!string.Equals(format, "csv", System.StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse("Only 'csv' format is supported for template."));
                }

                var csv = "email\r\n"; // header only; users should add one email per line
                var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
                return File(bytes, "text/csv", "bulk-invite-template.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating bulk invite template");
                throw;
            }
        }

        [HttpPost("verify-signup-otp")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<object>>> VerifySignUpOtp([FromBody] VerifyOtpRequest request)
        {
            try
            {
                await _authService.VerifySignUpOtpAsync(request.Email, request.Code);
                return Ok(ApiResponse<object>.SuccessResponse(null, "Account verified"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying signup OTP for {Email}", request.Email);
                throw;
            }
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<object>>> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                await _authService.SendForgotPasswordOtpAsync(request.Email);
                return Ok(ApiResponse<object>.SuccessResponse(null, "Password reset code sent"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending forgot password OTP for {Email}", request.Email);
                throw;
            }
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<object>>> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                await _authService.ResetPasswordWithOtpAsync(request.Email, request.Code, request.NewPassword);
                return Ok(ApiResponse<object>.SuccessResponse(null, "Password reset successful"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for {Email}", request.Email);
                throw;
            }
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                {
                    return Unauthorized(ApiResponse<object>.ErrorResponse("Invalid user token"));
                }

                await _authService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);
                return Ok(ApiResponse<object>.SuccessResponse(null, "Password changed"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user");
                throw;
            }
        }

        [HttpPost("signup")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<SignUpResponse>>> SignUp([FromBody] SignUpRequest request)
        {
            try
            {
                var result = await _authService.SignUpAsync(request);
                return Ok(ApiResponse<SignUpResponse>.SuccessResponse(result, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during signup for email: {Email}", request.Email);
                throw;
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var result = await _authService.LoginAsync(request);
                return Ok(ApiResponse<LoginResponse>.SuccessResponse(result, "Login successful"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
                throw;
            }
        }

        [HttpPost("invite")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<InviteUserResponse>>> InviteUser([FromBody] InviteUserRequest request)
        {
            try
            {
                // Get current user ID from claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var adminUserId))
                {
                    return Unauthorized(ApiResponse<InviteUserResponse>.ErrorResponse("Invalid user token"));
                }

                var result = await _authService.InviteUserAsync(request, adminUserId);
                return Ok(ApiResponse<InviteUserResponse>.SuccessResponse(result, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user invitation by admin");
                throw;
            }
        }

        [HttpPost("resend-invite")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<InviteUserResponse>>> ResendInvite([FromBody] ResendInviteRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var adminUserId))
                {
                    return Unauthorized(ApiResponse<InviteUserResponse>.ErrorResponse("Invalid user token"));
                }

                var result = await _authService.ResendInvitationAsync(request.Email, request.Role, adminUserId);
                return Ok(ApiResponse<InviteUserResponse>.SuccessResponse(result, result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending invitation for {Email}", request.Email);
                throw;
            }
        }
    }
}

