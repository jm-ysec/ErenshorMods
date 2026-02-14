// Character
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class Character : MonoBehaviour
{
	public enum Faction
	{
		Player,
		Villager,
		PreyAnimal,
		PredatorAnimal,
		Undead,
		EvilHuman,
		GoodHuman,
		EvilGuard,
		GoodGuard,
		OtherEvil,
		OtherGood,
		PC,
		DEBUG,
		Mineral,
		TreasureChest,
		Unseen
	}

	public bool Alive = true;

	public GameObject TargetRing;

	public WorldFaction MyWorldFaction;

	public Faction MyFaction;

	public Faction BaseFaction;

	public Faction TempFaction;

	public float AggroRange;

	public Transform MyAggro;

	public List<Faction> AggressiveTowards;

	public List<Faction> Allies;

	public List<Faction> HoldForCharm = new List<Faction>();

	public bool isNPC;

	public bool isVendor;

	private LootTable MyLootTable;

	private int xp = 1;

	public Vector2 BonusRangeXP = Vector2.zero;

	private Animator MyAnim;

	public UseSkill MySkills;

	public CastSpell MySpells;

	public float AttackRange = 3f;

	private NavMeshAgent MyNav;

	public List<Character> NearbyFriends = new List<Character>();

	public List<Character> NearbyEnemies = new List<Character>();

	public List<Door> NearbyDoors = new List<Door>();

	public Stats MyStats;

	public NPC MyNPC;

	public Quest QuestCompleteOnDeath;

	private int DmgFromPlayerSource;

	private bool damagedByPlayerEver;

	public AudioClip MyAttackSound;

	public AudioClip MyHurtSound;

	public AudioClip MyDieSound;

	public AudioClip[] Footsteps;

	public AudioSource MyAudio;

	public bool Invulnerable;

	public float OverrideAnimSpeed;

	private Transform alternateAttacker;

	public GameObject HitFX;

	public ModifyFaction[] factionMods;

	public NPC MyCharmedNPC;

	public Character Master;

	private bool removedHate;

	public bool MiningNode;

	public CapsuleCollider MyCap;

	public bool ShrinkColliderOnDeath = true;

	private bool affectPlayer;

	public bool Relax = true;

	public List<string> ShoutOnDeath;

	private float deadTimer = 60f;

	private int RollingDmg;

	private float RollingDPSTimer;

	private float RollingDPS;

	private List<int> IncHits = new List<int>();

	private int highestHit;

	private int lowestHit = 999999;

	private int AverageHit;

	public bool DestroyOnDeath;

	public CorpseData savedCorpse;

	private ParticleSystem targetParticle;

	public bool trailerRecord;

	public float TargetRingMod;

	private float TotalDmg;

	public bool NoFlinch;

	private GameObject LootPrompt;

	public float UnderThreat;

	public float BossXp;

	public List<AudioClip> IdleSounds;

	private float IdleSoundDel = 100f;

	public Character LastHitBy;

	public bool IsWyrm;

	public GameObject SpawnOnDeath;

	public float contributedDPS;

	public bool NoRun;

	public bool SeeInvisible;

	public bool CanNeverSeeInvis;

	private void Awake()
	{
		if (MyStats == null)
		{
			MyStats = GetComponent<Stats>();
		}
		if (MyAudio == null)
		{
			MyAudio = GetComponent<AudioSource>();
		}
		MyAudio.maxDistance = 50f;
		if (MySkills == null)
		{
			MySkills = GetComponent<UseSkill>();
		}
		if (MySpells == null)
		{
			MySpells = GetComponent<CastSpell>();
		}
		if (GetComponent<NPC>() != null)
		{
			isNPC = true;
		}
	}

	private void Start()
	{
		if (MyCap == null)
		{
			MyCap = GetComponent<CapsuleCollider>();
		}
		if (GetComponent<MiningNode>() != null)
		{
			MiningNode = true;
		}
		BaseFaction = MyFaction;
		base.transform.tag = "Character";
		if (TargetRing == null)
		{
			TargetRing = base.transform.Find("TargetRing").gameObject;
		}
		Vector3 position = TargetRing.transform.position;
		Vector3 localScale = Vector3.one;
		if (TargetRingMod != 0f)
		{
			localScale = new Vector3(TargetRingMod, TargetRingMod, TargetRingMod);
		}
		Object.DestroyImmediate(TargetRing);
		TargetRing = null;
		TargetRing = Object.Instantiate(GameData.GM.TargetRing, base.transform);
		TargetRing.transform.position = position;
		TargetRing.transform.localScale = localScale;
		targetParticle = TargetRing.GetComponent<ParticleSystem>();
		TargetRing.SetActive(value: false);
		if (MyFaction != Faction.PC || GetComponent<SimPlayer>() != null)
		{
			if (MyAggro == null)
			{
				MyAggro = GetComponentInChildren<NPCAggroArea>().transform;
			}
			MyAggro.localScale = Vector3.one * AggroRange;
			factionMods = GetComponents<ModifyFaction>();
		}
		MyNav = GetComponent<NavMeshAgent>();
		MyStats = GetComponent<Stats>();
		MyAnim = GetComponent<Animator>();
		if (OverrideAnimSpeed != 0f)
		{
			MyAnim.speed = OverrideAnimSpeed;
		}
		if (GetComponent<NPC>() != null)
		{
			isNPC = true;
		}
		else
		{
			MySpells = GetComponent<CastSpell>();
		}
		if (GetComponent<VendorInventory>() != null)
		{
			isVendor = true;
		}
		if (isNPC)
		{
			MyLootTable = GetComponent<LootTable>();
		}
		xp = MyStats.Level * 4;
		HitFX = Object.Instantiate(GameData.Misc.HitFX);
		HitFX.transform.position = base.transform.position + Vector3.up;
		HitFX.transform.parent = base.transform;
		if (MyWorldFaction == null && MyFaction != Faction.PC && MyFaction != 0)
		{
			LoadGenericWorldFaction();
		}
		if (isNPC && !GetComponent<NPC>().SimPlayer && MyFaction != Faction.PC && MyFaction != 0)
		{
			LootPrompt = Object.Instantiate(GameData.Misc.LootableTxt);
			LootPrompt.transform.SetParent(base.transform);
			LootPrompt.transform.localPosition = new Vector3(0f, 0.7f, 0f);
			LootPrompt.SetActive(value: false);
		}
		if (MyCap == null)
		{
			UpdateSocialLog.LogAdd("ERROR: No Collider on " + base.transform.name, "yellow");
		}
		if (!CanNeverSeeInvis && GetComponent<SimPlayer>() == null)
		{
			if (BossXp > 1f)
			{
				SeeInvisible = true;
			}
			else if (Random.Range(0, 100) < Mathf.Max(20, MyStats.Level - 10))
			{
				SeeInvisible = true;
			}
			else if (!string.IsNullOrEmpty(GameData.GuildManager.trueDest) && SceneManager.GetActiveScene().name == GameData.GuildManager.trueDest)
			{
				SeeInvisible = true;
			}
		}
	}

	private void Update()
	{
		if (UnderThreat > 0f)
		{
			UnderThreat -= 60f * Time.deltaTime;
		}
		if (contributedDPS > 0f)
		{
			contributedDPS -= 60f * Time.deltaTime;
		}
		if (trailerRecord)
		{
			MyStats.CurrentHP = MyStats.CurrentMaxHP;
		}
		if (MyStats.GetCurrentHP() <= 0 && Alive)
		{
			MyStats.CurrentHP = 0;
			Relax = true;
			DoDeath();
		}
		if (MyStats.Charmed && MyFaction == BaseFaction)
		{
			MyFaction = TempFaction;
		}
		else if (!MyStats.Charmed && MyFaction != BaseFaction)
		{
			MyFaction = BaseFaction;
		}
		if (MyStats.Charmed && (Master == null || !Master.Alive))
		{
			BreakCharm();
		}
		if (MyWorldFaction != null && MyFaction != Faction.PC && MyFaction != 0)
		{
			MyWorldFaction.FactionValue = GlobalFactionManager.FindFactionData(MyWorldFaction.REFNAME).Value;
			if (MyWorldFaction.FactionValue < 0f)
			{
				if (Allies.Contains(Faction.PC))
				{
					Allies.Remove(Faction.PC);
				}
				if (!AggressiveTowards.Contains(Faction.PC))
				{
					AggressiveTowards.Add(Faction.PC);
				}
			}
		}
		if (!isNPC && !Alive)
		{
			if (GroupMemberAlive() && GroupMembersRelaxed() && IsGroupMemberNearby())
			{
				if (deadTimer <= 0f)
				{
					for (int i = 0; i <= 9; i++)
					{
						if (GameData.PlayerStats.StatusEffects[i].Effect != null)
						{
							GameData.PlayerStats.RemoveStatusEffect(i);
						}
					}
					GameData.PlayerControl.Myself.Alive = true;
					GameData.PlayerControl.Myself.MyStats.CurrentHP = 10;
					GameData.PlayerControl.Myself.GetComponent<Animator>().SetBool("Dead", value: false);
					GameData.PlayerControl.Myself.GetComponent<Animator>().SetTrigger("Revive");
					GameData.PlayerControl.Myself.GetComponent<PlayerCombat>().ForceAttackOff();
					deadTimer = 60f;
				}
				else
				{
					deadTimer -= 60f * Time.deltaTime;
				}
			}
			else if (!GroupMemberAlive())
			{
				GameData.RespawnWindow.SetActive(value: true);
			}
		}
		if (!Alive)
		{
			MyStats.CurrentHP = 0;
			Relax = true;
			if (LootPrompt != null && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 5f)
			{
				if (GameData.PlayerCombat.FindNearestLootable() == this)
				{
					if (!LootPrompt.activeSelf)
					{
						LootPrompt.SetActive(value: true);
						LootPrompt.GetComponent<TextMeshPro>().text = InputManager.Loot.ToString() + " - Loot";
					}
				}
				else if (LootPrompt.activeSelf)
				{
					LootPrompt.SetActive(value: false);
				}
			}
			if (MyNPC != null && !MyNPC.SimPlayer && MyLootTable != null && MyLootTable.special)
			{
				Object.Instantiate(GameData.GM.SpecialLootBeam, base.transform.position + Vector3.up, base.transform.rotation).transform.SetParent(base.transform);
				MyLootTable.special = false;
			}
		}
		if ((MyNPC != null && MyNPC.CurrentAggroTarget != null) || (!isNPC && GameData.PlayerControl.CurrentTarget != null && (GameData.Autoattacking || MySpells.isCasting() || contributedDPS > 0f)))
		{
			RollingDPSTimer += Time.deltaTime;
			TotalDmg = RollingDmg;
			RollingDPS = TotalDmg / RollingDPSTimer;
		}
		if (Alive && !isNPC && (GameData.GroupMembers[0] != null || GameData.GroupMembers[1] != null || GameData.GroupMembers[2] != null || GameData.GroupMembers[3] != null) && GameData.Misc.RevWindow.activeSelf)
		{
			GameData.Misc.RevWindow.SetActive(value: false);
		}
		if (Alive && IdleSounds.Count > 0 && IdleSoundDel >= 0f)
		{
			IdleSoundDel -= 60f * Time.deltaTime;
			if (IdleSoundDel <= 0f)
			{
				MyAudio.PlayOneShot(IdleSounds[Random.Range(0, IdleSounds.Count)], GameData.SFXVol * GameData.MasterVol);
				IdleSoundDel = Random.Range(130, 600);
			}
		}
	}

	public bool ValidNav()
	{
		if (MyNav != null)
		{
			return true;
		}
		return false;
	}

	public Vector3 GetNavPos()
	{
		if (TargetRing == null)
		{
			return base.transform.position;
		}
		return TargetRing.transform.position;
	}

	public void ResetRollingDPS()
	{
		RollingDPSTimer = 0f;
		RollingDPS = 0f;
		RollingDmg = 0;
		TotalDmg = 0f;
	}

	private bool GroupMembersRelaxed()
	{
		if (GameData.GroupMembers[0] != null && GameData.GroupMembers[0].MyAvatar != null && !GameData.GroupMembers[0].MyAvatar.MyStats.Myself.Relax)
		{
			return false;
		}
		if (GameData.GroupMembers[1] != null && GameData.GroupMembers[1].MyAvatar != null && !GameData.GroupMembers[1].MyAvatar.MyStats.Myself.Relax)
		{
			return false;
		}
		if (GameData.GroupMembers[2] != null && GameData.GroupMembers[2].MyAvatar != null && !GameData.GroupMembers[2].MyAvatar.MyStats.Myself.Relax)
		{
			return false;
		}
		if (GameData.GroupMembers[3] != null && GameData.GroupMembers[3].MyAvatar != null && !GameData.GroupMembers[3].MyAvatar.MyStats.Myself.Relax)
		{
			return false;
		}
		GameData.SimPlayerGrouping.GroupTargets.Clear();
		return true;
	}

	private bool GroupMemberAlive()
	{
		if (GameData.GroupMembers[0] != null && GameData.GroupMembers[0].MyAvatar != null && GameData.GroupMembers[0].MyAvatar.MyStats.Myself.Alive)
		{
			return true;
		}
		if (GameData.GroupMembers[1] != null && GameData.GroupMembers[1].MyAvatar != null && GameData.GroupMembers[1].MyAvatar.MyStats.Myself.Alive)
		{
			return true;
		}
		if (GameData.GroupMembers[2] != null && GameData.GroupMembers[2].MyAvatar != null && GameData.GroupMembers[2].MyAvatar.MyStats.Myself.Alive)
		{
			return true;
		}
		if (GameData.GroupMembers[3] != null && GameData.GroupMembers[3].MyAvatar != null && GameData.GroupMembers[3].MyAvatar.MyStats.Myself.Alive)
		{
			return true;
		}
		return false;
	}

	private bool IsGroupMemberNearby()
	{
		if (GameData.GroupMembers[0] != null && GameData.GroupMembers[0].MyAvatar != null && GameData.GroupMembers[0].MyAvatar.MyStats.Myself.Alive && Vector3.Distance(GameData.GroupMembers[0].MyAvatar.transform.position, base.transform.position) < 7f)
		{
			return true;
		}
		if (GameData.GroupMembers[1] != null && GameData.GroupMembers[1].MyAvatar != null && GameData.GroupMembers[1].MyAvatar.MyStats.Myself.Alive && Vector3.Distance(GameData.GroupMembers[1].MyAvatar.transform.position, base.transform.position) < 7f)
		{
			return true;
		}
		if (GameData.GroupMembers[2] != null && GameData.GroupMembers[2].MyAvatar != null && GameData.GroupMembers[2].MyAvatar.MyStats.Myself.Alive && Vector3.Distance(GameData.GroupMembers[2].MyAvatar.transform.position, base.transform.position) < 7f)
		{
			return true;
		}
		if (GameData.GroupMembers[3] != null && GameData.GroupMembers[3].MyAvatar != null && GameData.GroupMembers[3].MyAvatar.MyStats.Myself.Alive && Vector3.Distance(GameData.GroupMembers[3].MyAvatar.transform.position, base.transform.position) < 7f)
		{
			return true;
		}
		return false;
	}

	private void FixedUpdate()
	{
		if (!isNPC || !(MyNPC != null))
		{
			return;
		}
		if (!affectPlayer && MyNPC.CurrentAggroTarget == GameData.PlayerControl)
		{
			affectPlayer = true;
		}
		if (affectPlayer || MyNPC.AggroTable.Count <= 0)
		{
			return;
		}
		foreach (AggroSlot item in MyNPC.AggroTable)
		{
			if (item.Player != null && item.Player.transform.name == "Player")
			{
				affectPlayer = true;
			}
		}
	}

	public void TargetMe()
	{
		int num = MyStats.Level - GameData.PlayerStats.Level;
		Color color = ((num >= 7) ? GameData.PlayerControl.DeepRed : ((num >= 5) ? GameData.PlayerControl.Red : ((num >= 3) ? GameData.PlayerControl.Orange : ((num >= 1) ? GameData.PlayerControl.Yellow : ((num == 0) ? Color.white : ((num >= -2) ? GameData.PlayerControl.CloseLow : ((num < -4) ? GameData.PlayerControl.Green : GameData.PlayerControl.EasyLow)))))));
		TargetRing.SetActive(value: true);
		if (isNPC && NearbyEnemies != null && MyNPC != null && !MyNPC.SimPlayer && GameData.PlayerStats != null && GameData.PlayerStats.Myself != null && !NearbyEnemies.Contains(GameData.PlayerStats.Myself) && base.transform.name != "Trick Target")
		{
			GameData.PlayerCombat.ForceAttackOff();
		}
		ParticleSystem.MainModule main = targetParticle.main;
		main.startColor = color;
		GameData.NPCEffects.UpdateTargetEffects(MyStats.StatusEffects);
		if (base.transform.name != "Player")
		{
			UpdateSocialLog.CombatLogAdd("New Target: " + base.transform.name);
		}
		else
		{
			UpdateSocialLog.CombatLogAdd("New Target: " + GameData.PlayerStats.MyName);
		}
	}

	public void UntargetMe()
	{
		_ = GameData.PlayerControl.CurrentTarget == this;
		TargetRing.SetActive(value: false);
	}

	public void UntargetMe(bool _attackoff)
	{
		if (GameData.PlayerControl.CurrentTarget == this && _attackoff)
		{
			GameData.PlayerControl.GetComponent<PlayerCombat>().ForceAttackOff();
		}
		TargetRing.SetActive(value: false);
	}

	public void CreditDPS(int _incDmg)
	{
		contributedDPS = 300f;
		RollingDmg += _incDmg;
		if (_incDmg > highestHit)
		{
			highestHit = _incDmg;
		}
		if (_incDmg < lowestHit)
		{
			lowestHit = _incDmg;
		}
		if (IncHits.Count < 50)
		{
			IncHits.Add(_incDmg);
		}
	}

	public int GetDPS()
	{
		return Mathf.RoundToInt(RollingDPS);
	}

	public int GetHightHit()
	{
		return highestHit;
	}

	public int GetLowestHit()
	{
		if (lowestHit != 999999)
		{
			return lowestHit;
		}
		return 0;
	}

	public int GetAvgHit()
	{
		if (IncHits.Count > 0)
		{
			return Mathf.RoundToInt((float)IncHits.Average());
		}
		return 0;
	}

	private void DoWorldEventCredit(bool _PlayerParty)
	{
		List<LiveGuildData> list = new List<LiveGuildData>();
		if (_PlayerParty)
		{
			list.Add(GameData.GuildManager.GetGuildDataByID(GameData.PlayerControl.MyGuild));
			for (int i = 0; i < GameData.GroupMembers.Length; i++)
			{
				if (GameData.GroupMembers[i] != null && !list.Contains(GameData.GuildManager.FindGuildBySimPlayerName(GameData.GroupMembers[i].SimName)))
				{
					list.Add(GameData.GuildManager.FindGuildBySimPlayerName(GameData.GroupMembers[i].SimName));
				}
			}
		}
		else
		{
			if (MyNPC == null || MyNPC.AggroTable == null)
			{
				return;
			}
			foreach (AggroSlot item in MyNPC.AggroTable)
			{
				if (item.Player.isNPC && item.Player.MyNPC != null && item.Player.MyNPC.SimPlayer && !list.Contains(GameData.GuildManager.FindGuildBySimPlayerName(item.Player.MyNPC.NPCName)))
				{
					list.Add(GameData.GuildManager.FindGuildBySimPlayerName(item.Player.MyNPC.NPCName));
				}
			}
		}
		if (list.Count > 0)
		{
			list.RemoveAll((LiveGuildData x) => x == null);
		}
		if (list.Count > 1)
		{
			GameData.GuildManager.CreditForGodTarget(list);
		}
		if (list.Count == 1 && list[0] != null)
		{
			GameData.GuildManager.CreditSingleForGodTarget(list[0]);
		}
	}

	private void DoDeath()
	{
		if (Random.Range(0, 100) < MySkills.GetAscensionRank("73824309") * 10)
		{
			MyStats.Stunned = false;
			MyStats.Feared = false;
			MyStats.RecentDmg = 0f;
			MyAnim.SetBool("Stunned", value: false);
			MyStats.AddStatusEffectNoChecks(GameData.Misc.UndyingRage, _fromPlayer: false, 0, this);
			MyAudio.PlayOneShot(GameData.Misc.UndyingSFX, GameData.SFXVol * 0.5f);
			MyStats.CurrentHP = MyStats.CurrentMaxHP;
			MyStats.CurrentMana = MyStats.GetCurrentMaxMana();
			if (!isNPC)
			{
				UpdateSocialLog.LogAdd("YOU REFUSE THE EMBRACE OF DEATH, AND REPLACE IT WITH RAGE", "red");
				GameData.CamControl.ShakeScreen(2f, 4f);
			}
			else
			{
				UpdateSocialLog.LogAdd(base.transform.name.ToUpper() + " IS OVERCOME BY UNDYING RAGE", "red");
			}
			return;
		}
		Alive = false;
		if (DestroyOnDeath && GetComponent<NPC>() != null)
		{
			GetComponent<NPC>().ExpediteRot(50f);
		}
		if (SpawnOnDeath != null)
		{
			Object.Instantiate(SpawnOnDeath, base.transform.position, base.transform.rotation);
		}
		if (factionMods.Length != 0)
		{
			if (MyNPC.CurrentAggroTarget == GameData.PlayerControl || damagedByPlayerEver)
			{
				affectPlayer = true;
			}
			if (affectPlayer)
			{
				ModifyFaction[] array = factionMods;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SubmitForModification();
				}
			}
		}
		MyStats.Stunned = false;
		MyStats.Feared = false;
		MyStats.RecentDmg = 0f;
		MyAnim.SetBool("Stunned", value: false);
		Alive = false;
		UntargetMe(_attackoff: true);
		if (isNPC)
		{
			NearbyEnemies.Clear();
		}
		MyAnim.SetBool("Dead", value: true);
		MyStats.RemoveAllStatusEffects();
		if (ShrinkColliderOnDeath)
		{
			GetComponent<CapsuleCollider>().center = new Vector3(0f, 0f, 0f);
		}
		if (MyDieSound != null && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 8f)
		{
			MyAudio.PlayOneShot(MyDieSound, MyAudio.volume * GameData.SFXVol * GameData.MasterVol);
		}
		if (isNPC)
		{
			MyNPC.CurrentAggroTarget = null;
			MyNPC.Die();
			if (GameData.PlayerControl.HuntingMe.Contains(MyNPC))
			{
				GameData.PlayerControl.HuntingMe.Remove(MyNPC);
			}
			if (NPCTable.LiveNPCs.Contains(MyNPC))
			{
				NPCTable.LiveNPCs.Remove(MyNPC);
			}
			if (GameData.AttackingPlayer.Contains(GetComponent<NPC>()))
			{
				GameData.AttackingPlayer.Remove(GetComponent<NPC>());
			}
			if (GameData.GroupMatesInCombat.Contains(GetComponent<NPC>()))
			{
				GameData.GroupMatesInCombat.Remove(GetComponent<NPC>());
			}
			GetComponent<NPC>().NamePlate.GetComponent<TextMeshPro>().text += "'s corpse";
			if (GetComponent<NavMeshAgent>() != null && GetComponent<NavMeshAgent>().isOnNavMesh)
			{
				GetComponent<NavMeshAgent>().isStopped = true;
			}
			if (DmgFromPlayerSource > MyStats.CurrentMaxHP / 2)
			{
				bool flag = false;
				if (!string.IsNullOrEmpty(GameData.GuildManager.WorldEventMob) && GameData.GuildManager.WorldEventMob == MyNPC.NPCName)
				{
					DoWorldEventCredit(_PlayerParty: true);
				}
				foreach (AggroSlot item in MyNPC.AggroTable)
				{
					if (item != null && item.Player != null && item.Player.transform != null && item.Player.transform.name == "Player")
					{
						flag = true;
					}
				}
				if (flag)
				{
					if (GameData.ZoneAnnounce != null)
					{
						GameData.ZoneAnnounceData.MobsKilledByPlayerParty++;
					}
					int num = 0;
					if (MyStats.Level >= GameData.PlayerStats.Level)
					{
						num = MyStats.Level - GameData.PlayerStats.Level;
						if (GameData.PlayerStats.Level > 10)
						{
							num++;
						}
						if (num > 5)
						{
							num = 5;
						}
						num *= MyStats.Level;
					}
					if (BossXp == 0f)
					{
						BossXp = 1f;
					}
					GameData.Misc.GetXPBalls(MyStats.Level / 3, base.transform.position + Vector3.up, GameData.PlayerControl.transform);
					int num2 = xp + num;
					num2 = Mathf.RoundToInt((float)num2 * GameData.ServerXPMod);
					if (num2 <= 0)
					{
						num2 = 1;
					}
					if (GameData.PlayerStats.Level - MyStats.Level <= 4)
					{
						GameData.AddExperience(Mathf.RoundToInt((float)num2 * BossXp), useMod: true);
					}
					else
					{
						UpdateSocialLog.LogAdd("You did not learn anything from this trivial opponent.", "yellow");
					}
					if (LastHitBy != null)
					{
						if (LastHitBy.transform.name == "Player")
						{
							UpdateSocialLog.LogAdd("You have slain " + base.transform.name);
							if (LastHitBy.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByID("35876675")))
							{
								GameData.HKMngr.ResetAllCooldowns();
							}
						}
						else
						{
							UpdateSocialLog.LogAdd(base.transform.name + " has been slain by " + LastHitBy.transform.name);
						}
					}
					else
					{
						UpdateSocialLog.LogAdd(base.transform.name + " has been slain.");
					}
					if (!string.IsNullOrEmpty(MyNPC.SetAchievementOnDefeat))
					{
						SetAchievement.Unlock(MyNPC.SetAchievementOnDefeat);
					}
					if (GameData.PlayerStats.CharacterClass == GameData.ClassDB.Duelist && GameData.PlayerStats.GetComponent<UseSkill>().KnownSkills.Contains(GameData.SkillDatabase.GetSkillByID("16164576")))
					{
						GameData.PlayerStats.AddStatusEffect(GameData.SpellDatabase.GetSpellByID("18174920"), _fromPlayer: true, 0);
					}
				}
				else if (Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 15f)
				{
					if (LastHitBy != null)
					{
						if (LastHitBy.transform.name == "Player")
						{
							UpdateSocialLog.LogAdd("You have slain " + base.transform.name);
						}
						else
						{
							UpdateSocialLog.LogAdd(base.transform.name + " has been slain by " + LastHitBy.transform.name);
						}
					}
					else
					{
						UpdateSocialLog.LogAdd(base.transform.name + " has been slain.");
					}
				}
			}
			else if (alternateAttacker != null)
			{
				if (!string.IsNullOrEmpty(GameData.GuildManager.WorldEventMob) && GameData.GuildManager.WorldEventMob == MyNPC.NPCName)
				{
					DoWorldEventCredit(_PlayerParty: false);
				}
				GameData.Misc.GetXPBalls(MyStats.Level / 3, base.transform.position + Vector3.up, alternateAttacker);
				if (alternateAttacker.GetComponent<SimPlayer>() != null)
				{
					if (GameData.SimMngr.IsSimGrouped(GameData.SimMngr.Sims[alternateAttacker.GetComponent<SimPlayer>().myIndex]))
					{
						SimPlayerIndependentGroup simGroup = GameData.SimMngr.GetSimGroup(GameData.SimMngr.Sims[alternateAttacker.GetComponent<SimPlayer>().myIndex]);
						if (simGroup != null)
						{
							foreach (SimPlayerTracking member in simGroup.Members)
							{
								if (member.MyAvatar != null)
								{
									if (simGroup.Members.Count > 1)
									{
										member.MyAvatar.MyStats.CurrentExperience += Mathf.RoundToInt((float)xp / (float)simGroup.Members.Count - 1f);
									}
									else
									{
										member.MyAvatar.MyStats.CurrentExperience += xp;
									}
								}
							}
						}
					}
					else
					{
						alternateAttacker.GetComponent<SimPlayer>().MyStats.CurrentExperience += xp;
					}
				}
			}
			if (MyStats.Charmed)
			{
				BreakCharm();
			}
			GetComponent<NPC>().ResetSpawnPoint();
			if (GameData.SimPlayerGrouping.GroupTargets.Contains(this))
			{
				GameData.SimPlayerGrouping.GroupTargets.Remove(this);
			}
			LootTable component = GetComponent<LootTable>();
			if ((component != null && component.ActualDrops.Count > 0) || component.MyGold > 0)
			{
				savedCorpse = CorpseDataManager.AddCorpseData(new CorpseData(14400f, base.transform.position, SceneManager.GetActiveScene().name, component.ActualDrops, component.MyGold, GetComponent<NPC>()));
			}
			if (GetComponent<NPC>().InGroup)
			{
				GetComponent<NPC>().ExpediteRot(120f);
				UpdateSocialLog.LogAdd(base.transform.name + " tells the group: " + GameData.SimMngr.PersonalizeString("I'm down!", MyNPC.ThisSim), "#00B2B7");
				if (GetComponent<SimPlayer>() != null)
				{
					GetComponent<SimPlayer>().CurrentPullPhase = SimPlayer.PullPhases.NotPulling;
					if (MyNPC != null && MyNPC.ThisSim != null && GameData.SimMngr.Sims[MyNPC.ThisSim.myIndex] != null && GameData.SimMngr.Sims[MyNPC.ThisSim.myIndex].MyCurrentMemory != null)
					{
						GameData.SimMngr.Sims[MyNPC.ThisSim.myIndex].MyCurrentMemory.Died++;
					}
				}
				if (GetComponent<NavMeshAgent>() != null && GetComponent<NavMeshAgent>().isOnNavMesh)
				{
					GetComponent<NavMeshAgent>().isStopped = true;
				}
			}
			else if (GetComponent<SimPlayer>() != null)
			{
				GetComponent<SimPlayer>().SaveSim();
				GetComponent<NPC>().ExpediteRot(120f);
				GameData.SimMngr.RespawnSimAfterDeath(base.transform.GetComponent<NPC>().NPCName);
				GameData.SimMngr.ActiveSimInstances.Remove(GetComponent<SimPlayer>());
			}
		}
		else
		{
			GetComponent<PlayerCombat>().ForceAttackOff();
			if (GameData.GroupMembers[0] != null || GameData.GroupMembers[1] != null || GameData.GroupMembers[2] != null || GameData.GroupMembers[3] != null)
			{
				GameData.Misc.RevWindow.SetActive(value: true);
			}
			Relax = true;
			GameData.PlayerAud.PlayOneShot(GameData.Misc.PlayerDieSound, GameData.PlayerAud.volume * GameData.SFXVol * GameData.MasterVol);
			GetComponent<Fishing>().resetFishing();
			if (MyCharmedNPC != null)
			{
				ClearCharmedNPC();
			}
		}
		if (ShoutOnDeath.Count > 0)
		{
			UpdateSocialLog.LogAdd(ShoutOnDeath[Random.Range(0, ShoutOnDeath.Count)], "#FF9000");
		}
	}

	public void DamageShieldTaken(int _dmg, Stats _giver)
	{
		if (!isNPC && _giver.Myself.isNPC && _giver.Myself.MyNPC.SimPlayer)
		{
			return;
		}
		bool involvesPlayer = false;
		if (!_giver.Myself.isNPC || !isNPC)
		{
			involvesPlayer = true;
		}
		if (_giver.Charmed || (_giver.Myself.isNPC && _giver.Myself.MyNPC.ThisSim != null && _giver.Myself.MyNPC.ThisSim.InGroup) || !_giver.Myself.isNPC)
		{
			DmgFromPlayerSource += _dmg;
		}
		if (MyStats.ReduceHP(_dmg, GameData.DamageType.Magic, involvesPlayer, _isCritical: false) && isNPC)
		{
			base.gameObject.layer = 7;
		}
		if (_dmg <= 0)
		{
			return;
		}
		if (!isNPC)
		{
			UpdateSocialLog.CombatLogAdd("YOU have taken " + _dmg + " from " + _giver.Myself.MyNPC.NPCName + "'s damage shield!", "red");
		}
		else if (_giver.Myself.isNPC)
		{
			if (Vector3.Distance(GameData.PlayerControl.transform.position, base.transform.position) < 15f)
			{
				UpdateSocialLog.CombatLogAdd(MyNPC.NPCName + " has taken " + _dmg + " from " + _giver.Myself.MyNPC.NPCName + "'s damage shield!", "grey");
			}
		}
		else
		{
			UpdateSocialLog.CombatLogAdd(MyNPC.NPCName + " has taken " + _dmg + " from your damage shield!");
		}
	}

	public int SelfDamageMe(float _incDmg)
	{
		int num = Mathf.RoundToInt((float)MyStats.CurrentMaxHP * (_incDmg / 100f));
		if (Alive && !Invulnerable)
		{
			MyStats.RecentDmg = 240f;
			MyStats.ReduceHP(num, GameData.DamageType.Physical, _involvesPlayer: true, _isCritical: false);
			if (!isNPC)
			{
				UpdateSocialLog.CombatLogAdd("You take " + num + " damage from your combat stance", "red");
			}
		}
		else
		{
			num = 0;
		}
		return num;
	}

	public int SelfDamageMeFlat(int _incDmg)
	{
		if (Alive && !Invulnerable)
		{
			MyStats.RecentDmg = 240f;
			MyStats.ReduceHP(_incDmg, GameData.DamageType.Physical, _involvesPlayer: true, _isCritical: false);
			return _incDmg;
		}
		return 0;
	}

	public int BleedDamageMe(int _incdmg, bool _fromPlayer, Character _attacker)
	{
		int num = _incdmg;
		if (Alive && !Invulnerable && (!_fromPlayer || MyFaction != 0))
		{
			if (isNPC && MyNPC.MiningNode)
			{
				return -5;
			}
			if (isNPC && MyNPC.TreasureChest)
			{
				return -6;
			}
			if (isNPC && _attacker != null)
			{
				MyNPC.ManageAggro(num, _attacker);
				if (_attacker.Master != null)
				{
					MyNPC.ManageAggro(Mathf.RoundToInt((float)num / 4f), _attacker.Master);
				}
			}
			if (_attacker != null && _attacker.transform.name == "Player")
			{
				MyStats.RecentDmgByPlayer = 120f;
			}
			MyStats.RecentDmg = 240f;
			if (num == -1 && _attacker != null)
			{
				num = 1;
				MyNPC.CurrentAggroTarget = _attacker;
				MyNPC.ManageAggro(200, _attacker);
			}
			if (MyStats.Charmed && Master != null && _attacker != null && _attacker == Master)
			{
				BreakCharm();
			}
			if (num > 0 && _attacker != null)
			{
				LastHitBy = _attacker;
				MyStats.RemoveBreakableEffects();
				if (MyHurtSound != null && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 8f)
				{
					if (Random.Range(0, 10) > 7)
					{
						MyAudio.PlayOneShot(MyHurtSound, MyAudio.volume * 2f * GameData.CombatVol * GameData.MasterVol);
					}
					else
					{
						MyAudio.PlayOneShot(GameData.Misc.HitSoundsGeneric[Random.Range(0, GameData.Misc.HitSoundsGeneric.Count)], MyAudio.volume * 1.25f * GameData.CombatVol * GameData.MasterVol);
					}
				}
				else if (MyHurtSound == null && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 8f)
				{
					MyAudio.PlayOneShot(GameData.Misc.HitSoundsGeneric[Random.Range(0, GameData.Misc.HitSoundsGeneric.Count)], MyAudio.volume * 1.25f * GameData.CombatVol * GameData.MasterVol);
				}
			}
			if (_fromPlayer || (_attacker != null && _attacker.GetComponent<SimPlayer>() != null && _attacker.GetComponent<SimPlayer>().InGroup))
			{
				DmgFromPlayerSource += num;
				if (isNPC)
				{
					GetComponent<NPC>().AggroOn(_attacker);
				}
			}
			else if (_attacker != null)
			{
				alternateAttacker = _attacker.transform;
			}
			if (MyCharmedNPC != null && MyCharmedNPC.GetCurrentTarget() == null && _attacker != null)
			{
				MyCharmedNPC.AggroOn(_attacker);
			}
			bool involvesPlayer = false;
			if (!isNPC || (_attacker != null && !_attacker.isNPC))
			{
				involvesPlayer = true;
			}
			if (!MyStats.ReduceHP(num - MyStats.SpellShield, GameData.DamageType.Physical, involvesPlayer, _isCritical: false))
			{
				if (num > 0 && !NoFlinch)
				{
					MyAnim.SetTrigger("Hurt");
				}
			}
			else if (isNPC)
			{
				base.gameObject.layer = 7;
			}
			return num;
		}
		if (_fromPlayer && MyFaction == Faction.Player)
		{
			return -3;
		}
		return -1;
	}

	public int DamageMe(int _incdmg, bool _fromPlayer, GameData.DamageType _dmgType, Character _attacker, bool _animEffect, bool _criticalHit, int _bonusDmg = 0)
	{
		bool isCritical = _criticalHit;
		int num = _incdmg;
		int num2 = 0;
		float num3 = MyStats.CombatStance?.AggroGenMod ?? 1f;
		if (MyStats.CurrentHP < MyStats.CurrentMaxHP / 2)
		{
			num3 *= 1.25f;
		}
		if (MyStats.CurrentHP < MyStats.CurrentMaxHP / 4)
		{
			num3 *= 1.75f;
		}
		if (Alive && !Invulnerable && (!_fromPlayer || MyFaction != 0))
		{
			if (isNPC && MyNPC.MiningNode)
			{
				return -5;
			}
			if (isNPC && MyNPC.TreasureChest)
			{
				return -6;
			}
			if (isNPC && _attacker != null)
			{
				MyNPC.ManageAggro(Mathf.RoundToInt((float)num * num3), _attacker);
				if (_attacker.Master != null)
				{
					MyNPC.ManageAggro(Mathf.RoundToInt((float)num * num3 / 4f), _attacker.Master);
				}
			}
			if (_attacker != null && _attacker.transform.name == "Player")
			{
				MyStats.RecentDmgByPlayer = 120f;
			}
			if (_attacker != null && !isNPC && GameData.PlayerControl.CurrentTarget == null)
			{
				GameData.PlayerControl.CurrentTarget = _attacker;
				GameData.PlayerControl.CurrentTarget.TargetMe();
			}
			MyStats.RecentDmg = 240f;
			if (_dmgType == GameData.DamageType.Physical && num != -1)
			{
				num = MyStats.MitigatePhysical(num, _attacker.MyStats.AttackAbility, _attacker.MyStats);
				if (_attacker.isNPC && !_attacker.MyNPC.SimPlayer && (float)num > (float)(_attacker.MyStats.Level * 15) * GameData.ServerDMGMod && !_attacker.MyNPC.NoDmgCap)
				{
					num = Mathf.RoundToInt((float)(_attacker.MyStats.Level * 15) * GameData.ServerDMGMod + (float)Random.Range(-_attacker.MyStats.Level, _attacker.MyStats.Level));
				}
			}
			if (num == -1)
			{
				num = 1;
				MyNPC.CurrentAggroTarget = _attacker;
				MyNPC.ManageAggro(Mathf.RoundToInt(200f * num3), _attacker);
			}
			if (MyStats.Invisible)
			{
				MyStats.BreakEffectsOnAction();
			}
			if (MyStats.Charmed && Master != null && _attacker == Master)
			{
				BreakCharm();
			}
			if (num > 0)
			{
				num += _bonusDmg;
				LastHitBy = _attacker;
				MyStats.RemoveBreakableEffects();
				if (MyHurtSound != null && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 8f)
				{
					if (Random.Range(0, 10) > 7)
					{
						MyAudio.PlayOneShot(MyHurtSound, MyAudio.volume * 4f * GameData.CombatVol * GameData.MasterVol);
					}
					else
					{
						MyAudio.PlayOneShot(GameData.Misc.HitSoundsGeneric[Random.Range(0, GameData.Misc.HitSoundsGeneric.Count)], MyAudio.volume * 1.25f * GameData.CombatVol * GameData.MasterVol);
					}
				}
				else if (MyHurtSound == null && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 8f)
				{
					MyAudio.PlayOneShot(GameData.Misc.HitSoundsGeneric[Random.Range(0, GameData.Misc.HitSoundsGeneric.Count)], MyAudio.volume * 1.25f * GameData.CombatVol * GameData.MasterVol);
				}
				HitFX.GetComponent<ParticleSystem>().Play();
			}
			if (_fromPlayer || (_attacker != null && _attacker.GetComponent<SimPlayer>() != null && _attacker.GetComponent<SimPlayer>().InGroup))
			{
				DmgFromPlayerSource += num;
				if (isNPC)
				{
					GetComponent<NPC>().AggroOn(_attacker);
				}
			}
			if (isNPC && MyNPC.SimPlayer)
			{
				if (GameData.SimMngr.IsSimGrouped(GameData.SimMngr.Sims[MyNPC.ThisSim.myIndex]) && _attacker != null)
				{
					GameData.SimMngr.GetSimGroup(GameData.SimMngr.Sims[MyNPC.ThisSim.myIndex]).CallForAssist(_attacker);
				}
			}
			else if (_attacker != null)
			{
				alternateAttacker = _attacker.transform;
			}
			if (MyCharmedNPC != null && MyCharmedNPC.GetCurrentTarget() == null && _attacker != null)
			{
				MyCharmedNPC.AggroOn(_attacker);
			}
			if ((!isNPC || (isNPC && MyNPC.ThisSim != null && MyNPC.ThisSim.InGroup && MyNPC.ThisSim.CurrentPullPhase == SimPlayer.PullPhases.NotPulling)) && _attacker != null && _attacker.Alive)
			{
				SimPlayerTracking[] groupMembers = GameData.GroupMembers;
				foreach (SimPlayerTracking simPlayerTracking in groupMembers)
				{
					if (simPlayerTracking != null && simPlayerTracking.MyAvatar != null && (bool)_attacker && simPlayerTracking.MyAvatar.GetThisNPC().GetChar().Alive)
					{
						if (Vector3.Distance(simPlayerTracking.MyAvatar.transform.position, _attacker.transform.position) < 30f && !simPlayerTracking.MyAvatar.GetThisNPC().GetChar().NearbyEnemies.Contains(_attacker))
						{
							simPlayerTracking.MyAvatar.GetThisNPC().GetChar().NearbyEnemies.Add(_attacker);
						}
						if (simPlayerTracking.MyAvatar.GetThisNPC().CurrentAggroTarget == null && !simPlayerTracking.MyAvatar.GuardSpot)
						{
							GameData.SimPlayerGrouping.GroupAttack(_attacker);
						}
					}
				}
			}
			else if (GetComponent<SimPlayer>() != null)
			{
				if (Random.Range(0, 10) > 8 && GetComponent<NPC>().InGroup)
				{
					GameData.SimPlayerGrouping.AddStringForDisplay(GetComponent<NPC>().NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString(GetComponent<SimPlayer>().MyDialog.GetAggro(), MyNPC.ThisSim), "#00B2B7");
				}
				if (GameData.SimMngr.Sims[GetComponent<SimPlayer>().myIndex].isPuller)
				{
					_ = MyNPC.CurrentAggroTarget == null;
				}
			}
			if (MyStats.SpellShield <= num)
			{
				num2 = num - MyStats.SpellShield;
				bool involvesPlayer = false;
				if (!isNPC || (_attacker != null && !_attacker.isNPC))
				{
					involvesPlayer = true;
				}
				if (!MyStats.ReduceHP(num2, _dmgType, involvesPlayer, isCritical))
				{
					if (num2 > 0 && _animEffect && !NoFlinch)
					{
						if (isNPC)
						{
							MyAnim.SetTrigger("Hurt");
						}
						else if (Random.Range(0, 10) > 5)
						{
							MyAnim.SetTrigger("Hurt");
						}
					}
				}
				else if (isNPC)
				{
					base.gameObject.layer = 7;
				}
				if (MyStats.SpellShield > 0)
				{
					MyStats.SpellShield -= num;
					MyAudio.PlayOneShot(GameData.GM.GetComponent<Misc>().ShieldAbsorb, MyAudio.volume * GameData.SFXVol * GameData.MasterVol);
					if (MyStats.SpellShield <= 0)
					{
						MyStats.SpellShield = 0;
					}
				}
				if (num2 > 0 && _attacker != null && _attacker.MyStats.PercentLifesteal > 0f)
				{
					_attacker.MyStats.HealMe(Mathf.RoundToInt((float)num2 * (_attacker.MyStats.PercentLifesteal / 100f)));
				}
				if ((bool)_attacker && num > 0)
				{
					Stats myStats = _attacker.MyStats;
					if ((object)myStats == null || !myStats.Charmed)
					{
						_attacker.CreditDPS(num2);
					}
					else
					{
						_attacker.Master?.CreditDPS(num2);
					}
				}
				return num2;
			}
			MyStats.SpellShield -= num;
			if (MyStats.SpellShield <= 0)
			{
				MyStats.SpellShield = 0;
			}
			MyAudio.PlayOneShot(GameData.GM.GetComponent<Misc>().ShieldAbsorb, MyAudio.volume * GameData.SFXVol * GameData.MasterVol);
			num2 = 0;
			return -2;
		}
		if (_fromPlayer && MyFaction == Faction.Player)
		{
			return -3;
		}
		return -1;
	}

	public int MagicDamageMe(int _dmg, bool _fromPlayer, GameData.DamageType _dmgType, Character _attacker, float resistMod, int _baseDmg)
	{
		float num = MyStats.CombatStance?.AggroGenMod ?? 1f;
		if (MyStats.CurrentHP < MyStats.CurrentMaxHP / 2)
		{
			num *= 1.25f;
		}
		if (MyStats.CurrentHP < MyStats.CurrentMaxHP / 4)
		{
			num *= 1.75f;
		}
		if (Alive)
		{
			MyStats.RecentDmg = 240f;
		}
		if (_attacker != null && _attacker.transform.name == "Player")
		{
			MyStats.RecentDmgByPlayer = 120f;
		}
		float num2 = 0f;
		if (_fromPlayer)
		{
			if (MyStats.Charmed && Master != null && _attacker == Master)
			{
				if (!MyNPC.SummonedByPlayer)
				{
					BreakCharm();
				}
				return -1;
			}
			if ((MyFaction == Faction.Player && isNPC) || (MyFaction == Faction.PC && _attacker.GetComponent<SimPlayer>() != null))
			{
				return -1;
			}
			if (isNPC && MyNPC.MiningNode)
			{
				return -5;
			}
			if (isNPC && MyNPC.TreasureChest)
			{
				return -6;
			}
		}
		else
		{
			if (_attacker.Master != null && _attacker.Master == GameData.PlayerControl.Myself && ((MyFaction == Faction.Player && isNPC) || MyFaction == Faction.PC))
			{
				return -1;
			}
			if (_attacker.isNPC && !_attacker.MyStats.Charmed && _attacker.MyFaction == MyFaction)
			{
				return -1;
			}
		}
		if (Invulnerable)
		{
			return -1;
		}
		num2 = CheckResistAmount(resistMod, _attacker.MyStats.Level, _dmgType);
		if (num2 >= 1f)
		{
			return 0;
		}
		if (_dmg > 0)
		{
			MyStats.RemoveBreakableEffects();
			if (MyStats.Invisible)
			{
				MyStats.BreakEffectsOnAction();
			}
			LastHitBy = _attacker;
		}
		_dmg = Mathf.RoundToInt((float)_dmg - (float)_dmg * num2 * MyStats.CombatStance.DamageTakenMod);
		if (MyHurtSound != null && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 8f)
		{
			if (Random.Range(0, 10) > 7)
			{
				MyAudio.PlayOneShot(MyHurtSound, MyAudio.volume * 2f * GameData.CombatVol * GameData.MasterVol);
			}
			else
			{
				MyAudio.PlayOneShot(GameData.Misc.HitSoundsGeneric[Random.Range(0, GameData.Misc.HitSoundsGeneric.Count)], MyAudio.volume * 1.25f * GameData.CombatVol * GameData.MasterVol);
			}
		}
		else if (MyHurtSound == null && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 8f)
		{
			MyAudio.PlayOneShot(GameData.Misc.HitSoundsGeneric[Random.Range(0, GameData.Misc.HitSoundsGeneric.Count)], MyAudio.volume * 1.25f * GameData.CombatVol * GameData.MasterVol);
		}
		if (isNPC && _attacker != null)
		{
			MyNPC.ManageAggro(Mathf.RoundToInt((float)_dmg * num), _attacker);
			if (_attacker.Master != null)
			{
				MyNPC.ManageAggro(Mathf.RoundToInt((float)Mathf.RoundToInt((float)_dmg * num) / 2f), _attacker.Master);
			}
		}
		bool involvesPlayer = false;
		if (!isNPC || (_attacker != null && !_attacker.isNPC))
		{
			involvesPlayer = true;
		}
		if (_dmg > _baseDmg * 15)
		{
			_dmg = _baseDmg * 15;
		}
		if (!MyStats.ReduceHP(_dmg, _dmgType, involvesPlayer, _isCritical: false))
		{
			if (_dmg > 0 && !NoFlinch)
			{
				MyAnim.SetTrigger("Hurt");
			}
		}
		else if (isNPC)
		{
			base.gameObject.layer = 7;
		}
		if (_fromPlayer)
		{
			if (MyFaction == Faction.Player && isNPC)
			{
				return -1;
			}
			DmgFromPlayerSource += _dmg;
			if (isNPC && _attacker != null)
			{
				GetComponent<NPC>().AggroOn(_attacker);
			}
		}
		else
		{
			alternateAttacker = _attacker.transform;
		}
		if ((bool)_attacker && _dmg > 0)
		{
			Stats myStats = _attacker.MyStats;
			if ((object)myStats != null && !myStats.Charmed)
			{
				_attacker.CreditDPS(_dmg);
			}
			else
			{
				_attacker.Master?.CreditDPS(_dmg);
			}
		}
		return _dmg;
	}

	public float GetRawResist(GameData.DamageType _dmgType)
	{
		int num = 0;
		return _dmgType switch
		{
			GameData.DamageType.Magic => MyStats.GetCurrentMR(), 
			GameData.DamageType.Elemental => MyStats.GetCurrentER(), 
			GameData.DamageType.Void => MyStats.GetCurrentVR(), 
			GameData.DamageType.Poison => MyStats.GetCurrentPR(), 
			_ => MyStats.GetCurrentMR(), 
		};
	}

	public float CheckResistAmount(float _resistMod, int attackerLevel, GameData.DamageType _dmgType)
	{
		float num = _dmgType switch
		{
			GameData.DamageType.Magic => MyStats.GetCurrentMR(), 
			GameData.DamageType.Elemental => MyStats.GetCurrentER(), 
			GameData.DamageType.Void => MyStats.GetCurrentVR(), 
			GameData.DamageType.Poison => MyStats.GetCurrentPR(), 
			_ => MyStats.GetCurrentMR(), 
		} - _resistMod;
		return num * 0.01f;
	}

	public void FootR()
	{
		if (Footsteps.Length != 0 && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 5f)
		{
			MyAudio.PlayOneShot(Footsteps[Random.Range(0, Footsteps.Length)], 0.1f * GameData.FootVol * GameData.MasterVol);
			_ = isNPC;
		}
	}

	public void FootLScuff()
	{
		if (Footsteps.Length != 0 && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 5f)
		{
			MyAudio.PlayOneShot(Footsteps[Random.Range(0, Footsteps.Length)], 0.03f * GameData.FootVol * GameData.MasterVol);
			_ = isNPC;
		}
	}

	public void FootRScuff()
	{
		if (Footsteps.Length != 0 && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 5f)
		{
			MyAudio.PlayOneShot(Footsteps[Random.Range(0, Footsteps.Length)], 0.03f * GameData.FootVol * GameData.MasterVol);
			_ = isNPC;
		}
	}

	public void FootL()
	{
		if (Footsteps.Length != 0 && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 5f)
		{
			MyAudio.PlayOneShot(Footsteps[Random.Range(0, Footsteps.Length)], 0.1f * GameData.FootVol * GameData.MasterVol);
			_ = isNPC;
		}
	}

	private void LoadGenericWorldFaction()
	{
		if (MyFaction == Faction.GoodHuman || MyFaction == Faction.GoodGuard || MyFaction == Faction.OtherGood || MyFaction == Faction.PreyAnimal || MyFaction == Faction.Villager || MyFaction == Faction.Unseen || MyFaction == Faction.DEBUG)
		{
			MyWorldFaction = Resources.Load("Factions/Generic Good") as WorldFaction;
		}
		else
		{
			MyWorldFaction = Resources.Load("Factions/Generic Evil") as WorldFaction;
		}
	}

	public void Hit()
	{
	}

	public void Land()
	{
	}

	public void Shoot()
	{
	}

	public Character CharmMe(Character _master)
	{
		if (Master == null)
		{
			TempFaction = _master.BaseFaction;
			MyStats.Charmed = true;
			Master = _master;
			if (MyNPC != null)
			{
				Master.MyCharmedNPC = MyNPC;
				MyNPC.AggroTable.Clear();
			}
			HoldForCharm.Clear();
			if (AggressiveTowards.Count > 0)
			{
				foreach (Faction aggressiveToward in AggressiveTowards)
				{
					HoldForCharm.Add(aggressiveToward);
				}
			}
			removedHate = true;
			AggressiveTowards.Clear();
			Allies.Add(_master.MyFaction);
			if (NearbyEnemies.Count > 0)
			{
				foreach (Character nearbyEnemy in NearbyEnemies)
				{
					if (nearbyEnemy.NearbyEnemies.Contains(this))
					{
						nearbyEnemy.NearbyEnemies.Remove(this);
					}
				}
			}
			if (NearbyFriends.Count > 0)
			{
				foreach (Character nearbyFriend in NearbyFriends)
				{
					if (nearbyFriend.NearbyFriends.Contains(this))
					{
						nearbyFriend.NearbyFriends.Remove(this);
					}
				}
			}
			MyNPC.CurrentAggroTarget = null;
			NearbyFriends.Clear();
			NearbyEnemies.Clear();
			return this;
		}
		return null;
	}

	public void BreakCharm()
	{
		if (Master != null)
		{
			if (removedHate)
			{
				AggressiveTowards.Add(Master.MyFaction);
				Allies.Remove(Master.MyFaction);
			}
			if (isNPC)
			{
				MyNPC.CurrentAggroTarget = Master;
				MyNPC.AggroOn(Master);
			}
			Master.ClearCharmedNPC();
		}
		MyStats.Charmed = false;
		MyFaction = BaseFaction;
		if (HoldForCharm.Count > 0)
		{
			foreach (Faction item in HoldForCharm)
			{
				AggressiveTowards.Add(item);
			}
		}
		int num = 0;
		StatusEffect[] statusEffects = MyStats.StatusEffects;
		foreach (StatusEffect statusEffect in statusEffects)
		{
			if (statusEffect.Effect != null && statusEffect.Effect.CharmTarget)
			{
				statusEffect.Effect = null;
				statusEffect.fromPlayer = false;
				statusEffect.Duration = 0f;
			}
			num++;
		}
		if (NearbyFriends.Count > 0)
		{
			foreach (Character nearbyFriend in NearbyFriends)
			{
				if (nearbyFriend.NearbyFriends.Contains(this))
				{
					nearbyFriend.NearbyFriends.Remove(this);
				}
			}
		}
		if (NearbyEnemies.Count > 0)
		{
			foreach (Character nearbyEnemy in NearbyEnemies)
			{
				if (nearbyEnemy.NearbyFriends.Contains(this))
				{
					nearbyEnemy.NearbyFriends.Remove(this);
					if (nearbyEnemy.MyNPC != null && nearbyEnemy.MyNPC.CurrentAggroTarget == null)
					{
						nearbyEnemy.MyNPC.AggroOn(this);
					}
				}
			}
		}
		NearbyFriends.Clear();
		NearbyEnemies.Clear();
		MyStats.Charmed = false;
		Master = null;
		if (MyNPC != null && MyNPC.SummonedByPlayer)
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void ClearCharmedNPC()
	{
		MyCharmedNPC.GetChar().Invulnerable = false;
		MyCharmedNPC.GetChar().Master = null;
		MyCharmedNPC = null;
	}

	public bool IsMezzed()
	{
		StatusEffect[] statusEffects = MyStats.StatusEffects;
		foreach (StatusEffect statusEffect in statusEffects)
		{
			if (statusEffect.Effect != null && statusEffect.Effect.CrowdControlSpell)
			{
				return true;
			}
		}
		return false;
	}

	public int EnvironmentalDamageMe(int _dmg)
	{
		MyStats.RemoveBreakableEffects();
		if (MyHurtSound != null && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 8f)
		{
			if (Random.Range(0, 10) > 7)
			{
				MyAudio.PlayOneShot(MyHurtSound, MyAudio.volume * 2f * GameData.CombatVol * GameData.MasterVol);
			}
			else
			{
				MyAudio.PlayOneShot(GameData.Misc.HitSoundsGeneric[Random.Range(0, GameData.Misc.HitSoundsGeneric.Count)], MyAudio.volume * GameData.CombatVol * GameData.MasterVol);
			}
		}
		else if (MyHurtSound == null && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 8f)
		{
			MyAudio.PlayOneShot(GameData.Misc.HitSoundsGeneric[Random.Range(0, GameData.Misc.HitSoundsGeneric.Count)], MyAudio.volume * GameData.CombatVol * GameData.MasterVol);
		}
		if (Invulnerable || !Alive)
		{
			return -1;
		}
		bool involvesPlayer = !isNPC;
		if (!MyStats.ReduceHP(_dmg, GameData.DamageType.Physical, involvesPlayer, _isCritical: false))
		{
			if (_dmg > 0 && !NoFlinch)
			{
				MyAnim.SetTrigger("Hurt");
			}
		}
		else if (isNPC)
		{
			base.gameObject.layer = 7;
		}
		return _dmg;
	}

	public void FlagForFactionHit(bool _bool)
	{
		damagedByPlayerEver = _bool;
	}

	private float NewMRCheck(float attackerLevel, float _resistMod)
	{
		float num = 5f * (float)MyStats.Level;
		float num2 = 6f * attackerLevel;
		float num3 = 0.59f;
		float num4 = 0.71f;
		float num5 = 0.91f;
		float num6 = 0.02f * ((float)MyStats.GetCurrentMR() - num) + num3 * ((float)MyStats.Level - attackerLevel) - num4 - num5 * (_resistMod / num2);
		return 1f / (1f + Mathf.Exp(0f - num6));
	}

	public int GetCurHealthAsIntPercentage()
	{
		if (MyStats.CurrentMaxHP == 0)
		{
			return 0;
		}
		return Mathf.RoundToInt((float)MyStats.CurrentHP / (float)MyStats.CurrentMaxHP * 100f);
	}
}
