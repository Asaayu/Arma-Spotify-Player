params
[
	["_display",displayNull,[displayNull]]
];

#define LOADED (uinamespace getVariable ['aasp_spotify_preloaded', false])

_display displayAddEventHandler
[
	"KeyDown",
	{
		params ["_display", "_key", "_shift", "_ctrl", "_alt"];

		private _return = switch (_key == 1) do
		{
			case (ctrlShown (_display displayCtrl 50000)):
			{
				// Close the control group then stop ESC from closing the display
				ctrlDelete (_display displayCtrl 50000);
				true
			};
			case (ctrlShown (_display displayCtrl 55000) && {false}):
			{
				(_display displayCtrl 55000) ctrlShow false;
				true
			};
			default
			{
				false
			};
		};
		_return
	}
];

// The very first time loading the extension will run all the serup items
if !LOADED then
{
	"ArmaSpotifyController" callExtension "init_load";
	uinamespace setVariable ['aasp_spotify_preloaded', true];
};

// If the user opens this menu without being authorised, open the authorise menu instead.
if (("ArmaSpotifyController" callExtension "authorised") != "true") exitWith
{
	closeDialog 2;
	createDialog "AASP_setup";
};
if (("ArmaSpotifyController" callExtension "spotify:premium") != "true") exitWith
{
	closeDialog 2;
	createDialog "AASP_premium";
};

private _last_update = "ArmaSpotifyController" callExtension "legal_update";
if ((profileNamespace getVariable ["aasp_legal_update", ""]) != _last_update) exitWith
{
	[] spawn
	{
		["The <a href='https://github.com/Asaayu/Arma-Spotify-Player/blob/main/EULA.md'>EULA</a> and <a href='https://github.com/Asaayu/Arma-Spotify-Player/blob/main/PRIVACY-POLICY.md'>Privacy Policy</a> for Asaayu's Arma Spotify Player has been updated.<br/>By re-authorising your Spotify account you agree to the changes, if you do not want to agree to the changes <a href='https://steamcommunity.com/id/asaayu/'>unsubscribe from this mod on the Steam Workshop</a>", "Important Information", "I Understand", false] call BIS_fnc_guiMessage;

		closeDialog 2;
		createDialog "AASP_setup";
	};
};

// Load playlists
"ArmaSpotifyController" callExtension "spotify:request_playlists";

private _last_window = uinamespace getVariable ["aasp_last_window", "home"];

switch true do
{
	case (_last_window == "recent"):
	{
		call spotify_fnc_open_recent;
	};
	case (_last_window == "liked"):
	{
		call spotify_fnc_open_liked;
	};
	case (_last_window find "playlist:" == 0):
	{
		private _info = _last_window splitString ":";
		if (count _info >= 2) then
		{
			[_info#1] call spotify_fnc_open_playlist;
		}
		else
		{
			call spotify_fnc_open_home;
		};
	};
	case (_last_window find "album:" == 0):
	{
		private _info = _last_window splitString ":";
		if (count _info >= 2) then
		{
			[_info#1] call spotify_fnc_open_album;
		}
		else
		{
			call spotify_fnc_open_home;
		};
	};
	case (_last_window == "home");
	default
	{
		// Open the last menu
		call spotify_fnc_open_home;
	};
};

// Force update info
"ArmaSpotifyController" callExtension "spotify:request_info";
