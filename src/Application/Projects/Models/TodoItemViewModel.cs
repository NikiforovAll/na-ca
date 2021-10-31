// Copyright (c) Oleksii Nikiforov, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace Nikiforoval.CA.Template.Application.Projects.Models;

using Nikiforoval.CA.Template.Application.SharedKernel.Mappings;
using Nikiforoval.CA.Template.Domain.ProjectAggregate;

public class TodoItemViewModel : IMapFrom<ToDoItem>
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public bool IsDone { get; set; }
}
