params
[
	["_mode", "", [""]],
	["_request", false, [false]],
	["_auto", false, [false]]
];

if _auto then
{
	private _current_mode = uiNamespace getVariable ["aasp_repeat_mode",""];
	private _modes = ["off","context","track"];
	private _index = (_modes findIf {_x == _current_mode}) + 1;
	if (_index >= 3) then
	{
		_index = 0;
	};
	_mode = _modes#_index;
};
if (_mode == "") exitWith {};

private _display = uiNamespace getVariable ["aasp_spotify_display", displayNull];
private _button = _display displayCtrl 1025;
private _dot = _display displayCtrl 1026;
if (isNull _display) then {};

_dot ctrlSetTextColor [profilenamespace getvariable ['GUI_BCG_RGB_R',0.13], profilenamespace getvariable ['GUI_BCG_RGB_G',0.54], profilenamespace getvariable ['GUI_BCG_RGB_B',0.21], 0.7];
_dot ctrlSetActiveColor [profilenamespace getvariable ['GUI_BCG_RGB_R',0.13], profilenamespace getvariable ['GUI_BCG_RGB_G',0.54], profilenamespace getvariable ['GUI_BCG_RGB_B',0.21], 0.7];
_button ctrlSetTextColor [profilenamespace getvariable ['GUI_BCG_RGB_R',0.13], profilenamespace getvariable ['GUI_BCG_RGB_G',0.54], profilenamespace getvariable ['GUI_BCG_RGB_B',0.21], 0.7];
_button ctrlSetActiveColor [profilenamespace getvariable ['GUI_BCG_RGB_R',0.13], profilenamespace getvariable ['GUI_BCG_RGB_G',0.54], profilenamespace getvariable ['GUI_BCG_RGB_B',0.21], 1];

switch _mode do
{
	case "context":
	{
		_button ctrlSetText "\spotify\ui_f_spotify\data\icons\repeat_ca.paa";
		_dot ctrlShow true;
	};
	case "track":
	{
		_button ctrlSetText "\spotify\ui_f_spotify\data\icons\repeat_single_ca.paa";
		_dot ctrlShow true;
	};
	default
	{
		_mode = "off";
		_button ctrlSetText "\spotify\ui_f_spotify\data\icons\repeat_ca.paa";
		_dot ctrlShow false;

		_button ctrlSetTextColor [1, 1, 1, 0.7];
		_button ctrlSetActiveColor [1, 1, 1, 1];
	};
};

uiNamespace setVariable ["aasp_repeat_mode",_mode];

if _request then
{
	"ArmaSpotifyController" callExtension format["spotify:repeat:%1",_mode];
};

_display setVariable ["aasp_last_click", diag_tickTime + 1];
