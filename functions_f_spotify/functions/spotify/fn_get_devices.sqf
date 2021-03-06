params
[
	["_mode", "", [""]],
	["_data", [], [[]]]
];

#define ICONS \
[ \
	["unknown", "\spotify\ui_f_spotify\data\icons\unknown_ca.paa"], \
	["computer", "\spotify\ui_f_spotify\data\icons\laptop_ca.paa"], \
	["tablet", "\spotify\ui_f_spotify\data\icons\tablet_ca.paa"], \
	["smartphone", "\spotify\ui_f_spotify\data\icons\phone_ca.paa"], \
	["speaker", "\spotify\ui_f_spotify\data\icons\speaker_ca.paa"], \
	["tv", "\spotify\ui_f_spotify\data\icons\tv_ca.paa"], \
	["avr", "\spotify\ui_f_spotify\data\icons\tv_ca.paa"], \
	["stb", "\spotify\ui_f_spotify\data\icons\tv_ca.paa"], \
	["audiodongle", "\spotify\ui_f_spotify\data\icons\audio_ca.paa"], \
	["gameconsole", "\spotify\ui_f_spotify\data\icons\console_ca.paa"], \
	["castvideo", "\spotify\ui_f_spotify\data\icons\video_ca.paa"], \
	["castaudio", "\spotify\ui_f_spotify\data\icons\audio_ca.paa"], \
	["automobile", "\spotify\ui_f_spotify\data\icons\car_ca.paa"] \
]
#define PROFILE_COLOR [profilenamespace getvariable ['GUI_BCG_RGB_R',0.13], profilenamespace getvariable ['GUI_BCG_RGB_G',0.54], profilenamespace getvariable ['GUI_BCG_RGB_B',0.21],1]

private _display = uinamespace getVariable ["aasp_spotify_display", displaynull];

switch (toLower _mode) do
{
	case "button":
	{
		private _ctrl = _display displayCtrl 50000;
		if (isNull _ctrl) then
		{
			_ctrl = _display ctrlCreate ["devices_control_group", 50000];
			private _ctrl_pos = ctrlPosition _ctrl;
			private _pos = ctrlPosition (_display displayCtrl 555);
			private _diff = [(0.1 * safezoneX) - (_pos#0), (0.1 * safezoneY) - (_pos#1)];
			_ctrl ctrlSetPosition
			[
				(_pos#0) + (0.8 * safezoneW) - (0.115 * safezoneW) - (0.035 * safezoneW),
				(_pos#1) + (0.8 * safezoneH) - (0.25 * safezoneH),
				_ctrl_pos#2,
				_ctrl_pos#3
			];
			_ctrl ctrlCommit 0;
		}
		else
		{
			[_ctrl] spawn
			{
				ctrlDelete (_this#0);
			};
		};

		if (isNull _display) exitWith {};

		private _list = _ctrl controlsGroupCtrl 110;

		// Empty the list of options
		lbClear _list;

		// Add loading item
		_list lbAdd "No Spotify devices found";

		// Create random string
		private _variable = str round random 100000;
		uiNamespace setVariable [_variable, _list];

		// Load data from Spotify Web API
		"ArmaSpotifyController" callExtension format["spotify:get_devices:%1",_variable];
	};
	case "display":
	{
		_data params [["_list_variable","",[""]], ["_data",[],[[]]]];

		if (_list_variable == "" || count _data <= 0) exitWith {};

		private _list = uiNamespace getVariable [_list_variable, controlNull];
		if (isNull _list) exitWith {};

		// Clear the list
		lbClear _list;

		// Disable event handlers
		_list setVariable ["aasp_edit", true];

		private _active_index = -1;
		{
			_x params
			[
				["_id","",[""]],
				["_active",false,[false]],
				["_private",false,[false]],
				["_restricted",false,[false]],
				["_name","",[""]],
				["_type","",[""]],
				["_volume",100,[100]]
			];

			// Restricted devices will not accept Web API requests
			if !_restricted then
			{
				private _index = _list lbAdd _name;
				_list lbSetTooltip [_index, _name];
				_list lbSetData [_index, format["%1:%2",_id, _volume]];
				_list lbSetPicture
				[
					_index,
					ICONS#((ICONS findIf {_x#0 == _type}) max 0)#1
				];
				_list lbSetPictureRight
				[
					_index,
					["","\spotify\ui_f_spotify\data\icons\hidden_ca.paa"] select _private
				];

				_list lbSetPictureColor [_index, [[1,1,1,1], PROFILE_COLOR] select _active];
				_list lbSetColor [_index, [[1,1,1,1], PROFILE_COLOR] select _active];

				if _active then
				{
					_list lbSetValue [_index, -99];
					_list lbSetCurSel _index;

				};
			};
		} foreach _data;

		// Save current selection
		_list setVariable ["aasp_selection", lbCurSel _list];

		// Enable user selection
		_list ctrlEnable true;

		// Reallow event handlers
		_list setVariable ["aasp_edit", false];
	};
	case "list":
	{
		_data params [["_ctrl", controlNull, [controlNull]], ["_index", -1, [-1]]];

		// Make sure input is good
		if (isNull _ctrl || _index <= -1) exitWith {};
		// Make sure selection is not the current selection
		if (_ctrl getVariable ["aasp_selection", -1] == _index) exitWith {};
		// Make sure this isn't an automated change
		if (_ctrl getVariable ["aasp_edit", false]) exitWith {};

		private _data = _ctrl lbData _index;

		if (_data != "") then
		{
			private _data = _data splitString ":";
			// Request new playback device to Spotify Web API
			"ArmaSpotifyController" callExtension format["spotify:set_device:%1",_data#0];

			profilenamespace setVariable ["aasp_last_device",_data#0];
			saveProfileNamespace;

			[_data#1, false] call spotify_fnc_volume;
		}
		else
		{
			// Log error
			"ArmaSpotifyController" callExtension format["error:User requested device, but could not find device ID"];
		};

		// Force close to stop spamming
		private _display = ctrlParent _ctrl;
		["button", [_display, _display displayCtrl 50000, true]] call spotify_fnc_get_devices;
	};
};
