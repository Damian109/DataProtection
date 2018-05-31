#include <iostream>
#include "Crypt.h"

int main() {
	Crypt c = Crypt("text");
	int t;
	std::cout << "1 - crypt, 2 - decrypt";
	std::cin >> t;
	std::string key;
	std::cin >> key;
	if (t == 1)
		c.CryptString(key);
	else
		c.DecryptString(key);
	return 0;
}