using System.Text;

namespace OnlineSalesTool.Service
{
    public static class Extentions
    {
        public static byte[] ConvertToBytes(this string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }
        public static string ConvertToString(this byte[] arr)
        {
            return Encoding.UTF8.GetString(arr);
        }
    }
}
