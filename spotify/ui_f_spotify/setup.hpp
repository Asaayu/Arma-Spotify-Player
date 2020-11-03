class AASP_spotify
{
	idd = 57450;
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
                class authorise_background: ctrlStaticBackground
		{
			idc = 15001;
			x = "((getResolution select 2) * 0.5 * pixelW) - 100 * (pixelW * pixelGridNoUIScale * 0.50)";
			y = "0.5 - (safezoneH min (160 * (pixelH * pixelGridNoUIScale * 0.50))) * 0.5 + 10 * (pixelH * pixelGridNoUIScale * 0.50)";
			w = "200 * (pixelW * pixelGridNoUIScale * 0.50)";
			h = "115 * (pixelH * pixelGridNoUIScale *  0.50)";
			colorBackground[] = {0.2,0.2,0.2,1};
		};
                class title_bar: ctrlStaticTitle
		{
			idc = 15002;
                        style = 2;
			x = "((getResolution select 2) * 0.5 * pixelW) - 100 * (pixelW * pixelGridNoUIScale * 0.50)";
			y = "0.5 - (safezoneH min (160 * (pixelH * pixelGridNoUIScale * 0.50))) * 0.5 + 10 * (pixelH * pixelGridNoUIScale * 0.50)";
                        w = "200 * (pixelW * pixelGridNoUIScale * 0.50)";
                        h = "3 * (pixelH * pixelGridNoUIScale *  0.50)";
                        shadow = 0;
                        font = "RobotoCondensedBold";
                        text = "Asaayu's Arma Spotify Player";
                        sizeEx = "3 * (pixelH * pixelGridNoUIScale *  0.50)";
                        colorBackground[] = {"profilenamespace getvariable ['GUI_BCG_RGB_R',0.13]", "profilenamespace getvariable ['GUI_BCG_RGB_G',0.54]", "profilenamespace getvariable ['GUI_BCG_RGB_B',0.21]", 1};
                };
                class exit_button: ctrlActivePicture
                {
			idc = 2;
			text = "A3\Ui_f\data\GUI\Rsc\RscDisplayArcadeMap\icon_exit_cross_ca.paa";
			x = "((getResolution select 2) * 0.5 * pixelW) + 97 * (pixelW * pixelGridNoUIScale * 0.50)";
			y = "0.5 - (safezoneH min (160 * (pixelH * pixelGridNoUIScale * 0.50))) * 0.5 + 10 * (pixelH * pixelGridNoUIScale * 0.50)";
			w = "3 * (pixelW * pixelGridNoUIScale * 0.50)";
			h = "3 * (pixelH * pixelGridNoUIScale * 0.50)";
			tooltip = "Close";
                        colorActive[] = {1,1,1,1};
		};
                class spotify_logo: ctrlStaticPicture
                {
			idc = 15003;
			text = "spotify\ui_f_spotify\data\spotify\logo_green_ca.paa";
			tooltip = "spotify\ui_f_spotify\data\spotify\logo_green_ca.paa";
			x = "((getResolution select 2) * 0.5 * pixelW) - 25 * (pixelW * pixelGridNoUIScale * 0.50)";
			y = "0.5 - (safezoneH min (160 * (pixelH * pixelGridNoUIScale * 0.50))) * 0.5 + 13 * (pixelH * pixelGridNoUIScale * 0.50)";
			w = "50 * (pixelW * pixelGridNoUIScale * 0.50)";
			h = "25 * (pixelH * pixelGridNoUIScale * 0.50)";
                        colorBackground[] = {1,0,0,0.5};
                        colorActive[] = {1,1,1,1};
		};
                class title: ctrlStatic
                {
			idc = 15004;
                        style = 2;
			text = "Welcome to Asaayu's Arma Spotify Player.";
			x = "((getResolution select 2) * 0.5 * pixelW) - 100 * (pixelW * pixelGridNoUIScale * 0.50)";
			y = "0.5 - (safezoneH min (160 * (pixelH * pixelGridNoUIScale * 0.50))) * 0.5 + 38 * (pixelH * pixelGridNoUIScale * 0.50)";
                        w = "200 * (pixelW * pixelGridNoUIScale * 0.50)";
                        h = "6 * (pixelH * pixelGridNoUIScale *  0.50)";
                        shadow = 0;
                        font = "RobotoCondensedBold";
                        sizeEx = "6 * (pixelH * pixelGridNoUIScale * 0.50)";
		};
                class description: ctrlControlsGroupNoHScrollbars
                {
			idc = 15005;
			x = "((getResolution select 2) * 0.5 * pixelW) - 100 * (pixelW * pixelGridNoUIScale * 0.50)";
			y = "0.5 - (safezoneH min (160 * (pixelH * pixelGridNoUIScale * 0.50))) * 0.5 + 47 * (pixelH * pixelGridNoUIScale * 0.50)";
                        w = "200 * (pixelW * pixelGridNoUIScale * 0.50)";
			h = "35 * (pixelH * pixelGridNoUIScale * 0.50)";
			class controls
			{
				class description: ctrlStructuredText
		                {
					text = "Asaayu's Arma Spotify Player (AASP) is an in-game system to allow users to interact with their Spotify application through through Arma 3.<br/>In order for this mod to work you will need to have a <a href='https://www.spotify.com/premium/'>Spotify Premium</a> account, this is a requirement of the Spotify API.<br/>This mod is open source and <a href='https://github.com/Asaayu/Arma-Spotify-Player'>avaliable on GitHub</a>, along with <a href='https://github.com/Asaayu/Arma-Spotify-Player#legal-information'>important legal information.</a><br/><br/>This message has appeared because you have not authorised this mod to connect to your Spotify account.<br/>Once connected you'll be able to control and interact with your Spotify account and connected devices.<br/><br/>Click the button below to open a webpage to begin the authorisation process between this mod and your Spotify account.";
					x = 0;
					y = 0;
		                        w = "200 * (pixelW * pixelGridNoUIScale * 0.50)";
					h = "35 * (pixelH * pixelGridNoUIScale * 0.50)";
					shadow = 0;
		                        size = "4 * (pixelH * pixelGridNoUIScale * 0.50)";
					onload = "[(_this#0)] call BIS_fnc_ctrlFitToTextHeight";
		                        class Attributes
		                        {
		                                align = "center";
		                                color = "#ffffff";
		                                colorLink = "#1DB954";
		                                font = "RobotoCondensed";
		                                size = 1;
		                        };
				};
			};
			class VScrollBar
			{
				color[] = {1, 1, 1, 0.6};
				colorActive[] = {1, 1, 1, 1};
				colorDisabled[] = {1, 1, 1, 0.3};
				thumb = "\A3\ui_f\data\gui\cfg\scrollbar\thumb_ca.paa";
				arrowEmpty = "\A3\ui_f\data\gui\cfg\scrollbar\arrowEmpty_ca.paa";
				arrowFull = "\A3\ui_f\data\gui\cfg\scrollbar\arrowFull_ca.paa";
				border = "\A3\ui_f\data\gui\cfg\scrollbar\border_ca.paa";
				shadow = 0;
				scrollSpeed = 0.06;
				width = "4 * (pixelH * pixelGridNoUIScale * 0.50)";
				height = 0;
				autoScrollEnabled = 0;
				autoScrollDelay = 1;
				autoScrollRewind = 1;
				autoScrollSpeed = 1;
			};
		};
		class authorise_button: ctrlButton
                {
			idc = 15006;
			text = "AUTHORISE";
			x = "((getResolution select 2) * 0.5 * pixelW) - 25 * (pixelW * pixelGridNoUIScale * 0.50)";
			y = "0.5 - (safezoneH min (160 * (pixelH * pixelGridNoUIScale * 0.50))) * 0.5 + 84 * (pixelH * pixelGridNoUIScale * 0.50)";
			w = "50 * (pixelW * pixelGridNoUIScale * 0.50)";
			h = "5 * (pixelH * pixelGridNoUIScale * 0.50)";
                        shadow = 2;
                        font = "RobotoCondensedBold";
                        sizeEx = "5 * (pixelH * pixelGridNoUIScale * 0.50)";
			colorBackground[] = {"profilenamespace getvariable ['GUI_BCG_RGB_R',0.77]","profilenamespace getvariable ['GUI_BCG_RGB_G',0.51]","profilenamespace getvariable ['GUI_BCG_RGB_B',0.08]",1};
			colorBackgroundActive[] = {0,0,0,1};
			colorText[] = {1,1,1,1};
		};
		class access_token_title: title
                {
			idc = 15007;
			show = 0;
			text = "Once you've authorised the connection you will be given a unique identifier to input into the box below to finalize the connection.";
			y = "0.5 - (safezoneH min (160 * (pixelH * pixelGridNoUIScale * 0.50))) * 0.5 + 91 * (pixelH * pixelGridNoUIScale * 0.50)";
			h = "4 * (pixelH * pixelGridNoUIScale * 0.50)";
                        sizeEx = "4 * (pixelH * pixelGridNoUIScale * 0.50)";
		};
		class access_token: ctrlEdit
                {
			idc = 15008;
			show = 0;
			text = "";
			x = "((getResolution select 2) * 0.5 * pixelW) - 65 * (pixelW * pixelGridNoUIScale * 0.50)";
			y = "0.5 - (safezoneH min (160 * (pixelH * pixelGridNoUIScale * 0.50))) * 0.5 + 95 * (pixelH * pixelGridNoUIScale * 0.50)";
			w = "130 * (pixelW * pixelGridNoUIScale * 0.50)";
			h = "4 * (pixelH * pixelGridNoUIScale * 0.50)";
			sizeEx = "4 * (pixelH * pixelGridNoUIScale * 0.50)";
                        font = "RobotoCondensed";
		};
		class connect_button: authorise_button
                {
			idc = 15009;
			show = 0;
			text = "CONNECT";
			y = "0.5 - (safezoneH min (160 * (pixelH * pixelGridNoUIScale * 0.50))) * 0.5 + 101 * (pixelH * pixelGridNoUIScale * 0.50)";
		};
		class connect_error: access_token_title
                {
			idc = 15010;
			text = "";
			y = "0.5 - (safezoneH min (160 * (pixelH * pixelGridNoUIScale * 0.50))) * 0.5 + 109 * (pixelH * pixelGridNoUIScale * 0.50)";
			colorText[] = {1,0,0,1};
		};
		class legal_footer_01: ctrlStatic
                {
			idc = 15011;
			style = 2;
			text = "Asaayu's Arma Spotify Player (AASP) is not affiliated, created by, nor endorsed by Spotify, Spotify AB, and Spotify Technology S.A.";
			x = "((getResolution select 2) * 0.5 * pixelW) - 100 * (pixelW * pixelGridNoUIScale * 0.50)";
			y = "0.5 - (safezoneH min (160 * (pixelH * pixelGridNoUIScale * 0.50))) * 0.5 + 119 * (pixelH * pixelGridNoUIScale * 0.50)";
			w = "200 * (pixelW * pixelGridNoUIScale * 0.50)";
			h = "3 * (pixelH * pixelGridNoUIScale * 0.50)";
			sizeEx = "3 * (pixelH * pixelGridNoUIScale * 0.50)";
                        shadow = 0;
                        font = "RobotoCondensedBold";
		};
		class legal_footer_02: legal_footer_01
                {
			idc = 15012;
			text = "All Spotify trademarks, service marks, trade names, logos, domain names, and any other features of the Spotify brand (“Spotify Brand Features”) are the sole property of Spotify or its licensors.";
			y = "0.5 - (safezoneH min (160 * (pixelH * pixelGridNoUIScale * 0.50))) * 0.5 + 122 * (pixelH * pixelGridNoUIScale * 0.50)";
		};
	};
};
