params
[
	["_playing",false,[false]],
	["_nsfw",false,[false]],
	["_length",0,[0]],
	["_progress",0,[0]],
	["_song_name","",[""]],
	["_artist_name","",[""]],
	["_image","",[""]]
];

private _display = uiNamespace getVariable ["aasp_spotify_display", displayNull];
private _play_button = _display displayCtrl 1000;
private _song_icon_ctrl = _display displayCtrl 1500;
private _song_name_ctrl = _display displayCtrl 1505;
private _song_artist_ctrl = _display displayCtrl 1510;
private _song_like_ctrl = _display displayCtrl 1515;

if (isNull _display) exitWith {};
if (isNull _play_button) exitWith {};

_play_button ctrlSetText (["\spotify\ui_f_spotify\data\icons\play_ca.paa","\spotify\ui_f_spotify\data\icons\pause_ca.paa"] select _playing);

private _text_width = ((_song_name getTextWidth ["Spotify", 2.1 * (pixelH * pixelGridNoUIScale *  0.50)]) + 0.016) min (27 * (pixelW * pixelGridNoUIScale * 0.50));

private _title_position = ctrlPosition _song_name_ctrl;
_song_name_ctrl ctrlSetPositionW _text_width;
_song_like_ctrl ctrlSetPositionX (_title_position#0) - (_title_position#2) - (pixelW * pixelGridNoUIScale * 0.50);

{ _x ctrlCommit 0 } foreach [_song_like_ctrl,_song_name_ctrl];

_song_name_ctrl ctrlSetText _song_name;
_song_artist_ctrl ctrlSetText _artist_name;

[_progress/1000,_length/1000] call spotify_fnc_set_playback;

// Create random string
private _variable = str round random 100000;
uiNamespace setVariable [_variable, _song_icon_ctrl];

// Load data from Spotify Web API
"ArmaSpotifyController" callExtension format["spotify:download_image:%1:%2",_image,_variable];
