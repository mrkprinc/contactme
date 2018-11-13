using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
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
      return "Email sent.";
    }

    [HttpGet]
    public string TestMe() {
      return "API ready...";
    }
  }

  public interface IEmailService {
    void Send(EmailMessage emailMessage);
  }

  public class EmailService : IEmailService {
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
        emailClient.Connect(Environment.GetEnvironmentVariable("SMTP_SERVER"), Convert.ToInt32(Environment.GetEnvironmentVariable("SMTP_PORT")), true);
        emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
        emailClient.Authenticate(Environment.GetEnvironmentVariable("SMTP_USER"), Environment.GetEnvironmentVariable("SMTP_PASS"));
        emailClient.Send(msg);
        emailClient.Disconnect(true);
      }
    }
    
  }
}