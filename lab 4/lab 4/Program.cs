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

            while (true)
            {
                Console.WriteLine("1 - Зашифрувати файл (у *.dat)");
                Console.WriteLine("2 - Розшифрувати файл (*.dat)");
                Console.WriteLine("0 - Вихід");
                Console.Write("\nОберіть дію: ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                if (choice == "1")
                {
                    Console.Write("Введіть шлях до файлу для шифрування: ");
                    string inputPath = Console.ReadLine()?.Trim('"');

                    if (!File.Exists(inputPath))
                    {
                        Console.WriteLine("Помилка: файл не знайдено!\n");
                        continue;
                    }

                    Console.Write("Придумайте та введіть ключ (пароль): ");
                    string password = Console.ReadLine();

                    string outputPath = Path.ChangeExtension(inputPath, ".dat");

                    EncryptFile(inputPath, outputPath, password);
                    Console.WriteLine($"Успішно зашифровано! Створено файл: {outputPath}\n");
                }
                else if (choice == "2")
                {
                    Console.Write("Введіть шлях до зашифрованого файлу (*.dat): ");
                    string inputPath = Console.ReadLine()?.Trim('"');

                    if (!File.Exists(inputPath))
                    {
                        Console.WriteLine("Помилка: файл не знайдено!\n");
                        continue;
                    }

                    Console.Write("Введіть ключ (пароль) для розшифрування: ");
                    string password = Console.ReadLine();

                    string outputPath = Path.Combine(
                        Path.GetDirectoryName(inputPath),
                        Path.GetFileNameWithoutExtension(inputPath) + "_decrypted.txt"
                    );

                    try
                    {
                        DecryptFile(inputPath, outputPath, password);
                        Console.WriteLine($"Успішно розшифровано! Створено файл: {outputPath}\n");
                    }
                    catch (CryptographicException)
                    {
                        Console.WriteLine("Помилка: невірний ключ або файл пошкоджено!\n");
                    }
                }
                else if (choice == "0")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Невідома команда. Натисніть 1, 2 або 0.\n");
                }
            }
        }

        static byte[] GetKeyFromPassword(string password)
        {
            return SHA256.HashData(Encoding.UTF8.GetBytes(password));
        }

        static void EncryptFile(string inputFile, string outputFile, string password)
        {
            byte[] key = GetKeyFromPassword(password);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.GenerateIV();

                using (FileStream fsOut = new FileStream(outputFile, FileMode.Create))
                {
                    fsOut.Write(aes.IV, 0, aes.IV.Length);

                    using (CryptoStream cs = new CryptoStream(fsOut, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    using (FileStream fsIn = new FileStream(inputFile, FileMode.Open))
                    {
                        fsIn.CopyTo(cs);
                    }
                }
            }
        }

        static void DecryptFile(string inputFile, string outputFile, string password)
        {
            byte[] key = GetKeyFromPassword(password);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;

                using (FileStream fsIn = new FileStream(inputFile, FileMode.Open))
                {
                    byte[] iv = new byte[16];
                    fsIn.Read(iv, 0, iv.Length);
                    aes.IV = iv;

                    using (CryptoStream cs = new CryptoStream(fsIn, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    using (FileStream fsOut = new FileStream(outputFile, FileMode.Create))
                    {
                        cs.CopyTo(fsOut);
                    }
                }
            }
        }
    }
}