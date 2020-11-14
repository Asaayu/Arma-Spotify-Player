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
			systemChat _data;
			private _array_data = parseSimpleArray _data;
			_array_data params ["_namespace","_variable","_value","_global"];
			(call compile _namespace) setVariable [_variable, _value, _global];
		};
		default
		{
			"ArmaSpotifyController" callExtension format["error:ExtensionCallback EVH ran into undefined function (%1)",__function];
		};
	};
}];
