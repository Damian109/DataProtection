#include "Crypto.h"
#include <fstream>
#include <conio.h>
#include <io.h>
#include <fcntl.h>
#include <stdlib.h>
#include <iomanip> 

#pragma comment(lib, "ws2_32.lib")
#pragma comment(lib, "Crypt32.lib")

//Конструктор
Crypto::Crypto() {

}

//Генерация открытого и закрытого ключа по ключевому слову
void Crypto::GenKeys(char* key) {
	//Структура для хранения ключей
	RSA* rsa = NULL;

	//Длина ключа
	unsigned long bits = 1024;

	//Файлы для хранения ключей
	FILE* filePrivate = fopen("private.key", "wb");
	FILE* filePublic = fopen("public.key", "wb");
	/*
	*Генерация ключей
	*/

	//Контекст шифрования
	const EVP_CIPHER *cipher = NULL;

	//Генерация
	rsa = RSA_generate_key(bits, RSA_F4, NULL, NULL);

	//Формирование контекста шифрования
	cipher = EVP_get_cipherbyname("fb-ofb");

	//Получаем из структуры RSA открытый ключ и сохраняем в файле
	PEM_write_RSAPublicKey(filePublic, rsa);

	//Получаем из структуры RSA закрытый ключ с помощью ключа и сохраняем в файл
	PEM_write_RSAPrivateKey(filePrivate, rsa, cipher, NULL, 0, NULL, key);

	//Освобождаем выделенную память
	RSA_free(rsa);
	fclose(filePrivate);
	fclose(filePublic);
}

//Генерация хеш-функции
void Crypto::GetHash() {
	uint8_t* result = new uint8_t[32];

	//Устанавливаем русскую локаль
	setlocale(LC_ALL, "rus");

	//Открываем файл
	std::ifstream fileIn("text.txt", std::ios::in);

	// Узнаем размер файла, чтобы по нему сформировать массив
	fileIn.seekg(0, std::ios::end);
	int length = fileIn.tellg();

	//Объявляем массив байт, для хранения текста сообщения
	unsigned char* text = new unsigned char[length];

	//Возвращаем указатель к началу
	fileIn.seekg(0, std::ios::beg);

	//Считываем данные в массив байт
	fileIn.read((char*)text, length);
	fileIn.close();

	//Вычисляем хеш-функцию SHA-2
	SHA256((const uint8_t*)text, length, result);

	//Записываем хеш-функцию в файл
	FILE* fout = fopen("hash.hash", "w");
	for (int i = 0; i < 32; i++)
		fprintf(fout, "%02x", result[i]);
	fclose(fout);
	//Освобождаем выделенную память
	delete text;
}

//Формирование цифровой подписи
void Crypto::CreateDigital(char* key) {
	//Генерация открытого и закрытого ключа по ключевому слову
	GenKeys(key);

	//Генерация хеш-функции
	GetHash();

	//Структура для хранения ключей
	RSA* rsa = NULL;

	//Файлы для хранения ключей
	FILE* filePrivate = fopen("private.key", "rb");

	//Открываем ключевой файл и считываем закрытый ключ
	OpenSSL_add_all_algorithms();
	rsa = PEM_read_RSAPrivateKey(filePrivate, NULL, NULL, key);


	//Определение размера ключа
	int keySize = RSA_size(rsa);
	unsigned char *ptext = (unsigned char *)malloc(keySize);
	unsigned char *ctext = (unsigned char *)malloc(keySize);

	//Создаем цифровую подпись
	int digital = _open("digital.txt", O_CREAT | O_TRUNC | O_RDWR, 0600);
	int hash = _open("hash.hash", O_RDWR);

	//Записываем результаты в файл
	int inlen = _read(hash, ptext, keySize);
	int outlen = RSA_private_encrypt(inlen, ptext, ctext, rsa, RSA_PKCS1_PADDING);
	_write(digital, ctext, outlen);
}

//Проверка цифровой подписи
bool Crypto::CompareDigitals() {
	//Генерация хеш-функции
	GetHash();

	//Структура для хранения ключей
	RSA* rsa = NULL;

	//Файлы для хранения ключей
	FILE* filePublic = fopen("public.key", "rb");

	//Открываем ключевой файл и считываем закрытый ключ
	OpenSSL_add_all_algorithms();
	rsa = PEM_read_RSAPublicKey(filePublic, NULL, NULL, NULL);
	fclose(filePublic);

	//Определение размера ключа
	int keySize = RSA_size(rsa);
	unsigned char *ptext = (unsigned char *)malloc(keySize);
	unsigned char *ctext = (unsigned char *)malloc(keySize);

	//Создаем цифровую подпись
	int digital = _open("hash2.hash", O_CREAT | O_TRUNC | O_RDWR, 0600);
	int hash = _open("digital.txt", O_RDWR);

	//Записываем результаты в файл
	int inlen = _read(hash, ptext, keySize);
	int outlen = RSA_public_decrypt(inlen, ptext, ctext, rsa, RSA_PKCS1_PADDING);
	_write(digital, ctext, outlen);

	//Сравнение полученных хеш-функций
	char* hash1 = new char[32];
	char* hash2 = new char[32];

	std::ifstream Hash1("hash.hash", std::ios::in);
	std::ifstream Hash2("hash2.hash", std::ios::in);

	Hash1 >> hash1;
	Hash2 >> hash2;

	Hash1.close();
	Hash2.close();

	for(int i = 0; i < 32; i++)
		if(hash1[i] != hash2[i])
			return false;
	return true;
}