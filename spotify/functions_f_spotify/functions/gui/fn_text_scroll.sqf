params
[
	["_ctrl", controlNull , [controlNull]],
	["_visable", 0 , [0]]
];

#define NULL_CHECK if (isNull _ctrl) exitWith {};
#define CHANGE_CHECK if !(_ctrl getVariable ["aasp_text_scroll", false]) exitWith { _ctrl ctrlSetPositionX (_position select 0); _ctrl ctrlCommit 0; };

if (isNull _ctrl || _visable <= 0 || _ctrl getVariable ["aasp_text_scroll", false]) exitWith {};

private _position = ctrlPosition _ctrl;
if ((_position#2) < (_visable + (0.0025*safezoneW))) exitWith {};

_ctrl setVariable ["aasp_text_scroll", true];

_ctrl ctrlSetPositionX (_position#0) - ((_position#2) - _visable);
_ctrl ctrlCommit (_position#2)/(0.075*safezoneW);

waitUntil {uisleep 0.1; ctrlCommitted _ctrl || isNull _ctrl};
NULL_CHECK
CHANGE_CHECK

for "_i" from 1 to 50 do
{
	uisleep 0.02;
	CHANGE_CHECK
};
CHANGE_CHECK

_ctrl ctrlSetPositionX (_position#0);
_ctrl ctrlCommit ((_position#2)/(0.075*safezoneW))/2;

waitUntil {uisleep 0.1; ctrlCommitted _ctrl || isNull _ctrl};
CHANGE_CHECK
_ctrl setVariable ["aasp_text_scroll", false];
