params
[
	["_ctrl", controlNull, [controlNull]]
];

if (ctrlText _ctrl find "pause_ca.paa" >= 0) then
{
	// Spotify is currently playing
	// Stop
	"ArmaSpotifyController" callExtension "spotify:pause";

	if ((profilenamespace getVariable ['aasp_info_delay', 3]) > 1) then
	{
		_ctrl ctrlSetText "\spotify\ui_f_spotify\data\icons\play_ca.paa";
	};
}
else
{
	// Spotify is not playing
	// Start
	"ArmaSpotifyController" callExtension "spotify:play";

	if ((profilenamespace getVariable ['aasp_info_delay', 3]) > 1) then
	{
		_ctrl ctrlSetText "\spotify\ui_f_spotify\data\icons\pause_ca.paa";
	};
};
