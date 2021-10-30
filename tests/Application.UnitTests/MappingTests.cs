// Copyright (c) Oleksii Nikiforov, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace Nikiforoval.CA.Template.Domain.UnitTests;

using System.Runtime.Serialization;
using AutoMapper;
using Nikiforoval.CA.Template.Application;
using Nikiforoval.CA.Template.Application.Projects.Models;
using Nikiforoval.CA.Template.Domain.ProjectAggregate;

public class MappingTests
{
    private readonly IConfigurationProvider configuration;
    private readonly IMapper mapper;

    public MappingTests()
    {
        this.configuration = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());

        this.mapper = this.configuration.CreateMapper();
    }

    [Fact]
    public void ShouldHaveValidConfiguration() =>
        this.configuration.AssertConfigurationIsValid();

    [Theory]
    [InlineData(typeof(Project), typeof(ProjectViewModel))]
    [InlineData(typeof(ToDoItem), typeof(TodoItemSummaryViewModel))]
    public void ShouldSupportMappingFromSourceToDestination(Type source, Type destination)
    {
        var instance = GetInstanceOf(source);

        this.mapper.Map(instance, source, destination);
    }

    private static object? GetInstanceOf(Type type)
    {
        if (type.GetConstructor(Type.EmptyTypes) != null)
        {
            return Activator.CreateInstance(type);
        }

        // Type without parameterless constructor
        return FormatterServices.GetUninitializedObject(type);
    }
}