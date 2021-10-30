// Copyright (c) Oleksii Nikiforov, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace Nikiforoval.CA.Template.Application.IntegrationTests.Projects;

using AutoFixture.Xunit2;
using Nikiforoval.CA.Template.Application.Projects.Commands;
using Nikiforoval.CA.Template.Application.SharedKernel.Exceptions;
using Nikiforoval.CA.Template.Domain.ProjectAggregate;

[Trait("Category", "Integration")]
public class CreateProjectCommandTests
{
    [Theory]
    [InlineAutoData("")]
    [InlineAutoData("a")]
    [InlineAutoData("aa")]
    public void NameIsTooShort_ExceptionThrown(string invalidName, CreateProjectCommand command)
    {
        command.Name = invalidName;
        FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }


    [Theory, AutoData]
    public async Task ValidCommand_ProjectCreated(CreateProjectCommand command)
    {
        var project = await SendAsync(command);

        var entity = await FindAsync<Project>(project.Id);

        entity.Should().NotBeNull();
        entity.Name.Should().Be(command.Name);
        entity.Status.Should().Be(ProjectStatus.Complete);
        entity.Items.Should().BeEmpty();
    }
}
