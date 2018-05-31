#include <fstream>
#include <list>
#include <strstream>
#include <string>
#include <iterator>
#include <iomanip>
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

char* Crypt::GetBytes(const std::string& text) {
	char* bytes = new char[text.length()];
	std::ostrstream stream(bytes, text.length());
	for (int i = 0; i < text.length(); i++)
		stream << std::hex << std::setw(2) << int(text[i]);
	return bytes;
}


//Шифрование/ дешифрование текста по двум ключам(введенному и сортированному)
std::string Crypt::Crypto(const char* text, const char* key) {
	std::string result = "";
	int count = 0;

	if (strlen(text) < strlen(key))
		text += '\0';

	for (int i = 0; i < strlen(text); i++)
	{
		result += text[i] ^ key[count++];
		if (count >= strlen(key))
			count = 0;
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
	std::string tmp = Crypto(GetBytes(uncryptString), GetBytes(key));
	WriteFile(_filename + ".dec", tmp);
	return;
}

//Функция дешифрования. На выходе расшифрованная строка
void Crypt::DecryptString(const std::string& key) {
	std::string cryptString = ReadFile(_filename + ".dec");
	std::string tmp = Crypto(GetBytes(cryptString), GetBytes(key));
	WriteFile(_filename + ".undec", tmp);
	return;
}