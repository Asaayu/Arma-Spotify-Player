class party_control_group: ctrlControlsGroupNoHScrollbars
{
	idc = 75000;
	show = 0;
	x = X(0.1) + W(0.8/8);
	y = Y(0.1) + HI(0.0125);
	w = W(0.8) - W(0.8/8);
	h = H(0.8) - H(0.8/10) - HI(0.0125);
	class controls
	{
		class title: ctrlStatic
		{
			text = "Listen Along";
			x = W(0.1);
			y = 0;
			w = W(0.45);
			h = H(0.05);
			sizeEx = H(0.05);
			color[] = {1,1,1,1};
			shadow = 0;
			font = "RobotoCondensedBold";
		};
		class spacer_01: ctrlStatic
		{
			x = W(0.1);
			y = H(0.06);
			w = W(0.45);
			h = H(0.0025);
			colorBackground[] = {0.2,0.2,0.2,1};
		};
		class description_01: ctrlStatic
		{
			text = "Listen Along allows you to listen along with your friends who also have this mod enabled.";
			x = W(0.1);
			y = LHX(1);
			w = W(0.45);
			h = LH;
			sizeEx = LH;
			font = "RobotoCondensedBold";
		};
		class description_02: description_01
		{
			text = "All audio is played through Spotify, therefore users will need to have a Spotify Premium subscription";
			y = LHX(2);
		};
		class description_03: description_01
		{
			text = "and will need to authorise the connection between their Spotify account and the mod.";
			y = LHX(3);
		};
		class blocker: description_01
		{
			text = "This feature is not currently available.";
			y = LHX(5);
		};
	};
};
