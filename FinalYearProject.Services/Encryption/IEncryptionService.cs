using System;
using System.Collections.Generic;
using System.Text;

namespace FinalYearProject.Services.Encryption;

public interface IEncryptionService
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
}