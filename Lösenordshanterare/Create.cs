using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Security.Cryptography;

namespace Lösenordshanterare
{

    public class Create
    {

        public void Createcommand(string[] args)
        {
            //Validerar att rätt antal argument
            if (args.Length != 3)
            {
                Console.WriteLine("\r\n" +
                    "Not the right amount of arguments.\r\n" +
                    "Please enter client- and serverpath." +
                    "\r\n");
                return;
            }
            string clientpath = args[1];
            string serverpath = args[2];

            //Användare skriver in MP
            Console.WriteLine("Enter master password");
            string mpw = Console.ReadLine();
            while (string.IsNullOrEmpty(mpw))
            {
                Console.WriteLine("Password cannot be empty.");
                Console.WriteLine("Enter master password");
                mpw = Console.ReadLine();
            }
            //Användare skriver in Secret key
            Console.WriteLine("Enter secret key from client file you seek to recreate");
            string sk = Console.ReadLine();
            while (string.IsNullOrEmpty(sk))
            {
                Console.WriteLine("Secret key cannot be empty.");
                Console.WriteLine("Enter secret key from client file you seek to recreate");
                sk = Console.ReadLine();
            }
            //Om vi dekryptera valvet med angiven SK så kopieras SK till ny clientfil.
            if (Decryptionpossible(serverpath,sk,mpw))
            {
                Createfile client = new Createfile();
                client.file = clientpath;
                client.Filemaker(client.file);
                Console.WriteLine("\r\n" +
                    "Secret key was successfully stored in file " + clientpath);
                File.WriteAllText(clientpath, sk);

            }
            else
            {
                Console.WriteLine("A new client file could not be created." +
                    "\r\n");
            }

        }

        public bool Decryptionpossible(string serverpath,string secretkey,string mpw)
        {
            string encjsonvault = null;
            Get get = new Get();
            try
            {
                 encjsonvault = get.Getjson(serverpath);
            }
            catch 
            {
                Console.WriteLine("\r\n" +
                    "Could not find server file " + serverpath );
                return false;
            }
           
            Dictionary<string, string> serverdict = get.Separator(encjsonvault);

            string iv = get.Getiv(serverdict);
            //Console.WriteLine(iv);

            byte[] vaultbyte = new byte[16];
            vaultbyte = Convert.FromBase64String(serverdict["Vault"]);

            byte[] vkbyte = GetvkWithSK(secretkey, mpw);

            byte[] ivbyte = new byte[16];
            ivbyte = Convert.FromBase64String(iv);

            try
            {
                get.DecryptStringFromBytes_Aes(vaultbyte, vkbyte, ivbyte);
                return true;
            }
            catch
            {
                Console.WriteLine("\r\n" +
                    "Decryption of vault with entered MP & SK failed.");
                return false;
            }
          
            
        }

        public byte[] GetvkWithSK(string secretkey, string mpw)
        {
            try
            {
                byte[] skbyte = new byte[16];
                skbyte = Convert.FromBase64String(secretkey);


                Init init = new Init();
                byte[] vkbyte = init.Createvaultkey(mpw, skbyte);

                return vkbyte;
            }
            catch 
            {
                byte[] error = new byte[16];
                return error;
            }
           

        }
        public Create()
        {
           
        }
    }
}