params
[
	["_ctrl", controlnull,[controlnull]],
	["_index", -1,[-1]]
];

// Non-user actions should not set off this functions
if (_ctrl getVariable ["aasp_auto_selection",false]) exitWith {};
if (_index <= -1) exitWith {};

switch _index do
{
	// Recently Played
	case 0:
	{
		call spotify_fnc_open_recent;
	};
	// Liked
	case 1:
	{
		call spotify_fnc_open_liked;
	};
};
