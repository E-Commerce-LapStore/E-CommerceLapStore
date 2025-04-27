using System;
using System.IO;
using System.Threading.Tasks;
using LapStore.BLL.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace LapStore.BLL.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _senderEmail;
        private readonly string _senderName;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _smtpServer = _configuration["EmailSettings:SmtpServer"];
            _smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            _smtpUsername = _configuration["EmailSettings:SmtpUsername"];
            _smtpPassword = _configuration["EmailSettings:SmtpPassword"];
            _senderEmail = _configuration["EmailSettings:SenderEmail"];
            _senderName = _configuration["EmailSettings:SenderName"];

            // Validate SMTP settings
            if (string.IsNullOrEmpty(_smtpServer))
                throw new ArgumentException("SMTP server is not configured");
            if (_smtpPort <= 0)
                throw new ArgumentException("Invalid SMTP port");
            if (string.IsNullOrEmpty(_smtpUsername))
                throw new ArgumentException("SMTP username is not configured");
            if (string.IsNullOrEmpty(_smtpPassword))
                throw new ArgumentException("SMTP password is not configured");
            if (string.IsNullOrEmpty(_senderEmail))
                throw new ArgumentException("Sender email is not configured");
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_senderName, _senderEmail));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            if (isHtml)
                bodyBuilder.HtmlBody = body;
            else
                bodyBuilder.TextBody = body;

            email.Body = bodyBuilder.ToMessageBody();

            await SendEmailMessageAsync(email);
        }

        public async Task SendEmailWithAttachmentAsync(string to, string subject, string body, string attachmentPath, bool isHtml = false)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_senderName, _senderEmail));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            if (isHtml)
                bodyBuilder.HtmlBody = body;
            else
                bodyBuilder.TextBody = body;

            if (File.Exists(attachmentPath))
            {
                bodyBuilder.Attachments.Add(attachmentPath);
            }
            else
            {
                throw new FileNotFoundException("Attachment file not found", attachmentPath);
            }

            email.Body = bodyBuilder.ToMessageBody();

            await SendEmailMessageAsync(email);
        }

        private async Task SendEmailMessageAsync(MimeMessage email)
        {
            using var client = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                await client.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);
                
                // Log authentication attempt
                Console.WriteLine($"Attempting to authenticate with username: {_smtpUsername}");
                
                await client.AuthenticateAsync(_smtpUsername, _smtpPassword);
                await client.SendAsync(email);
            }
            catch (Exception ex)
            {
                // Log the detailed error
                Console.WriteLine($"SMTP Error Details: {ex.GetType().Name} - {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                
                throw new ApplicationException($"Failed to send email: {ex.Message}", ex);
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }

        public async Task SendEmailConfirmationAsync(string email, string confirmationLink)
        {
            var subject = "Confirm your email address";
            var body = $@"
                <h2>Welcome to LapStore!</h2>
                <p>Please confirm your email address by clicking the link below:</p>
                <p><a href='{confirmationLink}'>Confirm Email</a></p>
                <p>If you didn't create an account with us, please ignore this email.</p>";

            await SendEmailAsync(email, subject, body, true);
        }

        public async Task SendPasswordResetAsync(string email, string resetLink)
        {
            var subject = "Reset your password";
            var body = $@"
                <h2>Password Reset Request</h2>
                <p>You have requested to reset your password. Click the link below to proceed:</p>
                <p><a href='{resetLink}'>Reset Password</a></p>
                <p>If you didn't request a password reset, please ignore this email.</p>";

            await SendEmailAsync(email, subject, body, true);
        }

        public async Task SendOrderConfirmationAsync(string email, string orderDetails)
        {
            var subject = "Order Confirmation";
            var body = $@"
                <h2>Thank you for your order!</h2>
                <p>We're processing your order and will send you updates soon.</p>
                <h3>Order Details:</h3>
                {orderDetails}
                <p>If you have any questions, please contact our support team.</p>";

            await SendEmailAsync(email, subject, body, true);
        }
    }
} 