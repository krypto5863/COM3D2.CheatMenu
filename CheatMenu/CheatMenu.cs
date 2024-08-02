using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using COM3D2API;
using System;
using System.Security;
using System.Security.Permissions;
using UnityEngine;

//These two lines tell your plugin to not give a flying fuck about accessing private variables/classes whatever. It requires a publicized stub of the library with those private objects though.
[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace CheatMenu
{
	//This is the metadata set for your plugin.
	[BepInPlugin("CheatMenu", "CheatMenu", "1.2")]
	public class CheatMenu : BaseUnityPlugin
	{
		//static saving of the main instance. This makes it easier to run stuff like co-routines from static methods or accessing non-static vars.
		public static CheatMenu Instance { get; private set; }

		//Static var for the logger so you can log from other classes.
		internal static ManualLogSource PluginLogger => Instance.Logger;

		internal static ConfigEntry<bool> HotKeyEnabled;
		internal static ConfigEntry<KeyboardShortcut> HotKey;

		internal static bool DrawUi;

		private const string Base64Icon = "iVBORw0KGgoAAAANSUhEUgAAABwAAAAcCAYAAAByDd + UAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsIAAA7CARUoSoAAAAPgSURBVEhLvVZLSFRRGP6stDJfVKAV2JQQRAxMG2sh2CYilVrkuA0iiFoEbYypRY9FGbUrJilwp0FFQagUBUVKTq / N2ANdFBpl0QONMsrq9H33v3dmqInsMX3wcf7z33PPf885//ffkwegiJxLlpNzyJnkFPJf4Cs5Tr4hn5OvSSyMRCLRZDJ53+UIfX19dxSDsUIKuEIO/1nOoAUplrZ0Lfvdipxr5OXl1eusCq37XzBTAf9VgkwGU1LBOjo6tOSsjMVi/qjJo6WlJfV+V1eX7+W2ktHOzs7TDQ0NwLZtwLx5wIwZwOXLwNGjwLVrwJYt4BjU1NRgbGwMvb29Xqt+ZWVlyifIV1pairKyMuDwYeDGDeD8eYyOjsrXpDFKV4emJoeREYfNm43V1Q5PnzqcPeuwdKnD/PnOGycuWmQsKbF+fn7aN326+crLHZiNOHTIs/kR8iuWHzAWc7hwIT2pqIAvXzp0d9vLjY2OX+u4JIdLl8xXVeXQ3OyQTDpcueIwMeGwYIFDImHPnzwxu6BAc27MmjAal8K5c0BdHXDzJrgnQDgM7NwJrFkDTgQsXgy0tQFbtwL799s7Iep75Urg/XvgyBHPrqmu1pPxyWeo8quIVXDqVOD4ceDzZ+DjR6C/38771Clg/Xpg2jQKzVeaAmoc0dPT47XpgOMsefn5nqnM+gFMBG8yBVFyyV61ilWSZTISAerrgX37/ME+3r3zDX0jP5LwAtZr8NWrtl3aTnH7dm9ACgoo3LoFxOM2ZngY+PQJTGHwDIHbt21MAG35sWPAjh2pgEJ0aGhIh2acNcsY9P+Q4XDY7MJCr43H42qjng6ZJKfZ2lbu3SsD2LMHOHgQePwYOHEC7e3tnt6kO7GfZzfMFUpz0p4QPBeCZ+qrFTi/6ZABPdB23FqHt2/Nlu/kSbNDIdNZRYX1A92JxcXmW7LE2gybfwmbnGA/mj1Li4uB2bP9DqFzuHgRaG0FiwNQWws8egTq1ls9PxBcCjAwAKxbZxkte/lyr2RmIntApfKmTTaRsGwZWHFMe2fOgGI3v6SwejXw7JmNESYmrPWhrc9E9oAPH4Klzs5PkPZ27wa+fLGV3Ltn/gAFBabRSSB7wAcPTFtKe0HaO3DAAquCSBqZCCQjifwCP1+hisDgoPUVYNcupgDPPfiITPgFA9evg3XXqo8PysO30khlKdM7nXFFRdYG/T/k91kqHW6gzYwwSEvfI9DR34I6bFTAOgZM/5JzCAas0xm+ZuoyS3KLRCJxl40uxAjpkprLu6nupP5FeKG2VALSFb+CVHnRzyx79v4+dNX/QOqK/wLAq2/dvP1xxbJvLwAAAABJRU5ErkJggg==";

		private void Awake()
		{
			//Useful for engaging co-routines or accessing variables non-static variables. Completely optional though.
			Instance = this;

			HotKeyEnabled = Config.Bind("HotKey", "1. Enable HotKey", false, "Use a hotkey to open CheatMenu.");
			HotKey = Config.Bind("HotKey", "2. HotKey", new KeyboardShortcut(KeyCode.C, KeyCode.LeftControl, KeyCode.LeftAlt), "HotKey to open CheatMenu with.");

			SystemShortcutAPI.AddButton("CheatMenu", () =>
			{
				DrawUi = !DrawUi;
			}, "Open CheatMenu UI", Convert.FromBase64String(Base64Icon));
		}

		private void Update()
		{
			if (HotKeyEnabled.Value && HotKey.Value.IsDown())
			{
				DrawUi = !DrawUi;
			}
		}

		private void OnGUI()
		{
			if (DrawUi)
			{
				UserInterface.Gui.StartGui();
			}
		}
	}
}