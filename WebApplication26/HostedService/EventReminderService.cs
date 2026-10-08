using WebApplication26.Data;
using WebApplication26.Services;
using Microsoft.EntityFrameworkCore;

namespace WebApplication26.HostedService
{
    public class EventReminderService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<EventReminderService> _logger;

        public EventReminderService(
            IServiceScopeFactory scopeFactory,
            ILogger<EventReminderService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Event Reminder Service запущено.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var db = scope.ServiceProvider
                        .GetRequiredService<ApplicationDbContext>();

                    var emailService = scope.ServiceProvider
                        .GetRequiredService<EmailService>();

                    var now = DateTime.Now;

                    var reminderStart = now.AddMinutes(9);
                    var reminderEnd = now.AddMinutes(11);

                    var events = await db.Events
                        .Where(e =>
                            !e.NotificationSent &&
                            e.EventDate >= reminderStart &&
                            e.EventDate <= reminderEnd)
                        .ToListAsync(stoppingToken);

                    foreach (var eventItem in events)
                    {
                        try
                        {
                            var subject =
                                $"Нагадування про подію: {eventItem.Title}";

                            var message =
                                $"Нагадування!\n\n" +
                                $"Через 10 хвилин відбудеться подія:\n\n" +
                                $"Назва: {eventItem.Title}\n" +
                                $"Дата: {eventItem.EventDate:dd.MM.yyyy HH:mm}\n" +
                                $"Опис: {eventItem.Description}";

                            await emailService.SendEmailAsync(
                                eventItem.UserEmail,
                                subject,
                                message);

                            eventItem.NotificationSent = true;

                            await db.SaveChangesAsync(
                                stoppingToken);

                            _logger.LogInformation(
                                "Нагадування відправлено на {Email}",
                                eventItem.UserEmail);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(
                                ex,
                                "Помилка відправки email");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Помилка BackgroundService");
                }

                await Task.Delay(
                    TimeSpan.FromMinutes(1),
                    stoppingToken);
            }
        }
    }
}
