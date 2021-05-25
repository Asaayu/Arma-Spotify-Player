"ArmaSpotifyController" callExtension "preinit";

addMissionEventHandler ["ExtensionCallback",
{
	params ["_name", "_function", "_data"];
	// This eventhandler is specifically for AASP
	if (_name != "ArmaSpotifyController") exitWith {};

	// Switch cases for all the different callbacks
	switch (toLower _function) do
	{
		case ("setvariable"):
		{
			private _array_data = parseSimpleArray _data;
			_array_data params [["_namespace","localnamespace",[""]],["_variable","",[""]],["_value", false, [false]]];
			(call compile _namespace) setVariable [_variable, _value];
		};
		case ("ctrlsettext"):
		{
			private _data = _data splitString "|";
			(uiNamespace getVariable [_data#1, controlNull]) ctrlSetText (_data#0);
		};
		case ("spotify_fnc_update_like"):
		{
			(parseSimpleArray _data) params ["_result","_id"];
			[_result, false, _id] call spotify_fnc_like;
		};
		case ("spotify_fnc_set_playback"):
		{
			(parseSimpleArray _data) call spotify_fnc_set_playback;
		};
		case ("spotify_fnc_get_devices"):
		{
			(parseSimpleArray _data) call spotify_fnc_get_devices;
		};
		case ("spotify_fnc_update_display"):
		{
			(parseSimpleArray _data) call spotify_fnc_update_display;
		};
		case ("spotify_fnc_update_song_info"):
		{
			(parseSimpleArray _data) call spotify_fnc_update_song_info;
		};
		case ("device_required"):
		{
			private _display = uinamespace getVariable ["aasp_spotify_display", displaynull];
			private _no_device_ctrl = _display displayCtrl 1306;
			private _private_ctrl = _display displayCtrl 1307;
			_no_device_ctrl ctrlShow true;
			_private_ctrl ctrlShow false;

			// On the first time attempt to connect to the last connected device
			if !(uiNamespace getVariable ["aasp_auto_connect", false]) then
			{
				uiNamespace setVariable ["aasp_auto_connect", true];

				// Attempt to reconnect to the last connected deivce
				private _last_device = profileNamespace getVariable ["aasp_last_device", ""];
				if (_last_device != "") then
				{
					// Request new playback device to Spotify Web API
					"ArmaSpotifyController" callExtension format["spotify:set_device:%1", _last_device];
				};
			};
		};
		case ("private_session"):
		{
			private _display = uinamespace getVariable ["aasp_spotify_display", displaynull];
			private _no_device_ctrl = _display displayCtrl 1306;
			private _private_ctrl = _display displayCtrl 1307;
			_no_device_ctrl ctrlShow false;
			_private_ctrl ctrlShow true;
		};
		case ("append_list"):
		{
			(parseSimpleArray _data) params ["_variable","_limit","_type","_id","_image_url","_title","_artist"];

			private _display = uinamespace getVariable ["aasp_spotify_display", displaynull];
			if (isNull _display) exitWith {};

			private _ctrl = uinamespace getVariable [_variable, controlNull];
			if (isNull _ctrl) exitWith {};

			private _index = _ctrl getVariable ["aasp_ctrl_index", 0];
			if ((_index/3) >= _limit) exitWith {};

			private _x = (0.115 * safeZoneW) * (_index/3);
			_title params
			[
				["_url", "", [""]],
				["_width", 0, [0]],
				["_height", 0, [0]]
			];

			private _image_width =  0.1 * safeZoneW;
			private _image_height =  0.1 * 1.75 * safezoneH;
			private _title_height =  0.00975 * 1.75 * safezoneH;
			private _artist_height =  0.0075 * 1.75 * safezoneH;

			private _image = _display ctrlCreate ["ctrlActivePictureKeepAspect", -1, _ctrl];
			_image ctrlSetPosition [ _x, 0, _image_width, _image_height];
			_image ctrlSetActiveColor [1,1,1,1];
			_image ctrlSetTextColor [1,1,1,0.8];
			switch true do
			{
				case (_type == "playlist"):
				{
					_image ctrlAddEventHandler ["ButtonClick", format["['%1'] call spotify_fnc_open_playlist", _id]];
				};
				case (_type == "album"):
				{
					_image ctrlAddEventHandler ["ButtonClick", format["['%1'] call spotify_fnc_open_album", _id]];
				};
				case (_type == "track"):
				{
					_image ctrlAddEventHandler ["ButtonClick", format["'ArmaSpotifyController' callExtension 'spotify:play_%1:%2'", _type, _id]];
				};
			};

			private _variable = str round random 100000;
			uiNamespace setVariable [_variable, _image];
			"ArmaSpotifyController" callExtension format["spotify:download_image:%1:%2",_image_url,_variable];

			private _title_holder = _display ctrlCreate ["ctrlControlsGroupNoHScrollbars", -1, _ctrl];
			_title_holder ctrlSetPosition [_x, _image_height, _image_width, _title_height];
			_title_holder ctrlCommit 0;

			private _title = _display ctrlCreate ["ctrlActivePictureKeepAspect", -1, _title_holder];
			_title ctrlSetPosition [0, 0, _image_width, _title_height * 0.98];
			_title ctrlAddEventHandler ["MouseEnter", { [_this#0, 0.1 * safezoneW] spawn spotify_fnc_text_scroll }];
			_title ctrlAddEventHandler ["ButtonClick", format["'ArmaSpotifyController' callExtension 'spotify:%1:%2'", _type, _id]];
			_title ctrlSetTooltip "Click to open this in Spotify";
			_title ctrlSetActiveColor [1,1,1,1];
			_title ctrlSetTextColor [1,1,1,1];
			_title ctrlCommit 0;

			private _coeff = (ctrlPosition _title#3)/(pixelH * _height);
			_title ctrlSetText _url;
			_title ctrlSetPositionX 0;
			_title ctrlSetPositionW (pixelW * (_width * _coeff));
			_title ctrlCommit 0;

			_artist params
			[
				["_url", "", [""]],
				["_width", 0, [0]],
				["_height", 0, [0]]
			];

			private _artist_holder = _display ctrlCreate ["ctrlControlsGroupNoHScrollbars", -1, _ctrl];
			_artist_holder ctrlSetPosition [_x, _image_height + _title_height, _image_width, _artist_height];
			_artist_holder ctrlCommit 0;

			private _artist = _display ctrlCreate ["ctrlActivePictureKeepAspect", -1, _artist_holder];
			_artist ctrlSetPosition [0, 0, _image_width, _artist_height * 0.98];
			_artist ctrlAddEventHandler ["MouseEnter", { [_this#0, 0.1 * safezoneW] spawn spotify_fnc_text_scroll }];
			_artist ctrlSetActiveColor [1,1,1,1];
			_artist ctrlSetTextColor [1,1,1,1];
			_artist ctrlCommit 0;

			private _coeff = (ctrlPosition _artist#3)/(pixelH * _height);
			_artist ctrlSetText _url;
			_artist ctrlSetPositionW (pixelW * (_width * _coeff));
			_artist ctrlCommit 0;

			{_x ctrlCommit 0} foreach [_image,_title_holder,_title,_artist_holder,_artist];

			_ctrl setVariable ["aasp_ctrl_index", _index + 3];
		};
		case ("append_grid"):
		{
			(parseSimpleArray _data) params ["_variable","_id","_type","_image_url","_title","_artist"];

			private _display = uinamespace getVariable ["aasp_spotify_display", displaynull];
			if (isNull _display) exitWith {};

			private _ctrl = uinamespace getVariable [_variable, controlNull];
			if (isNull _ctrl) exitWith {};

			private _x_index = _ctrl getVariable ["aasp_x_index", 0];
			private _y_index = _ctrl getVariable ["aasp_y_index", 0];

			private _x = (0.115 * safeZoneW) * _x_index;
			private _y = (0.12225 * 1.75 * safeZoneH) * _y_index;
			_title params
			[
				["_url", "", [""]],
				["_width", 0, [0]],
				["_height", 0, [0]]
			];

			private _image_width =  0.095 * safeZoneW;
			private _image_height =  0.095 * 1.75 * safezoneH;
			private _title_height =  0.00975 * 1.75 * safezoneH;
			private _artist_height =  0.0075 * 1.75 * safezoneH;

			private _image = _display ctrlCreate ["ctrlActivePictureKeepAspect", -1, _ctrl];
			_image ctrlSetPosition [ _x, _y, _image_width, _image_height];
			_image ctrlSetActiveColor [1,1,1,1];
			_image ctrlSetTextColor [1,1,1,0.8];
			_image ctrlAddEventHandler ["ButtonClick", format["'ArmaSpotifyController' callExtension 'spotify:play_%1:%2'", _type, _id]];

			private _variable = str round random 100000;
			uiNamespace setVariable [_variable, _image];
			"ArmaSpotifyController" callExtension format["spotify:download_image:%1:%2",_image_url,_variable];

			private _title_holder = _display ctrlCreate ["ctrlControlsGroupNoHScrollbars", -1, _ctrl];
			_title_holder ctrlSetPosition [_x, _y + _image_height, _image_width, _title_height];
			_title_holder ctrlCommit 0;

			private _title = _display ctrlCreate ["ctrlActivePictureKeepAspect", -1, _title_holder];
			_title ctrlSetPosition [0, 0, _image_width, _title_height * 0.98];
			_title ctrlAddEventHandler ["MouseEnter", { [_this#0, 0.1 * safezoneW] spawn spotify_fnc_text_scroll }];
			_title ctrlAddEventHandler ["ButtonClick", format["'ArmaSpotifyController' callExtension 'spotify:%1:%2'", _type, _id]];
			_title ctrlSetActiveColor [1,1,1,1];
			_title ctrlSetTextColor [1,1,1,1];
			_title ctrlCommit 0;

			private _coeff = (ctrlPosition _title#3)/(pixelH * _height);
			_title ctrlSetText _url;
			_title ctrlSetTooltip "Click to open the song in Spotify";
			_title ctrlSetPositionX 0;
			_title ctrlSetPositionW (pixelW * (_width * _coeff));
			_title ctrlCommit 0;

			_artist params
			[
				["_url", "", [""]],
				["_width", 0, [0]],
				["_height", 0, [0]]
			];

			private _artist_holder = _display ctrlCreate ["ctrlControlsGroupNoHScrollbars", -1, _ctrl];
			_artist_holder ctrlSetPosition [_x, _y + _image_height + _title_height, _image_width, _artist_height];
			_artist_holder ctrlCommit 0;

			private _artist = _display ctrlCreate ["ctrlActivePictureKeepAspect", -1, _artist_holder];
			_artist ctrlSetPosition [0, 0, _image_width, _artist_height * 0.98];
			_artist ctrlAddEventHandler ["MouseEnter", { [_this#0, 0.1 * safezoneW] spawn spotify_fnc_text_scroll }];
			_artist ctrlSetActiveColor [1,1,1,1];
			_artist ctrlSetTextColor [1,1,1,1];
			_artist ctrlCommit 0;

			private _coeff = (ctrlPosition _artist#3)/(pixelH * _height);
			_artist ctrlSetText _url;
			_artist ctrlSetPositionW (pixelW * (_width * _coeff));
			_artist ctrlCommit 0;

			{_x ctrlCommit 0} foreach [_image,_title_holder,_title,_artist_holder,_artist];

			_ctrl ctrlSetPositionH (0.12225 * 1.75 * safeZoneH) * (_y_index + 1);
			_ctrl ctrlCommit 0;

			if (_x_index >= 5) then
			{
				_x_index = -1;
				_y_index = _y_index + 1;
			};

			_ctrl setVariable ["aasp_x_index", _x_index + 1];
			_ctrl setVariable ["aasp_y_index", _y_index];
		};
		case ("append_playlist"):
		{
			(parseSimpleArray _data) params
			[
				["_url", "", [""]],
				["_width", 0, [0]],
				["_height", 0, [0]],
				["_id", "", [""]],
				["_total", 0, [0]],
				["_offset", 0, [0]]
			];

			private _display = uinamespace getVariable ["aasp_spotify_display", displaynull];
			private _list = _display displayCtrl 8025;
			if (isNull _display) exitWith {};

			private _title_height = 0.018 * safeZoneH;
			private _title_width = 0.1 * safezoneW;

			private _index = _list getVariable ["aasp_ctrl_index", 10];
			private _y = _title_height * (_index - 10);

			private _title_holder = _display ctrlCreate ["ctrlControlsGroupNoHScrollbars", _index, _list];
			_title_holder ctrlSetPosition [0, _y, _title_width, _title_height];
			_title_holder ctrlCommit 0;

			private _title = _display ctrlCreate ["ctrlActivePictureKeepAspect", _index, _title_holder];
			_title ctrlSetPosition [0, 0, _title_width, _title_height * 0.98];
			_title ctrlAddEventHandler ["MouseEnter", { [_this#0, 0.1 * safezoneW] spawn spotify_fnc_text_scroll }];
			_title ctrlAddEventHandler ["ButtonClick", format["['%1'] call spotify_fnc_open_playlist", _id]];
			_title ctrlSetActiveColor [1,1,1,1];
			_title ctrlSetTextColor [1,1,1,0.7];
			_title ctrlCommit 0;

			private _load_button = _display displayCtrl 8030;
			private _show = (_total > 50 && _offset + 50 < _total);
			_load_button ctrlShow _show;

			private _coeff = (ctrlPosition _title#3)/(pixelH * _height);
			_title ctrlSetText _url;
			_title ctrlSetPositionX 0;
			_title ctrlSetPositionW (pixelW * (_width * _coeff));
			_title ctrlCommit 0;

			_list setVariable ["aasp_ctrl_index", _index+1];
			_list setVariable ["aasp_playlist_id", _id];
		};
		case ("append_textlist_playlist"):
		{
			(parseSimpleArray _data) params ["_variable","_id","_title","_artist","_album","_album_id","_song_uri","_total","_offset",["_liked_playlist", false, [false]]];

			private _display = uinamespace getVariable ["aasp_spotify_display", displaynull];
			if (isNull _display) exitWith {};

			private _ctrl = uinamespace getVariable [_variable, controlNull];
			if (isNull _ctrl) exitWith {};

			private _load_button = (_display displayCtrl 86000) controlsGroupCtrl 1000;
			if !_liked_playlist then
			{
				_load_button = (_display displayCtrl 87000) controlsGroupCtrl 1000;
			};
			private _show = (_total > 50 && _offset + 50 < _total);
			_load_button ctrlShow _show;
			_load_button setVariable ['aasp_show', _show];

			private _index = _ctrl getVariable ["aasp_ctrl_index", 10];
			private _x_value = 0;
			private _y_value = (0.0275 * safeZoneH) * (_index - 10)/3;

			private _type = ["playlist","album"] select (_id == _album_id);

			private _ctrl_height = 0.02 * safezoneH;
			{
				_x params
				[
					["_url", "", [""]],
					["_width", 0, [0]],
					["_height", 0, [0]]
				];

				private _ctrl_width = ([0.34, 0.15, 0.13]#_foreachindex) * safeZoneW;
				if (_type == "album") then
				{
					_ctrl_width = ([0.34, 0.22, 0.06]#_foreachindex) * safeZoneW;
				};

				private _holder = _display ctrlCreate ["ctrlControlsGroupNoHScrollbars", _index, _ctrl];
				_holder ctrlSetPosition [_x_value, _y_value + (0.0025 * safeZoneH), _ctrl_width, _ctrl_height];
				_holder ctrlCommit 0;

				private _text_image = _display ctrlCreate ["ctrlActivePictureKeepAspect", -1, _holder];
				_text_image ctrlSetPosition [0, 0, _ctrl_width, _ctrl_height * 0.98];
				_text_image ctrlAddEventHandler ["MouseEnter", format["[_this#0, %1] spawn spotify_fnc_text_scroll", _ctrl_width]];
				_text_image ctrlSetActiveColor [1,1,1,1];
				_text_image ctrlSetTextColor [1,1,1,1];
				switch _foreachindex do
				{
					case 0:
					{
						// Title
						_text_image ctrlAddEventHandler ["MouseButtonClick", format["(_this + ['%1',%2,'%3',%4]) call spotify_fnc_track_click", _id, (_index - 10)/3, _type, _liked_playlist]];
						_text_image ctrlSetTooltip "[Left Click] to play this song";
					};
					case 2:
					{
						if (_type != "album") then
						{
							// Album
							_text_image ctrlAddEventHandler ["ButtonClick", format["[] spawn {['%1'] call spotify_fnc_open_album}",_album_id]];
							_text_image ctrlSetTooltip "[Left Click] to open this album";
						};
					};
				};
				_text_image ctrlCommit 0;

				private _coeff = (ctrlPosition _text_image#3)/(pixelH * _height);
				_text_image ctrlSetText _url;
				_text_image ctrlSetPositionX 0;
				_text_image ctrlSetPositionW (pixelW * (_width * _coeff));
				_text_image ctrlCommit 0;

				if (_type == "album") then
				{
					_x_value = _x_value + ([0.35, 0.23, 0.13]#_foreachindex) * safeZoneW;
				}
				else
				{
					_x_value = _x_value + ([0.35, 0.16, 0.06]#_foreachindex) * safeZoneW;
				};
			} foreach [_title, _artist, _album];

			// Add extra missing space
			if (_type != "album") then
			{
				_x_value = _x_value + (0.07 * safeZoneW);
			};

			private _link = _display ctrlCreate ["ctrlActivePicture", _index + 1, _ctrl];
			_link ctrlSetText "\spotify\ui_f_spotify\data\icons\link_ca.paa";
			_link ctrlSetTooltip "Open this song in Spotify";
			_link ctrlSetPosition [_x_value + (0.01 * safeZoneW), _y_value + (0.0025 * safeZoneH), 0.0114 * safeZoneW, 0.0114 * 1.75 * safeZoneH];
			_link ctrlAddEventHandler ["ButtonClick", format["'ArmaSpotifyController' callExtension '%1'",_song_uri]];
			_link ctrlCommit 0;

			private _spacer = _display ctrlCreate ["ctrlStatic", _index + 2, _ctrl];
			_spacer ctrlSetPosition [0, _y_value + _ctrl_height + (0.005 * safeZoneH), 0.69 * safeZoneW, 0.0025 * safeZoneH];
			_spacer ctrlSetBackgroundColor [0.2,0.2,0.2,1];
			_spacer ctrlCommit 0;

			_ctrl setVariable ["aasp_ctrl_index", _index + 3];
			_ctrl setVariable ["aasp_playlist_id", _id];
		};
		case ("set_playlist_info"):
		{
			(parseSimpleArray _data) params ["_title","_subtitle","_image"];

			private _display = uinamespace getVariable ["aasp_spotify_display", displaynull];
			if (isNull _display) exitWith {};

			private _ctrl = _display displayCtrl 87000;
			if (isNull _ctrl) exitWith {};

			private _image_ctrl = _ctrl controlsGroupCtrl 50;
			private _title_ctrl_master = _ctrl controlsGroupCtrl 100;
			private _subtitle_ctrl_master = _ctrl controlsGroupCtrl 150;
			private _title_ctrl = _title_ctrl_master controlsGroupCtrl 105;
			private _subtitle_ctrl = _subtitle_ctrl_master controlsGroupCtrl 155;

			{
				_x params
				[
					["_url", "", [""]],
					["_width", 0, [0]],
					["_height", 0, [0]]
				];

				private _ctrl = [_title_ctrl,_subtitle_ctrl]#_foreachindex;

				(ctrlPosition _ctrl) params ["_x","_y","_w","_h"];
				_ctrl ctrlSetPosition [0,0,_w,_h];
				_ctrl ctrlAddEventHandler ["MouseEnter", format["[_this#0, %1] spawn spotify_fnc_text_scroll", 0.57 * safeZoneW]];
				_ctrl ctrlSetActiveColor [1,1,1,1];
				_ctrl ctrlSetTextColor [1,1,1,1];
				_ctrl ctrlCommit 0;

				private _coeff = _h/(pixelH * _height);
				_ctrl ctrlSetText _url;
				_ctrl ctrlSetPositionX 0;
				_ctrl ctrlSetPositionW (pixelW * (_width * _coeff));
				_ctrl ctrlCommit 0;
			} foreach [_title, _subtitle];

			if (_subtitle#0 == "") then
			{
				_title_ctrl_master ctrlSetPositionY ((0.1 * 1.75 * safeZoneH) - (0.035 * safeZoneH));
			};

			if (_image != "") then
			{
				_image_ctrl ctrlSetText _image;
			}
			else
			{
				_image_ctrl ctrlShow false;
				_title_ctrl_master ctrlSetPositionX (0.01 * safeZoneW);
				_subtitle_ctrl_master ctrlSetPositionX (0.01 * safeZoneW);
				{_x ctrlCommit 0} foreach [_title_ctrl_master, _subtitle_ctrl_master];
			};
		};
		case ("set_album_info"):
		{
			(parseSimpleArray _data) params ["_title","_subtitle","_copyright","_image"];

			private _display = uinamespace getVariable ["aasp_spotify_display", displaynull];
			if (isNull _display) exitWith {};

			private _ctrl = _display displayCtrl 88000;
			if (isNull _ctrl) exitWith {};

			private _image_ctrl = _ctrl controlsGroupCtrl 50;
			private _title_ctrl_master = _ctrl controlsGroupCtrl 100;
			private _subtitle_ctrl_master = _ctrl controlsGroupCtrl 150;
			private _copyright_ctrl_master = _ctrl controlsGroupCtrl 170;
			private _title_ctrl = _title_ctrl_master controlsGroupCtrl 105;
			private _subtitle_ctrl = _subtitle_ctrl_master controlsGroupCtrl 155;
			private _copyright_ctrl = _copyright_ctrl_master controlsGroupCtrl 175;

			{
				_x params
				[
					["_url", "", [""]],
					["_width", 0, [0]],
					["_height", 0, [0]]
				];

				private _ctrl = [_title_ctrl,_subtitle_ctrl,_copyright_ctrl]#_foreachindex;

				(ctrlPosition _ctrl) params ["_x","_y","_w","_h"];
				_ctrl ctrlSetPosition [0,0,_w,_h];
				_ctrl ctrlAddEventHandler ["MouseEnter", format["[_this#0, %1] spawn spotify_fnc_text_scroll", 0.57 * safeZoneW]];
				_ctrl ctrlSetActiveColor [1,1,1,1];
				_ctrl ctrlSetTextColor [1,1,1,1];
				_ctrl ctrlCommit 0;

				private _coeff = _h/(pixelH * _height);
				_ctrl ctrlSetText _url;
				_ctrl ctrlSetPositionX 0;
				_ctrl ctrlSetPositionW (pixelW * (_width * _coeff));
				_ctrl ctrlCommit 0;
			} foreach [_title, _subtitle, _copyright];

			if (_subtitle#0 == "") then
			{
				_title_ctrl_master ctrlSetPositionY ((0.1 * 1.75 * safeZoneH) - (0.035 * safeZoneH));
			};

			if (_image != "") then
			{
				_image_ctrl ctrlSetText _image;
			}
			else
			{
				_image_ctrl ctrlShow false;
				_title_ctrl_master ctrlSetPositionX (0.01 * safeZoneW);
				_subtitle_ctrl_master ctrlSetPositionX (0.01 * safeZoneW);
				{_x ctrlCommit 0} foreach [_title_ctrl_master, _subtitle_ctrl_master];
			};
		};
		case ("show_notification_song"):
		{
			private _display = (findDisplay 46);
			if (isNull _display) exitWith {};

			if !(isNil "aasp_notification_script") then
			{
				terminate aasp_notification_script;
				aasp_notification_script = nil;
			};

			aasp_notification_data = (parseSimpleArray _data);

			"aasp_notification" cutRsc ["aasp_notification", "PLAIN", -1, false];
		};
		case ("reload_display"):
		{
			closeDialog 2;
			[] spawn
			{
				createDialog "AASP_spotify";
			};
		};
		default
		{
			"ArmaSpotifyController" callExtension format["error:ExtensionCallback EVH ran into undefined function (%1)",_function];
		};
	};
}];

[
	"itemAdd",
	[
		"aasp_master_loop",
		{
			private _authorised = "ArmaSpotifyController" callExtension "authorised";
			private _premium = "ArmaSpotifyController" callExtension "spotify:premium";

			if (_premium == "true" && {_authorised == "true"}) then
			{
				private _display = uiNamespace getVariable ["aasp_spotify_display", displayNull];
				private _play = _display displayCtrl 1000;
				private _seek = _display displayCtrl 1100;

				private _delay = profilenamespace getVariable ['aasp_info_delay', 3];
				private _loop_interation = missionNamespace getVariable ["aasp_master_loop_iteration", 1];

				private _playing = (ctrlText _play) find "pause_ca.paa" > -1;

				private _slider_range = sliderRange _seek;
				private _new_position = sliderPosition _seek + ([0,1] select _playing);

				if (_loop_interation >= (_delay max 1) || _new_position > _slider_range#1) then
				{
					// Request update
					"ArmaSpotifyController" callExtension "spotify:request_info";

					// Reset loop iteration
					_loop_interation = 0;
				}
				else
				{
					// Update seek bar
					[_new_position, _slider_range#1, true] call spotify_fnc_set_playback;
				};

				// Increment the loop iteration number
				missionNamespace setVariable ["aasp_master_loop_iteration", _loop_interation + 1];

				if (uiNamespace getVariable ["aaps_background_sync", true]) then
				{
					"ArmaSpotifyController" callExtension "background_sync";
					uiNamespace setVariable ["aaps_background_sync",false];
				};
			};
		},
		1
	]
] call BIS_fnc_loop;
