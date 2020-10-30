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
}

// Called when the extension is first loaded
void RVExtensionVersion(char *output, int outputSize)
{
	// Reduce output by 1 to avoid accidental overflow
	outputSize--;
	char *information = "ASJ authentication DLL - Version 0.0.1";
	strncpy_s(output, outputSize, information, _TRUNCATE);
}

// STRING callExtension STRING
void RVExtension(char *output, int outputSize, const char *function)
{
	// Reduce output by 1 to avoid accidental overflow
	outputSize--;

	if (atoi(function) == 1)
	{
		char *text = "Arma Spotify Jukebox DLL - Version 0.0.1 - Please view https://github.com/Asaayu/Arma-Spotify-Jukebox for more information including important legal information.";
		strncpy_s(output, outputSize, text, _TRUNCATE);
		return;
	};
}