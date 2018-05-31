#pragma once

#include <string>

class Crypt {
private:
	std::string _filename;
	//Чтение файла для шифровки/дешифровки
	std::string ReadFile(const std::string& filename);
	//Запись зашифрованного файла
	void WriteFile(const std::string& filename, const std::string& text);
	//Получение массива строк
	std::string* GetMasString(std::string text, const int& keyLength);
	//Обработка ключа согласно шифрованию
	std::string ChangeKey(const std::string& oldKey);
	//Шифрование/ дешифрование текста по двум ключам(введенному и сортированному)
	std::string Crypto(const std::string* text, const std::string& trueKey, const std::string& falseKey);
public:
	Crypt(const std::string& filename);
	~Crypt();
	//Функция шифрования. На выходе получается зашифрованная строка
	void CryptString(const std::string& key);
	//Функция дешифрования. На выходе расшифрованная строка
	void DecryptString(const std::string& key);
};







