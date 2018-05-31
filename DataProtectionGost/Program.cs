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

            Console.WriteLine("Введите имя файла без расширения. файл должен иметь формат обычного текстового файла");
            filename = Console.ReadLine();
            Console.WriteLine("Введите ключ шифрования");
            key = Console.ReadLine();

            Crypt crypt = new Crypt(filename);
            crypt.CryptText(key);
        }
    }
}
