using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text;
using System.Security.Cryptography;

namespace Lösenordshanterare
{
    public class Init
    {

        public void Initcommand(string[] args)
        {
            if (args.Length!=3)
            {
                Console.WriteLine("Not the right amount of arguments.\r\n" +
                    "Please enter client- and serverpath.");
                return;
            }

            string clientpath = args[1];
            string serverpath = args[2];
            //Basic UI and pwd input
            Console.WriteLine("Password Manager");
            Console.WriteLine("INIT");
            Console.WriteLine("Create master password");
            
            string mpwd = Console.ReadLine();
            while (string.IsNullOrEmpty(mpwd))
            {
                Console.WriteLine("Password cannot be empty.");
                Console.WriteLine("Enter Master Password");
                mpwd = Console.ReadLine();
            }
            
            //Filer skapas
            Createfiles(clientpath, serverpath);


            //Skapa IV 
            GenerateRandomString IV = new GenerateRandomString();
            byte[] ivbyte = IV.RNG();
            string ivstring = Convert.ToBase64String(ivbyte);


            //Skapa Secret Key
            GenerateRandomString Secretkey = new GenerateRandomString();
            byte[] skbyte = Secretkey.RNG();
            string skstring = Convert.ToBase64String(skbyte);

            //Skapa Valv och serialisera
            Dictionary<string, string> valv = Createvault();
            string jsonvalv = JsonSerializer.Serialize(valv);
            

            //Skapa valvnyckel
            byte[] Vkbyte = Createvaultkey(mpwd, skbyte);
            string vkstring = Convert.ToBase64String(Vkbyte);

            //Skapa AES-Object och kryptera valv
            string encjsonvault = Convert.ToBase64String(Encryptvault(jsonvalv, Vkbyte, ivbyte));

            Dictionary<string, string> serverdict = new Dictionary<string, string>();
            serverdict.Add("IV", ivstring);
            serverdict.Add("Vault", encjsonvault);
            
            string vaultjson = JsonSerializer.Serialize(serverdict);
            
            
            //Skriv Vault till till serverfil.
            Writetofile(serverpath, vaultjson);

            //Skriv Secret key till clientfil.
            Writetofile(clientpath, skstring);
            


        }

        static void Createfiles(string clientpath, string serverpath)
        {
            //Check if Client.txt exists, if yes removes and creates new, if no creates Client.txt
            Createfile client = new Createfile();
            client.file = clientpath;
            client.Filemaker(client.file);

            //Check if Server.txt exists, if yes removes and creates new, if no creates Server.txt
            Createfile server = new Createfile();
            server.file = serverpath;
            server.Filemaker(server.file);

        }

        public byte[] Createvaultkey(string mpw, byte[] sk)
        {
            try
            {
                Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(mpw, sk);
                byte[] vaultkey = rfc.GetBytes(16);
                return vaultkey;
            }
            catch 
            {

                Console.WriteLine("\r\n" +
                    "Vault key could not be created..");
                byte[] error = new byte[16];
                return error;
            }
            
            
        }

        static Dictionary<string, string> Createvault()
        {
            Dictionary<string, string> valv = new Dictionary<string, string>();
            valv.Add("testkey", "testvalue");
            return valv;

        }

        public void Writetofile(string path, string input)
        {
            File.WriteAllText(path, input);

        }

        public byte[] Encryptvault(string valv, byte[] vkbyte, byte[] ivbyte)
        {

            byte[] encrypted = EncryptStringToBytes_Aes(valv, vkbyte, ivbyte);

            return encrypted;
        }
       
        static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        
        public Init()
        {
        }




    }
}
