class aasp_notification
{
	idd = -1;
	fadein = 0;
	fadeout = 0;
	duration = 6;
	class controls
	{
		class aasp_notification_control_group: ctrlControlsGroupNoHScrollbars
		{
			x = X(0);
			y = Y(0);
			w = W(0.15);
			h = H(0.1);
			show = 0;
			onLoad = "_this call spotify_fnc_notification;";
			class HScrollbar: ScrollBar
			{
				height = 0;
			};
			class VScrollbar: ScrollBar
			{
				width = 0;
			};
			class controls
			{
				class background: ctrlStatic
				{
					text = "";
					x = 0;
					y = 0;
					w = W(0.15);
					h = H(0.1);
					colorBackground[] = {0.17,0.17,0.17,1};
				};
				class title: ctrlStatic
				{
					style = 2;
					text = "NOW PLAYING";
					x = 0;
					y = 0;
					w = W(0.15);
					h = H(0.025);
					sizeEx = H(0.025);
					font = "RobotoCondensedBold";
					shadow = 0;
					colorText[] = {1,1,1,1};
					colorBackground[] = {"profilenamespace getvariable ['GUI_BCG_RGB_R',0.13]", "profilenamespace getvariable ['GUI_BCG_RGB_G',0.54]", "profilenamespace getvariable ['GUI_BCG_RGB_B',0.21]", 1};
				};
				class spotify_logo: ctrlStaticPicture
				{
					text = "\spotify\ui_f_spotify\data\spotify\icon_x32_white_ca.paa";
					x = W(0.15) - W(0.025/2);
					y = H(0.001);
					w = W(0.025/2);
					h = HI(0.025/2);
					colorText[] = {1,1,1,1};
					colorBackground[] = {0,0,0,0};
				};
				class song_image: ctrlStaticPicture
				{
					idc = 500;
					text = "";
					x = W(0.0025);
					y = H(0.03);
					w = W(0.0375);
					h = HI(0.0375);
					colorText[] = {1,1,1,1};
				};
				class song_title: ctrlControlsGroupNoHScrollbars
				{
					idc = 750;
					x = W(0.0425);
					y = H(0.0675) - H(0.03);
					w = W(0.1);
					h = H(0.0175);
					class controls
					{
						class song_title_image: ctrlStaticPictureKeepAspect
						{
							idc = 751;
							text = "";
							x = 0;
							y = 0;
							w = W(0.1);
							h = H(0.0175);
							colorText[] = {1,1,1,1};
						};
					};
					class HScrollbar: ScrollBar
					{
						height = 0;
					};
					class VScrollbar: ScrollBar
					{
						width = 0;
					};
				};
				class song_artist: song_title
				{
					idc = 1000;
					y = H(0.0675);
					h = H(0.015);
					class controls
					{
						class song_artist_image: ctrlStaticPictureKeepAspect
						{
							idc = 1001;
							text = "";
							x = 0;
							y = 0;
							w = W(0.1);
							h = H(0.015);
							colorText[] = {1,1,1,1};
						};
					};
				};


			};
		};
	};
};
