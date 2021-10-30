﻿// Copyright (c) Oleksii Nikiforov, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace Nikiforoval.CA.Template.Domain.ProjectAggregate;
using Nikiforoval.CA.Template.Domain.ProjectAggregate.Events;
using Nikiforoval.CA.Template.Domain.SharedKernel;
using Nikiforoval.CA.Template.Domain.ValueObjects;

public class Project : AuditableEntity, IHasDomainEvent, IAggregateRoot
{
    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public Colour Colour { get; private set; }

    private readonly List<ToDoItem> items = new();

    public IEnumerable<ToDoItem> Items => this.items.AsReadOnly();

    public ProjectStatus Status => this.items.All(i => i.IsDone) ? ProjectStatus.Complete : ProjectStatus.InProgress;

    public List<DomainEvent> DomainEvents { get; private set; } = new();

    public Project(string name, Colour colour)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
        }
        this.Name = name;
        this.Colour = colour;
    }

    public void AddItem(ToDoItem newItem)
    {
        if (newItem is null)
        {
            throw new ArgumentNullException(nameof(newItem));
        }

        this.items.Add(newItem);

        var newItemAddedEvent = new NewItemAddedEvent(this, newItem);
        this.DomainEvents.Add(newItemAddedEvent);
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            throw new ArgumentException($"'{nameof(newName)}' cannot be null or whitespace.", nameof(newName));
        }

        this.Name = newName;
    }
}
