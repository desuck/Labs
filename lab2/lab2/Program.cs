using System;
using System.Security.Cryptography;
using System.Text;

namespace Lab2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("=== 1. Порівняння хеш-функцій ===\n");

            string text1 = "Hello World";
            string text2 = "Hello World";
            string text3 = "Hello world";

            PrintHashes(text1);
            PrintHashes(text2);
            PrintHashes(text3);


            Console.WriteLine(" 2. Відновлення 8-значного цифрового пароля \n");

            string targetGuid = "db93c299-351e-79f3-0bfc-a6424576a5a8";
            byte[] targetGuidBytes = new Guid(targetGuid).ToByteArray();

            Console.WriteLine($"Цільовий хеш: {{{targetGuid}}}");
            Console.WriteLine("Йде перебір від 00000000 до 99999999...");

            using (MD5 md5 = MD5.Create())
            {
                for (int i = 0; i <= 99999999; i++)
                {
                    string candidate = i.ToString("D8");
                    byte[] hashBytes = md5.ComputeHash(Encoding.Unicode.GetBytes(candidate));

                    if (hashBytes.AsSpan().SequenceEqual(targetGuidBytes))
                    {
                        Console.WriteLine($"Знайдений пароль: {candidate}");
                        break;
                    }
                }
            }

            Console.WriteLine("3. Перевірка автентичності повідомлення \n");

            string message = "Переказ 5000 грн на рахунок №12345";
            byte[] secretKey = Encoding.UTF8.GetBytes("SuperSecretKey2026");

            string originalHmac = ComputeHmac(message, secretKey);
            Console.WriteLine($"Оригінальне повідомлення: {message}");
            Console.WriteLine($"Згенерований HMAC: {originalHmac}\n");

            string receivedMessageGood = "Переказ 5000 грн на рахунок №12345";
            string checkHmacGood = ComputeHmac(receivedMessageGood, secretKey);
            Console.WriteLine($"Перевірка 1 (оригінал): {(originalHmac == checkHmacGood ? "Автентичність ПІДТВЕРДЖЕНО" : "Помилка!")}");

            string receivedMessageFake = "Переказ 5000 грн на рахунок №99999";
            string checkHmacFake = ComputeHmac(receivedMessageFake, secretKey);
            Console.WriteLine($"Перевірка 2 (підміна):  {(originalHmac == checkHmacFake ? "Автентичність ПІДТВЕРДЖЕНО" : "УВАГА! Повідомлення було змінено або невірний ключ!")}");

            Console.ReadLine();
        }

        // Метод для виведення всіх основних хешів рядка
        static void PrintHashes(string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);

            byte[] md5 = MD5.HashData(bytes);
            byte[] sha1 = SHA1.HashData(bytes);
            byte[] sha256 = SHA256.HashData(bytes);
            byte[] sha512 = SHA512.HashData(bytes);

            Console.WriteLine($"Текст: \"{input}\"");
            Console.WriteLine($"  MD5    ({md5.Length} байт): {Convert.ToHexString(md5)}");
            Console.WriteLine($"  SHA1   ({sha1.Length} байт): {Convert.ToHexString(sha1)}");
            Console.WriteLine($"  SHA256 ({sha256.Length} байт): {Convert.ToHexString(sha256)}");
            Console.WriteLine($"  SHA512 ({sha512.Length} байт): {Convert.ToHexString(sha512)}\n");
        }

        // HMAC-SHA256
        static string ComputeHmac(string message, byte[] key)
        {
            using (HMACSHA256 hmac = new HMACSHA256(key))
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                byte[] hashBytes = hmac.ComputeHash(messageBytes);
                return Convert.ToHexString(hashBytes);
            }
        }
    }
}