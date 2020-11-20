params
[
	["_like", true, [true]],
	["_request", false, [false]],
	["_id", "", [""]]
];

private _display = uiNamespace getVariable ["aasp_spotify_display", displayNull];
private _song_like_ctrl = _display displayCtrl 1515;

if (isNull _display) exitWith {};

private _song_id = _song_like_ctrl getVariable ["aasp_song_id", ""];
if (_song_id != _id) exitWith {};

if _like then
{
	_song_like_ctrl ctrlSetText "\spotify\ui_f_spotify\data\icons\like_full_ca.paa";
}
else
{
	_song_like_ctrl ctrlSetText "\spotify\ui_f_spotify\data\icons\like_empty_ca.paa";
};

if _request then
{
	"ArmaSpotifyController" callExtension format["spotify:%1:%2", ["unlike","like"] select _like, _id];
};
