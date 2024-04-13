namespace CityInfo.API.Services
{
    public class LocalMailService : IMailService
    {
        private string mailTo = "admin@cityinfo.com";

        private string mailFrom = "noreply@cityinfo.com";

        public void Send(string subject, string message)
        {
            Console.WriteLine($"Mail from {mailFrom} mail to {mailTo}, with {nameof(LocalMailService)}. ");
            Console.WriteLine($"Subjet : {subject}");
            Console.WriteLine($"Mail: {message}");

        }
    }
}
