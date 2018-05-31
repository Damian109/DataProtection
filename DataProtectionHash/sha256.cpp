#define _CRT_SECURE_NO_WARNINGS
#include "sha256.h"

sha256::sha256() {
	Init();
}

void Block_to_word(const unsigned char* block, unsigned int* num) {
	*(num) = ((unsigned int) *((block)+3)) |
		((unsigned int) *((block)+2) << 8) |
		((unsigned int) *((block)+1) << 16) |
		((unsigned int) *((block)+0) << 24);
}

void Word_to_block(unsigned int num, unsigned char* block) {
	*((block)+3) = (unsigned char)num;
	*((block)+2) = (unsigned char)(num >> 8);
	*((block)+1) = (unsigned char)(num >> 16);
	*((block)+0) = (unsigned char)(num >> 24);
}

unsigned int RotateR(unsigned int x, unsigned int n) {
	unsigned int x_n = x;
	return ((x_n >> n) | (x_n << ((sizeof(x_n) << 3) - n)));
}

std::string sha256::sha256_get(const std::string message) {
	Execute((unsigned char*)message.c_str(), message.length());

	unsigned int block_count = (1 + ((BLOCK_SIZE - 9) < _lenght % BLOCK_SIZE));
	unsigned int b_lenght = (_total_lenght + _lenght) << 3;
	unsigned int pLenght = block_count << 6;
	memset(_block + _lenght, 0, pLenght - _lenght);
	_block[_lenght] = 0x80;
	Word_to_block(b_lenght, _block + pLenght - 4);
	Transform(_block, block_count);

	//Формируем финальную строку
	unsigned char hash[HASH_SIZE];
	memset(hash, 0, HASH_SIZE);
	for (int i = 0; i < 8; i++)
		Word_to_block(_h[i], &hash[i << 2]);

	char buf[2 * HASH_SIZE + 1];
	buf[2 * HASH_SIZE] = 0;
	for (int i = 0; i < HASH_SIZE; i++)
		sprintf(buf + i * 2, "%02x", hash[i]);
	return std::string(buf);
}

void sha256::Init() {
	_h[0] = 0x6a09e667;
	_h[1] = 0xbb67ae85;
	_h[2] = 0x3c6ef372;
	_h[3] = 0xa54ff53a;
	_h[4] = 0x510e527f;
	_h[5] = 0x9b05688c;
	_h[6] = 0x1f83d9ab;
	_h[7] = 0x5be0cd19;
	_lenght = 0;
	_total_lenght = 0;
}

void sha256::Transform(const unsigned char* message, unsigned int block_count) {
	unsigned int w[64], wv[8];
	for (int i = 0; i < (int)block_count; i++) {
		const unsigned char* sub_block = message + (i << 6);
		//Разбить подблок на 16 частей длиной 8 байт
		for (int j = 0; j < 16; j++) {
			Block_to_word(&sub_block[j << 2], &w[j]);
		}

		//Сгенерировать дополнительные 48 слов
		for (int j = 16; j < 64; j++) {
			unsigned int s0 = RotateR(w[j - 2], 17) ^ RotateR(w[j - 2], 19) ^ (w[j - 2] >> 10);
			unsigned int s1 = RotateR(w[j - 15], 7) ^ RotateR(w[j - 15], 18) ^ (w[j - 15] >> 3);
			w[j] = s0 + w[j - 7] + s1 + w[j - 16];
		}

		//Формируем дополнительные переменные по сохраненным
		for (int j = 0; j < 8; j++)
			wv[j] = _h[j];

		//Основной цикл
		for (int j = 0; j < 64; j++) {
			unsigned int s0 = RotateR(wv[4], 6) ^ RotateR(wv[4], 11) ^ RotateR(wv[4], 25);
			unsigned int t1 = wv[7] + s0 + ((wv[4] & wv[5]) ^ (~wv[4] & wv[6])) + sha256_const[j] + w[j];

			unsigned int s1 = RotateR(wv[0], 2) ^ RotateR(wv[0], 13) ^ RotateR(wv[0], 22);
			unsigned int t2 = s1 + ((wv[0] & wv[1]) ^ (wv[0] & wv[2]) ^ (wv[1] & wv[2]));

			wv[7] = wv[6];
			wv[6] = wv[5];
			wv[5] = wv[4];
			wv[4] = wv[3] + t1;
			wv[3] = wv[2];
			wv[2] = wv[1];
			wv[1] = wv[0];
			wv[0] = t1 + t2;
		}
		for (int j = 0; j < 8; j++) {
			_h[j] += wv[j];
		}
	}
}

void sha256::Execute(const unsigned char* message, unsigned int lenght) {
	//Предварительная обработка
	unsigned int tmp_lenght = BLOCK_SIZE - _lenght;
	unsigned int new_tmp_lenght = lenght < tmp_lenght ? lenght : tmp_lenght;
	memcpy(&_block[_lenght], message, new_tmp_lenght);
	if (_lenght + lenght < BLOCK_SIZE) {
		_lenght += lenght;
		return;
	}

	//Вычисляем количество блоков
	unsigned int new_lenght = lenght - new_tmp_lenght;
	unsigned int block_count = new_lenght / BLOCK_SIZE;

	//Записываем сообщение + длину
	const unsigned char* new_message = message + new_tmp_lenght;

	//Обрабатываем сообщение в основном цикле
	Transform(_block, 1);
	Transform(new_message, block_count);
	new_tmp_lenght = new_lenght % BLOCK_SIZE;
	memcpy(_block, &new_message[block_count << 6], new_tmp_lenght);
	_lenght = new_tmp_lenght;
	_total_lenght += (block_count + 1) << 6;
}