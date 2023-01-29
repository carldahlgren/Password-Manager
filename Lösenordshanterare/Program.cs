using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Security.Cryptography;

namespace Lösenordshanterare
{
    class Program
    {
        static void Main(string[] args)
        {
            
            string choice = args[0];

            
            switch(choice)
            {
                case "init":
                    Init init = new Init();
                    init.Initcommand(args);
                    
                    break;

                case "create":
                    Create create = new Create();
                    create.Createcommand(args);
                    break;

                case "get":
                    Get get = new Get();
                    get.Getcommand(args);
                    break;

                case "set":
                    Set set = new Set();
                    set.Setcommand(args);
                    break;

                    
                case "delete":
                    Delete delete = new Delete();
                    delete.Deletecommand(args);
                    break;
                    

                case "secret":
                    Secret secret = new Secret();
                    secret.Getsecretkey(args);
                    break;
               

                default:

                Console.WriteLine("Only commands init, create, get, set, delete, secret, exit applicable");

                break;



            }
            

            

        }

       




    }
   


}

/*




 * 
 */
