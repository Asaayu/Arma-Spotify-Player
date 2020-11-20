"ArmaSpotifyController" callExtension "preinit";

addMissionEventHandler ["ExtensionCallback",
{
	params ["_name", "_function", "_data"];
	// This eventhandler is specifically for AASP
	if (_name != "ArmaSpotifyController") exitWith {};

	// Switch cases for all the different callbacks
	switch (toLower _function) do
	{
		case ("setvariable"):
		{
			private _array_data = parseSimpleArray _data;
			_array_data params ["_namespace","_variable","_value","_global"];
			(call compile _namespace) setVariable [_variable, _value, _global];
		};
		case ("ctrlsettext"):
		{
			private _data = _data splitString "|";
			(uiNamespace getVariable [_data#1, controlNull]) ctrlSetText (_data#0);
		};
		case ("spotify_fnc_update_like"):
		{
			(parseSimpleArray _data) params ["_result","_id"];
			[_result, false, _id] call spotify_fnc_like;
		};
		case ("spotify_fnc_set_playback"):
		{
			(parseSimpleArray _data) call spotify_fnc_set_playback;
		};
		case ("spotify_fnc_get_devices"):
		{
			(parseSimpleArray _data) call spotify_fnc_get_devices;
		};
		case ("spotify_fnc_update_display"):
		{
			(parseSimpleArray _data) call spotify_fnc_update_display;
		};
		case ("spotify_fnc_update_song_info"):
		{
			(parseSimpleArray _data) call spotify_fnc_update_song_info;
		};
		case ("device_required"):
		{
			private _display = uinamespace getVariable ["aasp_spotify_display", displaynull];
			private _no_device_ctrl = _display displayCtrl 1306;
			_no_device_ctrl ctrlShow true;
		};
		default
		{
			"ArmaSpotifyController" callExtension format["error:ExtensionCallback EVH ran into undefined function (%1)",_function];
		};
	};
}];

[
	"itemAdd",
	[
		"aasp_refresh_token_loop",
		{
			// Every X seconds send a request to refresh users token, will only send a request if enough time has passed since last token refresh
			"ArmaSpotifyController" callExtension "spotify:refresh_token";
		},
		(5*60)
	]
] call BIS_fnc_loop;
