namespace BLL.Helpers;

public class SmtpConfig
{
    public string Host => Environment.GetEnvironmentVariable("SmtpHost");
    public int Port => int.TryParse(Environment.GetEnvironmentVariable("SmtpPort"), out var port) ? port : 0;
    public string Username => Environment.GetEnvironmentVariable("SmtpUsername");
    public string Password => Environment.GetEnvironmentVariable("SmtpPassword");
    public string SenderName => Environment.GetEnvironmentVariable("SmtpSenderName");
}