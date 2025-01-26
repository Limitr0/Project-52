using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MobilePhoneLab
{
    // Interfaces
    public interface IScreenImage
    {
        string Content { get; }
    }

    public interface IPlayback
    {
        void Play(string soundData);
    }

    public interface IOutput
    {
        void Write(string text);
    }

    // Output classes
    public class ConsoleOutput : IOutput
    {
        public void Write(string text)
        {
            Console.WriteLine(text);
        }
    }

    // Base classes and components
    public abstract class ScreenBase
    {
        public abstract void Show(IScreenImage screenImage);

        public override string ToString() => "Generic Screen";
    }

    public class MonochromeScreen : ScreenBase
    {
        public override void Show(IScreenImage screenImage)
        {
            Console.WriteLine($"[Monochrome Screen] Displaying: {screenImage.Content}");
        }

        public override string ToString() => "Monochrome Screen";
    }

    public class ColorfulScreen : ScreenBase
    {
        public override void Show(IScreenImage screenImage)
        {
            Console.WriteLine($"[Colorful Screen] Displaying in color: {screenImage.Content}");
        }

        public override string ToString() => "Colorful Screen";
    }

    public class OLEDScreen : ColorfulScreen
    {
        public override void Show(IScreenImage screenImage)
        {
            Console.WriteLine($"[OLED Screen] Vivid Display: {screenImage.Content}");
        }

        public override string ToString() => "OLED Screen";
    }

    public abstract class Mobile
    {
        public abstract ScreenBase Screen { get; }
        public abstract IPlayback PlaybackDevice { get; }

        public string GetDescription()
        {
            var descriptionBuilder = new StringBuilder();
            descriptionBuilder.AppendLine($"Screen Type: {Screen}");
            return descriptionBuilder.ToString();
        }
    }

    public class SimCorpMobile : Mobile
    {
        private readonly OLEDScreen _oledScreen = new OLEDScreen();
        private readonly PhoneSpeaker _phoneSpeaker = new PhoneSpeaker(new ConsoleOutput());
        public SMSProvider SmsProvider { get; } = new SMSProvider();

        public override ScreenBase Screen => _oledScreen;
        public override IPlayback PlaybackDevice => _phoneSpeaker;
    }

    public class PhoneSpeaker : IPlayback
    {
        private readonly IOutput _output;

        public PhoneSpeaker(IOutput output)
        {
            _output = output;
        }

        public void Play(string soundData)
        {
            _output.Write($"Playing sound: {soundData} through phone speaker");
        }
    }

    public class SMSProvider
    {
        public event EventHandler<MessageEventArgs> SMSReceived;
        private readonly Random _random = new Random();
        private bool _running;

        public void Start()
        {
            _running = true;
            Task.Run(() => GenerateMessages());
        }

        public void Stop()
        {
            _running = false;
        }

        private void GenerateMessages()
        {
            while (_running)
            {
                Thread.Sleep(_random.Next(2000, 5000)); // Generate message every 2-5 seconds
                var message = new Message
                {
                    Sender = $"User{_random.Next(1, 10)}",
                    Content = $"Hello from User{_random.Next(1, 10)}!",
                    ReceivedTime = DateTime.Now
                };
                OnSMSReceived(message);
            }
        }

        protected virtual void OnSMSReceived(Message message)
        {
            SMSReceived?.Invoke(this, new MessageEventArgs { Message = message });
        }
    }

    public class Message
    {
        public string Sender { get; set; }
        public string Content { get; set; }
        public DateTime ReceivedTime { get; set; }

        public override string ToString()
        {
            return $"[{ReceivedTime:HH:mm:ss}] {Sender}: {Content}";
        }
    }

    public class MessageEventArgs : EventArgs
    {
        public Message Message { get; set; }
    }

    // Console Application
    class Program
    {
        static void Main(string[] args)
        {
            var myPhone = new SimCorpMobile();
            Console.WriteLine("--- Mobile Phone Description ---");
            Console.WriteLine(myPhone.GetDescription());

            // Display an image on the phone's screen
            IScreenImage sampleImage = new ScreenImage { Content = "Hello, OOP World!" };
            myPhone.Screen.Show(sampleImage);

            // Play sound through playback device
            myPhone.PlaybackDevice.Play("Startup Sound");

            // SMS simulation
            myPhone.SmsProvider.SMSReceived += (sender, eventArgs) =>
            {
                Console.WriteLine($"New SMS: {eventArgs.Message}");
            };

            Console.WriteLine("Starting SMS generator...");
            myPhone.SmsProvider.Start();

            Console.WriteLine("\nPress any key to stop SMS generator...");
            Console.ReadKey();
            myPhone.SmsProvider.Stop();
        }
    }

    // Sample screen image
    public class ScreenImage : IScreenImage
    {
        public string Content { get; set; }
    }
}


