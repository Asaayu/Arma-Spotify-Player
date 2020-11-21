#define SW safezoneW
#define SH safezoneH
#define SX safezoneX
#define SY safezoneY

#define HI(VALUE) (VALUE * 1.75 * SH)

#define W(VALUE) (VALUE * SW)
#define H(VALUE) (VALUE * SH)
#define X(VALUE) (SX + VALUE * SW)
#define Y(VALUE) (SY + VALUE * SH)

class AASP_spotify
{
	idd = 57445;
	enablesimulation = 1;
	enabledisplay = 1;
	onLoad = "uinamespace setVariable ['aasp_spotify_display', _this#0]; _this spawn spotify_fnc_menu_onload;";
	onUnload = "uinamespace setVariable ['aasp_spotify_display', displayNull];";
	class controlsbackground
	{
		class background_main: ctrlStaticBackground
		{
			x = X(0.1);
			y = Y(0.1);
			w = W(0.8);
			h = H(0.8);
			colorBackground[] = {0.14,0.14,0.14,1};
		};
		class background_second: background_main {};
                class background_left: background_main
		{
                        w = W(0.8/8);
                        colorBackground[] = {0.1,0.1,0.1,1};
                };
                class background_bottom: background_main
		{
                        y = Y(0.9) - H(0.8/10);
                        h = H(0.8/10);
                        colorBackground[] = {0.17,0.17,0.17,1};
                };
		class title_bar: ctrlStaticTitle
		{
			x = X(0.1);
			y = Y(0.1);
                        w = W(0.8);
                        h = HI(0.0125);
                        colorBackground[] = {0,0,0,0};
                };
	};
	class controls
	{
                class exit_button: ctrlActivePicture
                {
			idc = 2;
			text = "A3\Ui_f\data\GUI\Rsc\RscDisplayArcadeMap\icon_exit_cross_ca.paa";
			x = X(0.9) - W(0.0125);
			y = Y(0.1);
			w = W(0.0125);
			h = HI(0.0125);
			tooltip = "Close";
                        color[] = {1,1,1,0.15};
                        colorActive[] = {1,1,1,1};
		};
                class settings_button: exit_button
                {
			idc = 50;
			text = "\spotify\ui_f_spotify\data\icons\gear_ca.paa";
			x = X(0.9) - W(0.0125*2);
			tooltip = "Settings";
			onButtonClick = "private _options_group = (ctrlParent (_this#0)) displayCtrl 55000; if (ctrlShown _options_group) then { _options_group ctrlShow false; } else { _options_group ctrlShow true; };";
		};

		// Bottom Box Buttons
                class play_button: ctrlActivePicture
                {
			idc = 1000;
			text = "\spotify\ui_f_spotify\data\icons\play_ca.paa";
			x = X(0.5) - W(0.0175/2);
			y = Y(0.832);
			w = W(0.0175);
			h = HI(0.0175);
			onButtonClick = "_this call spotify_fnc_play";
			color[] = {1,1,1,0.7};
			colorActive[] = {1,1,1,1};
			tooltip = "Play Button";
		};
                class skip_start_button: play_button
                {
			idc = 1010;
			text = "\spotify\ui_f_spotify\data\icons\skip_start_ca.paa";
			x = X(0.5) - W(0.02) - W(0.0125);
			y = Y(0.8375);
			w = W(0.0125);
			h = HI(0.0125);
			tooltip = "Previous";
			onButtonClick = "[_this#0, -1] call spotify_fnc_skip;";
		};
                class skip_end_button: skip_start_button
                {
			idc = 1015;
			text = "\spotify\ui_f_spotify\data\icons\skip_end_ca.paa";
			x = X(0.5) + W(0.02);
			tooltip = "Next";
			onButtonClick = "[_this#0, 1] call spotify_fnc_skip;";
		};
                class shuffle_button: skip_start_button
                {
			idc = 1020;
			text = "\spotify\ui_f_spotify\data\icons\shuffle_ca.paa";
			x = X(0.5) - W(0.04) - W(0.0125);
			onButtonClick = "[!(missionNamespace getVariable ['aasp_shuffle_mode',false]), true] call spotify_fnc_set_shuffle;";
			tooltip = "Shuffle";
		};
		class shuffle_dot: shuffle_button
                {
			idc = 1021;
			show = 0;
			text = "\spotify\ui_f_spotify\data\icons\dot_ca.paa";
			y = Y(0.8375) + HI(0.01);
			color[] = {1,1,1,0.7};
			colorActive[] = {1,1,1,0.7};
			onButtonClick = "";
			tooltip = "";
		};
                class repeat_button: skip_start_button
                {
			idc = 1025;
			text = "\spotify\ui_f_spotify\data\icons\repeat_ca.paa";
			x = X(0.5) + W(0.04);
			onButtonClick = "['',true,true] call spotify_fnc_set_repeat;";
			tooltip = "Repeat";
		};
		class repeat_dot: repeat_button
                {
			idc = 1026;
			show = 0;
			text = "\spotify\ui_f_spotify\data\icons\dot_ca.paa";
			y = Y(0.8375) + HI(0.01);
			color[] = {1,1,1,0.7};
			colorActive[] = {1,1,1,0.7};
			tooltip = "";
			onButtonClick = "";
		};

