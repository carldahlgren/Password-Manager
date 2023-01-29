using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text;
using System.Security.Cryptography;


namespace Lösenordshanterare
{

    public class Delete
    {
        public Delete()
        {

        }

        public void Deletecommand(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("\r\n" +
                   "Not the right amount of arguments.\r\n" +
                   "Please enter clientpath, serverpath and prop." +
                   "\r\n");
                return;
            }
            string clientpath = args[1];
            string serverpath = args[2];
            string prop = args[3];
            Console.WriteLine("Password Manager");
            Console.WriteLine("Delete");

            
            Console.WriteLine("Enter Master Password");
            string mpw = Console.ReadLine();
            while (string.IsNullOrEmpty(mpw))
            {
                Console.WriteLine("Password cannot be empty.");
                Console.WriteLine("Enter Master Password");
                mpw = Console.ReadLine();
            }
            
            try
            {
                Deletefromvault(clientpath, serverpath, mpw, prop);
            }
            catch
            {
                Console.WriteLine("Delete was not sucessful.");
            }
                

            

        }
        
        public void Deletefromvault(string clientpath, string serverpath, string mpw, string prop)
        {
            Get get = new Get();
            string sdictstring;
            
            
            sdictstring = get.Getjson(serverpath);
            
            
            

            Dictionary<string, string> sdict = JsonSerializer.Deserialize<Dictionary<string, string>>(sdictstring);
            string iv = get.Getiv(sdict);
            


            string encvault = sdict["Vault"];

            byte[] encvaultbyte = new byte[16];
            encvaultbyte = Convert.FromBase64String(encvault);

            byte[] vkbyte = get.Getvk(clientpath, mpw);

            byte[] ivbyte = new byte[16];
            ivbyte = Convert.FromBase64String(iv);
            string vault = DecryptStringFromBytes_Aes(encvaultbyte, vkbyte, ivbyte);
            Dictionary<string, string> vaultdict = JsonSerializer.Deserialize<Dictionary<string, string>>(vault);

            vaultdict.Remove(prop);

            string vstring = JsonSerializer.Serialize(vaultdict);

            byte[] encvault2 = EncryptStringToBytes_Aes(vstring, vkbyte, ivbyte);

            string updvault = Convert.ToBase64String(encvault2);
            sdict["Vault"] = updvault;
            string vaultjson = JsonSerializer.Serialize(sdict);


            File.WriteAllText(serverpath, vaultjson);
        }

        static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
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

    }
}
    

