if (!is3DEN && {!dialog}) then
{
	// Stop people from spamming the API when quickly opening and closing the display
	private _last_open = missionNamespace getVariable ["aasp_last_open", 0];
	if (_last_open <= diag_tickTime) then
	{
		createDialog "AASP_spotify";
		missionNamespace setVariable ["aasp_last_open", diag_tickTime + 1];
	}
	else
	{
		systemChat "You need to wait at least a second between opening the menu.";
	};
};
