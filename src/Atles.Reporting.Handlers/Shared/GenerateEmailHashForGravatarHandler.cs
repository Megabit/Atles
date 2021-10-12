﻿using Atles.Reporting.Shared.Queries;
using OpenCqrs.Queries;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Atles.Reporting.Handlers.Shared
{
    public class GenerateEmailHashForGravatarHandler : IQueryHandler<GenerateEmailHashForGravatar, string>
    {
        /// <summary>
        /// https://www.danesparza.net/2010/10/using-gravatar-images-with-c-asp-net/
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<string> Handle(GenerateEmailHashForGravatar query)
        {
            await Task.CompletedTask;

            // Create a new instance of the MD5CryptoServiceProvider object.  
            MD5 md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.  
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(query.Email));

            // Create a new Stringbuilder to collect the bytes  
            // and create a string.  
            StringBuilder sBuilder = new();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string.  
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();  // Return the hexadecimal string. 
        }
    }
}