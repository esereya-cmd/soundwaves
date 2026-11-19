
using Microsoft.Extensions.Hosting;

namespace Soundwaves.Api.Services;

public class CurrentUserService : ICurrentUserService
{
    private const ulong DevelopmentUserId = 1001;

    private readonly IHostEnvironment _environment;

    public CurrentUserService(IHostEnvironment environment)
    {
        _environment = environment;
    }

    public ulong? GetCurrentUserId()
    {
        if (_environment.IsDevelopment())
        {
            return DevelopmentUserId;
        }

        return null;
    }
}
