class home_control_group: ctrlControlsGroupNoHScrollbars
{
	idc = 69000;
	show = 0;
	x = X(0.1) + W(0.8/8);
	y = Y(0.1) + HI(0.0125);
	w = W(0.8) - W(0.8/8);
	h = H(0.8) - H(0.8/10) - HI(0.0125);
	class controls
	{
		class title: ctrlStatic
		{
			text = "Home";
			x = W(0.01);
			y = 0;
			w = W(0.67);
			h = H(0.05);
			sizeEx = H(0.05);
			color[] = {1,1,1,1};
			shadow = 0;
			font = "RobotoCondensedBold";
		};
		class spacer_01: ctrlStatic
		{
			x = W(0.01);
			y = LHX(0);
			w = W(0.67);
			h = H(0.0025);
			colorBackground[] = {0.2,0.2,0.2,1};
		};
		class recently_played_title: ctrlStatic
		{
			text = "Recently Played";
			x = W(0.01);
			y = LHX(0.25);
			w = W(0.67);
			h = LH*1.5;
			sizeEx = LH*1.5;
			shadow = 0;
			font = "RobotoCondensed";
		};
		class recently_played_control_group: ctrlControlsGroup
		{
			idc = 500;
			x = W(0.01);
			y = LHX(2);
			w = W(0.67);
			h = LH*12;
			class controls {};
			class HScrollbar: ScrollBar
			{
				height = H(0.01);
			};
		};
		class spacer_02: spacer_01
		{
			y = LHX(14);
		};
		class new_releases_title: recently_played_title
		{
			text = "New Releases";
			y = LHX(14.25);
		};
		class new_releases_control_group: recently_played_control_group
		{
			idc = 550;
			y = LHX(16);
		};
		class spacer_03: spacer_01
		{
			y = LHX(28);
		};
		class featured_playlists_title: recently_played_title
		{
			text = "Featured Playlists";
			y = LHX(28.25);
		};
		class featured_playlists_control_group: recently_played_control_group
		{
			idc = 600;
			y = LHX(30);
		};
	};
};
