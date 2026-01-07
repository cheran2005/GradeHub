using Microsoft.AspNetCore.Mvc;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using GradifyApi.Data;
using Microsoft.Extensions.Options;


namespace GradifyApi.Service
{
    public class EmailService
    {
        private readonly SmtpSettings _smtp;

        //Dependency Injection for smtp configurations
        public EmailService(IOptions<SmtpSettings> smtpOptions)
        {
            _smtp = smtpOptions.Value;
        }


        //Send Email Verificaiton Code Method
        public async Task SendEmailverifyAsync(string ToEmail, int VerficationCode)
        {
            //Creating email message object
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Gradify", _smtp.Username));
            message.To.Add(MailboxAddress.Parse(ToEmail));
            message.Subject = "Your verification code";

            message.Body = new TextPart("plain")
            {
                Text = $"Your verification code is: {VerficationCode}\nThis code expires in 15 minutes."
            };

            //smtp object to send email
            using var smtp = new SmtpClient();
            //connect to gradify email account
            await smtp.ConnectAsync(
                _smtp.Host,
                _smtp.Port,
                SecureSocketOptions.StartTls
            );
            await smtp.AuthenticateAsync(
                _smtp.Username,
                _smtp.Password
            );

            //send email
            await smtp.SendAsync(message);
            //disconnect from gradify email account connection
            await smtp.DisconnectAsync(true);  
            

        }



        //Send Email temporary password Method
        public async Task SendEmailTempPasswordAsync(string ToEmail, int Temp_Password)
        {
            //Creating email message object
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Gradify", _smtp.Username));
            message.To.Add(MailboxAddress.Parse(ToEmail));
            message.Subject = "Your verification code";

            message.Body = new TextPart("plain")
            {
                Text = $"Your Temporary Password is: {Temp_Password}\n Temp Password expires in 10 minutes."
            };

            //smtp object to send email
            using var smtp = new SmtpClient();
            //connect to gradify email account
            await smtp.ConnectAsync(
                _smtp.Host,
                _smtp.Port,
                SecureSocketOptions.StartTls
            );
            await smtp.AuthenticateAsync(
                _smtp.Username,
                _smtp.Password
            );

            //send email
            await smtp.SendAsync(message);
            //disconnect from gradify email account connection
            await smtp.DisconnectAsync(true);  
            

        }

        //Check if email is in proper email format by making a MailAddress object and check if any exceptions
        //are flagged from the format of the email
        public static bool VerifyEmail(string email)
        {  
            return MailboxAddress.TryParse(email, out _);
        }
        

    }
}
