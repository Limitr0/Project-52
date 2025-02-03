// SharedComponents (Общие интерфейсы и классы)
namespace SharedComponents
{
    public interface IScreen
    {
        void Show(string content);
    }

    public class Battery
    {
        public int Charge { get; private set; } = 100;
        public void Use(int amount) => Charge = Math.Max(0, Charge - amount);
        public void ChargeBattery(int amount) => Charge = Math.Min(100, Charge + amount);
    }

    public class SimCard
    {
        public string Number { get; }
        public SimCard(string number) => Number = number;
    }

    public interface IPlayback
    {
        void PlaySound(string sound);
    }
}

// BusinessLogic (Логика работы)
namespace BusinessLogic
{
    using SharedComponents;
    
    public class MonochromeScreen : IScreen
    {
        public void Show(string content) => Console.WriteLine("[Monochrome] " + content);
    }

    public class PhoneSpeaker : IPlayback
    {
        public void PlaySound(string sound) => Console.WriteLine("Playing sound: " + sound);
    }

    public class MobilePhone
    {
        public IScreen Screen { get; }
        public Battery Battery { get; }
        public SimCard Sim { get; }
        public IPlayback Speaker { get; }

        public MobilePhone(IScreen screen, SimCard sim, IPlayback speaker)
        {
            Screen = screen;
            Sim = sim;
            Battery = new Battery();
            Speaker = speaker;
        }

        public void DisplayInfo()
        {
            Screen.Show($"SIM: {Sim.Number}, Battery: {Battery.Charge}%");
        }

        public void PlayRingtone()
        {
            Speaker.PlaySound("Default Ringtone");
        }
    }
}

// ConsoleApp (Консольное приложение)
namespace ConsoleApp
{
    using BusinessLogic;
    using SharedComponents;
    
    class Program
    {
        static void Main()
        {
            var phone = new MobilePhone(new MonochromeScreen(), new SimCard("+123456789"), new PhoneSpeaker());
            phone.DisplayInfo();
            phone.PlayRingtone();
        }
    }
}

// UnitTests (Простые тесты)
namespace UnitTests
{
    using BusinessLogic;
    using SharedComponents;
    using NUnit.Framework;
    
    [TestFixture]
    public class MobilePhoneTests
    {
        [Test]
        public void Battery_Decreases_WhenUsed()
        {
            var phone = new MobilePhone(new MonochromeScreen(), new SimCard("+123456789"), new PhoneSpeaker());
            phone.Battery.Use(10);
            Assert.AreEqual(90, phone.Battery.Charge);
        }

        [Test]
        public void Battery_Increases_WhenCharged()
        {
            var phone = new MobilePhone(new MonochromeScreen(), new SimCard("+123456789"), new PhoneSpeaker());
            phone.Battery.Use(50);
            phone.Battery.ChargeBattery(30);
            Assert.AreEqual(80, phone.Battery.Charge);
        }
    }
}
