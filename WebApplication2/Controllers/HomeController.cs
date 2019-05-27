using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{


    public class HomeController : Controller
    {

        const string ServiceBusConnectionString = "Endpoint=sb://ciprianxqueue1.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=GDotHc7eIwrB/XWaobsZXQQp2CqI9xXcbnXVZQt+CAE=";
        const string QueueName = "msgtoprocess";
        static IQueueClient queueClient;


        private readonly IConfiguration configurationSection;

        public HomeController(IConfiguration configurationSection)
        {
            this.configurationSection = configurationSection;
        }
        public IActionResult Index()
        {
            return View("Index", this.configurationSection.GetValue<string>("Environment"));
        }

        public IActionResult Privacy()
        {

            throw new Exception("error");
            return View();
        }

        public async Task<IActionResult> SendMessage()
        {
            queueClient = new QueueClient(ServiceBusConnectionString, QueueName);
            await SendMessagesAsync(2);
            await queueClient.CloseAsync();
            return View("Index");
        }

        static async Task SendMessagesAsync(int numberOfMessagesToSend)
        {
            try
            {
                for (var i = 0; i < numberOfMessagesToSend; i++)
                {
                    // Create a new message to send to the queue
                    string messageBody = $"Message {i}";
                    var message = new Message(Encoding.UTF8.GetBytes(messageBody));

                    // Write the body of the message to the console
                    Console.WriteLine($"Sending message: {messageBody}");

                    // Send the message to the queue
                    await queueClient.SendAsync(message);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }

        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
