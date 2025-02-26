using Shared.Models;
using BusinessLogic.Services;
using DataAccess.Repositories;
using NUnit.Framework;
using Moq;

namespace DataAccess;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;

public class MessageRepository : IMessageRepository
{
    private List<Message> messages = new List<Message>();

    public void Save(Message message)
    {
        messages.Add(message);
    }

    public List<Message> GetMessages()
    {
        return messages.OrderByDescending(m => m.ReceivedTime).ToList();
    }
}
