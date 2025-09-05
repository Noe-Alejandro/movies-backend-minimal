using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Movies.Application.Abstractions.Repository;
using System.Security.Claims;

namespace Movies.WebApi.Security.Requirements;

public sealed class AllowedDomainHandler(
    IAuthRepository repo,
    IMemoryCache cache,
    ILogger<AllowedDomainHandler> log)
    : AuthorizationHandler<AllowedDomainRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AllowedDomainRequirement requirement)
    {
        var email = context.User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrWhiteSpace(email))
            return;

        var domain = email.Split('@').LastOrDefault();
        if (string.IsNullOrWhiteSpace(domain))
            return;

        // cache key para no ir a DB siempre
        var cacheKey = $"alloweddomain:{domain}";
        var allowed = await cache.GetOrCreateAsync(cacheKey, async e =>
        {
            e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return await repo.IsDomainAllowedAsync(domain, CancellationToken.None);
        });

        if (allowed)
            context.Succeed(requirement);
        else
            log.LogWarning("Email domain {Domain} not allowed", domain);
    }
}