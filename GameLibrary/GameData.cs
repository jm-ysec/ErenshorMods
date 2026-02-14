// GameData
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class GameData
{
	public enum DamageType
	{
		Physical,
		Magic,
		Elemental,
		Void,
		Poison
	}

	public static Vector2 WindowDimensionsThisSession = new Vector2(-1f, -1f);

	public static PlayerCombat PlayerCombat;

	public static IDLog ChatLog;

	public static IDLogLocal LocalLog;

	public static IDLogLocal VendorLog;

	public static IDLogLocal AHLog;

	public static EditControls EditControls;

	public static AuctionHouseUI AHUI;

	public static SimInspect InspectSim;

	public static Vector3 Wind;

	public static Smithing Smithing;

	public static HotkeyManager HKMngr;

	public static bool Gamepad;

	public static SimPlayerLanguage SimLang;

	public static Transform GameCamPos;

	public static bool PlayerTyping = false;

	public static bool InCharSelect = false;

	public static PlayerControl PlayerControl;

	public static Inventory PlayerInv;

	public static Stats PlayerStats;

	public static LootWindow LootWindow;

	public static TradeWindow TradeWindow;

	public static SkillBook PlayerSkill;

	public static SpellBook PlayerSpell;

	public static QuestLog QuestLog;

	public static SpellDB SpellDatabase;

	public static SkillDB SkillDatabase;

	public static SpellEffectDB EffectDB;

	public static CalcStats PlayerStatDisp;

	public static ItemIcon ItemOnCursor = null;

	public static ItemIcon HighlightedItem = null;

	public static VendorWindow VendorWindow;

	public static bool VendorWindowOpen = false;

	public static bool AuctionWindowOpen = false;

	public static bool PlayerAuctionItemsOpen = false;

	public static ItemIcon SlotToBeListed = null;

	public static ItemIcon SlotActiveForVendor;

	public static ItemIcon SlotActiveForAuction;

	public static int CurSellVal;

	public static ItemInfoWindow ItemInfoWindow;

	public static SceneChange SceneChange;

	public static ItemDatabase ItemDB;

	public static GameObject ItemToHK;

	public static bool Trading;

	public static Image ScreenFader;

	public static bool Zoning = false;

	public static DayNight Time;

	public static GameObject RespawnWindow;

	public static ClassDB ClassDB;

	public static SaveGameData CurrentCharacterSlot;

	public static List<SaveGameData> SaveSlots = new List<SaveGameData>();

	public static SaveGameData ActiveSaveSlot;

	public static GameManager GM;

	public static bool usingSun = false;

	public static bool InDungeon = false;

	public static NPC CharmedNPC = null;

	public static ItemIcon MouseSlot;

	public static string SceneName;

	public static AudioSource PlayerAud;

	public static bool InCombat = false;

	public static SimPlayerMngr SimMngr;

	public static CastBar CB;

	public static Misc Misc;

	public static CameraController CamControl;

	public static TutorialPopup TutPopup;

	public static QuestDB QuestDB;

	public static List<SimPlayerTracking> PlayerGroup = new List<SimPlayerTracking>();

	public static string ReliqL1;

	public static string ReliqL2;

	public static string ReliqL3;

	public static string ReliqL4;

	public static string ReliqR1;

	public static string ReliqR2;

	public static string ReliqR3;

	public static string ReliqR4;

	public static SimPlayerTracking[] GroupMembers = new SimPlayerTracking[4];

	public static KEYWORDS KEYWORDS;

	public static SimPlayerGrouping SimPlayerGrouping;

	public static SimPlayerShoutParse ShoutParse;

	public static Transform SimNameKey;

	public static int SimNameIndex = 0;

	public static bool SteamDeck = false;

	public static bool ScreenShakeOn = true;

	public static bool AutorecoverUI = true;

	public static float SFXVol = 1f;

	public static float BGMVol = 1f;

	public static float AMBVol = 1f;

	public static float FootVol = 1f;

	public static float SpellVol = 1f;

	public static float CombatVol = 1f;

	public static float MasterVol = 1f;

	public static float XPChimeVol = 1f;

	public static float UIVolume = 1f;

	public static float GrassQual = 1f;

	public static AtmosphereColors Atmos;

	public static List<string> CompletedQuests = new List<string>();

	public static List<string> HasQuest = new List<string>();

	public static TypeText TextInput;

	public static Vector3 ensureSafeLanding;

	public static string BindZone = "Hidden";

	public static Vector3 BindLoc = Vector3.zero;

	public static bool SunInBindZone = true;

	public static List<GameObject> DDOLOBJS = new List<GameObject>();

	public static bool UsingSteam = false;

	public static List<string> Keyring = new List<string>();

	public static List<string> popupText = new List<string>();

	public static bool Autoattacking = false;

	public static List<NPC> AttackingPlayer = new List<NPC>();

	public static List<NPC> GroupMatesInCombat = new List<NPC>();

	public static SimTradeWindow SimTrade;

	public static BankUI BankUI;

	public static ZoneAnnounce CurrentZoneAnnounce;

	public static int DaysSinceCharLastPlayed = -1;

	public static int backupIndex = 0;

	public static float HPScale = 1.3f;

	public static float AstraTimer = 0f;

	public static List<PointOfInterest> EgressLocations = new List<PointOfInterest>();

	public static PointOfInterest NearestZoneIn;

	public static EventSystem mEventSystem;

	public static List<NPCShoutListener> NPCShoutListeners = new List<NPCShoutListener>();

	public static ShowTargetEffects NPCEffects;

	public static float CamDrawDistance = 1f;

	public static float Brightness = 0f;

	public static bool ShowPlayerName = true;

	public static int ShowSimPlayerName = 2;

	public static bool SimCastBars = true;

	public static bool NPCCastBars = true;

	public static int ShowNPCName = 2;

	public static bool HideCorpseNametag = false;

	public static int MouseYMod = 1;

	public static int MouseXMod = 1;

	public static int NamePlateDrawDist = PlayerPrefs.GetInt("NAMEDRAWDIST", 25);

	public static bool FlashingCombatUI = true;

	public static bool SimBanter = true;

	public static bool HQRain = true;

	public static bool DraggingUIElement = false;

	public static bool MyPopups = true;

	public static bool OthersPopups = true;

	public static float CamSmooth = 0.33f;

	public static float HOffset = 0f;

	public static float VOffset = 0f;

	public static float FOV = 60f;

	public static bool XPChimeSFX = true;

	public static bool PetAssistGroupMA = true;

	public static int CurrentCamLimit = 3000;

	public static GEcMngr GEcMngr;

	public static SpellBook PlayerSpellBook;

	public static SkillBook PlayerSkillBook;

	public static string LoadFromBackup = "";

	public static bool SkipAwake = false;

	public static GroupBuilder GroupBuilder;

	public static Canvas MainCanvas;

	public static bool RequireRightClickInfo = false;

	public static Color blueOverride = new Color(0f, 0.34f, 1f, 1f);

	public static string ReadableBlue = "#00A1FF";

	public static float ServerXPMod = 1f;

	public static float ServerHPMod = 1f;

	public static float ServerDMGMod = 1f;

	public static bool NPCFlee = true;

	public static bool Jail = true;

	public static int ServerPop = 100;

	public static bool ShowTargetLevel;

	public static bool UseMap;

	public static bool UseMarkers;

	public static float ServerLootRate = 1f;

	public static string SpellOrSkill = "Spell";

	public static Transform ZoneAnnounce;

	public static ZoneAnnounce ZoneAnnounceData;

	public static bool ShowShouts = true;

	public static bool ShowWTB = true;

	public static bool UseChatBackground = false;

	public static bool UsePrivateWindow = true;

	public static float ChatWindowHeight = 250f;

	public static bool Level10Requirement;

	public static ClassIcon ClassIcon;

	public static bool RequireRMBRelease = false;

	public static bool CloudSavesSetOffAtRuntime = false;

	public static List<DragUI> AllUIElements = new List<DragUI>();

	public static float ParticleSystemScaling = 1f;

	public static float ParticleSystemScalingOther = 1f;

	public static CamGetPPFXProfile CamGetPPFX;

	public static bool XPLossOnDeath = false;

	public static float RunSpeedMod = 1f;

	public static LiveStatUpdate StatUpdater;

	public static float RespawnTimeMod = 1f;

	public static float SPDrawDist = 25000f;

	public static List<string> SivakayanSpawnedZones = new List<string>();

	public static GuildManager GuildManager;

	public static KnowledgeDatabase KnowledgeDatabase;

	public static GuildManagerUI GuildManagerUI;

	public static string ReliqDest;

	public static Vector3 ReliqLanding;

	public static int SunInReliqZone = 1;

	public static PlanningTableUI PlanningUI;

	public static bool EditUIMode = false;

	public static Crafting CraftWindow;

	public static bool AccountClaimedReliquary = false;

	public static int XPLock = 0;

	public static PointOfInterest LiveGodTargetPOI = null;

	public static int HidePlayerHelm = 0;

	public static int DynamicMusicFade = 1;

	public static List<string> CurrentIgnoreList = new List<string>();

	public static void AddSivakayanSpawn(string _scene)
	{
		SivakayanSpawnedZones.Add(_scene);
		if (SivakayanSpawnedZones.Count > 5)
		{
			SivakayanSpawnedZones.RemoveAt(0);
		}
	}

	public static void AddExperience(int xp, bool useMod)
	{
		int num = 0;
		float num2 = 1f;
		if (useMod)
		{
			if (GroupMembers[0] != null)
			{
				SimMngr.Sims[GroupMembers[0].simIndex].OpinionOfPlayer += 0.015f;
				num++;
			}
			if (GroupMembers[1] != null)
			{
				SimMngr.Sims[GroupMembers[1].simIndex].OpinionOfPlayer += 0.015f;
				num++;
			}
			if (GroupMembers[2] != null)
			{
				SimMngr.Sims[GroupMembers[2].simIndex].OpinionOfPlayer += 0.015f;
				num++;
			}
			if (GroupMembers[3] != null)
			{
				SimMngr.Sims[GroupMembers[3].simIndex].OpinionOfPlayer += 0.015f;
				num++;
			}
			switch (num)
			{
			case 1:
				num2 -= 0.25f;
				break;
			case 2:
				num2 -= 0.45f;
				break;
			case 3:
				num2 -= 0.55f;
				break;
			case 4:
				num2 -= 0.65f;
				break;
			}
		}
		int num3 = 0;
		int num4 = 99;
		if (GroupMembers.Length != 0)
		{
			SimPlayerTracking[] groupMembers = GroupMembers;
			foreach (SimPlayerTracking simPlayerTracking in groupMembers)
			{
				if (simPlayerTracking != null)
				{
					if (simPlayerTracking.Level > num3)
					{
						num3 = simPlayerTracking.Level;
					}
					if (simPlayerTracking.Level < num4)
					{
						num4 = simPlayerTracking.Level;
					}
				}
			}
		}
		if (PlayerStats.Level > num3)
		{
			num3 = PlayerStats.Level;
		}
		if (PlayerStats.Level < num4)
		{
			num4 = PlayerStats.Level;
		}
		if (GroupMembers[0] != null)
		{
			float num5 = num3 - GroupMembers[0].MyStats.Level;
			float num6 = 1f;
			if (num5 >= 6f)
			{
				num6 = 1f - 5f * num5 * 0.01f;
				num6 = Mathf.Clamp(num6, 0.05f, 1f);
			}
			GroupMembers[0].MyStats.EarnedXP(Mathf.RoundToInt((float)xp * num2 * num6));
		}
		if (GroupMembers[1] != null)
		{
			float num7 = num3 - GroupMembers[1].MyStats.Level;
			float num8 = 1f;
			if (num7 >= 6f)
			{
				num8 = 1f - 5f * num7 * 0.01f;
				num8 = Mathf.Clamp(num8, 0.05f, 1f);
			}
			GroupMembers[1].MyStats.EarnedXP(Mathf.RoundToInt((float)xp * num2 * num8));
		}
		if (GroupMembers[2] != null)
		{
			float num9 = num3 - GroupMembers[2].MyStats.Level;
			float num10 = 1f;
			if (num9 >= 6f)
			{
				num10 = 1f - 5f * num9 * 0.01f;
				num10 = Mathf.Clamp(num10, 0.05f, 1f);
			}
			GroupMembers[2].MyStats.EarnedXP(Mathf.RoundToInt((float)xp * num2 * num10));
		}
		if (GroupMembers[3] != null)
		{
			float num11 = num3 - GroupMembers[3].MyStats.Level;
			float num12 = 1f;
			if (num11 >= 6f)
			{
				num12 = 1f - 5f * num11 * 0.01f;
				num12 = Mathf.Clamp(num12, 0.05f, 1f);
			}
			GroupMembers[3].MyStats.EarnedXP(Mathf.RoundToInt((float)xp * num2 * num12));
		}
		PlayerControl.Myself.MyStats.EarnedXP(Mathf.RoundToInt((float)xp * num2));
	}

	public static bool IsQuestDone(string _questName)
	{
		if (_questName != null && _questName != "" && CompletedQuests.Contains(_questName))
		{
			return true;
		}
		return false;
	}

	public static bool IsQuestAssigned(string _questName)
	{
		if (_questName != null && _questName != "" && HasQuest.Contains(_questName))
		{
			return true;
		}
		return false;
	}

	public static void FinishQuest(string _questName)
	{
		if (_questName == null || !(_questName != "") || CompletedQuests.Contains(_questName))
		{
			return;
		}
		CompletedQuests.Add(_questName);
		if (HasQuest.Contains(_questName))
		{
			HasQuest.Remove(_questName);
		}
		HasQuest.RemoveAll(string.IsNullOrEmpty);
		UpdateSocialLog.LogAdd("Quest Completed: " + QuestDB.GetQuestByName(_questName).QuestName, "yellow");
		UpdateSocialLog.LocalLogAdd("Quest Completed: " + QuestDB.GetQuestByName(_questName).QuestName, "yellow");
		if (QuestDB.GetQuestByName(_questName) != null && !string.IsNullOrEmpty(QuestDB.GetQuestByName(_questName).SetAchievementOnFinish))
		{
			SetAchievement.Unlock(QuestDB.GetQuestByName(_questName).SetAchievementOnFinish);
		}
		if (QuestDB.GetQuestByName(_questName).AssignNewQuestOnComplete != null)
		{
			AssignQuest(QuestDB.GetQuestByName(_questName).AssignNewQuestOnComplete.DBName);
		}
		if (QuestDB.GetQuestByName(_questName).AffectFactions.Count > 0 && QuestDB.GetQuestByName(_questName).AffectFactionAmts.Count == QuestDB.GetQuestByName(_questName).AffectFactions.Count)
		{
			for (int i = 0; i < QuestDB.GetQuestByName(_questName).AffectFactions.Count; i++)
			{
				GlobalFactionManager.ModifyFaction(QuestDB.GetQuestByName(_questName).AffectFactionAmts[i], QuestDB.GetQuestByName(_questName).AffectFactions[i].REFNAME);
			}
		}
	}

	public static void CloseQuestline(Quest _quest)
	{
		if (HasQuest.Contains(_quest.QuestName))
		{
			HasQuest.Remove(_quest.QuestName);
		}
		if (!CompletedQuests.Contains(_quest.QuestName))
		{
			CompletedQuests.Add(_quest.QuestName);
		}
	}

	public static void AssignQuest(string _questName)
	{
		if (_questName != null && _questName != "" && !HasQuest.Contains(_questName) && !CompletedQuests.Contains(_questName))
		{
			HasQuest.Add(_questName);
			PlayerAud.PlayOneShot(Misc.QuestAdd, PlayerAud.volume * SFXVol * MasterVol);
			UpdateSocialLog.LogAdd("Received New Quest: " + QuestDB.GetQuestByName(_questName).QuestName, "yellow");
			UpdateSocialLog.LocalLogAdd("Received New Quest: " + QuestDB.GetQuestByName(_questName).QuestName, "yellow");
			if (QuestDB.GetQuestByName(_questName) != null && !string.IsNullOrEmpty(QuestDB.GetQuestByName(_questName).SetAchievementOnGet))
			{
				SetAchievement.Unlock(QuestDB.GetQuestByName(_questName).SetAchievementOnGet);
			}
		}
	}

	public static void ActivateSlotForVendor(ItemIcon _newActive)
	{
		DeactivateSlotForVendor();
		SlotActiveForVendor = _newActive;
		SlotActiveForVendor.transform.parent.GetComponent<Image>().color = Color.yellow;
		CurSellVal = Mathf.RoundToInt(_newActive.MyItem.ItemValue);
		if (_newActive.VendorSlot)
		{
			if (VendorWindow != null && VendorWindow.BuyBack != null && _newActive == VendorWindow.BuyBack)
			{
				UpdateSocialLog.LogAdd(VendorWindow.GetVendor() + " says: That'll be " + Mathf.RoundToInt((float)CurSellVal * 0.65f) + " gold to buy back the " + _newActive.MyItem.ItemName);
				UpdateSocialLog.LocalLogAdd(VendorWindow.GetVendor() + " says: That'll be " + Mathf.RoundToInt((float)CurSellVal * 0.65f) + " gold to buy back the " + _newActive.MyItem.ItemName);
			}
			else
			{
				UpdateSocialLog.LogAdd(VendorWindow.GetVendor() + " says: That'll be " + CurSellVal + " gold for the " + _newActive.MyItem.ItemName);
				UpdateSocialLog.LocalLogAdd(VendorWindow.GetVendor() + " says: That'll be " + CurSellVal + " gold for the " + _newActive.MyItem.ItemName);
			}
			VendorWindow.ButtonContext.text = "Purchase";
		}
		else
		{
			if (_newActive.MyItem.ItemValue > 0 && !_newActive.MyItem.NoTradeNoDestroy)
			{
				UpdateSocialLog.LogAdd(VendorWindow.GetVendor() + " says: I'll give you " + Mathf.RoundToInt((float)_newActive.MyItem.ItemValue * 0.65f) + " gold for the " + _newActive.MyItem.ItemName);
				UpdateSocialLog.LocalLogAdd(VendorWindow.GetVendor() + " says: I'll give you " + Mathf.RoundToInt((float)_newActive.MyItem.ItemValue * 0.65f) + " gold for the " + _newActive.MyItem.ItemName);
			}
			else
			{
				UpdateSocialLog.LogAdd(VendorWindow.GetVendor() + " says: Sorry, but I'm not buying the " + _newActive.MyItem.ItemName + "... just throw it away if you don't want it.");
				UpdateSocialLog.LocalLogAdd(VendorWindow.GetVendor() + " says: Sorry, but I'm not buying the " + _newActive.MyItem.ItemName + "... just throw it away if you don't want it.");
			}
			VendorWindow.ButtonContext.text = "Sell";
			if (_newActive.Quantity > 1 && _newActive.MyItem != null && _newActive.MyItem.RequiredSlot == Item.SlotType.General)
			{
				VendorWindow.SellStack.interactable = true;
			}
			else
			{
				VendorWindow.SellStack.interactable = false;
			}
		}
	}

	public static void DeactivateSlotForVendor()
	{
		if (SlotActiveForVendor != null)
		{
			SlotActiveForVendor.transform.parent.GetComponent<Image>().color = SlotActiveForVendor.ParCol;
		}
	}

	public static void ActivateSlotForAuction(ItemIcon _newActive)
	{
		DeactivateSlotForAuction();
		AHUI.CloseListItem();
		if (_newActive.VendorSlot && AHUI.CurrentSellerData.SellerName != PlayerStats.MyName)
		{
			SlotToBeListed = null;
			SlotActiveForAuction = _newActive;
			if (AHUI.CurrentSellerData.PriceMod == 0f)
			{
				AHUI.CurrentSellerData.PriceMod = 1f;
			}
			SlotActiveForAuction.transform.parent.GetComponent<Image>().color = Color.yellow;
			CurSellVal = Mathf.RoundToInt((float)_newActive.MyItem.ItemValue * 5f * AHUI.CurrentSellerData.PriceMod);
		}
		else if (_newActive.VendorSlot && AHUI.CurrentSellerData.SellerName == PlayerStats.MyName)
		{
			SlotToBeListed = null;
			SlotActiveForAuction = _newActive;
			if (AHUI.CurrentSellerData.PriceMod == 0f)
			{
				AHUI.CurrentSellerData.PriceMod = 1f;
			}
			SlotActiveForAuction.transform.parent.GetComponent<Image>().color = Color.yellow;
			CurSellVal = Mathf.RoundToInt((float)_newActive.MyItem.ItemValue * 5f * AHUI.CurrentSellerData.PriceMod);
		}
		if (!_newActive.VendorSlot && _newActive.MyItem.ItemLevel != 0 && _newActive.MyItem.ItemValue != 0 && !_newActive.MyItem.NoTradeNoDestroy)
		{
			SlotToBeListed = _newActive;
			AHUI.OpenListItem();
			AHUI.OpenPlayerListing();
		}
		else if (_newActive.MyItem.ItemLevel != 0 && _newActive.MyItem.ItemValue != 0)
		{
			_ = _newActive.MyItem.NoTradeNoDestroy;
		}
	}

	public static void DeactivateSlotForAuction()
	{
		if (SlotActiveForAuction != null)
		{
			SlotActiveForAuction.transform.parent.GetComponent<Image>().color = SlotActiveForAuction.ParCol;
		}
	}

	public static void CheckLoadData(int index)
	{
		string text = Path.Combine(Application.persistentDataPath, "charSlot" + index);
		string text2 = Path.Combine(Application.persistentDataPath, "ESSaveData", "charSlot" + index);
		string text3 = null;
		if (File.Exists(text2))
		{
			text3 = text2;
		}
		else
		{
			if (!File.Exists(text))
			{
				Debug.LogWarning("No save file found for slot " + index);
				return;
			}
			text3 = text;
		}
		string json = null;
		try
		{
			using FileStream stream = new FileStream(text3, FileMode.Open, FileAccess.Read, FileShare.Read);
			using StreamReader streamReader = new StreamReader(stream);
			json = streamReader.ReadToEnd();
		}
		catch (Exception ex)
		{
			Debug.LogError("Failed to read save file at " + text3 + ": " + ex.Message);
			return;
		}
		try
		{
			SaveSlots[index] = JsonUtility.FromJson<SaveGameData>(json);
			if (SaveSlots[index].SEData == null || SaveSlots[index].SEData.Length != 30)
			{
				SaveSlots[index].SEData = new StatusEffectSaveData[30];
			}
			for (int i = 0; i < 30; i++)
			{
				if (SaveSlots[index].SEData[i] == null)
				{
					SaveSlots[index].SEData[i] = new StatusEffectSaveData("", 0f);
				}
			}
			try
			{
				AuctionHouse.LoadCharData(SaveSlots[index].CharName);
			}
			catch (Exception ex2)
			{
				Debug.LogWarning("Auction data failed for " + SaveSlots[index].CharName + ": " + ex2.Message);
			}
		}
		catch (Exception ex3)
		{
			Debug.LogError("Failed to parse save data from " + text3 + ": " + ex3.Message);
		}
	}

	public static bool SaveData()
	{
		string text = Application.persistentDataPath + "\\backups\\";
		string text2 = Application.persistentDataPath + "\\ESSaveData\\";
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		if (!Directory.Exists(text2))
		{
			Directory.CreateDirectory(text2);
		}
		string text3 = text2 + "charSlot" + CurrentCharacterSlot.index;
		string text4 = JsonUtility.ToJson(CurrentCharacterSlot, prettyPrint: true);
		try
		{
			JsonUtility.FromJson<SaveGameData>(text4);
		}
		catch
		{
			UpdateSocialLog.LogAdd("FAILED TO SAVE CHARACTER...", "yellow");
			Debug.Log("SAVE DATA FAILED");
			return false;
		}
		if (!string.IsNullOrEmpty(text4))
		{
			string text5 = text3 + ".tmp";
			using (FileStream fileStream = new FileStream(text5, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				using StreamWriter streamWriter = new StreamWriter(fileStream);
				streamWriter.Write(text4);
				streamWriter.Flush();
				fileStream.Flush(flushToDisk: true);
			}
			if (File.Exists(text3))
			{
				File.Replace(text5, text3, null);
			}
			else
			{
				File.Move(text5, text3);
			}
			using FileStream fileStream2 = new FileStream(text + PlayerStats.MyName + CurrentCharacterSlot.index + "-" + backupIndex, FileMode.Create, FileAccess.Write, FileShare.None);
			using StreamWriter streamWriter2 = new StreamWriter(fileStream2);
			streamWriter2.Write(text4);
			streamWriter2.Flush();
			fileStream2.Flush(flushToDisk: true);
		}
		backupIndex++;
		if (backupIndex > 20)
		{
			backupIndex = 0;
		}
		return true;
	}

	public static bool CheckKeyring(string _id)
	{
		foreach (string item in Keyring)
		{
			if (item == _id)
			{
				return true;
			}
		}
		return false;
	}

	public static void CreateBackupFolders()
	{
		for (int i = 1; i <= 10; i++)
		{
			string path = Path.Combine(Application.persistentDataPath, $"Data{i}");
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}
	}

	public static void SaveBackupSnapshot()
	{
		int num = FindBackupSlotPreferEmpty();
		if (num == -1)
		{
			return;
		}
		string text = Path.Combine(Application.persistentDataPath, "ESSaveData");
		string text2 = Path.Combine(Application.persistentDataPath, "Data" + num);
		if (!Directory.Exists(text))
		{
			Debug.LogWarning("SaveBackupSnapshot: source dir missing: " + text);
			return;
		}
		Directory.CreateDirectory(text2);
		string[] files = Directory.GetFiles(text, "*", SearchOption.TopDirectoryOnly);
		int num2 = 0;
		string[] array = files;
		foreach (string path in array)
		{
			string fileName = Path.GetFileName(path);
			if (fileName.Equals("metaData.json", StringComparison.OrdinalIgnoreCase))
			{
				continue;
			}
			string text3 = Path.Combine(text2, fileName);
			string text4 = text3 + ".tmp";
			byte[] array2;
			using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				array2 = new byte[fileStream.Length];
				int num3;
				for (int j = 0; j < array2.Length; j += num3)
				{
					num3 = fileStream.Read(array2, j, array2.Length - j);
					if (num3 <= 0)
					{
						break;
					}
				}
			}
			using (FileStream fileStream2 = new FileStream(text4, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				fileStream2.Write(array2, 0, array2.Length);
				fileStream2.Flush(flushToDisk: true);
			}
			TryReplace(text4, text3);
			num2++;
		}
		Debug.Log($"{num2} files saved to backup slot {num} at logout: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
		string value = JsonUtility.ToJson(new SaveMetaData(DateTime.Now), prettyPrint: true);
		if (string.IsNullOrEmpty(value))
		{
			return;
		}
		string text5 = Path.Combine(text2, "metaData.json");
		string text6 = text5 + ".tmp";
		using (FileStream fileStream3 = new FileStream(text6, FileMode.Create, FileAccess.Write, FileShare.None))
		{
			using StreamWriter streamWriter = new StreamWriter(fileStream3);
			streamWriter.Write(value);
			streamWriter.Flush();
			fileStream3.Flush(flushToDisk: true);
		}
		TryReplace(text6, text5);
	}

	private static void TryReplace(string tempPath, string finalPath)
	{
		try
		{
			if (File.Exists(finalPath))
			{
				File.Replace(tempPath, finalPath, null);
			}
			else
			{
				File.Move(tempPath, finalPath);
			}
		}
		catch
		{
			if (File.Exists(finalPath))
			{
				File.Delete(finalPath);
			}
			File.Move(tempPath, finalPath);
		}
	}

	private static int FindBackupSlotPreferEmpty()
	{
		int num = -1;
		int num2 = -1;
		TimeSpan timeSpan = TimeSpan.MinValue;
		for (int i = 1; i <= 10; i++)
		{
			string path = Path.Combine(Application.persistentDataPath, $"Data{i}");
			if (!Directory.Exists(path))
			{
				continue;
			}
			string text = null;
			foreach (string item in Directory.EnumerateFiles(path))
			{
				if (string.Equals(Path.GetFileName(item), "metaData.json", StringComparison.OrdinalIgnoreCase))
				{
					text = item;
					break;
				}
			}
			if (text == null)
			{
				if (num == -1)
				{
					num = i;
				}
				continue;
			}
			TimeSpan timeSpan2 = DateTime.UtcNow - File.GetLastWriteTimeUtc(text);
			if (timeSpan2 > timeSpan)
			{
				timeSpan = timeSpan2;
				num2 = i;
			}
		}
		if (num != -1)
		{
			return num;
		}
		if (num2 != -1)
		{
			return num2;
		}
		Debug.LogWarning("FindBackupSlotPreferEmpty: no valid slots found - defaulting to 1.");
		return 1;
	}

	public static Vector3 GetSafeNavMeshPoint(Vector3 _destination, float sampleRadius = 2f, float maxVerticalDiff = 0.25f, float verticalSearchHeight = 4f, float step = 0.5f)
	{
		if (NavMesh.SamplePosition(_destination + Vector3.up, out var hit, sampleRadius, -1) && Mathf.Abs(hit.position.y - _destination.y) <= maxVerticalDiff)
		{
			return hit.position;
		}
		return _destination;
	}

	public static Vector3 GetSafeNavMeshPointInRange(Vector3 origin, float searchRadius = 20f, float maxVerticalDiff = 2f, int attempts = 10)
	{
		for (int i = 0; i < attempts; i++)
		{
			Vector2 vector = UnityEngine.Random.insideUnitCircle * searchRadius;
			if (NavMesh.SamplePosition(origin + new Vector3(vector.x, 0f, vector.y), out var hit, 1f, -1) && Mathf.Abs(hit.position.y - origin.y) <= maxVerticalDiff)
			{
				return hit.position;
			}
		}
		return origin;
	}

	public static Vector3 GetNavMeshUnderFeet(Vector3 origin, float raycastHeight = 2f, float sampleRadius = 1f)
	{
		Vector3 origin2 = origin + Vector3.up * raycastHeight;
		if (Physics.Raycast(new Ray(origin2, Vector3.down), out var hitInfo, raycastHeight * 2f) && NavMesh.SamplePosition(hitInfo.point, out var hit, sampleRadius, -1))
		{
			return hit.position;
		}
		return origin;
	}

	public static void DebugCheckDuplicateSimIndexes(List<SimPlayerTracking> sims)
	{
		Dictionary<int, SimPlayerTracking> dictionary = new Dictionary<int, SimPlayerTracking>();
		foreach (SimPlayerTracking sim in sims)
		{
			if (sim != null)
			{
				int simIndex = sim.simIndex;
				if (dictionary.TryGetValue(simIndex, out var value))
				{
					Debug.LogWarning($"Duplicate simIndex detected: {simIndex} | Sim A: {value.SimName} | Sim B: {sim.SimName}");
				}
				else
				{
					dictionary.Add(simIndex, sim);
				}
			}
		}
	}

	public static void CloseWarning()
	{
	}
}
