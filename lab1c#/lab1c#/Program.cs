using System;
using System.Security.Cryptography;

namespace Lab1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("1. Однаковий Seed (100):");
            Random rnd1 = new Random(100);
            Random rnd2 = new Random(100);

            Console.Write("Ряд 1: ");
            for (int i = 0; i < 10; i++)
            {
                Console.Write(rnd1.Next(0, 100) + " ");
            }
            Console.WriteLine();

            Console.Write("Ряд 2: ");
            for (int i = 0; i < 10; i++)
            {
                Console.Write(rnd2.Next(0, 100) + " ");
            }
            Console.WriteLine("\nВисновок: при однаковому Seed числа повністью співпадають.\n");


            Console.WriteLine("2. Різний Seed (100 и 200):");
            Random rnd3 = new Random(100);
            Random rnd4 = new Random(200);

            Console.Write("Ряд 3 (Seed 100): ");
            for (int i = 0; i < 10; i++)
            {
                Console.Write(rnd3.Next(0, 100) + " ");
            }
            Console.WriteLine();

            Console.Write("Ряд 4 (Seed 200): ");
            for (int i = 0; i < 10; i++)
            {
                Console.Write(rnd4.Next(0, 100) + " ");
            }
            Console.WriteLine("\nВисновок: при різних Seed генеруються різні числа.\n");


            Console.WriteLine("3. RandomNumberGenerator (криптостійкий):");
            byte[] bytes = new byte[10];
            RandomNumberGenerator.Fill(bytes);

            Console.Write("Випадкові байти: ");
            for (int i = 0; i < bytes.Length; i++)
            {
                Console.Write(bytes[i] + " ");
            }
            Console.WriteLine();

            Console.ReadLine();
        }
    }
}