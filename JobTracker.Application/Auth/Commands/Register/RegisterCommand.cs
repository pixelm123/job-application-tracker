using MediatR;

namespace JobTracker.Application.Auth.Commands.Register;

public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password) : IRequest<AuthResult>;

public record AuthResult(string Token, string Email, string FirstName, string LastName);
