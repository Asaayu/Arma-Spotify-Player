class liked_control_group: ctrlControlsGroupNoHScrollbars
{
	idc = 86000;
	show = 0;
	x = X(0.1) + W(0.8/8);
	y = Y(0.1) + HI(0.0125);
	w = W(0.8) - W(0.8/8);
	h = H(0.8) - H(0.8/10) - HI(0.0125);
	class controls
	{
		class title: ctrlStatic
		{
			text = "Liked Songs";
			x = W(0.01);
			y = 0;
			w = W(0.67);
			h = H(0.05);
			sizeEx = H(0.05);
			color[] = {1,1,1,1};
			shadow = 0;
			font = "RobotoCondensedBold";
		};
		class title_01: ctrlStatic
		{
			text = "Title";
			x = W(0.01);
			y = LHX(0);
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
			y = LHX(1.25);
			w = W(0.67);
			h = H(0.0025);
			colorBackground[] = {0.3,0.3,0.3,1};
		};
		class liked_songs_control_group: ctrlControlsGroupNoHScrollbars
		{
			idc = 500;
			x = W(0.01);
			y = LHX(1.5);
			w = W(0.67);
			h = (LH * 28) + H(0.0025);
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
			onButtonClick ="private _ctrl = (ctrlParentControlsGroup (_this#0)) controlsGroupCtrl 500;       private _offset = _ctrl getVariable ['aasp_ctrl_index', 0];	private _variable = str round random 100000;        uiNamespace setVariable [_variable, _ctrl];       'ArmaSpotifyController' callExtension format['spotify:get_liked_main:%1:%2',_variable,(_offset-10)/3];";
		};
	};
};
