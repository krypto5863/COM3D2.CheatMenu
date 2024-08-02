using System.Runtime.Serialization;
using MaidStatus;
using UnityEngine;
using Math = System.Math;
using Status = PlayerStatus.Status;

namespace CheatMenu.UserInterface
{
	public static class Gui
	{
		private static bool _ranOnce;

		private const int WindowId = 112323718;
		internal static Rect WindowRect;
		private static Rect _dragWindow = new Rect(0, 0, 10000, 20);
		private static Rect _closeButton = new Rect(0, 0, 25, 15);
		private static Vector2 _scrollPosition;

		//internal static GUIStyle Separator;
		internal static GUIStyle MainWindow;

		internal static GUIStyle Sections;
		internal static GUIStyle Sections2;

		private static int _currentHeight;
		private static int _currentWidth;

		private static string _currentGuid = string.Empty;

		internal static void StartGui()
		{
			//Setup some UI properties.
			if (_ranOnce == false)
			{
				/*
				Separator = new GUIStyle(GUI.skin.horizontalSlider)
				{
					fixedHeight = 1f,
					normal =
					{
						background = UiToolbox.MakeTex(2, 2, new Color(0, 0, 0, 0.8f))
					},
					margin =
					{
						top = 10,
						bottom = 10
					}
				};
				*/

				MainWindow = new GUIStyle(GUI.skin.window)
				{
					normal =
					{
						background = UiToolbox.MakeWindowTex(new Color(0, 0, 0, 0.05f), new Color(0, 0, 0, 0.5f)),
						textColor = new Color(1, 1, 1, 0.05f)
					},
					hover =
					{
						background = UiToolbox.MakeWindowTex(new Color(0.3f, 0.3f, 0.3f, 0.3f), new Color(0, 1, 1, 0.5f)),
						textColor = new Color(1, 1, 1, 0.3f)
					},
					onNormal =
					{
						background = UiToolbox.MakeWindowTex(new Color(0.3f, 0.3f, 0.3f, 0.6f), new Color(0, 1, 1, 0.5f))
					}
				};

				Sections = new GUIStyle(GUI.skin.box)
				{
					normal =
					{
						background = UiToolbox.MakeTex(2, 2, new Color(0, 0, 0, 0.3f))
					}
				};

				Sections2 = new GUIStyle(GUI.skin.box)
				{
					normal =
					{
						background = UiToolbox.MakeTexWithRoundedCorner(new Color(0, 0, 0, 0.8f))
					}
				};

				_ranOnce = true;
			}

			//Sometimes the UI can be improperly sized, this sets it to some measurements.
			if (_currentHeight != Screen.height || _currentWidth != Screen.width)
			{
				WindowRect.height = Math.Max(Screen.height / 2f, 540);
				WindowRect.width = Math.Max(Screen.width / 4f, 270);

				WindowRect.y = Screen.height / 4f;
				WindowRect.x = Screen.width / 3f;

				_currentHeight = Screen.height;
				_currentWidth = Screen.width;
			}

			WindowRect = GUILayout.Window(WindowId, WindowRect, GuiWindowControls, "CheatMenu", MainWindow);
		}

		private static void GuiWindowControls(int id)
		{
			_closeButton.x = WindowRect.width - (_closeButton.width + 5);
			_dragWindow.width = WindowRect.width - (_closeButton.width + 5);

			GUI.DragWindow(_dragWindow);

			if (GUI.Button(_closeButton, "X"))
			{
				CheatMenu.DrawUi = false;
			}

			_scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

			GUILayout.BeginVertical();

			GameMain.instance.CharacterMgr.status.playerName = UiToolbox.LabeledField("Player Name", GameMain.instance.CharacterMgr.status.playerName);

			GUILayout.BeginHorizontal();
			GameMain.instance.CharacterMgr.status.money = UiToolbox.NumberField(GameMain.instance.CharacterMgr.status.money, "Money", max: Status.MoneyMax);
			GUILayout.FlexibleSpace();
			GameMain.instance.CharacterMgr.status.clubGauge = UiToolbox.NumberField(GameMain.instance.CharacterMgr.status.clubGauge, "Club Gauge");
			GUILayout.FlexibleSpace();
			GameMain.instance.CharacterMgr.status.clubGrade = UiToolbox.NumberField(GameMain.instance.CharacterMgr.status.clubGrade, "Club Grade", max: Status.ClubGradeMax);
			GUILayout.EndHorizontal();

			if (Trophy.commonIdManager != null && GUILayout.Button("Unlock All Trophies"))
			{
				UnlockAllTrophies();
			}

			if (GameMain.Instance.CharacterMgr.status.lockNTRPlay)
			{
				GUILayout.Label("NTR Blocked: Yes (can disable with event)");
			}
			else
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label("NTR Blocked: No ಠ_ಠ");
				if (GUILayout.Button("Block NTR"))
				{
					GameMain.Instance.CharacterMgr.status.lockNTRPlay = true;
					try
					{
						DisplayFakeTrophy("A cuck no more");
					}
					catch
					{
						//Joke failed. Ignore.
					}
				}
				GUILayout.EndHorizontal();
			}

			GUILayout.EndVertical();

			GUILayout.BeginVertical(Sections2);

			foreach (var maid in GameMain.instance.CharacterMgr.GetStockMaidList())
			{
				GUILayout.BeginHorizontal(Sections2);
				GUILayout.Label(maid.status.fullNameEnStyle);
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("☰"))
				{
					_currentGuid = _currentGuid == maid.status.guid ? string.Empty : maid.status.guid;
				}
				GUILayout.EndHorizontal();

				if (_currentGuid.Equals(maid.status.guid) == false)
				{
					continue;
				}

