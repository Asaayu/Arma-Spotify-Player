params
[
	["_display",displayNull,[displayNull]]
];

#define LOADED (uinamespace getVariable ['aasp_spotify_preloaded', false])

#define DISPLAY (uinamespace getVariable ['aasp_spotify_display', displayNull])

#define AUTHORISE_BUTTON (DISPLAY displayCtrl 15006)
#define CONNECT_BUTTON (DISPLAY displayCtrl 15009)

#define AUTHORISATION_OVERLAY	\
[ \
	15001, \
	15002, \
	15003, \
	15004, \
	15005, \
	15006, \
	15007, \
	15008, \
	15009, \
	15010, \
	15011, \
	15012 \
]

// The very first time loading the extension will run all the serup items
if !LOADED then
{
	"ArmaSpotifyController" callExtension "";
	uinamespace setVariable ['aasp_spotify_preloaded', true];
};


AUTHORISE_BUTTON ctrlAddEventHandler ["ButtonClick",
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
	"ArmaSpotifyController" callExtension "authorise_request";

	ctrlSetFocus (_display displayCtrl 15008);
}];

CONNECT_BUTTON ctrlAddEventHandler ["ButtonClick",
{
	private _display = (ctrlParent (_this#0));
	private _ctrl = _display displayCtrl 15008;

	private _text = ctrlText _ctrl;
	private _function = format['authorise_submit:%1', _text];
	[_display, _function] spawn
	{
		_this params ["_display", "_function"];
		private _return_01 = "ArmaSpotifyController" callExtension _function;

		private _error_ctrl = _display displayCtrl 15010;
		if (count _return_01 != 25 && {_return_01 select [0,6] == "ERROR:"}) exitWith
		{
			_error_ctrl ctrlSetText _return_01;
		};

		private _handle = [_error_ctrl] spawn
		{
			_this params ["_error_ctrl"];
			_error_ctrl ctrlSetTextColor [0,1,0,1];
			for "_i" from 0 to 5 do
			{
				private _add = ["",".","..","...","....","....."] select _i;
				_error_ctrl ctrlSetText format["Loading%1",_add];
				sleep 0.2;
			};
		};

		{
			private _ctrl = _display displayCtrl _x;
			_ctrl ctrlEnable false;
			_ctrl ctrlCommit 0;
		} foreach [15006,15008,15009];

		uisleep 1;

		private _return_02 = "ArmaSpotifyController" callExtension format["authorise:%1", _return_01];

		terminate _handle;
		if (_return_02 select [0,6] == "ERROR:") exitWith
		{
			_error_ctrl ctrlSetTextColor [1,0,0,1];
			_error_ctrl ctrlSetText _return_02;

			{
				private _ctrl = _display displayCtrl _x;
				_ctrl ctrlEnable true;
				_ctrl ctrlCommit 0;
			} foreach [15006,15008,15009];

			ctrlSetFocus (_display displayCtrl 15008);
		};

		_error_ctrl ctrlSetTextColor [0,1,0,1];
		_error_ctrl ctrlSetText _return_02;

	};
}];
