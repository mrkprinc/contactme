using HandlebarsDotNet;
using pfContactMe.Models;

namespace pfContactMe.Templates {
  public interface IMessageTemplate {
    string createHTML(EmailMessage msg);
  }

  public class MessageTemplate {
    private string _source;
    public MessageTemplate() {
      _source = @"
      <h1>A message from Portfolio</h1>
      <p><span>Contact Name: </span><span>{{contactName}}</span></p>
      <p><span>Contact Email: </span><span>{{contactEmail}}</span></p>
      <p><span>Received At: </span><span>{{timeStamp}}</span></p>
      <h2>Message:</h2>
      <p>{{message}}</p>
    ";
    }

    public string createHTML(EmailMessage msg) {
      var template = Handlebars.Compile(_source);
      return template(msg);
    }
  }
}