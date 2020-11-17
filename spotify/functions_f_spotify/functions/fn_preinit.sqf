// This function contains all the data that is run at the start of every mission

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
		case ("spotify_fnc_get_devices"):
		{
			(parseSimpleArray _data) call spotify_fnc_get_devices;
		};
		case ("spotify_fnc_update_display"):
		{
			(parseSimpleArray _data) call spotify_fnc_update_display;
		};
		default
		{
			"ArmaSpotifyController" callExtension format["error:ExtensionCallback EVH ran into undefined function (%1)",_function];
		};
	};
}];
