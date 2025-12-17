using AutoMapper;
using CMS.Application.Auth.DTOs.Requests;
using CMS.Application.Auth.DTOs.Responses;
using CMS.Application.Auth.Interfaces;
using CMS.Domain.Auth.Entities;
using CMS.Domain.Auth.Enums;
using CMS.Domain.Shared.Exceptions;
using Microsoft.Extensions.Logging;

namespace CMS.Application.Auth.Services
{
    public class AuthService : IAuthService
    {
        private readonly IEmailService _emailService;
        private readonly IVerificationCodeRepository _verificationCodeRepository;
        private readonly IInvitationRepository _invitationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            IJwtService jwtService,
            IMapper mapper,
            ILogger<AuthService> logger,
            IEmailService emailService,
            IVerificationCodeRepository verificationCodeRepository,
            IInvitationRepository invitationRepository)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _mapper = mapper;
            _logger = logger;
            _emailService = emailService;
            _verificationCodeRepository = verificationCodeRepository;
            _invitationRepository = invitationRepository;
        }

        public async Task<SignUpResponse> SignUpAsync(SignUpRequest request)
        {
            // If a user exists, handle active vs inactive cases
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            User createdUser;
            if (existingUser != null)
            {
                if (existingUser.IsActive)
                {
                    throw new ValidationException("User with this email already exists.");
                }

                // Update existing inactive user with newest details and reset password
                existingUser.Name = request.Name;
                existingUser.PhoneNumber = request.PhoneNumber;
                existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                existingUser.Role = RoleType.User;
                existingUser.IsActive = false;

                createdUser = await _userRepository.UpdateAsync(existingUser);
            }
            else
            {
                // Map request to user entity
                var user = _mapper.Map<User>(request);

                // Hash password
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                // Ensure role is User (default from mapper, but explicit for safety)
                user.Role = RoleType.User;

                // Create user as inactive until OTP verification
                user.IsActive = false;
                createdUser = await _userRepository.CreateAsync(user);
            }

            // Generate OTP and send via email
            if (_verificationCodeRepository != null && _emailService != null)
            {
                var code = GenerateNumericCode(6);
                var verification = new Domain.Auth.Entities.VerificationCode
                {
                    UserID = createdUser.UserID,
                    Code = code,
                    Purpose = "Signup",
                    ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                    CreatedAt = DateTime.UtcNow,
                    IsUsed = false
                };
                await _verificationCodeRepository.CreateAsync(verification);
                await _emailService.SendEmailAsync(createdUser.Email, "Your signup OTP", $"Your verification code is: {code}", null);
            }

            _logger.LogInformation("New user signed up (pending verification): {Email}, Role: {Role}", createdUser.Email, createdUser.Role);

            return new SignUpResponse
            {
                Message = "User registered. Check your email for the verification code.",
                User = _mapper.Map<UserResponse>(createdUser)
            };
        }

        public async Task SendSignUpOtpAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) throw new Domain.Shared.Exceptions.NotFoundException("User not found.");
            var code = GenerateNumericCode(6);
            var verification = new Domain.Auth.Entities.VerificationCode
            {
                UserID = user.UserID,
                Code = code,
                Purpose = "Signup",
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                CreatedAt = DateTime.UtcNow,
                IsUsed = false
            };
            await _verificationCodeRepository.CreateAsync(verification);
            await _emailService.SendEmailAsync(email, "Your signup OTP", $"Your verification code is: {code}", null);
        }

        public async Task VerifySignUpOtpAsync(string email, string code)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) throw new Domain.Shared.Exceptions.NotFoundException("User not found.");
            var v = await _verificationCodeRepository.GetLatestAsync(user.UserID, "Signup");
            if (v == null || v.IsUsed || v.ExpiresAt < DateTime.UtcNow || v.Code != code) throw new Domain.Shared.Exceptions.ValidationException("Invalid or expired code.");
            v.IsUsed = true;
            await _verificationCodeRepository.UpdateAsync(v);
            user.IsActive = true;
            await _userRepository.UpdateAsync(user);
        }

        public async Task SendForgotPasswordOtpAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) throw new Domain.Shared.Exceptions.NotFoundException("User not found.");
            var code = GenerateNumericCode(6);
            var verification = new Domain.Auth.Entities.VerificationCode
            {
                UserID = user.UserID,
                Code = code,
                Purpose = "ForgotPassword",
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                CreatedAt = DateTime.UtcNow,
                IsUsed = false
            };
            await _verificationCodeRepository.CreateAsync(verification);
            await _emailService.SendEmailAsync(email, "Password reset code", $"Your password reset code is: {code}", null);
        }

        public async Task ResetPasswordWithOtpAsync(string email, string code, string newPassword)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) throw new Domain.Shared.Exceptions.NotFoundException("User not found.");
            var v = await _verificationCodeRepository.GetLatestAsync(user.UserID, "ForgotPassword");
            if (v == null || v.IsUsed || v.ExpiresAt < DateTime.UtcNow || v.Code != code) throw new Domain.Shared.Exceptions.ValidationException("Invalid or expired code.");
            v.IsUsed = true;
            await _verificationCodeRepository.UpdateAsync(v);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.IsActive = true; // reactivate if needed
            await _userRepository.UpdateAsync(user);
        }

        public async Task ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new Domain.Shared.Exceptions.NotFoundException("User not found.");
            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash)) throw new Domain.Shared.Exceptions.ValidationException("Current password is incorrect.");
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            // Find user by email
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                throw new NotFoundException("Invalid email or password.");
            }

            // Check if user is active
            if (!user.IsActive)
            {
                throw new ForbiddenAccessException("Your account is inactive. Please contact administrator.");
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new ValidationException("Invalid email or password.");
            }

            // Generate tokens
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Save token to database
            await _jwtService.SaveTokenAsync(
                user.UserID,
                accessToken,
                refreshToken,
                60, // Access token expires in 60 minutes
                7   // Refresh token expires in 7 days
            );

            _logger.LogInformation("User logged in: {Email}, Role: {Role}", user.Email, user.Role);

            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = 3600, // 60 minutes in seconds
                User = _mapper.Map<UserResponse>(user)
            };
        }

        public async Task<InviteUserResponse> InviteUserAsync(InviteUserRequest request, Guid adminUserId)
        {
            // Verify admin user exists and is admin
            var adminUser = await _userRepository.GetByIdAsync(adminUserId);
            if (adminUser == null)
            {
                throw new NotFoundException("Admin user not found.");
            }

            if (adminUser.Role != RoleType.Admin)
            {
                throw new ForbiddenAccessException("Only administrators can invite users.");
            }

            // Validate role - admin can only invite Staff or Doctor
            if (request.Role != RoleType.Staff && request.Role != RoleType.Doctor)
            {
                throw new ValidationException("Admin can only invite Staff or Doctor roles.");
            }

            // Check if user already exists; if inactive, resend invitation instead of failing
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                if (existingUser.IsActive)
                {
                    throw new ValidationException("User with this email already exists.");
                }

                // If there is an existing pending invitation that hasn't expired, return that information
                if (_invitationRepository != null)
                {
                    var latest = await _invitationRepository.GetLatestByEmailAsync(existingUser.Email);
                    if (latest != null && !latest.IsAccepted && latest.ExpiresAt > DateTime.UtcNow)
                    {
                        return new InviteUserResponse
                        {
                            Message = $"Invitation already sent to {existingUser.Email}. Expires at {latest.ExpiresAt:O}",
                            User = _mapper.Map<UserResponse>(existingUser),
                            InvitationLink = $"http://localhost:4200/auth/accept-invite?token={latest.Token}"
                        };
                    }
                }

                // Otherwise create and send a new invitation
                if (_invitationRepository != null && _emailService != null)
                {
                    var token = Guid.NewGuid().ToString();
                    var invitation = new Domain.Auth.Entities.Invitation
                    {
                        UserID = existingUser.UserID,
                        Email = existingUser.Email,
                        Role = request.Role.ToString(),
                        Token = token,
                        ExpiresAt = DateTime.UtcNow.AddDays(7),
                        CreatedAt = DateTime.UtcNow,
                        InvitedBy = adminUserId,
                        IsAccepted = false
                    };
                    await _invitationRepository.CreateAsync(invitation);
                    var inviteLink = $"http://localhost:4200/auth/accept-invite?token={token}";
                    await _emailService.SendEmailAsync(existingUser.Email, "You're invited to CMS", $"You were invited as {request.Role}. Accept: {inviteLink}", $"<p>You were invited as {request.Role}.</p><p><a href=\"{inviteLink}\">Accept invitation</a></p>");

                    _logger.LogInformation("Re-sent invitation to existing inactive user {Email} by admin {AdminEmail}", existingUser.Email, adminUser.Email);

                    return new InviteUserResponse
                    {
                        Message = $"Invitation re-sent to {existingUser.Email}.",
                        User = _mapper.Map<UserResponse>(existingUser),
                        InvitationLink = inviteLink
                    };
                }

                // If no invitation/email service available, still return conflict
                throw new ValidationException("User with this email already exists.");
            }

            // Map request to user entity
            var user = _mapper.Map<User>(request);
            user.Role = request.Role;

            // Generate a temporary password (user will need to set their own password)
            var tempPassword = GenerateTemporaryPassword();
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(tempPassword);

            // User is inactive until they set their password
            user.IsActive = false;

            // Create user as inactive until they accept invitation
            user.IsActive = false;
            var createdUser = await _userRepository.CreateAsync(user);

            // create invitation record and send email
            if (_invitationRepository != null && _emailService != null)
            {
                var token = Guid.NewGuid().ToString();
                var invitation = new Domain.Auth.Entities.Invitation
                {
                    UserID = createdUser.UserID,
                    Email = createdUser.Email,
                    Role = request.Role.ToString(),
                    Token = token,
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    CreatedAt = DateTime.UtcNow,
                    InvitedBy = adminUserId,
                    IsAccepted = false
                };
                await _invitationRepository.CreateAsync(invitation);
                var inviteLink = $"http://localhost:4200/auth/accept-invite?token={token}";
                await _emailService.SendEmailAsync(createdUser.Email, "You're invited to CMS", $"You were invited as {request.Role}. Accept: {inviteLink}", $"<p>You were invited as {request.Role}.</p><p><a href=\"{inviteLink}\">Accept invitation</a></p>");
            }

            _logger.LogInformation("User invited by admin {AdminEmail}: {Email}, Role: {Role}", 
                adminUser.Email, createdUser.Email, createdUser.Role);

            return new InviteUserResponse
            {
                Message = $"User invited successfully. An invitation email has been sent to {createdUser.Email}.",
                User = _mapper.Map<UserResponse>(createdUser),
                InvitationLink = $"http://localhost:4200/auth/accept-invite?token={("(token-generated-in-email)") }"
            };
        }

        public async Task<InviteUserResponse> ResendInvitationAsync(string email, RoleType? role, Guid adminUserId)
        {
            var adminUser = await _userRepository.GetByIdAsync(adminUserId);
            if (adminUser == null) throw new NotFoundException("Admin user not found.");
            if (adminUser.Role != RoleType.Admin) throw new ForbiddenAccessException("Only administrators can invite users.");

            var user = await _userRepository.GetByEmailAsync(email);
            if (user != null && user.IsActive)
            {
                throw new ValidationException("User with this email already exists.");
            }

            User targetUser;
            if (user == null)
            {
                targetUser = new User
                {
                    Email = email,
                    Name = email.Split('@')[0],
                    PhoneNumber = "0000000000",
                    Role = role ?? RoleType.Staff,
                    IsActive = false
                };
                targetUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(GenerateTemporaryPassword());
                targetUser = await _userRepository.CreateAsync(targetUser);
            }
            else
            {
                targetUser = user;
            }

            if (_invitationRepository != null && _emailService != null)
            {
                var token = Guid.NewGuid().ToString();
                var invitation = new Domain.Auth.Entities.Invitation
                {
                    UserID = targetUser.UserID,
                    Email = targetUser.Email,
                    Role = (role ?? targetUser.Role).ToString(),
                    Token = token,
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    CreatedAt = DateTime.UtcNow,
                    InvitedBy = adminUserId,
                    IsAccepted = false
                };
                await _invitationRepository.CreateAsync(invitation);
                var inviteLink = $"http://localhost:4200/auth/accept-invite?token={token}";
                await _emailService.SendEmailAsync(targetUser.Email, "You're invited to CMS", $"You were invited as {invitation.Role}. Accept: {inviteLink}", $"<p>You were invited as {invitation.Role}.</p><p><a href=\"{inviteLink}\">Accept invitation</a></p>");

                return new InviteUserResponse
                {
                    Message = $"Invitation re-sent to {targetUser.Email}.",
                    User = _mapper.Map<UserResponse>(targetUser),
                    InvitationLink = inviteLink
                };
            }

            throw new ValidationException("Unable to resend invitation at this time.");
        }

        public async Task AcceptInvitationAsync(string token, string newPassword)
        {
            if (_invitationRepository == null) throw new System.InvalidOperationException("Invitation repository not available");
            var invitation = await _invitationRepository.GetByTokenAsync(token);
            if (invitation == null) throw new NotFoundException("Invalid invitation token.");
            if (invitation.IsAccepted) throw new ValidationException("Invitation already accepted.");
            if (invitation.ExpiresAt < DateTime.UtcNow) throw new ValidationException("Invitation expired.");
            if (!invitation.UserID.HasValue) throw new ValidationException("Invitation missing user reference.");

            var user = await _userRepository.GetByIdAsync(invitation.UserID.Value);
            if (user == null) throw new NotFoundException("User not found for invitation.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.IsActive = true;
            await _userRepository.UpdateAsync(user);

            invitation.IsAccepted = true;
            await _invitationRepository.UpdateAsync(invitation);
        }

        private string GenerateTemporaryPassword()
        {
            // Generate a secure random password
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 16)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private string GenerateNumericCode(int digits)
        {
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            var seed = BitConverter.ToInt32(bytes, 0);
            var random = new Random(seed);
            var min = (int)Math.Pow(10, digits - 1);
            var max = (int)Math.Pow(10, digits) - 1;
            return random.Next(min, max + 1).ToString();
        }
    }
}

