using FluentEmail.Core;
using Services.Interfaces;

namespace Providers.Mail;

public class MailAccessor : IMailAccessor
{
    private readonly IFluentEmail _mail;
    private readonly IApiAccessor _ApiAccessor;
    public MailAccessor(IFluentEmail mail, IApiAccessor ApiAccessor)
    {
        _ApiAccessor = ApiAccessor;
        _mail = mail;
    }

    public async Task<bool> SendMail(string to, string subject, string displayName, MailButton mailButton, string body = null)
    {
        body = body is not null ? body : "Test email of RentX";

        mailButton.Link += mailButton.Link ?? $"{_ApiAccessor.GetOrigin()}";
        mailButton.Text += mailButton.Text ?? $"Go to RentX";

        var path = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "test.cshtml");

        var newEmail = await _mail
            .To(to)
            .Subject(subject)
            .UsingTemplateFromFile(path, new { DisplayName = displayName, Body = body, MailButton = mailButton })
            .SendAsync();

        if (!newEmail.Successful) return false;

        return true;
    }
}
