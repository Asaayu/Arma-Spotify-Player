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

[_value, (sliderRange _slider#1), false, true] call spotify_fnc_set_playback;

if _request then
{
	aasp_seek = _value;
	if (isNil "aasp_seek_wait") then
	{
		aasp_seek_wait = [] spawn
		{
			private _display = uiNamespace getVariable ["aasp_spotify_display", displayNull];
			private _slider = _display displayCtrl 1100;
			waitUntil {uisleep 0.1; !(_slider getVariable ["aasp_seeking", false]) || isNull _slider};
			if !(isNull _slider) then
			{
				"ArmaSpotifyController" callExtension format["spotify:seek:%1", round (aasp_seek * 1000)];
			};
			aasp_seek_wait = nil;
		};
	};
};
