using MailKit.Net.Smtp;
using MimeKit;

namespace BankConsole;

public static class EmailService{

    public static void SendMail(){
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress ("Saul Perez", "perezsaul165@gmail.com"));
        message.To.Add(new MailboxAddress ("Saul Perez", "perezsaul165@gmail.com"));
        message.Subject = "BankConsole: Lista de usuarios";

        message.Body = new TextPart("plain"){
            Text = GetEmailText()
        };

        string filePath = Storage.GetFilePath();

        var attachment = new MimePart(){
            Content = new MimeContent (File.OpenRead (filePath)),
            ContentDisposition = new ContentDisposition (ContentDisposition.Attachment),
            ContentTransferEncoding = ContentEncoding.Base64,
            FileName = Path.GetFileName (filePath)
        };

        var multipart = new Multipart ("mixed");
        multipart.Add (message.Body);
        multipart.Add (attachment);

        message.Body = multipart;

        using (var client = new SmtpClient()){
            client.Connect("smtp.gmail.com", 587, false);
            client.Authenticate("perezsaul165@gmail.com", "ehpyypxphpvyqqgu");
            client.Send(message);
            client.Disconnect(true);
        }
    }

    private static string GetEmailText(){
        var listUsers = Storage.GetAllUsers();

        var newUsers = listUsers.Where(user => user.GetRegisterDate().Date.Equals(DateTime.Today)).ToList();
        
        if(listUsers.Count == 0)
            return "No hay usuarios.";

        string emailText = "Usuarios agregados hoy:\n";

        foreach(User user in newUsers)
            emailText += "\t+" + user.ShowData() + "\n";
        
        emailText += "\nTodos los usuarios agregados:\n";

        foreach(User user in listUsers)
            emailText += "\t+" + user.ShowData() + "\n";

        return emailText;
    }
}