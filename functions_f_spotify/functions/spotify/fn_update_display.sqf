params
[
	["_playing",false,[false]],
	["_nsfw",false,[false]],
	["_length",0,[0]],
	["_progress",0,[0]],
	["_image","",[""]],
	["_volume",100,[100]],
	["_shuffle",false,[false]],
	["_repeat","off",["off"]],
	["_id","",[""]],
	["_local",false,[false]]
];

private _display = uiNamespace getVariable ["aasp_spotify_display", displayNull];
private _play_button = _display displayCtrl 1000;
private _song_icon_ctrl = _display displayCtrl 1500;
private _no_device_ctrl = _display displayCtrl 1306;
private _private_ctrl = _display displayCtrl 1307;
private _song_like_ctrl = _display displayCtrl 1515;
if (isNull _display) exitWith {};

private _last_click = _display getVariable ["aasp_last_click", 0];

_no_device_ctrl ctrlShow false;
_private_ctrl ctrlShow false;

private _old_id = _song_like_ctrl getVariable ["aasp_song_id", ""];
_song_like_ctrl setVariable ["aasp_song_id", _id];

// This stops the update from instaly reversing a users input in the GUI
if (_last_click < diag_tickTime) then
{
	_play_button ctrlSetText (["\spotify\ui_f_spotify\data\icons\play_ca.paa","\spotify\ui_f_spotify\data\icons\pause_ca.paa"] select _playing);

	[_progress/1000,_length/1000] call spotify_fnc_set_playback;

	[_shuffle, false] call spotify_fnc_set_shuffle;
	[_repeat, false] call spotify_fnc_set_repeat;

	[_volume] call spotify_fnc_volume;
};

if (_id != "") then
{
	if (_old_id != _id) then
	{
		_song_icon_ctrl ctrlSetEventHandler ["ButtonClick", format["'ArmaSpotifyController' callExtension 'spotify:track:%1'", _id]];
		// Check if song is liked
		"ArmaSpotifyController" callExtension format["spotify:liked:%1",_id];
	};
	_song_like_ctrl ctrlShow true;
}
else
{
	_song_icon_ctrl ctrlSetText "\spotify\ui_f_spotify\data\placeholder\64x_co.paa";
	_song_like_ctrl ctrlShow false;
};

if (!_local && {_old_id != _id}) then
{
	// Load data from Spotify Web API
	private _variable = str round random 100000;
	uiNamespace setVariable [_variable, _song_icon_ctrl];
	"ArmaSpotifyController" callExtension format["spotify:download_image:%1:%2",_image,_variable];
};
