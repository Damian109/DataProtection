#pragma once
#include <string>
#include <openssl/evp.h>
#include <openssl/sha.h> //�������� SHA-2
#include <openssl/rsa.h> // �������� RSA
#include <openssl/pem.h> // ��� ������ � ������� ������

class Crypto {
public:
	//�����������
	Crypto();

	//������������ �������� �������
	void CreateDigital(char* key);

	//�������� �������� �������
	bool CompareDigitals();
private:
	//��������� ���-�������
	void GetHash();

	//��������� ��������� � ��������� ����� �� ��������� �����
	void GenKeys(char* key);
};



