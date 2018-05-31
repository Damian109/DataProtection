#include <fstream>
#include <list>
#include <iterator>
#include "Crypt.h"

//Чтение файла для шифровки/дешифровки
std::string Crypt::ReadFile(const std::string& filename) {
	setlocale(LC_ALL, "rus");
	std::ifstream fin(filename);
	std::string str = "";
	char c;
	while (fin.get(c))
		str += c;
	fin.close();
	return str;
}

//Запись зашифрованного файла
void Crypt::WriteFile(const std::string& filename, const std::string& text) {
	std::ofstream fout(filename);
	fout << text;
	fout.close();
	return;
}

//Получение массива строк
std::string* Crypt::GetMasString(std::string text, const int& keyLength) {
	while (text.length() % keyLength != 0)
		text += " ";
	std::string* strings = new std::string[keyLength];

	int t = 0;

	for (int i = 0; i < keyLength; i++)
		for (int j = 0; j < text.length() / keyLength; j++)
			strings[i] += text[t++];
	return strings;
}

//Обработка ключа согласно шифрованию
std::string Crypt::ChangeKey(const std::string& oldKey) {
	std::list<char> newKeyList;
	for (int i = 0; i < oldKey.length(); i++)
		newKeyList.push_back(oldKey[i]);
	newKeyList.sort();

	std::string newKey;
	for (std::list<char>::const_iterator iterator = newKeyList.begin(); iterator != newKeyList.end(); iterator++)
		newKey += *iterator;
	return newKey;
}

//Шифрование/ дешифрование текста по двум ключам(введенному и сортированному)
std::string Crypt::Crypto(const std::string* text, const std::string& trueKey, const std::string& falseKey) {
	std::string result = "";
	int count;

	for (int i = 0; i < trueKey.length(); i++) {
		count = 0;
		for (int j = 0; j < i; j++)
			if (trueKey[j] == trueKey[i])
				count++;
		for (int j = 0; j < falseKey.length(); j++)
			if (trueKey[i] == falseKey[j]) {
				if (count > 0)
					count--;
				else {
					result += text[j];
					break;
				}
			}
	}
	return result;
}

Crypt::Crypt(const std::string& filename) {
	_filename = filename;
}

Crypt::~Crypt() {
}

//Функция шифрования. На выходе получается зашифрованная строка
void Crypt::CryptString(const std::string& key) {
	std::string uncryptString = ReadFile(_filename + ".txt");
	std::string* masStrings = GetMasString(uncryptString, key.length());
	std::string newKey = ChangeKey(key);
	std::string tmp = Crypto(masStrings, newKey, key);
	WriteFile(_filename + ".dec", tmp);
	return;
}

//Функция дешифрования. На выходе расшифрованная строка
void Crypt::DecryptString(const std::string& key) {
	std::string cryptString = ReadFile(_filename + ".dec");
	std::string* masStrings = GetMasString(cryptString, key.length());
	std::string newKey = ChangeKey(key);
	std::string tmp = Crypto(masStrings, key, newKey);
	WriteFile(_filename + ".undec", tmp);
	return;
}