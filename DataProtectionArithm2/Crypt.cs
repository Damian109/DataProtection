using System;
using System.Linq;
using System.Text;
using System.IO;

namespace zashita_lab1
{
    internal class Cryptograph
    {
        private readonly string _filename;
        public Cryptograph(string filename)
        {
            _filename = filename;
        }
        //Функция шифрования. На выходе получается зашифрованная строка
        public string Crypt(string key)
        {
            string uncryptString = ReadFile(_filename + ".txt");
            string[] masStrings = GetMasString(uncryptString, key.Length);
            string newKey = ChangeKey(key);
            string tmp = Crypto(masStrings, newKey, key);
            WriteFile(_filename + ".dec", tmp);
            return tmp;
        }
        //Функция дешифрования. На выходе расшифрованная строка
        public string Decrypt(string key)
        {
            string cryptString = ReadFile(_filename + ".dec");
            string[] masStrings = GetMasString(cryptString, key.Length);
            string newKey = ChangeKey(key);
            return Crypto(masStrings, key, newKey);
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

        //Получение массива строк
        private string[] GetMasString(string text, int keyLength)
        {
            while (text.Length % keyLength != 0)
                text += " ";
            string[] strings = new string[keyLength];

            int t = 0;

            for (int i = 0; i < keyLength; i++)
                for (int j = 0; j < text.Length / keyLength; j++)
                    strings[i] += text[t++];
            return strings;
        }

        //Обработка ключа согласно шифрованию
        private string ChangeKey(string oldKey)
        {
            char[] newKey = oldKey.ToArray();
            Array.Sort(newKey);
            string str = "";
            foreach (var elem in newKey)
                str += elem;
            return str;
        }

        //Шифрование/ дешифрование текста по двум ключам(введенному и сортированному)
        private string Crypto(string[] text, string trueKey, string falseKey)
        {
            string result = "";
            int count;

            for (int i = 0; i < trueKey.Length; i++)
            {
                count = 0;
                for (int j = 0; j < i; j++)
                    if (trueKey[j] == trueKey[i])
                        count++;
                for (int j = 0; j < falseKey.Length; j++)
                    if (trueKey[i] == falseKey[j])
                    {
                        if (count > 0)
                            count--;
                        else
                        {
                            result += text[j];
                            break;
                        }
                    }
            }
            return result;
        }
    }
}