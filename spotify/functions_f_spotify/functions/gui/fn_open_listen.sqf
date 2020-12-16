private _display = uinamespace getVariable ["aasp_spotify_display", displaynull];

if (isNull _display) exitWith {};

// Close all open menus that are already opened
call spotify_fnc_close_menus;

private _master_selection = _display displayCtrl 8000;
private _secondary_selection = _display displayCtrl 8010;

{
	_x setVariable ["aasp_auto_selection", true];
	_x lbSetCurSel ([-1, -1] select _foreachindex);
	_x setVariable ["aasp_auto_selection", false];
} foreach [_master_selection, _secondary_selection];

private _listen_group = _display displayCtrl 75000;
_listen_group ctrlShow true;
