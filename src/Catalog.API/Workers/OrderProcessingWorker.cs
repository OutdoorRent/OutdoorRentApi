using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Catalog.API.Configurations;
using Catalog.API.Data;
using Catalog.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Catalog.API.Workers;

public class OrderProcessingWorker: BackgroundService
{
    private readonly ServiceBusProcessor _processor;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OrderProcessingWorker> _logger;
    
    public OrderProcessingWorker(IOptions<ServiceBusConfig> options, IServiceScopeFactory scopeFactory, ILogger<OrderProcessingWorker> logger)
    {
        var config = options.Value;
        var client = new ServiceBusClient(config.ConnectionString);
        _processor = client.CreateProcessor(config.QueueName, new ServiceBusProcessorOptions());
        _scopeFactory = scopeFactory;
        _logger = logger;
    }
    
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("start processing order worker");
        _processor.ProcessMessageAsync += OnMessageReceived;
        _processor.ProcessErrorAsync += OnError;
        await _processor.StartProcessingAsync(stoppingToken);
    }

    private async Task OnMessageReceived(ProcessMessageEventArgs args)
    {
        var body = args.Message.Body.ToString();
        var dto = JsonSerializer.Deserialize<OrderCreateRequestDto>(body);
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MyDbContext>();
        
        var conflict = await db.InventoryBookings.AnyAsync(b =>
            b.EquipmentId == dto.EquipmentId &&
            b.Date >= dto.StartDate && b.Date <= dto.EndDate);

        if (conflict)
        {
            _logger.LogWarning("There is an equipment conflict：{0}", dto.EquipmentId);
            await args.CompleteMessageAsync(args.Message);
            return;
        }

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            EquipmentId = dto.EquipmentId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        db.Orders.Add(order);

        foreach (var date in EachDateBetween(dto.StartDate, dto.EndDate))
        {
            db.InventoryBookings.Add(new InventoryBooking
            {
                Id = Guid.NewGuid(),
                EquipmentId = dto.EquipmentId,
                Date = date,
                OrderId = order.Id
            });
        }

        await db.SaveChangesAsync();
        _logger.LogInformation("订单已处理：{0}", order.Id);
        await args.CompleteMessageAsync(args.Message);
    }

    private Task OnError(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "error processing order");
        return Task.CompletedTask;
    }

    private static IEnumerable<DateTime> EachDateBetween(DateTime start, DateTime end)
    {
        for (var date = start.Date; date <= end.Date; date = date.AddDays(1))
            yield return date;
    }
}