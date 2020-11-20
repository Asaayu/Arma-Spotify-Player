params
[
	["_value", 100, [100, ""]],
	["_request", false, [false]],
	["_instant", false, [false]]
];
disableSerialization;

private _display = uiNamespace getVariable ["aasp_spotify_display", displayNull];
private _slider = _display displayCtrl 1100;

if (_value isEqualType "") then
{
	_value = parseNumber _value;
};

// Slider eventhandlers are called on display killed
// This fixes that issue
if (isNil "aasp_seek") then { aasp_seek = 0; };
if (_value == aasp_seek) exitWith {};

private _length = missionNamespace getVariable ["aasp_song_length", 0];
[_value, _length/1000] call spotify_fnc_set_playback;

if _request then
{
	aasp_seek_last_request = time + 0.2;
	aasp_seek = _value;
	if (isNil "aasp_seek_wait") then
	{
		aasp_seek_wait = [] spawn
		{
			waitUntil {uisleep 0.1; time > aasp_seek_last_request};
			"ArmaSpotifyController" callExtension format["spotify:seek:%1", round (aasp_seek * 1000)];
			aasp_seek_wait = nil;
		};
	};
};
