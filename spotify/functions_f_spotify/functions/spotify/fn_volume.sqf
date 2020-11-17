params
[
	["_display", displayNull, [displayNull]],
	["_value", 100, [100, ""]],
	["_request", false, [false]],
	["_instant", false, [false]]
];

private _slider = _display displayCtrl 1315;
private _button = _display displayCtrl 1310;

if (_value isEqualType "") then
{
	_value = parseNumber _value;
};

systemchat str _value;

switch true do
{
	case (_value > 50):
	{
		_button ctrlSetText "\spotify\ui_f_spotify\data\icons\volume_high_ca.paa";
	};
	case (_value > 0):
	{
		_button ctrlSetText "\spotify\ui_f_spotify\data\icons\volume_low_ca.paa";
	};
	default
	{
		_button ctrlSetText "\spotify\ui_f_spotify\data\icons\volume_mute_ca.paa";
	};
};

// Incase this is called rather then through a user action
_slider sliderSetPosition _value;

// Save the last non-zero volume level
if (_value > 0) then
{
	missionNamespace setVariable ["aasp_volume_last", _value];
};
missionNamespace setVariable ["aasp_volume_variable", _value];

if _request then
{
	if _instant then
	{
		"ArmaSpotifyController" callExtension format["spotify:set_volume:%1", round _value];
	}
	else
	{
		aasp_last_request = time + 0.2;
		aasp_volume = _value;
		if (isNil "aasp_volume_wait") then
		{
			aasp_volume_wait = [] spawn
			{
				waitUntil {uisleep 0.1; time > aasp_last_request};
				"ArmaSpotifyController" callExtension format["spotify:set_volume:%1", round aasp_volume];
				aasp_volume_wait = nil;
			};
		};
	};
};
