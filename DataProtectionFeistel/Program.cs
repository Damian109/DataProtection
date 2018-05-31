using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zashita_lab5
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename, key;
            int raund;
            bool deshifr = false;

            Console.WriteLine("Введите имя файла без расширения. файл должен иметь формат обычного текстового файла");
            filename = Console.ReadLine();
            Console.WriteLine("Введите ключ шифрования");
            key = Console.ReadLine();
            Console.WriteLine("Введите количество раундов");
            raund = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Введите 0 - для шифрования, 1 - для дешифрования");
            int t = Convert.ToInt32(Console.ReadLine());
            if (t == 1)
                deshifr = true;

            Crypt crypt = new Crypt(filename);
            crypt.CryptText(key, raund, deshifr);
        }
    }
}
