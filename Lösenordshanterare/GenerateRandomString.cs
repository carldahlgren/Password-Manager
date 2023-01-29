using System;
using System.Security.Cryptography;
namespace Lösenordshanterare
{
    public class GenerateRandomString
    {
        public GenerateRandomString()
        {
        }

        public byte [] RNG()
        {
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] newbyte = new byte[16];
            rng.GetBytes(newbyte);

            
            return newbyte;
        }
    }
}
