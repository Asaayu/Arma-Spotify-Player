private _display = uinamespace getVariable ["aasp_spotify_display", displaynull];

if (isNull _display) exitWith {};

// Close all open menus that are already opened
call spotify_fnc_close_menus;

private _master_selection = _display displayCtrl 8000;
private _secondary_selection = _display displayCtrl 8010;

{
	_x setVariable ["aasp_auto_selection", true];
	_x lbSetCurSel ([0, -1] select _foreachindex);
	_x setVariable ["aasp_auto_selection", false];
} foreach [_master_selection, _secondary_selection];

uinamespace setVariable ["aasp_last_window", "home"];

private _home_group = _display displayCtrl 69000;
private _recent_ctrl = _home_group controlsGroupCtrl 500;
private _new_releases_ctrl = _home_group controlsGroupCtrl 550;
private _featured_playlists_ctrl = _home_group controlsGroupCtrl 600;

if !(_display getVariable ["aasp_home_loaded", false]) then
{
	private _variable = str round random 100000;
	uiNamespace setVariable [_variable, _recent_ctrl];
	"ArmaSpotifyController" callExtension format["spotify:get_recent:%1",_variable];

	private _variable = str round random 100000;
	uiNamespace setVariable [_variable, _new_releases_ctrl];
	"ArmaSpotifyController" callExtension format["spotify:get_releases:%1",_variable];

	private _variable = str round random 100000;
	uiNamespace setVariable [_variable, _featured_playlists_ctrl];
	"ArmaSpotifyController" callExtension format["spotify:get_featured:%1:%2",_variable];

	_display setVariable ["aasp_home_loaded", true];
};

_home_group ctrlShow true;
