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
		case ("ctrlsettext_playing"):
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
		"aasp_master_loop",
		{
			private _delay = profilenamespace getVariable ['aasp_info_delay', 3];
			if !(_delay isEqualType 1) then { _delay = 3; };
			_delay = _delay max 1;
			private _display = uiNamespace getVariable ["aasp_spotify_display", displayNull];
			private _no_device = _display displayCtrl 1306;
			if (!isNull _display && {!ctrlShown _no_device}) then
			{
				private _play = _display displayCtrl 1000;
				private _seek = _display displayCtrl 1100;

				private _playing = (ctrlText _play) find "pause_ca.paa" > -1;
				if _playing then
				{
					private _slider_range = sliderRange _seek;
					private _new_position = 1 + sliderPosition _seek;

					// Every X seconds or when the song 'ends' request new data from spotify
					private _loop_interation = missionNamespace getVariable ["aasp_master_loop_iteration", 1];
					if (_loop_interation >= _delay || _new_position > _slider_range#1) then
					{
						missionNamespace setVariable ["aasp_master_loop_iteration", 1];

						// Request update
						"ArmaSpotifyController" callExtension "spotify:request_info";
					}
					else
					{
						// Update seek bar
						[_new_position, _slider_range#1, true] call spotify_fnc_set_playback;

						// Just increment the loop iteration
						missionNamespace setVariable ["aasp_master_loop_iteration", _loop_interation + 1];
					};
				}
				else
				{
					// If nothing is playing send a request every 5 seconds to check if playback has started
					private _loop_interation = missionNamespace getVariable ["aasp_master_loop_iteration", 1];
					if (_loop_interation >= _delay) then
					{
						missionNamespace setVariable ["aasp_master_loop_iteration", 1];

						// Request update
						"ArmaSpotifyController" callExtension "spotify:request_info";
					}
					else
					{
						// Just increment the loop iteration
						missionNamespace setVariable ["aasp_master_loop_iteration", _loop_interation + 1];
					};
				};
			};
		},
		1
	]
] call BIS_fnc_loop;

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
