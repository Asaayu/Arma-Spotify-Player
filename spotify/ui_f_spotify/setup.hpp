class AASP_setup
{
	idd = 57450;
	enablesimulation = 1;
	enabledisplay = 1;
	onLoad = "uinamespace setVariable ['aasp_setup_display', _this#0]; _this spawn spotify_fnc_setup_onload;";
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
                class title_bar: ctrlStaticTitle
		{
                        style = 2;
			x = X(0.1);
			y = Y(0.1);
                        w = W(0.8);
                        h = HI(0.0125);
                        shadow = 0;
                        font = "RobotoCondensedBold";
                        text = "Asaayu's Arma Spotify Player";
                        sizeEx = HI(0.0125);
                        colorBackground[] = {"profilenamespace getvariable ['GUI_BCG_RGB_R',0.13]", "profilenamespace getvariable ['GUI_BCG_RGB_G',0.54]", "profilenamespace getvariable ['GUI_BCG_RGB_B',0.21]", 1};
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
                        colorActive[] = {1,1,1,1};
		};
                class spotify_logo: ctrlStaticPicture
                {
			text = "spotify\ui_f_spotify\data\spotify\logo_green_ca.paa";
			x = X(0.5) - W(0.25/2);
			y = Y(0.1125);
			w = W(0.25);
			h = HI(0.25/2);
                        colorBackground[] = {1,0,0,0.5};
		};
		class title: ctrlStatic
                {
                        style = 2;
			text = "Welcome to Asaayu's Arma Spotify Player";
			x = X(0.5) - W(0.8/2);
			y = Y(0.1125) + H(0.2);
			w = W(0.8);
			h = H(0.05);
                        shadow = 0;
                        font = "RobotoCondensedBold";
                        sizeEx = H(0.05);
		};
                class description: ctrlControlsGroupNoHScrollbars
                {
			idc = 15005;
			x = X(0.5) - W(0.8/2);
			y = Y(0.1125) + H(0.25);
			w = W(0.8);
			h = H(0.025*10);
			class controls
			{
				class description: ctrlStructuredText
		                {
					text = "Asaayu's Arma Spotify Player (AASP) is an in-game system to allow users to interact with their Spotify application through through Arma 3.<br/>In order for this mod to work you will need to have a <a href='https://www.spotify.com/premium/'>Spotify Premium</a> account, this is a requirement of the Spotify API.<br/>This mod is open source and <a href='https://github.com/Asaayu/Arma-Spotify-Player'>avaliable on GitHub</a>, along with <a href='https://github.com/Asaayu/Arma-Spotify-Player#legal-information'>important legal information.</a><br/><br/>This message has appeared because you have not authorised this mod to connect to your Spotify account.<br/>Once connected you'll be able to control and interact with your Spotify account and connected devices.<br/><br/>Click the button below to open a webpage to begin the authorisation process between this mod and your Spotify account.<br/>By authorising your Spotify account to Asaayu's Arma Spotify Player you are agreeing to the <a href='https://github.com/Asaayu/Arma-Spotify-Player/blob/main/EULA.md'>End-User License Agreement</a> and <a href='https://github.com/Asaayu/Arma-Spotify-Player/blob/main/PRIVACY-POLICY.md'>Privacy Policy</a>.<br/>If you do not want to agree to them <a href='https://steamcommunity.com/id/asaayu/'>unsubscribe from this mod on the Steam Workshop</a>.";
					x = 0;
					y = 0;
					w = W(0.8);
					h = H(0.2);
					shadow = 0;
		                        size = H(0.025);
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
			x = X(0.5) - W(0.2/2);
			y = Y(0.1125) + H(0.5);
			w = W(0.2);
			h = H(0.03);
                        shadow = 2;
                        font = "RobotoCondensedBold";
                        sizeEx = H(0.03);
			colorBackground[] = {"profilenamespace getvariable ['GUI_BCG_RGB_R',0.77]","profilenamespace getvariable ['GUI_BCG_RGB_G',0.51]","profilenamespace getvariable ['GUI_BCG_RGB_B',0.08]",1};
			colorBackgroundActive[] = {0,0,0,1};
			colorText[] = {1,1,1,1};
		};
		class access_token_title: title
                {
			idc = 15007;
			show = 0;
			text = "Once you've authorised the connection you will be given a unique identifier to input into the box below to finalize the connection.";
			y = Y(0.1125) + H(0.54);
			h = H(0.03);
                        sizeEx = H(0.03);
		};
		class access_token: ctrlEdit
                {
			idc = 15008;
			show = 0;
			text = "";
			x = X(0.5) - W(0.5/2);
			y = Y(0.1125) + H(0.58);
			w = W(0.5);
			h = H(0.03);
			sizeEx = H(0.03);
                        font = "RobotoCondensed";
		};
		class connect_button: authorise_button
                {
			idc = 15009;
			show = 0;
			text = "CONNECT";
			y = Y(0.1125) + H(0.63);
		};
		class connect_error: access_token_title
                {
			idc = 15010;
			show = 0;
			text = "";
			y = Y(0.1125) + H(0.68);
			colorText[] = {1,0,0,1};
		};
		class legal_footer_01: ctrlStatic
                {
			idc = 15011;
			style = 2;
			text = "Asaayu's Arma Spotify Player (AASP) is not affiliated, created by, nor endorsed by Spotify, Spotify AB, and Spotify Technology S.A.";
			x = X(0.5) - W(0.8/2);
			y = Y(0.9) - H(0.04);
			w = W(0.8);
			h = H(0.02);
			sizeEx = H(0.02);
                        shadow = 0;
                        font = "RobotoCondensedBold";
		};
		class legal_footer_02: legal_footer_01
                {
			idc = 15012;
			text = "All Spotify trademarks, service marks, trade names, logos, domain names, and any other features of the Spotify brand (“Spotify Brand Features”) are the sole property of Spotify or its licensors.";
			y = Y(0.9) - H(0.02);
		};
	};
};
