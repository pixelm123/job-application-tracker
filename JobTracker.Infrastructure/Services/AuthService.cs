using JobTracker.Application.Auth.Commands.Register;
using JobTracker.Application.Common.Interfaces;
using JobTracker.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace JobTracker.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;

    public AuthService(UserManager<ApplicationUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<AuthResult> RegisterAsync(
        string firstName, string lastName, string email, string password,
        CancellationToken cancellationToken = default)
    {
        var existing = await _userManager.FindByEmailAsync(email);
        if (existing is not null)
            throw new InvalidOperationException("An account with this email already exists.");

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException(errors);
        }

        var token = _tokenService.GenerateToken(user.Id, user.Email!, user.FirstName, user.LastName);
        return new AuthResult(token, user.Email!, user.FirstName, user.LastName);
    }

    public async Task<AuthResult?> LoginAsync(string email, string password,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null) return null;

        var valid = await _userManager.CheckPasswordAsync(user, password);
        if (!valid) return null;

        var token = _tokenService.GenerateToken(user.Id, user.Email!, user.FirstName, user.LastName);
        return new AuthResult(token, user.Email!, user.FirstName, user.LastName);
    }
}
