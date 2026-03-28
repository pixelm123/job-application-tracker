using JobTracker.Application.Auth.Commands.Register;

namespace JobTracker.Application.Common.Interfaces;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(string firstName, string lastName, string email, string password, CancellationToken cancellationToken = default);
    Task<AuthResult?> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
}
