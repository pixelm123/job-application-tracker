using JobTracker.Application.Auth.Commands.Register;
using JobTracker.Application.Common.Interfaces;
using MediatR;

namespace JobTracker.Application.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResult>
{
    private readonly IAuthService _authService;

    public LoginCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<AuthResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request.Email, request.Password, cancellationToken);

        if (result is null)
            throw new UnauthorizedAccessException("Invalid email or password.");

        return result;
    }
}
