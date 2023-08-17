using System.Text;

namespace BankAPI.Services;

public class Encoder
{
    public string Encrypt (string Pwd)
    {
        return Convert.ToBase64String(Encoding.Unicode.GetBytes(Pwd));
    }
    public string Desencrypt (string Pwd)
    {
        return Encoding.Unicode.GetString(Convert.FromBase64String(Pwd));
    }
}