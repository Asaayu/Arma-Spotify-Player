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
	// Skip backwards
	"ArmaSpotifyController" callExtension "spotify:skip:back";
};
