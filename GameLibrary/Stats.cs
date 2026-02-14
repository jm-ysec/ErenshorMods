// Stats
using System;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
	public Class CharacterClass;

	public string MyName = "";

	public int Level = 1;

	public int CurrentExperience;

	public int ExperienceToLevelUp;

	public int CurrentAscensionXP;

	public int AscensionXPtoLevelUp = 38000;

	public Inventory MyInv;

	public int BaseHP;

	public int BaseAC = 5;

	public int BaseMana;

	public int BaseStr = 3;

	public int BaseEnd = 3;

	public int BaseDex = 3;

	public int BaseAgi = 3;

	public int BaseInt = 3;

	public int BaseWis = 3;

	public int BaseCha = 3;

	public int BaseRes;

	public int BaseMR;

	public int BaseER;

	public int BasePR;

	public int BaseVR;

	public Stance CombatStance;

	public float BaseLifesteal;

	public float RunSpeed;

	public float BaseMHAtkDelay = 60f;

	private float ActualMHAtkDelay;

	public float BaseOHAtkDelay = 60f;

	private float ActualOHAtkDelay;

	public int CurrentHP = 1;

	public int CurrentMaxHP = 1;

	public int CurrentAC;

	public int CurrentMana = 100;

	private int CurrentMaxMana;

	public int CurrentStamina = 100;

	public int CurrentMaxStamina = 100;

	private int CurrentStr;

	private int CurrentEnd;

	private int CurrentDex;

	private int CurrentAgi;

	private int CurrentInt;

	private int CurrentWis;

	private int CurrentCha;

	private int CurrentRes;

	private int CurrentMR;

	private int CurrentER;

	private int CurrentPR;

	private int CurrentVR;

	private int CurrentDS;

	private float CurrentMHAtkDelay;

	private float CurrentOHAtkDelay;

	private int seHP;

	private int seAC;

	private int seMana;

	private int seStr;

	private int seEnd;

	private int seDex;

	private int seAgi;

	private int seInt;

	private int seWis;

	private int seCha;

	private int seRes;

	private int seDS;

	private float seXPBonus;

	private int seAtkMod;

	private int seRegen;

	private int seMR;

	private int seER;

	private int sePR;

	private int seVR;

	private int seManaRegen;

	private float seRunSpeed;

	private float seWeapHaste;

	private float seOverhaste;

	public float actualRunSpeed;

	public float AttackAbility;

	private float seLifesteal;

	public float PercentLifesteal;

	public bool DualWield;

	public int SpellShield;

	public int AtkRollModifier;

	public bool OverrideHPforNPC = true;

	public float RecentDmgByPlayer;

	public List<StatusEffectIcon> StatusIcons = new List<StatusEffectIcon>();

	private float TickTime = 300f;

	public bool Rooted;

	public bool Stunned;

	public bool Feared;

	public Character Myself;

	public bool Charmed;

	private CastSpell MySpells;

	public float RecentDmg;

	public float RecentCast;

	private int currentShieldSpell = -1;

	public Spell MyAura;

	public ParticleSystem LvlUp;

	public bool StopAllRegen;

	public float resonanceCD;

	private float XPBonus;

	public bool Unstunnable;

	public bool PlayerOrSimPlayer;

	public int StrScaleMod;

	public int EndScaleMod;

	public int DexScaleMod;

	public int AgiScaleMod;

	public int IntScaleMod;

	public int WisScaleMod;

	public int ChaScaleMod;

	public float MitScaleMod;

	public float RstScaleMod;

	public int StrScaleSpent;

	public int EndScaleSpent;

	public int DexScaleSpent;

	public int AgiScaleSpent;

	public int IntScaleSpent;

	public int WisScaleSpent;

	public int ChaScaleSpent;

	public int TotalSpentProficiencies;

	public int TotalAvailableProficiencies;

	private int ExpectedSpent;

	private bool medMsg;

	public bool isRetreating;

	public float StunCooldown;

	public bool Invisible;

	public StatusEffect[] StatusEffects { get; } = new StatusEffect[30];


	private void Awake()
	{
		if (Myself == null)
		{
			Myself = GetComponent<Character>();
		}
		if (GetComponent<PlayerControl>() != null)
		{
			GameData.PlayerStats = this;
			PlayerOrSimPlayer = true;
		}
		if (GetComponent<SimPlayer>() != null)
		{
			PlayerOrSimPlayer = true;
		}
		for (int i = 0; i < 30; i++)
		{
			StatusEffects[i] = new StatusEffect(null, _fromPlayer: false, _CastedByPC: false, null);
		}
		if (MyInv == null)
		{
			MyInv = GetComponent<Inventory>();
		}
	}

	private void Start()
	{
		if (CharacterClass == null)
		{
			CharacterClass = GameData.EffectDB.DefaultClass;
		}
		if (Myself.isNPC && GetComponent<SimPlayer>() == null && !GetComponent<NPC>().HandSetResistances)
		{
			BaseMR = ReturnBaseResitances(Level);
			BaseER = ReturnBaseResitances(Level);
			BasePR = ReturnBaseResitances(Level);
			BaseVR = ReturnBaseResitances(Level);
			if (GetComponent<NPC>().BaseAtkDmg < Level)
			{
				GetComponent<NPC>().BaseAtkDmg = Level;
			}
		}
		if (MyInv == null)
		{
			MyInv = GetComponent<Inventory>();
		}
		if (Myself.isNPC && !GetComponent<NPC>().SimPlayer)
		{
			OverrideHPforNPC = true;
		}
		CalcStats();
		CurrentHP = CurrentMaxHP;
		CurrentMana = BaseMana;
		MySpells = GetComponent<CastSpell>();
		SetXpForLevelUp();
		CombatStance = GameData.SkillDatabase.NormalStance;
	}

	private int ReturnBaseResitances(int mobLevel)
	{
		return Mathf.RoundToInt((float)Level * UnityEngine.Random.Range(0.5f, 1.2f));
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Delete) && GetComponent<TestDummy>() != null)
		{
			CalcStats();
		}
		if (StunCooldown > 0f)
		{
			StunCooldown -= 60f * Time.deltaTime;
		}
		if (isRetreating && actualRunSpeed > 7f)
		{
			CalcStats();
		}
		if (resonanceCD > 0f)
		{
			resonanceCD -= 60f * Time.deltaTime;
		}
		if (RecentCast > 0f)
		{
			RecentCast -= 60f * Time.deltaTime;
			medMsg = false;
		}
		if (RecentCast <= 0f && RecentDmg <= 0f && base.transform.name == "Player" && !medMsg)
		{
			UpdateSocialLog.LogAdd("You settle into a meditative state", "lightblue");
			medMsg = true;
		}
		if (RecentDmgByPlayer > 0f)
		{
			RecentDmgByPlayer -= 60f * Time.deltaTime;
		}
		if (ActualMHAtkDelay > 0f)
		{
			ActualMHAtkDelay -= 60f * Time.deltaTime;
		}
		if (ActualOHAtkDelay > 0f)
		{
			ActualOHAtkDelay -= 60f * Time.deltaTime;
		}
		if (Myself.Alive && GameData.SceneName != "LoadScene" && (Myself.isNPC || GameData.PlayerControl.enabled))
		{
			TickTime -= 60f * Time.deltaTime;
			if (TickTime <= 0f)
			{
				CheckAuras();
				TickTime = 180f;
				TickEffects();
				if (!StopAllRegen)
				{
					float num = 1f;
					if (Myself.isNPC && Myself.MyNPC != null && Myself.MyNPC.SimPlayer && Myself.MyNPC.ThisSim != null && Myself.MyNPC.ThisSim.sitting)
					{
						num = 25f;
					}
					if (!Myself.isNPC && GameData.PlayerControl.Sitting)
					{
						num += 25f;
					}
					if (RecentDmg <= 0f && RecentCast <= 0f && (!Myself.isNPC || !(Myself.MyNPC.CurrentAggroTarget != null)))
					{
						num = 50f;
					}
					if (Myself.isNPC && Myself.MyNPC != null && !Myself.MyNPC.SimPlayer && Myself.MyNPC.CurrentAggroTarget != null)
					{
						if (Vector3.Distance(base.transform.position, Myself.MyNPC.CurrentAggroTarget.transform.position) < 40f)
						{
							num = 0.5f;
						}
						else if (Vector3.Distance(base.transform.position, Myself.MyNPC.CurrentAggroTarget.transform.position) < 60f)
						{
							num = 3f;
						}
					}
					RegenEffects(num);
				}
			}
			if (RecentDmg >= 0f)
			{
				medMsg = false;
				RecentDmg -= 60f * Time.deltaTime;
			}
		}
		else if (RecentDmg > 0f)
		{
			RecentDmg = 0f;
		}
		if (CurrentMana > CurrentMaxMana)
		{
			CurrentMana = CurrentMaxMana;
		}
		if (CurrentHP > CurrentMaxHP)
		{
			CurrentHP = CurrentMaxHP;
		}
	}

	public void CalcStats()
	{
		Stance stance = CombatStance;
		if (stance == null)
		{
			stance = GameData.SkillDatabase.NormalStance;
		}
		float num = (float)Myself.MySkills.GetAscensionRank("6643117") * 0.05f;
		num += (float)Myself.MySkills.GetAscensionRank("30223144") * 0.1f;
		num += (float)Myself.MySkills.GetAscensionRank("18639750") * 0.1f;
		float num2 = (float)Myself.MySkills.GetAscensionRank("79142720") * 0.05f;
		float num3 = (float)Myself.MySkills.GetAscensionRank("27534800") * 0.05f;
		float num4 = (float)Myself.MySkills.GetAscensionRank("21032190") * 0.05f;
		float num5 = (float)Myself.MySkills.GetAscensionRank("35032770") * 0.05f;
		float num6 = (float)Myself.MySkills.GetAscensionRank("27274762") * 0.05f;
		float num7 = (float)Myself.MySkills.GetAscensionRank("63223382") * 0.05f;
		num4 += (float)Myself.MySkills.GetAscensionRank("4886685") * 0.03f;
		num6 += (float)Myself.MySkills.GetAscensionRank("4886685") * 0.03f;
		num5 += (float)Myself.MySkills.GetAscensionRank("4886685") * 0.03f;
		num7 += (float)Myself.MySkills.GetAscensionRank("4886685") * 0.03f;
		TotalSpentProficiencies = StrScaleSpent + EndScaleSpent + DexScaleSpent + AgiScaleSpent + IntScaleSpent + WisScaleSpent + ChaScaleSpent;
		ExpectedSpent = 10 + Mathf.Max(0, Mathf.FloorToInt((float)(Level - 2) / 2f) + 1);
		TotalAvailableProficiencies = Mathf.Max(0, ExpectedSpent - TotalSpentProficiencies);
		CountStatusEffects();
		CurrentStr = BaseStr + MyInv.ItemStr + seStr;
		CurrentEnd = BaseEnd + MyInv.ItemEnd + seEnd;
		CurrentDex = BaseDex + MyInv.ItemDex + seDex;
		CurrentAgi = BaseAgi + MyInv.ItemAgi + seAgi;
		CurrentInt = BaseInt + MyInv.ItemInt + seInt;
		CurrentWis = BaseWis + MyInv.ItemWis + seWis;
		CurrentCha = BaseCha + MyInv.ItemCha + seCha;
		CurrentRes = Mathf.RoundToInt((float)(BaseRes + MyInv.ItemRes + seRes) * stance.ResonanceAmount);
		AtkRollModifier = seAtkMod;
		StrScaleMod = Mathf.Clamp(CharacterClass.StrBenefit + MyInv.StrScaleMod + StrScaleSpent, 1, 40);
		EndScaleMod = Mathf.Clamp(CharacterClass.EndBenefit + MyInv.EndScaleMod + EndScaleSpent, 1, 40);
		DexScaleMod = Mathf.Clamp(CharacterClass.DexBenefit + MyInv.DexScaleMod + DexScaleSpent, 1, 40);
		AgiScaleMod = Mathf.Clamp(CharacterClass.AgiBenefit + MyInv.AgiScaleMod + AgiScaleSpent, 1, 40);
		IntScaleMod = Mathf.Clamp(CharacterClass.IntBenefit + MyInv.IntScaleMod + IntScaleSpent, 1, 40);
		WisScaleMod = Mathf.Clamp(CharacterClass.WisBenefit + MyInv.WisScaleMod + WisScaleSpent, 1, 40);
		ChaScaleMod = Mathf.Clamp(CharacterClass.ChaBenefit + MyInv.ChaScaleMod + ChaScaleSpent, 1, 40);
		MitScaleMod = CharacterClass.MitigationBonus + MyInv.MitScaleMod / 100f;
		CurrentMR = BaseMR + MyInv.ItemMR + seMR + ChaScaleMod / 100 * GetCurrentCha();
		CurrentER = BaseER + MyInv.ItemER + seER + ChaScaleMod / 100 * GetCurrentCha();
		CurrentPR = BasePR + MyInv.ItemPR + sePR + ChaScaleMod / 100 * GetCurrentCha();
		CurrentVR = BaseVR + MyInv.ItemVR + seVR + ChaScaleMod / 100 * GetCurrentCha();
		CurrentDS = seDS;
		XPBonus = seXPBonus;
		CurrentMHAtkDelay = BaseMHAtkDelay + MyInv.MHDelay * 60f - (BaseMHAtkDelay + MyInv.MHDelay * 60f) * seWeapHaste / 100f;
		if (!Myself.isNPC)
		{
			if (CurrentMHAtkDelay < 80f)
			{
				GameData.PlayerControl.GetAnim().SetFloat("AttackSpeed", 1.25f);
			}
			else
			{
				GameData.PlayerControl.GetAnim().SetFloat("AttackSpeed", 1f);
			}
		}
		float num8 = RunSpeed + seRunSpeed;
		if (isRetreating && num8 > 7f)
		{
			num8 = 7f;
		}
		if (num8 <= 2f)
		{
			num8 = 2f;
		}
		actualRunSpeed = num8;
		PercentLifesteal = (BaseLifesteal + seLifesteal) * stance.LifestealAmount;
		if (!OverrideHPforNPC)
		{
			CurrentMaxHP = Mathf.RoundToInt((float)(BaseHP + MyInv.ItemHP + seHP + Level * 5 + (GetCurrentEnd() * (2 * EndScaleMod) / 100 + GetCurrentEnd() * Mathf.RoundToInt((float)Level / 200f)) * Level) * stance.MaxHPMod);
		}
		else
		{
			CurrentMaxHP = BaseHP;
		}
		if ((Myself != null && !Myself.isNPC) || GetComponent<SimPlayer>() != null)
		{
			CurrentAC = BaseAC + MyInv.ItemAC + seAC + Mathf.RoundToInt((float)CurrentAgi * (float)AgiScaleMod / 200f * (float)Level);
		}
		else if (Myself.MyNPC != null)
		{
			if (Myself.MyNPC.HardSetAC == 0)
			{
				CurrentAC = Level * 15 + seAC;
			}
			else
			{
				CurrentAC = Myself.MyNPC.HardSetAC + seAC;
			}
		}
		else
		{
			CurrentAC = Level * 15 + seAC;
		}
		CurrentAC = Mathf.RoundToInt((float)CurrentAC * CharacterClass.MitigationBonus);
		CurrentMaxMana = BaseMana + MyInv.ItemMana + IntScaleMod * Level + WisScaleMod * Level + Mathf.RoundToInt((float)CurrentInt * ((float)IntScaleMod / 3f));
		if ((Myself != null && !Myself.isNPC) || GetComponent<SimPlayer>() != null)
		{
			AttackAbility = (float)Level * 10f + (float)CurrentDex * 1.5f + (float)CurrentDex * ((float)DexScaleMod / 10f);
		}
		else
		{
			float num9 = 100 + (Level - 1) * 40;
			if (Level >= 20)
			{
				float num10 = Mathf.Clamp01(((float)Level - 20f) / 20f);
				float num11 = 3f * num10 * num10 - 2f * num10 * num10 * num10;
				float num12 = 0.33f * num11;
				num9 += num9 * num12;
			}
			num9 = (AttackAbility = ((!(Myself.MyNPC != null)) ? (num9 * GetComponent<NPC>().ArmorPenMult) : (num9 * Myself.MyNPC.ArmorPenMult)));
		}
		CurrentMaxHP += Mathf.RoundToInt((float)CurrentMaxHP * num);
		CurrentMaxMana += Mathf.RoundToInt((float)CurrentMaxMana * num3);
		CurrentAC += Mathf.RoundToInt((float)CurrentAC * num2);
		CurrentMR += Mathf.RoundToInt((float)CurrentMR * num5);
		CurrentER += Mathf.RoundToInt((float)CurrentER * num4);
		CurrentPR += Mathf.RoundToInt((float)CurrentPR * num6);
		CurrentVR += Mathf.RoundToInt((float)CurrentVR * num7);
	}

	public void EarnedXP(int _incomingXP)
	{
		if (GameData.XPLock == 1)
		{
			if (!Myself.isNPC)
			{
				UpdateSocialLog.LogAdd("Experience Lock is enabled. To retore experience gains, type /explock", "yellow");
			}
			return;
		}
		if (Myself.isNPC && Myself.MyNPC.SimPlayer)
		{
			if (GameData.GroupMembers[0] != null && GameData.GroupMembers[0] == GameData.SimMngr.Sims[Myself.MyNPC.ThisSim.myIndex])
			{
				XPBonus = GameData.PlayerStats.XPBonus;
			}
			if (GameData.GroupMembers[1] != null && GameData.GroupMembers[1] == GameData.SimMngr.Sims[Myself.MyNPC.ThisSim.myIndex])
			{
				XPBonus = GameData.PlayerStats.XPBonus;
			}
			if (GameData.GroupMembers[2] != null && GameData.GroupMembers[2] == GameData.SimMngr.Sims[Myself.MyNPC.ThisSim.myIndex])
			{
				XPBonus = GameData.PlayerStats.XPBonus;
			}
			if (GameData.GroupMembers[3] != null && GameData.GroupMembers[3] == GameData.SimMngr.Sims[Myself.MyNPC.ThisSim.myIndex])
			{
				XPBonus = GameData.PlayerStats.XPBonus;
			}
		}
		int num = Mathf.RoundToInt((float)_incomingXP * XPBonus);
		if (Myself.MyNPC != null && Myself.MyNPC.SimPlayer && (GameData.ZoneAnnounceData?.MobsKilledByPlayerParty ?? 0) >= 10)
		{
			if (GameData.SimMngr?.Sims[Myself.MyNPC.ThisSim.myIndex].MyCurrentMemory != null)
			{
				GameData.SimMngr.Sims[Myself.MyNPC.ThisSim.myIndex].MyCurrentMemory.XPGain += _incomingXP + num;
				GameData.SimMngr.Sims[Myself.MyNPC.ThisSim.myIndex].MyCurrentMemory.ZoneName = GameData.ZoneAnnounceData.ZoneName;
				GameData.SimMngr.Sims[Myself.MyNPC.ThisSim.myIndex].MyCurrentMemory.PlayedDay = DateTime.Now.DayOfYear;
				GameData.SimMngr.Sims[Myself.MyNPC.ThisSim.myIndex].MyCurrentMemory.PlayedYear = DateTime.Now.Year;
				GameData.SimMngr.Sims[Myself.MyNPC.ThisSim.myIndex].MyCurrentMemory.GroupedLastYear = DateTime.Now.Year;
				GameData.SimMngr.Sims[Myself.MyNPC.ThisSim.myIndex].MyCurrentMemory.GroupedLastDay = DateTime.Now.DayOfYear;
			}
			else
			{
				Debug.Log("Sim Memory not intialized");
			}
		}
		if (Level < 35)
		{
			if (!Myself.isNPC)
			{
				if (num == 0)
				{
					UpdateSocialLog.LogAdd("You've gained " + _incomingXP + " experience!", "yellow");
				}
				else
				{
					UpdateSocialLog.LogAdd("You've gained " + (_incomingXP + num) + " experience!", "yellow");
				}
			}
			_incomingXP += num;
			CurrentExperience += _incomingXP;
			if (Myself.MyNPC != null && Myself.MyNPC.SimPlayer && Myself.MyNPC.ThisSim.InGroup)
			{
				UpdateSocialLog.LogAdd(base.transform.name + " receives " + _incomingXP + " +(" + num + " XP bonus) xp - (" + CurrentExperience + " / " + ExperienceToLevelUp + ")", "yellow");
			}
			if (CurrentExperience >= ExperienceToLevelUp && Level < 35)
			{
				DoLevelUp();
			}
			else if (Level == 35)
			{
				CurrentExperience = ExperienceToLevelUp;
			}
			return;
		}
		CurrentAscensionXP += _incomingXP + num;
		if (!Myself.isNPC)
		{
			if (num == 0)
			{
				UpdateSocialLog.LogAdd("You've gained " + _incomingXP + " ASCENSION experience!", "yellow");
			}
			else
			{
				UpdateSocialLog.LogAdd("You've gained " + (_incomingXP + num) + " ASCENSION experience!", "yellow");
			}
		}
		else
		{
			if (Myself.MyNPC != null && Myself.MyNPC.SimPlayer && Myself.MyNPC.ThisSim.InGroup)
			{
				UpdateSocialLog.LogAdd(base.transform.name + " receives " + _incomingXP + " +(" + num + " XP bonus) ASCENSION xp - (" + CurrentAscensionXP + " / " + AscensionXPtoLevelUp + ")", "yellow");
			}
			if (Myself.MySkills.AscensionPoints > 0)
			{
				SimPlayerChooseAscension();
			}
		}
		if (CurrentAscensionXP >= AscensionXPtoLevelUp)
		{
			Myself.MySkills.AscensionPoints++;
			if (!Myself.isNPC)
			{
				Myself.MySpells.LearnSpell?.Play();
				Myself.MyAudio?.PlayOneShot(GameData.Misc.NewSkill, GameData.SFXVol * GameData.MasterVol);
			}
			CurrentAscensionXP = 0;
			if (!Myself.isNPC)
			{
				UpdateSocialLog.LogAdd("You've gained an ASCENSION POINT!", "yellow");
				SetAchievement.Unlock("ASCENSION");
			}
			if (Myself.isNPC && Myself.MyNPC.SimPlayer)
			{
				SimPlayerChooseAscension();
			}
		}
	}

	public void SimPlayerChooseAscension()
	{
		UseSkill mySkills = Myself.MySkills;
		List<Ascension> list = FindNextAscensions();
		if (list.Count <= 0)
		{
			return;
		}
		int num = -99;
		int num2 = -99;
		Ascension ascension = null;
		foreach (Ascension item in list)
		{
			num2 = item.SimPlayerWeight - mySkills.GetAscensionRank(item.Id);
			if (num2 > num)
			{
				ascension = item;
				num = num2;
			}
		}
		if (ascension != null)
		{
			if (!mySkills.HasAscension(ascension.Id))
			{
				mySkills.AddAscension(ascension.Id);
			}
			else
			{
				mySkills.LevelUpAscension(ascension.Id);
			}
			UpdateSocialLog.LogAdd(base.transform.name + " has added a point to Ascension: " + ascension.SkillName, "green");
			Myself.MySkills.AscensionPoints--;
		}
	}

	private List<Ascension> FindNextAscensions()
	{
		List<Ascension> list = new List<Ascension>();
		bool flag = Myself.MySkills.GetPointsSpent() >= 8;
		Ascension[] ascensionDatabase = GameData.SkillDatabase.AscensionDatabase;
		foreach (Ascension ascension in ascensionDatabase)
		{
			if (!flag)
			{
				continue;
			}
			switch (Myself.MyStats.CharacterClass.ClassName)
			{
			case "Duelist":
				if (ascension.UsedBy == Ascension.Class.Duelist && Myself.MySkills.GetAscensionRank(ascension.Id) < ascension.MaxRank)
				{
					list.Add(ascension);
				}
				break;
			case "Paladin":
				if (ascension.UsedBy == Ascension.Class.Paladin && Myself.MySkills.GetAscensionRank(ascension.Id) < ascension.MaxRank)
				{
					list.Add(ascension);
				}
				break;
			case "Arcanist":
				if (ascension.UsedBy == Ascension.Class.Arcanist && Myself.MySkills.GetAscensionRank(ascension.Id) < ascension.MaxRank)
				{
					list.Add(ascension);
				}
				break;
			case "Druid":
				if (ascension.UsedBy == Ascension.Class.Druid && Myself.MySkills.GetAscensionRank(ascension.Id) < ascension.MaxRank)
				{
					list.Add(ascension);
				}
				break;
			case "Stormcaller":
				if (ascension.UsedBy == Ascension.Class.Stormcaller && Myself.MySkills.GetAscensionRank(ascension.Id) < ascension.MaxRank)
				{
					list.Add(ascension);
				}
				break;
			case "Reaver":
				if (ascension.UsedBy == Ascension.Class.Reaver && Myself.MySkills.GetAscensionRank(ascension.Id) < ascension.MaxRank)
				{
					list.Add(ascension);
				}
				break;
			}
		}
		if (list.Count == 0)
		{
			ascensionDatabase = GameData.SkillDatabase.AscensionDatabase;
			foreach (Ascension ascension2 in ascensionDatabase)
			{
				if (ascension2.UsedBy == Ascension.Class.General && Myself.MySkills.GetAscensionRank(ascension2.Id) < ascension2.MaxRank)
				{
					list.Add(ascension2);
				}
			}
		}
		return list;
	}

	public void DoLevelUp()
	{
		if (Level < 35)
		{
			if (LvlUp != null)
			{
				LvlUp.Play();
			}
			Level++;
			if (!Myself.isNPC)
			{
				UpdateSocialLog.LogAdd("You've gained a level! You are now level " + Level, "yellow");
				GameData.PlayerAud.PlayOneShot(GameData.Misc.LvlUpSFX, GameData.PlayerAud.volume * GameData.SFXVol * GameData.MasterVol);
				GameData.Misc.Fanfaire.CallFanfaire(Level);
			}
			else
			{
				SimPlayer component = GetComponent<SimPlayer>();
				UpdateSocialLog.LogAdd(MyName + " has gained a level!", "yellow");
				if (component != null)
				{
					component.LoadSimSpells();
					component.LoadSimSkills();
					Myself?.MyNPC?.UpdateMemmedHeals();
					GameData.SimMngr.activeSimCongrats = MyName;
					UpdateSocialLog.LogAdd(MyName + " shouts: " + GameData.SimMngr.PersonalizeString(GameData.SimMngr.LevelUpCelebrations[UnityEngine.Random.Range(0, GameData.SimMngr.LevelUpCelebrations.Count)], component), "#FF9000");
					GameData.SimMngr.Sims[component.myIndex].Level = Level;
					if (GameData.SimMngr.Sims[component.myIndex].MyCurrentMemory != null)
					{
						GameData.SimMngr.Sims[component.myIndex].MyCurrentMemory.LevelGain++;
					}
					component.SaveSim();
				}
			}
			CurrentExperience = 0;
			ExperienceToLevelUp = Level * 10 * (Level + Level);
			if (Level > 9 && Level < 20)
			{
				ExperienceToLevelUp = Mathf.RoundToInt((float)ExperienceToLevelUp * 1.3f);
			}
			if (Level >= 20 && Level < 30)
			{
				ExperienceToLevelUp = Mathf.RoundToInt((float)ExperienceToLevelUp * 1.6f);
			}
			if (Level >= 30)
			{
				ExperienceToLevelUp *= 2;
			}
			CalcStats();
			if (base.transform.name == "Player")
			{
				if (Level >= 2)
				{
					SetAchievement.Unlock("LEVEL2");
				}
				if (Level >= 5)
				{
					SetAchievement.Unlock("LEVEL5");
				}
				if (Level >= 10)
				{
					SetAchievement.Unlock("LEVEL10");
				}
				if (Level >= 15)
				{
					SetAchievement.Unlock("LEVEL15");
				}
				if (Level >= 20)
				{
					SetAchievement.Unlock("LEVEL20");
				}
				if (Level >= 25)
				{
					SetAchievement.Unlock("LEVEL25");
				}
				if (Level >= 30)
				{
					SetAchievement.Unlock("LEVEL30");
				}
				if (Level >= 35)
				{
					SetAchievement.Unlock("LEVEL35");
				}
			}
			AwardPoints();
		}
		else
		{
			CurrentExperience = ExperienceToLevelUp;
		}
	}

	private void AwardPoints()
	{
		if (Level >= 2 && (Level - 2) % 2 == 0)
		{
			TotalAvailableProficiencies++;
			if (!Myself.isNPC)
			{
				UpdateSocialLog.LogAdd("You have a class proficiency point to spend! Open your inventory to assign it.", "yellow");
			}
			if ((Myself?.MyNPC?.SimPlayer).GetValueOrDefault())
			{
				AssignPoint();
			}
		}
	}

	public void SetXpForLevelUp()
	{
		ExperienceToLevelUp = Level * 10 * (Level + Level);
		if (Level > 9 && Level < 20)
		{
			ExperienceToLevelUp = Mathf.RoundToInt((float)ExperienceToLevelUp * 1.3f);
		}
		if (Level >= 20 && Level < 30)
		{
			ExperienceToLevelUp = Mathf.RoundToInt((float)ExperienceToLevelUp * 1.6f);
		}
		if (Level >= 30)
		{
			ExperienceToLevelUp *= 2;
		}
	}

	public int AddStatusEffect(Spell spell, bool _fromPlayer, int _dmgBonus)
	{
		if (!Myself.Alive)
		{
			return -1;
		}
		if (CheckForHigherLevelSE(spell) && spell.Line != Spell.SpellLine.Generic)
		{
			if (!Myself.isNPC)
			{
				UpdateSocialLog.LogAdd("The " + spell.SpellName + " spell did not take hold on you!", "lightblue");
			}
			else if (_fromPlayer)
			{
				UpdateSocialLog.LogAdd("Your " + spell.SpellName + " spell did not take hold!", "lightblue");
			}
			return -1;
		}
		bool flag = false;
		for (int i = 0; i < 30; i++)
		{
			if (StatusEffects[i] == null || !(spell != null) || (!(StatusEffects[i].Effect == spell) && (!(StatusEffects[i].Effect != null) || spell.Line == Spell.SpellLine.Generic || StatusEffects[i].Effect.Line != spell.Line || StatusEffects[i].Effect.RequiredLevel >= spell.RequiredLevel)))
			{
				continue;
			}
			CheckResist(spell.MyDamageType, 0f, Myself);
			StatusEffects[i].Effect = spell;
			StatusEffects[i].bonusDmg = _dmgBonus;
			StatusEffects[i].Duration = spell.SpellDurationInTicks;
			if (spell.RootTarget)
			{
				Rooted = true;
				if (!Myself.isNPC)
				{
					UpdateSocialLog.LogAdd("You have been ROOTED!", "lightblue");
				}
			}
			if (spell.StunTarget)
			{
				if (!Unstunnable)
				{
					if (spell.BreakOnDamage || StunCooldown <= 0f || Feared)
					{
						Stunned = true;
						if (!spell.BreakOnDamage)
						{
							StunCooldown = spell.SpellDurationInTicks * 6;
						}
						if (!Myself.isNPC)
						{
							UpdateSocialLog.LogAdd("You have been STUNNED!", "lightblue");
						}
					}
				}
				else if (!Myself.isNPC)
				{
					UpdateSocialLog.LogAdd("You shake off the stun!", "lightblue");
				}
				else if (Myself.isNPC && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) > 15f)
				{
					UpdateSocialLog.LogAdd(base.transform.name + " shakes off the stun!", "lightblue");
				}
			}
			if (spell.FearTarget)
			{
				if (Myself.BossXp < 1f)
				{
					Feared = true;
					if (!Myself.isNPC)
					{
						UpdateSocialLog.LogAdd("You have been FEARED!", "lightblue");
					}
				}
				else if (!Myself.isNPC)
				{
					UpdateSocialLog.LogAdd("You shake off the fear!", "lightblue");
				}
				else if (Myself.isNPC && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) > 15f)
				{
					UpdateSocialLog.LogAdd(base.transform.name + " shakes off the fear!", "lightblue");
				}
			}
			if (spell.GrantInvisibility)
			{
				Invisible = true;
			}
			if (StatusEffects[i].Effect.ShieldingAmt > 0 && SpellShield < StatusEffects[i].Effect.ShieldingAmt)
			{
				SpellShield += StatusEffects[i].Effect.ShieldingAmt;
				currentShieldSpell = i;
			}
			CalcStats();
			if (StatusIcons.Count > 0)
			{
				UpdateIcons();
			}
			if (Myself == GameData.PlayerControl.CurrentTarget)
			{
				GameData.NPCEffects.UpdateTargetEffects(StatusEffects);
			}
			if (!Myself.isNPC)
			{
				GameData.PlayerStatDisp.UpdateDisplayStats();
			}
			else if (_fromPlayer && !Charmed && spell.Type != Spell.SpellType.Beneficial)
			{
				GetComponent<NPC>().ManageAggro(spell.Aggro, GameData.PlayerControl.Myself);
			}
			flag = true;
			return i;
		}
		if (!flag)
		{
			for (int j = 0; j < 30; j++)
			{
				if (StatusEffects[j] == null || !(spell != null) || !(StatusEffects[j].Effect == null))
				{
					continue;
				}
				if (spell.RootTarget)
				{
					Rooted = true;
					if (!Myself.isNPC)
					{
						UpdateSocialLog.LogAdd("You have been ROOTED!", "lightblue");
					}
				}
				if (spell.StunTarget)
				{
					if (!Unstunnable)
					{
						if (spell.BreakOnDamage || StunCooldown <= 0f || Feared)
						{
							Stunned = true;
							if (!spell.BreakOnDamage)
							{
								StunCooldown = spell.SpellDurationInTicks * 6;
							}
							if (!Myself.isNPC)
							{
								UpdateSocialLog.LogAdd("You have been STUNNED!", "lightblue");
							}
						}
					}
					else if (!Myself.isNPC)
					{
						UpdateSocialLog.LogAdd("You shake off the stun!", "lightblue");
					}
					else if (Myself.isNPC && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) > 15f)
					{
						UpdateSocialLog.LogAdd(base.transform.name + " shakes off the stun!", "lightblue");
					}
				}
				if (spell.FearTarget)
				{
					if (Myself.BossXp < 1f)
					{
						Feared = true;
						if (!Myself.isNPC)
						{
							UpdateSocialLog.LogAdd("You have been FEARED!", "lightblue");
						}
					}
					else if (!Myself.isNPC)
					{
						UpdateSocialLog.LogAdd("You shake off the fear!", "lightblue");
					}
					else if (Myself.isNPC && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) > 15f)
					{
						UpdateSocialLog.LogAdd(base.transform.name + " shakes off the fear!", "lightblue");
					}
				}
				if (spell.GrantInvisibility)
				{
					Invisible = true;
				}
				CheckResist(spell.MyDamageType, 0f, Myself);
				StatusEffects[j].bonusDmg = _dmgBonus;
				StatusEffects[j].fromPlayer = _fromPlayer;
				StatusEffects[j].Effect = spell;
				if (!spell.UnstableDuration)
				{
					StatusEffects[j].Duration = spell.SpellDurationInTicks;
				}
				else
				{
					StatusEffects[j].Duration = UnityEngine.Random.Range(spell.SpellDurationInTicks / 3, spell.SpellDurationInTicks + 1);
				}
				if (StatusEffects[j].Effect.ShieldingAmt > 0 && SpellShield < StatusEffects[j].Effect.ShieldingAmt)
				{
					SpellShield = StatusEffects[j].Effect.ShieldingAmt;
					currentShieldSpell = j;
				}
				_ = StatusEffects[j].Effect.CharmTarget;
				CalcStats();
				if (StatusIcons.Count > 0)
				{
					UpdateIcons();
				}
				if (Myself == GameData.PlayerControl.CurrentTarget)
				{
					GameData.NPCEffects.UpdateTargetEffects(StatusEffects);
				}
				if (!Myself.isNPC)
				{
					GameData.PlayerStatDisp.UpdateDisplayStats();
				}
				else if (_fromPlayer && !Charmed && spell.Type != Spell.SpellType.Beneficial)
				{
					GetComponent<NPC>().ManageAggro(spell.Aggro, GameData.PlayerControl.Myself);
				}
				return j;
			}
		}
		if (!flag && spell != null && spell.Type == Spell.SpellType.StatusEffect)
		{
			int num = UnityEngine.Random.Range(0, 30);
			if (spell.RootTarget)
			{
				Rooted = true;
			}
			CheckResist(spell.MyDamageType, 0f, Myself);
			StatusEffects[num].bonusDmg = _dmgBonus;
			StatusEffects[num].fromPlayer = _fromPlayer;
			StatusEffects[num].Effect = spell;
			if (!spell.UnstableDuration)
			{
				StatusEffects[num].Duration = spell.SpellDurationInTicks;
			}
			else
			{
				StatusEffects[num].Duration = UnityEngine.Random.Range(spell.SpellDurationInTicks / 3, spell.SpellDurationInTicks + 1);
			}
			if (spell.RootTarget)
			{
				Rooted = true;
				if (!Myself.isNPC)
				{
					UpdateSocialLog.LogAdd("You have been ROOTED!", "lightblue");
				}
			}
			if (spell.StunTarget)
			{
				if (!Unstunnable)
				{
					if (spell.BreakOnDamage || StunCooldown <= 0f || Feared)
					{
						Stunned = true;
						if (!spell.BreakOnDamage)
						{
							StunCooldown = spell.SpellDurationInTicks * 6;
						}
						if (!Myself.isNPC)
						{
							UpdateSocialLog.LogAdd("You have been STUNNED!", "lightblue");
						}
					}
				}
				else if (!Myself.isNPC)
				{
					UpdateSocialLog.LogAdd("You shake off the stun!", "lightblue");
				}
				else if (Myself.isNPC && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) > 15f)
				{
					UpdateSocialLog.LogAdd(base.transform.name + " shakes off the stun!", "lightblue");
				}
			}
			if (spell.FearTarget)
			{
				if (Myself.BossXp < 1f)
				{
					Feared = true;
					if (!Myself.isNPC)
					{
						UpdateSocialLog.LogAdd("You have been FEARED!", "lightblue");
					}
				}
				else if (!Myself.isNPC)
				{
					UpdateSocialLog.LogAdd("You shake off the fear!", "lightblue");
				}
				else if (Myself.isNPC && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) > 15f)
				{
					UpdateSocialLog.LogAdd(base.transform.name + " shakes off the fear!", "lightblue");
				}
			}
			if (spell.GrantInvisibility)
			{
				Invisible = true;
			}
			if (StatusEffects[num].Effect.ShieldingAmt > 0 && SpellShield < StatusEffects[num].Effect.ShieldingAmt)
			{
				SpellShield = StatusEffects[num].Effect.ShieldingAmt;
				currentShieldSpell = num;
			}
			_ = StatusEffects[num].Effect.CharmTarget;
			CalcStats();
			if (StatusIcons.Count > 0)
			{
				UpdateIcons();
			}
			if (Myself == GameData.PlayerControl.CurrentTarget)
			{
				GameData.NPCEffects.UpdateTargetEffects(StatusEffects);
			}
			if (!Myself.isNPC)
			{
				GameData.PlayerStatDisp.UpdateDisplayStats();
			}
			else if (_fromPlayer && !Charmed && spell.Type != Spell.SpellType.Beneficial)
			{
				GetComponent<NPC>().ManageAggro(spell.Aggro, GameData.PlayerControl.Myself);
			}
			flag = true;
			return num;
		}
		if (!flag && spell != null)
		{
			if (!Myself.isNPC)
			{
				UpdateSocialLog.LogAdd("The " + spell.SpellName + " spell did not take hold on you!", "lightblue");
			}
			else if (_fromPlayer)
			{
				UpdateSocialLog.LogAdd("Your " + spell.SpellName + " spell did not take hold!", "lightblue");
			}
		}
		return -1;
	}

	public void AddEffectFromSave(Spell spell)
	{
		for (int i = 0; i <= 30; i++)
		{
			if (StatusEffects[i] != null && spell != null)
			{
				StatusEffects[i].Effect = spell;
				StatusEffects[i].bonusDmg = 0;
				StatusEffects[i].Duration = spell.SpellDurationInTicks;
				break;
			}
		}
		if (StatusIcons.Count > 0)
		{
			UpdateIcons();
		}
	}

	public void RemoveStatusEffect(int index)
	{
		if (StatusEffects[index].Effect != null)
		{
			if (StatusEffects[index].CastedByPC && Myself.isNPC)
			{
				UpdateSocialLog.LogAdd("Your " + StatusEffects[index].Effect.SpellName + " spell has worn off of " + base.transform.name, "yellow");
			}
			else if (!Myself.isNPC)
			{
				UpdateSocialLog.LogAdd("The " + StatusEffects[index].Effect.SpellName + " spell has worn off of you.", "yellow");
			}
			if (StatusEffects[index].Effect.ShieldingAmt > 0)
			{
				SpellShield -= StatusEffects[index].Effect.ShieldingAmt;
				if (SpellShield < 0)
				{
					SpellShield = 0;
				}
				currentShieldSpell = -1;
			}
			if (StatusEffects[index].Effect.RootTarget)
			{
				Rooted = false;
				if (!Myself.isNPC)
				{
					UpdateSocialLog.LogAdd("You have escaped from your bonds!", "lightblue");
				}
			}
			if (StatusEffects[index].Effect.CharmTarget)
			{
				Charmed = false;
				Myself.BreakCharm();
			}
			if (StatusEffects[index].Effect.GrantInvisibility)
			{
				Invisible = false;
			}
			StatusEffects[index].Effect = null;
			StatusEffects[index].fromPlayer = false;
			StatusEffects[index].Owner = null;
		}
		CalcStats();
		if (StatusIcons.Count > 0)
		{
			UpdateIcons();
		}
		if (Myself == GameData.PlayerControl.CurrentTarget)
		{
			GameData.NPCEffects.UpdateTargetEffects(StatusEffects);
		}
		if (Myself == GameData.PlayerControl.CurrentTarget)
		{
			GameData.NPCEffects.UpdateTargetEffects(StatusEffects);
		}
		if (!Myself.isNPC)
		{
			GameData.PlayerStatDisp.UpdateDisplayStats();
		}
		if (CurrentHP > CurrentMaxHP)
		{
			CurrentHP = CurrentMaxHP;
		}
	}

	private void TickEffects()
	{
		bool flag = isRetreating;
		isRetreating = false;
		for (int i = 0; i < 30; i++)
		{
			if (StatusEffects[i].Effect != null)
			{
				Spell effect = StatusEffects[i].Effect;
				if (effect == GameData.Misc.Retreat)
				{
					isRetreating = true;
				}
				if (effect.TargetDamage > 0 && StatusEffects[i].Duration > 0f)
				{
					string text = "";
					float num = CheckResist(effect.MyDamageType, 0f, Myself);
					float num2 = 1f - num;
					num2 = UnityEngine.Random.Range(num2 * 0.5f, num2 * 1.25f);
					if (num2 > 1f)
					{
						num2 = 1f;
					}
					if (num2 < 0f)
					{
						num2 = 0f;
					}
					if (UnityEngine.Random.Range(0f, 10f) > 6.5f && Myself.isNPC && Myself.MyNPC != null && !Myself.MyNPC.SimPlayer)
					{
						num2 = 1f;
					}
					int num3 = Mathf.RoundToInt(((float)effect.TargetDamage + (float)StatusEffects[i].bonusDmg) * num2);
					if (StatusEffects[i].Owner != null && StatusEffects[i].Owner != Myself && StatusEffects[i].Owner.MySkills != null)
					{
						float num4 = StatusEffects[i].Owner.MySkills.GetAscensionRank("19108265") * 33;
						if ((float)UnityEngine.Random.Range(0, 100) < num4)
						{
							num3 = Mathf.RoundToInt((float)num3 * 1.5f);
							text = " CRITICAL";
						}
					}
					if (num3 <= 0 && num3 <= 0 && StatusEffects[i].CastedByPC)
					{
						if (Myself.isNPC)
						{
							if (StatusEffects[i].fromPlayer || Vector3.Distance(GameData.PlayerControl.transform.position, base.transform.position) < 8f)
							{
								UpdateSocialLog.CombatLogAdd(base.transform.name + " resisted the damage from " + effect.SpellName, "lightblue");
							}
						}
						else
						{
							UpdateSocialLog.CombatLogAdd("You resisted the damage from " + effect.SpellName, "red");
						}
					}
					if (num3 > 0)
					{
						Myself.DamageMe(num3, StatusEffects[i].fromPlayer, effect.MyDamageType, StatusEffects[i].CreditDPS, _animEffect: false, _criticalHit: false);
					}
					if (Myself.isNPC && num3 > 0)
					{
						if (StatusEffects[i].fromPlayer || Vector3.Distance(GameData.PlayerControl.transform.position, base.transform.position) < 8f)
						{
							UpdateSocialLog.CombatLogAdd(base.transform.name + " took " + num3 + text + " damage from " + effect.SpellName, "lightblue");
						}
					}
					else if (num3 > 0)
					{
						UpdateSocialLog.CombatLogAdd("You took " + num3 + text + " damage from " + effect.SpellName, "red");
					}
					if (StatusEffects[i].Effect.ReapAndRenew && Myself.isNPC && Myself.MyNPC.CurrentAggroTarget != null)
					{
						Myself.MyNPC.CurrentAggroTarget.MyStats.HealMe(Mathf.RoundToInt((float)num3 * 0.5f));
						if (StatusEffects[i].Owner != null && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 15f)
						{
							UpdateSocialLog.LogAdd(StatusEffects[i].Owner.transform.name + "'s affliction renews " + Myself.MyNPC.CurrentAggroTarget.transform.name + " health for " + Mathf.RoundToInt((float)num3 * 0.5f) + "!", "green");
						}
					}
				}
				if (effect.BleedDamagePercent > 0)
				{
					int num5 = Mathf.RoundToInt((float)CurrentHP * 0.02f);
					if (num5 > 10000)
					{
						num5 = 10000;
					}
					Myself.BleedDamageMe(num5, StatusEffects[i].fromPlayer, null);
					if (Myself.isNPC)
					{
						if (StatusEffects[i].fromPlayer || Vector3.Distance(GameData.PlayerControl.transform.position, base.transform.position) < 8f)
						{
							UpdateSocialLog.CombatLogAdd(base.transform.name + " took " + num5 + " BLEED damage from " + effect.SpellName, "lightblue");
						}
					}
					else
					{
						UpdateSocialLog.CombatLogAdd("You took " + num5 + " BLEED damage from " + effect.SpellName, "red");
					}
				}
				if (effect.TargetHealing > 0 && StatusEffects[i].Duration > 0f && effect.MyDamageType == GameData.DamageType.Physical && (CombatStance == null || !CombatStance.StopRegen))
				{
					int num6 = effect.TargetHealing;
					if (StatusEffects[i].Owner != null && StatusEffects[i].Owner.MyStats != null && !effect.WornEffect)
					{
						num6 += Mathf.RoundToInt((float)StatusEffects[i].Owner.MyStats.WisScaleMod / 100f * (float)StatusEffects[i].Owner.MyStats.GetCurrentWis() * 10f);
						if (StatusEffects[i].Owner.MyStats.CharacterClass == GameData.ClassDB.Druid)
						{
							num6 += StatusEffects[i].Owner.MyStats.GetCurrentWis();
						}
					}
					CurrentHP += num6;
					if (CurrentHP > CurrentMaxHP)
					{
						CurrentHP = CurrentMaxHP;
					}
					if (StatusEffects[i].fromPlayer && !effect.WornEffect)
					{
						UpdateSocialLog.CombatLogAdd("Your " + effect.SpellName + " heals " + base.transform.name + " for " + num6 + " points of damage!", "green");
					}
				}
				if (effect.Mana > 0 && StatusEffects[i].Duration > 0f)
				{
					if (effect.Type == Spell.SpellType.Beneficial)
					{
						CurrentMana += effect.Mana;
					}
					if (CurrentMana > CurrentMaxMana)
					{
						CurrentMana = CurrentMaxMana;
					}
					if (effect.Type != Spell.SpellType.Beneficial)
					{
						CurrentMana -= effect.Mana;
					}
					if (CurrentMana < 0)
					{
						CurrentMana = 0;
					}
				}
				if (effect.PercentManaRestoration > 0)
				{
					if (effect.Type == Spell.SpellType.Beneficial)
					{
						CurrentMana += Mathf.RoundToInt((float)CurrentMaxMana * (float)effect.PercentManaRestoration / 100f);
					}
					if (CurrentMana > CurrentMaxMana)
					{
						CurrentMana = CurrentMaxMana;
					}
				}
				if (effect.UnstableDuration && UnityEngine.Random.Range(0, effect.RequiredLevel * 100) < CheckResistSimple(effect.MyDamageType))
				{
					StatusEffects[i].Duration = 0f;
				}
				StatusEffects[i].Duration -= 1f;
				if (StatusEffects[i].Duration <= 0f && StatusEffects[i].Effect != null)
				{
					RemoveStatusEffect(i);
				}
			}
			if (Myself == GameData.PlayerControl.CurrentTarget)
			{
				GameData.NPCEffects.UpdateTargetEffects(StatusEffects);
			}
		}
		if (StatusIcons.Count > 0)
		{
			UpdateIcons();
		}
		if (Myself == GameData.PlayerControl.CurrentTarget)
		{
			GameData.NPCEffects.UpdateTargetEffects(StatusEffects);
		}
		if (flag && !isRetreating)
		{
			CalcStats();
		}
	}

	public void HealMe(int _amt)
	{
		CurrentHP += _amt;
		if (CurrentHP > CurrentMaxHP)
		{
			CurrentHP = CurrentMaxHP;
		}
	}

	private void RegenEffects(float _mod)
	{
		if (CurrentHP < CurrentMaxHP && !CombatStance.StopRegen)
		{
			CurrentHP += Mathf.RoundToInt(_mod * (float)(Level + Mathf.RoundToInt((float)(2 * EndScaleMod) / 100f * (float)CurrentEnd)));
			if (CurrentHP > CurrentMaxHP)
			{
				CurrentHP = CurrentMaxHP;
			}
		}
		if (CurrentMana < CurrentMaxMana)
		{
			CurrentMana += Mathf.RoundToInt(Mathf.RoundToInt(_mod * ((float)WisScaleMod / 140f * (float)CurrentWis)));
			if (CurrentMana > CurrentMaxMana)
			{
				CurrentMana = CurrentMaxMana;
			}
		}
	}

	public void BreakEffectsOnAction()
	{
		for (int i = 0; i < StatusEffects.Length; i++)
		{
			StatusEffect obj = StatusEffects[i];
			if (obj != null && obj.Effect?.BreakOnAnyAction == true)
			{
				RemoveStatusEffect(i);
			}
		}
	}

	private void UpdateIcons()
	{
		if (StatusIcons.Count > 0)
		{
			for (int i = 0; i < 30; i++)
			{
				StatusIcons[i].SetSlot();
			}
		}
	}

	public void ReduceMana(int _amt)
	{
		CurrentMana -= _amt;
		if (CurrentMana < 0)
		{
			CurrentMana = 0;
		}
	}

	public bool ReduceHP(int _dmg, GameData.DamageType _dmgType, bool _involvesPlayer, bool _isCritical)
	{
		CurrentHP -= _dmg;
		bool crit = _isCritical;
		if ((_involvesPlayer && GameData.MyPopups) || (!_involvesPlayer && GameData.OthersPopups))
		{
			GameData.Misc.GenPopup(_dmg, crit, _dmgType, base.transform);
		}
		if (CurrentHP <= 0)
		{
			return true;
		}
		return false;
	}

	public int CalcMeleeDamageForNPC(Vector2 range, Stats _target)
	{
		return Mathf.RoundToInt(UnityEngine.Random.Range(range.x, range.y));
	}

	public int CalcMeleeDamage(int weaponDmg, int tarLvl, Stats _target, int _atkRollBonus)
	{
		int num = ((CharacterClass.ClassName == "Duelist") ? 1 : 0);
		int num2 = UnityEngine.Random.Range(0, 20);
		int num3 = Mathf.Clamp(Mathf.FloorToInt((float)Myself.MyStats.GetCurrentDex() * ((float)Myself.MyStats.DexScaleMod / 100f) / 25f), 0, 5);
		for (int i = 0; i < num3; i++)
		{
			int num4 = UnityEngine.Random.Range(0, 20);
			if (num4 > num2)
			{
				num2 = num4;
			}
		}
		float num5 = Myself.MyStats.Level - tarLvl;
		float num6 = (float)num2 + num5 + (float)_atkRollBonus;
		float num7 = Mathf.Abs(num5 * 0.5f);
		if (num6 <= 5f - (float)num - num7)
		{
			return 0;
		}
		float num8 = Mathf.Clamp(num6 / 20f, 0.4f, 1f);
		float num9 = (float)StrScaleMod / 100f;
		float num10 = (float)DexScaleMod / 100f;
		float num11 = 1f;
		Inventory myInv = MyInv;
		if ((object)myInv != null && myInv.TwoHandPrimary)
		{
			num11 = 1.1f;
		}
		float num12 = (float)CurrentStr * num9 * num11 + (float)CurrentDex * num10 / 2f;
		float num13 = 1f + (float)(weaponDmg - 1) * 0.4f;
		float num14 = 0.25f * (float)Myself.MyStats.Level;
		float num15 = CombatStance?.DamageMod ?? 1f;
		return Mathf.RoundToInt((num12 + num14) * num13 * num8 * num15);
	}

	public int CalcBowDamage(int weaponDmg, int tarLvl, Stats _target, int _atkRollBonus)
	{
		int num = 1;
		int num2 = UnityEngine.Random.Range(0, 20);
		int num3 = Mathf.Clamp(Mathf.FloorToInt((float)Myself.MyStats.GetCurrentDex() * ((float)Myself.MyStats.DexScaleMod / 100f) / 50f), 0, 3);
		for (int i = 0; i < num3; i++)
		{
			int num4 = UnityEngine.Random.Range(0, 20);
			if (num4 > num2)
			{
				num2 = num4;
			}
		}
		float num5 = num2 + _atkRollBonus;
		if (num5 <= 5f - (float)num)
		{
			return 0;
		}
		float num6 = Mathf.Clamp(num5 / 20f, 0.4f, 0.9f);
		float num7 = (float)StrScaleMod / 100f;
		float num8 = (float)DexScaleMod / 100f;
		float num9 = (float)CurrentStr * num7 * 2f + (float)CurrentDex * num8 / 2f;
		float num10 = 1f + (float)(weaponDmg - 1) * 0.4f;
		float num11 = 0.25f * (float)Myself.MyStats.Level;
		float num12 = CombatStance?.DamageMod ?? 1f;
		return Mathf.RoundToInt((num9 + num11) * num10 * num6 * 1f * num12);
	}

	public void CheckProc(ItemIcon slot, Character _tar)
	{
		float num = CombatStance?.ProcRateMod ?? 1f;
		if (slot.MyItem != null && slot.MyItem.WeaponProcOnHit != null && (float)UnityEngine.Random.Range(0, 100) < slot.MyItem.WeaponProcChance * num)
		{
			MySpells.StartSpellFromProc(slot.MyItem.WeaponProcOnHit, _tar.MyStats, 0f);
		}
		if (Myself.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByID("38827190")) && UnityEngine.Random.Range(0, 100) > 80)
		{
			MySpells.StartSpellFromProc(GameData.SpellDatabase.GetSpellByID("27767124"), _tar.MyStats, 0f);
		}
		if (Myself.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByID("69633733")) && UnityEngine.Random.Range(0, 100) < 10)
		{
			MySpells.StartSpellFromProc(GameData.SpellDatabase.GetSpellByID("12886874"), _tar.MyStats, 0f);
			Transform transform = MyInv?.Modulars?.WeaponR;
			if (transform == null)
			{
				return;
			}
			Transform transform2 = null;
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (child.gameObject.activeInHierarchy && child != transform)
				{
					transform2 = child;
					break;
				}
			}
			if (transform2 == null)
			{
				return;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(GameData.Misc.VoidFXReaverProc, transform2);
			ParticleSystem componentInChildren = gameObject.GetComponentInChildren<ParticleSystem>();
			if (!componentInChildren)
			{
				return;
			}
			ParticleSystem.ShapeModule shape = componentInChildren.shape;
			MeshFilter component = transform2.GetComponent<MeshFilter>();
			if ((bool)component && (bool)component.sharedMesh)
			{
				shape.shapeType = ParticleSystemShapeType.Mesh;
				shape.mesh = component.sharedMesh;
			}
			else
			{
				SkinnedMeshRenderer component2 = transform2.GetComponent<SkinnedMeshRenderer>();
				if ((bool)component2 && (bool)component2.sharedMesh)
				{
					shape.shapeType = ParticleSystemShapeType.SkinnedMeshRenderer;
					shape.skinnedMeshRenderer = component2;
				}
			}
			UnityEngine.Object.Destroy(gameObject, 5f);
		}
		StatusEffect[] statusEffects = StatusEffects;
		foreach (StatusEffect statusEffect in statusEffects)
		{
			if (statusEffect != null && statusEffect.Effect != null && statusEffect.Effect.AddProc != null && (float)UnityEngine.Random.Range(0, 100) < (float)statusEffect.Effect.AddProcChance * num)
			{
				MySpells.StartSpellFromProc(statusEffect.Effect.AddProc, _tar.MyStats, 0f);
				break;
			}
		}
	}

	public void CheckProc(Item slot, Character _tar)
	{
		float num = CombatStance?.ProcRateMod ?? 1f;
		float num2 = (float)CurrentDex * ((float)DexScaleMod / 100f / 20f);
		if (slot != null && slot.WeaponProcOnHit != null && (float)UnityEngine.Random.Range(0, 100) < (slot.WeaponProcChance + num2) * num)
		{
			MySpells.StartSpellFromProc(slot.WeaponProcOnHit, _tar.MyStats, 1f);
		}
		if ((Myself.MySkills?.KnownSkills?.Contains(GameData.SkillDatabase.GetSkillByID("38827190"))).GetValueOrDefault() && UnityEngine.Random.Range(0, 100) > 80)
		{
			MySpells.StartSpellFromProc(GameData.SpellDatabase.GetSpellByID("27767124"), _tar.MyStats, 0f);
		}
		if (Myself.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByID("69633733")) && UnityEngine.Random.Range(0, 100) > 80)
		{
			MySpells.StartSpellFromProc(GameData.SpellDatabase.GetSpellByID("12886874"), _tar.MyStats, 0f);
			Transform transform = Myself?.MyNPC?.ThisSim?.Mods?.WeaponR;
			if (transform == null)
			{
				return;
			}
			Transform transform2 = null;
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (child.gameObject.activeInHierarchy && child != transform && child.transform.name != "DEFAULT")
				{
					transform2 = child;
					break;
				}
			}
			if (transform2 == null)
			{
				return;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(GameData.Misc.VoidFXReaverProc, transform2);
			ParticleSystem componentInChildren = gameObject.GetComponentInChildren<ParticleSystem>();
			if (!componentInChildren)
			{
				return;
			}
			ParticleSystem.ShapeModule shape = componentInChildren.shape;
			MeshFilter component = transform2.GetComponent<MeshFilter>();
			if ((bool)component && (bool)component.sharedMesh)
			{
				shape.shapeType = ParticleSystemShapeType.Mesh;
				shape.mesh = component.sharedMesh;
			}
			else
			{
				SkinnedMeshRenderer component2 = transform2.GetComponent<SkinnedMeshRenderer>();
				if ((bool)component2 && (bool)component2.sharedMesh)
				{
					shape.shapeType = ParticleSystemShapeType.SkinnedMeshRenderer;
					shape.skinnedMeshRenderer = component2;
				}
			}
			UnityEngine.Object.Destroy(gameObject, 5f);
		}
		StatusEffect[] statusEffects = StatusEffects;
		foreach (StatusEffect statusEffect in statusEffects)
		{
			if (statusEffect != null && statusEffect.Effect != null && statusEffect.Effect.AddProc != null && (float)UnityEngine.Random.Range(0, 100) < (float)statusEffect.Effect.AddProcChance * num)
			{
				MySpells.StartSpellFromProc(statusEffect.Effect.AddProc, _tar.MyStats, 1f);
				break;
			}
		}
	}

	public Spell CheckSEProcsOnly()
	{
		StatusEffect[] statusEffects = StatusEffects;
		foreach (StatusEffect statusEffect in statusEffects)
		{
			if (statusEffect != null && statusEffect.Effect != null && statusEffect.Effect.AddProc != null && UnityEngine.Random.Range(0, 100) < statusEffect.Effect.AddProcChance)
			{
				return statusEffect.Effect.AddProc;
			}
		}
		return null;
	}

	public int MitigatePhysical(int _incomingDmg, float _hitBonus, Stats _attacker)
	{
		float num = 1f;
		int num2 = 0;
		if (Myself.MyStats.CharacterClass == GameData.ClassDB.Paladin || Myself.MyStats.CharacterClass == GameData.ClassDB.Reaver)
		{
			num2 = 1;
			if (MyInv != null && MyInv.SecondaryShield)
			{
				num = 1.2f;
			}
		}
		float num3 = (float)_attacker.Level - (float)Level;
		float num4 = ((float)CurrentAC * (UnityEngine.Random.Range((13f + (float)num2 - (float)_attacker.AtkRollModifier) * CharacterClass.MitigationBonus, (20f + (float)num2 - (float)_attacker.AtkRollModifier) * CharacterClass.MitigationBonus) / 20f) * num + 1f) * Myself.MyStats.CharacterClass.MitigationBonus / (_hitBonus + 1f);
		num4 *= num;
		if (num3 > 0f)
		{
			num4 *= 0.98f;
		}
		else if (num3 < 0f)
		{
			num4 *= 1.02f;
		}
		float num5 = 0.7f;
		if (Myself.MyStats.CharacterClass == GameData.ClassDB.Paladin)
		{
			num5 = 0.88f;
		}
		if (num4 > num5)
		{
			num4 = num5;
		}
		if (num4 < 0f)
		{
			num4 = 0f;
		}
		_incomingDmg = Mathf.RoundToInt(_incomingDmg);
		int num6 = Mathf.RoundToInt((float)Mathf.RoundToInt((float)_incomingDmg - (float)_incomingDmg * num4) * CombatStance.DamageTakenMod);
		return num6 - Mathf.RoundToInt((float)num6 * ((float)Myself.MySkills.GetAscensionRank("14210880") * 0.05f));
	}

	public float GetMHAtkDelay()
	{
		return ActualMHAtkDelay;
	}

	public void ResetMHAtkDelay()
	{
		float num = 0f;
		ActualMHAtkDelay = CurrentMHAtkDelay;
		if (CheckForSEByName("Affinity for Suffering"))
		{
			num = 0.02f;
		}
		if (CheckForSEByName("Quest for Suffering"))
		{
			num = 0.04f;
		}
		if (num != 0f)
		{
			float value = Mathf.Floor((float)(CurrentMaxHP - CurrentHP) / (float)CurrentMaxHP / 0.1f) * num;
			value = Mathf.Clamp(value, 0f, 0.9f);
			ActualMHAtkDelay *= 1f - value;
		}
		if (ActualMHAtkDelay == 0f)
		{
			ActualMHAtkDelay = 120f;
		}
	}

	public void ZeroMHAtkDelay()
	{
		ActualMHAtkDelay = 5f;
	}

	public void ResetMHAtkDelay(float _force)
	{
		ActualMHAtkDelay = _force;
	}

	public float GetOHAtkDelay()
	{
		return ActualOHAtkDelay;
	}

	public void ResetOHAtkDelay()
	{
		float num = 0f;
		if (MyInv != null)
		{
			ActualOHAtkDelay = MyInv.OHDelay * 60f;
			if (CheckForSEByName("Affinity for Suffering"))
			{
				num = 0.02f;
			}
			if (CheckForSEByName("Quest for Suffering"))
			{
				num = 0.04f;
			}
			if (num != 0f)
			{
				float value = Mathf.Floor((float)(CurrentMaxHP - CurrentHP) / (float)CurrentMaxHP / 0.1f) * num;
				value = Mathf.Clamp(value, 0f, 0.9f);
				num = value;
				ActualOHAtkDelay *= 1f - value;
			}
		}
		else
		{
			ActualOHAtkDelay = 120f;
		}
		if (ActualOHAtkDelay == 0f)
		{
			ActualOHAtkDelay = 120f;
		}
	}

	public void ResetOHAtkDelay(float _force)
	{
		ActualOHAtkDelay = _force;
	}

	public int GetCurrentHP()
	{
		return CurrentHP;
	}

	public int GetCurrentAC()
	{
		return CurrentAC;
	}

	public int GetCurrentMana()
	{
		return CurrentMana;
	}

	public int GetCurrentStr()
	{
		return CurrentStr;
	}

	public int GetCurrentEnd()
	{
		return CurrentEnd;
	}

	public int GetCurrentDex()
	{
		return CurrentDex;
	}

	public int GetCurrentAgi()
	{
		return CurrentAgi;
	}

	public int GetCurrentInt()
	{
		return CurrentInt;
	}

	public int GetCurrentWis()
	{
		return CurrentWis;
	}

	public int GetCurrentCha()
	{
		return CurrentCha;
	}

	public int GetCurrentMR()
	{
		return CurrentMR;
	}

	public int GetCurrentER()
	{
		return CurrentER;
	}

	public int GetCurrentPR()
	{
		return CurrentPR;
	}

	public int GetCurrentVR()
	{
		return CurrentVR;
	}

	public int GetCurrentMaxMana()
	{
		return CurrentMaxMana;
	}

	public int GetCurrentRes()
	{
		return CurrentRes;
	}

	public int GetCurrentHaste()
	{
		return Mathf.RoundToInt(seWeapHaste);
	}

	public int GetCurrentDS()
	{
		return CurrentDS;
	}

	public int GetCurrentHPRegen()
	{
		return Mathf.RoundToInt(2 * (Level + Mathf.RoundToInt((float)(2 * EndScaleMod) / 100f * (float)CurrentEnd)));
	}

	public int GetCurrentMPRegen()
	{
		CalcStats();
		return seManaRegen + Mathf.RoundToInt(Mathf.RoundToInt(1f * ((float)WisScaleMod / 140f * (float)CurrentWis)));
	}

	private void CountStatusEffects()
	{
		StatusEffect statusEffect = null;
		seHP = 0;
		seAC = 0;
		seMana = 0;
		seStr = 0;
		seEnd = 0;
		seDex = 0;
		seAgi = 0;
		seInt = 0;
		seWis = 0;
		seCha = 0;
		seRes = 0;
		seAtkMod = 0;
		seXPBonus = 0f;
		seDS = 0;
		seMR = 0;
		seER = 0;
		sePR = 0;
		seVR = 0;
		seManaRegen = 0;
		seLifesteal = 0f;
		seRunSpeed = 0f;
		seWeapHaste = 0f;
		Rooted = false;
		Stunned = false;
		Feared = false;
		Invisible = false;
		for (int i = 0; i < 30; i++)
		{
			if (!(StatusEffects[i].Effect != null))
			{
				continue;
			}
			statusEffect = StatusEffects[i];
			seHP += statusEffect.Effect.HP;
			seAC += statusEffect.Effect.AC;
			seMana += statusEffect.Effect.Mana;
			seStr += statusEffect.Effect.Str;
			seEnd += statusEffect.Effect.End;
			seDex += statusEffect.Effect.Dex;
			seAgi += statusEffect.Effect.Agi;
			seInt += statusEffect.Effect.Int;
			seWis += statusEffect.Effect.Wis;
			seCha += statusEffect.Effect.Cha;
			seRes += statusEffect.Effect.ResonateChance;
			seAtkMod += statusEffect.Effect.AtkRollModifier;
			seXPBonus += statusEffect.Effect.XPBonus;
			seManaRegen += statusEffect.Effect.Mana;
			seMR += statusEffect.Effect.MR;
			seER += statusEffect.Effect.ER;
			sePR += statusEffect.Effect.PR;
			seVR += statusEffect.Effect.VR;
			seDS += statusEffect.Effect.DamageShield;
			seRunSpeed += statusEffect.Effect.MovementSpeed;
			seWeapHaste += statusEffect.Effect.Haste;
			if (seWeapHaste > 60f)
			{
				seWeapHaste = 60f;
			}
			seLifesteal += statusEffect.Effect.percentLifesteal;
			if (statusEffect.Effect.RootTarget)
			{
				Rooted = true;
			}
			if (statusEffect.Effect.StunTarget && !Unstunnable && (statusEffect.Effect.BreakOnDamage || StunCooldown <= 0f || Feared))
			{
				Stunned = true;
				if (!Myself.isNPC)
				{
					UpdateSocialLog.LogAdd("You have been STUNNED!", "lightblue");
				}
			}
			if (statusEffect.Effect.FearTarget && Myself.BossXp < 1f)
			{
				Feared = true;
			}
			if (statusEffect.Effect.GrantInvisibility)
			{
				Invisible = true;
			}
		}
		if (seWeapHaste > 60f)
		{
			seWeapHaste = 60f;
		}
		if (seWeapHaste < -95f)
		{
			seWeapHaste = -95f;
		}
		if (seAtkMod < -5)
		{
			seAtkMod = -5;
		}
	}

	public int strBonus()
	{
		int num = 0;
		int num2 = 0;
		if (CharacterClass != null)
		{
			num2 = StrScaleMod;
		}
		for (int i = 0; i <= Myself.MyStats.GetCurrentStr(); i++)
		{
			if (UnityEngine.Random.Range(0, 100) < num2 || UnityEngine.Random.Range(0, 50) < Level)
			{
				num++;
			}
		}
		return num;
	}

	public int dexBonus()
	{
		int num = 0;
		int num2 = 0;
		if (CharacterClass != null)
		{
			num2 = DexScaleMod;
		}
		for (int i = 0; i <= Myself.MyStats.GetCurrentDex(); i++)
		{
			if (UnityEngine.Random.Range(0, 100) < num2 || UnityEngine.Random.Range(0, 50) < Level)
			{
				num++;
			}
		}
		return num;
	}

	public int HealMe(Spell _spell, int _amt, bool _isCrit, bool _isMana, Character _source)
	{
		float num = 100f;
		if (_source != null)
		{
			num = Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position);
		}
		int num2 = 0;
		string text = "";
		if (_isCrit)
		{
			text = "CRITICAL";
		}
		if (!_isMana)
		{
			int currentHP = CurrentHP;
			CurrentHP += _amt;
			if (CurrentHP > CurrentMaxHP)
			{
				CurrentHP = CurrentMaxHP;
			}
			num2 = CurrentHP - currentHP;
			if (_source != null && !_source.isNPC && num2 > 0)
			{
				UpdateSocialLog.CombatLogAdd("Your " + text + " healing spell restores " + num2 + " life!", "green");
			}
			else if (_source.isNPC && num2 > 0 && num < 15f)
			{
				UpdateSocialLog.CombatLogAdd(_source.transform.name + "'s " + text + " healing spell restores " + num2 + " life!", "green");
			}
		}
		else
		{
			CurrentMana += _amt;
			if (CurrentMana > CurrentMaxMana)
			{
				CurrentMana = CurrentMaxMana;
			}
		}
		if (Myself.isNPC && num < 10f && (num2 > 0 || _isMana))
		{
			UpdateSocialLog.LogAdd(Myself.transform.name + " " + _spell.StatusEffectMessageOnNPC, "lightblue");
		}
		else if (!Myself.isNPC && (num2 > 0 || _isMana))
		{
			UpdateSocialLog.LogAdd("You " + _spell.StatusEffectMessageOnPlayer, "lightblue");
		}
		return num2;
	}

	public bool isCriticalAttack()
	{
		bool result = false;
		int num = 0;
		int num2 = 0;
		if (CharacterClass != null)
		{
			num2 = DexScaleMod;
		}
		for (int i = 0; (float)i <= (float)Myself.MyStats.GetCurrentDex() * ((float)DexScaleMod / 100f); i++)
		{
			if (UnityEngine.Random.Range(0, 100 - num2) < Level)
			{
				num++;
			}
			if ((CharacterClass == GameData.ClassDB.Duelist || (CharacterClass == GameData.ClassDB.Stormcaller && UnityEngine.Random.Range(0, 10) > 5)) && UnityEngine.Random.Range(0, 100 - num2) < Level)
			{
				num++;
			}
		}
		if (UnityEngine.Random.Range(0, 100) < num)
		{
			result = true;
		}
		return result;
	}

	private void EndBonus()
	{
	}

	public float CheckResist(GameData.DamageType _dmgType, float resistMod, Character _attacker)
	{
		if (_attacker != null)
		{
			return Myself.CheckResistAmount(resistMod, _attacker.MyStats.Level, _dmgType);
		}
		return 0f;
	}

	public int CheckResistSimple(GameData.DamageType _dmgType)
	{
		return _dmgType switch
		{
			GameData.DamageType.Magic => GetCurrentMR(), 
			GameData.DamageType.Poison => GetCurrentPR(), 
			GameData.DamageType.Void => GetCurrentVR(), 
			GameData.DamageType.Elemental => GetCurrentER(), 
			_ => 999, 
		};
	}

	public void CalcSimStats()
	{
		if (Level > 35)
		{
			Level = 35;
		}
		if (MyInv.EquippedItems[0].WeaponDmg != 0)
		{
			GetComponent<NPC>().BaseAtkDmg = MyInv.EquippedItems[0].WeaponDmg;
			CurrentMHAtkDelay = MyInv.EquippedItems[0].WeaponDly * 60f;
		}
		else
		{
			CurrentOHAtkDelay = 150f;
		}
		if (MyInv.EquippedItems[1].WeaponDmg != 0)
		{
			GetComponent<NPC>().OHAtkDmg = MyInv.EquippedItems[1].WeaponDmg;
			CurrentOHAtkDelay = MyInv.OHDelay;
		}
		else
		{
			CurrentOHAtkDelay = 1f;
		}
		Skill[] skillDatabase = GameData.SkillDatabase.SkillDatabase;
		foreach (Skill skill in skillDatabase)
		{
			if (skill.SimPlayersAutolearn)
			{
				if (skill.PaladinRequiredLevel <= Level && skill.PaladinRequiredLevel > 0 && CharacterClass.ClassName == "Paladin" && !GetComponent<UseSkill>().KnownSkills.Contains(skill))
				{
					GetComponent<UseSkill>().KnownSkills.Add(skill);
				}
				if (skill.DuelistRequiredLevel <= Level && skill.DuelistRequiredLevel > 0 && CharacterClass.ClassName == "Duelist" && !GetComponent<UseSkill>().KnownSkills.Contains(skill))
				{
					GetComponent<UseSkill>().KnownSkills.Add(skill);
				}
				if (skill.DruidRequiredLevel <= Level && skill.DruidRequiredLevel > 0 && CharacterClass.ClassName == "Druid" && !GetComponent<UseSkill>().KnownSkills.Contains(skill))
				{
					GetComponent<UseSkill>().KnownSkills.Add(skill);
				}
				if (skill.ArcanistRequiredLevel <= Level && skill.ArcanistRequiredLevel > 0 && CharacterClass.ClassName == "Arcanist" && !GetComponent<UseSkill>().KnownSkills.Contains(skill))
				{
					GetComponent<UseSkill>().KnownSkills.Add(skill);
				}
				if (skill.StormcallerRequiredLevel <= Level && skill.StormcallerRequiredLevel > 0 && CharacterClass.ClassName == "Stormcaller" && !GetComponent<UseSkill>().KnownSkills.Contains(skill))
				{
					GetComponent<UseSkill>().KnownSkills.Add(skill);
				}
			}
		}
	}

	public int AddStatusEffect(Spell spell, bool _fromPlayer, int _dmgBonus, Character _specificCaster)
	{
		bool flag = false;
		if (!Myself.Alive)
		{
			return -1;
		}
		if (CheckForHigherLevelSE(spell))
		{
			if (!Myself.isNPC)
			{
				UpdateSocialLog.LogAdd("The " + spell.SpellName + " spell did not take hold on you!", "lightblue");
				return -1;
			}
			if (Myself.isNPC && Myself.GetComponent<NPC>().SimPlayer)
			{
				return -1;
			}
		}
		if (CheckForHigherLevelSEFromMe(spell, _specificCaster))
		{
			if (_fromPlayer)
			{
				UpdateSocialLog.LogAdd("Your " + spell.SpellName + " spell did not take hold!", "lightblue");
			}
			return -1;
		}
		if (Myself.isNPC && Myself.BossXp > 1f && spell.FearTarget)
		{
			UpdateSocialLog.LogAdd(base.transform.name + " is immune to this spell's effects!", "lightblue");
			return -1;
		}
		flag = _specificCaster.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByID("14198580"));
		bool flag2 = false;
		for (int i = 0; i < 30; i++)
		{
			if (StatusEffects[i] == null || !(spell != null) || !(StatusEffects[i].Owner == _specificCaster) || (!(StatusEffects[i].Effect == spell) && (!(StatusEffects[i].Effect != null) || StatusEffects[i].Effect.Line != spell.Line || StatusEffects[i].Effect.RequiredLevel > spell.RequiredLevel || spell.Line == Spell.SpellLine.Generic)))
			{
				continue;
			}
			CheckResist(spell.MyDamageType, 0f, _specificCaster);
			if (_specificCaster.transform.name == "Player")
			{
				StatusEffects[i].CastedByPC = true;
			}
			else
			{
				StatusEffects[i].CastedByPC = false;
			}
			StatusEffects[i].bonusDmg = _dmgBonus;
			StatusEffects[i].Duration = spell.SpellDurationInTicks;
			StatusEffects[i].Effect.ReapAndRenew = flag;
			StatusEffects[i].Effect = spell;
			StatusEffects[i].Owner = _specificCaster;
			StatusEffects[i].CreditDPS = _specificCaster;
			if (spell.RootTarget)
			{
				Rooted = true;
				if (!Myself.isNPC)
				{
					UpdateSocialLog.LogAdd("You have been ROOTED!", "lightblue");
				}
			}
			if (spell.StunTarget)
			{
				if (!Unstunnable)
				{
					if (spell.BreakOnDamage || StunCooldown <= 0f || Feared)
					{
						Stunned = true;
						if (!spell.BreakOnDamage)
						{
							StunCooldown = spell.SpellDurationInTicks * 8;
						}
						if (!Myself.isNPC)
						{
							UpdateSocialLog.LogAdd("You have been STUNNED!", "lightblue");
						}
					}
				}
				else if (!Myself.isNPC)
				{
					UpdateSocialLog.LogAdd("You shake off the stun!", "lightblue");
				}
				else if (Myself.isNPC && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 25f)
				{
					UpdateSocialLog.LogAdd(base.transform.name + " shakes off the stun!", "lightblue");
				}
			}
			if (StatusEffects[i].Effect.ShieldingAmt > 0 && SpellShield < StatusEffects[i].Effect.ShieldingAmt)
			{
				SpellShield += StatusEffects[i].Effect.ShieldingAmt;
				currentShieldSpell = i;
			}
			CalcStats();
			if (StatusIcons.Count > 0)
			{
				UpdateIcons();
			}
			if (Myself == GameData.PlayerControl.CurrentTarget)
			{
				GameData.NPCEffects.UpdateTargetEffects(StatusEffects);
			}
			if (!Myself.isNPC)
			{
				GameData.PlayerStatDisp.UpdateDisplayStats();
			}
			else if (_specificCaster != null && !Charmed && spell.Type != Spell.SpellType.Beneficial)
			{
				GetComponent<NPC>().ManageAggro(spell.Aggro, _specificCaster);
			}
			flag2 = true;
			return i;
		}
		if (!flag2)
		{
			for (int j = 0; j < 30; j++)
			{
				if (StatusEffects[j] == null || !(spell != null) || CheckForHigherLevelSEFromMe(spell, _specificCaster) || !(StatusEffects[j].Effect == null))
				{
					continue;
				}
				if (spell.RootTarget)
				{
					Rooted = true;
					if (!Myself.isNPC)
					{
						UpdateSocialLog.LogAdd("You have been ROOTED!", "lightblue");
					}
				}
				if (spell.StunTarget)
				{
					if (!Unstunnable)
					{
						if (spell.BreakOnDamage || StunCooldown <= 0f || Feared)
						{
							Stunned = true;
							if (!spell.BreakOnDamage)
							{
								StunCooldown = spell.SpellDurationInTicks * 8;
							}
							if (!Myself.isNPC)
							{
								UpdateSocialLog.LogAdd("You have been STUNNED!", "lightblue");
							}
						}
					}
					else if (!Myself.isNPC)
					{
						UpdateSocialLog.LogAdd("You shake off the stun!", "lightblue");
					}
					else if (Myself.isNPC && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 25f)
					{
						UpdateSocialLog.LogAdd(base.transform.name + " shakes off the stun!", "lightblue");
					}
				}
				CheckResist(spell.MyDamageType, 0f, Myself);
				if (_specificCaster.transform.name == "Player")
				{
					StatusEffects[j].CastedByPC = true;
				}
				else
				{
					StatusEffects[j].CastedByPC = false;
				}
				StatusEffects[j].bonusDmg = _dmgBonus;
				StatusEffects[j].fromPlayer = _fromPlayer;
				StatusEffects[j].Effect = spell;
				StatusEffects[j].Effect.ReapAndRenew = flag;
				StatusEffects[j].Owner = _specificCaster;
				StatusEffects[j].CreditDPS = _specificCaster;
				if (!spell.UnstableDuration)
				{
					StatusEffects[j].Duration = spell.SpellDurationInTicks;
				}
				else
				{
					StatusEffects[j].Duration = UnityEngine.Random.Range(spell.SpellDurationInTicks / 3, spell.SpellDurationInTicks + 1);
				}
				if (StatusEffects[j].Effect.ShieldingAmt > 0 && SpellShield < StatusEffects[j].Effect.ShieldingAmt)
				{
					SpellShield = StatusEffects[j].Effect.ShieldingAmt;
					currentShieldSpell = j;
				}
				if (StatusEffects[j].Effect.CharmTarget)
				{
					if (spell.MaxLevelTarget >= Level)
					{
						if (_specificCaster.MyCharmedNPC == null)
						{
							Charmed = true;
							Myself.CharmMe(_specificCaster);
						}
						else
						{
							UpdateSocialLog.LogAdd("You lack the power to charm two beings!", "yellow");
						}
					}
					else
					{
						UpdateSocialLog.LogAdd("This spell is not strong enough...", "yellow");
					}
				}
				CalcStats();
				if (StatusIcons.Count > 0)
				{
					UpdateIcons();
				}
				if (Myself == GameData.PlayerControl.CurrentTarget)
				{
					GameData.NPCEffects.UpdateTargetEffects(StatusEffects);
				}
				if (!Myself.isNPC)
				{
					GameData.PlayerStatDisp.UpdateDisplayStats();
				}
				else if (_specificCaster != null && !Charmed && spell.Type != Spell.SpellType.Beneficial)
				{
					GetComponent<NPC>().ManageAggro(spell.Aggro, _specificCaster);
				}
				flag2 = true;
				return j;
			}
		}
		if (!flag2 && spell != null && spell.Type == Spell.SpellType.StatusEffect && !CheckForHigherLevelSEFromMe(spell, _specificCaster))
		{
			int num = UnityEngine.Random.Range(0, 30);
			if (spell.RootTarget)
			{
				Rooted = true;
			}
			CheckResist(spell.MyDamageType, 0f, Myself);
			StatusEffects[num].bonusDmg = _dmgBonus;
			StatusEffects[num].fromPlayer = _fromPlayer;
			StatusEffects[num].Effect = spell;
			StatusEffects[num].Effect.ReapAndRenew = flag;
			StatusEffects[num].Owner = _specificCaster;
			StatusEffects[num].CreditDPS = _specificCaster;
			if (!spell.UnstableDuration)
			{
				StatusEffects[num].Duration = spell.SpellDurationInTicks;
			}
			else
			{
				StatusEffects[num].Duration = UnityEngine.Random.Range(spell.SpellDurationInTicks / 3, spell.SpellDurationInTicks + 1);
			}
			if (spell.RootTarget)
			{
				Rooted = true;
				if (!Myself.isNPC)
				{
					UpdateSocialLog.LogAdd("You have been ROOTED!", "lightblue");
				}
			}
			if (spell.StunTarget)
			{
				if (!Unstunnable)
				{
					if (spell.BreakOnDamage || StunCooldown <= 0f || Feared)
					{
						Stunned = true;
						if (!spell.BreakOnDamage)
						{
							StunCooldown = spell.SpellDurationInTicks * 8;
						}
						if (!Myself.isNPC)
						{
							UpdateSocialLog.LogAdd("You have been STUNNED!", "lightblue");
						}
					}
				}
				else if (!Myself.isNPC)
				{
					UpdateSocialLog.LogAdd("You shake off the stun!", "lightblue");
				}
				else if (Myself.isNPC && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 25f)
				{
					UpdateSocialLog.LogAdd(base.transform.name + " shakes off the stun!", "lightblue");
				}
			}
			if (StatusEffects[num].Effect.ShieldingAmt > 0 && SpellShield < StatusEffects[num].Effect.ShieldingAmt)
			{
				SpellShield = StatusEffects[num].Effect.ShieldingAmt;
				currentShieldSpell = num;
			}
			if (StatusEffects[num].Effect.CharmTarget)
			{
				if (spell.MaxLevelTarget >= Level)
				{
					if (_specificCaster.MyCharmedNPC == null)
					{
						Charmed = true;
						Myself.CharmMe(_specificCaster);
					}
					else
					{
						UpdateSocialLog.LogAdd("You lack the power to charm two beings!", "yellow");
					}
				}
				else
				{
					UpdateSocialLog.LogAdd("This spell is not strong enough...", "yellow");
				}
			}
			CalcStats();
			if (StatusIcons.Count > 0)
			{
				UpdateIcons();
			}
			if (Myself == GameData.PlayerControl.CurrentTarget)
			{
				GameData.NPCEffects.UpdateTargetEffects(StatusEffects);
			}
			if (!Myself.isNPC)
			{
				GameData.PlayerStatDisp.UpdateDisplayStats();
			}
			else if (_fromPlayer && !Charmed && spell.Type != Spell.SpellType.Beneficial)
			{
				GetComponent<NPC>().ManageAggro(spell.Aggro, _specificCaster);
			}
			flag2 = true;
			return num;
		}
		if (!flag2)
		{
			if (!Myself.isNPC)
			{
				UpdateSocialLog.LogAdd("The " + spell.SpellName + " spell did not take hold on you!", "lightblue");
			}
			else if (_fromPlayer)
			{
				UpdateSocialLog.LogAdd("Your " + spell.SpellName + " spell did not take hold!", "lightblue");
			}
		}
		return -1;
	}

	public void AddStatusEffectNoChecks(Spell spell, bool _fromPlayer, int _dmgBonus, Character _specificCaster)
	{
		bool reapAndRenew = false;
		for (int i = 0; i < 30; i++)
		{
			if (StatusEffects[i] == null || !(StatusEffects[i].Effect == null))
			{
				continue;
			}
			if (spell.RootTarget)
			{
				Rooted = true;
				if (!Myself.isNPC)
				{
					UpdateSocialLog.LogAdd("You have been ROOTED!", "lightblue");
				}
			}
			if (spell.StunTarget)
			{
				if (!Unstunnable)
				{
					if (spell.BreakOnDamage || StunCooldown <= 0f || Feared)
					{
						Stunned = true;
						if (!spell.BreakOnDamage)
						{
							StunCooldown = spell.SpellDurationInTicks * 8;
						}
						if (!Myself.isNPC)
						{
							UpdateSocialLog.LogAdd("You have been STUNNED!", "lightblue");
						}
					}
				}
				else if (!Myself.isNPC)
				{
					UpdateSocialLog.LogAdd("You shake off the stun!", "lightblue");
				}
				else if (Myself.isNPC && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 25f)
				{
					UpdateSocialLog.LogAdd(base.transform.name + " shakes off the stun!", "lightblue");
				}
			}
			CheckResist(spell.MyDamageType, 0f, Myself);
			if (_specificCaster != null && _specificCaster.transform.name == "Player")
			{
				StatusEffects[i].CastedByPC = true;
			}
			else
			{
				StatusEffects[i].CastedByPC = false;
			}
			StatusEffects[i].bonusDmg = _dmgBonus;
			StatusEffects[i].fromPlayer = _fromPlayer;
			StatusEffects[i].Effect = spell;
			StatusEffects[i].Effect.ReapAndRenew = reapAndRenew;
			StatusEffects[i].Owner = null;
			StatusEffects[i].CreditDPS = _specificCaster;
			if (!spell.UnstableDuration)
			{
				StatusEffects[i].Duration = spell.SpellDurationInTicks;
			}
			else
			{
				StatusEffects[i].Duration = UnityEngine.Random.Range(spell.SpellDurationInTicks / 3, spell.SpellDurationInTicks + 1);
			}
			if (StatusEffects[i].Effect.ShieldingAmt > 0 && SpellShield < StatusEffects[i].Effect.ShieldingAmt)
			{
				SpellShield = StatusEffects[i].Effect.ShieldingAmt;
				currentShieldSpell = i;
			}
			CalcStats();
			if (StatusIcons.Count > 0)
			{
				UpdateIcons();
			}
			if (Myself == GameData.PlayerControl.CurrentTarget)
			{
				GameData.NPCEffects.UpdateTargetEffects(StatusEffects);
			}
			if (!Myself.isNPC)
			{
				GameData.PlayerStatDisp.UpdateDisplayStats();
			}
			else if (_specificCaster != null && !Charmed && spell.Type != Spell.SpellType.Beneficial)
			{
				GetComponent<NPC>().ManageAggro(spell.Aggro, _specificCaster);
			}
			break;
		}
	}

	public bool CheckForHigherLevelSE(Spell spell)
	{
		for (int i = 0; i <= StatusEffects.Length - 1; i++)
		{
			if (StatusEffects[i] != null && StatusEffects[i].Effect != null && spell != null && StatusEffects[i].Effect.Line == spell.Line && spell.Line != Spell.SpellLine.Generic && StatusEffects[i].Effect.RequiredLevel > spell.RequiredLevel)
			{
				return true;
			}
		}
		return false;
	}

	public bool CheckForHigherLevelSEFromMe(Spell spell, Character _attacker)
	{
		for (int i = 0; i <= StatusEffects.Length - 1; i++)
		{
			if (StatusEffects[i] != null && StatusEffects[i].Effect != null && spell != null && StatusEffects[i].Owner == _attacker && StatusEffects[i].Effect.Line == spell.Line && spell.Line != Spell.SpellLine.Generic && StatusEffects[i].Effect.RequiredLevel > spell.RequiredLevel)
			{
				return true;
			}
		}
		return false;
	}

	public bool CheckForHigherLevelSEFromMeToRefresh(Spell spell, Character _attacker)
	{
		for (int i = 0; i <= StatusEffects.Length - 1; i++)
		{
			if (StatusEffects[i] != null && StatusEffects[i].Effect != null && spell != null && StatusEffects[i].Owner == _attacker && StatusEffects[i].Effect.Line == spell.Line && spell.Line != Spell.SpellLine.Generic && StatusEffects[i].Effect.RequiredLevel >= spell.RequiredLevel)
			{
				return true;
			}
		}
		return false;
	}

	public bool CheckForStatus(Spell spell)
	{
		for (int i = 0; i <= StatusEffects.Length - 1; i++)
		{
			if (StatusEffects[i] != null && StatusEffects[i].Effect != null && StatusEffects[i].Effect.Id != null && StatusEffects[i].Effect.Id == spell.Id)
			{
				return true;
			}
		}
		return false;
	}

	public bool CheckForSEByName(string _name)
	{
		for (int i = 0; i <= StatusEffects.Length - 1; i++)
		{
			if (StatusEffects[i] != null && StatusEffects[i].Effect != null && StatusEffects[i].Effect.SpellName == _name)
			{
				return true;
			}
		}
		return false;
	}

	public bool CheckForHigherSEByLine(Spell _spell)
	{
		if (_spell.Line == Spell.SpellLine.Generic)
		{
			return false;
		}
		for (int i = 0; i <= StatusEffects.Length - 1; i++)
		{
			if (StatusEffects[i] != null && StatusEffects[i].Effect != null && StatusEffects[i].Effect.Line == _spell.Line && StatusEffects[i].Effect.RequiredLevel >= _spell.RequiredLevel)
			{
				return true;
			}
		}
		return false;
	}

	public void RemoveBreakableEffects()
	{
		for (int i = 0; i <= StatusEffects.Length - 1; i++)
		{
			if (StatusEffects[i] == null || !(StatusEffects[i].Effect != null))
			{
				continue;
			}
			if (StatusEffects[i].Effect.BreakOnDamage)
			{
				RemoveStatusEffect(i);
			}
			else if (StatusEffects[i] != null && StatusEffects[i].Effect != null && StatusEffects[i].Effect.UnstableDuration && UnityEngine.Random.Range(0, 10) < 2)
			{
				RemoveStatusEffect(i);
				if (Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 10f)
				{
					UpdateSocialLog.LogAdd(base.transform.name + " comes to their senses!", "yellow");
				}
			}
		}
	}

	public void CheckAuras()
	{
		if (MyInv == null || Myself == null)
		{
			return;
		}
		if (MyInv.WornEffects.Count > 0)
		{
			foreach (Spell wornEffect in MyInv.WornEffects)
			{
				if (!(wornEffect == null))
				{
					if (!CheckForStatus(wornEffect) && !CheckForHigherLevelSE(wornEffect))
					{
						AddStatusEffect(wornEffect, !Myself.isNPC, 0, Myself);
					}
					else if (CheckForStatus(wornEffect))
					{
						RefreshWornSE(wornEffect);
					}
				}
			}
		}
		if (!Myself.isNPC)
		{
			if (MyInv.AuraSlot != null && MyInv.AuraSlot.MyItem != null && MyInv.AuraSlot.MyItem.Aura != null)
			{
				if (MyAura != null && MyAura != MyInv.AuraSlot.MyItem.Aura)
				{
					MyAura = null;
				}
				if (!CheckForStatus(MyInv.AuraSlot.MyItem.Aura) && !CheckForHigherLevelSE(MyInv.AuraSlot.MyItem.Aura))
				{
					AddStatusEffect(MyInv.AuraSlot.MyItem.Aura, !Myself.isNPC, 0, Myself);
					MyAura = MyInv.AuraSlot.MyItem.Aura;
				}
				else if (CheckForStatus(MyInv.AuraSlot.MyItem.Aura))
				{
					RefreshWornSE(MyInv.AuraSlot.MyItem.Aura);
				}
			}
			else
			{
				MyAura = null;
			}
		}
		else if (Myself.isNPC && Myself.MyNPC.SimPlayer)
		{
			if (MyAura != null && !CheckForStatus(MyAura) && !CheckForHigherLevelSE(MyAura))
			{
				AddStatusEffect(MyAura, !Myself.isNPC, 0, Myself);
			}
			else if (MyAura != null && CheckForStatus(MyAura))
			{
				RefreshWornSE(MyAura);
			}
		}
	}

	public void AddAuras(Spell _aura)
	{
		if (_aura != null && !CheckForStatus(_aura) && !CheckForHigherLevelSE(_aura))
		{
			AddStatusEffect(_aura, _fromPlayer: false, 0, Myself);
		}
		else if (CheckForStatus(_aura))
		{
			RefreshWornSE(_aura);
		}
	}

	public void RefreshWornSE(Spell _spell)
	{
		for (int i = 0; i <= StatusEffects.Length - 1; i++)
		{
			if (StatusEffects[i] != null && StatusEffects[i].Effect != null && StatusEffects[i].Effect.Id == _spell.Id)
			{
				StatusEffects[i].Duration = _spell.SpellDurationInTicks;
				break;
			}
		}
	}

	public void RemoveAllStatusEffects()
	{
		for (int i = 0; i < StatusEffects.Length; i++)
		{
			if (!(StatusEffects[i].Effect != null))
			{
				continue;
			}
			if (StatusEffects[i].CastedByPC && Myself.isNPC)
			{
				UpdateSocialLog.LogAdd("Your " + StatusEffects[i].Effect.SpellName + " spell has worn off of " + base.transform.name, "yellow");
			}
			else if (!Myself.isNPC)
			{
				UpdateSocialLog.LogAdd("The " + StatusEffects[i].Effect.SpellName + " spell has worn off of you.", "yellow");
			}
			if (StatusEffects[i].Effect.ShieldingAmt > 0)
			{
				SpellShield -= StatusEffects[i].Effect.ShieldingAmt;
				if (SpellShield < 0)
				{
					SpellShield = 0;
				}
				currentShieldSpell = -1;
			}
			if (StatusEffects[i].Effect.RootTarget)
			{
				Rooted = false;
				if (!Myself.isNPC)
				{
					UpdateSocialLog.LogAdd("You have escaped from your bonds!", "lightblue");
				}
			}
			if (StatusEffects[i].Effect.CharmTarget)
			{
				Charmed = false;
				Myself.BreakCharm();
			}
			StatusEffects[i].Effect = null;
			StatusEffects[i].fromPlayer = false;
			StatusEffects[i].Owner = null;
		}
		CalcStats();
		if (StatusIcons.Count > 0)
		{
			UpdateIcons();
		}
		if (Myself == GameData.PlayerControl.CurrentTarget)
		{
			GameData.NPCEffects.UpdateTargetEffects(StatusEffects);
		}
		if (!Myself.isNPC)
		{
			GameData.PlayerStatDisp.UpdateDisplayStats();
		}
		if (CurrentHP > CurrentMaxHP)
		{
			CurrentHP = CurrentMaxHP;
		}
	}

	public bool IsFullHP()
	{
		return CurrentHP >= CurrentMaxHP;
	}

	public bool RecentlyDamagedByPlayer()
	{
		return RecentDmgByPlayer > 0f;
	}

	public bool AmISoftMezzed()
	{
		if (CheckForSEByName("Dazzle") || CheckForSEByName("Coma") || CheckForSEByName("Sleep"))
		{
			return true;
		}
		return false;
	}

	public int AddStatusEffect(Spell spell, bool _fromPlayer, int _dmgBonus, Character _specificCaster, float _duration)
	{
		bool flag = false;
		if (!Myself.Alive)
		{
			return -1;
		}
		if (CheckForHigherLevelSE(spell))
		{
			if (!Myself.isNPC)
			{
				UpdateSocialLog.LogAdd("The " + spell.SpellName + " spell did not take hold on you!", "lightblue");
				return -1;
			}
			if (Myself.isNPC && Myself.GetComponent<NPC>().SimPlayer)
			{
				return -1;
			}
		}
		if (CheckForHigherLevelSEFromMe(spell, _specificCaster))
		{
			if (_fromPlayer)
			{
				UpdateSocialLog.LogAdd("Your " + spell.SpellName + " spell did not take hold!", "lightblue");
			}
			return -1;
		}
		flag = _specificCaster.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByID("14198580"));
		bool flag2 = false;
		for (int i = 0; i < 30; i++)
		{
			if (StatusEffects[i] == null || !(spell != null) || !(StatusEffects[i].Owner == _specificCaster) || (!(StatusEffects[i].Effect == spell) && (!(StatusEffects[i].Effect != null) || StatusEffects[i].Effect.Line != spell.Line || StatusEffects[i].Effect.RequiredLevel > spell.RequiredLevel || spell.Line == Spell.SpellLine.Generic)))
			{
				continue;
			}
			CheckResist(spell.MyDamageType, 0f, _specificCaster);
			if (_specificCaster.transform.name == "Player")
			{
				StatusEffects[i].CastedByPC = true;
			}
			else
			{
				StatusEffects[i].CastedByPC = false;
			}
			StatusEffects[i].bonusDmg = _dmgBonus;
			StatusEffects[i].Duration = _duration;
			StatusEffects[i].Effect.ReapAndRenew = flag;
			StatusEffects[i].Effect = spell;
			StatusEffects[i].Owner = _specificCaster;
			StatusEffects[i].CreditDPS = _specificCaster;
			if (spell.RootTarget)
			{
				Rooted = true;
				if (!Myself.isNPC)
				{
					UpdateSocialLog.LogAdd("You have been ROOTED!", "lightblue");
				}
			}
			if (spell.StunTarget)
			{
				if (!Unstunnable)
				{
					if (spell.BreakOnDamage || StunCooldown <= 0f || Feared)
					{
						Stunned = true;
						if (!spell.BreakOnDamage)
						{
							StunCooldown = spell.SpellDurationInTicks * 8;
						}
						if (!Myself.isNPC)
						{
							UpdateSocialLog.LogAdd("You have been STUNNED!", "lightblue");
						}
					}
				}
				else if (!Myself.isNPC)
				{
					UpdateSocialLog.LogAdd("You shake off the stun!", "lightblue");
				}
				else if (Myself.isNPC && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 25f)
				{
					UpdateSocialLog.LogAdd(base.transform.name + " shakes off the stun!", "lightblue");
				}
			}
			if (StatusEffects[i].Effect.ShieldingAmt > 0 && SpellShield < StatusEffects[i].Effect.ShieldingAmt)
			{
				SpellShield += StatusEffects[i].Effect.ShieldingAmt;
				currentShieldSpell = i;
			}
			CalcStats();
			if (StatusIcons.Count > 0)
			{
				UpdateIcons();
			}
			if (Myself == GameData.PlayerControl.CurrentTarget)
			{
				GameData.NPCEffects.UpdateTargetEffects(StatusEffects);
			}
			if (!Myself.isNPC)
			{
				GameData.PlayerStatDisp.UpdateDisplayStats();
			}
			else if (_fromPlayer && !Charmed && spell.Type != Spell.SpellType.Beneficial)
			{
				GetComponent<NPC>().ManageAggro(spell.Aggro, GameData.PlayerControl.Myself);
			}
			flag2 = true;
			return i;
		}
		if (!flag2)
		{
			for (int j = 0; j < 30; j++)
			{
				if (StatusEffects[j] == null || !(spell != null) || CheckForHigherLevelSEFromMe(spell, _specificCaster) || !(StatusEffects[j].Effect == null))
				{
					continue;
				}
				if (spell.RootTarget)
				{
					Rooted = true;
					if (!Myself.isNPC)
					{
						UpdateSocialLog.LogAdd("You have been ROOTED!", "lightblue");
					}
				}
				if (spell.StunTarget)
				{
					if (!Unstunnable)
					{
						if (spell.BreakOnDamage || StunCooldown <= 0f || Feared)
						{
							Stunned = true;
							if (!spell.BreakOnDamage)
							{
								StunCooldown = spell.SpellDurationInTicks * 8;
							}
							if (!Myself.isNPC)
							{
								UpdateSocialLog.LogAdd("You have been STUNNED!", "lightblue");
							}
						}
					}
					else if (!Myself.isNPC)
					{
						UpdateSocialLog.LogAdd("You shake off the stun!", "lightblue");
					}
					else if (Myself.isNPC && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 25f)
					{
						UpdateSocialLog.LogAdd(base.transform.name + " shakes off the stun!", "lightblue");
					}
				}
				CheckResist(spell.MyDamageType, 0f, Myself);
				if (_specificCaster.transform.name == "Player")
				{
					StatusEffects[j].CastedByPC = true;
				}
				else
				{
					StatusEffects[j].CastedByPC = false;
				}
				StatusEffects[j].bonusDmg = _dmgBonus;
				StatusEffects[j].fromPlayer = _fromPlayer;
				StatusEffects[j].Effect = spell;
				StatusEffects[j].Effect.ReapAndRenew = flag;
				StatusEffects[j].Owner = _specificCaster;
				StatusEffects[j].CreditDPS = _specificCaster;
				if (!spell.UnstableDuration)
				{
					StatusEffects[j].Duration = _duration;
				}
				else
				{
					StatusEffects[j].Duration = _duration;
				}
				if (StatusEffects[j].Effect.ShieldingAmt > 0 && SpellShield < StatusEffects[j].Effect.ShieldingAmt)
				{
					SpellShield = StatusEffects[j].Effect.ShieldingAmt;
					currentShieldSpell = j;
				}
				if (StatusEffects[j].Effect.CharmTarget)
				{
					if (spell.MaxLevelTarget >= Level)
					{
						if (_specificCaster.MyCharmedNPC == null)
						{
							Charmed = true;
							Myself.CharmMe(_specificCaster);
						}
						else
						{
							UpdateSocialLog.LogAdd("You lack the power to charm two beings!", "yellow");
						}
					}
					else
					{
						UpdateSocialLog.LogAdd("This spell is not strong enough...", "yellow");
					}
				}
				CalcStats();
				if (StatusIcons.Count > 0)
				{
					UpdateIcons();
				}
				if (Myself == GameData.PlayerControl.CurrentTarget)
				{
					GameData.NPCEffects.UpdateTargetEffects(StatusEffects);
				}
				if (!Myself.isNPC)
				{
					GameData.PlayerStatDisp.UpdateDisplayStats();
				}
				else if (_fromPlayer && !Charmed && spell.Type != Spell.SpellType.Beneficial)
				{
					GetComponent<NPC>().ManageAggro(spell.Aggro, GameData.PlayerControl.Myself);
				}
				flag2 = true;
				return j;
			}
		}
		if (!flag2 && spell != null && spell.Type == Spell.SpellType.StatusEffect && !CheckForHigherLevelSEFromMe(spell, _specificCaster))
		{
			int num = UnityEngine.Random.Range(0, 30);
			if (spell.RootTarget)
			{
				Rooted = true;
			}
			CheckResist(spell.MyDamageType, 0f, Myself);
			StatusEffects[num].bonusDmg = _dmgBonus;
			StatusEffects[num].fromPlayer = _fromPlayer;
			StatusEffects[num].Effect = spell;
			StatusEffects[num].Effect.ReapAndRenew = flag;
			StatusEffects[num].Owner = _specificCaster;
			StatusEffects[num].CreditDPS = _specificCaster;
			if (!spell.UnstableDuration)
			{
				StatusEffects[num].Duration = _duration;
			}
			else
			{
				StatusEffects[num].Duration = _duration;
			}
			if (spell.RootTarget)
			{
				Rooted = true;
				if (!Myself.isNPC)
				{
					UpdateSocialLog.LogAdd("You have been ROOTED!", "lightblue");
				}
			}
			if (spell.StunTarget)
			{
				if (!Unstunnable)
				{
					if (spell.BreakOnDamage || StunCooldown <= 0f || Feared)
					{
						Stunned = true;
						if (!spell.BreakOnDamage)
						{
							StunCooldown = spell.SpellDurationInTicks * 8;
						}
						if (!Myself.isNPC)
						{
							UpdateSocialLog.LogAdd("You have been STUNNED!", "lightblue");
						}
					}
				}
				else if (!Myself.isNPC)
				{
					UpdateSocialLog.LogAdd("You shake off the stun!", "lightblue");
				}
				else if (Myself.isNPC && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 25f)
				{
					UpdateSocialLog.LogAdd(base.transform.name + " shakes off the stun!", "lightblue");
				}
			}
			if (StatusEffects[num].Effect.ShieldingAmt > 0 && SpellShield < StatusEffects[num].Effect.ShieldingAmt)
			{
				SpellShield = StatusEffects[num].Effect.ShieldingAmt;
				currentShieldSpell = num;
			}
			if (StatusEffects[num].Effect.CharmTarget)
			{
				if (spell.MaxLevelTarget >= Level)
				{
					if (_specificCaster.MyCharmedNPC == null)
					{
						Charmed = true;
						Myself.CharmMe(_specificCaster);
					}
					else
					{
						UpdateSocialLog.LogAdd("You lack the power to charm two beings!", "yellow");
					}
				}
				else
				{
					UpdateSocialLog.LogAdd("This spell is not strong enough...", "yellow");
				}
			}
			CalcStats();
			if (StatusIcons.Count > 0)
			{
				UpdateIcons();
			}
			if (Myself == GameData.PlayerControl.CurrentTarget)
			{
				GameData.NPCEffects.UpdateTargetEffects(StatusEffects);
			}
			if (!Myself.isNPC)
			{
				GameData.PlayerStatDisp.UpdateDisplayStats();
			}
			else if (_fromPlayer && !Charmed && spell.Type != Spell.SpellType.Beneficial)
			{
				GetComponent<NPC>().ManageAggro(spell.Aggro, GameData.PlayerControl.Myself);
			}
			flag2 = true;
			return num;
		}
		if (!flag2)
		{
			if (!Myself.isNPC)
			{
				UpdateSocialLog.LogAdd("The " + spell.SpellName + " spell did not take hold on you!", "lightblue");
			}
			else if (_fromPlayer)
			{
				UpdateSocialLog.LogAdd("Your " + spell.SpellName + " spell did not take hold!", "lightblue");
			}
		}
		return -1;
	}

	public void AssignPoint()
	{
		int num = UnityEngine.Random.Range(0, 100);
		switch (CharacterClass.ClassName)
		{
		case "Paladin":
			if (num > 60)
			{
				EndScaleSpent++;
			}
			else if (num > 20)
			{
				AgiScaleSpent++;
			}
			else if (num > 10)
			{
				StrScaleSpent++;
			}
			else if (num > 1)
			{
				DexScaleSpent++;
			}
			else if (num == 0)
			{
				WisScaleSpent++;
			}
			else
			{
				ChaScaleSpent++;
			}
			break;
		case "Duelist":
			if (num > 50)
			{
				StrScaleSpent++;
			}
			else if (num > 25)
			{
				DexScaleSpent++;
			}
			else if (num > 10)
			{
				IntScaleSpent++;
			}
			else if (num > 5)
			{
				EndScaleSpent++;
			}
			else if (num > 2)
			{
				AgiScaleSpent++;
			}
			else
			{
				ChaScaleSpent++;
			}
			break;
		case "Arcanist":
			if (num > 35)
			{
				IntScaleSpent++;
			}
			else if (num > 10)
			{
				ChaScaleSpent++;
			}
			else if (num > 5)
			{
				WisScaleSpent++;
			}
			else if (num > 2)
			{
				EndScaleSpent++;
			}
			else if (num > 0)
			{
				AgiScaleSpent++;
			}
			else
			{
				StrScaleSpent++;
			}
			break;
		case "Druid":
			if (num > 60)
			{
				WisScaleSpent++;
			}
			else if (num > 30)
			{
				IntScaleSpent++;
			}
			else if (num > 20)
			{
				EndScaleSpent++;
			}
			else if (num > 10)
			{
				ChaScaleSpent++;
			}
			else if (num > 5)
			{
				DexScaleSpent++;
			}
			else
			{
				AgiScaleSpent++;
			}
			break;
		case "Reaver":
			if (num > 60)
			{
				StrScaleSpent++;
			}
			else if (num > 40)
			{
				DexScaleSpent++;
			}
			else if (num > 25)
			{
				IntScaleSpent++;
			}
			else if (num > 15)
			{
				EndScaleSpent++;
			}
			else if (num > 5)
			{
				AgiScaleSpent++;
			}
			else
			{
				ChaScaleSpent++;
			}
			break;
		case "Stormcaller":
			if (num > 40)
			{
				IntScaleSpent++;
			}
			else if (num > 20)
			{
				DexScaleSpent++;
			}
			else if (num > 10)
			{
				ChaScaleSpent++;
			}
			else if (num > 5)
			{
				EndScaleSpent++;
			}
			else if (num > 2)
			{
				StrScaleSpent++;
			}
			else
			{
				AgiScaleSpent++;
			}
			break;
		}
		TotalAvailableProficiencies--;
		CalcStats();
	}

	public void ClearSpentProfPoints()
	{
		StrScaleSpent = 0;
		EndScaleSpent = 0;
		DexScaleSpent = 0;
		AgiScaleSpent = 0;
		IntScaleSpent = 0;
		WisScaleSpent = 0;
		ChaScaleSpent = 0;
		CalcStats();
	}

	public void ModifySEDurationByPercent(int _index, float _byPercent)
	{
		if (StatusEffects[_index] != null && !(StatusEffects[_index].Effect == null))
		{
			StatusEffects[_index].Duration *= _byPercent / 100f;
		}
	}

	public void ChangeStance(Stance _newStance)
	{
		Myself.MyAudio.PlayOneShot(GameData.Misc.Stance, GameData.SFXVol * 0.25f * GameData.MasterVol);
		if (_newStance == CombatStance)
		{
			ChangeStance(GameData.SkillDatabase.NormalStance);
			return;
		}
		CombatStance = _newStance;
		if (Myself.isNPC && Myself.MyNPC.InGroup)
		{
			UpdateSocialLog.LogAdd(Myself.MyNPC.NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString("Switching to " + _newStance.DisplayName + " stance!", Myself.MyNPC.ThisSim), "#00B2B7");
		}
		if (Myself.isNPC && Vector3.Distance(GameData.PlayerControl.transform.position, base.transform.position) < 10f)
		{
			UpdateSocialLog.LogAdd(base.transform.name + " " + _newStance.SwitchMessage);
		}
		else if (!Myself.isNPC)
		{
			UpdateSocialLog.LogAdd(MyName + " " + _newStance.SwitchMessage);
		}
	}
}
