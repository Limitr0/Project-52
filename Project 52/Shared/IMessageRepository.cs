namespace DataAccess.Interfaces;
using Shared.Models;
using System.Collections.Generic;

public interface IMessageRepository
{
    void Save(Message message);
    List<Message> GetMessages();
}
