using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;

namespace GradifyApi.Service
{
    public class RefreshTokenHashService
    {
        //Turn a refreshToken to a hash by SHA256 standard, allowing to check refreshToken credentials already hashed if they match easily
        public static string GetRefreshHash(string refreshToken)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
            return Convert.ToHexString(bytes);
        }
        
    }
}
