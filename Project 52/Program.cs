// Разделение на уровни:
// - Shared (общие интерфейсы и базовые классы)
// - DataAccess (доступ к данным)
// - BusinessLogic (бизнес-логика)
// - API (Web API для взаимодействия)
// - ConsoleApp (консольное приложение)
// - GUI (WinForms приложение)
// - UnitTests (юнит-тесты)

// В каждом файле теперь только один класс или интерфейс.

// Shared/Models/Call.cs
namespace Shared.Models;

public class Call
{
    public string PhoneNumber { get; set; }
    public DateTime CallTime { get; set; }
    public bool IsIncoming { get; set; }
}

// Shared/Models/Message.cs
namespace Shared.Models;

public class Message
{
    public string Sender { get; set; }
    public string Content { get; set; }
    public DateTime ReceivedTime { get; set; }
}

// API/Controllers/MessageController.cs
namespace API.Controllers;
using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Interfaces;
using Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/messages")]
public class MessageController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessageController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpGet]
    public async Task<IEnumerable<Message>> GetMessages()
    {
        return await _messageService.GetMessagesAsync();
    }

    [HttpPost]
    public IActionResult AddMessage([FromBody] Message message)
    {
        _messageService.AddMessage(message);
        return Ok();
    }
}

// BusinessLogic/Interfaces/IMessageService.cs
namespace BusinessLogic.Interfaces;
using Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IMessageService
{
    void AddMessage(Message message);
    Task<List<Message>> GetMessagesAsync();
}

// BusinessLogic/Services/MessageService.cs
namespace BusinessLogic.Services;
using BusinessLogic.Interfaces;
using Shared.Models;
using DataAccess.Interfaces;
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

// DataAccess/Interfaces/IMessageRepository.cs
namespace DataAccess.Interfaces;
using Shared.Models;
using System.Collections.Generic;

public interface IMessageRepository
{
    void Save(Message message);
    List<Message> GetMessages();
}

// DataAccess/Repositories/MessageRepository.cs
namespace DataAccess.Repositories;
using DataAccess.Interfaces;
using Shared.Models;
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

// GUI/MainForm.cs
namespace GUI;
using System;
using System.Windows.Forms;
using BusinessLogic.Services;
using DataAccess.Repositories;
using System.Threading.Tasks;

public class MainForm : Form
{
    private ListBox callListBox;
    private ListBox messageListBox;
    private Button refreshButton;
    private MessageService messageService;

    public MainForm()
    {
        Text = "SimCorp Mobile";
        Width = 500;
        Height = 400;

        callListBox = new ListBox { Dock = DockStyle.Top, Height = 150 };
        messageListBox = new ListBox { Dock = DockStyle.Bottom, Height = 150 };
        refreshButton = new Button { Text = "Refresh", Dock = DockStyle.Fill };
        refreshButton.Click += async (s, e) => await RefreshData();

        Controls.Add(callListBox);
        Controls.Add(refreshButton);
        Controls.Add(messageListBox);

        messageService = new MessageService(new MessageRepository());
    }

    private async Task RefreshData()
    {
        messageListBox.Items.Clear();

        var messages = await messageService.GetMessagesAsync();
        foreach (var message in messages)
        {
            messageListBox.Items.Add(message.ToString());
        }
    }
}

// UnitTests/MessageServiceTests.cs
namespace UnitTests;
using NUnit.Framework;
using Moq;
using BusinessLogic.Interfaces;
using DataAccess.Interfaces;
using Shared.Models;
using BusinessLogic.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[TestFixture]
public class MessageServiceTests
{
    [Test]
    public async Task MessageService_Should_Retrieve_Saved_Messages()
    {
        var mockRepo = new Mock<IMessageRepository>();
        mockRepo.Setup(repo => repo.GetMessages()).Returns(new List<Message>
        {
            new Message { Sender = "Alice", Content = "Hello", ReceivedTime = DateTime.Now }
        });
        
        var service = new MessageService(mockRepo.Object);
        var messages = await service.GetMessagesAsync();
        
        Assert.AreEqual(1, messages.Count);
        Assert.AreEqual("Alice", messages[0].Sender);
    }
}
