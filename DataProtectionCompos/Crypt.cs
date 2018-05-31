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
            StreamReader reader = new StreamReader(filename, Encoding.Unicode);
            string str = reader.ReadToEnd();
            reader.Close();
            return str;
        }

        //Запись зашифрованного файла
        private void WriteFile(string filename, string text)
        {
            StreamWriter writer = new StreamWriter(filename, false, Encoding.Unicode, 255);
            writer.Write(text);
            writer.Close();
        }

        //*****************************************************************************************************//
        //**********************Функционал для выполнения перестановки*****************************************//
        //*****************************************************************************************************//
        //Получение массива строк
        /*private string[] GetMasString(string text, int keyLength)
        {
            while (text.Length % keyLength != 0)
                text += " ";
            string[] strings = new string[keyLength];

            int t = 0;
            int r = 0;

            while (t < text.Length)
            {
                strings[r++] += text[t++];
                if (r >= keyLength)
                    r = 0;
            }
            return strings;
        }*/

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
        private string Shuffle(string text, string trueKey, string falseKey)
        {
            string result = "";
            int count;

            int textc = 0;

            while (text.Length % trueKey.Length != 0)
                text += " ";

            while(textc < text.Length)
            {
                string temp = text.Substring(textc, trueKey.Length);
                textc += trueKey.Length;

                for (int i = 0; i < falseKey.Length; i++)
                {
                    for (int j = 0; j < trueKey.Length; j++)
                        if (falseKey[i] == trueKey[j])
                        {
                                result += temp[j];
                        }
                }
            }
            return result;
        }
        //*****************************************************************************************************//
        //**********************Функционал для выполнения замены***********************************************//
        //*****************************************************************************************************//
        //Шифрование/ дешифрование текста по шифру цезаря
        private const int MINCHAR = 1072;
        private const int MAXCHAR = 1103;
        private string Shift(string text, int key)
        {
            char[] array = text.ToLower().ToArray();

            for (int i = 0; i < array.Length; i++)
                if (array[i] >= MINCHAR && array[i] <= MAXCHAR)
                {
                    array[i] = (char)(MINCHAR + (array[i] - MINCHAR + key) % 32);
                    if (array[i] < MINCHAR)
                        array[i] = (char)(MAXCHAR - (MINCHAR - array[i]) + 1);
                }

            return new string(array);
        }
        //*****************************************************************************************************//
        //**********************Необходимый открытый функционал************************************************//
        //*****************************************************************************************************//
        //Функция шифрования. На выходе получается зашифрованная строка
        public string Crypt(string key, int count)
        {
            string uncryptString = ReadFile(_filename + ".txt");

            int keyForShift = int.Parse(GetKeyFromString(key));
            string keyForShuffle = GetKeyFromString(key, true);
            string newKey = ChangeKey(keyForShuffle);

            for (int i = 0; i < count; i++)
            {
                string tmp = Shift(uncryptString.ToLower(), keyForShift);
                uncryptString = Shuffle(tmp, newKey, keyForShuffle);
            }
            WriteFile(_filename + ".dec", uncryptString);
            return uncryptString;
        }


        //Функция дешифрования. На выходе расшифрованная строка
        public string Decrypt(string key, int count)
        {
            string decryptString = ReadFile(_filename + ".dec");

            int keyForShift = int.Parse(GetKeyFromString(key));
            string keyForShuffle = GetKeyFromString(key, true);
            string newKey = ChangeKey(keyForShuffle);

            for (int i = 0; i < count; i++)
            {
                string tmp = Shuffle(decryptString, keyForShuffle, newKey);
                decryptString = Shift(tmp, -keyForShift);
            }
            return decryptString;
        }

        //Функция получения ключа из строки(если б = true, то второго ключа)
        private string GetKeyFromString(string key, bool b = false)
        {
            string[] newKey = key.Split(',');
            if (b)
                return newKey[1];
            return newKey[0];
        }
    }
}