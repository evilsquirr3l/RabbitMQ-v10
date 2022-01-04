using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ_v10.Models;
using RabbitMQ.Client;

namespace RabbitMQ_v10.Controllers;

[ApiController]
[Route("[controller]")]
public class BeerController : ControllerBase
{
    private readonly RabbitMqConfiguration _configuration;

    public BeerController(IOptions<RabbitMqConfiguration> configuration)
    {
        _configuration = configuration.Value;
    }

    [HttpPost]
    public IActionResult CreateBeer(Beer beer)
    {
        ValidateModel(beer);
        PublishMessage(JsonConvert.SerializeObject(beer));
        
        return Ok("Message is sent!");
    }

    private void ValidateModel(Beer beer)
    {
        if (beer.Ingredients.Contains("oil"))
        {
            throw new ArgumentException("Invalid ingredients!");
        }

        if (beer.Manufacturer.Name == "Obolon")
        {
            throw new ArgumentException("This is not a beer!");
        }
    }

    private void PublishMessage(string beer)
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

        var body = Encoding.UTF8.GetBytes(beer);

        model.BasicPublish(string.Empty, _configuration.QueueName, null, body);
    }
}