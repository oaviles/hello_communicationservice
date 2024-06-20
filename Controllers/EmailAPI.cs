using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Azure;
using Azure.Communication.Email;

namespace oaemailapi.Controllers;

[ApiController]
[Route("[controller]")]
public class EmailAPIController : ControllerBase
{

    private readonly ILogger<EmailAPIController> _logger;

    public EmailAPIController(ILogger<EmailAPIController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetEmailApi")]
    public async Task<IActionResult> Get([FromQuery] string emailAddress)
    {
         try
        {

            string connectionString = Environment.GetEnvironmentVariable("COMMUNICATION_SERVICES_CONNECTION_STRING");
            //string connectionString = "Your Connection String Here";
            EmailClient emailClient = new EmailClient(connectionString);

            string messageGuid = Guid.NewGuid().ToString();

            string subject = "Test Message from Azure Communication Service OA Email API.";
            string htmlContent = $"<html><body><h1>Quick send email test: {messageGuid}</h1><br/><h4>This email message is sent from Azure Communication Service Email.</h4><p>This mail was sent using .NET SDK!!</p></body></html>";
            var sender = "donotreply@xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx.azurecomm.net";
            string recipient = emailAddress;
            
            EmailSendOperation emailSendOperation = await emailClient.SendAsync(
                Azure.WaitUntil.Completed,
                sender,
                recipient,
                subject,
                htmlContent);
            EmailSendResult statusMonitor = emailSendOperation.Value;

            // Get the GUID of the email send operation
            string guid = emailSendOperation.Id.ToString();

            return Ok(new { Message = "Email send operation completed successfully:", guid });
        }
        catch (RequestFailedException ex)
        {
                        return BadRequest(new { Message = "Email send operation failed", Error = ex.Message });

        }
    }
}