		// Bottom Box slider
		class playback_slider: ctrlXSliderH
                {
			idc = 1100;
			x = X(0.5) - W(0.2);
			y = Y(0.88);
			w = W(0.40);
			h = H(0.005);
			color[] = {1,1,1,1};
            		coloractive[] = {1,1,1,1};

			// Slider specific
			arrowEmpty = "\a3\ui_f\data\igui\cfg\targeting\empty_ca.paa";
			arrowFull = "\a3\ui_f\data\igui\cfg\targeting\empty_ca.paa";
			border = "\spotify\ui_f_spotify\data\ui\slider_background_ca.paa";
            		thumb = "\spotify\ui_f_spotify\data\ui\slider_foreground_ca.paa";
			lineSize = 0;
			sliderRange[] = {0, 1};
			onSliderPosChanged = "[_this#1, true] call spotify_fnc_seek;";
			sliderPosition = 0;
			onMouseButtonDown = "(_this#0) setVariable ['aasp_seeking', true];";
			onMouseButtonUp = "(_this#0) setVariable ['aasp_seeking', false];";
		};

		class current_time: ctrlStatic
                {
			idc = 1210;
			style = 1;
			text = "0:00";
			x = X(0.5) - W(0.2) - W(0.035);
			y = Y(0.87);
			w = W(0.04);
			h = H(0.02);
			sizeEx = H(0.02);
			font = "RobotoCondensed";
			color[] = {1,1,1,0.7};
			colorText[] = {1,1,1,0.7};
			colorActive[] = {1,1,1,0.7};
		};
		class total_time: current_time
                {
			idc = 1215;
			style = 0;
			text = "-0:00";
			x = X(0.5) + W(0.195);
		};

