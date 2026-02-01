using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace ExpenseSplitter.Services
{
    public interface IEmailService
    {
        Task SendInvitationEmailAsync(string toEmail, string groupName, string inviterName, string invitationToken);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _frontendUrl;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _frontendUrl = _configuration["Frontend:Url"] ?? "http://localhost:3000";
        }

        public async Task SendInvitationEmailAsync(string toEmail, string groupName, string inviterName, string invitationToken)
        {
            var smtpHost = _configuration["Email:SmtpHost"];
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            var smtpUser = _configuration["Email:SmtpUser"];
            var smtpPass = _configuration["Email:SmtpPassword"];
            var fromEmail = _configuration["Email:FromEmail"];
            var fromName = _configuration["Email:FromName"] ?? "Expense Splitter";

            // For development, if SMTP is not configured, just log
            if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPass))
            {
                Console.WriteLine($"[EMAIL] Would send invitation to: {toEmail}");
                Console.WriteLine($"[EMAIL] Group: {groupName}");
                Console.WriteLine($"[EMAIL] Inviter: {inviterName}");
                Console.WriteLine($"[EMAIL] Link: {_frontendUrl}/accept-invitation?token={invitationToken}");
                return;
            }

            var invitationUrl = $"{_frontendUrl}/accept-invitation?token={invitationToken}";

            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = $"You've been invited to join '{groupName}' on Expense Splitter",
                    Body = GenerateEmailBody(groupName, inviterName, invitationUrl),
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                using var smtpClient = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUser, smtpPass),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false
                };

                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine($"[EMAIL] Successfully sent invitation to: {toEmail}");
            }
            catch (SmtpException ex)
            {
                Console.WriteLine($"[EMAIL ERROR] Failed to send email to {toEmail}");
                Console.WriteLine($"[EMAIL ERROR] Message: {ex.Message}");
                Console.WriteLine($"[EMAIL ERROR] Status: {ex.StatusCode}");
                Console.WriteLine($"[EMAIL ERROR] Gmail users: Make sure you're using an App Password, not your regular password!");
                Console.WriteLine($"[EMAIL ERROR] Generate one at: https://myaccount.google.com/apppasswords");
                Console.WriteLine($"[EMAIL ERROR] Invitation link (for testing): {_frontendUrl}/accept-invitation?token={invitationToken}");
                throw new Exception($"Failed to send invitation email. Please check SMTP configuration. Gmail users need App Passwords.");
            }
        }

        private string GenerateEmailBody(string groupName, string inviterName, string invitationUrl)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>Expense Splitter Invitation</title>
    <style>
        body {{ font-family: 'Segoe UI', Arial, sans-serif; background: #f4f6f8; margin: 0; padding: 0; }}
        .container {{ max-width: 600px; margin: 40px auto; background: #fff; border-radius: 10px; box-shadow: 0 2px 8px rgba(0,0,0,0.07); overflow: hidden; }}
        .header {{ background: linear-gradient(90deg, #2563EB 0%, #1E40AF 100%); color: #fff; padding: 32px 24px; text-align: center; }}
        .header h1 {{ margin: 0; font-size: 2.2em; letter-spacing: 1px; }}
        .content {{ padding: 32px 24px; }}
        .content h2 {{ color: #2563EB; margin-top: 0; }}
        .button {{ display: inline-block; background: #2563EB; color: #fff; padding: 14px 36px; font-size: 1.1em; text-decoration: none; border-radius: 6px; margin: 28px 0; font-weight: 600; box-shadow: 0 2px 4px rgba(37,99,235,0.12); transition: background 0.2s; }}
        .button:hover {{ background: #1E40AF; }}
        .details {{ font-size: 1em; color: #374151; margin-bottom: 18px; }}
        .footer {{ background: #f1f5f9; text-align: center; padding: 18px 24px; color: #6b7280; font-size: 0.95em; border-radius: 0 0 10px 10px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Expense Splitter</h1>
        </div>
        <div class='content'>
            <h2>You're Invited to Join a Group!</h2>
            <p class='details'><strong>{inviterName}</strong> has invited you to join the group <strong>{groupName}</strong> on <b>Expense Splitter</b>.</p>
            <p class='details'>Expense Splitter helps you track shared expenses, split bills, and settle up easily with friends and family.</p>
            <p style='text-align: center;'>
                <a href='{invitationUrl}' class='button'>Accept Invitation</a>
            </p>
            <p class='details'>If you don't have an account yet, you'll be prompted to create one. If you already have an account, simply log in to join the group.</p>
            <p class='details'>This invitation will expire in <b>7 days</b>. If you received multiple invitations for the same group, only the most recent one is valid.</p>
        </div>
        <div class='footer'>
            <p>If you did not expect this invitation, you can safely ignore this email.</p>
            <p>Expense Splitter &copy; {DateTime.UtcNow.Year}</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}
