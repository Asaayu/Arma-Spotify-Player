params
[
	["_position", 0, [0]],
	["_length", 0, [0]],
	["_auto", false, [false]],
	["_seek", false, [false]]
];

if (_length <= 0) exitWith {};

private _display = uiNamespace getVariable ["aasp_spotify_display", displayNull];
private _slider = _display displayCtrl 1100;
private _current_time = _display displayCtrl 1210;
private _total_time = _display displayCtrl 1215;

// Error checking
if (isNull _display) then {};

// Don't allow auto updates to overwrite the user when they are seeking
if (_auto && {_slider getVariable ["aasp_seeking", false]}) exitWith {};

// Don't allow auto update to increment if there was a change recently
// This avoids ui updates from causing jumps in the seek position
if (_auto && {missionNamespace setVariable ["aasp_seek_update", 0] >= diag_tickTime}) exitWith {};

// Limit position to the minimum and maximum of the song length
private _position = ((_position max 0) min _length);

// If the user is seeking and this isn't a seek request and the song hasen't changed then stop running
if (_slider getVariable ["aasp_seeking", false] && {!_seek && { (sliderRange _slider) isEqualTo [0, _length]}}) exitWith {};

_slider sliderSetRange [0, _length];
_slider sliderSetPosition _position;

private _current_string = [_position, "MM:SS"] call BIS_fnc_secondsToString;
private _length_string = [_length, "MM:SS"] call BIS_fnc_secondsToString;
private _n_length_string = [_length - _position, "MM:SS"] call BIS_fnc_secondsToString;

_current_time ctrlSetText _current_string;
_total_time ctrlSetText ("-" + _n_length_string);

missionNamespace setVariable ["aasp_seek_update", diag_tickTime + 0.75];
