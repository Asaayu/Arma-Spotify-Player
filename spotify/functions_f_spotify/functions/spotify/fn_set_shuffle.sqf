params
[
	["_mode", false, [false]],
	["_request", false, [false]]
];

private _display = uiNamespace getVariable ["aasp_spotify_display", displayNull];
private _button = _display displayCtrl 1020;
private _dot = _display displayCtrl 1021;
if (isNull _display) then {};

uiNamespace setVariable ["aasp_shuffle_mode",_mode];
_dot ctrlShow _mode;

if _mode then
{
	_dot ctrlSetTextColor [profilenamespace getvariable ['GUI_BCG_RGB_R',0.13], profilenamespace getvariable ['GUI_BCG_RGB_G',0.54], profilenamespace getvariable ['GUI_BCG_RGB_B',0.21], 0.7];
	_dot ctrlSetActiveColor [profilenamespace getvariable ['GUI_BCG_RGB_R',0.13], profilenamespace getvariable ['GUI_BCG_RGB_G',0.54], profilenamespace getvariable ['GUI_BCG_RGB_B',0.21], 0.7];
	_button ctrlSetTextColor [profilenamespace getvariable ['GUI_BCG_RGB_R',0.13], profilenamespace getvariable ['GUI_BCG_RGB_G',0.54], profilenamespace getvariable ['GUI_BCG_RGB_B',0.21], 0.7];
	_button ctrlSetActiveColor [profilenamespace getvariable ['GUI_BCG_RGB_R',0.13], profilenamespace getvariable ['GUI_BCG_RGB_G',0.54], profilenamespace getvariable ['GUI_BCG_RGB_B',0.21], 1];
}
else
{
	_dot ctrlSetTextColor [1, 1, 1, 0.7];
	_dot ctrlSetActiveColor [1, 1, 1, 0.7];
	_button ctrlSetTextColor [1, 1, 1, 0.7];
	_button ctrlSetActiveColor [1, 1, 1, 1];
};

if _request then
{
	"ArmaSpotifyController" callExtension format["spotify:shuffle:%1",_mode];
};

_display setVariable ["aasp_last_click", diag_tickTime + 1];
