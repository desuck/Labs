using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Lab4
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            string originalFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.txt");
            File.WriteAllText(originalFile, "Секретний текст для четвертої лабораторної роботи. Перевірка шифрування AES!");

            byte[] key = RandomNumberGenerator.GetBytes(32);
            byte[] iv = RandomNumberGenerator.GetBytes(16);


            Console.WriteLine("=== 1. Шифрування файлу ===\n");

            string encryptedFile = Path.ChangeExtension(originalFile, ".dat");

            EncryptFile(originalFile, encryptedFile, key, iv);

            Console.WriteLine($"Вхідний файл:      {originalFile}");
            Console.WriteLine($"Зашифрований файл: {encryptedFile}");
            Console.WriteLine($"Вміст оригіналу:   {File.ReadAllText(originalFile)}");
            Console.WriteLine($"Вміст *.dat (Hex): {Convert.ToHexString(File.ReadAllBytes(encryptedFile))}\n");


            Console.WriteLine("=== 2. Розшифрування файлу ===\n");

            string decryptedFile = Path.Combine(
                Path.GetDirectoryName(encryptedFile)!,
                Path.GetFileNameWithoutExtension(encryptedFile) + "_decrypted.txt"
            );

            DecryptFile(encryptedFile, decryptedFile, key, iv);

            Console.WriteLine($"Розшифрований файл: {decryptedFile}");
            Console.WriteLine($"Вміст після розшифрування: {File.ReadAllText(decryptedFile)}\n");

            Console.WriteLine("Висновок: Алгоритм AES успішно зашифрував вміст файлу у формат *.dat та повністю відновив вихідні дані при наявності правильного ключа та вектора ініціалізації (IV).");

            Console.ReadLine();
        }

        static void EncryptFile(string inputFile, string outputFile, byte[] key, byte[] iv)
        {
            byte[] dataToEncrypt = File.ReadAllBytes(inputFile);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                using (FileStream fsOut = new FileStream(outputFile, FileMode.Create))
                using (CryptoStream cs = new CryptoStream(fsOut, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(dataToEncrypt, 0, dataToEncrypt.Length);
                }
            }
        }

        static void DecryptFile(string inputFile, string outputFile, byte[] key, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                using (FileStream fsIn = new FileStream(inputFile, FileMode.Open))
                using (CryptoStream cs = new CryptoStream(fsIn, aes.CreateDecryptor(), CryptoStreamMode.Read))
                using (FileStream fsOut = new FileStream(outputFile, FileMode.Create))
                {
                    cs.CopyTo(fsOut);
                }
            }
        }
    }
}