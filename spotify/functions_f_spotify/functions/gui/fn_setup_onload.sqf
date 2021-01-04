params
[
	["_display",displayNull,[displayNull]]
];

#define LOADED (uinamespace getVariable ['aasp_spotify_preloaded', false])

// The very first time loading the extension will run all the serup items
if !LOADED then
{
	"ArmaSpotifyController" callExtension "init_load";
	uinamespace setVariable ['aasp_spotify_preloaded', true];
};

private _legal_shown = profileNamespace getVariable ["aasp_legal_shown", false];
if !_legal_shown then
{
	["By authorising your Spotify account to Asaayu's Arma Spotify Player you are agreeing to the <a href='https://github.com/Asaayu/Arma-Spotify-Player/blob/main/EULA.md'>End User License Agreement</a> and <a href='https://github.com/Asaayu/Arma-Spotify-Player/blob/main/PRIVACY-POLICY.md'>Privacy Policy</a>.<br/>If you do not want to agree to them <a href='https://steamcommunity.com/id/asaayu/'>unsubscribe from this mod on the Steam Workshop</a>.", "Important Information", "I understand", false] spawn BIS_fnc_guiMessage;

	profileNamespace setVariable ["aasp_legal_shown", true];
};

(_display displayCtrl 15006) ctrlAddEventHandler ["ButtonClick",
{
	"ArmaSpotifyController" callExtension "authorise_website";
}];
