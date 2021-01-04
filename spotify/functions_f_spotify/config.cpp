class CfgPatches
{
	class functions_f_spotify
	{
		authors[]= {"Asaayu"};
		author = "Asaayu";
		name = "Asaayu's Arma Spotify Player - Functions";
		url = "https://github.com/Asaayu/Arma-Spotify-Player";
		units[] = {};
		weapons[] = {};
		requiredVersion = 0.1;
		requiredAddons[] = { "A3_Data_F", "main_f_spotify"};
	};
};

class Extended_PreInit_EventHandlers
{
	class aasp_init_event
	{
		init = "call compile preprocessFileLineNumbers '\spotify\functions_f_spotify\XEH_preInit.sqf'";
	};
};

class CfgFunctions
{
	class functions_f_spotify
	{
		tag = "spotify";
		class spotify_internal
		{
		        file = "\spotify\functions_f_spotify\functions";
			class preinit { preinit = 1; };
		};
		class spotify_api_functions
		{
		        file = "\spotify\functions_f_spotify\functions\spotify";
			class get_devices {};

			class set_playback {};
			class set_shuffle {};
			class set_repeat {};

			class update_display {};
			class update_song_info {};

			class like {};
			class skip {};
			class play {};
			class volume {};
			class seek {};
		};
		class spotify_gui_functions
		{
		        file = "\spotify\functions_f_spotify\functions\gui";
			class menu_onload {};
			class setup_onload {};
			class text_scroll {};

			class track_click {};

			class open_home {};
			class open_listen {};
			class open_recent {};
			class open_liked {};
			class open_playlist {};
			class open_album {};

			class close_menus {};

			class master_selection {};
			class secondary_selection {};

			class gui {};
			class notification {};
		};
	};
};
