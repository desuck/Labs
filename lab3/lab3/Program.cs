using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Lab3
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("=== 3. Хешування пароля із додаванням солі (SaltedHash) ===\n");

            string password = "MySuperPassword123";

            byte[] salt1 = SaltedHash.GenerateSalt();
            byte[] salt2 = SaltedHash.GenerateSalt();

            string hash1 = SaltedHash.HashPasswordWithSalt(password, salt1);
            string hash2 = SaltedHash.HashPasswordWithSalt(password, salt2);

            Console.WriteLine($"Пароль: {password}\n");
            Console.WriteLine($"Сіль 1 (Base64): {Convert.ToBase64String(salt1)}");
            Console.WriteLine($"Хеш 1  (SHA256): {hash1}\n");

            Console.WriteLine($"Сіль 2 (Base64): {Convert.ToBase64String(salt2)}");
            Console.WriteLine($"Хеш 2  (SHA256): {hash2}\n");

            Console.WriteLine("Висновок 3: Навіть для однакових паролів додавання унікальної випадкової солі дає абсолютно різні хеш-коди. Це повністю захищає від атак за допомогою райдужних таблиць (Rainbow Tables).\n");



            Console.WriteLine("=== 4. PBKDF2: Залежність часу обчислення від кількості ітерацій ===\n");

            int variant = 4;
            int startIterations = variant * 10000;
            int step = 50000;

            PBKDF2 pbkdf2 = new PBKDF2(HashAlgorithmName.SHA256);
            byte[] pbkdf2Salt = pbkdf2.GenerateSalt();

            Console.WriteLine($"Алгоритм: {pbkdf2.AlgorithmName}, Варіант: {variant}");
            Console.WriteLine($"Згенерована сіль: {Convert.ToBase64String(pbkdf2Salt)}\n");
            Console.WriteLine("Ітерації\tЧас (мс)\tХеш (перші 16 байт)");
            Console.WriteLine("------------------------------------------------------------");

            Stopwatch sw = new Stopwatch();

            for (int i = 0; i < 10; i++)
            {
                int currentIterations = startIterations + (i * step);

                sw.Restart();
                byte[] derivedKey = pbkdf2.HashPassword(password, pbkdf2Salt, currentIterations);
                sw.Stop();

                string shortHash = Convert.ToHexString(derivedKey, 0, 16);
                Console.WriteLine($"{currentIterations,-12}\t{sw.ElapsedMilliseconds,-10}\t{shortHash}...");
            }

            Console.WriteLine("\nВисновок 4: Час обчислення хешу зростає лінійно пропорційно кількості ітерацій. Збільшення ітерацій (key stretching) штучно уповільнює роботу алгоритму, роблячи перебір паролів (Brute-force) занадто дорогим і повільним для зловмисника.");

            Console.ReadLine();
        }
    }

    public class SaltedHash
    {
        public static byte[] GenerateSalt(int size = 32)
        {
            byte[] salt = new byte[size];
            RandomNumberGenerator.Fill(salt);
            return salt;
        }

        public static string HashPasswordWithSalt(string password, byte[] salt)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            byte[] combinedBytes = new byte[passwordBytes.Length + salt.Length];
            Buffer.BlockCopy(passwordBytes, 0, combinedBytes, 0, passwordBytes.Length);
            Buffer.BlockCopy(salt, 0, combinedBytes, passwordBytes.Length, salt.Length);

            byte[] hashBytes = SHA256.HashData(combinedBytes);
            return Convert.ToHexString(hashBytes);
        }
    }

    public class PBKDF2
    {
        public HashAlgorithmName AlgorithmName { get; private set; }

        public PBKDF2(HashAlgorithmName algorithmName)
        {
            AlgorithmName = algorithmName;
        }

        public byte[] GenerateSalt(int size = 32)
        {
            byte[] salt = new byte[size];
            RandomNumberGenerator.Fill(salt);
            return salt;
        }

        public byte[] HashPassword(string password, byte[] salt, int iterations, int outputBytes = 32)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            return Rfc2898DeriveBytes.Pbkdf2(passwordBytes, salt, iterations, AlgorithmName, outputBytes);
        }
    }
}