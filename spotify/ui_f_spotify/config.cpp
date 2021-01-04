class CfgPatches
{
	class ui_f_spotify
	{
		authors[]= {"Asaayu"};
		author = "Asaayu";
		name = "Asaayu's Arma Spotify Player - UI";
		url = "https://github.com/Asaayu/Arma-Spotify-Player";
		units[] = {};
		weapons[] = {};
		requiredVersion = 0.1;
		requiredAddons[] = { "A3_Ui_F", "main_f_spotify"};
	};
};

class ctrlStaticBackground;
class ctrlActiveText;
class ctrlActivePicture;
class ctrlActivePictureKeepAspect;
class ctrlStaticPicture;
class ctrlStaticPictureKeepAspect;
class ctrlStaticTitle;
class ctrlStatic;
class ctrlStructuredText;
class ctrlControlsGroup;
class ctrlControlsGroupNoHScrollbars;
class ctrlControlsGroupNoVScrollbars;
class ScrollBar;
class ctrlButton;
class ctrlEdit;
class ctrlXSliderH;
class ctrlListbox;
class ctrlCombo;

#define SW safezoneW
#define SH safezoneH
#define SX safezoneX
#define SY safezoneY

#define HI(VALUE) (VALUE * 1.75 * SH)

#define W(VALUE) (VALUE * SW)
#define H(VALUE) (VALUE * SH)
#define X(VALUE) (SX + VALUE * SW)
#define Y(VALUE) (SY + VALUE * SH)

#define LHX(VALUE) H(0.0625) + (LH * VALUE)
#define LH H(0.02)

// Spotify GUI
#include "spotify.hpp"

// Setup GUI
#include "setup.hpp"

// Premium Required GUI
#include "premium.hpp"

// Album control
#include "windows\album.hpp"

// Playlist control
#include "windows\playlist.hpp"

// Devices control
#include "windows\devices.hpp"

class RscTitles
{
	// Notification GUI
	#include "notification.hpp"
};
