params
[
	["_position", 0, [0]],
	["_length", 0, [0]]
];

if (_length <= 0) exitWith {};

private _display = uiNamespace getVariable ["aasp_spotify_display", displayNull];
private _slider = _display displayCtrl 1100;
private _current_time = _display displayCtrl 1210;
private _total_time = _display displayCtrl 1215;

if (isNull _display || isNull _slider || isNull _current_time || isNull _total_time) then {};

// Limit position to the minimum and maximum of the song length
_position = ((_position max 0) min _length);

// Set slider range
_slider sliderSetRange [0, _length];

// Set slider position
_slider sliderSetPosition _position;

private _current_string = [_position, "MM:SS"] call BIS_fnc_secondsToString;
private _length_string = [_length, "MM:SS"] call BIS_fnc_secondsToString;
private _n_length_string = [_length - _position, "MM:SS"] call BIS_fnc_secondsToString;

// Convert seconds to time string which allows
_current_time ctrlSetText _current_string;
_total_time ctrlSetText ("-" + _n_length_string);
