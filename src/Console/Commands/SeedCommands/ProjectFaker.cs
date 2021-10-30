// Copyright (c) Oleksii Nikiforov, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace Nikiforoval.CA.Template.Console.Commands.SeedCommands;

using AutoBogus;
using Nikiforoval.CA.Template.Domain.ProjectAggregate;
using Nikiforoval.CA.Template.Domain.ValueObjects;

public class ProjectFaker : AutoFaker<Project>
{
    public ProjectFaker()
    {
        this.CustomInstantiator(faker => new Project(this.FakerHub.Company.Random.Word(), Colour.Purple));

        this.RuleFor(faker => faker.Colour, faker => Colour.From(Colour.Purple));

        this.Configure(builder => builder
            .WithSkip<Project>(p => p.LastModified)
            .WithSkip<Project>(p => p.LastModifiedBy));
    }
}
