class options_control_group: ctrlControlsGroupNoHScrollbars
{
	idc = 55000;
	show = 0;
	x = X(0.1) + W(0.8/8);
	y = Y(0.1) + HI(0.0125);
	w = W(0.8) - W(0.8/8);
	h = H(0.8) - H(0.8/10);
	class controls
	{
		class options_background: ctrlStaticBackground
		{
			x = 0;
			y = 0;
			w = W(0.8) - W(0.8/8);
			h = H(0.8) - H(0.8/10) - HI(0.0125);
			colorBackground[] = {0.14,0.14,0.14,1};
		};
		class devices_title: ctrlStatic
		{
			text = "Settings";
			x = W(0.1);
			y = 0;
			w = W(0.45);
			h = H(0.05);
			sizeEx = H(0.05);
			color[] = {1,1,1,1};
			shadow = 0;
			font = "RobotoCondensedBold";
		};
		class github_button: ctrlActivePicture
		{
			text = "\spotify\ui_f_spotify\data\icons\github_ca.paa";
			x = W(0.55) - W(0.05);
			y = H(0.02);
			w = W(0.05);
			h = HI(0.0125);
			tooltip = "Click to open the mod's GitHub page in your browser.";
			onButtonClick = "'ArmaSpotifyController' callExtension 'github';";
		};
		class spacer_01: ctrlStatic
		{
			x = W(0.1);
			y = H(0.06);
			w = W(0.45);
			h = H(0.0025);
			colorBackground[] = {0.2,0.2,0.2,1};
		};
		class data_location_text: ctrlStatic
		{
			style = 1;
			text = "Cache location:";
			tooltip = "This is the location where images will be downloaded to.\nIn the future you may be able to change this file location.";
			x = W(0.1);
			y = H(0.0825);
			w = W(0.2);
			h = H(0.02);
			sizeEx = H(0.02);
			font = "RobotoCondensedBold";
		};
		class data_location_edit: ctrlEdit
		{
			style = 512;
			text = "";
			canModify = 0;
			x = W(0.1) + W(0.2);
			y = H(0.0825);
			w = W(0.25);
			h = H(0.02);
			sizeEx = H(0.02);
			onLoad = "(_this#0) ctrlSetText ('ArmaSpotifyController' callExtension 'data');";
		};
		class clear_cache: ctrlButton
		{
			text = "Clear Cache";
			tooltip = "Delete all files in the cache directory.";
			x = W(0.6/2) - W(0.08);
			y = H(0.1225);
			w = W(0.075);
			h = H(0.03);
			sizeEx = H(0.02);
			font = "RobotoCondensedBold";
			onButtonClick = "'ArmaSpotifyController' callExtension 'clear_cache';";
		};
		class open_cache: clear_cache
		{
			text = "Open Cache";
			tooltip = "Open the cache direcotry in the file browser.";
			x = W(0.6/2) + W(0.005);
			onButtonClick = "'ArmaSpotifyController' callExtension 'open_cache';";
		};
		class spacer_02: spacer_01
		{
			y = H(0.1725);
		};
		class log_location_text: data_location_text
		{
			text = "Log location:";
			tooltip = "This is the location where log files will be created.\nIn the future you may be able to change this file location.";
			y = H(0.1925);
		};
		class log_location_edit: data_location_edit
		{
			y = H(0.1925);
			onLoad = "(_this#0) ctrlSetText ('ArmaSpotifyController' callExtension 'log');";
		};
		class clear_logs: clear_cache
		{
			text = "Clear Logs";
			tooltip = "Delete all files in the log directory.\nDoes not delete the current log file.";
			y = H(0.2325);
			onButtonClick = "'ArmaSpotifyController' callExtension 'clear_logs';";
		};
		class open_logs: clear_logs
		{
			text = "Open Logs";
			tooltip = "Open the log direcotry in the file browser.";
			x = W(0.6/2) + W(0.005);
			onButtonClick = "'ArmaSpotifyController' callExtension 'open_logs';";
		};
		class spacer_03: spacer_01
		{
			y = H(0.2825);
		};
		class info_delay: log_location_text
		{
			text = "Sync Request Delay:";
			tooltip = "How often your client sends a request to resync with your Spotify client.";
			y = H(0.3025);
		};
		class info_delay_combo: ctrlCombo
		{
			x = W(0.1) + W(0.2);
			y = H(0.3025);
			w = W(0.25);
			h = H(0.02);
			sizeEx = H(0.02);
			onLoad = "for '_i' from 1 to 10 do { private _index = (_this#0) lbAdd format['%1s', _i]; (_this#0) lbSetValue [_index, _i]; if ((profilenamespace getVariable ['aasp_info_delay', 3]) == _i) then {(_this#0) lbSetCurSel _i} };";
			onLBSelChanged = "profilenamespace setVariable ['aasp_info_delay',(_this#0) lbValue (_this#1)]; saveProfileNamespace;";
		};
	};
};
