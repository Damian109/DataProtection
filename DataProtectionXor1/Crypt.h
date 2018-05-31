#pragma once

#include <string>

class Crypt {
private:
	std::string _filename;
	//������ ����� ��� ��������/����������
	std::string ReadFile(const std::string& filename);
	//������ �������������� �����
	void WriteFile(const std::string& filename, const std::string& text);
	//��������� ������ ������ �� ������
	char* GetBytes(const std::string& text);
	//����������/ ������������ ������
	std::string Crypto(const char* text, const char* key);
public:
	Crypt(const std::string& filename);
	~Crypt();
	//������� ����������. �� ������ ���������� ������������� ������
	void CryptString(const std::string& key);
	//������� ������������. �� ������ �������������� ������
	void DecryptString(const std::string& key);
};







