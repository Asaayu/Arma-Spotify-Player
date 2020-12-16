params [["_ctrl",controlNull,[controlNull]]];

if (isNull _ctrl) exitWith {};

private _display = uiNamespace getVariable ["aasp_spotify_display", displayNull];
if !(isNull _display) exitWith {};

private _location = profilenamespace getVariable ['aasp_notification_location', 0];
private _announce = profilenamespace getVariable ['aasp_notification_announce', 0];

if (_location <= 0 || _announce <= 0) exitWith {};

aasp_notification_data params
[
	["_image_url", "", [""]],
	["_title", [], [[]]],
	["_artist", [], [[]]]
];
_title params
[
	["_title_url", "", [""]],
	["_title_width", 0, [0]],
	["_title_height", 0, [0]]
];
_artist params
[
	["_artist_url", "", [""]],
	["_artist_width", 0, [0]],
	["_artist_height", 0, [0]]
];

private _image = _ctrl controlsGroupCtrl 500;
private _title_group = _ctrl controlsGroupCtrl 750;
private _artist_group = _ctrl controlsGroupCtrl 1000;
private _title = _title_group controlsGroupCtrl 751;
private _artist = _artist_group controlsGroupCtrl 1001;

_image ctrlSetText _image_url;

private _coeff = (ctrlPosition _title#3)/(pixelH * _title_height);
_title ctrlSetText _title_url;
_title ctrlSetPositionW (pixelW * (_title_width * _coeff));
_title ctrlCommit 0;


private _coeff = (ctrlPosition _artist#3)/(pixelH * _artist_height);
_artist ctrlSetText _artist_url;
_artist ctrlSetPositionW (pixelW * (_artist_width * _coeff));
_artist ctrlCommit 0;

#define W(VALUE) (VALUE * safezoneW)
#define H(VALUE) (VALUE * safezoneH)
#define X(VALUE) (safezoneX + VALUE * safezoneW)
#define Y(VALUE) (safezoneY + VALUE * safezoneH)

#define WIDTH 0.15
#define HEIGHT 0.1

private _orig_pos = [X(0),Y(-HEIGHT),W(WIDTH),H(HEIGHT)];
private _final_pos = [X(0),Y(-HEIGHT) + H(HEIGHT),W(WIDTH),H(HEIGHT)];

switch _location do
{
	case 2:
	{
		// Top Right
		_orig_pos = [X(1) - W(WIDTH),Y(-HEIGHT),W(WIDTH),H(HEIGHT)];
		_final_pos = [X(1) - W(WIDTH),Y(-HEIGHT) + H(HEIGHT),W(WIDTH),H(HEIGHT)];
	};
	case 3:
	{
		// Bottom Left
		_orig_pos = [X(0),Y(1),W(WIDTH),H(HEIGHT)];
		_final_pos = [X(0),Y(1) - H(HEIGHT),W(WIDTH),H(HEIGHT)];
	};
	case 4:
	{
		// Bottom Right
		_orig_pos = [X(1) - W(WIDTH),Y(1),W(WIDTH),H(HEIGHT)];
		_final_pos = [X(1) - W(WIDTH),Y(1) - H(HEIGHT),W(WIDTH),H(HEIGHT)];
	};
};

aasp_notification_script = [_ctrl,_orig_pos,_final_pos] spawn
{
	params ["_ctrl","_orig_pos","_final_pos"];

	_ctrl ctrlSetPosition _orig_pos;
	_ctrl ctrlCommit 0;

	private _fade_time = 0.2;

	_ctrl ctrlShow true;
	_ctrl ctrlSetPosition _final_pos;
	_ctrl ctrlCommit 0.2;

	uisleep 0.2;

	private _title_group = _ctrl controlsGroupCtrl 750;
	private _artist_group = _ctrl controlsGroupCtrl 1000;
	private _title = _title_group controlsGroupCtrl 751;
	private _artist = _artist_group controlsGroupCtrl 1001;

	[_title, 0.1 * safezoneW] spawn spotify_fnc_text_scroll;
	[_artist, 0.1 * safezoneW] spawn spotify_fnc_text_scroll;

	uisleep 5;

	_ctrl ctrlSetPosition _orig_pos;
	_ctrl ctrlCommit 0.2;
};
