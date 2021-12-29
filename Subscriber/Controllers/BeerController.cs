using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Subscriber.Controllers;

[ApiController]
[Route("[controller]")]
public class BeerController : ControllerBase
{
    private readonly RabbitMqConfiguration _configuration;
    private readonly IDbContextFactory<BeerDbContext> _dbContextFactory;

    public BeerController(IOptions<RabbitMqConfiguration> configuration, IDbContextFactory<BeerDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        _configuration = configuration.Value;
    }

    [HttpGet]
    public IActionResult ReadMessage()
    {
        GetMessage();

        return Ok("Message is here!");
    }

    private void GetMessage()
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration.Hostname,
            UserName = _configuration.UserName,
            Password = _configuration.Password
        };

        using var connection = factory.CreateConnection();
        using var model = connection.CreateModel();

        model.QueueDeclare(_configuration.QueueName, false, false, false, null);

        var consumer = new EventingBasicConsumer(model);

        consumer.Received += (sender, e) =>
        {
            var body = e.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine(message);
            SendToDb(message);
        };

        model.BasicConsume(_configuration.QueueName, true, consumer);

    }
    void SendToDb(string message)
    {
        var beer = new Beer()
        {
            Id = Guid.NewGuid().ToString(),
            Description = message
        };

        try
        {
            using var db = _dbContextFactory.CreateDbContext();
            db.Beer.Add(beer);

            db.SaveChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}