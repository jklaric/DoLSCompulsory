
using Cleaner.Models;
using EasyNetQ;

namespace Cleaner.Handler;

public class MessagePublisher(IBus bus)
{
    public async Task PublishCleanedEmail(ProcessedEmailDto cleanedEmail)
    {
        Console.WriteLine("Publishing email: " + cleanedEmail.EmailName);
        await bus.PubSub.PublishAsync(cleanedEmail, "CleanEmail");
        Console.WriteLine("Published email successfully: " + cleanedEmail.EmailName);
    }
}