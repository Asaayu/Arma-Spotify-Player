class CfgPatches
{
	class ui_f_spotify
	{
		author = "Asaayu";
		name = "ui_f_spotify";
		url = "https://www.arma3.com";
		units[] = {};
		weapons[] = {};
		requiredVersion = 0.1;
		requiredAddons[] = { "A3_Ui_F"};
	};
};

class ctrlStaticBackground;
class ctrlActivePicture;
class ctrlStaticPicture;
class ctrlStaticTitle;
class ctrlStatic;
class ctrlStructuredText;
class ctrlControlsGroupNoHScrollbars;
class ctrlButton;
class ctrlEdit;

// Spotify GUI
#include "spotify.hpp"

// Setup GUI
#include "setup.hpp"

// Premium Required GUI
#include "premium.hpp"
