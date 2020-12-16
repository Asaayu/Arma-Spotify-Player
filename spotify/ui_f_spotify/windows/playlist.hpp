class playlist_control_group: ctrlControlsGroupNoHScrollbars
{
	idc = 87000;
	x = X(0.1) + W(0.8/8);
	y = Y(0.1) + HI(0.0125);
	w = W(0.8) - W(0.8/8);
	h = H(0.8) - H(0.8/10) - HI(0.0125);
	class controls
	{
		class image: ctrlStaticPicture
		{
			idc = 50;
			text = "";
			x = W(0.01);
			y = 0;
			w = W(0.1);
			h = HI(0.1);
		};
		class title_control_group: ctrlControlsGroupNoHScrollbars
		{
			idc = 100;
			x = W(0.11);
			y = HI(0.1) - H(0.055);
			w = W(0.57);
			h = H(0.035);
			class controls
			{
				class title: ctrlStaticPicture
				{
					idc = 105;
					text = "";
					x = 0;
					y = 0;
					w = W(0.57);
					h = H(0.035);
				};
			};
		};
		class subtitle_control_group: ctrlControlsGroupNoHScrollbars
		{
			idc = 150;
			x = W(0.11);
			y = HI(0.1) - H(0.02);
			w = W(0.57);
			h = H(0.02);
			class controls
			{
				class subtitle: ctrlStaticPicture
				{
					idc = 155;
					text = "";
					x = 0;
					y = 0;
					w = W(0.57);
					h = H(0.02);
				};
			};
		};
		class title_01: ctrlStatic
		{
			text = "Title";
			x = W(0.01);
			y = LHX(6);
			w = W(0.35);
			h = LH;
			sizeEx = LH;
			font = "RobotoCondensedBold";
			shadow = 0;
			colorBackground[] = {0,0,0,0};
		};
		class title_02: title_01
		{
			text = "Artist";
			x = W(0.36);
			w = W(0.15);
		};
		class title_03: title_01
		{
			text = "Album";
			x = W(0.52);
			w = W(0.13);
		};
		class spacer_01: ctrlStatic
		{
			x = W(0.01);
			y = LHX(7.25);
			w = W(0.67);
			h = H(0.0025);
			colorBackground[] = {0.3,0.3,0.3,1};
		};
		class liked_songs_control_group: ctrlControlsGroupNoHScrollbars
		{
			idc = 500;
			x = W(0.01);
			y = LHX(7.5);
			w = W(0.67);
			h = (LH * 22) + H(0.0025);
			class controls {};
		};
		class load_button: ctrlButton
                {
			idc = 1000;
			text = "Load More Songs";
			x = W(0.335) - W(0.1/2);
			y = LHX(30);
			w = W(0.1);
			h = LH;
                        shadow = 2;
                        font = "RobotoCondensedBold";
                        sizeEx = LH;
			colorBackground[] = {"profilenamespace getvariable ['GUI_BCG_RGB_R',0.77]","profilenamespace getvariable ['GUI_BCG_RGB_G',0.51]","profilenamespace getvariable ['GUI_BCG_RGB_B',0.08]",1};
			colorBackgroundActive[] = {0,0,0,1};
			colorText[] = {1,1,1,1};
			onButtonClick ="private _ctrl = (ctrlParentControlsGroup (_this#0)) controlsGroupCtrl 500;       private _id = _ctrl getVariable ['aasp_playlist_id', ''];	private _offset = _ctrl getVariable ['aasp_ctrl_index', 0];       private _variable = str round random 100000;        uiNamespace setVariable [_variable, _ctrl];       'ArmaSpotifyController' callExtension format['spotify:LoadPlaylist:%1:%2:%3',_variable,_id,(_offset-10)/3];";
		};
		class play_button: load_button
                {
			idc = 200;
			text = "Play Playlist";
			x = W(0.01);
			onButtonClick ="'ArmaSpotifyController' callExtension format['spotify:play_%1:%2', 'playlist', (_this#0) getVariable ['aasp_playlist_id','']];";
		};
	};
};
