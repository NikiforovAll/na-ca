﻿// Copyright (c) Oleksii Nikiforov, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace Nikiforoval.CA.Template.Domain.ProjectAggregate.Events;
using Nikiforoval.CA.Template.Domain.ProjectAggregate;
using Nikiforoval.CA.Template.Domain.SharedKernel;

public class ToDoItemCompletedEvent : DomainEvent
{
    public ToDoItem CompletedItem { get; set; }

    public ToDoItemCompletedEvent(ToDoItem completedItem) => this.CompletedItem = completedItem;
}