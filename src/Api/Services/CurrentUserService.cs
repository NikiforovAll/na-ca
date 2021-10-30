// Copyright (c) Oleksii Nikiforov, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace Nikiforoval.CA.Template.Api.Services;

using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Nikiforoval.CA.Template.Application.SharedKernel.Interfaces;

internal class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor) =>
        this.httpContextAccessor = httpContextAccessor;

    public string? UserId => this.httpContextAccessor
        .HttpContext
        ?.User
        ?.FindFirstValue(ClaimTypes.NameIdentifier);
}
