namespace CityInfo.API.Services
{
    public class LocalMailService : IMailService
    {
        private string mailTo = string.Empty;

        private string mailFrom = string.Empty;

        public LocalMailService(IConfiguration configuration)
        {
            mailFrom = configuration["mailSettings:mailFromAddress"];
            mailTo = configuration["mailSettings:mailToAddress"];
        }

        public void Send(string subject, string message)
        {
            Console.WriteLine($"Mail from {mailFrom} mail to {mailTo}, with {nameof(LocalMailService)}. ");
            Console.WriteLine($"Subjet : {subject}");
            Console.WriteLine($"Mail: {message}");

        }
    }
}
