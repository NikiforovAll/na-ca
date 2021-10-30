﻿// Copyright (c) Oleksii Nikiforov, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace Nikiforoval.CA.Template.Domain.ProjectAggregate.Specifications;

using Ardalis.Specification;
using Nikiforoval.CA.Template.Domain.ProjectAggregate;

public class IncompleteItemsSpec : Specification<ToDoItem>
{
    public IncompleteItemsSpec() => this.Query.Where(item => !item.IsDone);
}
