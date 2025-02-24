using System;
using System.Windows.Forms;
using BusinessLogic;
using DataAccess;
using Shared;
using System.Threading.Tasks;

namespace GUI;

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
        refreshButton.Click += async (s, e) => await RefreshDataAsync();

        Controls.Add(callListBox);
        Controls.Add(refreshButton);
        Controls.Add(messageListBox);

        callService = new CallService(new CallRepository());
        messageService = new MessageService(new MessageRepository());

        Task.Run(async () => await RefreshDataAsync()).Wait(); // Асинхронный вызов при старте
    }

    private async Task RefreshDataAsync()
    {
        try
        {
            callListBox.Items.Clear();
            messageListBox.Items.Clear();

            var calls = await callService.GetCallsAsync();
            var messages = await messageService.GetMessagesAsync();

            foreach (var call in calls)
            {
                callListBox.Items.Add($"{call.CallTime}: {call.PhoneNumber} (Incoming: {call.IsIncoming})");
            }

            foreach (var message in messages)
            {
                messageListBox.Items.Add($"{message.ReceivedTime}: {message.Sender} - {message.Content}");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
