using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CalLCenter_Dataspecs.Security
{
  public interface IMSACCO_AES_Credentials
  {
    string CipherKey { get; }
    string CipherIV { get; }
  }

  public static class APICommunication
  {
    public static string Encrypt(string dataToEncrypt, IMSACCO_AES_Credentials cipherParameters)
    {
      UTF8Encoding utf8 = new UTF8Encoding();
      AesCryptoServiceProvider aes = new AesCryptoServiceProvider {
        Key = utf8.GetBytes(cipherParameters.CipherKey),
        IV = utf8.GetBytes(cipherParameters.CipherIV)
      };

      using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
      {
        MemoryStream ms = new MemoryStream();
        CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        byte[] bytes = utf8.GetBytes(dataToEncrypt);
        cs.Write(bytes, 0, bytes.Length);
        cs.FlushFinalBlock();
        ms.Position = 0;
        bytes = new byte[ms.Length];
        ms.Read(bytes, 0, bytes.Length);
        return Convert.ToBase64String(bytes);
      }
    }
  }

  public class MSACCO_AES : IMSACCO_AES_Credentials
  {
    public string CipherKey { get; private set; }

    public string CipherIV { get; private set; }

    public MSACCO_AES(string key, string iv)
    {
      CipherKey = key;
      CipherIV = iv;
    }
  }
}
