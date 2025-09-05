using Microsoft.AspNetCore.Authorization;

namespace Movies.WebApi.Security.Requirements;

public sealed class AllowedDomainRequirement : IAuthorizationRequirement { }
