params
[
	["_playing",false,[false]],
	["_nsfw",false,[false]],
	["_length",0,[0]],
	["_progress",0,[0]],
	["_song_name","",[""]],
	["_artist_name","",[""]],
	["_image","",[""]],
	["_volume",100,[100]],
	["_shuffle",false,[false]],
	["_repeat","off",["off"]],
	["_id","",[""]]
];
disableSerialization;

private _display = uiNamespace getVariable ["aasp_spotify_display", displayNull];
private _play_button = _display displayCtrl 1000;
private _song_icon_ctrl = _display displayCtrl 1500;
private _no_device_ctrl = _display displayCtrl 1306;
private _song_like_ctrl = _display displayCtrl 1515;

if (isNull _display) exitWith {};

_no_device_ctrl ctrlShow false;

_song_like_ctrl setVariable ["aasp_song_id", _id];

_play_button ctrlSetText (["\spotify\ui_f_spotify\data\icons\play_ca.paa","\spotify\ui_f_spotify\data\icons\pause_ca.paa"] select _playing);

[_progress/1000,_length/1000] call spotify_fnc_set_playback;

[_shuffle, false] call spotify_fnc_set_shuffle;
[_repeat, false] call spotify_fnc_set_repeat;

[_volume] call spotify_fnc_volume;

// Song Icon
private _variable = str round random 100000;
uiNamespace setVariable [_variable, _song_icon_ctrl];

// Load data from Spotify Web API
"ArmaSpotifyController" callExtension format["spotify:download_image:%1:%2",_image,_variable];

// Check if song is liked
"ArmaSpotifyController" callExtension format["spotify:liked:%1",_id];
