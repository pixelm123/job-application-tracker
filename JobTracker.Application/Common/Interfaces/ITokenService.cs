namespace JobTracker.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateToken(string userId, string email, string firstName, string lastName);
}
