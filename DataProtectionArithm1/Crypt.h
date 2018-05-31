#pragma once

#include <string>

class Crypt {
private:
	std::string _filename;
	//������ ����� ��� ��������/����������
	std::string ReadFile(const std::string& filename);
	//������ �������������� �����
	void WriteFile(const std::string& filename, const std::string& text);
	//��������� ������� �����
	std::string* GetMasString(std::string text, const int& keyLength);
	//��������� ����� �������� ����������
	std::string ChangeKey(const std::string& oldKey);
	//����������/ ������������ ������ �� ���� ������(���������� � ��������������)
	std::string Crypto(const std::string* text, const std::string& trueKey, const std::string& falseKey);
public:
	Crypt(const std::string& filename);
	~Crypt();
	//������� ����������. �� ������ ���������� ������������� ������
	void CryptString(const std::string& key);
	//������� ������������. �� ������ �������������� ������
	void DecryptString(const std::string& key);
};







