using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zashita_lab5
{
    internal sealed class Crypt
    {
        //Размер блока
        private const int BLOCK_SIZE = 64;

        //Название файла без расширения
        private readonly string FILE_NAME;

        public Crypt(string file_name)
        {
            FILE_NAME = file_name;
        }

        //Бинарное чтение файла
        private char[] ReadFile(string file_ext)
        {
            List<char> chars = new List<char>();
            using (BinaryReader reader = new BinaryReader(File.Open(FILE_NAME + file_ext, FileMode.Open)))
            {
                //Чтение до конца файла
                while(reader.PeekChar() > -1)
                    chars.Add(reader.ReadChar());
            }
            return chars.ToArray();
        }

        //Бинарная запись файла
        private void WriteFile(string file_ext, char[] chars)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(FILE_NAME + file_ext, FileMode.OpenOrCreate)))
            {
                writer.Write(chars);
            }
        }

        //Разбиваем текст на блоки строк
        private char[][] GetTextBlocks(char[] chars)
        {
            //Блоки должны быть одинаковой длины, поэтому при необходимости дополняем текст
            List<char> tmpChars = chars.ToList();
            while (tmpChars.Count % BLOCK_SIZE != 0)
                tmpChars.Add('0');

            //На выходе должен получиться массив значений
            chars = tmpChars.ToArray();
            char[][] result = new char[chars.Length / BLOCK_SIZE][];
            for (int i = 0; i < result.Length; i++)
                result[i] = new char[BLOCK_SIZE];

            //Заполняем массив
            int iter = 0;

            for(int i = 0; i < result.Length; i++)
            {
                for (int j = 0; j < BLOCK_SIZE; j++)
                    result[i][j] = chars[iter++];
                if (iter > chars.Length)
                    break;
            }
            return result;
        }

        //Определяем сдвиг ключа, используя текущий раунд
        private string GetNewKey(string key, int raund)
        {
            //Сдвигаться ключ будет на степень 2-ки, которая определяется раундом
            string result = "";
            foreach(var elem in key)
                result += elem * (int)Math.Pow(2, raund);
            return result;
        }

        //Шифрование одного блока
        private char[] CryptBlock(char[] block, string key, bool Reverse = false, bool notReverse = false)
        {
            //Определяем блоки r, l
            char[] blockL = block.Take(BLOCK_SIZE / 2).ToArray();
            char[] blockR = block.Skip(BLOCK_SIZE / 2).ToArray();

            //Меняем блоки местами при первом проходе дешифрования
            if (Reverse)
            {
                char[] tmp = blockL;
                blockL = blockR;
                blockR = tmp;
            }

            char[] temp = new char[BLOCK_SIZE / 2];
            int iter = 0;
            //Заполняем блоки используя функцию xor по ключу, а также xor с правой частью
            for (int i = 0; i < blockL.Length; i++)
            {
                temp[i] = (char)(blockR[i] ^ (blockL[i] ^ key[iter++]));
                if (iter >= key.Length)
                    iter = 0;
            }  

            //На последнем этапе расшифровки не меняем левый и правый блок
            if (notReverse)
                return blockL.Concat(temp).ToArray();
            return temp.Concat(blockL).ToArray();
        }

        //Функция шифрования текста
        public void CryptText(string key, int raund, bool decrypt = false)
        {
            //Если включено дешифрование, начинаем с последнего раунда и уменьшаем
            if (decrypt)
            {
                char[] chars = ReadFile(".enc");
                //Получаем блоки из текста
                char[][] blocks = GetTextBlocks(chars);

                //Выполняем по числу раундов
                for (int i = raund - 1; i >= 0; i--)
                {
                    for (int j = 0; j < blocks.Length; j++)
                    {
                        //Меняет блоки местами перед шифровкой,.. не меняет блоки местами
                        bool p = false, r = false;
                        if (i == raund - 1)
                            p = true;
                        if (i == 0)
                            r = true;

                        blocks[j] = CryptBlock(blocks[j], GetNewKey(key, i), p, r);
                    }
                }

                //Получаем результат
                char[] result = new char[blocks.Length * BLOCK_SIZE];
                int iter = 0;

                for (int i = 0; i < blocks.Length; i++)
                    for (int j = 0; j < BLOCK_SIZE; j++)
                        result[iter++] = blocks[i][j];

                WriteFile(".dec",result);
            }
            else
            {
                char[] chars = ReadFile(".txt");
                //Получаем блоки из текста
                char[][] blocks = GetTextBlocks(chars);

                //Выполняем по числу раундов
                for (int i = 0; i < raund; i++)
                {
                    for (int j = 0; j < blocks.Length; j++)
                        blocks[j] = CryptBlock(blocks[j], GetNewKey(key, i));
                }

                //Получаем результат
                char[] result = new char[blocks.Length * BLOCK_SIZE];
                int iter = 0;

                for (int i = 0; i < blocks.Length; i++)
                    for (int j = 0; j < BLOCK_SIZE; j++)
                        result[iter++] = blocks[i][j];

                WriteFile(".enc", result);
            }
        }
    }
}

    