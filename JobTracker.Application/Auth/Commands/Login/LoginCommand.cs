using MediatR;
using JobTracker.Application.Auth.Commands.Register;

namespace JobTracker.Application.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<AuthResult>;
