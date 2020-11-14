class AASP_premium
{
	idd = 57455;
	enablesimulation = 1;
	enabledisplay = 1;
	onLoad = "uinamespace setVariable ['aasp_premium_display', _this#0];";
	class controlsbackground
	{
		class authorise_background: ctrlStaticBackground
		{
			x = "((getResolution select 2) * 0.5 * pixelW) - 100 * (pixelW * pixelGridNoUIScale * 0.50)";
			y = "0.5 - (safezoneH min (160 * (pixelH * pixelGridNoUIScale * 0.50))) * 0.5 + 10 * (pixelH * pixelGridNoUIScale * 0.50)";
			w = "200 * (pixelW * pixelGridNoUIScale * 0.50)";
			h = "115 * (pixelH * pixelGridNoUIScale *  0.50)";
			colorBackground[] = {0.2,0.2,0.2,1};
		};
                class title_bar: ctrlStaticTitle
		{
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
                        colorActive[] = {1,1,1,1};
		};
                class spotify_logo: ctrlStaticPicture
                {
			text = "spotify\ui_f_spotify\data\spotify\logo_green_ca.paa";
			x = "((getResolution select 2) * 0.5 * pixelW) - 25 * (pixelW * pixelGridNoUIScale * 0.50)";
			y = "0.5 - (safezoneH min (160 * (pixelH * pixelGridNoUIScale * 0.50))) * 0.5 + 13 * (pixelH * pixelGridNoUIScale * 0.50)";
			w = "50 * (pixelW * pixelGridNoUIScale * 0.50)";
			h = "25 * (pixelH * pixelGridNoUIScale * 0.50)";
                        colorBackground[] = {1,0,0,0.5};
		};
                class title: ctrlStatic
                {
                        style = 2;
			text = "Spotify Premium Required";
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
			x = "((getResolution select 2) * 0.5 * pixelW) - 100 * (pixelW * pixelGridNoUIScale * 0.50)";
			y = "0.5 - (safezoneH min (160 * (pixelH * pixelGridNoUIScale * 0.50))) * 0.5 + 47 * (pixelH * pixelGridNoUIScale * 0.50)";
                        w = "200 * (pixelW * pixelGridNoUIScale * 0.50)";
			h = "25 * (pixelH * pixelGridNoUIScale * 0.50)";
			class controls
			{
				class description: ctrlStructuredText
		                {
					text = "This mod has successfully connected to your Spotify account but has not been able to confirm your <a href='https://www.spotify.com/premium/'>Spotify Premium</a> subscription.<br/>In order for this mod to work and due to Spotify API requirements you need to have a <a href='https://www.spotify.com/premium/'>Spotify Premium</a> subscription.<br/><br/><br/>Click the button below to open the <a href='https://www.spotify.com/premium/'>Spotify Premium</a> webpage to upgrade your account's subscription.";
					x = 0;
					y = 0;
		                        w = "200 * (pixelW * pixelGridNoUIScale * 0.50)";
					h = "25 * (pixelH * pixelGridNoUIScale * 0.50)";
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
		class premium_button: ctrlButton
                {
			idc = 15006;
			text = "Spotify Premium";
			x = "((getResolution select 2) * 0.5 * pixelW) - 25 * (pixelW * pixelGridNoUIScale * 0.50)";
			y = "0.5 - (safezoneH min (160 * (pixelH * pixelGridNoUIScale * 0.50))) * 0.5 + 72 * (pixelH * pixelGridNoUIScale * 0.50)";
			w = "50 * (pixelW * pixelGridNoUIScale * 0.50)";
			h = "5 * (pixelH * pixelGridNoUIScale * 0.50)";
                        shadow = 2;
                        font = "RobotoCondensedBold";
                        sizeEx = "5 * (pixelH * pixelGridNoUIScale * 0.50)";
			colorBackground[] = {"profilenamespace getvariable ['GUI_BCG_RGB_R',0.77]","profilenamespace getvariable ['GUI_BCG_RGB_G',0.51]","profilenamespace getvariable ['GUI_BCG_RGB_B',0.08]",1};
			colorBackgroundActive[] = {0,0,0,1};
			colorText[] = {1,1,1,1};
			onButtonClick = "'ArmaSpotifyController' callExtension 'premium_website'";
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
