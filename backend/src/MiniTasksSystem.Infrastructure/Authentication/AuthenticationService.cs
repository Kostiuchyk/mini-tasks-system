using MiniTasksSystem.Application.Authentication;
using MiniTasksSystem.Application.Common;
using MiniTasksSystem.Application.Users;
using MiniTasksSystem.Domain.Users;

namespace MiniTasksSystem.Infrastructure.Authentication;

internal sealed class AuthenticationService(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    JwtProvider jwtProvider,
    BcryptPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork) : IAuthenticationService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
    private readonly JwtProvider _jwtProvider = jwtProvider;
    private readonly BcryptPasswordHasher _passwordHasher = passwordHasher;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<AuthenticationResult> Login(string email, string password)
    {
        var user = await _userRepository.GetByEmail(email)
            ?? throw new UnauthorizedAccessException("Invalid email or password.");

        if (!_passwordHasher.Verify(password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        return await GenerateAuthenticationResult(user);
    }

    public async Task<AuthenticationResult> Register(string fullName, string email, string password)
    {
        var existing = await _userRepository.GetByEmail(email);
        if (existing is not null)
        {
            throw new InvalidOperationException("A user with this email already exists.");
        }

        User user = new()
        {
            FullName = fullName,
            Email = email,
            PasswordHash = _passwordHasher.Hash(password),
            Role = UserRole.Member,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            await _userRepository.Create(user);
            var result = await GenerateAuthenticationResult(user);
            await _unitOfWork.CommitTransactionAsync();
            return result;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<AuthenticationResult> Refresh(string refreshToken)
    {
        var storedToken = await _refreshTokenRepository.GetByToken(refreshToken)
            ?? throw new UnauthorizedAccessException("Invalid refresh token.");

        if (storedToken.ExpiresAt < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Refresh token expired or revoked.");
        }

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            await _refreshTokenRepository.Revoke(refreshToken);

            var user = await _userRepository.GetById(storedToken.UserId)
                ?? throw new UnauthorizedAccessException("User not found.");

            var result = await GenerateAuthenticationResult(user);
            await _unitOfWork.CommitTransactionAsync();
            return result;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task Logout(string refreshToken)
    {
        await _refreshTokenRepository.Revoke(refreshToken);
    }

    private async Task<AuthenticationResult> GenerateAuthenticationResult(User user)
    {
        var accessToken = _jwtProvider.GenerateAccessToken(user);
        var refreshTokenValue = _jwtProvider.GenerateRefreshToken();

        RefreshToken refreshToken = new()
        {
            UserId = user.Id,
            Token = refreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        await _refreshTokenRepository.Create(refreshToken);

        UserDto userDto = new(user.Id, user.FullName, user.Email, user.Role.ToString());
        return new AuthenticationResult(accessToken, refreshTokenValue, userDto);
    }
}
