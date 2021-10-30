// Copyright (c) Oleksii Nikiforov, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace Nikiforoval.CA.Template.Application.Projects.Models;

using Nikiforoval.CA.Template.Application.SharedKernel.Mappings;
using Nikiforoval.CA.Template.Domain.ProjectAggregate;

public class ProjectViewModel : IMapFrom<Project>
{
    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public string Colour { get; private set; }

    public IList<TodoItemSummaryViewModel> Items { get; private set; }

    public ProjectStatus Status { get; private set; }
}
