private _display = uinamespace getVariable ["aasp_spotify_display", displaynull];

if (isNull _display) exitWith {};

// Close all open menus that are already opened
call spotify_fnc_close_menus;

private _master_selection = _display displayCtrl 8000;
private _secondary_selection = _display displayCtrl 8010;

{
	_x setVariable ["aasp_auto_selection", true];
	_x lbSetCurSel ([-1, 1] select _foreachindex);
	_x setVariable ["aasp_auto_selection", false];
} foreach [_master_selection, _secondary_selection];

uinamespace setVariable ["aasp_last_window", "liked"];

private _liked_group = _display displayCtrl 86000;
private _control_group = _liked_group controlsGroupCtrl 500;
private _load_button = _liked_group controlsGroupCtrl 1000;

if !(_display getVariable ["aasp_liked_loaded", false]) then
{
	private _variable = str round random 100000;
	uiNamespace setVariable [_variable, _control_group];
	"ArmaSpotifyController" callExtension format["spotify:get_liked_main:%1",_variable];

	_display setVariable ["aasp_liked_loaded", true];
};

_liked_group ctrlShow true;
_load_button ctrlShow (_load_button getVariable ['aasp_show', false]);
