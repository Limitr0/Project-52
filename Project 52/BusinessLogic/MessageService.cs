namespace BusinessLogic;
using Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;

    public MessageService(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public void AddMessage(Message message)
    {
        _messageRepository.Save(message);
    }

    public async Task<List<Message>> GetMessagesAsync()
    {
        return await Task.Run(() => _messageRepository.GetMessages());
    }
}