		// Left hand stuff
		class song_icon: ctrlStaticPicture
                {
			idc = 1500;
			text = "";
			x = X(0.1) + W(0.005);
			y = Y(0.82) + H(0.01);
			w = W(0.035);
			h = HI(0.035);
			color[] = {1,0,0,0.7};
			colorActive[] = {1,1,1,1};
		};
		class song_title_control_group: ctrlControlsGroupNoHScrollbars
		{
			idc = 1505;
			x = X(0.1) + W(0.0435);
			y = Y(0.82) + H(0.02);
			w = W(0.1025);
			h = H(0.0185);
			class controls
			{
				class song_title: ctrlActivePictureKeepAspect
		                {
					idc = 1000;
					text = "";
					x = 0;
					y = 0;
					w = W(0.1025);
					h = H(0.01835);
					onMouseEnter = "[_this#0, 0.1025 * safezoneW] spawn spotify_fnc_text_scroll;";
					color[] = {1,1,1,1};
					colorActive[] = {1,1,1,1};
				};
			};
		};
		class song_author_control_group: song_title_control_group
		{
			idc = 1510;
			y = Y(0.82) + H(0.043);
			w = W(0.12);
			h = H(0.02);
			class controls
			{
				class song_author: ctrlActivePictureKeepAspect
		                {
					idc = 1000;
					text = "";
					x = 0;
					y = H(0.005);
					w = W(0.12);
					h = H(0.0135);
					onMouseEnter = "[_this#0, 0.12 * safezoneW] spawn spotify_fnc_text_scroll;";
					color[] = {1,1,1,1};
					colorActive[] = {1,1,1,1};
				};
			};
		};
		class song_like: ctrlActivePicture
                {
			idc = 1515;
			show = 0;
			text = "\spotify\ui_f_spotify\data\icons\like_empty_ca.paa";
			x = X(0.14) + W(0.11);
			y = Y(0.8425);
			w = W(0.01835/2);
			h = HI(0.01835/2);
			color[] = {1,1,1,0.7};
			colorActive[] = {1,1,1,0.7};
			onButtonClick = "[ctrlText (_this#0) find 'like_empty_ca.paa' > -1, true, (_this#0) getVariable ['aasp_song_id', '']] call spotify_fnc_like";
			tooltip = "Like Button";
		};

		// Right hand bottom buttons
		class playlist_button: ctrlActivePicture
                {
			idc = 1300;
			text = "\spotify\ui_f_spotify\data\icons\playlist_ca.paa";
			x = X(0.775);
			y = Y(0.86) - HI(0.015/2);
			w = W(0.015);
			h = HI(0.015);
			color[] = {1,1,1,0.7};
			colorActive[] = {1,1,1,1};
			tooltip = "Playlist Button";
		};
		class devices_button: playlist_button
                {
			idc = 1305;
			text = "\spotify\ui_f_spotify\data\icons\devices_ca.paa";
			x = X(0.775) + W(0.02625);
			tooltip = "Devices Button";
			onButtonClick = "private _display = ctrlParent (_this#0); ['button', [_display, _display displayCtrl 50000]] call spotify_fnc_get_devices;";
		};
		class volume_button: playlist_button
                {
			idc = 1310;
			text = "\spotify\ui_f_spotify\data\icons\volume_high_ca.paa";
			x = X(0.775) + W(0.0525);
			tooltip = "Volume Button";
			onButtonClick = "private _volume = missionNamespace getVariable ['aasp_volume_variable', 100]; if (_volume > 0) then { [0, true, true] call spotify_fnc_volume; } else { [missionNamespace getVariable ['aasp_volume_last', 100], true, true] call spotify_fnc_volume; }";
		};
		class volume_slider: playback_slider
                {
			idc = 1315;
			x = X(0.8275) + W(0.014);
			y = Y(0.86) - H(0.00125);
			w = W(0.055);
			h = W(0.0035);
			color[] = {1,1,1,1};
            		coloractive[] = {1,1,1,1};
			onSliderPosChanged = "[_this#1, true, false, true] call spotify_fnc_volume;";
			sliderRange[] = {0, 100};
			sliderPosition = 100;
			onMouseButtonDown = "(_this#0) setVariable ['aasp_seeking', true];";
			onMouseButtonUp = "(_this#0) setVariable ['aasp_seeking', false];";
		};

		// Options menu
		//#include "windows\options.hpp"

		class no_device_text: ctrlStatic
                {
			idc = 1306;
			style = 2;
			show = 0;
			text = "Click the devices button at the bottom right to select a device to connect to.";
			x = X(0.5) - W(0.3);
			y = Y(0.83) - H(0.045);
			w = W(0.6);
			h = H(0.03);
			font = "RobotoCondensedBold";
			sizeEx = H(0.03);
		};

		// Options menu
		#include "windows\options.hpp"

		// Devices list
		#include "windows\devices.hpp"
	};
};
