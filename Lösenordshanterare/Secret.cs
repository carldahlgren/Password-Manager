using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Security.Cryptography;

namespace Lösenordshanterare
{
    public class Secret
    {

        public void Getsecretkey(string [] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("\r\n" +
                    "Not the right amount of arguments.\r\n" +
                    "Please enter clientpath ONLY" +
                    "\r\n");
                return;
            }
            string clientpath = args[1];
            string loadedsk = File.ReadAllText(clientpath);
            Console.WriteLine(loadedsk);

        }
        public Secret()
        {
        }
    }
}
