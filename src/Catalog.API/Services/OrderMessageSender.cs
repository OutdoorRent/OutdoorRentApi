using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Catalog.API.Configurations;
using Catalog.API.Models;
using Microsoft.Extensions.Options;

namespace Catalog.API.Services;

public class OrderMessageSender
{
    private readonly ServiceBusSender _sender;

    public OrderMessageSender(IOptions<ServiceBusConfig> options)
    {
        var config = options.Value;
        var client = new ServiceBusClient(config.ConnectionString);
        _sender = client.CreateSender(config.QueueName);
    }

    public async Task CreateOrderAsync(OrderCreateRequestDto dto)
    {
        var json = JsonSerializer.Serialize(dto);
        var message = new ServiceBusMessage(json);
        await _sender.SendMessageAsync(message);
    }
}