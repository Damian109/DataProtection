#include "sha256.h"
#include <iostream>

int main()
{
	sha256 sha;
	std::string str = ".";
	std::cout << sha.sha256_get(str);
	int t;
	std::cin >> t;
    return 0;
}

