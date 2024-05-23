using System.Security.Claims;
using DAL.Helpers;

namespace BLL.Sevices;

public class ClaimsService
{
    public int GetId(IEnumerable<Claim> claims)
    {
        var id = claims.FirstOrDefault(x => x.Type == "id")?.Value;
        if (id == null) throw new ObjectNotFoundException("Unauthorized access.");
        return int.Parse(id);
    }
}