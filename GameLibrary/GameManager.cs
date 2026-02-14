// GameManager
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public HotkeyManager HKManager;

	public CastSpell PlayerSpells;

	public UseSkill PlayerSkills;

	public Stats PlayerStats;

	public List<Item> CommonWorldItems;

	public List<GameObject> DynamicAudioSources = new List<GameObject>();

	public Item Sivak;

	public List<Item> Maps;

	public Item Empty2;

	public Item Planar;

	public Item PlanarShard;

	public Item InertDiamond;

	public Item XPPot;

	public Item CrystallizedBalance;

	public GameObject SpecialLootBeam;

	public GameObject DynamicAud;

	public float SpawnTimeMod = 1f;

	private float CampTime = 4f;

	private float TwoSec = 120f;

	private Vector3 CampPos;

	public bool DemoBuild;

	public bool DemonstrationMode;

	public bool RocFestMode;

	public bool DemoInit;

	private string[] gamePad;

	private bool loggingOff;

	public Item GuaranteeMine;

	public List<string> DevTeam;

	public List<string> WikiTeam;

	public List<string> Patron;

	public List<string> GameRant;

	public TextMeshProUGUI ZoneName;

	public Color32 announceCol = Color.white;

	private float holdDisplay = 120f;

	private float a = 255f;

	public GameObject EscapeMenu;

	public GameObject ControlScheme;

	public float DamageBalanceFactor = 1f;

	public float HPBalanceFactor = 1f;

	public List<Item> WorldDropMolds;

	private float CleanCombatData;

	public int NPCGlobalDamageMod;

	public GameObject BookUI;

	public GameObject TargetRing;

	public GameObject AscensionWindow;

	public TextMeshProUGUI AscensionText;

	public TextMeshProUGUI InvButtonText;

	public string TreasureZone;

	public Vector3 TreasureLoc;

	public bool DropMasks;

	public float GlobalChatShift;

	public bool AutoEngageAttackOnSkill = true;

	public Toggle AutoEngageDD;

	public PostProcessProfile PPFX;

	private float LogOffDel;

	public float BGMDel;

	public GameObject PlanningTable;

	public bool JailedFromTutorial;

	private void Awake()
	{
		GameData.GM = this;
		ZoneAtlas.Atlas = Resources.LoadAll<ZoneAtlasEntry>("Atlases");
	}

	private void Start()
	{
		InputManager.LoadScheme();
		GlobalFactionManager.LoadFactions();
		ApplyOptions.ApplyAll();
		for (int i = 0; i <= 24; i++)
		{
			DynamicAudio.AudioSources.Add(UnityEngine.Object.Instantiate(DynamicAud).GetComponent<AudioSource>());
		}
		BookUI.SetActive(value: true);
		BookUI.SetActive(value: false);
		SetAchievement.Initialize();
		if (PlayerPrefs.GetInt("AutoEngageAttack", 1) == 1)
		{
			AutoEngageAttackOnSkill = true;
			AutoEngageDD.SetIsOnWithoutNotify(value: true);
		}
		else
		{
			AutoEngageAttackOnSkill = false;
			AutoEngageDD.SetIsOnWithoutNotify(value: false);
		}
		GameData.XPLock = PlayerPrefs.GetInt("EXPLOCK", 0);
	}

	private void OnApplicationQuit()
	{
		if (GameData.WindowDimensionsThisSession.x != -1f)
		{
			PlayerPrefs.SetFloat("WINDOWDIMENSIONX", GameData.WindowDimensionsThisSession.x);
			PlayerPrefs.SetFloat("WINDOWDIMENSIONY", GameData.WindowDimensionsThisSession.y);
		}
		if (SceneManager.GetActiveScene().name != "LoadScene" && SceneManager.GetActiveScene().name != "Menu")
		{
			Debug.Log("Player force closed app, saving game...");
			SaveReliq();
			GameData.GM.SaveGameData(wScene: true);
			SimPlayerDataManager.SaveAllSimData();
			GameData.GuildManager.SaveGuildData();
			GameData.SaveBackupSnapshot();
		}
		StopAllCoroutines();
		SteamAPI.Shutdown();
		Debug.Log("SteamAPI shutdown complete");
	}

	private void Update()
	{
		if (LogOffDel > 0f)
		{
			LogOffDel -= 60f * Time.deltaTime;
			if (LogOffDel <= 0f)
			{
				LogOffComplete();
			}
		}
		if (BGMDel > 0f)
		{
			BGMDel -= 60f * Time.deltaTime;
		}
		if (GameData.SteamDeck && !GameData.Gamepad)
		{
			ToggleGamepad();
		}
		if (Input.GetKeyDown(KeyCode.F9) && DemonstrationMode)
		{
			GetComponent<DemonstrationMngr>().SetArcanist();
		}
		if (Input.GetKeyDown(KeyCode.F10) && DemonstrationMode)
		{
			GetComponent<DemonstrationMngr>().SetDuelist();
		}
		if (Input.GetKeyDown(KeyCode.F11) && DemonstrationMode)
		{
			GetComponent<DemonstrationMngr>().SetDruid();
		}
		if (Input.GetKeyDown(KeyCode.F12) && DemonstrationMode)
		{
			GetComponent<DemonstrationMngr>().SetPaladin();
		}
		if (Input.GetKeyDown(KeyCode.F9) && RocFestMode)
		{
			GetComponent<DemonstrationMngrROC>().SetArcanist();
		}
		if (Input.GetKeyDown(KeyCode.F10) && RocFestMode)
		{
			GetComponent<DemonstrationMngrROC>().SetDuelist();
		}
		if (Input.GetKeyDown(KeyCode.F11) && RocFestMode)
		{
			GetComponent<DemonstrationMngrROC>().SetDruid();
		}
		if (Input.GetKeyDown(KeyCode.F12) && RocFestMode)
		{
			GetComponent<DemonstrationMngrROC>().SetPaladin();
		}
		if (Input.GetKeyDown(KeyCode.F8) && RocFestMode)
		{
			GetComponent<DemonstrationMngrROC>().SetStorm();
		}
		Input.GetKeyDown(KeyCode.F11);
		if (!GameData.GM.DemoBuild)
		{
			Input.GetKeyDown(KeyCode.F8);
		}
		if (GameData.GroupMembers[0] != null && GameData.GroupMembers[1] != null && GameData.GroupMembers[2] != null && GameData.GroupMembers[3] != null)
		{
			SpawnTimeMod = 1.8f;
		}
		else if (GameData.GroupMembers[0] != null && GameData.GroupMembers[1] != null && GameData.GroupMembers[2] != null)
		{
			SpawnTimeMod = 1.5f;
		}
		else if (GameData.GroupMembers[0] != null || GameData.GroupMembers[1] != null)
		{
			SpawnTimeMod = 1.1f;
		}
		else
		{
			SpawnTimeMod = 1f;
		}
		if (Input.GetKeyDown(KeyCode.Escape) || (GameData.PlayerControl.usingGamepad && Input.GetKeyDown(KeyCode.JoystickButton7)))
		{
			bool flag = false;
			GameObject gameObject = GameData.Misc.UIWindows.FirstOrDefault((GameObject window) => window.activeSelf && window.transform.name == "Tutorial Popup");
			if (gameObject != null)
			{
				gameObject.SetActive(value: false);
				flag = true;
				return;
			}
			if (!flag)
			{
				flag = CloseUIWindows();
			}
			if (GameData.PlayerControl.CurrentTarget != null && !flag)
			{
				GameData.PlayerControl.CurrentTarget.UntargetMe();
				GameData.PlayerControl.CurrentTarget = null;
				GameData.NPCEffects.ClearTargetEffects();
				GameData.PlayerControl.TargetWindow.SetActive(value: false);
				flag = true;
			}
			if (!flag && SceneManager.GetActiveScene().name != "LoadScene")
			{
				EscapeMenu.SetActive(!EscapeMenu.activeSelf);
			}
		}
		if (loggingOff && TwoSec > 0f)
		{
			TwoSec -= 60f * Time.deltaTime;
			if (TwoSec <= 0f)
			{
				if (GameData.PlayerControl.transform.position != CampPos || !GameData.PlayerControl.Myself.Alive)
				{
					CampTime = 4f;
					UpdateSocialLog.LogAdd("Moved during disconnect - please remain still!", "yellow");
					CampPos = GameData.PlayerControl.transform.position;
					loggingOff = false;
					TwoSec = 120f;
				}
				else
				{
					TwoSec = 120f;
					CampTime -= 2f;
					if (CampTime > 0f)
					{
						UpdateSocialLog.LogAdd("Disconnecting from server in " + CampTime + " seconds... please remain still.", "yellow");
						if (GameData.GroupMembers[0] != null)
						{
							GameData.SimPlayerGrouping.ForceDismissFromGroup(GameData.GroupMembers[0].MyAvatar.MyStats.Myself);
						}
						if (GameData.GroupMembers[1] != null)
						{
							GameData.SimPlayerGrouping.ForceDismissFromGroup(GameData.GroupMembers[1].MyAvatar.MyStats.Myself);
						}
						if (GameData.GroupMembers[2] != null)
						{
							GameData.SimPlayerGrouping.ForceDismissFromGroup(GameData.GroupMembers[2].MyAvatar.MyStats.Myself);
						}
						if (GameData.GroupMembers[3] != null)
						{
							GameData.SimPlayerGrouping.ForceDismissFromGroup(GameData.GroupMembers[3].MyAvatar.MyStats.Myself);
						}
					}
					else
					{
						CorpseDataManager.CheckAllCorpses();
						loggingOff = false;
						if (!Screen.fullScreen)
						{
							GameData.WindowDimensionsThisSession.x = Screen.width;
							GameData.WindowDimensionsThisSession.y = Screen.height;
						}
						GameData.TextInput.CloseInputBox();
						SceneManager.LoadScene("Menu");
					}
				}
			}
		}
		if (holdDisplay > 0f)
		{
			holdDisplay -= 60f * Time.deltaTime;
			if (a < 255f)
			{
				a += 160f * Time.deltaTime;
			}
			if (a > 255f)
			{
				a = 255f;
			}
			announceCol.a = (byte)a;
			ZoneName.color = announceCol;
		}
		else if (a > 0f)
		{
			a -= 160f * Time.deltaTime;
			if (a <= 0f)
			{
				a = 0f;
				ZoneName.gameObject.SetActive(value: false);
			}
			announceCol.a = (byte)a;
			ZoneName.color = announceCol;
		}
		if (CleanCombatData > 0f)
		{
			CleanCombatData -= 60f * Time.deltaTime;
			return;
		}
		CleanUpData();
		CleanCombatData = 60f;
	}

	public bool CloseUIWindows()
	{
		bool result = false;
		foreach (GameObject uIWindow in GameData.Misc.UIWindows)
		{
			if (uIWindow.activeSelf && uIWindow.transform.name != "Tutorial Popup")
			{
				result = true;
				if (uIWindow.transform.name == "TradeWindow")
				{
					GameData.TradeWindow.CancelTrade();
				}
				else if (uIWindow.transform.name == "Bank" && GameData.BankUI != null)
				{
					GameData.BankUI.CloseBank();
				}
				else if (uIWindow.transform.name == "LootWindow")
				{
					GameData.LootWindow.CloseWindow();
				}
				else if (uIWindow.transform.name == "PlayerInv")
				{
					GameData.PlayerInv.ForceCloseInv();
				}
				else if (uIWindow.transform.name == "HallCustomizer")
				{
					uIWindow.SetActive(value: false);
				}
				else if (uIWindow.transform.name == "GuildManager")
				{
					uIWindow.SetActive(value: false);
				}
				else if (uIWindow.transform.name == "Marketplace")
				{
					uIWindow.SetActive(value: false);
					GameData.AuctionWindowOpen = false;
				}
				else if (uIWindow.transform.name == "VendorWindow")
				{
					uIWindow.SetActive(value: false);
					GameData.VendorWindowOpen = false;
				}
				else
				{
					uIWindow.SetActive(value: false);
				}
			}
		}
		return result;
	}

	private void CleanUpData()
	{
		if (GameData.AttackingPlayer.Count > 0)
		{
			for (int num = GameData.AttackingPlayer.Count - 1; num >= 0; num--)
			{
				if (GameData.AttackingPlayer[num] == null || (GameData.AttackingPlayer[num] != null && GameData.AttackingPlayer[num].GetChar() != null && !GameData.AttackingPlayer[num].GetChar().Alive))
				{
					GameData.AttackingPlayer.RemoveAt(num);
				}
			}
		}
		if (GameData.PlayerControl.HuntingMe.Count <= 0)
		{
			return;
		}
		for (int num2 = GameData.PlayerControl.HuntingMe.Count - 1; num2 >= 0; num2--)
		{
			if (GameData.PlayerControl.HuntingMe[num2] == null)
			{
				GameData.PlayerControl.HuntingMe.RemoveAt(num2);
			}
		}
	}

	public void ShowZoneName(string _name)
	{
		ZoneName.gameObject.SetActive(value: true);
		ZoneName.text = _name;
		a = 0f;
		holdDisplay = 200f;
	}

	public void OpenEscMenu()
	{
		EscapeMenu.SetActive(value: true);
	}

	public void CloseEscMenu()
	{
		EscapeMenu.SetActive(value: false);
	}

	public void ToggleEscapeMenu()
	{
		if (!EscapeMenu.activeSelf)
		{
			EscapeMenu.SetActive(value: true);
		}
		else
		{
			EscapeMenu.SetActive(value: false);
		}
	}

	public void OpenOptions()
	{
		GameData.HKMngr.OptionsMenu.SetActive(value: true);
		GameData.HKMngr.OptionsMenu.GetComponent<OptionsMenu>().DoGFX();
	}

	public void LogOff()
	{
		UpdateSocialLog.LogAdd(">>Saving Game Data...<<", "yellow");
		CloseEscMenu();
		GameData.Misc.SavingDataWarn.SetActive(value: true);
		LogOffDel = 5f;
	}

	private void LogOffComplete()
	{
		SaveGameData(wScene: true);
		SaveReliq();
		CampPos = GameData.PlayerControl.transform.position;
		loggingOff = true;
		UpdateSocialLog.LogAdd(">>Disconnecting from Server - please remain still!<<", "yellow");
		GameData.Misc.SavingDataWarn.SetActive(value: false);
		SimPlayerDataManager.SaveAllSimData();
		GameData.SaveBackupSnapshot();
		GameData.GuildManager.SaveGuildData();
		GameData.GM.CloseUIWindows();
	}

	public void ToggleGamepad()
	{
		GameData.Gamepad = !GameData.Gamepad;
		if (GameData.Gamepad)
		{
			UpdateSocialLog.LogAdd("Now listening for Gamepad input... press any button on gamepad to activate", "yellow");
		}
		else
		{
			UpdateSocialLog.LogAdd("Now muting all non-keyboard and mouse inputs.", "yellow");
		}
	}

	public void OpenFeedback()
	{
		Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLSddwZLmup_b_M8Nqs_NOqIVBy6WGF8mfjEJDAtO3HNVsO0yhg/viewform?usp=sharing&ouid=111449527573569095904");
	}

	public void OpenWiki()
	{
		Application.OpenURL("https://erenshor.wiki.gg/wiki/Erenshor_Wiki");
	}

	public void SaveGameData(bool wScene)
	{
		GameData.CurrentCharacterSlot.CharacterEquip.Clear();
		GameData.CurrentCharacterSlot.CharacterInv.Clear();
		GameData.CurrentCharacterSlot.InvSlotQuantities.Clear();
		GameData.CurrentCharacterSlot.EquipSlotQuantities.Clear();
		GameData.CurrentCharacterSlot.CharacterSkills.Clear();
		GameData.CurrentCharacterSlot.CharacterSpells.Clear();
		GameData.CurrentCharacterSlot.CompletedQuests.Clear();
		GameData.CurrentCharacterSlot.ActiveQuests.Clear();
		GameData.CurrentCharacterSlot.FactionData.Clear();
		GameData.CurrentCharacterSlot.LoadWithSun = GameData.usingSun;
		GameData.CurrentCharacterSlot.TutorialsDone.Clear();
		GameData.CurrentCharacterSlot.Keyring.Clear();
		GameData.CurrentCharacterSlot.Ascensions.Clear();
		GameData.CurrentCharacterSlot.IgnoreList.Clear();
		if (GameData.CurrentIgnoreList.Count > 0)
		{
			foreach (string currentIgnore in GameData.CurrentIgnoreList)
			{
				GameData.CurrentCharacterSlot.IgnoreList.Add(currentIgnore);
			}
		}
		if (GameData.popupText.Count > 0)
		{
			foreach (string item in GameData.popupText)
			{
				if (!GameData.CurrentCharacterSlot.TutorialsDone.Contains(item))
				{
					GameData.CurrentCharacterSlot.TutorialsDone.Add(item);
				}
			}
		}
		for (int i = 0; i < 30; i++)
		{
			if (GameData.CurrentCharacterSlot.SE.Length > i)
			{
				GameData.CurrentCharacterSlot.SE[i] = null;
			}
			if (GameData.CurrentCharacterSlot.SEData.Length > i)
			{
				GameData.CurrentCharacterSlot.SEData[i] = new StatusEffectSaveData("", 0f);
			}
		}
		GameData.CurrentCharacterSlot.CharLevel = GameData.PlayerStats.Level;
		if (wScene)
		{
			GameData.CurrentCharacterSlot.CurScene = SceneManager.GetActiveScene().name;
			GameData.CurrentCharacterSlot.currentPos = GameData.PlayerControl.transform.position;
		}
		if (GameData.Keyring.Count > 0)
		{
			foreach (string item2 in GameData.Keyring)
			{
				GameData.CurrentCharacterSlot.Keyring.Add(item2);
			}
		}
		foreach (ItemIcon equipmentSlot in GameData.PlayerInv.EquipmentSlots)
		{
			if (equipmentSlot != null)
			{
				GameData.CurrentCharacterSlot.CharacterEquip.Add(equipmentSlot.MyItem.Id);
				GameData.CurrentCharacterSlot.EquipSlotQuantities.Add(equipmentSlot.Quantity);
			}
		}
		foreach (ItemIcon storedSlot in GameData.PlayerInv.StoredSlots)
		{
			if (storedSlot != null)
			{
				GameData.CurrentCharacterSlot.CharacterInv.Add(storedSlot.MyItem.Id);
				GameData.CurrentCharacterSlot.InvSlotQuantities.Add(storedSlot.Quantity);
			}
		}
		if (GameData.PlayerInv.AuraSlot != null && GameData.PlayerInv.AuraSlot.MyItem != null)
		{
			GameData.CurrentCharacterSlot.AuraItem = GameData.PlayerInv.AuraSlot.MyItem.Id;
		}
		if (GameData.PlayerInv.CharmSlot != null && GameData.PlayerInv.CharmSlot.MyItem != null)
		{
			GameData.CurrentCharacterSlot.CharmItem = GameData.PlayerInv.CharmSlot.MyItem.Id;
			GameData.CurrentCharacterSlot.CharmQual = GameData.PlayerInv.CharmSlot.Quantity;
		}
		if (PlayerSpells.KnownSpells.Count > 0)
		{
			foreach (Spell knownSpell in PlayerSpells.KnownSpells)
			{
				GameData.CurrentCharacterSlot.CharacterSpells.Add(knownSpell.Id);
			}
		}
		if (PlayerSkills.KnownSkills.Count > 0)
		{
			foreach (Skill knownSkill in PlayerSkills.KnownSkills)
			{
				GameData.CurrentCharacterSlot.CharacterSkills.Add(knownSkill.Id);
			}
		}
		if (PlayerSkills.MyAscensions.Count > 0)
		{
			foreach (AscensionSkillEntry myAscension in PlayerSkills.MyAscensions)
			{
				GameData.CurrentCharacterSlot.Ascensions.Add(new AscensionSkillEntry(myAscension.id, myAscension.level));
			}
		}
		GameData.CurrentCharacterSlot.AscensionXP = PlayerStats.CurrentAscensionXP;
		GameData.CurrentCharacterSlot.AscensionPointsUnspent = PlayerSkills.AscensionPoints;
		int num = 0;
		foreach (Hotkeys firstHotkey in GameData.GM.HKManager.FirstHotkeys)
		{
			if (firstHotkey.AssignedSpell != null)
			{
				GameData.CurrentCharacterSlot.HKSpells[num] = firstHotkey.AssignedSpell.Id;
				GameData.CurrentCharacterSlot.HKItems[num] = -1;
				GameData.CurrentCharacterSlot.HKSkills[num] = null;
			}
			else if (firstHotkey.AssignedSkill != null)
			{
				GameData.CurrentCharacterSlot.HKSkills[num] = firstHotkey.AssignedSkill.Id;
				GameData.CurrentCharacterSlot.HKSpells[num] = null;
				GameData.CurrentCharacterSlot.HKItems[num] = -1;
			}
			else if (firstHotkey.AssignedItem != null)
			{
				GameData.CurrentCharacterSlot.HKItems[num] = firstHotkey.InvSlotIndex;
				GameData.CurrentCharacterSlot.HKSpells[num] = null;
				GameData.CurrentCharacterSlot.HKSkills[num] = null;
			}
			else
			{
				GameData.CurrentCharacterSlot.HKSpells[num] = null;
				GameData.CurrentCharacterSlot.HKItems[num] = -1;
				GameData.CurrentCharacterSlot.HKSkills[num] = null;
			}
			num++;
		}
		int num2 = 0;
		foreach (Hotkeys secondHotkey in GameData.GM.HKManager.SecondHotkeys)
		{
			if (secondHotkey.AssignedSpell != null)
			{
				GameData.CurrentCharacterSlot.SecHKSpells[num2] = secondHotkey.AssignedSpell.Id;
				GameData.CurrentCharacterSlot.SecHKItems[num2] = -1;
				GameData.CurrentCharacterSlot.SecHKSkills[num2] = null;
			}
			else if (secondHotkey.AssignedSkill != null)
			{
				GameData.CurrentCharacterSlot.SecHKSkills[num2] = secondHotkey.AssignedSkill.Id;
				GameData.CurrentCharacterSlot.SecHKSpells[num2] = null;
				GameData.CurrentCharacterSlot.SecHKItems[num2] = -1;
			}
			else if (secondHotkey.AssignedItem != null)
			{
				GameData.CurrentCharacterSlot.SecHKItems[num2] = secondHotkey.InvSlotIndex;
				GameData.CurrentCharacterSlot.SecHKSpells[num2] = null;
				GameData.CurrentCharacterSlot.SecHKSkills[num2] = null;
			}
			else if (num2 >= 0 && num2 < 10)
			{
				if (GameData.CurrentCharacterSlot.SecHKSpells == null)
				{
					GameData.CurrentCharacterSlot.SecHKSpells = new string[10];
				}
				if (GameData.CurrentCharacterSlot.SecHKItems == null)
				{
					GameData.CurrentCharacterSlot.SecHKItems = new int[10];
				}
				if (GameData.CurrentCharacterSlot.SecHKSkills == null)
				{
					GameData.CurrentCharacterSlot.SecHKSkills = new string[10];
				}
				GameData.CurrentCharacterSlot.SecHKSpells[num2] = null;
				GameData.CurrentCharacterSlot.SecHKItems[num2] = -1;
				GameData.CurrentCharacterSlot.SecHKSkills[num2] = null;
			}
			num2++;
		}
		int num3 = Mathf.Min(PlayerStats.StatusEffects.Length, GameData.CurrentCharacterSlot.SE.Length);
		int num4 = Mathf.Min(PlayerStats.StatusEffects.Length, GameData.CurrentCharacterSlot.SEData.Length);
		for (int j = 0; j < 30; j++)
		{
			if (PlayerStats.StatusEffects[j].Effect != null)
			{
				if (j < num3)
				{
					GameData.CurrentCharacterSlot.SE[j] = PlayerStats.StatusEffects[j].Effect.Id;
				}
				if (j < num4)
				{
					GameData.CurrentCharacterSlot.SEData[j] = new StatusEffectSaveData(PlayerStats.StatusEffects[j].Effect.Id, PlayerStats.StatusEffects[j].Duration);
				}
			}
		}
		GameData.CurrentCharacterSlot.BindLoc = GameData.BindLoc;
		GameData.CurrentCharacterSlot.BindZone = GameData.BindZone;
		GameData.CurrentCharacterSlot.isMale = GameData.PlayerInv.isMale;
		GameData.CurrentCharacterSlot.CurHP = GameData.PlayerStats.CurrentHP;
		GameData.CurrentCharacterSlot.CurMana = GameData.PlayerStats.GetCurrentMana();
		GameData.CurrentCharacterSlot.CurrentXP = GameData.PlayerStats.CurrentExperience;
		GameData.CurrentCharacterSlot.HairName = GameData.PlayerInv.Modulars.HairName;
		GameData.CurrentCharacterSlot.HairColor = GameData.PlayerInv.Modulars.HairCol;
		GameData.CurrentCharacterSlot.SkinColor = GameData.PlayerInv.Modulars.SkinCol;
		GameData.CurrentCharacterSlot.Beard = GameData.PlayerInv.Modulars.Beard;
		GameData.CurrentCharacterSlot.CharClass = GameData.PlayerStats.CharacterClass.ClassName;
		GameData.CurrentCharacterSlot.Gold = GameData.PlayerInv.Gold;
		GameData.CurrentCharacterSlot.SceneName = GameData.SceneName;
		GameData.CurrentCharacterSlot.StrProf = GameData.PlayerStats.StrScaleSpent;
		GameData.CurrentCharacterSlot.EndProf = GameData.PlayerStats.EndScaleSpent;
		GameData.CurrentCharacterSlot.DexProf = GameData.PlayerStats.DexScaleSpent;
		GameData.CurrentCharacterSlot.AgiProf = GameData.PlayerStats.AgiScaleSpent;
		GameData.CurrentCharacterSlot.IntProf = GameData.PlayerStats.IntScaleSpent;
		GameData.CurrentCharacterSlot.WisProf = GameData.PlayerStats.WisScaleSpent;
		GameData.CurrentCharacterSlot.ChaProf = GameData.PlayerStats.ChaScaleSpent;
		GameData.CurrentCharacterSlot.PlayerGuildID = GameData.PlayerControl.MyGuild;
		GameData.CurrentCharacterSlot.ReliqDest = GameData.ReliqDest;
		GameData.CurrentCharacterSlot.ReliqLanding = GameData.ReliqLanding;
		GameData.CurrentCharacterSlot.SunInReliqZone = GameData.SunInReliqZone;
		if (GameData.SunInBindZone)
		{
			GameData.CurrentCharacterSlot.SunInBindZone = 1;
		}
		else
		{
			GameData.CurrentCharacterSlot.SunInBindZone = 0;
		}
		if (GameData.CompletedQuests.Count > 0)
		{
			foreach (string completedQuest in GameData.CompletedQuests)
			{
				GameData.CurrentCharacterSlot.CompletedQuests.Add(completedQuest);
			}
		}
		if (GameData.HasQuest.Count > 0)
		{
			foreach (string item3 in GameData.HasQuest)
			{
				GameData.CurrentCharacterSlot.ActiveQuests.Add(item3);
			}
		}
		GameData.CurrentCharacterSlot.LastPlayedDay = DateTime.Now.DayOfYear;
		GameData.CurrentCharacterSlot.LastPlayedYear = DateTime.Now.Year;
		GameData.CurrentCharacterSlot.HideHelm = GameData.HidePlayerHelm;
		GlobalFactionManager.PushFactionToSave();
		if (GameData.SaveData())
		{
			UpdateSocialLog.LogAdd("Save file updated", "yellow");
		}
		else
		{
			UpdateSocialLog.LogAdd("Save file update faled", "yellow");
		}
		AuctionHouse.SavePlayerAHData();
		GameData.GEcMngr.SaveGEc();
	}

	public void ShowControlScheme()
	{
		ControlScheme.SetActive(!ControlScheme.activeSelf);
	}

	public void OpenBook(string _title)
	{
		if (AllBooks.Books.ContainsKey(_title))
		{
			BookUI.SetActive(value: true);
			BookUI.GetComponent<BookHost>().LoadBookByKey(_title);
		}
	}

	public void OpenAscensionWindow()
	{
		AscensionWindow.SetActive(value: true);
		AscensionWindow.GetComponent<AAScreen>().SetGeneral();
		AscensionText.color = Color.white;
		InvButtonText.color = Color.yellow;
	}

	public void CloseAscensionWindow()
	{
		AscensionWindow.GetComponent<AAScreen>().CloseAllButtons();
		AscensionWindow.SetActive(value: false);
		AscensionText.color = Color.yellow;
		InvButtonText.color = Color.white;
	}

	public void LoadBackQuestAchievements()
	{
		foreach (string completedQuest in GameData.CompletedQuests)
		{
			if (GameData.QuestDB.GetQuestByName(completedQuest) != null && !string.IsNullOrEmpty(GameData.QuestDB.GetQuestByName(completedQuest).SetAchievementOnFinish))
			{
				SetAchievement.Unlock(GameData.QuestDB.GetQuestByName(completedQuest).SetAchievementOnFinish);
			}
			if (GameData.QuestDB.GetQuestByName(completedQuest) != null && !string.IsNullOrEmpty(GameData.QuestDB.GetQuestByName(completedQuest).SetAchievementOnGet))
			{
				SetAchievement.Unlock(GameData.QuestDB.GetQuestByName(completedQuest).SetAchievementOnGet);
			}
		}
		foreach (string item in GameData.HasQuest)
		{
			if (GameData.QuestDB.GetQuestByName(item) != null && !string.IsNullOrEmpty(GameData.QuestDB.GetQuestByName(item).SetAchievementOnGet))
			{
				SetAchievement.Unlock(GameData.QuestDB.GetQuestByName(item).SetAchievementOnGet);
			}
		}
		int level = GameData.PlayerStats.Level;
		if (level >= 2)
		{
			SetAchievement.Unlock("LEVEL2");
		}
		if (level >= 5)
		{
			SetAchievement.Unlock("LEVEL5");
		}
		if (level >= 10)
		{
			SetAchievement.Unlock("LEVEL10");
		}
		if (level >= 15)
		{
			SetAchievement.Unlock("LEVEL15");
		}
		if (level >= 20)
		{
			SetAchievement.Unlock("LEVEL20");
		}
		if (level >= 25)
		{
			SetAchievement.Unlock("LEVEL25");
		}
		if (level >= 30)
		{
			SetAchievement.Unlock("LEVEL30");
		}
		if (level >= 35)
		{
			SetAchievement.Unlock("LEVEL35");
		}
	}

	public void OpenPlanningWindow()
	{
		PlanningTable.SetActive(value: true);
		GameData.PlayerInv.ForceOpenInv();
		GameData.PlanningUI.UpdateTableUI();
	}

	public void ToggleUIEditing()
	{
		GameData.EditUIMode = !GameData.EditUIMode;
	}

	public void ClosePlanningWindow()
	{
		SaveReliq();
		PlanningTable.SetActive(value: false);
	}

	public void SaveReliq()
	{
		ReliquarySave reliquarySave = new ReliquarySave();
		reliquarySave.L1 = GameData.PlanningUI.L1.MyItem.Id;
		reliquarySave.L2 = GameData.PlanningUI.L2.MyItem.Id;
		reliquarySave.L3 = GameData.PlanningUI.L3.MyItem.Id;
		reliquarySave.L4 = GameData.PlanningUI.L4.MyItem.Id;
		reliquarySave.R1 = GameData.PlanningUI.R1.MyItem.Id;
		reliquarySave.R2 = GameData.PlanningUI.R2.MyItem.Id;
		reliquarySave.R3 = GameData.PlanningUI.R3.MyItem.Id;
		reliquarySave.R4 = GameData.PlanningUI.R4.MyItem.Id;
		reliquarySave.Rune1 = GameData.PlanningUI.Rune1.MyItem.Id;
		reliquarySave.Rune2 = GameData.PlanningUI.Rune2.MyItem.Id;
		reliquarySave.Rune3 = GameData.PlanningUI.Rune3.MyItem.Id;
		reliquarySave.Rune4 = GameData.PlanningUI.Rune4.MyItem.Id;
		reliquarySave.Rune5 = GameData.PlanningUI.Rune5.MyItem.Id;
		reliquarySave.Rune6 = GameData.PlanningUI.Rune6.MyItem.Id;
		if (GameData.AccountClaimedReliquary)
		{
			reliquarySave.ReliqClaimed = 1;
		}
		else
		{
			reliquarySave.ReliqClaimed = 0;
		}
		string text = "ReliquaryData";
		string text2 = Path.Combine(Application.persistentDataPath, "ESSaveData");
		string text3 = Path.Combine(text2, text);
		string text4 = text3 + ".tmp";
		string destinationBackupFileName = Path.Combine(Application.persistentDataPath, "backups", text + ".bak");
		if (!Directory.Exists(text2))
		{
			Directory.CreateDirectory(text2);
		}
		if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "backups")))
		{
			Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "backups"));
		}
		string value = JsonUtility.ToJson(reliquarySave, prettyPrint: true);
		if (string.IsNullOrEmpty(value))
		{
			Debug.LogWarning("Guild Data save failed: empty JSON for " + text);
			return;
		}
		using (FileStream fileStream = new FileStream(text4, FileMode.Create, FileAccess.Write, FileShare.None))
		{
			using StreamWriter streamWriter = new StreamWriter(fileStream);
			streamWriter.Write(value);
			streamWriter.Flush();
			fileStream.Flush(flushToDisk: true);
		}
		if (File.Exists(text3))
		{
			File.Replace(text4, text3, destinationBackupFileName);
		}
		else
		{
			File.Move(text4, text3);
		}
	}

	public void LoadReliquary()
	{
		ReliquarySave reliquarySave = new ReliquarySave();
		string text = "ReliquaryData";
		string path = Path.Combine(Application.persistentDataPath, text);
		string path2 = Path.Combine(Path.Combine(Application.persistentDataPath, "ESSaveData"), text);
		string path3 = Path.Combine(Application.persistentDataPath, "backups", text + ".bak");
		if (!File.Exists(path2))
		{
			if (File.Exists(path))
			{
				reliquarySave = JsonUtility.FromJson<ReliquarySave>(File.ReadAllText(path));
			}
			else
			{
				if (!File.Exists(path3))
				{
					return;
				}
				reliquarySave = JsonUtility.FromJson<ReliquarySave>(File.ReadAllText(path3));
			}
		}
		else
		{
			string json = File.ReadAllText(path2);
			try
			{
				reliquarySave = JsonUtility.FromJson<ReliquarySave>(json);
			}
			catch
			{
				if (File.Exists(path3))
				{
					try
					{
						reliquarySave = JsonUtility.FromJson<ReliquarySave>(File.ReadAllText(path3));
					}
					catch
					{
						reliquarySave = new ReliquarySave();
					}
				}
				else
				{
					reliquarySave = new ReliquarySave();
				}
			}
		}
		GameData.PlanningUI.InitToEmpty();
		GameData.PlanningUI.L1.MyItem = GameData.ItemDB.GetItemByID(reliquarySave.L1);
		GameData.PlanningUI.L2.MyItem = GameData.ItemDB.GetItemByID(reliquarySave.L2);
		GameData.PlanningUI.L3.MyItem = GameData.ItemDB.GetItemByID(reliquarySave.L3);
		GameData.PlanningUI.L4.MyItem = GameData.ItemDB.GetItemByID(reliquarySave.L4);
		GameData.PlanningUI.R1.MyItem = GameData.ItemDB.GetItemByID(reliquarySave.R1);
		GameData.PlanningUI.R2.MyItem = GameData.ItemDB.GetItemByID(reliquarySave.R2);
		GameData.PlanningUI.R3.MyItem = GameData.ItemDB.GetItemByID(reliquarySave.R3);
		GameData.PlanningUI.R4.MyItem = GameData.ItemDB.GetItemByID(reliquarySave.R4);
		GameData.PlanningUI.Rune1.MyItem = GameData.ItemDB.GetItemByID(reliquarySave.Rune1);
		GameData.PlanningUI.Rune2.MyItem = GameData.ItemDB.GetItemByID(reliquarySave.Rune2);
		GameData.PlanningUI.Rune3.MyItem = GameData.ItemDB.GetItemByID(reliquarySave.Rune3);
		GameData.PlanningUI.Rune4.MyItem = GameData.ItemDB.GetItemByID(reliquarySave.Rune4);
		GameData.PlanningUI.Rune5.MyItem = GameData.ItemDB.GetItemByID(reliquarySave.Rune5);
		GameData.PlanningUI.Rune6.MyItem = GameData.ItemDB.GetItemByID(reliquarySave.Rune6);
		if (reliquarySave.ReliqClaimed == 1)
		{
			GameData.AccountClaimedReliquary = true;
		}
	}
}
