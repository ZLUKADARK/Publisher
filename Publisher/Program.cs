using Newtonsoft.Json;
using Publisher.Dto;
using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory() { HostName = "localhost" };
var connection = factory.CreateConnection();
var channel = connection.CreateModel();
channel.ExchangeDeclare("MultiThread", ExchangeType.Direct, true, false);

var count = 0;
while (true)
{
    var message = new EmailMessage
    {
        ContentHtml = "Основной текс: " + count,
        Id = Guid.NewGuid().ToString(),
        SenderName = "Ахмад",
        Subject = "Проверка: " + TimeOnly.FromDateTime(DateTime.Now),
        ContentPlain = "ContentPlain",
        Receiver = new EmailReceiverInformation("Ahmad", "ahmadcakaev@mail.ru")
    };

    channel.ConfirmSelect(); channel.ConfirmSelect();
    var json = JsonConvert.SerializeObject(message);

    var body = Encoding.UTF8.GetBytes(json);

    channel.BasicPublish(
        "MultiThread",
        "MultiThread.Queue",
        null,
        body);

    Console.WriteLine($"Отпарвлено: {count} | {message.Id} | {message.ContentHtml}");
    count++;
    //Thread.Sleep(500);

    channel.WaitForConfirmsOrDie(TimeSpan.FromSeconds(5));
}


