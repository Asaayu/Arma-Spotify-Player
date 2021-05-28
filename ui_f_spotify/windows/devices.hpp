class devices_control_group: ctrlControlsGroupNoHScrollbars
{
	idc = 50000;
	show = 1;
	x = X(0.80875) - W(0.115/2);
	y = Y(0.8675) - H(0.2275);
	w = W(0.115);
	h = H(0.2);
	class controls
	{
		class devices_background: ctrlStaticBackground
		{
			idc = 100;
			x = 0;
			y = 0;
			w = W(0.115);
			h = H(0.2);
			colorBackground[] = {0.2,0.2,0.2,1};
		};
		class devices_title: ctrlStatic
		{
			idc = 105;
			style = 2;
			text = "Connect to a device";
			tooltip = "If the 'Arma Spotify Player' device does not appear in the list\nclick the options button at the top right of the Spotify window\nthen click the 'Restart Background Player' button";
			x = 0;
			y = 0;
			w = W(0.115) - W(0.015);
			h = H(0.275/10);
			sizeEx = H(0.225/10);
			color[] = {1,1,1,1};
			shadow = 0;
			font = "RobotoCondensed";
		};
		class connect_button: ctrlActivePicture
		{
			text = "\spotify\ui_f_spotify\data\icons\unknown_ca.paa";
			tooltip = "What is Spotify Connect?";
			x = W(0.115) - W(0.015);
			y = 0;
			w = W(0.015);
			h = HI(0.015);
			color[] = {1,1,1,0.7};
			colorActive[] = {1,1,1,1};
			onButtonClick = "'ArmaSpotifyController' callExtension 'spotify:connect_website';";
		};
		class devices_list: ctrlListbox
		{
			idc = 110;
			x = 0;
			y = H(0.275/10);
			w = W(0.115);
			h = H(0.199) - H(0.275/10);
			shadow = 0;
			rowHeight = H(0.225/6);
			sizeEx = H(0.225/12);
			onLBSelChanged = "['list', _this] call spotify_fnc_get_devices;";
		};
	};
};
