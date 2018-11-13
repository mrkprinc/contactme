using HandlebarsDotNet;
using pfContactMe.Templates;

namespace pfContactMe.Models {
  public class EmailMessage {
    public string Message { get; set; }
    public string ContactName { get; set; }
    public string ContactEmail { get; set; }
    public string TimeStamp { get; set; }
  }
}