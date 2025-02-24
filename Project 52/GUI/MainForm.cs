using System.Windows.Forms;
namespace GUI;
using System;
using System.Windows.Forms;
using BusinessLogic;
using DataAccess;
using Shared;

public class MainForm : Form
{
    private ListBox callListBox;
    private ListBox messageListBox;
    private Button refreshButton;
    private ICallService callService;
    private IMessageService messageService;

    public MainForm()
    {
        Text = "SimCorp Mobile";
        Width = 600;
        Height = 400;

        callListBox = new ListBox { Dock = DockStyle.Top, Height = 150 };
        messageListBox = new ListBox { Dock = DockStyle.Bottom, Height = 150 };
        refreshButton = new Button { Text = "Refresh", Dock = DockStyle.Fill };
        refreshButton.Click += (s, e) => RefreshData();

        Controls.Add(callListBox);
        Controls.Add(refreshButton);
        Controls.Add(messageListBox);

        callService = new CallService(new CallRepository());
        messageService = new MessageService(new MessageRepository());

        RefreshData();
    }

    private void RefreshData()
    {
        callListBox.Items.Clear();
        messageListBox.Items.Clear();

        var calls = callService.GetCallsAsync().Result;
        var messages = messageService.GetMessagesAsync().Result;

        foreach (var call in calls)
        {
            callListBox.Items.Add($"{call.CallTime}: {call.PhoneNumber} (Incoming: {call.IsIncoming})");
        }

        foreach (var message in messages)
        {
            messageListBox.Items.Add($"{message.ReceivedTime}: {message.Sender} - {message.Content}");
        }
    }
}
