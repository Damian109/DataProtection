#include "Crypto.h"
#include <fstream>
#include <conio.h>
#include <io.h>
#include <fcntl.h>
#include <stdlib.h>
#include <iomanip> 

#pragma comment(lib, "ws2_32.lib")
#pragma comment(lib, "Crypt32.lib")

//�����������
Crypto::Crypto() {

}

//��������� ��������� � ��������� ����� �� ��������� �����
void Crypto::GenKeys(char* key) {
	//��������� ��� �������� ������
	RSA* rsa = NULL;

	//����� �����
	unsigned long bits = 1024;

	//����� ��� �������� ������
	FILE* filePrivate = fopen("private.key", "wb");
	FILE* filePublic = fopen("public.key", "wb");
	/*
	*��������� ������
	*/

	//�������� ����������
	const EVP_CIPHER *cipher = NULL;

	//���������
	rsa = RSA_generate_key(bits, RSA_F4, NULL, NULL);

	//������������ ��������� ����������
	cipher = EVP_get_cipherbyname("fb-ofb");

	//�������� �� ��������� RSA �������� ���� � ��������� � �����
	PEM_write_RSAPublicKey(filePublic, rsa);

	//�������� �� ��������� RSA �������� ���� � ������� ����� � ��������� � ����
	PEM_write_RSAPrivateKey(filePrivate, rsa, cipher, NULL, 0, NULL, key);

	//����������� ���������� ������
	RSA_free(rsa);
	fclose(filePrivate);
	fclose(filePublic);
}

//��������� ���-�������
void Crypto::GetHash() {
	uint8_t* result = new uint8_t[32];

	//������������� ������� ������
	setlocale(LC_ALL, "rus");

	//��������� ����
	std::ifstream fileIn("text.txt", std::ios::in);

	// ������ ������ �����, ����� �� ���� ������������ ������
	fileIn.seekg(0, std::ios::end);
	int length = fileIn.tellg();

	//��������� ������ ����, ��� �������� ������ ���������
	unsigned char* text = new unsigned char[length];

	//���������� ��������� � ������
	fileIn.seekg(0, std::ios::beg);

	//��������� ������ � ������ ����
	fileIn.read((char*)text, length);
	fileIn.close();

	//��������� ���-������� SHA-2
	SHA256((const uint8_t*)text, length, result);

	//���������� ���-������� � ����
	FILE* fout = fopen("hash.hash", "w");
	for (int i = 0; i < 32; i++)
		fprintf(fout, "%02x", result[i]);
	fclose(fout);
	//����������� ���������� ������
	delete text;
}

//������������ �������� �������
void Crypto::CreateDigital(char* key) {
	//��������� ��������� � ��������� ����� �� ��������� �����
	GenKeys(key);

	//��������� ���-�������
	GetHash();

	//��������� ��� �������� ������
	RSA* rsa = NULL;

	//����� ��� �������� ������
	FILE* filePrivate = fopen("private.key", "rb");

	//��������� �������� ���� � ��������� �������� ����
	OpenSSL_add_all_algorithms();
	rsa = PEM_read_RSAPrivateKey(filePrivate, NULL, NULL, key);


	//����������� ������� �����
	int keySize = RSA_size(rsa);
	unsigned char *ptext = (unsigned char *)malloc(keySize);
	unsigned char *ctext = (unsigned char *)malloc(keySize);

	//������� �������� �������
	int digital = _open("digital.txt", O_CREAT | O_TRUNC | O_RDWR, 0600);
	int hash = _open("hash.hash", O_RDWR);

	//���������� ���������� � ����
	int inlen = _read(hash, ptext, keySize);
	int outlen = RSA_private_encrypt(inlen, ptext, ctext, rsa, RSA_PKCS1_PADDING);
	_write(digital, ctext, outlen);
}

//�������� �������� �������
bool Crypto::CompareDigitals() {
	//��������� ���-�������
	GetHash();

	//��������� ��� �������� ������
	RSA* rsa = NULL;

	//����� ��� �������� ������
	FILE* filePublic = fopen("public.key", "rb");

	//��������� �������� ���� � ��������� �������� ����
	OpenSSL_add_all_algorithms();
	rsa = PEM_read_RSAPublicKey(filePublic, NULL, NULL, NULL);
	fclose(filePublic);

	//����������� ������� �����
	int keySize = RSA_size(rsa);
	unsigned char *ptext = (unsigned char *)malloc(keySize);
	unsigned char *ctext = (unsigned char *)malloc(keySize);

	//������� �������� �������
	int digital = _open("hash2.hash", O_CREAT | O_TRUNC | O_RDWR, 0600);
	int hash = _open("digital.txt", O_RDWR);

	//���������� ���������� � ����
	int inlen = _read(hash, ptext, keySize);
	int outlen = RSA_public_decrypt(inlen, ptext, ctext, rsa, RSA_PKCS1_PADDING);
	_write(digital, ctext, outlen);

	//��������� ���������� ���-�������
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