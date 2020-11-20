class CfgPatches
{
	class functions_f_spotify
	{
		author = "Asaayu";
		name = "functions_f_spotify";
		url = "https://www.arma3.com";
		units[] = {};
		weapons[] = {};
		requiredVersion = 0.1;
		requiredAddons[] = { "A3_Data_F"};
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
		};
	};
};
