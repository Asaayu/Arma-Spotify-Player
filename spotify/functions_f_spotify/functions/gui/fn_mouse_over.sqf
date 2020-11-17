params
[
	["_ctrl",controlNull,[controlNull]],
	["_over",false,[false]]
];

private _display = ctrlParent _ctrl;

private _play_button_background = _display displayCtrl 1005;
private _play_button = _display displayCtrl 1000;

// Only the play button/background image should call this function
if !(_ctrl in [_play_button,_play_button_background]) exitWith {};


if _over then
{
	_play_button_background ctrlSetTextColor [1,1,1,1];
	_play_button ctrlSetTextColor [1,1,1,1];
}
else
{
	_play_button_background ctrlSetTextColor [1,1,1,0.7];
	_play_button ctrlSetTextColor [1,1,1,0.7];
};
