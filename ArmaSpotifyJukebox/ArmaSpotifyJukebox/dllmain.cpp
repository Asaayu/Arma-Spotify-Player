// Header
#include "pch.h"

// Fix issue with std/stoi function
#define _GLIBCXX_USE_C99 1

// Includes
#include <string>
#include <functional>
#include <iostream>

extern "C"
{
	// Called when extension is loaded
	__declspec(dllexport) void __stdcall RVExtensionVersion(char *output, int outputSize);

	// STRING callExtension STRING
	__declspec(dllexport) void __stdcall RVExtension(char *output, int outputSize, const char *function);

	// STRING callExtension ARRAY
	__declspec(dllexport) int __stdcall RVExtensionArgs(char *output, int outputSize, const char *function, const char **argv, int argc);
}

// Called when the extension is first loaded
void RVExtensionVersion(char *output, int outputSize)
{
	// Reduce output by 1 to avoid accidental overflow
	outputSize--;
	char *information = "ASJ DLL - Version 0.0.1";
	strncpy_s(output, outputSize, information, _TRUNCATE);
}

// STRING callExtension STRING
void RVExtension(char *output, int outputSize, const char *function)
{
	// Reduce output by 1 to avoid accidental overflow
	outputSize--;

	if (function == "info")
	{
		char *text = "Arma Spotify Jukebox DLL - Version 0.0.1 - Please view https://github.com/Asaayu/Arma-Spotify-Jukebox for more information including important legal information.";
		strncpy_s(output, outputSize, text, _TRUNCATE);
		return;
	};
}

// STRING callExtension ARRAY
int RVExtensionArgs(char *output, int outputSize, const char *function, const char **argv, int argc)
{
	// Reduce output by 1 to avoid accidental overflow
	outputSize--;

	int index = 0;
	int i;
	for (i = 0; i < argc && index < outputSize; i++)
	{
		index += strncpy_s(output + index, outputSize - 1 - index, argv[i], _TRUNCATE);
	}
	return 0;
}