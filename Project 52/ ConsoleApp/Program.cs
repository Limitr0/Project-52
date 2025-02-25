using System;
using BusinessLogic.Services;
using DataAccess.Repositories;
using Shared.Models;
using System.Threading.Tasks;

namespace ConsoleApp
{
    internal class Program
    {
        static async Task Main()
        {
            Console.WriteLine("Console Application Running...\n");

            // Создаем сервисы
            var callService = new CallService(new CallRepository());
            var messageService = new MessageService(new MessageRepository());

            // Добавляем тестовые данные
            callService.AddCall(new Call { PhoneNumber = "12345", CallTime = DateTime.Now, IsIncoming = true });
            messageService.AddMessage(new Message { Sender = "Alice", Content = "Hello!", ReceivedTime = DateTime.Now });

            // Получаем и выводим звонки
            var calls = await callService.GetCallsAsync();
            Console.WriteLine("Calls:");
            foreach (var call in calls)
            {
                Console.WriteLine($"{call.CallTime}: {call.PhoneNumber} (Incoming: {call.IsIncoming})");
            }

            // Получаем и выводим сообщения
            var messages = await messageService.GetMessagesAsync();
            Console.WriteLine("\nMessages:");
            foreach (var message in messages)
            {
                Console.WriteLine($"{message.ReceivedTime}: {message.Sender} - {message.Content}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
