using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using pfContactMe.Models;
using MimeKit;
using MailKit.Net.Smtp;
using pfContactMe.Templates;

namespace pfContactMe.Controllers {
  [Route("/api/[controller]")]
  [ApiController]
  public class MailController : ControllerBase {
    private EmailService _emailService;
    public MailController() {
      _emailService = new EmailService();
    }

    [HttpPost]
    public string SendMessage([FromBody] EmailMessage msg) {
      _emailService.Send(msg);
      return "executed";
    }

    [HttpGet]
    public string TestMe() {
      return "api working";
    }
  }

  public interface IEmailService {
    void Send(EmailMessage emailMessage);
  }

  public class EmailService : IEmailService {
    private IDictionary<string, string> _emailConfig;
    public EmailService() {
      _emailConfig = (IDictionary<string, string>)Environment.GetEnvironmentVariables();
    }

    public void Send(EmailMessage emailMessage) {
      var msg = new MimeMessage();
      msg.To.Add(new MailboxAddress("me", "mark@markprince.ca"));
      msg.From.Add(new MailboxAddress("contactme", "contactme@markprince.ca"));
      msg.Subject = "A message from Portfolio - Contact Me";

      var template = new MessageTemplate();
      msg.Body = new TextPart("html") 
      {
        Text = template.createHTML(emailMessage)
      };

      using(var emailClient = new SmtpClient())
      {
        emailClient.Connect(_emailConfig["SMTP_SERVER"], Convert.ToInt32(_emailConfig["SMTP_PORT"]), true);
        emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
        // emailClient.Authenticate(_emailConfig, _emailConfig);
        emailClient.Send(msg);
        emailClient.Disconnect(true);
      }
    }
    
  }
}