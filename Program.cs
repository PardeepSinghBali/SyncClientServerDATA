using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace synchProject
{
    internal class Program
    {
        
        static void Main(string[] args)
        {
            
            Console.WriteLine("welcome");
            try
            {
                ReadAccount();
            }
            catch (Exception ex)
            {
                MakeAccount();
            }
            
            finally
            { 
             watcher();
            }
           
        }


        //This security key should be very complex and Random for encrypting the text. This playing vital role in encrypting the text.
        private const string SecurityKey = "ComplexKeyHere_12121";

        //This method is used to convert the plain text to Encrypted/Un-Readable Text format.
        public static string EncryptPlainTextToCipherText(string PlainText)
        {
            // Getting the bytes of Input String.
            byte[] toEncryptedArray = UTF8Encoding.UTF8.GetBytes(PlainText);

            MD5CryptoServiceProvider objMD5CryptoService = new MD5CryptoServiceProvider();
            //Gettting the bytes from the Security Key and Passing it to compute the Corresponding Hash Value.
            byte[] securityKeyArray = objMD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(SecurityKey));
            //De-allocatinng the memory after doing the Job.
            objMD5CryptoService.Clear();

            var objTripleDESCryptoService = new TripleDESCryptoServiceProvider();
            //Assigning the Security key to the TripleDES Service Provider.
            objTripleDESCryptoService.Key = securityKeyArray;
            //Mode of the Crypto service is Electronic Code Book.
            objTripleDESCryptoService.Mode = CipherMode.ECB;
            //Padding Mode is PKCS7 if there is any extra byte is added.
            objTripleDESCryptoService.Padding = PaddingMode.PKCS7;


            var objCrytpoTransform = objTripleDESCryptoService.CreateEncryptor();
            //Transform the bytes array to resultArray
            byte[] resultArray = objCrytpoTransform.TransformFinalBlock(toEncryptedArray, 0, toEncryptedArray.Length);
            objTripleDESCryptoService.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        //This method is used to convert the Encrypted/Un-Readable Text back to readable  format.
        public static string DecryptCipherTextToPlainText(string CipherText)
        {
            byte[] toEncryptArray = Convert.FromBase64String(CipherText);
            MD5CryptoServiceProvider objMD5CryptoService = new MD5CryptoServiceProvider();

            //Gettting the bytes from the Security Key and Passing it to compute the Corresponding Hash Value.
            byte[] securityKeyArray = objMD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(SecurityKey));
            objMD5CryptoService.Clear();

            var objTripleDESCryptoService = new TripleDESCryptoServiceProvider();
            //Assigning the Security key to the TripleDES Service Provider.
            objTripleDESCryptoService.Key = securityKeyArray;
            //Mode of the Crypto service is Electronic Code Book.
            objTripleDESCryptoService.Mode = CipherMode.ECB;
            //Padding Mode is PKCS7 if there is any extra byte is added.
            objTripleDESCryptoService.Padding = PaddingMode.PKCS7;

            var objCrytpoTransform = objTripleDESCryptoService.CreateDecryptor();
            //Transform the bytes array to resultArray
            byte[] resultArray = objCrytpoTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            objTripleDESCryptoService.Clear();

            //Convert and return the decrypted data/byte into string format.
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
    


       
        static void MakeAccount()
        {
            Console.WriteLine("Input your username");
            string Username = Console.ReadLine();
            Username = EncryptPlainTextToCipherText(Username);
            Console.WriteLine("Input your password");
            string password = Console.ReadLine();
            password = EncryptPlainTextToCipherText(password);
            string folder =  Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\isms\\";
            string fileName = "AccessMethod.txt";
            string fullPath = folder + fileName;
            string[] lines = { Username, password };
            File.WriteAllLines(fullPath, lines);
            string readText = File.ReadAllText(fullPath);
            Console.WriteLine(readText);


        }
        public static string ReadAccount()
        {

            // Read each line of the file into a string array. Each element
            // of the array is one line of the file.
            string pathval = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\isms\AccessMethod.txt";
            //string[] lines = System.IO.File.ReadAllLines(@"C:\Users\pardeep.singh\Desktop\testShare\AccessMethod.txt");
            string[] lines = System.IO.File.ReadAllLines(pathval);
            string information,Password;
           
          
            
            
                information = DecryptCipherTextToPlainText(lines[0].ToString());
                Password= DecryptCipherTextToPlainText(lines[1].ToString());
                // Use a tab to indent each line of the file.
                // Console.WriteLine("\t" + information);
            

            return information + ';' + Password;
            
        }
        [PermissionSet(SecurityAction.Demand,Name ="FullTrust")] 
        private static void watcher()
        {
            using (FileSystemWatcher watcher = new FileSystemWatcher())
            {
                Program pg = new Program();
                //Program.GlobalCounter = 1;
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                watcher.Path = path+ @"\isms";
                
                watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.LastAccess | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size;
                watcher.Filter = "*.*";
                
                watcher.Changed += OnChanged;
                
                
                watcher.Created += OnChanged;
                watcher.Deleted += OnChanged;
                //watcher.Renamed += OnRenamed;
                watcher.EnableRaisingEvents = true;
                
                Console.ReadKey();
                  
            } 
        }
        
        public static void OnChanged(object source, FileSystemEventArgs e)
        {

            string credentials = ReadAccount();

            Console.WriteLine("File synchronization InProgress...");
            Process p = new Process();

           //p.StartInfo.Arguments = string.Format("/ C NET USE \\192.168.0.9\\IPC$ / user:pardeep.singh EvTz_! *@~1");
            //p.StartInfo.Arguments = string.Format("/C Robocopy /S /e {0} {1}", Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\testShare", "\\192.168.0.9\\dcg_one\\" + System.Security.Principal.WindowsIdentity.GetCurrent().Name);
            //p.StartInfo.Arguments = string.Format("/ C NET USE \\192.168.0.9\\IPC$ / user:pardeep.singh EvTz_! *@~1");
            //p.StartInfo.Arguments = string.Format("/C Robocopy /S /e {0} {1}", Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\testShare", "\\\\192.168.0.9\\dcg_one\\pardeep.singh");
            //p.StartInfo.FileName = "CMD.EXE";
            //p.StartInfo.CreateNoWindow = true;
            //p.StartInfo.UseShellExecute = false;
            //p.Start();
            //p.WaitForExit();
            string[] values = credentials.Split(';');
           
                values[0] = values[0].Trim();
            values[1]= values[1].Trim();

            Process cmd = new Process();

            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.UseShellExecute = false;

            cmd.Start();
            //double timeSpan = DateTime.Now.Subtract(File.GetLastWriteTime(e.FullPath)).TotalSeconds;
            //if (timeSpan > 10)
                if(e.ChangeType== System.IO.WatcherChangeTypes.Created)
                notificationCorner(1);
            using (StreamWriter sw = cmd.StandardInput)
            {
                if (sw.BaseStream.CanWrite)
                {
                    sw.WriteLine("NET USE \\\\192.168.0.9\\IPC$ /user:"+ values[0]+" "+values[1]);
                    //sw.WriteLine("NET USE \\\\192.168.0.9\\IPC$ /user:pardeep.singh EvTz_!*@~1");
                    sw.WriteLine("robocopy /s /e %USERPROFILE%\\Desktop\\isms \\\\192.168.0.9\\dcg_one\\%username%");
                    
                }
            }
            
            Console.WriteLine("File synchronization Completed");
            //notificationCorner(2);


        }
        static void notificationCorner(int stage)
        {
            NotifyIcon trayIcon = new NotifyIcon();
            trayIcon.Text = "TestApp";

            trayIcon.Icon = new System.Drawing.Icon(Path.GetFullPath("iconni.ico"));

            trayIcon.Visible = true;
            
            trayIcon.BalloonTipText =stage==1? "Data Synchronizaiton InProgress":"Data Synchronization Completed";
            trayIcon.ShowBalloonTip(100);
        }
        public static void OnRenamed(object source, RenamedEventArgs e)
        {
            //onRenaming of file this portion will execute so write code accordingly: Pardeep
        }
    }
}
