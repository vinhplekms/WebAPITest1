using System.Text;

namespace WebAPITest2.Helpers
{
    public class EncyptAndDecrypt
    {
        private static readonly string Key = "abcd@#4123ajsdfk";

        public static string Encrypt(string keyword)
        {
            if (string.IsNullOrEmpty(keyword)) return "";
            keyword += Key;
            var result = Encoding.UTF8.GetBytes(keyword);
            return Convert.ToBase64String(result);
        }

        public static string Decrypt(string base64EncodeData)
        {
            if (string.IsNullOrEmpty(base64EncodeData)) return "";
            var base64EncodeBytes = Convert.FromBase64String(base64EncodeData);
            var result = Encoding.UTF8.GetString(base64EncodeBytes);
            result = result.Substring(0, result.Length - Key.Length);
            return result;
        }
    }
}
