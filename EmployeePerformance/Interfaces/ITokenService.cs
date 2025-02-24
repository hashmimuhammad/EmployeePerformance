using EmployeePerformance.Models;

public interface ITokenService
{
    string GenerateJwtToken(ApplicationUser user, IList<string> roles); 
}
