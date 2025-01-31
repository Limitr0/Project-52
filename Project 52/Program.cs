// Проект разделен на несколько под-проектов:
// - SharedComponents (интерфейсы и базовые классы)
// - BusinessLogic (логика работы, например SMSProvider, Battery, CallHistory)
// - ConsoleApp (консольное приложение)
// - GUI (оконное приложение на WinForms)
// - UnitTests (проект с юнит-тестами)

// В каждом файле теперь только один класс или интерфейс.

// BusinessLogic/CallHistory.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class CallHistory
{
    private List<Call> calls = new List<Call>();
    public event EventHandler CallsUpdated;

    public void AddCall(Call call)
    {
        var lastCall = calls.FirstOrDefault(c => c.PhoneNumber == call.PhoneNumber && c.IsIncoming == call.IsIncoming);
        if (lastCall != null && (call.CallTime - lastCall.CallTime).TotalSeconds < 60)
        {
            lastCall.Duration += call.Duration;
        }
        else
        {
            calls.Add(call);
            calls.Sort();
        }
        CallsUpdated?.Invoke(this, EventArgs.Empty);
    }

    public async Task<List<Call>> GetCallsAsync(bool? isIncoming = null)
    {
        return await Task.Run(() =>
            calls
                .Where(c => isIncoming == null || c.IsIncoming == isIncoming)
                .OrderByDescending(c => c.CallTime)
                .ToList());
    }
}

// BusinessLogic/MessageHistory.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class MessageHistory
{
    private List<Message> messages = new List<Message>();
    public event EventHandler MessagesUpdated;

    public void AddMessage(Message message)
    {
        messages.Add(message);
        MessagesUpdated?.Invoke(this, EventArgs.Empty);
    }

    public async Task<List<Message>> GetMessagesAsync(string senderFilter = "", string textFilter = "")
    {
        return await Task.Run(() =>
            messages
                .Where(m => (string.IsNullOrEmpty(senderFilter) || m.Sender.Contains(senderFilter))
                         && (string.IsNullOrEmpty(textFilter) || m.Content.Contains(textFilter)))
                .OrderByDescending(m => m.ReceivedTime)
                .ToList());
    }
}

// GUI/MainForm.cs
using System;
using System.Windows.Forms;
using System.Threading.Tasks;

public class MainForm : Form
{
    private ListBox callListBox;
    private ListBox messageListBox;
    private Button refreshButton;
    private CallHistory callHistory;
    private MessageHistory messageHistory;

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

        callHistory = new CallHistory();
        messageHistory = new MessageHistory();
    }

    private async Task RefreshData()
    {
        callListBox.Items.Clear();
        messageListBox.Items.Clear();

        var calls = await callHistory.GetCallsAsync();
        foreach (var call in calls)
        {
            callListBox.Items.Add(call.ToString());
        }

        var messages = await messageHistory.GetMessagesAsync();
        foreach (var message in messages)
        {
            messageListBox.Items.Add(message.ToString());
        }
    }
}

// UnitTests/FiltersTests.cs
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[TestFixture]
public class FiltersTests
{
    [Test]
    public async Task CallHistory_Should_Filter_Incoming_Calls_Async()
    {
        var callHistory = new CallHistory();
        callHistory.AddCall(new Call { PhoneNumber = "12345", CallTime = DateTime.Now, IsIncoming = true });
        callHistory.AddCall(new Call { PhoneNumber = "67890", CallTime = DateTime.Now, IsIncoming = false });
        
        var incomingCalls = await callHistory.GetCallsAsync(true);
        Assert.AreEqual(1, incomingCalls.Count);
        Assert.IsTrue(incomingCalls[0].IsIncoming);
    }

    [Test]
    public async Task MessageHistory_Should_Filter_By_Sender_Async()
    {
        var messageHistory = new MessageHistory();
        messageHistory.AddMessage(new Message { Sender = "Alice", Content = "Hello" });
        messageHistory.AddMessage(new Message { Sender = "Bob", Content = "Hi" });
        
        var filteredMessages = await messageHistory.GetMessagesAsync(senderFilter: "Alice");
        Assert.AreEqual(1, filteredMessages.Count);
        Assert.AreEqual("Alice", filteredMessages[0].Sender);
    }
}



