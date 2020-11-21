params
[
	["_display",displayNull,[displayNull]]
];
disableSerialization;

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
				["button", [_display, _display displayCtrl 50000, true]] call spotify_fnc_get_devices;
				true
			};
			case (ctrlShown (_display displayCtrl 55000)):
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
	private _master = displayParent _display;
	_display closeDisplay 2;
	_master createDisplay "AASP_setup";
};
if (("ArmaSpotifyController" callExtension "spotify:premium") != "true") exitWith
{
	private _master = displayParent _display;
	_display closeDisplay 2;
	_master createDisplay "AASP_premium";
};

// Force update info
"ArmaSpotifyController" callExtension "spotify:request_info";
