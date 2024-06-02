using Domain.Services.Abstractions;

namespace Api.BackgroundServices;

public class NotificationBackgroundServce(IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            using var scope = serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<INotificationService>();
            service.GenerateEventStartsSoonNotifications();
        }
    }
}
