﻿// Copyright (c) Oleksii Nikiforov, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace Nikiforoval.CA.Template.Application.SharedKernel.Interfaces;

using System.Threading.Tasks;
using Nikiforoval.CA.Template.Domain.SharedKernel;

public interface IDomainEventService
{
    Task PublishAsync(DomainEvent domainEvent);
}