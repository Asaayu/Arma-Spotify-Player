#include "\a3\editor_f\Data\Scripts\dikCodes.h"

[
	"Asaayu's Arma Spotify Player",
	"aasp_show_gui",
	"Open Spotify GUI",
	{true},
	{
		call spotify_fnc_gui;
	},
	[DIK_GRAVE, [false, true, false]]
] call CBA_fnc_addKeybind;
