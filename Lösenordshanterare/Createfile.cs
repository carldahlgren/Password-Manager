using System;
using System.IO;

namespace Lösenordshanterare
{

    public class Createfile
    {
        public string file;
        public void Filemaker(string file)
        {

            try
            {
                // Check if file already exists. If yes, deletion.     
                if (File.Exists(file))
                {
                    File.Delete(file);
                }

                // Creates new file    
                using (FileStream fs = File.Create(file))
                {

                }

            }
            //can remove try catch, not necessary 
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }

        }
        public Createfile()
        {
        }
    }
}
