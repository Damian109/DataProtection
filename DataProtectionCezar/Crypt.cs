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
        private const int MINCHAR = 1072;
        private const int MAXCHAR = 1103;

        public int CountChars { get; private set; }

        public Cryptograph(string filename)
        {
            _filename = filename;
        }

        //Чтение файла для шифровки/дешифровки
        public string ReadFile(string fileExtension)
        {
            StreamReader reader = new StreamReader(_filename + fileExtension, Encoding.Unicode);
            string str = reader.ReadToEnd().ToLower();
            reader.Close();
            return str;
        }

        //Подсчет символов из диапазона в тексте
        private int CalculateCountChars(string str)
        {
            int length = str.Length;
            int result = 0;

            for (int i = 0; i < length; i++)
                if (str[i] >= MINCHAR && str[i] <= MAXCHAR)
                    result++;
            return result;
        }

        //Функция шифрования. На выходе получается зашифрованная строка
        public string Crypt(string uncryptString, int key) => Crypto(uncryptString, key);

        //Функция дешифрования. На выходе расшифрованная строка
        public string Decrypt(string cryptString, int key) => Crypto(cryptString, -key);

        //Шифрование/ дешифрование текста
        private string Crypto(string text, int key)
        {
            char[] array = text.ToArray();

            for (int i = 0; i < array.Length; i++)
                if (array[i] >= MINCHAR && array[i] <= MAXCHAR)
                {
                    array[i] = (char)(MINCHAR + (array[i] - MINCHAR + key) % 32);
                    if (array[i] < MINCHAR)
                        array[i] = (char)(MAXCHAR - (MINCHAR - array[i]) + 1);
                }

            return new string(array);
        }

        //Получение распределения букв в тексте
        public List<MyChar> GetCharsFromText(string text)
        {
            CountChars = CalculateCountChars(text);
            List<MyChar> dictionary = new List<MyChar>();

            int length = text.Length;
            bool b;

            for(int i = 0; i < length; i++)
                if (text[i] >= MINCHAR && text[i] <= MAXCHAR)
                {
                    b = false;

                    for (int j = 0; j < dictionary.Count; j++)
                        if (dictionary[j].Key == text[i])
                        {
                            dictionary[j].Value++;
                            b = true;
                            break;
                        }

                    if (!b)
                        dictionary.Add(new MyChar(text[i]));
                }

            for (int i = 0; i < dictionary.Count; i++)
                dictionary[i].Value = dictionary[i].Value / CountChars;
            dictionary.Sort();
            return dictionary;
        }

        //Подбор строк, похожих на изначальное значение
        private const string stringChars = "оеаинтслвркдмупягьыбзч";

        //Хранение массива возможных сдвигов
        public int[] sdvig;

        public List<string> FindKey(List<MyChar> chars, string text)
        {
            List<string> result = new List<string>();
            sdvig = new int[30];
            int iter = 0, iterList = 0, iterChars = 0;

            while (iter < 30)
            {
                sdvig[iter++] = FindSdvig(chars[iterList++], stringChars[iterChars]);

                if(iterList >= chars.Count)
                {
                    iterList = 0;
                    iterChars++;
                }
            }

            string temp = "";

            if (text.Length > 30)
                temp = text.Substring(0, 30);
            else
                temp = text;

            foreach (int elem in sdvig)
                result.Add(Crypto(temp, -elem));
            return result;
        }

        private int FindSdvig(MyChar myChar, char nchar)
        {
            if (myChar.Key < nchar)
                return MAXCHAR - nchar + myChar.Key - MINCHAR + 1;
            else
                return myChar.Key - nchar;
        }
    }

    public class MyChar: IComparable
    {
        public char Key { get; set; }
        public double Value { get; set; }

        public MyChar(char key)
        {
            Key = key;
            Value = 1;
        }

        public int CompareTo(object obj)
        {
            MyChar tmp = (MyChar)obj;
            return -Value.CompareTo(tmp.Value);
        }
    }
}