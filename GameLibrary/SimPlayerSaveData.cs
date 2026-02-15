// SimPlayerSaveData
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SimPlayerSaveData
{
	public List<string> MyEquippedItems = new List<string>();

	public List<int> ItemQuantities = new List<int>();

	public int MHQ = 1;

	public int OHQ = 1;

	public int MyLevel;

	public List<string> RecentNewItems = new List<string>();

	public string NPCName;

	public int PlayerFriendLevel;

	public int XpForLevelUp;

	public float OpinionOfPlayer = 5f;

	public bool HasMetPlayer;

	public float Dedication = 5f;

	public string AuraSlot;

	public int SimGold = 100;

	public int AscensionXP;

	public int AscensionPoints;

	public List<AscensionSkillEntry> MyAscensions = new List<AscensionSkillEntry>();

	public List<string> AllInventory = new List<string>();

	public int Year = 1;

	public int Day = 1;

	public int Hour = 1;

	public int Min = 1;

	public List<string> StoredItems = new List<string>();

	public bool War;

	public bool Arc;

	public bool Duel;

	public bool Dru;

	public bool Storm;

	public bool Reav;

	public float MySkill = 40f;

	public int TiedToSlot;

	public int PersonalityType;

	public int BioIndex = -1;

	public Color SkinColor = new Color(190f, 156f, 137f, 255f);

	public Color HairColor = new Color(108f, 107f, 99f, 255f);

	public int SkinColorIndex = -1;

	public int HairColorIndex = -1;

	public int Beard = 1;

	public string hairName = "DEFAULT";

	public List<string> AcquiredSkills = new List<string>();

	public bool newlyGenerated;

	public bool DisallowUpgrades;

	public int Sivakruxes;

	public int PlanarStones;

	public int FriendedBy = -1;

	public SimPlayerMemory MyLastAdventure;

	public int StrPointsSpent;

	public int EndPointsSpent;

	public int DexPointsSpent;

	public int AgiPointsSpent;

	public int IntPointsSpent;

	public int WisPointsSpent;

	public int ChaPointsSpent;

	public float Greed = 1f;

	public string GuildID = "";

	public int IsGuildLeader;

	public int Male;

	public int HideHelm;

	public SimPlayerSaveData(string _myName, int _level, List<Item> inv, float _skill)
	{
		War = false;
		Arc = false;
		Dru = false;
		Duel = false;
		Storm = false;
		Reav = false;
		MySkill = _skill;
		NPCName = _myName;
		foreach (Item item in inv)
		{
			MyEquippedItems.Add(item.Id);
		}
		MyLevel = _level;
		OpinionOfPlayer = 5f;
		SimGold = 100;
		HasMetPlayer = false;
		FriendedBy = -1;
		Year = DateTime.Now.Year;
		Day = DateTime.Now.DayOfYear;
		Hour = DateTime.Now.Hour;
		Min = DateTime.Now.Minute;
	}
}
