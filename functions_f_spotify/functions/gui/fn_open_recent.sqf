private _display = uinamespace getVariable ["aasp_spotify_display", displaynull];

if (isNull _display) exitWith {};

// Close all open menus that are already opened
call spotify_fnc_close_menus;

private _master_selection = _display displayCtrl 8000;
private _secondary_selection = _display displayCtrl 8010;

{
	_x setVariable ["aasp_auto_selection", true];
	_x lbSetCurSel ([-1, 0] select _foreachindex);
	_x setVariable ["aasp_auto_selection", false];
} foreach [_master_selection, _secondary_selection];

uinamespace setVariable ["aasp_last_window", "recent"];

private _recent_group = _display displayCtrl 85000;
private _control_group = _recent_group controlsGroupCtrl 500;

if !(_display getVariable ["aasp_recent_loaded", false]) then
{
	private _variable = str round random 100000;
	uiNamespace setVariable [_variable, _control_group];
	"ArmaSpotifyController" callExtension format["spotify:get_recent_main:%1",_variable];

	_display setVariable ["aasp_recent_loaded", true];
};

_recent_group ctrlShow true;
