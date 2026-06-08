using System;
using System.Collections.Generic;
using System.Text;

namespace FinalYearProject.Services.Encryption;

public interface IEncryptionService
{
    (string payload, string aesKey) Encrypt(string message);
    string Decrypt(string cipherJson);
}
