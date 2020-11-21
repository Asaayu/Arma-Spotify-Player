params
[
	["_title_data", [], [[]]],
	["_artist_data", [], [[]]]
];
disableSerialization;

private _display = uiNamespace getVariable ["aasp_spotify_display", displayNull];
private _song_name_group = _display displayCtrl 1505;
private _song_artist_group = _display displayCtrl 1510;
private _song_like_ctrl = _display displayCtrl 1515;

private _name_ctrl = _song_name_group controlsGroupCtrl 1000;
private _artist_ctrl = _song_artist_group controlsGroupCtrl 1000;

if (isNull _display) exitWith {};
if (count _title_data <= 0 || count _artist_data <= 0) exitWith {};

_title_data params
[
	["_url", "", [""]],
	["_width", 0, [0]],
	["_height", 0, [0]]
];

// Only edit the name control if the image has changed
if (ctrlText _name_ctrl != _url) then
{
	_name_ctrl setVariable ["aasp_text_scroll", false];

	private _coeff = (ctrlPosition _name_ctrl#3)/(pixelH * _height);
	_name_ctrl ctrlSetText _url;
	_name_ctrl ctrlSetPositionX 0;
	_name_ctrl ctrlSetPositionW (pixelW * (_width * _coeff));
	_name_ctrl ctrlCommit 0;

	private _length = ((pixelW * (_width * _coeff)) min (0.1195 * safezoneW));
	_song_name_group ctrlSetPositionW _length;
	_song_name_group ctrlCommit 0;

	private _position = ctrlPosition _song_name_group;

	_song_like_ctrl ctrlShow true;
	_song_like_ctrl ctrlSetPositionX (_position#0) + (_position#2) + (0.0025 * safezoneW);
	_song_like_ctrl ctrlCommit 0;
};

_artist_data params
[
	["_url", "", [""]],
	["_width", 0, [0]],
	["_height", 0, [0]]
];

// Only edit the artist control if the image has changed
if (ctrlText _artist_ctrl != _url) then
{
	_artist_ctrl setVariable ["aasp_text_scroll", false];

	private _coeff = (ctrlPosition _artist_ctrl#3)/(pixelH * _height);
	_artist_ctrl ctrlSetText _url;
	_artist_ctrl ctrlSetPositionX 0;
	_artist_ctrl ctrlSetPositionW (pixelW * (_width * _coeff));
	_artist_ctrl ctrlCommit 0;
};