				GUILayout.BeginHorizontal(Sections);
				maid.status.firstName = UiToolbox.LabeledField("First Name", maid.status.firstName);
				maid.status.lastName = UiToolbox.LabeledField("Last Name", maid.status.lastName);
				maid.status.nickName = UiToolbox.LabeledField("Nickname", maid.status.nickName);
				maid.status.isNickNameCall = GUILayout.Toggle(maid.status.isNickNameCall, "Use Nickname");
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal(Sections);
				GUILayout.Label("Trainee: " + (maid.status.studyRate > 500 ? "Yes" : "No" + (maid.status.contract == Contract.Trainee ? " (contract change next morning!)" : string.Empty)));
				if (maid.status.studyRate > 500)
				{
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Complete Training"))
					{
						maid.status.studyRate = 500;
					}
				}
				GUILayout.EndHorizontal();

				/*
				GUILayout.BeginHorizontal(Sections);
				if (GUILayout.Button("Max Out Current Job Class"))
				{
					maid.status.selectedJobClass.expSystem.SetLevel(maid.status.selectedJobClass.expSystem
						.GetMaxLevel());
				}
				if (GUILayout.Button("Max Out Current Yotogi Class"))
				{
					maid.status.selectedYotogiClass.expSystem.SetLevel(maid.status.selectedYotogiClass.expSystem
						.GetMaxLevel());
				}
				GUILayout.EndHorizontal();
				*/

				GUILayout.BeginVertical(Sections);
				maid.status.baseLovely = UiToolbox.NumberField(maid.status.baseLovely, "Lovely", 0, int.MaxValue);
				maid.status.baseElegance = UiToolbox.NumberField(maid.status.baseElegance, "Elegance", 0, int.MaxValue);
				maid.status.baseCharm = UiToolbox.NumberField(maid.status.baseCharm, "Charm", 0, int.MaxValue);
				maid.status.baseCare = UiToolbox.NumberField(maid.status.baseCare, "Care", 0, int.MaxValue);
				maid.status.baseReception = UiToolbox.NumberField(maid.status.baseReception, "Company", 0, int.MaxValue);
				maid.status.baseCooking = UiToolbox.NumberField(maid.status.baseCooking, "Cooking", 0, int.MaxValue);
				maid.status.baseDance = UiToolbox.NumberField(maid.status.baseDance, "Dance", 0, int.MaxValue);
				maid.status.baseVocal = UiToolbox.NumberField(maid.status.baseVocal, "Vocal", 0, int.MaxValue);
				//maid.status.playCountNightWork = NumberField(maid.status.playCountNightWork, "Service", 0, int.MaxValue);
				GUILayout.EndVertical();

				/*
				GUILayout.BeginVertical(Sections);
				//maid.status.baseTeachRate = UiToolbox.NumberField(maid.status.baseTeachRate, "Teach Rate", 0, int.MaxValue);
				GUILayout.EndVertical();
				*/

				GUILayout.BeginVertical(Sections);
				maid.status.likability = UiToolbox.NumberField(maid.status.likability, "Favor", 0, int.MaxValue);
				maid.status.baseMaxHp = UiToolbox.NumberField(maid.status.baseMaxHp, "Energy", 0, int.MaxValue);
				maid.status.baseMaxMind = UiToolbox.NumberField(maid.status.baseMaxMind, "Mind", 0, int.MaxValue);
				maid.status.baseMaxReason = UiToolbox.NumberField(maid.status.baseMaxReason, "Reason", 0, int.MaxValue);
				maid.status.baseInyoku = UiToolbox.NumberField(maid.status.baseInyoku, "Lust", 0, int.MaxValue);
				maid.status.baseMvalue = UiToolbox.NumberField(maid.status.baseMvalue, "Masochism", 0, int.MaxValue);
				maid.status.baseHentai = UiToolbox.NumberField(maid.status.baseHentai, "Hentai", 0, int.MaxValue);
				maid.status.baseHousi = UiToolbox.NumberField(maid.status.baseHousi, "Service", 0, int.MaxValue);
				//maid.status.playCountYotogi = NumberField(maid.status.playCountYotogi, "Yotogis", 0, int.MaxValue);
				GUILayout.EndVertical();
			}

			GUILayout.EndVertical();

			GUILayout.EndScrollView();

			UiToolbox.ChkMouseClick(WindowRect, ref CheatMenu.DrawUi);
		}

		public static void UnlockAllTrophies()
		{
			var data = Trophy.GetAllDatas(true);

			foreach (var trophyData in data)
			{
				GameMain.Instance.CharacterMgr.status.AddHaveTrophy(trophyData.id);
			}
		}

		public static void DisplayFakeTrophy(string trophyText, string shopItem = "")
		{
			var gameObject = GameObject.Find("SystemUI Root/TrophyAchieveEffect");
			if (gameObject == null)
			{
				return;
			}

			var component = gameObject.GetComponent<TrophyAchieveEffect>();
			if (component == null)
			{
				return;
			}

			var trophy = FormatterServices.GetUninitializedObject(typeof(Trophy.Data)) as Trophy.Data;

			if (trophy == null)
			{
				CheatMenu.PluginLogger.LogWarning("Trophy fail :(");
				return;
			}

			typeof(Trophy.Data).GetField(nameof(Trophy.Data.rarity)).SetValue(trophy, 3);
			typeof(Trophy.Data).GetField(nameof(Trophy.Data.name)).SetValue(trophy, trophyText);
			typeof(Trophy.Data).GetField(nameof(Trophy.Data.effectDrawItemName)).SetValue(trophy, shopItem);

			component.EffectStart(trophy);
		}
	}
}