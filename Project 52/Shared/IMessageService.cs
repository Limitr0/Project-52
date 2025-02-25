namespace BusinessLogic.Interfaces;
using Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IMessageService
{
    void AddMessage(Message message);
    Task<List<Message>> GetMessagesAsync();
}
