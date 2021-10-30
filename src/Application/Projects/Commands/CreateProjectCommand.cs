// Copyright (c) Oleksii Nikiforov, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace Nikiforoval.CA.Template.Application.Projects.Commands;

using AutoMapper;
using MediatR;
using Nikiforoval.CA.Template.Application.Interfaces;
using Nikiforoval.CA.Template.Application.Projects.Models;
using Nikiforoval.CA.Template.Domain.ProjectAggregate;
using Nikiforoval.CA.Template.Domain.ValueObjects;

public class CreateProjectCommand : IRequest<ProjectViewModel>
{
    public string Name { get; set; }

    public Colour Colour { get; set; }
}

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectViewModel>
{
    private readonly IApplicationDbContext context;
    private readonly IMapper mapper;

    public CreateProjectCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<ProjectViewModel> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var entity = MapFrom(request);
        this.context.Projects.Add(entity);
        await this.context.SaveChangesAsync(cancellationToken);
        return this.mapper.Map<ProjectViewModel>(entity);
    }

    public static Project MapFrom(CreateProjectCommand command) =>
        new(command.Name, command.Colour);
}
