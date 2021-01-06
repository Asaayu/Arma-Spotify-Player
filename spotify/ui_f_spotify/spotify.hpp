class AASP_spotify
{
	idd = 57445;
	enablesimulation = 1;
	enabledisplay = 0;
	onLoad = "uinamespace setVariable ['aasp_spotify_display', _this#0]; _this spawn spotify_fnc_menu_onload;";
	onUnload = "uinamespace setVariable ['aasp_spotify_display', displayNull];";
	class controlsbackground
	{
		class background_main: ctrlStaticBackground
		{
			idc = 550;
			x = X(0.1);
			y = Y(0.1);
			w = W(0.8);
			h = H(0.8);
			colorBackground[] = {0.14,0.14,0.14,1};
		};
		class background_second: background_main {};
                class background_left: background_main
		{
			idc = 555;
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
			style = 2;
			x = X(0.1);
			y = Y(0.1);
                        w = W(0.8);
                        h = HI(0.0125);
                        sizeEx = HI(0.0125);
                        colorBackground[] = {0,0,0,0};
                        colorText[] = {1,1,1,0.1};
			shadow = 0;
			font = "RobotoCondensedBold";
			text = "All cover art and song metadata is supplied and made avalaible by Spotify.";
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
			x = X(0.9) - W(0.0125*2.2);
			tooltip = "Settings";
			onButtonClick = "call spotify_fnc_close_menus; private _options_group = (ctrlParent (_this#0)) displayCtrl 55000; if (ctrlShown _options_group) then { _options_group ctrlShow false; } else { _options_group ctrlShow true; };";
		};
                class spotify_button: settings_button
                {
			text = "\spotify\ui_f_spotify\data\spotify\icon_x32_white_ca.paa";
			x = X(0.9) - W(0.0125*3.4);
			tooltip = "Open Spotify";
			onButtonClick = "'ArmaSpotifyController' callExtension 'open_spotify'";
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
			onButtonClick = "[!(uiNamespace getVariable ['aasp_shuffle_mode',false]), true] call spotify_fnc_set_shuffle;";
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
		class song_icon: ctrlActivePicture
                {
			idc = 1500;
			text = "";
			tooltip = "Click to open the song in Spotify";
			x = X(0.1) + W(0.005);
			y = Y(0.82) + H(0.01);
			w = W(0.035);
			h = HI(0.035);
			color[] = {1,1,1,1};
			colorActive[] = {1,1,1,1};
		};
		class song_title_control_group: ctrlControlsGroupNoHScrollbars
		{
			idc = 1505;
			x = X(0.1) + W(0.045);
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

		//Left hande menu items
		class left_selection_text: ctrlListbox
                {
			idc = 8000;
			x = X(0.1);
			y = Y(0.1) + HI(0.0125);
			w = W(0.8/8);
			h = H(0.03*3);
			rowHeight = H(0.03);
			sizeEx = H(0.03);
			font = "RobotoCondensedBold";
			shadow = 0;
			colorBackground[] = {0,0,0,0};
			colorText[] = {1,1,1,0.7};
			colorTextActive[] = {1,1,1,1};
			onLBSelChanged = "_this call spotify_fnc_master_selection";
			class items
			{
				class home
				{
					text = "Home";
					picture = "\spotify\ui_f_spotify\data\icons\home_ca.paa";
				};
				class browse
				{
					text = "Browse";
					picture = "\spotify\ui_f_spotify\data\icons\browse_ca.paa";
				};
			};
		};
		class your_library: ctrlStatic
                {
			style = 2;
			text = "Your Library";
			x = X(0.1);
			y = Y(0.21) + HI(0.0125);
			w = W(0.8/8);
			h = H(0.02);
			sizeEx = H(0.02);
			font = "RobotoCondensedLight";
			colorText[] = {1,1,1,0.7};
		};
		class left_selection_listbox: left_selection_text
                {
			idc = 8010;
			x = X(0.1);
			y = Y(0.23) + HI(0.0125);
			w = W(0.8/8);
			h = H(0.03*7);
			rowHeight = H(0.03);
			sizeEx = H(0.02);
			font = "RobotoCondensed";
			onLBSelChanged = "_this call spotify_fnc_secondary_selection";
			class items
			{
				class recently_played
				{
					text = "Recently Played";
				};
				class liked_songs
				{
					text = "Liked Songs";
				};
				/*
				class albums
				{
					text = "Albums";
				};
				class artists
				{
					text = "Artists";
				};
				class podcasts
				{
					text = "Podcasts";
				};
				*/
			};
		};
		class playlists: your_library
                {
			text = "Playlists";
			y = Y(0.43) + HI(0.0125);
		};
		class playlist_selection_group: ctrlControlsGroupNoHScrollbars
		{
			idc = 8025;
			x = X(0.1);
			y = Y(0.45) + HI(0.0125);
			w = W(0.8/8);
			h = H(0.03*9);
		};
		class playlist_selection_load: ctrlButton
		{
			idc = 8030;
			show = 0;
			text = "Load More Playlists";
			x = X(0.1);
			y = Y(0.1) + H(0.8) - H(0.8/10) - H(0.06);
			w = W(0.8/8);
			h = H(0.03);
			sizeEx = H(0.02);
			colorBackground[] = {0.1,0.1,0.1,1};
			font = "RobotoCondensedBold";
			shadow = 0;
		};
		class create_playlist: playlist_selection_load
                {
			idc = -1;
			show = 0;
			text = "Create a new playlist";
			y = Y(0.1) + H(0.8) - H(0.8/10) - H(0.03);
		};

		// Right hand bottom buttons
		class party_button: ctrlActivePicture
                {
			idc = 1295;
			text = "\spotify\ui_f_spotify\data\icons\party_ca.paa";
			x = X(0.775);
			y = Y(0.86) - HI(0.015/2);
			w = W(0.015);
			h = HI(0.015);
			color[] = {1,1,1,0.7};
			colorActive[] = {1,1,1,1};
			tooltip = "Listen Along";
			onButtonClick = "call spotify_fnc_open_listen;";
		};
		class devices_button: party_button
                {
			idc = 1305;
			text = "\spotify\ui_f_spotify\data\icons\devices_ca.paa";
			x = X(0.775) + W(0.02625);
			tooltip = "Devices Button";
			onButtonClick = "private _display = ctrlParent (_this#0); ['button'] call spotify_fnc_get_devices;";
		};
		class volume_button: party_button
                {
			idc = 1310;
			text = "\spotify\ui_f_spotify\data\icons\volume_high_ca.paa";
			x = X(0.775) + W(0.0525);
			tooltip = "Volume Button";
			onButtonClick = "private _volume = uiNamespace getVariable ['aasp_volume_variable', 100]; if (_volume > 0) then { [0, true, true] call spotify_fnc_volume; } else { [uiNamespace getVariable ['aasp_volume_last', 100], true, true] call spotify_fnc_volume; }";
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
		class no_device_text: ctrlStatic
                {
			idc = 1306;
			style = 1;
			show = 0;
			text = "Click the devices button below to select a device to connect to.";
			x = X(0.6);
			y = Y(0.9) - H(0.8/10);
			w = W(0.3);
			h = H(0.025);
			font = "RobotoCondensedBold";
			sizeEx = H(0.02);
			colorBackground[] = {0.17,0.17,0.17,1};
		};
		class private_text: no_device_text
                {
			idc = 1307;
			text = "Current device is in a private session. Synchronization features will be unavalaible.";
		};

		// Liked menu
		#include "windows\liked.hpp"

		// Recent menu
		#include "windows\recent.hpp"

		// Home menu
		#include "windows\home.hpp"

		// Party menu
		#include "windows\party.hpp"

		// Options menu
		#include "windows\options.hpp"
	};
};
