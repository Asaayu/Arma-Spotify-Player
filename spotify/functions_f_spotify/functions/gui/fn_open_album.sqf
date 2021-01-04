params [["_id","",[""]]];

private _display = uinamespace getVariable ["aasp_spotify_display", displaynull];

if (isNull _display) exitWith {};
if (_id == "") exitWith {};

// Close all open menus that are already opened
call spotify_fnc_close_menus;

private _master_selection = _display displayCtrl 8000;
private _secondary_selection = _display displayCtrl 8010;

{
	_x setVariable ["aasp_auto_selection", true];
	_x lbSetCurSel ([-1, -1] select _foreachindex);
	_x setVariable ["aasp_auto_selection", false];
} foreach [_master_selection, _secondary_selection];

uinamespace setVariable ["aasp_last_window", format["album:%1",_id]];
ctrlDelete (_display displayCtrl 88000);

_album_group = _display ctrlCreate ["album_control_group", 88000];
(ctrlPosition _album_group) params ["_x","_y","_w","_h"];

private _pos = ctrlPosition (_display displayCtrl 55000);
_album_group ctrlSetPosition _pos;
_album_group ctrlCommit 0;

private _control_group = _album_group controlsGroupCtrl 500;
private _load_button = _album_group controlsGroupCtrl 1000;
private _play_button = _album_group controlsGroupCtrl 200;
_play_button setVariable ['aasp_album_id', _id];

private _variable = str round random 100000;
uiNamespace setVariable [_variable, _control_group];
"ArmaSpotifyController" callExtension format["spotify:load_album:%1:%2",_variable, _id];

_load_button ctrlShow (_load_button getVariable ['aasp_show', false]);
