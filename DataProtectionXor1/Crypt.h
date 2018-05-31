#pragma once

#include <string>

class Crypt {
private:
	std::string _filename;
	//Чтение файла для шифровки/дешифровки
	std::string ReadFile(const std::string& filename);
	//Запись зашифрованного файла
	void WriteFile(const std::string& filename, const std::string& text);
	//Получение списка байтов из строки
	char* GetBytes(const std::string& text);
	//Шифрование/ дешифрование текста
	std::string Crypto(const char* text, const char* key);
public:
	Crypt(const std::string& filename);
	~Crypt();
	//Функция шифрования. На выходе получается зашифрованная строка
	void CryptString(const std::string& key);
	//Функция дешифрования. На выходе расшифрованная строка
	void DecryptString(const std::string& key);
};







