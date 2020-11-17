class AASP_spotify
{
	idd = 57445;
	enablesimulation = 1;
	enabledisplay = 1;
	onLoad = "uinamespace setVariable ['aasp_spotify_display', _this#0]; _this spawn spotify_fnc_menu_onload;";
	class controlsbackground
	{
		class background_main: ctrlStaticBackground
		{
			x = "((getResolution select 2) * 0.5 * pixelW) - 100 * (pixelW * pixelGridNoUIScale * 0.50)";
			y = "0.5 - (safezoneH min (160 * (pixelH * pixelGridNoUIScale * 0.50))) * 0.5 + 10 * (pixelH * pixelGridNoUIScale * 0.50)";
			w = "200 * (pixelW * pixelGridNoUIScale * 0.50)";
			h = "115 * (pixelH * pixelGridNoUIScale * 0.50)";
			colorBackground[] = {0.14,0.14,0.14,1};
		};
		class background_second: background_main {};
                class background_left: background_main
		{
                        w = "(200/8) * (pixelW * pixelGridNoUIScale * 0.50)";
                        colorBackground[] = {0.1,0.1,0.1,1};
                };
                class background_bottom: background_main
		{
                        y = "0.5 - (safezoneH min (160 * (pixelH * pixelGridNoUIScale * 0.50))) * 0.5 + 113.5 * (pixelH * pixelGridNoUIScale * 0.50)";
                        h = "(115/10) * (pixelH * pixelGridNoUIScale *  0.50)";
                        colorBackground[] = {0.17,0.17,0.17,1};
                };
	};
	class controls
	{
                class exit_button: ctrlActivePicture
                {
			idc = 2;
			text = "A3\Ui_f\data\GUI\Rsc\RscDisplayArcadeMap\icon_exit_cross_ca.paa";
			x = "((getResolution select 2) * 0.5 * pixelW) + 97 * (pixelW * pixelGridNoUIScale * 0.50)";
			y = "0.5 - (safezoneH min (160 * (pixelH * pixelGridNoUIScale * 0.50))) * 0.5 + 10 * (pixelH * pixelGridNoUIScale * 0.50)";
			w = "3 * (pixelW * pixelGridNoUIScale * 0.50)";
			h = "3 * (pixelH * pixelGridNoUIScale * 0.50)";
			tooltip = "Close";
                        color[] = {1,1,1,0.15};
                        colorActive[] = {1,1,1,1};
		};

		// Bottom Box Buttons
                class play_button_background: ctrlStaticPicture
                {
			idc = 1005;
			text = "\spotify\ui_f_spotify\data\icons\circle_ca.paa";
			x = "((getResolution select 2) * 0.5 * pixelW) - 2 * (pixelW * pixelGridNoUIScale * 0.50)";
			y = "0.5 - (safezoneH min (160 * (pixelH * pixelGridNoUIScale * 0.50))) * 0.5 + (113.5 + 2) * (pixelH * pixelGridNoUIScale * 0.50)";
			w = "4 * (pixelW * pixelGridNoUIScale * 0.50)";
			h = "4 * (pixelH * pixelGridNoUIScale * 0.50)";
			colorText[] = {1,1,1,0.7};
		};
                class play_button: ctrlActivePicture
                {
			idc = 1000;
			text = "\spotify\ui_f_spotify\data\icons\play_ca.paa";
			x = "((getResolution select 2) * 0.5 * pixelW) - 1.5 * (pixelW * pixelGridNoUIScale * 0.50)";
			y = "0.5 - (safezoneH min (160 * (pixelH * pixelGridNoUIScale * 0.50))) * 0.5 + 116 * (pixelH * pixelGridNoUIScale * 0.50)";
			w = "3 * (pixelW * pixelGridNoUIScale * 0.50)";
			h = "3 * (pixelH * pixelGridNoUIScale * 0.50)";
			onMouseEnter = "[_this#0, true] call spotify_fnc_mouse_over";
			onMouseExit = "[_this#0, false] call spotify_fnc_mouse_over";
			onButtonClick = "_this call spotify_fnc_play_button";
			color[] = {1,1,1,0.7};
			colorActive[] = {1,1,1,1};
			tooltip = "Play Button";
		};
                class skip_start_button: play_button
                {
			idc = 1010;
			text = "\spotify\ui_f_spotify\data\icons\skip_start_ca.paa";
			x = "((getResolution select 2) * 0.5 * pixelW) - (1.5 + 4 + 1.5) * (pixelW * pixelGridNoUIScale * 0.50)";
			tooltip = "Previous";
			onButtonClick = "[_this#0, -1] call spotify_fnc_skip_button;";
		};
                class skip_end_button: skip_start_button
                {
			idc = 1015;
			text = "\spotify\ui_f_spotify\data\icons\skip_end_ca.paa";
			x = "((getResolution select 2) * 0.5 * pixelW) - (1.5 - 4 - 1.5) * (pixelW * pixelGridNoUIScale * 0.50)";
			tooltip = "Next";
			onButtonClick = "[_this#0, 1] call spotify_fnc_skip_button;";
		};
                class shuffle_button: skip_start_button
                {
			idc = 1020;
			text = "\spotify\ui_f_spotify\data\icons\shuffle_ca.paa";
			x = "((getResolution select 2) * 0.5 * pixelW) - (1.5 + 10 + 1.5) * (pixelW * pixelGridNoUIScale * 0.50)";
			tooltip = "Shuffle";
		};
                class repeat_button: skip_start_button
                {
			idc = 1025;
			text = "\spotify\ui_f_spotify\data\icons\repeat_ca.paa";
			x = "((getResolution select 2) * 0.5 * pixelW) - (1.5 - 10 - 1.5) * (pixelW * pixelGridNoUIScale * 0.50)";
			tooltip = "Repeat";
		};
		class shuffle_dot: play_button_background
                {
			idc = 1021;
			text = "\spotify\ui_f_spotify\data\icons\dot_ca.paa";
			x = "((getResolution select 2) * 0.5 * pixelW) - (1.5 + 10 + 1.5) * (pixelW * pixelGridNoUIScale * 0.50)";
			y = "0.5 - (safezoneH min (160 * (pixelH * pixelGridNoUIScale * 0.50))) * 0.5 + 119 * (pixelH * pixelGridNoUIScale * 0.50)";
			w = "3 * (pixelW * pixelGridNoUIScale * 0.50)";
			h = "3 * (pixelH * pixelGridNoUIScale * 0.50)";
		};
		class repeat_dot: shuffle_dot
                {
			idc = 1026;
			text = "\spotify\ui_f_spotify\data\icons\dot_ca.paa";
			x = "((getResolution select 2) * 0.5 * pixelW) - (1.5 - 10 - 1.5) * (pixelW * pixelGridNoUIScale * 0.50)";
		};

		// Bottom Box slider
		class playback_slider: ctrlXSliderH
                {
			idc = 1100;
			x = "((getResolution select 2) * 0.5 * pixelW) - 50 * (pixelW * pixelGridNoUIScale * 0.50)";
			y = "0.5 - (safezoneH min (160 * (pixelH * pixelGridNoUIScale * 0.50))) * 0.5 + 122 * (pixelH * pixelGridNoUIScale * 0.50)";
			w = "100 * (pixelW * pixelGridNoUIScale * 0.50)";
			h = "0.66 * (pixelH * pixelGridNoUIScale * 0.50)";
			color[] = {1,1,1,1};
            		coloractive[] = {1,1,1,1};

			// Slider specific
			arrowEmpty = "\a3\ui_f\data\igui\cfg\targeting\empty_ca.paa";
			arrowFull = "\a3\ui_f\data\igui\cfg\targeting\empty_ca.paa";
			border = "\spotify\ui_f_spotify\data\ui\slider_background_ca.paa";
            		thumb = "\spotify\ui_f_spotify\data\ui\slider_foreground_ca.paa";
			lineSize = 0;
			sliderRange[] = {0, 1};
			sliderPosition = 0;
		};

		class current_time: ctrlStatic
                {
			idc = 1210;
			style = 1;
			text = "0:00";
			x = "((getResolution select 2) * 0.5 * pixelW) - 59 * (pixelW * pixelGridNoUIScale * 0.50)";
			y = "0.5 - (safezoneH min (160 * (pixelH * pixelGridNoUIScale * 0.50))) * 0.5 + 121 * (pixelH * pixelGridNoUIScale * 0.50)";
			w = "10 * (pixelW * pixelGridNoUIScale * 0.50)";
			h = "2 * (pixelH * pixelGridNoUIScale * 0.50)";
			sizeEx = "2 * (pixelH * pixelGridNoUIScale *  0.50)";
			font = "RobotoCondensed";
			colorText[] = {1,1,1,0.7};
		};
		class total_time: current_time
                {
			idc = 1215;
			style = 0;
			text = "-0:00";
			x = "((getResolution select 2) * 0.5 * pixelW) + 49 * (pixelW * pixelGridNoUIScale * 0.50)";
		};

		// Left hand stuff
		class song_icon: ctrlStaticPicture
                {
			idc = 1500;
			text = "\spotify\ui_f_spotify\data\placeholder\512x_placeholder_co.paa";
			x = "((getResolution select 2) * 0.5 * pixelW) - 98.5 * (pixelW * pixelGridNoUIScale * 0.50)";
			y = "0.5 - (safezoneH min (160 * (pixelH * pixelGridNoUIScale * 0.50))) * 0.5 + 114.75 * (pixelH * pixelGridNoUIScale * 0.50)";
			w = "9 * (pixelW * pixelGridNoUIScale * 0.50)";
			h = "9 * (pixelH * pixelGridNoUIScale * 0.50)";
			color[] = {1,1,1,0.7};
			colorActive[] = {1,1,1,1};
			tooltip = "Song Icon";
		};
		class song_title: ctrlStatic
                {
			idc = 1505;
			text = "Song Title";
			x = "((getResolution select 2) * 0.5 * pixelW) - 88.5 * (pixelW * pixelGridNoUIScale * 0.50)";
			y = "0.5 - (safezoneH min (160 * (pixelH * pixelGridNoUIScale * 0.50))) * 0.5 + 115.75 * (pixelH * pixelGridNoUIScale * 0.50)";
			w = "27 * (pixelW * pixelGridNoUIScale * 0.50)";
			h = "2.5 * (pixelH * pixelGridNoUIScale * 0.50)";
			sizeEx = "2.1 * (pixelH * pixelGridNoUIScale *  0.50)";
			font = "Spotify";
			shadow= 0;
			colorText[] = {1,1,1,1};
			colorBackground[] = {1,0,0,0.4};
		};
		class song_author: song_title
                {
			idc = 1510;
			text = "Song Author";
			w = "30 * (pixelW * pixelGridNoUIScale * 0.50)";
			y = "0.5 - (safezoneH min (160 * (pixelH * pixelGridNoUIScale * 0.50))) * 0.5 + 119.75 * (pixelH * pixelGridNoUIScale * 0.50)";
			sizeEx = "1.9 * (pixelH * pixelGridNoUIScale *  0.50)";
			colorText[] = {0.7,0.7,0.7,1};
		};
		class song_like: ctrlActivePicture
                {
			idc = 1515;
			text = "\spotify\ui_f_spotify\data\icons\like_empty_ca.paa";
			x = "((getResolution select 2) * 0.5 * pixelW) - (88.5 - 26.25 - 1) * (pixelW * pixelGridNoUIScale * 0.50)";
			y = "0.5 - (safezoneH min (160 * (pixelH * pixelGridNoUIScale * 0.50))) * 0.5 + 115.75 * (pixelH * pixelGridNoUIScale * 0.50)";
			w = "2.5 * (pixelW * pixelGridNoUIScale * 0.50)";
			h = "2.5 * (pixelH * pixelGridNoUIScale * 0.50)";
			color[] = {1,1,1,0.7};
			colorActive[] = {1,1,1,0.7};
			tooltip = "Like Button";
		};

		// Right hand bottom buttons
		class playlist_button: ctrlActivePicture
                {
			idc = 1300;
			text = "\spotify\ui_f_spotify\data\icons\playlist_ca.paa";
			x = "((getResolution select 2) * 0.5 * pixelW) + 70 * (pixelW * pixelGridNoUIScale * 0.50)";
			y = "0.5 - (safezoneH min (160 * (pixelH * pixelGridNoUIScale * 0.50))) * 0.5 + 117.5 * (pixelH * pixelGridNoUIScale * 0.50)";
			w = "3 * (pixelW * pixelGridNoUIScale * 0.50)";
			h = "3 * (pixelH * pixelGridNoUIScale * 0.50)";
			color[] = {1,1,1,0.7};
			colorActive[] = {1,1,1,1};
			tooltip = "Playlist Button";
		};
		class devices_button: playlist_button
                {
			idc = 1305;
			text = "\spotify\ui_f_spotify\data\icons\devices_ca.paa";
			x = "((getResolution select 2) * 0.5 * pixelW) + 76 * (pixelW * pixelGridNoUIScale * 0.50)";
			tooltip = "Devices Button";
			onButtonClick = "private _display = ctrlParent (_this#0); ['button', [_display, _display displayCtrl 50000]] call spotify_fnc_get_devices;";
		};
		class volume_button: playlist_button
                {
			idc = 1310;
			text = "\spotify\ui_f_spotify\data\icons\volume_high_ca.paa";
			x = "((getResolution select 2) * 0.5 * pixelW) + 82 * (pixelW * pixelGridNoUIScale * 0.50)";
			tooltip = "Volume Button";
			onButtonClick = "private _volume = missionNamespace getVariable ['aasp_volume_variable', 100]; if (_volume > 0) then { [ctrlParent (_this#0), 0, true, true] call spotify_fnc_volume; } else { [ctrlParent (_this#0), missionNamespace getVariable ['aasp_volume_last', 100], true, true] call spotify_fnc_volume; }";
		};
		class volume_slider: playback_slider
                {
			idc = 1315;
			x = "((getResolution select 2) * 0.5 * pixelW) + 85 * (pixelW * pixelGridNoUIScale * 0.50)";
			y = "0.5 - (safezoneH min (160 * (pixelH * pixelGridNoUIScale * 0.50))) * 0.5 + 118.75 * (pixelH * pixelGridNoUIScale * 0.50)";
			w = "12.5 * (pixelW * pixelGridNoUIScale * 0.50)";
			h = "0.66 * (pixelH * pixelGridNoUIScale * 0.50)";
			color[] = {1,1,1,1};
            		coloractive[] = {1,1,1,1};
			onSliderPosChanged = "[ctrlParent (_this#0), _this#1, true] call spotify_fnc_volume;";
			sliderRange[] = {0, 100};
			sliderPosition = 100;
		};

		// Devices list
		class devices_control_group: ctrlControlsGroupNoHScrollbars
		{
			idc = 50000;
			show = 0;
			x = "((getResolution select 2) * 0.5 * pixelW) + (76 - 15) * (pixelW * pixelGridNoUIScale * 0.50)";
			y = "0.5 - (safezoneH min (160 * (pixelH * pixelGridNoUIScale * 0.50))) * 0.5 + (117 - 40) * (pixelH * pixelGridNoUIScale * 0.50)";
			w = "30 * (pixelW * pixelGridNoUIScale * 0.50)";
			h = "40 * (pixelH * pixelGridNoUIScale *  0.50)";
			class controls
			{
				class devices_background: ctrlStaticBackground
				{
					idc = 100;
					x = 0;
					y = 0;
					w = "30 * (pixelW * pixelGridNoUIScale * 0.50)";
					h = "40 * (pixelH * pixelGridNoUIScale * 0.50)";
					colorBackground[] = {0.2,0.2,0.2,1};
				};
				class devices_title: ctrlStatic
				{
					idc = 105;
					style = 2;
					text = "Connect to a device";
					x = 0;
					y = 0;
					w = "25.5 * (pixelW * pixelGridNoUIScale * 0.50)";
					h = "6 * (pixelH * pixelGridNoUIScale * 0.50)";
					sizeEx = "3 * (pixelH * pixelGridNoUIScale * 0.50)";
					font = "Spotify";
				};
				class connect_button: ctrlActivePicture
		                {
					idc = 106;
					text = "\spotify\ui_f_spotify\data\icons\unknown_ca.paa";
					x = "25.5 * (pixelW * pixelGridNoUIScale * 0.50)";
					y = "1.5 * (pixelH * pixelGridNoUIScale * 0.50)";
					w = "3 * (pixelW * pixelGridNoUIScale * 0.50)";
					h = "3 * (pixelH * pixelGridNoUIScale * 0.50)";
					color[] = {1,1,1,0.7};
					colorActive[] = {1,1,1,1};
					tooltip = "Spotify Connect Button";
					onButtonClick = "'ArmaSpotifyController' callExtension 'spotify:connect_website';";
				};
				class devices_list: ctrlListbox
				{
					idc = 110;
					x = 0;
					y = "6 * (pixelH * pixelGridNoUIScale * 0.50)";
					w = "30 * (pixelW * pixelGridNoUIScale * 0.50)";
					h = "34 * (pixelH * pixelGridNoUIScale * 0.50)";
					shadow = 0;
					rowHeight = "5 * (pixelH * pixelGridNoUIScale * 0.50)";
					sizeEx = "3 * (pixelH * pixelGridNoUIScale * 0.50)";
					onLBSelChanged = "['list', _this] call spotify_fnc_get_devices;";
				};
			};
		};
	};
};
