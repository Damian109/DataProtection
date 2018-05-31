using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace zashita_lab1
{
    internal class Cryptograph
    {
        private readonly string _filename;
        public Cryptograph(string filename)
        {
            _filename = filename;
        }

        //Чтение файла для шифровки/дешифровки
        private string ReadFile(string filename)
        {
            StreamReader reader = new StreamReader(filename, Encoding.GetEncoding(1251));
            string str = reader.ReadToEnd();
            reader.Close();
            return str;
        }

        //Запись зашифрованного файла
        private void WriteFile(string filename, string text)
        {
            StreamWriter writer = new StreamWriter(filename, false, Encoding.GetEncoding(1251), 255);
            writer.Write(text);
            writer.Close();
        }

        //Получение списка байтов из строки
        private List<byte> GetBytes(string str)
        {
            byte[] byteString = Encoding.GetEncoding(1251).GetBytes(str);
            List<byte> resultByte = new List<byte>();

            foreach (byte elem in byteString)
                resultByte.Add(elem);

            return resultByte;
        }

        //Функция шифрования. На выходе получается зашифрованная строка
        public string Crypt(string key)
        {
            string uncryptString = ReadFile(_filename + ".txt");
            string tmp = Crypto(GetBytes(uncryptString),GetBytes(key));
            WriteFile(_filename + ".dec", tmp);
            return tmp;
        }
        //Функция дешифрования. На выходе расшифрованная строка
        public string Decrypt(string key)
        {
            string cryptString = ReadFile(_filename + ".dec");
            return Crypto(GetBytes(cryptString), GetBytes(key), true);
        }

        //Шифрование/ дешифрование текста по двум ключам(введенному и сортированному)
        private string Crypto(List<byte> text, List<byte> key, bool des = false)
        {
            if (text.Count < key.Count)
                text.Add(new byte());

            List<byte> result = new List<byte>();

            int countKey = 0;

            foreach (byte elem in text)
            {
                result.Add((byte)(elem ^ key[countKey++]));
                if (countKey >= key.Count)
                    countKey = 0;
            }
            return Encoding.GetEncoding(1251).GetString(result.ToArray());
        }
    }
}