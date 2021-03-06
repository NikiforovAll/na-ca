// Copyright (c) Oleksii Nikiforov, 2021. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace NikiforovAll.CA.Template.Worker;

using NikiforovAll.CA.Template.Application.SharedKernel.Interfaces;

public class WorkerUserService : ICurrentUserService
{
    public string? UserId => default;
}
