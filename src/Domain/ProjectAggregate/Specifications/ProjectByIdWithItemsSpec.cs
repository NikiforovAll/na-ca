// Copyright (c) Oleksii Nikiforov, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace Nikiforoval.CA.Template.Domain.ProjectAggregate.Specifications;

using Ardalis.Specification;

public class ProjectByIdWithItemsSpec : Specification<Project>, ISingleResultSpecification
{
    public ProjectByIdWithItemsSpec(Guid projectId) =>
        this.Query
            .Where(project => project.Id == projectId)
            .Include(project => project.Items);
}
