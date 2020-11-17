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

(_display displayCtrl 15006) ctrlAddEventHandler ["ButtonClick",
{
	private _display = (ctrlParent (_this#0));
	for "_i" from 15007 to 15010 do
	{
		private _ctrl = _display displayCtrl _i;
		_ctrl ctrlShow true;
		if (_i == 15010) then
		{
			_ctrl ctrlSetText "";
		};
		_ctrl ctrlCommit 0;
	};
	"ArmaSpotifyController" callExtension "authorise_website";

	ctrlSetFocus (_display displayCtrl 15008);
}];

(_display displayCtrl 15009) ctrlAddEventHandler ["ButtonClick",
{
	private _display = (ctrlParent (_this#0));
	private _ctrl = _display displayCtrl 15008;

	private _text = ctrlText _ctrl;
	private _function = format['authorise:%1', _text];
	[_display, _function] spawn
	{
		_this params ["_display", "_function"];
		private _return = "ArmaSpotifyController" callExtension _function;

		private _error_ctrl = _display displayCtrl 15010;
		if (count _return != 25 && {_return select [0,6] == "ERROR:"}) exitWith
		{
			_error_ctrl ctrlSetText _return;
		};

		if (_return select [0,6] == "ERROR:") exitWith
		{
			_error_ctrl ctrlSetTextColor [1,0,0,1];
			_error_ctrl ctrlSetText _return;

			ctrlSetFocus (_display displayCtrl 15008);
		};

		private _handle = [_error_ctrl] spawn
		{
			_this params ["_error_ctrl"];
			_error_ctrl ctrlSetTextColor [0,1,0,1];
			while {true} do
			{
				for "_i" from 0 to 5 do
				{
					private _add = ["",".","..","...","....","....."] select _i;
					_error_ctrl ctrlSetText format["Loading%1",_add];
					sleep 0.2;
				};
			};
		};

		// Log start of authorisation
		"ArmaSpotifyController" callExtension "error:Starting authorisation";

		// Wait until authorization has completed
		waitUntil {uisleep 0.5; missionNamespace getVariable ["aasp_authorised", false]};
		missionNamespace setVariable ["aasp_authorised", false];

		// Log end of authorisation
		"ArmaSpotifyController" callExtension "error:Authorisation successful";

		// Log start of info saving
		"ArmaSpotifyController" callExtension "error:Saving user info";

		// Wait until authorization has completed
		waitUntil {uisleep 0.5; missionNamespace getVariable ["aasp_info_saved", false]};
		missionNamespace setVariable ["aasp_info_saved", false];

		// Log end of info saving
		"ArmaSpotifyController" callExtension "error:User info saved successfully";

		// Stop loading text
		terminate _handle;

		// Now that the user is authorised, check if they have premium
		private _master = displayParent _display;
		_display closeDisplay 2;
		if (("ArmaSpotifyController" callExtension "spotify:premium") != "true") then
		{
			_master createDisplay "AASP_premium";
		}
		else
		{
			_master createDisplay "AASP_spotify";
		};
	};
}];
