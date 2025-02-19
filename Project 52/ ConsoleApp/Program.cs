namespace ConsoleApp;
using BusinessLogic;
using DataAccess;
using Shared;
using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("Console Application Running...");

        ICallRepository callRepository = new CallRepository();
        IMessageRepository messageRepository = new MessageRepository();

        ICallService callService = new CallService(callRepository);
        IMessageService messageService = new MessageService(messageRepository);

        callService.AddCall(new Call { PhoneNumber = "12345", CallTime = DateTime.Now, IsIncoming = true });
        messageService.AddMessage(new Message { Sender = "Alice", Content = "Hello!", ReceivedTime = DateTime.Now });

        var calls = callService.GetCallsAsync().Result;
        var messages = messageService.GetMessagesAsync().Result;

        Console.WriteLine("Calls:");
        foreach (var call in calls)
        {
            Console.WriteLine($"{call.CallTime}: {call.PhoneNumber} (Incoming: {call.IsIncoming})");
        }

        Console.WriteLine("\nMessages:");
        foreach (var message in messages)
        {
            Console.WriteLine($"{message.ReceivedTime}: {message.Sender} - {message.Content}");
        }

        Console.ReadKey();
    }
}
