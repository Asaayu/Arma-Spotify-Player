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

uinamespace setVariable ["aasp_last_window", "playlist:" + _id];
ctrlDelete (_display displayCtrl 87000);

_playlist_group = _display ctrlCreate ["playlist_control_group", 87000];

(ctrlPosition _playlist_group) params ["_x","_y","_w","_h"];

private _pos = ctrlPosition (_display displayCtrl 55000);
_playlist_group ctrlSetPosition _pos;
_playlist_group ctrlCommit 0;

private _control_group = _playlist_group controlsGroupCtrl 500;
private _load_button = _playlist_group controlsGroupCtrl 1000;
private _play_button = _playlist_group controlsGroupCtrl 200;
_play_button setVariable ['aasp_playlist_id', _id];

private _show = (_total > 50 && _offset + 50 < _total);
_load_button ctrlShow _show;
_load_button setVariable ['aasp_show', _show];

private _variable_title = str round random 100000;
private _variable_subtitle = str round random 100000;
private _variable_image = str round random 100000;
uiNamespace setVariable [_variable_title, (_playlist_group controlsGroupCtrl 100) controlsGroupCtrl 100];
uiNamespace setVariable [_variable_subtitle, (_playlist_group controlsGroupCtrl 150) controlsGroupCtrl 150];
uiNamespace setVariable [_variable_image, _playlist_group controlsGroupCtrl 50];
"ArmaSpotifyController" callExtension format["spotify:set_playlist_info:%1:%2:%3",_variable_title, _variable_subtitle, _variable_image];

private _variable = str round random 100000;
uiNamespace setVariable [_variable, _control_group];
"ArmaSpotifyController" callExtension format["spotify:load_playlist:%1:%2",_variable, _id];

_load_button ctrlShow (_load_button getVariable ['aasp_show', false]);
