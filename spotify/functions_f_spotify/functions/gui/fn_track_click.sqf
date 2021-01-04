params ["_ctrl", "_button", "_xPos", "_yPos", "_shift", "_ctrl", "_alt", ["_id", "", [""]], ["_index", -1, [-1]], ["_type", "track", [""]], ["_liked_playlist", false, [false]]];

if (_id == "") exitWith {};

#define LEFT 0
#define RIGHT 1
#define MIDDLE 2

switch true do
{
	// Left click
	case (_button == LEFT):
	{
		switch _type do
		{
			case "track":
			{
				"ArmaSpotifyController" callExtension format["spotify:play_track:%1",_id];
			};
			case "playlist":
			{
				if _liked_playlist then
				{
					// Liked playlists are not actually playlists
					// Therefore the require a work around to play a track from them
					"ArmaSpotifyController" callExtension format["spotify:play_track:%1",_id];
				}
				else
				{
					// Normal playlists can be done normally
					"ArmaSpotifyController" callExtension format["spotify:play_playlist:%1:%2",_id,_index];
				};
			};
			case "album":
			{
				"ArmaSpotifyController" callExtension format["spotify:play_album:%1:%2",_id,_index];
			};
		};
		playSound "click";
	};
};
false
