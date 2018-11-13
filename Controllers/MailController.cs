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
    private readonly IEmailConfig _emailConfig;
    private EmailService _emailService;
    public MailController() {
      _emailConfig = new EmailConfig();
      _emailService = new EmailService(_emailConfig);
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
    private readonly IEmailConfig _emailConfig;
    public EmailService(IEmailConfig emailConfig) {
      _emailConfig = emailConfig;
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
        emailClient.Connect(Environment.GetEnvironmentVariable("SMTP_SERVER"), _emailConfig.SmtpPort, true);
        emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
        emailClient.Authenticate(_emailConfig.SmtpUser, _emailConfig.SmtpPass);
        emailClient.Send(msg);
        emailClient.Disconnect(true);
      }
    }
    
  }

  public interface IEmailConfig {
    string SmtpServer { get; }
    int SmtpPort { get; }
    string SmtpUser { get; set; }
    string SmtpPass { get; set; }
  }

  public class EmailConfig : IEmailConfig {
    public EmailConfig() {
        SmtpServer = Environment.GetEnvironmentVariable("STMP_SERVER");
        SmtpPort = Convert.ToInt32(Environment.GetEnvironmentVariable("SMTP_PORT"));
        SmtpUser = Environment.GetEnvironmentVariable("SMTP_USER");
        SmtpPass = Environment.GetEnvironmentVariable("SMTP_PASS");
    }
    public string SmtpServer { get; set; }
    public int SmtpPort { get; set; } 
    public string SmtpUser { get; set; }
    public string SmtpPass { get; set; }
  }
}