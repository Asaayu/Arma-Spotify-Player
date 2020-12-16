class recent_control_group: ctrlControlsGroupNoHScrollbars
{
	idc = 85000;
	show = 0;
	x = X(0.1) + W(0.8/8);
	y = Y(0.1) + HI(0.0125);
	w = W(0.8) - W(0.8/8);
	h = H(0.8) - H(0.8/10) - HI(0.0125);
	class controls
	{
		class title: ctrlStatic
		{
			text = "Recently Played";
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
		class recently_played_control_group: ctrlControlsGroup
		{
			idc = 500;
			x = W(0.01);
			y = LHX(0.5);
			w = W(0.69);
			h = LH;
			class controls {};
			class VScrollbar: ScrollBar
			{
				width = H(0);
			};
			class HScrollbar: ScrollBar
			{
				height = H(0);
			};
		};
	};
};
