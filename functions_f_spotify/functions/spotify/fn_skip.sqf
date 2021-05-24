params
[
	["_ctrl", controlNull, [controlNull]],
	["_direction", 0, [0]]
];
if (_direction == 0) exitWith {};

if (_direction > 0) then
{
	// Skip forewards
	"ArmaSpotifyController" callExtension "spotify:skip:next";
}
else
{
	private _last_click = _ctrl getVariable ["aasp_last_click", 0];
	if (_last_click < diag_tickTime) then
	{
		// Skip to the start of the track
		"ArmaSpotifyController" callExtension "spotify:seek:0";
		_ctrl setVariable ["aasp_last_click", diag_tickTime + 3];
	}
	else
	{
		// Skip backwards
		"ArmaSpotifyController" callExtension "spotify:skip:back";
	};
};
