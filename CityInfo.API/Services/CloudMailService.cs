namespace CityInfo.API.Services
{
    public class CloudMailService : IMailService
    {
        private string _mailTo = string.Empty;

        private string _mailFrom = string.Empty;

        public CloudMailService(IConfiguration configuration)
        {
            _mailFrom = configuration["mailSettings:mailFromAddress"];
            _mailTo = configuration["mailSettings:mailToAddress"];
        }

        public void Send(string subject, string message)
        {
            Console.WriteLine($"Mail from {_mailFrom} mail to {_mailTo}, with {nameof(CloudMailService)}. ");
            Console.WriteLine($"Subjet : {subject}");
            Console.WriteLine($"Mail: {message}");

        }
    }
}
