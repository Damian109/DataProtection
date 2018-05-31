#pragma once
#include <string>
#include <openssl/evp.h>
#include <openssl/sha.h> //Алгоритм SHA-2
#include <openssl/rsa.h> // Алгоритм RSA
#include <openssl/pem.h> // Для работы с файлами ключей

class Crypto {
public:
	//Конструктор
	Crypto();

	//Формирование цифровой подписи
	void CreateDigital(char* key);

	//Проверка цифровой подписи
	bool CompareDigitals();
private:
	//Генерация хеш-функции
	void GetHash();

	//Генерация открытого и закрытого ключа по ключевому слову
	void GenKeys(char* key);
};



