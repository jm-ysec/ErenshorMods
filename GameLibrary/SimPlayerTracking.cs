// SimPlayerTracking
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SimPlayerTracking
{
	public string SimName;

	public float TimeInZone;

	public string CurScene;

	public string TarScene;

	public int simIndex;

	public Vector3 ForceSpawnLoc;

	public bool Grouped;

	public int online = 1;

	public int spokeRecently = 30;

	public string InvitedPlayerToZone = "";

	public bool StayInZone;

	public float ignorePlayer;

	public float OpinionOfPlayer = 5f;

	public float notGrouping;

	public bool seekingPlayer;

	public bool playerFriend;

	public int LoreChase;

	public int GearChase;

	public int SocialChase;

	public int Troublemaker;

	public int DedicationLevel;

	public bool KnowsPlayer;

	public bool GreetedThisSession;

	public bool InTutorial;

	public SimPlayer MyAvatar;

	public Stats MyStats;

	public SimPlayerTracking GroupedWithOtherSim;

	public bool isLeader;

	public bool Caution;

	public bool isPuller;

	public List<string> NewEquippedItems = new List<string>();

	public List<string> CurrentSEs = new List<string>();

	public int CurXp;

	public int Level;

	public int NewLevel;

	public AuctionHouseSave MyAHData;

	public int TiedToSlot;

	public bool FakeInvited;

	public bool PersonalInvite;

	public int Planars;

	public int Sivaks;

	public int FriendedBy = -1;

	public SimPlayerMemory MyCurrentMemory;

	public SimPlayerMemory MyPreviousMemory;

	public float FriendLevel;

	public string GuildID;

	public int IsGuildLeader;

	public float Greed;

	public bool IsGMCharacter;

	public bool InvitedToGuildThisSession;

	public bool InvitedPlayerToGuild;

	public bool OpenToGuildInvite;

	public string ClassName = "";

	public string Gender = "Male";

	public string HairIndex = "Chr_Hair_01";

	public int HairColor;

	public int SkinColor;

	public int Personality = 1;

	public int BioIndex = 1;

	public bool Rival;

	public int HideHelm;

	public bool askedForGuildInvite;

	public bool GoingToReliq;

	public float HoldDPSNum = 100f;

	public SimPlayerTracking(string name, float _time, string _sceneName, int _index)
	{
		SimName = name;
		TimeInZone = _time;
		CurScene = _sceneName;
		simIndex = _index;
		online = 1;
		FriendedBy = -1;
		MyCurrentMemory = new SimPlayerMemory("", 0, 0, 0, 0, 0, "", DateTime.Now.DayOfYear, DateTime.Now.Year, 0, 2000, "");
		int num = UnityEngine.Random.Range(1, 39);
		string text = "Chr_Hair_";
		HairIndex = ((num >= 10) ? (text + num) : (text + "0" + num));
	}

	public void TravelToZone(string _zoneName)
	{
		TarScene = _zoneName;
		seekingPlayer = true;
	}

	public void PopulateData(SimPlayer _sim)
	{
		LoreChase = _sim.LoreChase;
		GearChase = _sim.GearChase;
		SocialChase = _sim.SocialChase;
		Troublemaker = _sim.Troublemaker;
		DedicationLevel = _sim.DedicationLevel;
		InTutorial = _sim.InTutorial;
		SimName = _sim.GetComponent<NPC>().NPCName;
	}

	public SimPlayer SpawnMeInGame(Vector3 pos, SimPlayerTracking _sim)
	{
		GameData.SimMngr.BlankSPTemplate.GetComponent<NPC>().NPCName = _sim.SimName;
		GameObject gameObject = null;
		foreach (GameObject actualSim in GameData.SimMngr.ActualSims)
		{
			if (actualSim.transform.name == _sim.SimName)
			{
				gameObject = actualSim;
			}
		}
		GameObject blankSPTemplate = GameData.SimMngr.BlankSPTemplate;
		SimPlayer component = blankSPTemplate.GetComponent<SimPlayer>();
		component.LovesEmojis = false;
		component.TypesInAllCaps = false;
		component.TypesInThirdPerson = false;
		component.TypoRate = 0.25f;
		component.TypoChance = 0.25f;
		component.myIndex = _sim.simIndex;
		ModularPar componentInChildren = blankSPTemplate.GetComponentInChildren<ModularPar>();
		if (_sim.Gender == "Male")
		{
			componentInChildren.FemaleBase.SetActive(value: false);
			componentInChildren.MaleBase.SetActive(value: true);
			componentInChildren.Male.enabled = true;
			componentInChildren.Female.enabled = false;
			component.GetComponent<Inventory>().isMale = true;
		}
		else
		{
			componentInChildren.FemaleBase.SetActive(value: true);
			componentInChildren.MaleBase.SetActive(value: false);
			componentInChildren.Male.enabled = false;
			componentInChildren.Female.enabled = true;
			component.GetComponent<Inventory>().isMale = false;
		}
		SimPlayer component2 = blankSPTemplate.GetComponent<SimPlayer>();
		component2.IsGMCharacter = false;
		component2.SignOffLine.Clear();
		component2.Bio = "";
		SimPlayerLanguage simPlayerLanguage = null;
		if (gameObject != null)
		{
			SimPlayer component3 = gameObject.GetComponent<SimPlayer>();
			simPlayerLanguage = gameObject.GetComponent<SimPlayerLanguage>();
			component.TypesInAllCaps = component3.TypesInAllCaps;
			component.TypesInAllCaps = component3.TypesInAllCaps;
			component.LovesEmojis = component3.LovesEmojis;
			component.TypesInAllCaps = component3.TypesInAllCaps;
			component.TypesInThirdPerson = component3.TypesInThirdPerson;
			component.TypoRate = component3.TypoRate;
			component.TypoChance = component3.TypoChance;
			LoadSpawnTemplateFromPremade(gameObject.GetComponent<SimPlayer>(), component2);
		}
		GameObject gameObject2 = UnityEngine.Object.Instantiate(blankSPTemplate, pos, GameData.PlayerControl.transform.rotation);
		GameData.SimMngr.LoadSimPlayerTemplate(gameObject2, _sim);
		if (gameObject != null && simPlayerLanguage != null)
		{
			LoadLanguageFromPremade(simPlayerLanguage, gameObject2);
		}
		gameObject2.transform.name = _sim.SimName;
		gameObject2.GetComponent<NPC>().NPCName = _sim.SimName;
		MyAvatar = gameObject2.GetComponent<SimPlayer>();
		MyAvatar.InGroup = GameData.SimMngr.Sims[MyAvatar.myIndex].Grouped;
		MyAvatar.GetThisNPC().InGroup = MyAvatar.InGroup;
		MyStats = MyAvatar.GetComponent<Stats>();
		SimName = gameObject2.GetComponent<NPC>().NPCName;
		component2.IsGMCharacter = false;
		component2.SignOffLine.Clear();
		component2.Bio = "";
		component2.LovesEmojis = false;
		component2.TypesInAllCaps = false;
		component2.TypesInThirdPerson = false;
		component2.TypoChance = 0.25f;
		component2.TypoRate = 0.25f;
		return gameObject2.GetComponent<SimPlayer>();
	}

	private void LoadSpawnTemplateFromPremade(SimPlayer SimToTakeInfoFrom, SimPlayer actualSimToLoad)
	{
		actualSimToLoad.GetComponent<SimPlayerLanguage>();
		SimPlayer component = actualSimToLoad.GetComponent<SimPlayer>();
		component.Bio = SimToTakeInfoFrom.Bio;
		component.HairColor = SimToTakeInfoFrom.HairColor;
		component.SkinColor = SimToTakeInfoFrom.SkinColor;
		component.HairName = actualSimToLoad.HairName;
		component.GearChase = SimToTakeInfoFrom.GearChase;
		component.Greed = SimToTakeInfoFrom.Greed;
		component.Patience = SimToTakeInfoFrom.Patience;
		component.IsGMCharacter = SimToTakeInfoFrom.IsGMCharacter;
		component.SignOffLine = new List<string>();
		foreach (string item in SimToTakeInfoFrom.SignOffLine)
		{
			component.SignOffLine.Add(item);
		}
	}

	private void LoadLanguageFromPremade(SimPlayerLanguage _lang, GameObject actualSimToLoad)
	{
		SimPlayerLanguage component = actualSimToLoad.GetComponent<SimPlayerLanguage>();
		component.Greetings.Clear();
		foreach (string greeting in _lang.Greetings)
		{
			component.Greetings.Add(greeting);
		}
		component.ReturnGreeting.Clear();
		foreach (string item in _lang.ReturnGreeting)
		{
			component.ReturnGreeting.Add(item);
		}
		component.Invites.Clear();
		foreach (string invite in _lang.Invites)
		{
			component.Invites.Add(invite);
		}
		component.Justifications.Clear();
		foreach (string justification in _lang.Justifications)
		{
			component.Justifications.Add(justification);
		}
		component.Confirms.Clear();
		foreach (string confirm in _lang.Confirms)
		{
			component.Confirms.Add(confirm);
		}
		component.GenericLines.Clear();
		foreach (string genericLine in _lang.GenericLines)
		{
			component.GenericLines.Add(genericLine);
		}
		component.Aggro.Clear();
		foreach (string item2 in _lang.Aggro)
		{
			component.Aggro.Add(item2);
		}
		component.Died.Clear();
		foreach (string item3 in _lang.Died)
		{
			component.Died.Add(item3);
		}
		component.InsultsFun.Clear();
		foreach (string item4 in _lang.InsultsFun)
		{
			component.InsultsFun.Add(item4);
		}
		component.RetortsFun.Clear();
		foreach (string item5 in _lang.RetortsFun)
		{
			component.RetortsFun.Add(item5);
		}
		component.Exclamations.Clear();
		foreach (string exclamation in _lang.Exclamations)
		{
			component.Exclamations.Add(exclamation);
		}
		component.Denials.Clear();
		foreach (string denial in _lang.Denials)
		{
			component.Denials.Add(denial);
		}
		component.DeclineGroup.Clear();
		foreach (string item6 in _lang.DeclineGroup)
		{
			component.DeclineGroup.Add(item6);
		}
		component.Negative.Clear();
		foreach (string item7 in _lang.Negative)
		{
			component.Negative.Add(item7);
		}
		component.LFGPublic.Clear();
		foreach (string item8 in _lang.LFGPublic)
		{
			component.LFGPublic.Add(item8);
		}
		component.OTW.Clear();
		foreach (string item9 in _lang.OTW)
		{
			component.OTW.Add(item9);
		}
		component.Goodnight.Clear();
		foreach (string item10 in _lang.Goodnight)
		{
			component.Goodnight.Add(item10);
		}
		component.Hello.Clear();
		foreach (string item11 in _lang.Hello)
		{
			component.Hello.Add(item11);
		}
		component.LocalFriendHello.Clear();
		foreach (string item12 in _lang.LocalFriendHello)
		{
			component.LocalFriendHello.Add(item12);
		}
		component.UnsureResponse.Clear();
		foreach (string item13 in _lang.UnsureResponse)
		{
			component.UnsureResponse.Add(item13);
		}
		component.AngerResponse.Clear();
		foreach (string item14 in _lang.AngerResponse)
		{
			component.AngerResponse.Add(item14);
		}
		component.Affirms.Clear();
		foreach (string affirm in _lang.Affirms)
		{
			component.Affirms.Add(affirm);
		}
		component.EnvDmg.Clear();
		foreach (string item15 in _lang.EnvDmg)
		{
			component.EnvDmg.Add(item15);
		}
		component.WantsDrop.Clear();
		foreach (string item16 in _lang.WantsDrop)
		{
			component.WantsDrop.Add(item16);
		}
		component.Gratitude.Clear();
		foreach (string item17 in _lang.Gratitude)
		{
			component.Gratitude.Add(item17);
		}
		component.Impressed.Clear();
		foreach (string item18 in _lang.Impressed)
		{
			component.Impressed.Add(item18);
		}
		component.ImpressedEnd.Clear();
		foreach (string item19 in _lang.ImpressedEnd)
		{
			component.ImpressedEnd.Add(item19);
		}
		component.AcknowledgeGratitude.Clear();
		foreach (string item20 in _lang.AcknowledgeGratitude)
		{
			component.AcknowledgeGratitude.Add(item20);
		}
		component.LevelUpCelebration.Clear();
		foreach (string item21 in _lang.LevelUpCelebration)
		{
			component.LevelUpCelebration.Add(item21);
		}
		component.GoodLastOuting.Clear();
		foreach (string item22 in _lang.GoodLastOuting)
		{
			component.GoodLastOuting.Add(item22);
		}
		component.BadLastOuting.Clear();
		foreach (string item23 in _lang.BadLastOuting)
		{
			component.BadLastOuting.Add(item23);
		}
		component.GotAnItemLastOuting.Clear();
		foreach (string item24 in _lang.GotAnItemLastOuting)
		{
			component.GotAnItemLastOuting.Add(item24);
		}
		component.ReturnToZone.Clear();
		foreach (string item25 in _lang.ReturnToZone)
		{
			component.ReturnToZone.Add(item25);
		}
		component.BeenAWhile.Clear();
		foreach (string item26 in _lang.BeenAWhile)
		{
			component.BeenAWhile.Add(item26);
		}
		component.Unsure.Clear();
		foreach (string item27 in _lang.Unsure)
		{
			component.Unsure.Add(item27);
		}
	}
}
