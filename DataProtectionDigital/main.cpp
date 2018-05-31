#include "Crypto.h"
#include <iostream>

#pragma comment(lib, "ws2_32.lib")
#pragma comment(lib, "Crypt32.lib")

int main()
{
	setlocale(LC_ALL, "rus");
	std::cout << "�������� ��������: 1 - ������������ �������� �������, 2 - ��������� �������� �������" << std::endl;
	int r = 0;
	Crypto C;
	std::cin >> r;
	if (r == 1){
		std::string t;
		std::cout << "������� �������� �����" << std::endl;
		std::cin >> t;
		C.CreateDigital((char*)t.c_str());
	}
	else {
		if (C.CompareDigitals()) {
			std::cout << "�������� ������� �����";
		}
		else {
			std::cout << "�������� ������� ��� ���� ���� ��������������";
		}
	}
	std::cin >> r;
}