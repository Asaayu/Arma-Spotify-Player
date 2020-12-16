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
			text = "This system is currently in BETA and will contain bugs, be missing features and will probably break unexpectedly.";
			x = W(0.1);
			y = LHX(1);
			w = W(0.45);
			h = LH;
			sizeEx = LH;
			font = "RobotoCondensedBold";
		};
		class description_02: description_01
		{
			text = "This feature is not currently implemented.";
			y = LHX(3);
		};
	};
};
