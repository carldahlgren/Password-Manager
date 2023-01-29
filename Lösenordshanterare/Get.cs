using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text;
using System.Security.Cryptography;


namespace Lösenordshanterare
{
    public class Get
    {

        public void Getcommand(string [] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Not the right amount of arguments.\r\n" +
                    "Please enter clientpath, serverpath (and prop if desired).");
                return;
            }
            Console.WriteLine("Password Manager");
            Console.WriteLine("Get");

            string clientpath = args[1];
            string serverpath = args[2];
            string prop;

            Console.WriteLine("Input Master Password");
            string mpw = Console.ReadLine();
            while (string.IsNullOrEmpty(mpw))
            {
                Console.WriteLine("Password cannot be empty.");
                Console.WriteLine("Enter Master Password");
                mpw = Console.ReadLine();
            }

            if (args.Length==3)
            {
                prop = "empty";
            }
            else
            {
                prop = args[3];
            }

            bool quit = false;
            Dictionary<string, string> vaultdict = new Dictionary<string, string>();
            try
            {
                vaultdict = Getvault(clientpath, serverpath, mpw);
            }
            catch
            {
                Console.WriteLine("Wrong Password or wrong client- and/or serverpath.");
                quit = true;
                
            }

            if (quit==true)
            {
                return;
            }



            if (prop == "empty")
            {
                List<string> keys = new List<string>(vaultdict.Keys);
                Console.WriteLine("KEYS:");
                foreach (var key in keys)
                {
                    Console.WriteLine(key);
                }
                
            }
            else
            {
                try
                {
                    Console.WriteLine("Password for key '" + prop + "':");
                    Console.WriteLine(vaultdict[prop]);
                }
                catch
                {
                    Console.WriteLine("Key " + prop + " was not found in vault.");
                }
                
            }
            
        }

        public Dictionary<string,string> Getvault(string clientpath, string serverpath,string mpw)
        {

            string encjsonvault = Getjson(serverpath);
            Dictionary<string, string> serverdict = Separator(encjsonvault);

            string iv = Getiv(serverdict);
            //Console.WriteLine(iv);

            byte[] vaultbyte = new byte[16];
            vaultbyte = Convert.FromBase64String(serverdict["Vault"]);

            byte[] vkbyte = Getvk(clientpath,mpw);

            byte[] ivbyte = new byte[16];
            ivbyte = Convert.FromBase64String(iv);
            string vault = DecryptStringFromBytes_Aes(vaultbyte, vkbyte, ivbyte);
            Dictionary<string, string> vaultdict = JsonSerializer.Deserialize<Dictionary<string, string>>(vault);
            return vaultdict;

        }

        public string Getjson(string filepath)
        {
                string encjsonvault = File.ReadAllText(filepath);
                return encjsonvault;
            
        }

        
        public Dictionary<string, string> Separator(string encjsonvault)
        {
            Dictionary<string, string> serverdict = JsonSerializer.Deserialize<Dictionary<string, string>>(encjsonvault);
            
            return serverdict;


        }
        
        public string Getiv(Dictionary<string, string> serverdict)
        {
            string iv = serverdict["IV"];
            return iv;
        }

        public byte [] Getvk(string clientpath, string mpw)
        {
            string secretkey = Getjson(clientpath);
            
            byte[] skbyte = new byte[16];
            skbyte = Convert.FromBase64String(secretkey);
            

            Init init = new Init();
            byte [] vkbyte = init.Createvaultkey(mpw, skbyte);

            return vkbyte;
        }
        

        public string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }


    }
}




