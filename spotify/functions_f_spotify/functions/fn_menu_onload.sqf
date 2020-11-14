params
[
	["_display",displayNull,[displayNull]]
];

#define LOADED (uinamespace getVariable ['aasp_spotify_preloaded', false])

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
