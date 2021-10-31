// Copyright (c) Oleksii Nikiforov, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace Nikiforoval.CA.Template.Application.Projects.Commands.UpdateProject;

using FluentValidation;
using Nikiforoval.CA.Template.Application.Projects.Models;
using Nikiforoval.CA.Template.Application.SharedKernel.Models;

internal class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
    public UpdateProjectCommandValidator()
    {
        this.RuleFor(x => x.Name).ValidProjectName();
    }
}
