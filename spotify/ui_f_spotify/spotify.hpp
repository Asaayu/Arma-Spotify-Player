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
                        y = "0.5 - (safezoneH min (160 * (pixelH * pixelGridNoUIScale * 0.50))) * 0.5 + (125-(115/10)) * (pixelH * pixelGridNoUIScale * 0.50)";
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
                class play_button: ctrlActivePicture
                {
			idc = 1000;
			text = "A3\Ui_f\data\GUI\Rsc\RscDisplayArcadeMap\icon_exit_cross_ca.paa";
			x = "((getResolution select 2) * 0.5 * pixelW) + 97 * (pixelW * pixelGridNoUIScale * 0.50)";
			y = "0.5 - (safezoneH min (160 * (pixelH * pixelGridNoUIScale * 0.50))) * 0.5 + 10 * (pixelH * pixelGridNoUIScale * 0.50)";
			w = "3 * (pixelW * pixelGridNoUIScale * 0.50)";
			h = "3 * (pixelH * pixelGridNoUIScale * 0.50)";
			tooltip = "Close";
                        color[] = {1,1,1,0.15};
                        colorActive[] = {1,1,1,1};
		};
	};
};
