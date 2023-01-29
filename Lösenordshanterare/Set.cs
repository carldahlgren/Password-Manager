using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;
using System.Security.Cryptography;
namespace Lösenordshanterare
{
    public class Set
    {
        public void Setcommand(string [] args)
        {
            if (args.Length < 4)
            {
                Console.WriteLine("\r\n" +
                    "Not the right amount of arguments.\r\n" +
                    "Please enter clientpath, serverpath and prop (and '-g' if desired)." +
                    "\r\n");
                return;
            }
            string clientpath = args[1];
            string serverpath = args[2];
            string prop = args[3];

            Console.WriteLine("Password Manager");
            Console.WriteLine("Set");

            Console.WriteLine("Enter master password");
            string mpw = Console.ReadLine();
            while (string.IsNullOrEmpty(mpw))
            {
                Console.WriteLine("Password cannot be empty.");
                Console.WriteLine("Enter Master Password");
                mpw = Console.ReadLine();
            }


            
           

            Get get = new Get();
            Dictionary<string, string> vaultdict = new Dictionary<string, string>();
            try
            {
               vaultdict = get.Getvault(clientpath, serverpath, mpw);
            }
            catch 
            {
                Console.WriteLine("Decryption of vault failed \r\n" +
                    "Wrong Password or wrong client- and/or serverpath. \r\n" +
                    "Password for " + prop + " was not set.");
                return;
            }
            
            Dictionary<string, string> newvaultdict = new Dictionary<string, string>();

            if (args.Length==4)//Om 4 argument är givna (alltså med prop utan -g)
            {
                Console.WriteLine("Enter a password for " + prop);
                string value = Console.ReadLine();
                newvaultdict = Addvaluetovault(value,prop, vaultdict);


            }
            else if (args[4]=="-g"||args[4]=="--generate")
            {
                string value = Randomstring();

                newvaultdict = Addvaluetovault(value, prop, vaultdict);
                Console.WriteLine("Password for " + prop + " was set to " + value) ;
            }

           


            string encjsonserver = get.Getjson(serverpath);
            Dictionary<string, string> serverdict = get.Separator(encjsonserver);

            string iv = get.Getiv(serverdict);
            

            string jsonvault = JsonSerializer.Serialize(newvaultdict);
            

            byte[] vkbyte = get.Getvk(clientpath, mpw);

            byte[] ivbyte = new byte[16];
            ivbyte = Convert.FromBase64String(iv);

            Init init = new Init();
            string encjsondict= Convert.ToBase64String(init.Encryptvault(jsonvault, vkbyte, ivbyte));
            serverdict["Vault"] = encjsondict;
            string serverdictjson = JsonSerializer.Serialize(serverdict);
            
            init.Writetofile(serverpath, serverdictjson);
            

        }
        public Dictionary<string,string> Addvaluetovault(string value,string prop,Dictionary<string,string> vaultdict )
        {
            List<string> keys = new List<string>(vaultdict.Keys);
            if (keys.Contains(prop))
            {
                vaultdict[prop] = value;
            }
            else
            {
                vaultdict.Add(prop, value);
            }
            return vaultdict;
        }
        public string Randomstring()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var chararray = new char[20];
            var random = new Random();

            for (int i = 0; i < chararray.Length; i++)
            {
                chararray[i] = chars[random.Next(chars.Length)];
            }

            return new String(chararray);
        }
        public Set()
        {
           
        }
    }
}
