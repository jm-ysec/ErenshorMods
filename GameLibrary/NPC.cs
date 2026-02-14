// NPC
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Character))]
[RequireComponent(typeof(LootTable))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Stats))]
[RequireComponent(typeof(Inventory))]
[RequireComponent(typeof(CastSpell))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(UseSkill))]
public class NPC : MonoBehaviour
{
	public enum NPCAttackType
	{
		Hit,
		Slash,
		Stab,
		Bash,
		Claw,
		Bite,
		Crush
	}

	private int npc;

	private int player;

	private int AggroMask;

	public Collider[] InRange;

	public bool SimPlayer;

	public bool InGroup;

	private Character Myself;

	private NavMeshAgent MyNav;

	private Animator MyAnim;

	private bool castingSpell;

	public bool inMeleeRange;

	private Stats MyStats;

	public Character CurrentAggroTarget;

	public float TickTime = 20f;

	private float navTick = 5f;

	private Vector3 HomePos;

	private Transform patrolPoint;

	private List<Transform> PatrolPath = new List<Transform>();

	private SpawnPoint MySpawnPoint;

	private CastSpell MySpells;

	public string NPCName;

	public string SpawnEmote;

	public List<Material> Colorvariations;

	private float wander;

	public float HailTimer;

	private int CurrentPatrolPoint;

	private bool loopPatrol;

	private float updatePatrolDel;

	private int patrolDir = 1;

	public string AggroMsg = "";

	public string AggroEmote = "";

	public int BaseAtkDmg = 5;

	public Vector2 DamageRange = new Vector3(1f, 1f);

	public int HardSetAC;

	public int OHAtkDmg;

	public int MinAtkDmg;

	public List<Spell> MyBuffSpells;

	public List<Spell> MyAttackSpells;

	public List<Spell> MyHealSpells;

	public List<Spell> MyCCSpells;

	public List<Spell> MyTauntSpell;

	public Spell AETaunt;

	public List<Spell> GroupHeals;

	public List<Skill> MyAttackSkills;

	public Spell MyPetSpell;

	public Spell MyHOTSpell;

	public Transform NamePlate;

	private float atkSpellDelay = 120f;

	private float rotTimer = 14400f;

	private AnimatorOverrideController AnimOverride;

	public AnimationClip TwoHandSwordIdle;

	public AnimationClip TwoHandStaffIdle;

	public AnimationClip TwoHandStaffWalk;

	public AnimationClip TwoHandStaffRun;

	public AnimationClip TwoHandSwordWalk;

	public AnimationClip TwoHandSwordRun;

	public AnimationClip ArmedIdle;

	public AnimationClip RelaxedIdle;

	public AnimationClip Run;

	public AnimationClip Walk;

	public bool TwoHandSword;

	public bool TwoHandStaff;

	public bool DualWield;

	public bool OneHand;

	public string GuildName = "";

	private bool armedIdleAnim;

	public bool SummonedByPlayer;

	private float healCD;

	public bool noClick;

	public SimPlayer ThisSim;

	public bool GroupEncounter;

	public bool retreat;

	public float LeashRange;

	private bool Leashing;

	public bool TakingEnvironmentalDamage;

	private float takingEnvDmg;

	public bool AggroRegardlessOfLevel;

	public bool AggroRegardlessofLOS;

	private float AllHKCD;

	private int errorCount;

	public bool HandSetResistances;

	private float checkPathDelay = 30f;

	public bool NoDmgCap;

	public float DamageMult = 1f;

	public float ArmorPenMult = 1f;

	public bool CanPhantomStrike;

	public float Enrage;

	private float EnrageReset;

	private bool enraging;

	public int PowerAttackBaseDmg;

	public float PowerAttackFreq;

	private bool warnForIncomingAtk = true;

	private float powerAttackTimer;

	public Spell SpawnWithStatus;

	private float enrageTick = 60f;

	private float HoldDamageMult;

	private float stuckTime;

	public ParticleSystem AttackParticles;

	public NPCAttackType AttackType;

	public Spell NPCProcOnHit;

	public float NPCProcOnHitChance;

	public Transform AggroArea;

	private bool offNav;

	private float offNavTolerance = 25f;

	public bool Mobile = true;

	public bool MiningNode;

	public bool TreasureChest;

	public int ChestDurability;

	public TreasureChestEvent MyChestEvent;

	public List<AggroSlot> AggroTable = new List<AggroSlot>();

	public FlashUIColors NameFlash;

	private List<Character> Combatants = new List<Character>();

	private float spamCD = 100f;

	public float HealTolerance = 0.7f;

	public IEnumerator navDo;

	public IEnumerator behDo;

	private bool isNavRunning;

	private bool isBehRunning;

	private float spawnCD = 100f;

	private float TauntCD;

	private float UnstunnableWarnCD;

	private float targetReminderCD;

	public ParticleSystem SpecialFXParticles;

	private UseSkill MySkills;

	[SerializeField]
	public List<Spell> MemmedHealSpells = new List<Spell>();

	private float spawnEmoteDelay;

	public string SetAchievementOnDefeat;

	public string SetAchievementOnSpawn;

	public bool DoNotLeaveCorpse;

	private bool MHWand;

	private bool OHWand;

	private bool MHBow;

	private bool WandEquipped;

	private bool BowEquipped;

	private SpriteRenderer MapArrow;

	private bool questToAssign;

	private bool MarkerUp;

	private float NPCDamageScalingBonus = 1f;

	private float navMeshCheckCD = 10f;

	private List<GameObject> trickShotTarg = new List<GameObject>();

	private int roundsSinceTarg;

	public float PlannedChantEnd = 1f;

	public TextMeshPro NamePlateTxt;

	public float NPCSpellCooldown = 120f;

	private GameObject QuestMarker;

	private QuestManager MyQuests;

	private NPCDialogManager MyDialog;

	private float tauntCD;

	public bool NeverAggro;

	private Transform FearDestination;

	private bool nameShowsInvis;

	public float HoldDPS = 100f;

	private bool InPosition;

	private const float MinSpeedSq = 0.01f;

	private const float StuckSeconds = 3f;

	private const float WarmupSeconds = 0.25f;

	private const float UnstickCooldown = 1f;

	private const float ArriveBuffer = 0.05f;

	private float pathAcquireTime;

	private float lastUnstickTime;

	private bool prevHasPath;

	private Vector3 lastDest;

	private void Start()
	{
		GuildName = "";
		NPCSpellCooldown = Random.Range(20, 360);
		powerAttackTimer = PowerAttackFreq;
		HoldDamageMult = DamageMult;
		EnrageReset = Enrage;
		npc = 1 << LayerMask.NameToLayer("NPC");
		player = 1 << LayerMask.NameToLayer("Player");
		AggroMask = npc | player;
		if (TreasureChest)
		{
			ChestDurability = Random.Range(3, 7);
			MyChestEvent = GetComponent<TreasureChestEvent>();
		}
		if (GetComponent<SimPlayer>() != null)
		{
			ThisSim = GetComponent<SimPlayer>();
			SimPlayer = true;
		}
		if (GetComponent<UseSkill>() != null)
		{
			MySkills = GetComponent<UseSkill>();
		}
		TickTime += Random.Range(5, 60);
		Myself = GetComponent<Character>();
		MyNav = GetComponent<NavMeshAgent>();
		MyNav.angularSpeed = 720f;
		if (ThisSim == null)
		{
			MyNav.acceleration = 999f;
		}
		MyAnim = GetComponent<Animator>();
		MyStats = GetComponent<Stats>();
		MySpells = GetComponent<CastSpell>();
		HomePos = base.transform.position;
		MyStats.RunSpeed *= GameData.RunSpeedMod;
		MyStats.actualRunSpeed = MyStats.RunSpeed;
		MyAnim.SetFloat("RunSpeedModifier", Mathf.Clamp(GameData.RunSpeedMod, 0.7f, 1.3f));
		MyNav.speed = MyStats.RunSpeed;
		base.transform.name = NPCName;
		if (MyAnim != null && MyAnim.runtimeAnimatorController != null)
		{
			MyAnim.SetBool("1HSmall", value: true);
		}
		if (!SimPlayer && !GroupEncounter)
		{
			if (Random.Range(0, 10) > 7)
			{
				MyStats.Level--;
			}
			else if (Random.Range(0, 10) > 7)
			{
				MyStats.Level++;
			}
		}
		if (MyStats.Level < 1)
		{
			MyStats.Level = 1;
		}
		if (!SimPlayer)
		{
			MyQuests = GetComponent<QuestManager>();
			MyDialog = GetComponent<NPCDialogManager>();
			BaseAtkDmg = Mathf.RoundToInt((float)BaseAtkDmg * GameData.GM.DamageBalanceFactor);
			if (BaseAtkDmg <= 0)
			{
				BaseAtkDmg = 1;
			}
			OHAtkDmg = BaseAtkDmg;
			MyStats.BaseHP = Mathf.RoundToInt((float)MyStats.BaseHP * GameData.ServerHPMod);
			MyStats.BaseHP = Mathf.RoundToInt((float)MyStats.BaseHP * GameData.HPScale);
			if (MyStats.BaseHP <= 0)
			{
				MyStats.BaseHP = 1;
			}
			MyStats.CurrentMaxHP = MyStats.BaseHP;
			MyStats.CurrentHP = MyStats.CurrentMaxHP;
		}
		else if (ThisSim.IsFullyLoaded)
		{
			GetComponent<Stats>().CalcSimStats();
		}
		Myself.MyNPC = this;
		MyStats.MyName = NPCName;
		if (Colorvariations.Count > 0)
		{
			GetComponentInChildren<SkinnedMeshRenderer>().material = Colorvariations[Random.Range(0, Colorvariations.Count)];
		}
		NamePlate = Object.Instantiate(GameData.GM.GetComponent<Misc>().NamePlate, base.transform.position, base.transform.rotation).transform;
		NamePlateTxt = NamePlate.GetComponent<TextMeshPro>();
		NamePlate.position = base.transform.position + new Vector3(0f, GetComponent<CapsuleCollider>().height * Mathf.Max(base.transform.localScale.y, 1f) + 0.3f, 0f);
		NamePlate.GetComponent<TextMeshPro>().text = NPCName;
		if (GetComponent<VendorInventory>() != null)
		{
			TextMeshPro component = NamePlate.GetComponent<TextMeshPro>();
			component.text = component.text + "\n<" + GetComponent<VendorInventory>().VendorDesc + " Merchant>";
		}
		if (NPCName == "Validus Greencent" || NPCName == "Prestigio Valusha" || NPCName == "Comstock Retalio" || NPCName == "Wealthen Giallara")
		{
			NamePlate.GetComponent<TextMeshPro>().text += "\n<Banker>";
		}
		if (NPCName == "Thella Steepleton" || NPCName == "Goldie Retalio")
		{
			NamePlate.GetComponent<TextMeshPro>().text += "\n<Auction Broker>";
		}
		if (SimPlayer)
		{
			UpdateNamePlate();
		}
		NamePlate.transform.SetParent(base.transform);
		if (NamePlate != null)
		{
			NameFlash = NamePlate.GetComponent<FlashUIColors>();
		}
		AnimOverride = new AnimatorOverrideController(MyAnim.runtimeAnimatorController);
		if (MyAnim.runtimeAnimatorController != null)
		{
			MyAnim.runtimeAnimatorController = AnimOverride;
		}
		UpdateAnims();
		if (!string.IsNullOrEmpty(SpawnEmote))
		{
			spawnEmoteDelay = 100f;
		}
		if (Myself.MyAudio != null)
		{
			Myself.MyAudio.volume = 0.2f;
		}
		if (MyStats.Level > 12)
		{
			Myself.MySkills.KnownSkills.Add(GameData.SkillDatabase.GetSkillByName("Double Attack"));
		}
		if (MyStats.Level > 18)
		{
			Myself.MySkills.KnownSkills.Add(GameData.SkillDatabase.GetSkillByName("Dodge"));
		}
		if (MyStats.Level > 13)
		{
			Myself.MySkills.KnownSkills.Add(GameData.SkillDatabase.GetSkillByName("Block"));
		}
		navDo = NavUpdate(0.3f);
		StartCoroutine(navDo);
		behDo = BehaviorUpdate(0.1f);
		StartCoroutine(behDo);
		if (base.transform.localScale.x > 1f)
		{
			MyNav.radius = 0.08f;
		}
		else
		{
			MyNav.radius = 0.25f;
		}
		if (MyBuffSpells.Count > 0 && MyBuffSpells.Contains(GameData.SpellDatabase.GetSpellByID("11784192")))
		{
			MyBuffSpells.Remove(GameData.SpellDatabase.GetSpellByID("11784192"));
		}
		if (MyStats.CharacterClass == GameData.ClassDB.Druid || MyStats.CharacterClass == GameData.ClassDB.Paladin)
		{
			List<Spell> list = (from spell in MyHealSpells
				where !spell.GroupEffect
				orderby spell.HP descending
				select spell).Take(3).ToList();
			Spell spell2 = (from spell in MyHealSpells
				where spell.GroupEffect
				orderby spell.HP descending
				select spell).FirstOrDefault();
			if (spell2 != null && !list.Contains(spell2))
			{
				if (list.Count < 3)
				{
					list.Add(spell2);
				}
				else
				{
					list[list.Count - 1] = spell2;
				}
			}
			MemmedHealSpells = list.OrderByDescending((Spell spell) => spell.HP).ToList();
		}
		else
		{
			MemmedHealSpells = MyHealSpells;
		}
		if (MyStats.CharacterClass == GameData.ClassDB.Druid)
		{
			foreach (Spell myHealSpell in MyHealSpells)
			{
				if (myHealSpell.PercentManaRestoration > 0)
				{
					MemmedHealSpells.Add(myHealSpell);
				}
			}
		}
		if (!string.IsNullOrEmpty(SetAchievementOnSpawn))
		{
			SetAchievement.Unlock(SetAchievementOnSpawn);
		}
		if (MyQuests != null && MyQuests.NPCQuests.Count > 0)
		{
			foreach (Quest nPCQuest in MyQuests.NPCQuests)
			{
				if (nPCQuest != null && !GameData.HasQuest.Contains(nPCQuest.DBName) && !GameData.CompletedQuests.Contains(nPCQuest.DBName))
				{
					SpawnQuestMarker();
				}
			}
		}
		if (SpawnWithStatus != null)
		{
			MyStats.AddStatusEffect(SpawnWithStatus, _fromPlayer: false, 0);
		}
		if (!SimPlayer)
		{
			CheckForBow();
		}
		SpawnMapArrow();
		if (Myself.BossXp > 0f)
		{
			NoDmgCap = true;
		}
	}

	public void UpdateNamePlate()
	{
		GuildName = "";
		if (ThisSim == null)
		{
			return;
		}
		if (GameData.SimMngr.Sims[ThisSim.myIndex]?.GuildID != "")
		{
			GuildName = GameData.GuildManager.GetGuildNameByID(GameData.SimMngr.Sims[ThisSim.myIndex].GuildID);
		}
		if (!(NamePlate == null))
		{
			if (ThisSim.IsGMCharacter)
			{
				NamePlate.GetComponent<TextMeshPro>().color = Color.white;
				NamePlate.GetComponent<FlashUIColors>().col1 = Color.white;
			}
			else
			{
				NamePlate.GetComponent<TextMeshPro>().color = Color.green;
				NamePlate.GetComponent<FlashUIColors>().col1 = Color.green;
			}
			string text = NPCName;
			if (MyStats.Invisible)
			{
				text = "(" + NPCName + ")";
			}
			NamePlate.GetComponent<TextMeshPro>().text = text;
			if (GuildName != "")
			{
				TextMeshPro component = NamePlate.GetComponent<TextMeshPro>();
				component.text = component.text + "\n<" + GuildName + ">";
			}
		}
	}

	public void SpawnQuestMarker()
	{
		if (GameData.UseMarkers && !MarkerUp)
		{
			GameObject gameObject = Object.Instantiate(GameData.Misc.QuestIndicator, base.transform.position, base.transform.rotation);
			gameObject.transform.parent = base.transform;
			gameObject.transform.position = gameObject.transform.position + new Vector3(0f, GetComponent<CapsuleCollider>().height * base.transform.localScale.y + 0.8f, 0f);
			MarkerUp = true;
			QuestMarker = gameObject;
		}
	}

	private void QuestMarkerListener()
	{
		if (QuestMarker == null || !QuestMarker.activeSelf)
		{
			return;
		}
		bool flag = true;
		if (MyDialog != null && MyDialog.MyDialogOptions.Length != 0)
		{
			NPCDialog[] myDialogOptions = MyDialog.MyDialogOptions;
			foreach (NPCDialog nPCDialog in myDialogOptions)
			{
				if ((bool)nPCDialog.QuestToAssign && nPCDialog.QuestToAssign != null && !GameData.HasQuest.Contains(nPCDialog.QuestToAssign.DBName) && !GameData.CompletedQuests.Contains(nPCDialog.QuestToAssign.DBName))
				{
					flag = false;
					break;
				}
			}
		}
		if (MyQuests != null && MyQuests.NPCQuests.Count > 0)
		{
			foreach (Quest nPCQuest in MyQuests.NPCQuests)
			{
				if (nPCQuest != null && (!GameData.CompletedQuests.Contains(nPCQuest.DBName) || nPCQuest.repeatable))
				{
					flag = false;
					break;
				}
			}
		}
		if (flag && QuestMarker.activeSelf)
		{
			QuestMarker.SetActive(value: false);
			MarkerUp = false;
		}
	}

	private void SpawnMapArrow()
	{
		GameObject gameObject = Object.Instantiate(GameData.Misc.NPCMapArrow, base.transform.position + Vector3.up, GameData.Misc.NPCMapArrow.transform.rotation);
		gameObject.transform.eulerAngles += new Vector3(0f, -90f, 0f);
		gameObject.transform.parent = base.transform;
		MapArrow = gameObject.GetComponent<SpriteRenderer>();
		if (Myself.AggressiveTowards.Contains(Character.Faction.Player))
		{
			MapArrow.color = Color.red;
		}
		else if ((Myself?.MyWorldFaction?.FactionValue).GetValueOrDefault() < 0f)
		{
			MapArrow.color = Color.red;
		}
		else if (SimPlayer)
		{
			MapArrow.color = Color.cyan;
		}
		else if (GetComponent<QuestManager>() != null && GameData.UseMarkers)
		{
			MapArrow.color = Color.yellow;
		}
		else if (GetComponent<VendorInventory>() != null && GameData.UseMarkers)
		{
			MapArrow.color = Color.green;
		}
		else
		{
			MapArrow.color = Color.white;
		}
	}

	public void Revive()
	{
		StartCoroutine(navDo);
		StartCoroutine(behDo);
	}

	public void Die()
	{
		StopCoroutine(navDo);
		StopCoroutine(behDo);
	}

	public void UpdateMemmedHeals()
	{
		if (MemmedHealSpells == null)
		{
			MemmedHealSpells = new List<Spell>();
		}
		else
		{
			MemmedHealSpells.Clear();
		}
		if (MyStats.CharacterClass == GameData.ClassDB.Druid || MyStats.CharacterClass == GameData.ClassDB.Paladin)
		{
			List<Spell> list = (from spell in MyHealSpells
				where !spell.GroupEffect
				orderby spell.HP descending
				select spell).Take(3).ToList();
			Spell spell2 = (from spell in MyHealSpells
				where spell.GroupEffect
				orderby spell.HP descending
				select spell).FirstOrDefault();
			if (spell2 != null && !list.Contains(spell2))
			{
				if (list.Count < 3)
				{
					list.Add(spell2);
				}
				else
				{
					list[list.Count - 1] = spell2;
				}
			}
			MemmedHealSpells = list.OrderByDescending((Spell spell) => spell.HP).ToList();
		}
		else
		{
			MemmedHealSpells = MyHealSpells;
		}
		if (!(MyStats.CharacterClass == GameData.ClassDB.Druid))
		{
			return;
		}
		foreach (Spell myHealSpell in MyHealSpells)
		{
			if (myHealSpell.PercentManaRestoration > 0)
			{
				MemmedHealSpells.Add(myHealSpell);
			}
		}
	}

	private void CheckForWand()
	{
		MHWand = (MyStats?.MyInv?.SimMH?.MyItem?.IsWand).GetValueOrDefault();
		OHWand = (MyStats?.MyInv?.SimOH?.MyItem?.IsWand).GetValueOrDefault();
		if (MHWand || OHWand)
		{
			WandEquipped = true;
		}
	}

	private void CheckForBow()
	{
		if (SimPlayer)
		{
			MHBow = (MyStats?.MyInv?.SimMH?.MyItem?.IsBow).GetValueOrDefault();
		}
		if (!SimPlayer)
		{
			MHBow = MyStats.MyInv.PrimaryBow;
		}
		if (MHBow)
		{
			BowEquipped = true;
		}
	}

	private void CheckAttackRange()
	{
		if (MHBow)
		{
			Myself.AttackRange = MyStats?.MyInv?.SimMH?.MyItem?.BowRange ?? 4;
		}
		else if (MHWand)
		{
			Myself.AttackRange = MyStats?.MyInv?.SimMH?.MyItem?.WandRange ?? 4;
		}
		else
		{
			Myself.AttackRange = 4f;
		}
	}

	private void DoPowerAttack()
	{
		warnForIncomingAtk = true;
		powerAttackTimer = PowerAttackFreq;
		PerformMeleeHitPreCalc(PowerAttackBaseDmg, isOffhand: false);
		string myName = CurrentAggroTarget.transform.name;
		if (myName == "Player")
		{
			myName = GameData.PlayerStats.MyName;
		}
		if (Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 15f)
		{
			UpdateSocialLog.LogAdd(base.transform.name + " unleashes a POWERFUL ATTACK on " + myName, "red");
		}
	}

	private void EnsureNav()
	{
		if (MyNav == null || !MyNav.enabled || MyNav.isOnOffMeshLink || Myself == null || Myself.MyStats == null)
		{
			return;
		}
		if (navMeshCheckCD > 0f)
		{
			navMeshCheckCD -= 60f * Time.deltaTime;
			return;
		}
		navMeshCheckCD = 10f;
		if (MyNav.isOnNavMesh || MyNav.isOnOffMeshLink)
		{
			return;
		}
		NavMeshHit hit2;
		if (NavMesh.SamplePosition(MyNav.transform.position, out var hit, 2f, -1))
		{
			MyNav.Warp(hit.position);
		}
		else if (NavMesh.SamplePosition(MyNav.transform.position, out hit2, 2f, -1))
		{
			MyNav.Warp(hit2.position);
		}
		else if (GameData.ZoneAnnounce != null)
		{
			if (NavMesh.SamplePosition(GameData.ZoneAnnounce.transform.position, out var hit3, 15f, -1))
			{
				MyNav.Warp(hit3.position);
				UpdateSocialLog.LogAdd("[DEBUG MSG] NPC warped to safe point @ x: " + hit3.position.x.ToString("F2") + ", y: " + hit3.position.y.ToString("F2") + ", z: " + hit3.position.z.ToString("F2") + " - OffNav detected", "yellow");
			}
			else if (SimPlayer)
			{
				MyNav.Warp(GameData.PlayerControl.transform.position);
				UpdateSocialLog.LogAdd("[DEBUG MSG] SimPlayer detected off NavMesh - warping to Player position", "yellow");
			}
			else
			{
				UpdateSocialLog.LogAdd("[DEBUG MSG] NPC Disabled: " + base.transform.name + " - no acceptable NavMesh detected within parameters -- You may need to leave the Zone to reset navigation and restore game performance", "yellow");
				Myself.MyStats.CurrentHP = -1;
				rotTimer = 1f;
			}
		}
	}

	private void CheckSpellEarlyEnd()
	{
		if (!(MyStats.CharacterClass != GameData.ClassDB.Arcanist) && MySpells.isCasting() && (!MySpells.isCasting() || MySpells.GetCurrentCast().Type == Spell.SpellType.Damage || MySpells.GetCurrentCast().Type == Spell.SpellType.AE || MySpells.GetCurrentCast().Type == Spell.SpellType.PBAE) && PlannedChantEnd < 1f && MySpells.GetCastProgress() > PlannedChantEnd)
		{
			MySpells.EndCastWithScaling();
			PlannedChantEnd = 1f;
		}
	}

	private void Update()
	{
		if (CurrentAggroTarget != null && !CurrentAggroTarget.gameObject.activeSelf)
		{
			CurrentAggroTarget = null;
		}
		if (NeverAggro && CurrentAggroTarget != null)
		{
			CurrentAggroTarget = null;
		}
		if (MyStats.Invisible && !nameShowsInvis)
		{
			UpdateNamePlate();
			nameShowsInvis = true;
		}
		if (!MyStats.Invisible && nameShowsInvis)
		{
			UpdateNamePlate();
			nameShowsInvis = false;
		}
		EnsureNav();
		if (SimPlayer)
		{
			CheckForWand();
			CheckForBow();
			CheckAttackRange();
		}
		else
		{
			if (MarkerUp)
			{
				QuestMarkerListener();
			}
			if (PowerAttackFreq > 0f && CurrentAggroTarget != null && !MyStats.Stunned && !MyStats.Feared && Myself.Alive && powerAttackTimer > 0f)
			{
				powerAttackTimer -= 60f * Time.deltaTime;
				if (powerAttackTimer < 300f && warnForIncomingAtk)
				{
					warnForIncomingAtk = false;
					if (Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 15f)
					{
						UpdateSocialLog.LogAdd(base.transform.name + " is preparing a POWERFUL ATTACK!", "red");
					}
				}
				if (powerAttackTimer <= 0f)
				{
					if (GetChar() != null && GetChar().TargetRing != null && Vector3.Distance(GetChar().TargetRing.transform.position, CurrentAggroTarget.transform.position) <= Myself.AttackRange + 3f)
					{
						DoPowerAttack();
					}
					else
					{
						warnForIncomingAtk = true;
						powerAttackTimer = PowerAttackFreq;
					}
				}
			}
			if (CurrentAggroTarget != null && !MyStats.Stunned && !MyStats.Feared && Myself.Alive && NPCSpellCooldown > 0f)
			{
				NPCSpellCooldown -= 60f * Time.deltaTime;
			}
			if (!Myself.NoRun && GameData.NPCFlee && CurrentAggroTarget != null && Myself.BossXp == 0f && LeashRange == 0f)
			{
				List<Character> nearbyFriends = Myself.NearbyFriends;
				if (nearbyFriends != null && nearbyFriends.Count < 1 && (float)MyStats.CurrentHP / (float)MyStats.CurrentMaxHP < 0.12f)
				{
					Character myself = Myself;
					if ((object)myself == null || myself.MyFaction != Character.Faction.Undead)
					{
						inMeleeRange = false;
						InPosition = false;
						MyStats.AddStatusEffect(GameData.Misc.Retreat, _fromPlayer: false, 1);
					}
				}
			}
		}
		if (GameData.UseMap && !SimPlayer)
		{
			if ((base.transform.position - GameData.PlayerControl.transform.position).sqrMagnitude < 10000f)
			{
				if (MapArrow != null && !MapArrow.enabled)
				{
					MapArrow.enabled = true;
				}
			}
			else if (MapArrow != null && MapArrow.enabled)
			{
				MapArrow.enabled = false;
			}
		}
		if (Myself != null && Myself.Alive && MapArrow != null && MapArrow.color != Color.red && MapArrow.color != Color.magenta && Myself.AggressiveTowards.Contains(Character.Faction.Player))
		{
			MapArrow.color = Color.red;
		}
		if (Myself != null && !Myself.Alive && MapArrow != null && MapArrow.color != Color.gray && Myself.AggressiveTowards.Contains(Character.Faction.Player))
		{
			MapArrow.color = Color.gray;
		}
		if (spawnEmoteDelay > 0f)
		{
			spawnEmoteDelay -= 60f * Time.deltaTime;
			if (spawnEmoteDelay <= 0f)
			{
				UpdateSocialLog.LogAdd(SpawnEmote, "#FF9000");
			}
		}
		if (checkPathDelay > 0f)
		{
			checkPathDelay -= 60f * Time.deltaTime;
		}
		if (tauntCD > 0f)
		{
			tauntCD -= 60f * Time.deltaTime;
		}
		if (spawnCD > 0f)
		{
			spawnCD -= 60f * Time.deltaTime;
		}
		if (UnstunnableWarnCD > 0f)
		{
			UnstunnableWarnCD -= 60f * Time.deltaTime;
		}
		if (targetReminderCD > 0f)
		{
			targetReminderCD -= 60f * Time.deltaTime;
		}
		if (AllHKCD > 0f)
		{
			AllHKCD -= 60f * Time.deltaTime;
		}
		Input.GetKey(KeyCode.F10);
		if (spamCD > 0f)
		{
			spamCD -= 60f * Time.deltaTime;
		}
		if (CurrentAggroTarget != null && Myself.Alive)
		{
			if (!NameFlash.flashing)
			{
				NameFlash.Flash(on: true);
			}
		}
		else
		{
			if (NameFlash.flashing)
			{
				NameFlash.Flash(on: false);
			}
			inMeleeRange = false;
		}
		_ = SimPlayer;
		if (offNav && IsAgentOnNavMesh(base.gameObject))
		{
			MyNav.enabled = true;
			offNavTolerance = 60f;
			offNav = false;
		}
		if (LeashRange > 0f && !Leashing && Vector3.Distance(base.transform.position, HomePos) > LeashRange)
		{
			Leashing = true;
		}
		if (Leashing && Vector3.Distance(base.transform.position, HomePos) < 5f)
		{
			AggroTable.Clear();
			CurrentAggroTarget = null;
			Leashing = false;
		}
		HandleEnrage();
		if (CurrentAggroTarget != null && !CurrentAggroTarget.Alive)
		{
			CurrentAggroTarget = null;
		}
		if (CurrentAggroTarget != null && Myself.Alive)
		{
			Character @char = GetChar();
			if ((object)@char == null || @char.MyFaction != Character.Faction.Mineral)
			{
				if (MyStats.RunSpeed > 0f || (MyStats.RunSpeed <= 0f && Vector3.Distance(CurrentAggroTarget.transform.position, base.transform.position) <= Myself.AttackRange))
				{
					CurrentAggroTarget.UnderThreat = 60f;
				}
				if (CurrentAggroTarget.isNPC && CurrentAggroTarget.MyNPC.SimPlayer && CurrentAggroTarget.MyNPC.ThisSim.InGroup)
				{
					if (Myself.Alive && !GameData.SimPlayerGrouping.GroupTargets.Contains(Myself))
					{
						GameData.SimPlayerGrouping.GroupTargets.Add(Myself);
					}
				}
				else if (CurrentAggroTarget == GameData.PlayerControl.Myself && (GameData.GroupMembers[0] != null || GameData.GroupMembers[1] != null || GameData.GroupMembers[2] != null || GameData.GroupMembers[3] != null) && Myself.Alive && !GameData.SimPlayerGrouping.GroupTargets.Contains(Myself))
				{
					GameData.SimPlayerGrouping.GroupTargets.Add(Myself);
				}
			}
		}
		if (Myself.Alive)
		{
			if (base.gameObject.layer == 7)
			{
				base.gameObject.layer = 9;
			}
			MonitorEnvDmg();
			_ = isNavRunning;
		}
		if (updatePatrolDel > 0f)
		{
			updatePatrolDel -= 60f * Time.deltaTime;
		}
		if (atkSpellDelay >= 0f)
		{
			atkSpellDelay -= 60f * Time.deltaTime;
		}
		if (HailTimer > 0f)
		{
			HailTimer -= 60f * Time.deltaTime;
		}
		if (!CheckLiving())
		{
			rotTimer -= 60f * Time.deltaTime;
			if (rotTimer <= 0f && (!SimPlayer || (SimPlayer && !ThisSim.InGroup && !GameData.SimMngr.IsSimGrouped(GameData.SimMngr.Sims[ThisSim.myIndex]))))
			{
				if (GameData.AttackingPlayer.Contains(this))
				{
					GameData.AttackingPlayer.Remove(this);
				}
				if (GameData.GroupMatesInCombat.Contains(this))
				{
					GameData.GroupMatesInCombat.Remove(this);
				}
				Object.Destroy(base.gameObject);
			}
		}
		if (healCD > 0f)
		{
			healCD -= 60f * Time.deltaTime;
		}
		if (CurrentAggroTarget != null && !CurrentAggroTarget.Alive)
		{
			if (Myself.NearbyEnemies.Contains(CurrentAggroTarget))
			{
				Myself.NearbyEnemies.Remove(CurrentAggroTarget);
			}
			CurrentAggroTarget = null;
			if (Myself.GetDPS() > 0 && Myself.contributedDPS <= 0f)
			{
				Myself.ResetRollingDPS();
			}
			CheckAggro();
		}
		if ((CurrentAggroTarget == null && MyStats.RecentDmg <= 0f) || !Myself.Alive)
		{
			Myself.Relax = true;
		}
		else if (Myself.Alive)
		{
			Myself.Relax = false;
		}
		if (CurrentAggroTarget == GameData.PlayerControl && Myself.Alive)
		{
			if (!GameData.PlayerControl.HuntingMe.Contains(this))
			{
				GameData.PlayerControl.HuntingMe.Add(this);
			}
		}
		else if (GameData.PlayerControl.HuntingMe.Contains(this))
		{
			GameData.PlayerControl.HuntingMe.Remove(this);
		}
		if (!Myself.Alive && SimPlayer && GameData.GroupMatesInCombat.Contains(this))
		{
			GameData.GroupMatesInCombat.Remove(this);
		}
		if (!SimPlayer && Myself.Alive && CurrentAggroTarget != null && Myself.NearbyFriends.Count > 0)
		{
			Myself.NearbyFriends.RemoveAll((Character f) => f == null || !f.Alive);
		}
		HandleNameTag();
	}

	private void HandleEnrage()
	{
		if (!Myself.Alive)
		{
			return;
		}
		if (MyStats.RecentDmg <= 0f && CurrentAggroTarget == null)
		{
			enraging = false;
			Enrage = EnrageReset;
			enrageTick = 60f;
			if (HoldDamageMult != DamageMult)
			{
				DamageMult = HoldDamageMult;
			}
			return;
		}
		if (Enrage > 0f && CurrentAggroTarget != null)
		{
			Enrage -= 60f * Time.deltaTime;
			if (Enrage <= 0f && !enraging)
			{
				enraging = true;
				if (Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 30f)
				{
					UpdateSocialLog.LogAdd(base.transform.name + " becomes ENRAGED!", "red");
				}
			}
		}
		if (!enraging || !(enrageTick > 0f))
		{
			return;
		}
		enrageTick -= 60f * Time.deltaTime;
		if (enrageTick <= 0f)
		{
			DamageRange.x *= 1.1f;
			DamageRange.y *= 1.1f;
			enrageTick = 600f;
			if (Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 30f)
			{
				UpdateSocialLog.LogAdd(base.transform.name + "'s rage grows...", "red");
			}
		}
	}

	public void SetMeleeRange(bool _set)
	{
		inMeleeRange = _set;
	}

	public void ExpediteRot()
	{
		rotTimer = 5f;
	}

	public void ExpediteRot(float _time)
	{
		noClick = true;
		rotTimer = _time;
	}

	public void InitNewNPC(SpawnPoint _sp, float _wander)
	{
		MySpawnPoint = _sp;
		wander = _wander;
		loopPatrol = MySpawnPoint.LoopPatrol;
	}

	public void InitNewNPC(SpawnPoint _sp, List<Transform> _patrol)
	{
		MySpawnPoint = _sp;
		PatrolPath = _patrol;
		loopPatrol = MySpawnPoint.LoopPatrol;
	}

	public void InitNewNPC(List<Transform> Patrol)
	{
		PatrolPath = Patrol;
		loopPatrol = true;
	}

	private bool IsAgentOnNavMesh(GameObject agentObject)
	{
		Vector3 position = agentObject.transform.position;
		if (NavMesh.SamplePosition(position, out var hit, 1f, -1) && Mathf.Approximately(position.x, hit.position.x) && Mathf.Approximately(position.z, hit.position.z))
		{
			return position.y >= hit.position.y - 0.5f;
		}
		return false;
	}

	public void CastInvisOnPlayer()
	{
		if (ThisSim == null)
		{
			return;
		}
		if (ThisSim.InvisSpell == null)
		{
			UpdateSocialLog.LogAdd(base.transform.name + " says: " + GameData.SimMngr.PersonalizeString("I don't have a way to cast that on you.", ThisSim));
			return;
		}
		if (Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) > 15f)
		{
			GameData.SimMngr.LoadResponse("[WHISPER FROM] " + base.transform.name + ": " + GameData.SimMngr.PersonalizeString("You're too far away, get closer to me.", ThisSim), base.transform.name);
		}
		if (!MySpells.isCasting())
		{
			MyNav.speed = 0f;
			MyNav.velocity = Vector3.zero;
			if (MyNav.isOnNavMesh)
			{
				MyNav.isStopped = true;
			}
			MyAnim.SetBool("Walking", value: false);
			MyAnim.SetBool("Patrol", value: false);
			MySpells.StartSpell(ThisSim.InvisSpell, GameData.PlayerStats);
		}
		else
		{
			UpdateSocialLog.LogAdd(base.transform.name + " says: " + GameData.SimMngr.PersonalizeString("One second, I'm casting something else. Ask when I'm done if you still need that invis.", ThisSim));
		}
	}

	private void CheckBuffs()
	{
		if (MySpells.isCasting() || MyStats.Invisible || MyBuffSpells.Count <= 0)
		{
			return;
		}
		foreach (Spell myBuffSpell in MyBuffSpells)
		{
			if ((SimPlayer && (ThisSim.GetSpellCooldownByIndex(ThisSim.GetSpellIndexInBook(myBuffSpell)) > 0f || (myBuffSpell.ForHardEncounters && (CurrentAggroTarget == null || CurrentAggroTarget.MyStats?.Level < MyStats.Level + 2)) || myBuffSpell.HardcodedUseCase)) || myBuffSpell.Type != Spell.SpellType.Beneficial)
			{
				continue;
			}
			if (!MyStats.CheckForSEByName(myBuffSpell.SpellName) && !MyStats.CheckForHigherSEByLine(myBuffSpell))
			{
				if (MyNav.isOnNavMesh)
				{
					MyNav.isStopped = true;
				}
				MyAnim.SetBool("Walking", value: false);
				MyAnim.SetBool("Patrol", value: false);
				MySpells.StartSpell(myBuffSpell, MyStats);
				if (SimPlayer)
				{
					ThisSim.SetSpellCooldownBySpell(myBuffSpell, myBuffSpell.Cooldown + myBuffSpell.SpellChargeTime / 60f);
				}
				break;
			}
			if (MySpells.isCasting() || Myself.NearbyFriends.Count <= 0 || (myBuffSpell.SelfOnly && !myBuffSpell.GroupEffect))
			{
				continue;
			}
			if (!MyStats.Charmed)
			{
				foreach (Character nearbyFriend in Myself.NearbyFriends)
				{
					if (!(nearbyFriend != null) || !nearbyFriend.Alive || !(nearbyFriend.MyStats != null) || nearbyFriend.MyStats.CheckForStatus(myBuffSpell) || nearbyFriend.MyStats.CheckForHigherSEByLine(myBuffSpell) || MySpells.isCasting())
					{
						continue;
					}
					if (MyStats.GetCurrentMana() > myBuffSpell.ManaCost && Vector3.Distance(base.transform.position, nearbyFriend.transform.position) < myBuffSpell.SpellRange)
					{
						if (MyNav.isOnNavMesh)
						{
							MyNav.isStopped = true;
						}
						MyAnim.SetBool("Walking", value: false);
						MyAnim.SetBool("Patrol", value: false);
						MySpells.StartSpell(myBuffSpell, nearbyFriend.MyStats);
						if (SimPlayer)
						{
							ThisSim.SetSpellCooldownBySpell(myBuffSpell, myBuffSpell.Cooldown + myBuffSpell.SpellChargeTime / 60f);
						}
						if (InGroup)
						{
							UpdateSocialLog.LogAdd(NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString(myBuffSpell.SpellName + " incoming on " + nearbyFriend.MyStats.MyName, ThisSim), "#00B2B7");
						}
					}
					break;
				}
				continue;
			}
			bool flag = true;
			StatusEffect[] statusEffects = GameData.PlayerStats.StatusEffects;
			for (int i = 0; i < statusEffects.Length; i++)
			{
				if (statusEffects[i].Effect == myBuffSpell)
				{
					flag = false;
				}
			}
			if (flag && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 10f && !MySpells.isCasting())
			{
				if (MyNav.isOnNavMesh)
				{
					MyNav.isStopped = true;
				}
				MyAnim.SetBool("Walking", value: false);
				MyAnim.SetBool("Patrol", value: false);
				MySpells.StartSpell(myBuffSpell, GameData.PlayerStats);
				if (SimPlayer)
				{
					ThisSim.SetSpellCooldownBySpell(myBuffSpell, myBuffSpell.Cooldown + myBuffSpell.SpellChargeTime / 60f);
				}
				if (InGroup)
				{
					UpdateSocialLog.LogAdd(NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString(myBuffSpell.SpellName + " incoming on " + GameData.PlayerStats.MyName, ThisSim), "#00B2B7");
				}
			}
			break;
		}
	}

	private int GetHealThresh(int _healer, int targetIndex)
	{
		if (targetIndex == -1)
		{
			return GameData.SimPlayerGrouping.HealCalls[_healer, 0];
		}
		if (GameData.GroupMembers.Length != 0 && targetIndex != -1)
		{
			for (int i = 0; i < GameData.GroupMembers.Length; i++)
			{
				if (GameData.GroupMembers[i] != null && GameData.GroupMembers[i].simIndex == targetIndex)
				{
					return GameData.SimPlayerGrouping.HealCalls[_healer, i + 1];
				}
			}
		}
		return 0;
	}

	private int FindHealerIndexInGroup()
	{
		int result = 0;
		if (GameData.GroupMembers.Length != 0)
		{
			for (int i = 0; i < GameData.GroupMembers.Length; i++)
			{
				if (GameData.GroupMembers[i] != null && GameData.GroupMembers[i].simIndex == ThisSim.myIndex)
				{
					result = i;
				}
			}
		}
		return result;
	}

	private void CheckHeals()
	{
		Character myself = Myself;
		if (((object)myself != null && !myself.Alive) || MyStats.Invisible)
		{
			return;
		}
		SimPlayerTracking simPlayerTracking = null;
		SimPlayerTracking simPlayerTracking2 = null;
		float num = 0f;
		int healer = -1;
		int num2 = 0;
		if (ThisSim != null && GameData.SimMngr.Sims.Count > ThisSim.myIndex)
		{
			simPlayerTracking = GameData.SimMngr.Sims[ThisSim.myIndex];
		}
		if (simPlayerTracking != null && GameData.GroupMembers.Contains(simPlayerTracking))
		{
			num = ThisSim.SkillLevel;
			healer = FindHealerIndexInGroup();
		}
		Stats stats = null;
		if (simPlayerTracking != null && GameData.GroupMembers.Contains(simPlayerTracking))
		{
			if (CheckManaSpell())
			{
				return;
			}
			for (int i = 0; i < GameData.GroupMembers.Length; i++)
			{
				if (GameData.GroupMembers[i] == null)
				{
					continue;
				}
				Stats myStats = GameData.GroupMembers[i].MyStats;
				if (myStats == null || myStats.CurrentHP <= 0)
				{
					continue;
				}
				int healThresh = GetHealThresh(healer, GameData.GroupMembers[i].simIndex);
				if (healThresh != 0 && (float)myStats.CurrentHP < (float)myStats.CurrentMaxHP * (float)healThresh / 100f)
				{
					if (stats == null || (GameData.SimPlayerGrouping.MainTank != null && stats != GameData.SimPlayerGrouping.MainTank.MyStats))
					{
						stats = myStats;
					}
					simPlayerTracking2 = GameData.GroupMembers[i];
					num2++;
				}
			}
			if (stats == null && (float)GameData.PlayerStats.CurrentHP < (float)GameData.PlayerStats.CurrentMaxHP * ((float)GetHealThresh(healer, -1) / 100f) && GameData.PlayerStats.CurrentHP > 0)
			{
				if (stats == null || (GameData.SimPlayerGrouping.MainTank != null && stats != GameData.SimPlayerGrouping.MainTank.MyStats))
				{
					stats = GameData.PlayerStats;
				}
				num2++;
			}
			if (stats != null)
			{
				int num3 = stats.CurrentMaxHP - stats.CurrentHP;
				Spell spell = null;
				Spell spell2 = null;
				Spell spell3 = null;
				foreach (Spell memmedHealSpell in MemmedHealSpells)
				{
					if (!SimPlayer)
					{
						continue;
					}
					int spellIndexInBook = ThisSim.GetSpellIndexInBook(memmedHealSpell);
					if (spellIndexInBook >= 0 && !(ThisSim.GetSpellCooldownByIndex(spellIndexInBook) > 0f) && memmedHealSpell.ManaCost <= MyStats.CurrentMana && !memmedHealSpell.SelfOnly)
					{
						if (spell2 == null)
						{
							spell2 = memmedHealSpell;
						}
						else if (memmedHealSpell.TargetHealing > spell2.TargetHealing)
						{
							spell2 = memmedHealSpell;
						}
					}
				}
				foreach (Spell memmedHealSpell2 in MemmedHealSpells)
				{
					if (!SimPlayer)
					{
						continue;
					}
					int spellIndexInBook2 = ThisSim.GetSpellIndexInBook(memmedHealSpell2);
					if (spellIndexInBook2 >= 0 && !(ThisSim.GetSpellCooldownByIndex(spellIndexInBook2) > 0f) && memmedHealSpell2.ManaCost <= MyStats.CurrentMana && !memmedHealSpell2.SelfOnly)
					{
						if (spell == null)
						{
							spell = memmedHealSpell2;
						}
						if (memmedHealSpell2.TargetHealing > num3 && memmedHealSpell2.TargetHealing < spell.TargetHealing)
						{
							spell = memmedHealSpell2;
						}
					}
				}
				if ((GameData.SimPlayerGrouping.MainTank != null && stats == GameData.SimPlayerGrouping.MainTank.MyStats) || (GameData.SimPlayerGrouping.PlayerIsTank && stats == GameData.PlayerStats))
				{
					foreach (Spell groupHeal in GroupHeals)
					{
						if (!SimPlayer)
						{
							continue;
						}
						int spellIndexInBook3 = ThisSim.GetSpellIndexInBook(groupHeal);
						if (spellIndexInBook3 >= 0 && !(ThisSim.GetSpellCooldownByIndex(spellIndexInBook3) > 0f) && groupHeal.ManaCost <= MyStats.CurrentMana && !groupHeal.SelfOnly)
						{
							if (spell3 == null)
							{
								spell3 = groupHeal;
							}
							else if (groupHeal.TargetHealing > spell3.TargetHealing)
							{
								spell3 = groupHeal;
							}
						}
					}
					if (spell3 != null && (float)num3 <= (float)spell3.TargetHealing * 2.5f && num2 > 1)
					{
						spell = spell3;
					}
				}
				else if (stats != null)
				{
					foreach (Spell groupHeal2 in GroupHeals)
					{
						if (!SimPlayer)
						{
							continue;
						}
						int spellIndexInBook4 = ThisSim.GetSpellIndexInBook(groupHeal2);
						if (spellIndexInBook4 >= 0 && !(ThisSim.GetSpellCooldownByIndex(spellIndexInBook4) > 0f) && groupHeal2.ManaCost <= MyStats.CurrentMana)
						{
							if (spell3 == null)
							{
								spell3 = groupHeal2;
							}
							else if (groupHeal2.TargetHealing > spell3.TargetHealing)
							{
								spell3 = groupHeal2;
							}
						}
					}
					if (spell3 != null && num2 > 1)
					{
						spell = spell3;
					}
				}
				if (spell == null)
				{
					spell = spell2;
				}
				if (spell != null)
				{
					Spell spell4 = null;
					if (simPlayerTracking2 != null)
					{
						spell4 = ((GameData.SimPlayerGrouping.MainTank == null || !(GameData.SimPlayerGrouping.MainTank.SimName == simPlayerTracking2.SimName)) ? spell : spell);
					}
					else if (stats != null && !stats.Myself.isNPC)
					{
						spell4 = ((!GameData.SimPlayerGrouping.PlayerIsTank || !(stats == GameData.PlayerStats)) ? spell : spell);
					}
					if (spell4 != null)
					{
						if (MySpells.isCasting() && MySpells.GetCurrentCast() != null && MySpells.GetCurrentCast().Type != Spell.SpellType.Heal)
						{
							MySpells.InterruptCast();
						}
						if (!MySpells.isCasting() && !retreat)
						{
							if (InGroup)
							{
								string myName = stats.transform.name;
								if (myName == "Player")
								{
									myName = GameData.PlayerStats.MyName;
								}
								UpdateSocialLog.LogAdd(NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString("Casting " + spell4.SpellName.ToUpper() + " on " + myName, ThisSim), "#00B2B7");
							}
							if (MyNav.isOnNavMesh)
							{
								MyNav.isStopped = true;
							}
							MyAnim.SetBool("Walking", value: false);
							MyAnim.SetBool("Patrol", value: false);
							MySpells.StartSpell(spell4, stats);
							if (stats.RecentDmg > 0f)
							{
								MyStats.RecentDmg = 240f;
							}
							if (SimPlayer)
							{
								ThisSim.SetSpellCooldownBySpell(spell4, spell4.Cooldown + spell4.SpellChargeTime / 60f);
							}
							if (MyStats.CharacterClass == GameData.ClassDB.Druid)
							{
								healCD = 140f - num;
							}
							else
							{
								healCD = spell4.Cooldown * 60f;
							}
							return;
						}
					}
				}
			}
			if (SimPlayer && InGroup && MyHOTSpell != null && MyStats.CurrentMana > MyHOTSpell.ManaCost && !MySpells.isCasting() && !retreat)
			{
				NPC nPC = GameData.SimPlayerGrouping?.MainTank?.MyAvatar?.GetThisNPC();
				if (nPC != null && nPC.CurrentAggroTarget != null && nPC.Myself.Alive)
				{
					if (!nPC.MyStats.CheckForSEByName(MyHOTSpell.StatusEffectToApply.SpellName) && !nPC.MyStats.CheckForHigherSEByLine(MyHOTSpell) && nPC.CurrentAggroTarget.MyStats.Level >= nPC.MyStats.Level - 6)
					{
						if (MyNav.isOnNavMesh)
						{
							MyNav.isStopped = true;
						}
						MyAnim.SetBool("Walking", value: false);
						MyAnim.SetBool("Patrol", value: false);
						MySpells.StartSpell(MyHOTSpell, nPC.MyStats);
						if (SimPlayer)
						{
							ThisSim.SetSpellCooldownBySpell(MyHOTSpell, MyHOTSpell.Cooldown + MyHOTSpell.SpellChargeTime / 60f);
						}
						if (InGroup)
						{
							UpdateSocialLog.LogAdd(NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString("HOT INCOMING on " + MyHOTSpell.SpellName.ToUpper() + " on " + nPC.transform.name, ThisSim), "#00B2B7");
						}
						if (MyStats.CharacterClass == GameData.ClassDB.Druid)
						{
							healCD = 140f - num;
						}
						else
						{
							healCD = MyHOTSpell.Cooldown * 60f;
						}
						if (SimPlayer)
						{
							ThisSim.SetSpellCooldownBySpell(MyHOTSpell, MyHOTSpell.Cooldown + MyHOTSpell.SpellChargeTime / 60f);
						}
						return;
					}
				}
				else if (nPC == null && GameData.SimPlayerGrouping.PlayerIsTank && GameData.PlayerControl.CurrentTarget != null && GameData.PlayerControl.CurrentTarget.MyNPC != null && GameData.PlayerControl.Myself.Alive && !GameData.PlayerStats.CheckForSEByName(MyHOTSpell.StatusEffectToApply.SpellName) && !GameData.PlayerStats.CheckForHigherSEByLine(MyHOTSpell) && GameData.PlayerControl.CurrentTarget.MyStats.Level >= GameData.PlayerStats.Level - 6 && GameData.PlayerControl.CurrentTarget.MyNPC.CurrentAggroTarget == GameData.PlayerControl.Myself)
				{
					if (MyNav.isOnNavMesh)
					{
						MyNav.isStopped = true;
					}
					MyAnim.SetBool("Walking", value: false);
					MyAnim.SetBool("Patrol", value: false);
					MySpells.StartSpell(MyHOTSpell, GameData.PlayerStats);
					if (SimPlayer)
					{
						ThisSim.SetSpellCooldownBySpell(MyHOTSpell, MyHOTSpell.Cooldown);
					}
					if (InGroup)
					{
						UpdateSocialLog.LogAdd(NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString("HOT INCOMING: " + MyHOTSpell.SpellName.ToUpper() + " on " + GameData.PlayerStats.MyName, ThisSim), "#00B2B7");
					}
					if (MyStats.CharacterClass == GameData.ClassDB.Druid)
					{
						healCD = 140f - num;
					}
					else
					{
						healCD = MyHOTSpell.Cooldown * 60f;
					}
					if (SimPlayer)
					{
						ThisSim.SetSpellCooldownBySpell(MyHOTSpell, MyHOTSpell.Cooldown + MyHOTSpell.SpellChargeTime / 60f);
					}
					return;
				}
			}
		}
		if (MySpells.isCasting() || MyHealSpells.Count <= 0 || retreat || (SimPlayer && (!SimPlayer || !(ThisSim != null) || ThisSim.MySimTracking == null || GameData.GroupMembers.Contains(ThisSim.MySimTracking))))
		{
			return;
		}
		foreach (Spell memmedHealSpell3 in MemmedHealSpells)
		{
			if ((SimPlayer && ThisSim.GetSpellCooldownByIndex(ThisSim.GetSpellIndexInBook(memmedHealSpell3)) > 0f) || memmedHealSpell3.MyDamageType != 0 || MyStats.CurrentMana <= memmedHealSpell3.ManaCost)
			{
				continue;
			}
			if ((float)MyStats.CurrentHP < (float)MyStats.CurrentMaxHP * 0.66f)
			{
				if (MyNav.isOnNavMesh)
				{
					MyNav.isStopped = true;
				}
				MyAnim.SetBool("Walking", value: false);
				MyAnim.SetBool("Patrol", value: false);
				MySpells.StartSpell(memmedHealSpell3, MyStats);
				if (SimPlayer)
				{
					ThisSim.SetSpellCooldownBySpell(memmedHealSpell3, memmedHealSpell3.Cooldown + memmedHealSpell3.SpellChargeTime / 60f);
				}
				if (MyStats.CharacterClass == GameData.ClassDB.Druid)
				{
					healCD = 140f - num;
				}
				else
				{
					healCD = memmedHealSpell3.Cooldown * 60f;
				}
				return;
			}
			if (InGroup && (float)GameData.PlayerStats.CurrentHP < (float)GameData.PlayerStats.CurrentMaxHP * 0.66f && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 60f && GameData.PlayerControl.Myself.Alive)
			{
				if (MyNav.isOnNavMesh)
				{
					MyNav.isStopped = true;
				}
				MyAnim.SetBool("Walking", value: false);
				MyAnim.SetBool("Patrol", value: false);
				MySpells.StartSpell(memmedHealSpell3, GameData.PlayerStats);
				if (SimPlayer)
				{
					ThisSim.SetSpellCooldownBySpell(memmedHealSpell3, memmedHealSpell3.Cooldown + memmedHealSpell3.SpellChargeTime / 60f);
				}
				if (InGroup)
				{
					UpdateSocialLog.LogAdd(NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString("Casting " + memmedHealSpell3.SpellName.ToUpper() + " on " + GameData.PlayerStats.MyName, ThisSim), "#00B2B7");
				}
				if (MyStats.CharacterClass == GameData.ClassDB.Druid)
				{
					healCD = 140f - num;
				}
				else
				{
					healCD = memmedHealSpell3.Cooldown * 60f;
				}
				return;
			}
			if (Myself.NearbyFriends.Count <= 0)
			{
				continue;
			}
			if (!MyStats.Charmed)
			{
				foreach (Character nearbyFriend in Myself.NearbyFriends)
				{
					if (nearbyFriend != null && (float)nearbyFriend.MyStats.CurrentHP < (float)nearbyFriend.MyStats.CurrentMaxHP * 0.66f && nearbyFriend.Alive)
					{
						if (MyNav.isOnNavMesh)
						{
							MyNav.isStopped = true;
						}
						MyAnim.SetBool("Walking", value: false);
						MyAnim.SetBool("Patrol", value: false);
						MySpells.StartSpell(memmedHealSpell3, nearbyFriend.MyStats);
						if (SimPlayer)
						{
							ThisSim.SetSpellCooldownBySpell(memmedHealSpell3, memmedHealSpell3.Cooldown + memmedHealSpell3.SpellChargeTime / 60f);
						}
						if (InGroup)
						{
							UpdateSocialLog.LogAdd(NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString("Casting " + memmedHealSpell3.SpellName.ToUpper() + " on " + nearbyFriend.MyStats.MyName, ThisSim), "#00B2B7");
						}
						if (MyStats.CharacterClass == GameData.ClassDB.Druid)
						{
							healCD = 140f - num;
						}
						else
						{
							healCD = memmedHealSpell3.Cooldown * 60f;
						}
						return;
					}
				}
				continue;
			}
			if ((float)GameData.PlayerStats.CurrentHP < (float)GameData.PlayerStats.CurrentMaxHP * 0.66f && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 60f && GameData.PlayerStats.Myself.Alive)
			{
				if (MyNav.isOnNavMesh)
				{
					MyNav.isStopped = true;
				}
				MyAnim.SetBool("Walking", value: false);
				MyAnim.SetBool("Patrol", value: false);
				MySpells.StartSpell(memmedHealSpell3, GameData.PlayerStats);
				if (MyStats.CharacterClass == GameData.ClassDB.Druid)
				{
					healCD = 140f - num;
				}
				else
				{
					healCD = memmedHealSpell3.Cooldown * 60f;
				}
				if (SimPlayer)
				{
					ThisSim.SetSpellCooldownBySpell(memmedHealSpell3, memmedHealSpell3.Cooldown + memmedHealSpell3.SpellChargeTime / 60f);
				}
				if (InGroup)
				{
					UpdateSocialLog.LogAdd(NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString("Casting " + memmedHealSpell3.SpellName.ToUpper() + " on " + GameData.PlayerStats.MyName, ThisSim), "#00B2B7");
				}
				return;
			}
			if (Myself.MyCharmedNPC != null && (float?)Myself.MyCharmedNPC.MyStats?.CurrentHP < (float?)Myself.MyCharmedNPC.MyStats?.CurrentMaxHP * 0.66f)
			{
				if (MyNav.isOnNavMesh)
				{
					MyNav.isStopped = true;
				}
				MyAnim.SetBool("Walking", value: false);
				MyAnim.SetBool("Patrol", value: false);
				MySpells.StartSpell(memmedHealSpell3, Myself.MyCharmedNPC.MyStats);
				if (MyStats.CharacterClass == GameData.ClassDB.Druid)
				{
					healCD = 140f - num;
				}
				else
				{
					healCD = memmedHealSpell3.Cooldown * 60f;
				}
				if (SimPlayer)
				{
					ThisSim.SetSpellCooldownBySpell(memmedHealSpell3, memmedHealSpell3.Cooldown + memmedHealSpell3.SpellChargeTime / 60f);
				}
				if (InGroup)
				{
					UpdateSocialLog.LogAdd(NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString("Casting " + memmedHealSpell3.SpellName.ToUpper() + " on " + Myself.MyCharmedNPC.transform.name, ThisSim), "#00B2B7");
				}
				return;
			}
			break;
		}
		int num4 = 0;
		if (SimPlayer)
		{
			foreach (Character nearbyFriend2 in Myself.NearbyFriends)
			{
				if (nearbyFriend2 != null && (float)nearbyFriend2.MyStats.CurrentHP < (float)nearbyFriend2.MyStats.CurrentMaxHP * 0.66f && nearbyFriend2.Alive)
				{
					num4++;
				}
			}
			if (num4 > 1 && GroupHeals.Count > 0)
			{
				foreach (Spell groupHeal3 in GroupHeals)
				{
					if (groupHeal3.MyDamageType == GameData.DamageType.Physical && MyStats.CurrentMana > groupHeal3.ManaCost)
					{
						if (MyNav.isOnNavMesh)
						{
							MyNav.isStopped = true;
						}
						MyAnim.SetBool("Walking", value: false);
						MyAnim.SetBool("Patrol", value: false);
						MySpells.StartSpell(groupHeal3, MyStats);
						if (MyStats.CharacterClass == GameData.ClassDB.Druid)
						{
							healCD = 140f - num;
						}
						else
						{
							healCD = groupHeal3.Cooldown * 60f;
						}
						if (SimPlayer)
						{
							ThisSim.SetSpellCooldownBySpell(groupHeal3, groupHeal3.Cooldown + groupHeal3.SpellChargeTime / 60f);
						}
						if (InGroup)
						{
							UpdateSocialLog.LogAdd(NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString("Casting " + groupHeal3.SpellName.ToUpper(), ThisSim), "#00B2B7");
						}
						return;
					}
				}
			}
		}
		if (!SimPlayer || !InGroup || !(MyHOTSpell != null) || MyStats.CurrentMana <= MyHOTSpell.ManaCost)
		{
			return;
		}
		NPC nPC2 = GameData.SimPlayerGrouping?.MainTank?.MyAvatar?.GetThisNPC();
		if (nPC2 != null && nPC2.CurrentAggroTarget != null)
		{
			if (!nPC2.MyStats.CheckForSEByName(MyHOTSpell.StatusEffectToApply.SpellName) && nPC2.CurrentAggroTarget.MyStats.Level >= nPC2.MyStats.Level - 6)
			{
				if (MyNav.isOnNavMesh)
				{
					MyNav.isStopped = true;
				}
				MyAnim.SetBool("Walking", value: false);
				MyAnim.SetBool("Patrol", value: false);
				MySpells.StartSpell(MyHOTSpell, nPC2.MyStats);
				if (SimPlayer)
				{
					ThisSim.SetSpellCooldownBySpell(MyHOTSpell, MyHOTSpell.Cooldown + MyHOTSpell.SpellChargeTime / 60f);
				}
				if (InGroup)
				{
					UpdateSocialLog.LogAdd(NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString("HOT INCOMING on " + MyHOTSpell.SpellName.ToUpper() + " on " + nPC2.transform.name, ThisSim), "#00B2B7");
				}
				if (MyStats.CharacterClass == GameData.ClassDB.Druid)
				{
					healCD = 140f - num;
				}
				else
				{
					healCD = MyHOTSpell.Cooldown * 60f;
				}
				if (SimPlayer)
				{
					ThisSim.SetSpellCooldownBySpell(MyHOTSpell, MyHOTSpell.Cooldown + MyHOTSpell.SpellChargeTime / 60f);
				}
			}
		}
		else if (nPC2 == null && GameData.SimPlayerGrouping.PlayerIsTank && GameData.PlayerControl.CurrentTarget != null && GameData.PlayerControl.CurrentTarget.MyNPC != null && !GameData.PlayerStats.CheckForSEByName(MyHOTSpell.StatusEffectToApply.SpellName) && GameData.PlayerControl.CurrentTarget.MyStats.Level >= GameData.PlayerStats.Level - 6 && GameData.PlayerControl.CurrentTarget.MyNPC.CurrentAggroTarget == GameData.PlayerControl.Myself)
		{
			if (MyNav.isOnNavMesh)
			{
				MyNav.isStopped = true;
			}
			MyAnim.SetBool("Walking", value: false);
			MyAnim.SetBool("Patrol", value: false);
			MySpells.StartSpell(MyHOTSpell, GameData.PlayerStats);
			if (SimPlayer)
			{
				ThisSim.SetSpellCooldownBySpell(MyHOTSpell, MyHOTSpell.Cooldown + MyHOTSpell.SpellChargeTime / 60f);
			}
			if (InGroup)
			{
				UpdateSocialLog.LogAdd(NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString("HOT INCOMING: " + MyHOTSpell.SpellName.ToUpper() + " on " + GameData.PlayerStats.MyName, ThisSim), "#00B2B7");
			}
			if (MyStats.CharacterClass == GameData.ClassDB.Druid)
			{
				healCD = 140f - num;
			}
			else
			{
				healCD = MyHOTSpell.Cooldown * 60f;
			}
			if (SimPlayer)
			{
				ThisSim.SetSpellCooldownBySpell(MyHOTSpell, MyHOTSpell.Cooldown + MyHOTSpell.SpellChargeTime / 60f);
			}
		}
	}

	public bool CheckManaSpell()
	{
		if (MyStats.CharacterClass == GameData.ClassDB.Arcanist || MyStats.CharacterClass == GameData.ClassDB.Druid)
		{
			Spell spell = null;
			foreach (Spell memmedHealSpell in MemmedHealSpells)
			{
				if (memmedHealSpell.PercentManaRestoration > 0)
				{
					spell = memmedHealSpell;
					break;
				}
			}
			if (spell == null)
			{
				return false;
			}
			if (CurrentAggroTarget != null)
			{
				bool flag = (float)MyStats.CurrentMana < (float)MyStats.GetCurrentMaxMana() * 0.1f || (MyStats.CharacterClass == GameData.ClassDB.Druid && !CanAffordAHealSpell());
				if (IsCasting())
				{
					return false;
				}
				if (flag)
				{
					if (MyNav.isOnNavMesh)
					{
						MyNav.isStopped = true;
					}
					MyAnim.SetBool("Walking", value: false);
					MyAnim.SetBool("Patrol", value: false);
					MySpells.StartSpell(spell, MyStats);
					if (SimPlayer)
					{
						ThisSim.SetSpellCooldownBySpell(spell, spell.Cooldown);
					}
					if (InGroup)
					{
						UpdateSocialLog.LogAdd(NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString("OOM! Casting meditate!", ThisSim), "#00B2B7");
					}
					if (SimPlayer)
					{
						ThisSim.SetSpellCooldownBySpell(spell, spell.Cooldown);
					}
					return true;
				}
			}
		}
		return false;
	}

	private bool CanAffordAHealSpell()
	{
		if (MyHealSpells.Count <= 0)
		{
			return true;
		}
		foreach (Spell myHealSpell in MyHealSpells)
		{
			if (MyStats.CurrentMana > myHealSpell.ManaCost && MyStats.Level - myHealSpell.RequiredLevel < 15)
			{
				return true;
			}
		}
		return false;
	}

	private void CheckAggro()
	{
		if (SimPlayer)
		{
			AggroTable.Clear();
			if (MyStats.Invisible)
			{
				return;
			}
		}
		if (AggroTable.Count > 0)
		{
			for (int num = AggroTable.Count - 1; num >= 0; num--)
			{
				if (AggroTable[num].Player == null || !AggroTable[num].Player.Alive || AggroTable[num].Player.MyFaction == Character.Faction.Mineral || !AggroTable[num].Player.gameObject.activeSelf)
				{
					AggroTable.RemoveAt(num);
				}
			}
		}
		if (CurrentAggroTarget != null)
		{
			if (!CurrentAggroTarget.Alive)
			{
				if (Myself.NearbyEnemies.Contains(CurrentAggroTarget))
				{
					Myself.NearbyEnemies.Remove(CurrentAggroTarget);
				}
				if (!CurrentAggroTarget.MyStats.Charmed)
				{
					CurrentAggroTarget = null;
				}
				else if (CurrentAggroTarget != null && CurrentAggroTarget.Master != null)
				{
					ManageAggro(1, CurrentAggroTarget.Master);
				}
				else
				{
					CurrentAggroTarget = null;
				}
			}
			else if (!SimPlayer && !Myself.MyStats.Charmed)
			{
				CurrentAggroTarget = null;
				CheckAggro();
			}
			if (CurrentAggroTarget == Myself.Master && MyStats.Charmed)
			{
				CurrentAggroTarget = null;
			}
		}
		else if (CurrentAggroTarget == null && AggroTable.Count > 0 && !MyStats.Charmed && (ThisSim == null || (ThisSim != null && !ThisSim.SeekPlayer && !ThisSim.awaitInvite)))
		{
			Character highestAggro = GetHighestAggro();
			if (highestAggro != null && (!SimPlayer || !ThisSim.InGroup || (SimPlayer && ThisSim.InGroup && highestAggro.isNPC && GameData.SimPlayerGrouping.GroupTargets.Contains(highestAggro))))
			{
				AggroOn(highestAggro);
				if (Myself.NearbyFriends.Count > 0)
				{
					foreach (Character nearbyFriend in Myself.NearbyFriends)
					{
						if (nearbyFriend != null && nearbyFriend.GetComponent<NPC>() != null && CheckLOS(nearbyFriend))
						{
							nearbyFriend.GetComponent<NPC>().CurrentAggroTarget = CurrentAggroTarget;
							nearbyFriend.GetComponent<NPC>().ManageAggro(1, highestAggro);
						}
					}
				}
			}
		}
		if (CurrentAggroTarget == null && SimPlayer && ThisSim.InGroup && GameData.SimPlayerGrouping.GroupTargets.Count > 0)
		{
			if (GameData.SimPlayerGrouping.GroupTargets.Count > 0 && GameData.SimPlayerGrouping.GroupTargets[0] != null && GameData.SimPlayerGrouping.GroupTargets[0].Alive && Myself.NearbyEnemies.Contains(GameData.SimPlayerGrouping.GroupTargets[0]))
			{
				if (GameData.SimPlayerGrouping.GroupTargets.Count > 1)
				{
					GameData.SimPlayerGrouping.GroupTargets = (from t in GameData.SimPlayerGrouping.GroupTargets
						where t != null && t.gameObject.activeSelf
						orderby t.MyStats.Unstunnable descending, !t.MyStats.Stunned descending, !t.MyStats.Feared descending
						select t).ToList();
				}
				if (!(GameData.SimPlayerGrouping.GroupTargets[0].MyNPC.CurrentAggroTarget != null))
				{
					return;
				}
				AggroOn(GameData.SimPlayerGrouping.GroupTargets[0]);
				if (CurrentAggroTarget != null && GameData.SimPlayerGrouping.GetMA() == base.transform.name && ThisSim.CurrentPullPhase == global::SimPlayer.PullPhases.NotPulling)
				{
					GameData.SimPlayerGrouping.GroupAttack(CurrentAggroTarget);
				}
				if (GameData.SimPlayerGrouping.GroupTargets[0].MyStats.Unstunnable && GameData.SimPlayerGrouping.GroupTargets.Count > 1 && ThisSim.CurrentPullPhase == global::SimPlayer.PullPhases.NotPulling)
				{
					if (Random.Range(0, 10) > 8)
					{
						UpdateSocialLog.LogAdd(NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString("Get on " + GameData.SimPlayerGrouping.GroupTargets[0].transform.name + " - it's immune to crowd control", ThisSim), "#00B2B7");
					}
					GameData.SimPlayerGrouping.GroupAttack(GameData.SimPlayerGrouping.GroupTargets[0]);
				}
			}
			else if (GameData.SimPlayerGrouping.GroupTargets.Count > 0 && Myself.NearbyEnemies.Count > 0 && !Myself.NearbyEnemies.Contains(GameData.SimPlayerGrouping.GroupTargets[0]))
			{
				GameData.SimPlayerGrouping.GroupTargets.RemoveAt(0);
			}
		}
		else
		{
			if (Myself.NearbyEnemies.Count > 0 && !MyStats.Charmed && (ThisSim == null || (ThisSim != null && !ThisSim.SeekPlayer && !ThisSim.awaitInvite)))
			{
				if (!(CurrentAggroTarget == null))
				{
					return;
				}
				{
					foreach (Character nearbyEnemy in Myself.NearbyEnemies)
					{
						if (!(nearbyEnemy != null) || !nearbyEnemy.gameObject.activeSelf || !nearbyEnemy.Alive || (!CheckLOS(nearbyEnemy) && !AggroRegardlessofLOS) || (nearbyEnemy.MyStats.Level > MyStats.Level + 5 && !AggroRegardlessOfLevel) || (ThisSim != null && nearbyEnemy.MyStats.Level < MyStats.Level - 6 && !nearbyEnemy.Invulnerable) || (nearbyEnemy.MyStats.Invisible && !Myself.SeeInvisible))
						{
							continue;
						}
						ManageAggro(3, nearbyEnemy);
						if (SimPlayer && ThisSim.InGroup && nearbyEnemy.MyNPC != null && nearbyEnemy.MyNPC.CurrentAggroTarget == null)
						{
							continue;
						}
						AggroOn(nearbyEnemy);
						if (AggroMsg != "" && Vector3.Distance(NamePlate.transform.position, GameData.PlayerControl.transform.position + Vector3.up * 2f) < 10f)
						{
							UpdateSocialLog.LogAdd(base.transform.name + " says: " + AggroMsg);
						}
						if (AggroEmote != "" && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 10f)
						{
							UpdateSocialLog.LogAdd(base.transform.name + " " + AggroEmote);
						}
						if (Myself.NearbyFriends.Count <= 0)
						{
							break;
						}
						{
							foreach (Character nearbyFriend2 in Myself.NearbyFriends)
							{
								if (nearbyFriend2 != null && nearbyFriend2.GetComponent<NPC>() != null && CheckLOS(nearbyFriend2))
								{
									nearbyFriend2.GetComponent<NPC>().CurrentAggroTarget = CurrentAggroTarget;
								}
							}
							break;
						}
					}
					return;
				}
			}
			if (ThisSim != null && CurrentAggroTarget != null && (ThisSim.SeekPlayer || ThisSim.awaitInvite))
			{
				CurrentAggroTarget = null;
			}
		}
	}

	public bool CheckLOS(Character _visible)
	{
		if (_visible == null || _visible.transform == null)
		{
			return false;
		}
		bool result = true;
		Vector3 direction = _visible.transform.position + Vector3.up - base.transform.position + Vector3.up;
		RaycastHit[] array = Physics.RaycastAll(base.transform.position + Vector3.up, direction, direction.magnitude);
		for (int i = 0; i < array.Length; i++)
		{
			RaycastHit raycastHit = array[i];
			if (raycastHit.transform != null && raycastHit.transform.GetComponent<Character>() == null)
			{
				result = false;
			}
		}
		return result;
	}

	private void Combat()
	{
		if (Myself.Master != null && CurrentAggroTarget != null && CurrentAggroTarget.MyStats.Stunned && CurrentAggroTarget.MyStats.AmISoftMezzed())
		{
			CurrentAggroTarget = null;
			return;
		}
		if (inMeleeRange && InPosition && CurrentAggroTarget != null && !retreat && (!SimPlayer || (SimPlayer && !ThisSim.IgnoreAllCombat && !MyStats.Invisible)))
		{
			if (MyNav.enabled && MyNav.isOnNavMesh)
			{
				MyNav.isStopped = true;
			}
			MyStats.RecentDmg = 240f;
			Vector3 worldPosition = new Vector3(CurrentAggroTarget.transform.position.x, base.transform.position.y, CurrentAggroTarget.transform.position.z);
			base.transform.LookAt(worldPosition);
			if ((MyAttackSpells.Count <= 0 || Random.Range(0, 10) < 7 || MHWand) && MyStats.GetMHAtkDelay() <= 0f)
			{
				ResetAttackAnimations();
				if (SimPlayer)
				{
					MyAnim.SetInteger("AttackIndex", Random.Range(0, 2));
					if (MyStats.Invisible)
					{
						MyStats.BreakEffectsOnAction();
					}
				}
				bool flag = ShouldDualWield();
				int doubleAttackCount = GetDoubleAttackCount();
				for (int i = 0; i <= doubleAttackCount; i++)
				{
					if (i == 0 && MyStats.CombatStance.SelfDamagePerAttack > 0f)
					{
						float num = MyStats.CombatStance.SelfDamagePerAttack / 100f;
						float f = (float)MyStats.CurrentMaxHP * num;
						MyStats.Myself.SelfDamageMeFlat(Mathf.RoundToInt(f));
					}
					if (!MHBow)
					{
						if (i == 0)
						{
							MyAnim.SetTrigger("MeleeSwing");
						}
						if (i == 1)
						{
							MyAnim.SetBool("DoubleAttack", value: true);
						}
						if (i == 2)
						{
							MyAnim.SetTrigger("Special1");
						}
					}
					else if (!SimPlayer)
					{
						MyAnim.SetTrigger("FireBow");
					}
					PerformMeleeHit(BaseAtkDmg, isOffhand: false);
					Stance combatStance = MyStats.CombatStance;
					if ((object)combatStance != null && combatStance.SelfDamagePerAttack > 0f)
					{
						MyStats.Myself.SelfDamageMe(MyStats.CombatStance.SelfDamagePerAttack);
					}
				}
				if (roundsSinceTarg < 5)
				{
					roundsSinceTarg++;
				}
				if (CanPhantomStrike && Myself.NearbyEnemies.Count > 1 && Random.Range(0, 100) > 66)
				{
					Character currentAggroTarget = CurrentAggroTarget;
					CurrentAggroTarget = Myself.NearbyEnemies[Random.Range(0, Myself.NearbyEnemies.Count)];
					if (CurrentAggroTarget != currentAggroTarget && CurrentAggroTarget != null)
					{
						PerformPhantomHit(Mathf.RoundToInt((float)BaseAtkDmg * 0.3f), isOffhand: false);
					}
					CurrentAggroTarget = currentAggroTarget;
				}
				if (flag && MyStats.GetOHAtkDelay() <= 0f)
				{
					int offhandDoubleAttackCount = GetOffhandDoubleAttackCount();
					for (int j = 0; j <= offhandDoubleAttackCount; j++)
					{
						if (!MHBow)
						{
							if (j == 0)
							{
								MyAnim.SetBool("DualWield", value: true);
							}
							if (j == 1)
							{
								MyAnim.SetBool("OHDoubleAttack", value: true);
							}
						}
						PerformMeleeHit(OHAtkDmg, isOffhand: true);
					}
				}
				TryCoupDeGrace();
			}
			if (CanCastSpell())
			{
				DoAttackSpell();
			}
			if (SimPlayer && ThisSim.Skillbook.Count > 0 && AllHKCD <= 0f)
			{
				DoAttackSkill();
			}
		}
		else
		{
			if (CurrentAggroTarget != null && SimPlayer && GameData.SimMngr.Sims.Count > ThisSim.myIndex && GameData.SimMngr.Sims[ThisSim.myIndex].isPuller && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) > 5f)
			{
				return;
			}
			if (CurrentAggroTarget != null && CanCastSpell())
			{
				DoAttackSpell();
			}
		}
		if (SimPlayer && MyStats.CharacterClass == GameData.ClassDB.Stormcaller && IsCasting() && MHBow && CurrentAggroTarget != null && CurrentAggroTarget != null && (float)CurrentAggroTarget.GetCurHealthAsIntPercentage() < HoldDPS)
		{
			DoImbued();
		}
	}

	private void ResetAttackAnimations()
	{
		MyAnim.SetBool("DoubleAttack", value: false);
		MyAnim.SetBool("DualWield", value: false);
		MyAnim.SetBool("OHDoubleAttack", value: false);
	}

	private bool ShouldDualWield()
	{
		if (MHBow)
		{
			return false;
		}
		if (!Myself.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByName("Dual Wield")))
		{
			return false;
		}
		if (!SimPlayer)
		{
			return true;
		}
		if (ThisSim.MyEquipment[0].MyItem.ThisWeaponType != Item.WeaponType.TwoHandMelee && ThisSim.MyEquipment[0].MyItem.ThisWeaponType != Item.WeaponType.TwoHandStaff && (ThisSim.MyEquipment[1].MyItem.ThisWeaponType == Item.WeaponType.OneHandDagger || ThisSim.MyEquipment[1].MyItem.ThisWeaponType == Item.WeaponType.OneHandMelee))
		{
			return Random.Range(0, 40) < MyStats.Level;
		}
		return false;
	}

	private int GetDoubleAttackCount()
	{
		int num = 10;
		int num2 = 0;
		if (MyStats.CharacterClass != null && MyStats.CharacterClass == GameData.ClassDB.Paladin)
		{
			num2 = (Myself?.MySkills?.GetAscensionRank("48388782")).GetValueOrDefault();
		}
		if (MyStats.CharacterClass != null && MyStats.CharacterClass == GameData.ClassDB.Reaver)
		{
			num2 = (Myself?.MySkills?.GetAscensionRank("27836061")).GetValueOrDefault();
			num = 12;
		}
		if (Random.Range(0, 100) < num2 * num)
		{
			return 2;
		}
		if (!Myself.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByName("Double Attack")) || Random.Range(0, 40) >= MyStats.Level)
		{
			return 0;
		}
		return 1;
	}

	private int GetOffhandDoubleAttackCount()
	{
		if (!Myself.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByName("Double Attack")) || Random.Range(0, 20) >= MyStats.Level)
		{
			return 0;
		}
		return 1;
	}

	private bool CanCastSpell()
	{
		return !MySpells.isCasting();
	}

	private void PerformMeleeHit(int baseDamage, bool isOffhand)
	{
		if (trickShotTarg.Count > 0)
		{
			trickShotTarg = trickShotTarg.Where((GameObject x) => x != null).ToList();
		}
		if (trickShotTarg.Count > 0 && roundsSinceTarg > 1)
		{
			Character target = trickShotTarg[0]?.GetComponent<Character>() ?? CurrentAggroTarget;
			DoBowAttack(target, 0);
		}
		bool criticalHit = false;
		int num = 0;
		if (SimPlayer)
		{
			num = MyStats.CalcMeleeDamage(baseDamage, CurrentAggroTarget.MyStats.Level, CurrentAggroTarget.MyStats, 0);
		}
		else
		{
			if (DamageRange.x == 1f && DamageRange.y == 1f)
			{
				DamageRange.x = Mathf.Pow(MyStats.Level, 1.9f) * 0.6f + 2f;
				DamageRange.y = Mathf.Pow(MyStats.Level, 1.9f) + 6f;
				DamageRange.x *= DamageMult;
				DamageRange.y *= DamageMult;
			}
			num = MyStats.CalcMeleeDamageForNPC(DamageRange, CurrentAggroTarget.MyStats);
		}
		if (!SimPlayer && !Myself.MyStats.Charmed)
		{
			float num2 = 0.008f;
			int num3 = MyStats.Level - CurrentAggroTarget.MyStats.Level;
			float num4 = Mathf.Clamp(MyStats.Level - 10, 1f, float.PositiveInfinity) * num2;
			float num5 = ((num3 <= 0) ? 0f : ((num3 > 2) ? ((float)num3 * num4) : ((float)num3 * (num4 / 2f))));
			float value = 1f + num5;
			value = Mathf.Clamp(value, 0.25f, 10f);
			num = Mathf.RoundToInt((float)num * GameData.ServerDMGMod * value);
		}
		if (SimPlayer)
		{
			MyStats.CheckProc(isOffhand ? MyStats.MyInv.EquippedItems[1] : MyStats.MyInv.EquippedItems[0], CurrentAggroTarget);
			if ((!isOffhand && !MHWand && !MHBow) || isOffhand)
			{
				if (MyStats.isCriticalAttack() && num > 0)
				{
					criticalHit = true;
					num = Mathf.RoundToInt((float)num * 1.5f);
					if (MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByName("Crippling Blow")) && Random.Range(0, 10) > 8)
					{
						num *= 2;
						if (Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 10f)
						{
							UpdateSocialLog.CombatLogAdd(Myself.MyNPC.NPCName + " scores a CRIPPLING BLOW!", "lightblue");
						}
					}
					GameData.CamControl.ShakeScreen(0.1f, 3f);
				}
			}
			else
			{
				if (MHWand && !isOffhand && !MHBow)
				{
					DoWandAttack(CurrentAggroTarget);
					MyStats.ResetMHAtkDelay();
					return;
				}
				if (MHBow && !isOffhand && !MHWand && (bool)CurrentAggroTarget)
				{
					if (GetChar().MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByName("Trick Shot")) && Random.Range(0, 100) > 94)
					{
						Vector3 safeNavMeshPointInRange = GameData.GetSafeNavMeshPointInRange(CurrentAggroTarget.transform.position, 20f, 4f);
						if (safeNavMeshPointInRange != Vector3.zero)
						{
							trickShotTarg.Add(Object.Instantiate(GameData.Misc.TrickShotTarg, safeNavMeshPointInRange, base.transform.rotation));
							UpdateSocialLog.CombatLogAdd(base.transform.name + " finds an opening and throws a target into the fray", "lightblue");
							roundsSinceTarg = 0;
						}
					}
					DoBowAttack(CurrentAggroTarget, 0);
					MyStats.ResetMHAtkDelay();
					return;
				}
			}
		}
		if (AttackParticles != null)
		{
			AttackParticles.Play();
		}
		string text = CheckTargetInnateAvoidance(CurrentAggroTarget);
		string text2 = ((CurrentAggroTarget.transform.name == "Player") ? "YOU" : CurrentAggroTarget.transform.name);
		string text3 = ((text2 == "YOU") ? "YOUR" : "their");
		string text4 = ((text2 == "YOU") ? "see" : "sees");
		string colorAsString = ((text2 == "YOU") ? "red" : "white");
		if (text != "")
		{
			if (CurrentAggroTarget == GameData.PlayerControl.Myself)
			{
				UpdateSocialLog.CombatLogAdd(base.transform.name + " tries to attack " + text2 + ", but " + text2 + text, colorAsString);
				if (text == " parried the attack!")
				{
					UpdateSocialLog.CombatLogAdd(text2 + " avoided the attack and " + text4 + " an opening for a counter-attack!", colorAsString);
					CurrentAggroTarget.MyStats.ZeroMHAtkDelay();
					text = "";
				}
			}
			if (!isOffhand)
			{
				MyStats.ResetMHAtkDelay();
			}
			if (isOffhand)
			{
				MyStats.ResetOHAtkDelay();
			}
			return;
		}
		MyStats.ResetMHAtkDelay();
		if (isOffhand)
		{
			MyStats.ResetOHAtkDelay();
		}
		int bonusDmg = 0;
		if (MyStats.CharacterClass == GameData.ClassDB.Duelist)
		{
			bonusDmg = ((!isOffhand) ? (MyStats?.MyInv?.MH?.MyItem?.WeaponDmg).GetValueOrDefault() : (MyStats?.MyInv?.OH?.MyItem?.WeaponDmg).GetValueOrDefault());
		}
		int num6 = CurrentAggroTarget.DamageMe(num, MyStats.Charmed || (ThisSim != null && ThisSim.InGroup), GameData.DamageType.Physical, Myself, _animEffect: true, criticalHit, bonusDmg);
		if (num6 > 0)
		{
			if (CurrentAggroTarget != null && CurrentAggroTarget.MyStats.GetCurrentDS() > 0)
			{
				Myself.DamageShieldTaken(CurrentAggroTarget.MyStats.GetCurrentDS(), CurrentAggroTarget.MyStats);
			}
			if (MyStats.Myself.MySkills.HasAscension("24943180"))
			{
				int num7 = Mathf.RoundToInt((float)(num6 * MyStats.Myself.MySkills.GetAscensionRank("24943180")) * 0.01f);
				MyStats.CurrentMana += num7;
			}
		}
		if (NPCProcOnHit != null && (float)Random.Range(0, 100) < NPCProcOnHitChance)
		{
			MySpells.StartSpellFromProc(NPCProcOnHit, CurrentAggroTarget.MyStats, 0.1f);
		}
		if (Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 10f || CurrentAggroTarget == GameData.PlayerControl.Myself)
		{
			if (num6 > 0 && text2 == "YOU")
			{
				UpdateSocialLog.CombatLogAdd(base.transform.name + " attacks " + text2 + " for " + num6 + " damage.", colorAsString);
			}
			else if (num6 == 0 && text2 == "YOU")
			{
				UpdateSocialLog.CombatLogAdd(base.transform.name + " tries to attack " + text2 + ", but misses!", colorAsString);
				if (CurrentAggroTarget.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByID("77724113")) && Random.Range(0, 100) < 5 + CurrentAggroTarget.MyStats.Level / 3)
				{
					if (Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 10f)
					{
						UpdateSocialLog.CombatLogAdd(text2 + " avoided the attack and " + text4 + " an opening for a counter-attack!", colorAsString);
					}
					CurrentAggroTarget.MyStats.ZeroMHAtkDelay();
				}
			}
			else if (num6 == -2 && text2 == "YOU")
			{
				UpdateSocialLog.CombatLogAdd(base.transform.name + " tries to attack " + text2 + ", but " + text3 + " shield absorbs the blow!", colorAsString);
			}
			MyStats.HealMe(Mathf.RoundToInt((float)num6 * (MyStats.PercentLifesteal / 100f)));
		}
		if (Myself.MyAttackSound != null && (Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 8f || CurrentAggroTarget == GameData.PlayerControl.Myself))
		{
			Myself.MyAudio.PlayOneShot(Myself.MyAttackSound, Myself.MyAudio.volume * GameData.CombatVol * GameData.MasterVol);
		}
	}

	private void PerformMeleeHitPreCalc(int calcDmg, bool isOffhand)
	{
		bool criticalHit = false;
		int num = calcDmg;
		if (!SimPlayer && !Myself.MyStats.Charmed)
		{
			num = Mathf.RoundToInt((float)num * GameData.ServerDMGMod);
		}
		if (SimPlayer)
		{
			MyStats.CheckProc(isOffhand ? MyStats.MyInv.EquippedItems[1] : MyStats.MyInv.EquippedItems[0], CurrentAggroTarget);
			if ((!isOffhand && !MHWand) || isOffhand)
			{
				if (MyStats.isCriticalAttack() && num > 0)
				{
					num = Mathf.RoundToInt((float)num * 1.5f);
					criticalHit = true;
					if (MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByName("Crippling Blow")) && Random.Range(0, 10) > 8)
					{
						num *= 2;
						if (Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 10f)
						{
							UpdateSocialLog.CombatLogAdd(Myself.MyNPC.NPCName + " scores a CRIPPLING BLOW!", "lightblue");
						}
					}
					GameData.CamControl.ShakeScreen(0.1f, 3f);
				}
			}
			else if (MHWand && !isOffhand)
			{
				DoWandAttack(CurrentAggroTarget);
				MyStats.ResetMHAtkDelay();
				return;
			}
		}
		string text = CheckTargetInnateAvoidance(CurrentAggroTarget);
		string text2 = ((CurrentAggroTarget.transform.name == "Player") ? "YOU" : CurrentAggroTarget.transform.name);
		string text3 = ((text2 == "YOU") ? "YOUR" : "their");
		string colorAsString = ((text2 == "YOU") ? "red" : "white");
		if (text != "")
		{
			MyStats.ResetMHAtkDelay();
			if (isOffhand)
			{
				MyStats.ResetOHAtkDelay();
			}
			if (CurrentAggroTarget == GameData.PlayerControl.Myself)
			{
				UpdateSocialLog.CombatLogAdd(base.transform.name + " tries to attack " + text2 + ", but " + text2 + text, colorAsString);
			}
			return;
		}
		if (!isOffhand)
		{
			MyStats.ResetMHAtkDelay();
		}
		if (isOffhand)
		{
			MyStats.ResetOHAtkDelay();
		}
		int num2 = CurrentAggroTarget.DamageMe(num, MyStats.Charmed || (ThisSim != null && ThisSim.InGroup), GameData.DamageType.Physical, Myself, _animEffect: true, criticalHit);
		if (NPCProcOnHit != null && (float)Random.Range(0, 100) < NPCProcOnHitChance)
		{
			MySpells.StartSpellFromProc(NPCProcOnHit, CurrentAggroTarget.MyStats, 0.1f);
		}
		if (Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 10f || CurrentAggroTarget == GameData.PlayerControl.Myself)
		{
			if (num2 > 0 && text2 == "YOU")
			{
				UpdateSocialLog.CombatLogAdd(base.transform.name + " attacks " + text2 + " for " + num2 + " damage.", colorAsString);
			}
			else if (num2 == 0 && text2 == "YOU")
			{
				UpdateSocialLog.CombatLogAdd(base.transform.name + " tries to attack " + text2 + ", but misses!", colorAsString);
			}
			else if (num2 == -2 && text2 == "YOU")
			{
				UpdateSocialLog.CombatLogAdd(base.transform.name + " tries to attack " + text2 + ", but " + text3 + " shield absorbs the blow!", colorAsString);
			}
			MyStats.HealMe(Mathf.RoundToInt((float)num2 * (MyStats.PercentLifesteal / 100f)));
		}
		if (Myself.MyAttackSound != null && (Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 8f || CurrentAggroTarget == GameData.PlayerControl.Myself))
		{
			Myself.MyAudio.PlayOneShot(Myself.MyAttackSound, Myself.MyAudio.volume * GameData.CombatVol * GameData.MasterVol);
		}
	}

	private void DoWandAttack(Character _target)
	{
		Item item = null;
		if (SimPlayer)
		{
			item = ThisSim.MyStats.MyInv.SimMH?.MyItem;
		}
		if (!(item == null))
		{
			WandBolt component = Object.Instantiate(GameData.Misc.WandBoltSimple, base.transform.position + Vector3.up + base.transform.forward, base.transform.rotation).GetComponent<WandBolt>();
			Spell spell = null;
			if (item.WandEffect != null && (float)Random.Range(0, 100) < item.WandProcChance)
			{
				spell = item.WandEffect;
			}
			if (spell == null)
			{
				spell = MyStats.CheckSEProcsOnly();
			}
			component.LoadWandBolt(item.WeaponDmg, spell, _target, MyStats.Myself, item.WandBoltSpeed, GameData.DamageType.Magic, item.WandBoltColor, item.WandAttackSound);
		}
	}

	public void DoBowAttack(Character _target, int _arrowIndex)
	{
		MyAnim.SetTrigger("FireBow");
		Item item = null;
		if (SimPlayer)
		{
			item = ThisSim.MyStats.MyInv.SimMH?.MyItem;
		}
		if (!(item == null))
		{
			WandBolt component = Object.Instantiate(GameData.Misc.ArcheryArrows[_arrowIndex], base.transform.position + Vector3.up + base.transform.forward, base.transform.rotation).GetComponent<WandBolt>();
			Spell spell = null;
			if (item.BowEffect != null && (float)Random.Range(0, 100) < item.BowProcChance)
			{
				spell = item.BowEffect;
			}
			if (spell == null)
			{
				spell = MyStats.CheckSEProcsOnly();
			}
			component.LoadArrow(item.WeaponDmg, spell, _target, MyStats.Myself, item.BowArrowSpeed, GameData.DamageType.Physical, item.BowAttackSound);
		}
	}

	public void DoBowAttack(Character _target, int _arrowIndex, bool _interrupt, Spell _force)
	{
		MyAnim.SetTrigger("FireBow");
		Item item = null;
		if (SimPlayer)
		{
			item = ThisSim.MyStats.MyInv.SimMH?.MyItem;
		}
		if (!(item == null))
		{
			WandBolt component = Object.Instantiate(GameData.Misc.ArcheryArrows[_arrowIndex], base.transform.position + Vector3.up + base.transform.forward, base.transform.rotation).GetComponent<WandBolt>();
			Spell spell = _force;
			if (item.BowEffect != null && (float)Random.Range(0, 100) < item.BowProcChance)
			{
				spell = item.BowEffect;
			}
			if (spell == null)
			{
				spell = MyStats.CheckSEProcsOnly();
			}
			component.LoadArrow(item.WeaponDmg, spell, _target, MyStats.Myself, item.BowArrowSpeed, GameData.DamageType.Physical, item.BowAttackSound);
		}
	}

	public void DoBowAttack(Character _target, Spell _force, int _arrowIndex, bool _noCheckEffect)
	{
		MyAnim.SetTrigger("FireBow");
		Item item = null;
		if (SimPlayer)
		{
			item = ThisSim.MyStats.MyInv.SimMH?.MyItem;
		}
		if (!(item == null))
		{
			WandBolt component = Object.Instantiate(GameData.Misc.ArcheryArrows[_arrowIndex], base.transform.position + Vector3.up + base.transform.forward, base.transform.rotation).GetComponent<WandBolt>();
			Spell spell = null;
			if (_force != null)
			{
				spell = _force;
			}
			if (spell == null)
			{
				spell = MyStats.CheckSEProcsOnly();
			}
			if (!_noCheckEffect)
			{
				component.LoadArrow(item.WeaponDmg, spell, _target, MyStats.Myself, item.BowArrowSpeed, GameData.DamageType.Physical, item.WandAttackSound);
			}
			else
			{
				component.LoadArrow(item.WeaponDmg, spell, _target, MyStats.Myself, item.BowArrowSpeed, GameData.DamageType.Physical, _forceEffectOnTarget: true, item.WandAttackSound);
			}
		}
	}

	public void DoBowAttack(Character _target, Spell _force, int _arrowIndex, bool _noCheckEffect, float _scaleDmg)
	{
		MyAnim.SetTrigger("FireBow");
		Item item = null;
		if (SimPlayer)
		{
			item = ThisSim.MyStats.MyInv.SimMH?.MyItem;
		}
		if (!(item == null))
		{
			WandBolt component = Object.Instantiate(GameData.Misc.ArcheryArrows[_arrowIndex], base.transform.position + Vector3.up + base.transform.forward, base.transform.rotation).GetComponent<WandBolt>();
			Spell spell = null;
			if (_force != null)
			{
				spell = _force;
			}
			if (spell == null)
			{
				spell = MyStats.CheckSEProcsOnly();
			}
			if (!_noCheckEffect)
			{
				component.LoadArrow(Mathf.RoundToInt((float)item.WeaponDmg * _scaleDmg), spell, _target, MyStats.Myself, item.BowArrowSpeed, GameData.DamageType.Physical, item.WandAttackSound);
			}
			else
			{
				component.LoadArrow(Mathf.RoundToInt((float)item.WeaponDmg * _scaleDmg), spell, _target, MyStats.Myself, item.BowArrowSpeed, GameData.DamageType.Physical, _forceEffectOnTarget: true, item.WandAttackSound);
			}
		}
	}

	public void DoBowAttack(Character _target, int _dmgMod, int _arrowIndex)
	{
		MyAnim.SetTrigger("FireBow");
		Item item = null;
		if (SimPlayer)
		{
			item = ThisSim.MyStats.MyInv.SimMH?.MyItem;
		}
		if (!(item == null))
		{
			WandBolt component = Object.Instantiate(GameData.Misc.ArcheryArrows[_arrowIndex], base.transform.position + Vector3.up + base.transform.forward, base.transform.rotation).GetComponent<WandBolt>();
			Spell spell = null;
			if (item.BowEffect != null && (float)Random.Range(0, 100) < item.BowProcChance)
			{
				spell = item.BowEffect;
			}
			if (spell == null)
			{
				spell = MyStats.CheckSEProcsOnly();
			}
			component.LoadArrow(item.WeaponDmg * _dmgMod, spell, _target, MyStats.Myself, item.BowArrowSpeed, GameData.DamageType.Physical, item.BowAttackSound);
			if (_dmgMod > 4)
			{
				component.DisableCrits();
			}
		}
	}

	private void TryCoupDeGrace()
	{
		if (!MyStats.Myself.MySkills.HasAscension("2806936") || Random.Range(0, 8) >= MyStats.Myself.MySkills.GetAscensionRank("2806936"))
		{
			return;
		}
		int calcDmg = MyStats.CalcMeleeDamage(BaseAtkDmg, CurrentAggroTarget.MyStats.Level, CurrentAggroTarget.MyStats, 0) * 2;
		if (!(CheckTargetInnateAvoidance(CurrentAggroTarget) != ""))
		{
			if (Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 15f)
			{
				UpdateSocialLog.CombatLogAdd(base.transform.name + " perfoms a COUP DE GRACE!", "lightblue");
			}
			PerformMeleeHitPreCalc(calcDmg, isOffhand: false);
		}
	}

	public void AggroOn(Character tar)
	{
		if (tar == null || MyStats == null || !Myself.Alive || (tar.MyNPC != null && tar.MyNPC.TreasureChest))
		{
			return;
		}
		if (tar != null && tar.transform.name == "Player" && MyStats != null && MyStats.CurrentHP > 0 && !MyStats.Charmed && !SimPlayer)
		{
			if (inMeleeRange && CurrentAggroTarget == null)
			{
				base.transform.LookAt(new Vector3(tar.transform.position.x, base.transform.position.y, tar.transform.position.z));
			}
			if (!GameData.AttackingPlayer.Contains(this))
			{
				GameData.AttackingPlayer.Add(this);
			}
		}
		if (tar != null)
		{
			ManageAggro(1, tar);
		}
		if (tar != null)
		{
			SimPlayer component = tar.GetComponent<SimPlayer>();
			bool flag = false;
			if (component != null && GameData.SimMngr != null && GameData.SimMngr.Sims != null && component.myIndex >= 0 && component.myIndex < GameData.SimMngr.Sims.Count)
			{
				SimPlayerTracking simPlayerTracking = GameData.SimMngr.Sims[component.myIndex];
				if ((GameData.GroupMembers[0] != null && GameData.GroupMembers[0] == simPlayerTracking) || (GameData.GroupMembers[1] != null && GameData.GroupMembers[1] == simPlayerTracking) || (GameData.GroupMembers[2] != null && GameData.GroupMembers[2] == simPlayerTracking) || (GameData.GroupMembers[3] != null && GameData.GroupMembers[3] == simPlayerTracking))
				{
					flag = true;
				}
			}
			if (flag)
			{
				ManageAggro(1, GameData.PlayerControl.Myself);
			}
		}
		if (GetComponent<NPCFightEvent>() != null)
		{
			GetComponent<NPCFightEvent>().ChainAggro();
		}
		if (ThisSim != null && (ThisSim.SeekPlayer || ThisSim.awaitInvite))
		{
			CurrentAggroTarget = null;
			return;
		}
		if (Myself.Master != null && CurrentAggroTarget != null && CurrentAggroTarget.MyStats.Stunned && CurrentAggroTarget.MyStats.AmISoftMezzed())
		{
			CurrentAggroTarget = null;
			return;
		}
		if (CurrentAggroTarget == null)
		{
			inMeleeRange = false;
			CurrentAggroTarget = tar;
			if (InGroup && !GameData.GroupMatesInCombat.Contains(this))
			{
				GameData.GroupMatesInCombat.Add(this);
				if (targetReminderCD <= 0f)
				{
					GameData.SimPlayerGrouping.AddStringForDisplay(NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString(GameData.SimPlayerGrouping.Targeting[Random.Range(0, GameData.SimPlayerGrouping.Targeting.Count)] + " " + tar.GetComponent<NPC>().NPCName, ThisSim), "#00B2B7");
					targetReminderCD = Random.Range(120, 300);
				}
			}
		}
		else if (Random.Range(0, 10) > 7)
		{
			CurrentAggroTarget = tar;
		}
		if (!(tar != null) || !(Myself != null) || Myself.NearbyFriends == null || !(MyStats != null) || Myself.NearbyFriends.Count <= 0 || MyStats.Charmed)
		{
			return;
		}
		foreach (Character nearbyFriend in Myself.NearbyFriends)
		{
			if (nearbyFriend != null && nearbyFriend.GetComponent<NPC>() != null)
			{
				if (nearbyFriend.GetComponent<NPC>().CurrentAggroTarget == null && CheckLOS(nearbyFriend) && nearbyFriend.GetComponent<SimPlayer>() == null)
				{
					nearbyFriend.GetComponent<NPC>().CurrentAggroTarget = CurrentAggroTarget;
					nearbyFriend.GetComponent<NPC>().ManageAggro(1, CurrentAggroTarget);
				}
				else if (nearbyFriend.GetComponent<NPC>().CurrentAggroTarget == null && CheckLOS(nearbyFriend) && nearbyFriend.GetComponent<SimPlayer>() != null && nearbyFriend.isNPC && nearbyFriend.GetComponent<NPC>().InGroup && InGroup)
				{
					nearbyFriend.GetComponent<NPC>().CurrentAggroTarget = CurrentAggroTarget;
				}
			}
		}
	}

	public void ForceAggroOn(Character tar)
	{
		if (tar == null)
		{
			CurrentAggroTarget = null;
			return;
		}
		CurrentAggroTarget = tar;
		if (InGroup && Myself.Alive && !GameData.GroupMatesInCombat.Contains(this))
		{
			GameData.GroupMatesInCombat.Add(this);
			if (GameData.SimPlayerGrouping.MainTank != null && GameData.SimPlayerGrouping.MainTank == GameData.SimMngr.Sims[ThisSim.myIndex] && tar != null && ThisSim != null && (ThisSim.CurrentPullPhase == global::SimPlayer.PullPhases.NotPulling || ThisSim.CurrentPullPhase == global::SimPlayer.PullPhases.AttackTarget))
			{
				GameData.SimPlayerGrouping.AddStringForDisplay(NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString(GameData.SimPlayerGrouping.Targeting[Random.Range(0, GameData.SimPlayerGrouping.Targeting.Count)] + " " + tar.GetComponent<NPC>().NPCName, ThisSim), "#00B2B7");
			}
		}
	}

	public bool CheckIfBehind(Character _target)
	{
		Vector3 forward = _target.transform.forward;
		Vector3 normalized = (base.transform.position - _target.transform.position).normalized;
		if (Vector3.Dot(forward, normalized) > 0f)
		{
			return false;
		}
		return true;
	}

	public string CheckTargetInnateAvoidance(Character _target)
	{
		if (_target == null || _target.MyStats == null || _target.MyStats.CharacterClass == null || _target.MySkills == null || _target.MySkills.KnownSkills == null)
		{
			return "";
		}
		string result = "";
		Vector3 forward = _target.transform.forward;
		Vector3 normalized = (base.transform.position - _target.transform.position).normalized;
		if (!(Vector3.Dot(forward, normalized) > 0f))
		{
			return "";
		}
		if (Random.Range(0, 100) < _target.MySkills.GetAscensionRank("16263128") * 5)
		{
			return " parried the attack!";
		}
		if (_target.MySkills.KnownSkills.Count > 0)
		{
			int num = 0;
			int num2 = 0;
			int num3 = MyStats.Level - _target.MyStats.Level;
			if (num3 < 0)
			{
				num3 = 0;
			}
			if (_target.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByName("Dodge")))
			{
				num++;
			}
			if (_target.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByName("Dodge II")))
			{
				num++;
			}
			if (_target.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByName("Dodge III")))
			{
				num++;
			}
			if (_target.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByName("Dodge IV")))
			{
				num++;
			}
			if (GameData.PlayerControl.Myself == _target)
			{
				if (GameData.PlayerControl.CurrentTarget == Myself)
				{
					num++;
				}
			}
			else if (_target.MyStats.Myself.MyNPC != null && _target.MyStats.Myself.MyNPC.CurrentAggroTarget != null && _target.MyStats.Myself.MyNPC.CurrentAggroTarget == Myself)
			{
				num++;
			}
			if ((float)Random.Range(0, 100) < (float)(15 * num - num3 * 3) + (float)_target.MyStats.GetCurrentAgi() * (float)_target.MyStats.AgiScaleMod / 100f)
			{
				float num4 = (float)Random.Range(0, 100) + (float)MyStats.GetCurrentDex() * (float)MyStats.DexScaleMod / 1000f;
				if (Random.Range(5f, 20f) / 20f * ((float)_target.MyStats.Level + 5f + (float)_target.MyStats.GetCurrentAgi() * (float)_target.MyStats.AgiScaleMod / 100f) > num4)
				{
					result = " dodged the attack!";
				}
			}
			if (_target.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByName("Block")) && _target.MyStats != null && _target.MyStats.MyInv != null && _target.MyStats.MyInv.SecondaryShield)
			{
				num2++;
			}
			if (_target.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByName("Block II")) && _target.MyStats != null && _target.MyStats.MyInv != null && _target.MyStats.MyInv.SecondaryShield)
			{
				num2++;
			}
			if (_target.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByName("Block III")) && _target.MyStats != null && _target.MyStats.MyInv != null && _target.MyStats.MyInv.SecondaryShield)
			{
				num2++;
			}
			if (_target.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByName("Block IV")) && _target.MyStats != null && _target.MyStats.MyInv != null && _target.MyStats.MyInv.SecondaryShield)
			{
				num2++;
			}
			if (GameData.PlayerControl.Myself == _target && GameData.PlayerControl.CurrentTarget == Myself)
			{
				num2 += 2;
			}
			else if (_target.MyStats.Myself.MyNPC != null && _target.MyStats.Myself.MyNPC.CurrentAggroTarget != null && _target.MyStats.Myself.MyNPC.CurrentAggroTarget == Myself)
			{
				num2 += 2;
			}
			else if (num2 > 0)
			{
				num2--;
			}
			if ((float)Random.Range(0, 100) < (float)(15 * num2 - num3 * 3) - (float)_target.MyStats.GetCurrentDex() * (float)_target.MyStats.DexScaleMod / 500f)
			{
				float num5 = (float)Random.Range(0, 100) + (float)MyStats.GetCurrentDex() * (float)MyStats.DexScaleMod / 1000f;
				float num6 = Random.Range(5f, 20f) / 20f * ((float)_target.MyStats.Level + 5f + (float)_target.MyStats.GetCurrentDex() * (float)_target.MyStats.DexScaleMod / 500f);
				int num7 = 0;
				if (_target.MyStats.MyInv.OH != null && _target.MyStats.MyInv.OH.MyItem != null)
				{
					num7 = _target.MyStats.MyInv.OH.MyItem.ItemLevel;
				}
				else
				{
					Item item = (_target.MyStats?.MyInv?.SimOH)?.MyItem;
					if (item != null && item.Shield)
					{
						num7 = item.ItemLevel;
					}
				}
				if (_target.MyStats.CharacterClass == GameData.ClassDB.Paladin)
				{
					num6 += (float)Mathf.RoundToInt((float)num7 / 2f);
				}
				if (num6 > num5)
				{
					result = " blocked the attack!";
					if (_target.MyStats.MyInv.OH != null && _target.MyStats.MyInv.OH.MyItem != null && _target.MyStats.MyInv.OH.MyItem.WeaponProcOnHit != null && (float)Random.Range(0, 900) < _target.MyStats.MyInv.OH.MyItem.WeaponProcChance && _target.MySpells != null)
					{
						_target.MySpells.StartSpellFromProc(_target.MyStats.MyInv.OH.MyItem.WeaponProcOnHit, Myself.MyStats, 0.1f);
					}
					Item item2 = (_target.MyStats?.MyInv?.SimOH)?.MyItem;
					if (item2 != null && item2.WeaponProcOnHit != null && (float)Random.Range(0, 900) < item2.WeaponProcChance && _target.MySpells != null && Myself != null && Myself.MyStats != null)
					{
						_target.MySpells.StartSpellFromProc(item2.WeaponProcOnHit, Myself.MyStats, 0.1f);
					}
				}
			}
		}
		return result;
	}

	public bool NeedsNavUpdate(Vector3 _newDest)
	{
		_ = MyNav.destination;
		if (Vector3.Distance(MyNav.destination, _newDest) > 0.2f)
		{
			return true;
		}
		return false;
	}

	public void HighPriorityNavUpdate(Vector3 _newDest)
	{
		NavMeshAgent component = GetComponent<NavMeshAgent>();
		if (component == null || !component.isOnNavMesh || !NavMesh.SamplePosition(base.transform.position, out var hit, component.height * 1.5f, -1) || !NavMesh.SamplePosition(_newDest, out var hit2, 2f, -1))
		{
			return;
		}
		NavMeshPath navMeshPath = new NavMeshPath();
		if (NavMesh.CalculatePath(hit.position, hit2.position, -1, navMeshPath))
		{
			if (navMeshPath.status != 0 || navMeshPath.corners.Length < 2)
			{
				CountOffNav(navMeshPath);
				return;
			}
			component.ResetPath();
			component.SetPath(navMeshPath);
			component.destination = hit2.position;
			component.isStopped = false;
		}
	}

	private void UpdateNav()
	{
		if (errorCount <= 5 && !NavMesh.SamplePosition(base.transform.position, out var hit, 2f, -1) && !SimPlayer && Myself.Master == null)
		{
			if (MySpawnPoint != null)
			{
				if (errorCount == 4)
				{
					NavMesh.SamplePosition(base.transform.position, out hit, float.PositiveInfinity, -1);
					MyNav.Warp(hit.position);
					CurrentAggroTarget = null;
					errorCount++;
					return;
				}
				MyNav.Warp(MySpawnPoint.transform.position);
				CurrentAggroTarget = null;
			}
			else
			{
				NavMesh.SamplePosition(base.transform.position, out hit, float.PositiveInfinity, -1);
				MyNav.Warp(hit.position);
				CurrentAggroTarget = null;
			}
			errorCount++;
			return;
		}
		if (errorCount > 5)
		{
			MyStats.CurrentHP = 0;
		}
		if (!MyStats.Myself.Alive)
		{
			return;
		}
		if (!MyNav.enabled)
		{
			MyNav.enabled = true;
		}
		if (!MySpells.isCasting())
		{
			if (!MyStats.Rooted && !MyStats.Stunned && !MyStats.Feared && MyStats.RunSpeed > 0f)
			{
				if (CurrentAggroTarget != null)
				{
					InRange = Physics.OverlapSphere(base.transform.position, Myself.AttackRange, AggroMask);
					if (SimPlayer && (GetComponent<SimPlayer>().SeekPlayer || GetComponent<SimPlayer>().awaitInvite))
					{
						return;
					}
					if (!armedIdleAnim)
					{
						AnimOverride["Idle"] = ArmedIdle;
						armedIdleAnim = true;
					}
					if (!retreat)
					{
						if (SimPlayer && MyStats.CharacterClass != GameData.ClassDB.Paladin && MyStats.CharacterClass != GameData.ClassDB.Reaver && !MHWand && !MHBow && CurrentAggroTarget != null && CurrentAggroTarget.MyNPC != null && CurrentAggroTarget.MyNPC.CurrentAggroTarget != null && CurrentAggroTarget.MyNPC.CurrentAggroTarget != Myself)
						{
							float num = Mathf.Clamp(Vector3.Distance(base.transform.position, CurrentAggroTarget.transform.position), 2.5f, 4f);
							Vector3 sourcePosition = CurrentAggroTarget.GetNavPos() - CurrentAggroTarget.transform.forward * num;
							sourcePosition = ((!NavMesh.SamplePosition(sourcePosition, out var hit2, 1f, -1)) ? CurrentAggroTarget.transform.position : hit2.position);
							Vector2 a = new Vector2(Myself.TargetRing.transform.position.x, Myself.TargetRing.transform.position.z);
							Vector2 b = new Vector2(sourcePosition.x, sourcePosition.z);
							if ((Vector2.Distance(a, b) <= 2f && Vector2.Distance(a, b) > 0.2f) || MHWand || MHBow)
							{
								InPosition = true;
							}
							else
							{
								InPosition = false;
							}
						}
						else
						{
							InPosition = true;
						}
					}
					if (InRange != null && InRange.Length != 0 && InRange.Contains(CurrentAggroTarget.MyCap) && CheckLOS(CurrentAggroTarget) && !castingSpell && !retreat && InPosition)
					{
						if (MyNav.isOnNavMesh)
						{
							MyNav.isStopped = true;
						}
						MyAnim.SetBool("Walking", value: false);
						MyAnim.SetBool("Patrol", value: false);
						MyNav.velocity = Vector3.zero;
						inMeleeRange = true;
					}
					else if (!retreat)
					{
						MyNav.speed = MyStats.actualRunSpeed;
						if (MyNav.isOnNavMesh)
						{
							MyNav.isStopped = false;
						}
						MyAnim.SetBool("Walking", value: true);
						MyAnim.SetBool("Patrol", value: false);
						if (SimPlayer && MyStats.CharacterClass != GameData.ClassDB.Paladin && CurrentAggroTarget != null && CurrentAggroTarget.MyNPC != null && CurrentAggroTarget.MyNPC.CurrentAggroTarget != null && CurrentAggroTarget.MyNPC.CurrentAggroTarget != Myself)
						{
							float num2 = 1.8f;
							Vector3 newDest = CurrentAggroTarget.GetNavPos() - CurrentAggroTarget.transform.forward * num2;
							MyAnim.SetBool("Walking", value: true);
							MyAnim.SetBool("Patrol", value: false);
							HighPriorityNavUpdate(newDest);
						}
						else if (Vector3.Distance(MyNav.destination, CurrentAggroTarget.transform.position) > Myself.AttackRange)
						{
							HighPriorityNavUpdate(CurrentAggroTarget.transform.position);
						}
						inMeleeRange = false;
					}
					if (retreat && SimPlayer && ThisSim.MyPOI != null)
					{
						if (MyNav.isOnNavMesh)
						{
							MyNav.isStopped = false;
						}
						MyNav.enabled = true;
						MyAnim.SetBool("Walking", value: true);
						if (NeedsNavUpdate(ThisSim.MyPOI.transform.position))
						{
							MyNav.SetDestination(GameData.GetSafeNavMeshPoint(ThisSim.MyPOI.transform.position));
						}
					}
					if (retreat && ThisSim.MyPOI == null)
					{
						ThisSim.MyPOI = GameData.EgressLocations[Random.Range(0, GameData.EgressLocations.Count)];
					}
				}
				else if (!SimPlayer)
				{
					if (!MyStats.Charmed)
					{
						if (armedIdleAnim)
						{
							AnimOverride["Idle"] = RelaxedIdle;
							armedIdleAnim = false;
						}
						if (HailTimer > 0f)
						{
							if (MyNav.isOnNavMesh)
							{
								MyNav.isStopped = true;
							}
							MyAnim.SetBool("Patrol", value: false);
							MyAnim.SetBool("Walking", value: false);
							inMeleeRange = false;
							base.transform.LookAt(new Vector3(GameData.PlayerControl.transform.position.x, base.transform.position.y, GameData.PlayerControl.transform.position.z));
						}
						else if (PatrolPath.Count > 0)
						{
							if (Vector3.Distance(new Vector3(base.transform.position.x, PatrolPath[CurrentPatrolPoint].position.y, base.transform.position.z), PatrolPath[CurrentPatrolPoint].position) > 2f)
							{
								if (updatePatrolDel <= 0f)
								{
									if (NeedsNavUpdate(PatrolPath[CurrentPatrolPoint].position))
									{
										MyNav.SetDestination(GameData.GetSafeNavMeshPoint(PatrolPath[CurrentPatrolPoint].position));
									}
									MyNav.speed = MyStats.actualRunSpeed / 3f;
									if (MyNav.isOnNavMesh)
									{
										MyNav.isStopped = false;
									}
									MyAnim.SetBool("Walking", value: true);
									MyAnim.SetBool("Patrol", value: true);
								}
							}
							else
							{
								if (MyNav.isOnNavMesh)
								{
									MyNav.isStopped = true;
								}
								MyAnim.SetBool("Patrol", value: false);
								MyAnim.SetBool("Walking", value: false);
								if (updatePatrolDel == 0f && PatrolPath[CurrentPatrolPoint] != null)
								{
									updatePatrolDel = PatrolPath[CurrentPatrolPoint]?.GetComponent<NPCWaypoint>()?.PauseHere ?? 300f;
								}
								if (loopPatrol)
								{
									if (CurrentPatrolPoint + patrolDir > PatrolPath.Count - 1)
									{
										CurrentPatrolPoint = 0;
									}
									else
									{
										CurrentPatrolPoint += patrolDir;
									}
								}
								if (!loopPatrol)
								{
									if (CurrentPatrolPoint + patrolDir > PatrolPath.Count - 1 || CurrentPatrolPoint + patrolDir < 0)
									{
										patrolDir *= -1;
									}
									else
									{
										CurrentPatrolPoint += patrolDir;
									}
								}
							}
						}
						else if (wander > 0f)
						{
							if (updatePatrolDel <= 0f)
							{
								updatePatrolDel = 600f;
								NavMesh.SamplePosition(Random.insideUnitSphere * wander + MySpawnPoint.transform.position, out var hit3, wander, 1);
								if (NeedsNavUpdate(hit3.position))
								{
									MyNav.SetDestination(GameData.GetSafeNavMeshPoint(hit3.position));
								}
							}
							else if (Vector3.Distance(base.transform.position, MyNav.destination) > 2f * base.transform.localScale.y)
							{
								MyNav.speed = MyStats.actualRunSpeed / 2f;
								if (MyNav.isOnNavMesh)
								{
									MyNav.isStopped = false;
								}
								if (MyNav.desiredVelocity.sqrMagnitude > 0.1f && MyNav.velocity.sqrMagnitude > 0.1f)
								{
									MyAnim.SetBool("Walking", value: true);
									MyAnim.SetBool("Patrol", value: true);
								}
								else
								{
									MyAnim.SetBool("Walking", value: false);
									MyAnim.SetBool("Patrol", value: false);
								}
							}
							else if (MyNav.isOnNavMesh && !MyNav.isStopped)
							{
								if (MyNav.isOnNavMesh)
								{
									MyNav.isStopped = true;
								}
								MyNav.velocity = Vector3.zero;
								MyAnim.SetBool("Patrol", value: false);
								MyAnim.SetBool("Walking", value: false);
								updatePatrolDel = Random.Range(120, 360);
							}
						}
						else if (Vector3.Distance(base.transform.position, HomePos) > 1f)
						{
							if (MyStats.GetMHAtkDelay() <= 0f)
							{
								if (AggroTable.Count > 0)
								{
									ForceAggroOn(GetHighestAggro());
								}
								else
								{
									if (NeedsNavUpdate(HomePos))
									{
										MyNav.SetDestination(GetSafeNavMeshPoint(HomePos));
									}
									if (MyNav.isOnNavMesh)
									{
										MyNav.isStopped = false;
									}
									MyAnim.SetBool("Patrol", value: true);
									MyAnim.SetBool("Walking", value: true);
									MyNav.speed = MyStats.actualRunSpeed / 2f;
								}
							}
						}
						else
						{
							if (MyNav.isOnNavMesh)
							{
								MyNav.isStopped = true;
							}
							if (MyAnim.runtimeAnimatorController != null)
							{
								MyAnim.SetBool("Patrol", value: false);
								MyAnim.SetBool("Walking", value: false);
							}
							inMeleeRange = false;
							if (MySpawnPoint != null)
							{
								base.transform.rotation = MySpawnPoint.transform.rotation;
							}
						}
					}
					else if (Myself.Master != null)
					{
						if (Vector3.Distance(base.transform.position, Myself.Master.transform.position) > 5f)
						{
							if (NeedsNavUpdate(Myself.Master.transform.position))
							{
								HighPriorityNavUpdate(Myself.Master.transform.position);
							}
							if (MyNav.isOnNavMesh)
							{
								MyNav.isStopped = false;
							}
							if (Vector3.Distance(base.transform.position, Myself.Master.transform.position) < 13f)
							{
								MyAnim.SetBool("Patrol", value: true);
								MyAnim.SetBool("Walking", value: true);
								MyNav.speed = Myself.Master.MyStats.actualRunSpeed / 2f;
							}
							else
							{
								MyNav.speed = Myself.Master.MyStats.actualRunSpeed;
								MyAnim.SetBool("Walking", value: true);
								MyAnim.SetBool("Patrol", value: false);
							}
						}
						else
						{
							MyNav.velocity = Vector3.zero;
							if (MyNav.isOnNavMesh)
							{
								MyNav.isStopped = true;
							}
							MyAnim.SetBool("Patrol", value: false);
							MyAnim.SetBool("Walking", value: false);
						}
					}
				}
				else if (armedIdleAnim && CurrentAggroTarget == null)
				{
					AnimOverride["Idle"] = RelaxedIdle;
					armedIdleAnim = false;
				}
			}
			else if (MyStats.Rooted || MyStats.RunSpeed > 0f)
			{
				MyNav.velocity = Vector3.zero;
				if (MyNav.isOnNavMesh)
				{
					MyNav.isStopped = true;
				}
				MyAnim.SetBool("Patrol", value: false);
				MyAnim.SetBool("Walking", value: false);
				if (CurrentAggroTarget != null)
				{
					if (Vector3.Distance(base.transform.position, CurrentAggroTarget.transform.position) < Myself.AttackRange && CheckLOS(CurrentAggroTarget))
					{
						InRange = Physics.OverlapSphere(base.transform.position, Myself.AttackRange, AggroMask);
						inMeleeRange = true;
						base.transform.LookAt(new Vector3(CurrentAggroTarget.transform.position.x, base.transform.position.y, CurrentAggroTarget.transform.position.z));
					}
					else
					{
						InRange = Physics.OverlapSphere(base.transform.position, Myself.AttackRange, AggroMask);
						inMeleeRange = false;
						base.transform.LookAt(new Vector3(CurrentAggroTarget.transform.position.x, base.transform.position.y, CurrentAggroTarget.transform.position.z));
					}
				}
			}
		}
		if (Myself.NearbyDoors.Count <= 0)
		{
			return;
		}
		foreach (Door nearbyDoor in Myself.NearbyDoors)
		{
			if (Vector3.Distance(nearbyDoor.transform.position, base.transform.position) < 5f && nearbyDoor.isClosed && !nearbyDoor.swinging)
			{
				nearbyDoor.OpenOrShut();
			}
		}
	}

	private void LateUpdate()
	{
		if (MyNav != null && MyNav.isOnNavMesh && (MyNav.isStopped || MyNav.velocity == Vector3.zero || MyNav.speed == 0f))
		{
			MyAnim.SetBool("Patrol", value: false);
			MyAnim.SetBool("Walking", value: false);
		}
	}

	public Character GetClosestEnemy(List<Character> nearbyEnemies)
	{
		if (nearbyEnemies == null || nearbyEnemies.Count == 0)
		{
			return null;
		}
		Character result = null;
		float num = float.PositiveInfinity;
		Vector3 position = base.transform.position;
		foreach (Character nearbyEnemy in nearbyEnemies)
		{
			if (!(nearbyEnemy == null) && nearbyEnemy.Alive)
			{
				float sqrMagnitude = (nearbyEnemy.transform.position - position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					result = nearbyEnemy;
				}
			}
		}
		return result;
	}

	public bool CheckLiving()
	{
		return Myself.Alive;
	}

	public void ResetSpawnPoint()
	{
		if (MySpawnPoint != null)
		{
			MySpawnPoint.corpsePos = base.transform.position;
			MySpawnPoint.MyNPCAlive = false;
			MySpawnPoint.ResetSpawnPoint();
		}
	}

	private bool StormCallerImbueReady()
	{
		if (!SimPlayer)
		{
			return true;
		}
		if (SimPlayer && MyStats.CharacterClass != GameData.ClassDB.Stormcaller)
		{
			return true;
		}
		foreach (SimPlayerSkillSlot item in ThisSim.Skillbook)
		{
			if (item.skill.Id == "58018670" && item.CD <= 0f)
			{
				return true;
			}
		}
		return false;
	}

	private void DoAttackSpell()
	{
		if (CurrentAggroTarget == null || (!SimPlayer && NPCSpellCooldown > 0f) || MySpells.isCasting())
		{
			return;
		}
		bool flag = SimPlayer && ThisSim != null && GameData.SimPlayerGrouping.Heals.Contains(GameData.SimMngr.Sims[ThisSim.myIndex]);
		if ((SimPlayer && (bool)ThisSim && ThisSim.MyStats.CharacterClass == GameData.ClassDB.Paladin && MyStats.CurrentMana < Mathf.RoundToInt((float)MyStats.GetCurrentMaxMana() * 0.5f)) || (SimPlayer && (bool)ThisSim && ThisSim.MyStats.CharacterClass == GameData.ClassDB.Reaver && Random.Range(0, 10) > 4 && MyStats.StrScaleMod > MyStats.IntScaleMod) || (SimPlayer && (bool)ThisSim && ThisSim.MyStats.CharacterClass == GameData.ClassDB.Reaver && ThisSim.InGroup && GameData.SimPlayerGrouping.FearKite && MyStats.CurrentMana < Mathf.RoundToInt((float)MyStats.GetCurrentMaxMana() * 0.4f)) || !(atkSpellDelay <= 0f) || MyAttackSpells.Count <= 0 || ((float)MyStats.CurrentMana < (float)MyStats.GetCurrentMaxMana() / 2f && MyStats.CharacterClass == GameData.ClassDB.Druid && flag))
		{
			return;
		}
		Spell spell = null;
		if (CurrentAggroTarget.MyStats.Level >= MyStats.Level)
		{
			foreach (Spell item in MyAttackSpells.OrderByDescending((Spell sp) => sp.RequiredLevel))
			{
				if (!SimPlayer)
				{
					spell = item;
					break;
				}
				if ((item.TauntSpell && item.TargetDamage == 0 && ThisSim != null && ThisSim.MyStats.CharacterClass == GameData.ClassDB.Paladin && GameData.SimMngr.Sims[ThisSim.myIndex].Grouped && GameData.SimPlayerGrouping.MainTank != GameData.SimMngr.Sims[ThisSim.myIndex]) || (SimPlayer && item.FearTarget && !GameData.SimPlayerGrouping.FearKite && InGroup) || ThisSim == null || item.ManaCost > MyStats.CurrentMana || (item.CasterHealing < 0 && (float)item.CasterHealing * -4.5f > (float)MyStats.CurrentHP) || (ThisSim != null && (float)CurrentAggroTarget.GetCurHealthAsIntPercentage() > HoldDPS && item.Type == Spell.SpellType.Damage))
				{
					continue;
				}
				int spellIndexInBook = ThisSim.GetSpellIndexInBook(item);
				if (ThisSim.GetSpellCooldownByIndex(spellIndexInBook) <= 0f)
				{
					if (item != null && item.Type == Spell.SpellType.StatusEffect && !CurrentAggroTarget.MyStats.CheckForHigherLevelSEFromMeToRefresh(item, Myself))
					{
						spell = item;
						break;
					}
					if (item.Type != Spell.SpellType.StatusEffect && ((item.Type != Spell.SpellType.AE && item.Type != Spell.SpellType.PBAE) || item.TargetDamage <= 0 || (Myself.NearbyEnemies.Count >= 2 && (!SimPlayer || !ThisSim.InGroup || GameData.SimPlayerGrouping.CC.Count <= 0) && (!SimPlayer || !ThisSim.InGroup || !GameData.SimPlayerGrouping.PlayerIsCC))))
					{
						spell = item;
						break;
					}
				}
			}
		}
		else
		{
			foreach (Spell item2 in MyAttackSpells.OrderByDescending((Spell sp) => sp.TargetDamage))
			{
				if (!SimPlayer)
				{
					spell = item2;
					break;
				}
				if (ThisSim == null || item2.ManaCost > MyStats.CurrentMana || (SimPlayer && item2.FearTarget && !GameData.SimPlayerGrouping.FearKite && InGroup))
				{
					continue;
				}
				int spellIndexInBook2 = ThisSim.GetSpellIndexInBook(item2);
				if (ThisSim.GetSpellCooldownByIndex(spellIndexInBook2) <= 0f)
				{
					if (item2 != null && item2.Type == Spell.SpellType.StatusEffect && !CurrentAggroTarget.MyStats.CheckForHigherLevelSEFromMeToRefresh(item2, Myself))
					{
						spell = item2;
						break;
					}
					if (item2.Type != Spell.SpellType.StatusEffect && ((item2.Type != Spell.SpellType.AE && item2.Type != Spell.SpellType.PBAE) || (Myself.NearbyEnemies.Count >= 2 && (!SimPlayer || !ThisSim.InGroup || GameData.SimPlayerGrouping.CC.Count <= 0) && (!SimPlayer || !ThisSim.InGroup || !GameData.SimPlayerGrouping.PlayerIsCC))))
					{
						spell = item2;
						break;
					}
				}
			}
		}
		if (spell == null || !(CurrentAggroTarget != null) || !(Vector3.Distance(base.transform.position, CurrentAggroTarget.transform.position) < spell.SpellRange))
		{
			return;
		}
		if (Myself.MyCharmedNPC != null)
		{
			Myself.MyCharmedNPC.AggroOn(CurrentAggroTarget);
		}
		if (spell.Type == Spell.SpellType.StatusEffect && MyStats.GetCurrentMana() > spell.ManaCost && !CurrentAggroTarget.MyStats.CheckForHigherLevelSEFromMeToRefresh(spell, Myself) && (CurrentAggroTarget.MyStats.GetCurrentHP() < Mathf.RoundToInt((float)CurrentAggroTarget.MyStats.CurrentMaxHP * 0.9f) || CurrentAggroTarget.transform.name == "Training Dummy") && !CurrentAggroTarget.IsMezzed())
		{
			if (!SimPlayer)
			{
				atkSpellDelay = Random.Range(240, 300);
			}
			else if (SimPlayer)
			{
				atkSpellDelay = spell.SpellChargeTime + 60f;
			}
			else if (MyStats.CharacterClass == GameData.ClassDB.Paladin)
			{
				atkSpellDelay = Random.Range(50, 90);
			}
			if (MyNav.isOnNavMesh)
			{
				MyNav.isStopped = true;
			}
			MyAnim.SetBool("Walking", value: false);
			MyAnim.SetBool("Patrol", value: false);
			MySpells.StartSpell(spell, CurrentAggroTarget.MyStats);
			if (SimPlayer)
			{
				ThisSim.SetSpellCooldownBySpell(spell, spell.Cooldown + spell.SpellChargeTime / 60f);
			}
		}
		if ((spell.Type == Spell.SpellType.AE || spell.Type == Spell.SpellType.PBAE) && (Myself.NearbyEnemies.Count >= 2 || !SimPlayer) && MyStats.GetCurrentMana() > spell.ManaCost && !spell.CrowdControlSpell && (!SimPlayer || (SimPlayer && !ThisSim.InGroup) || (SimPlayer && ThisSim.InGroup && GameData.SimPlayerGrouping.CC.Count == 0 && !GameData.SimPlayerGrouping.PlayerIsCC) || spell.TargetDamage <= 0))
		{
			if (!SimPlayer)
			{
				atkSpellDelay = Random.Range(240, 300);
			}
			else if (SimPlayer)
			{
				atkSpellDelay = spell.SpellChargeTime + 60f;
			}
			else if (MyStats.CharacterClass == GameData.ClassDB.Paladin)
			{
				atkSpellDelay = Random.Range(50, 90);
			}
			if (MyNav.isOnNavMesh)
			{
				MyNav.isStopped = true;
			}
			MyAnim.SetBool("Walking", value: false);
			MyAnim.SetBool("Patrol", value: false);
			MySpells.StartSpell(spell, CurrentAggroTarget.MyStats);
			PlannedChantEnd = Random.Range(0.6f, 1f);
			if (PlannedChantEnd > 0.9f)
			{
				PlannedChantEnd = 1f;
			}
			if (SimPlayer)
			{
				ThisSim.SetSpellCooldownBySpell(spell, spell.Cooldown + spell.SpellChargeTime / 60f);
			}
		}
		if (spell.Type != 0 || MyStats.GetCurrentMana() <= spell.ManaCost || spell.CrowdControlSpell || (CurrentAggroTarget.MyStats != null && CurrentAggroTarget.MyStats.Level > MyStats.Level + 3 && (float)(CurrentAggroTarget.MyStats.CurrentHP / CurrentAggroTarget.MyStats.CurrentMaxHP) > 0.93f))
		{
			return;
		}
		if (!StormCallerImbueReady())
		{
			spell = null;
			return;
		}
		if (!SimPlayer)
		{
			atkSpellDelay = Random.Range(240, 300);
		}
		else if (SimPlayer)
		{
			atkSpellDelay = spell.SpellChargeTime + 60f;
		}
		else if (MyStats.CharacterClass == GameData.ClassDB.Paladin)
		{
			atkSpellDelay = Random.Range(50, 90);
		}
		if (MyNav.isOnNavMesh)
		{
			MyNav.isStopped = true;
		}
		MyAnim.SetBool("Walking", value: false);
		MyAnim.SetBool("Patrol", value: false);
		MySpells.StartSpell(spell, CurrentAggroTarget.MyStats);
		PlannedChantEnd = Random.Range(0.6f, 1f);
		if (PlannedChantEnd > 0.9f)
		{
			PlannedChantEnd = 1f;
		}
		if (SimPlayer)
		{
			ThisSim.SetSpellCooldownBySpell(spell, spell.Cooldown + spell.SpellChargeTime / 60f);
		}
	}

	public Character GetCurrentTarget()
	{
		return CurrentAggroTarget;
	}

	public void UpdateAnims()
	{
		if (TwoHandStaff)
		{
			if (TwoHandStaffIdle != null)
			{
				MyAnim.SetBool("1HSmall", value: false);
				AnimOverride["Idle"] = TwoHandStaffIdle;
				AnimOverride["Walk"] = TwoHandStaffWalk;
				AnimOverride["Armed-Run-Forward"] = TwoHandStaffRun;
				MyAnim.SetBool("2HStaff", value: true);
			}
			if (TwoHandSword)
			{
				MyAnim.SetBool("1HSmall", value: false);
				AnimOverride["Idle"] = TwoHandSwordIdle;
				MyAnim.SetBool("2HMelee", value: true);
			}
		}
	}

	public void AnimalSound()
	{
	}

	public void Hit()
	{
	}

	public void Shoot()
	{
	}

	public void Attacking()
	{
	}

	public void Eating()
	{
	}

	public void Walking()
	{
	}

	public Character GetChar()
	{
		return Myself;
	}

	public Character GetHighestAggro()
	{
		int num = 0;
		Character result = null;
		if (AggroTable.Count > 0)
		{
			foreach (AggroSlot item in AggroTable)
			{
				if (item.Hate > num && item.Player.Alive)
				{
					result = item.Player;
					num = item.Hate;
				}
			}
			return result;
		}
		return result;
	}

	public void ManageAggro(int _aggro, Character _attacker)
	{
		float num = 1f;
		if (_attacker.MyStats.CheckForSEByName("Embrace of Shadow"))
		{
			num *= 0.4f;
		}
		if (_attacker.MyStats.CheckForSEByName("Hint of Shadow"))
		{
			num *= 0.7f;
		}
		if (_attacker.MyStats.CheckForSEByName("Contagious Rage"))
		{
			num *= 2f;
		}
		if (_attacker.MySkills.HasAscension("16681322"))
		{
			float num2 = _attacker.MySkills.GetAscensionRank("16681322");
			num -= num * (0.05f * num2);
		}
		if (_attacker.MySkills.HasAscension("7757160"))
		{
			float num3 = _attacker.MySkills.GetAscensionRank("16681322");
			num -= num * (0.05f * num3);
		}
		if (_attacker.MySkills.HasAscension("54790542"))
		{
			float num4 = _attacker.MySkills.GetAscensionRank("54790542");
			num -= num * (0.05f * num4);
		}
		if (_attacker.MySkills.HasAscension("34378120"))
		{
			float num5 = _attacker.MySkills.GetAscensionRank("34378120");
			num += num * (0.33f * num5);
		}
		AggroSlot aggroSlot = GetAggroSlot(_attacker);
		if (aggroSlot != null)
		{
			if (_aggro > 1)
			{
				aggroSlot.Hate += Mathf.RoundToInt((float)_aggro * _attacker.MyStats.CharacterClass.AggroMod * num);
			}
		}
		else
		{
			AggroTable.Add(new AggroSlot(_attacker, _aggro));
		}
	}

	public AggroSlot GetAggroSlot(Character _attacker)
	{
		if (AggroTable.Count > 0)
		{
			foreach (AggroSlot item in AggroTable)
			{
				if (item.Player == _attacker)
				{
					return item;
				}
			}
		}
		return null;
	}

	public void SetThreatToAggro()
	{
		if (CurrentAggroTarget == null)
		{
			return;
		}
		bool? flag = Myself?.MyStats?.Charmed;
		if (!flag.HasValue || flag.GetValueOrDefault() || AggroTable.Count <= 0)
		{
			return;
		}
		foreach (AggroSlot item in AggroTable)
		{
			item.Player.UnderThreat = 60f;
		}
	}

	private void CrowdControl()
	{
		bool flag = false;
		if (MySpells.isCasting())
		{
			if (MyStats.CharacterClass == GameData.ClassDB.Arcanist && MySpells.GetCurrentCast().Type == Spell.SpellType.Damage)
			{
				flag = true;
			}
			if (!flag)
			{
				return;
			}
		}
		if (GameData.SimMngr.Sims[ThisSim.myIndex].Grouped)
		{
			if (!GameData.SimPlayerGrouping.CC.Contains(GameData.SimMngr.Sims[ThisSim.myIndex]))
			{
				return;
			}
			Combatants.Clear();
			if (MySpells.isCasting())
			{
				return;
			}
			foreach (Character nearbyEnemy in Myself.NearbyEnemies)
			{
				if (!Combatants.Contains(nearbyEnemy) && nearbyEnemy.MyNPC.CurrentAggroTarget != null && !nearbyEnemy.MyStats.Unstunnable)
				{
					if ((GameData.GroupMembers[0] != null && nearbyEnemy.MyNPC.CurrentAggroTarget == GameData.GroupMembers[0].MyAvatar.MyStats.Myself) || (GameData.GroupMembers[1] != null && nearbyEnemy.MyNPC.CurrentAggroTarget == GameData.GroupMembers[1].MyAvatar.MyStats.Myself) || (GameData.GroupMembers[2] != null && nearbyEnemy.MyNPC.CurrentAggroTarget == GameData.GroupMembers[2].MyAvatar.MyStats.Myself) || (GameData.GroupMembers[3] != null && nearbyEnemy.MyNPC.CurrentAggroTarget == GameData.GroupMembers[3].MyAvatar.MyStats.Myself) || (GameData.PlayerControl != null && nearbyEnemy.MyNPC.CurrentAggroTarget == GameData.PlayerControl.Myself))
					{
						Combatants.Add(nearbyEnemy);
					}
				}
				else if (nearbyEnemy != null && nearbyEnemy.MyStats.Unstunnable && nearbyEnemy.Alive && nearbyEnemy.MyNPC != null && nearbyEnemy.MyNPC.CurrentAggroTarget != null && GameData.SimPlayerGrouping.PlayerIsMA && GameData.PlayerControl.CurrentTarget != nearbyEnemy && UnstunnableWarnCD <= 0f)
				{
					UpdateSocialLog.LogAdd(NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString(GameData.PlayerStats.MyName.ToUpper() + "! " + nearbyEnemy.transform.name + " can't be stunned, get on that one ASAP!", ThisSim), "#00B2B7");
					UnstunnableWarnCD = Random.Range(60, 140);
				}
			}
			if (GameData.GroupMembers[0] != null && !GameData.GroupMembers[0].isPuller)
			{
				foreach (Character nearbyEnemy2 in GameData.GroupMembers[0].MyAvatar.MyStats.Myself.NearbyEnemies)
				{
					if (!Combatants.Contains(nearbyEnemy2) && nearbyEnemy2.MyNPC.CurrentAggroTarget != null && !nearbyEnemy2.MyStats.Unstunnable)
					{
						Combatants.Add(nearbyEnemy2);
					}
				}
			}
			if (GameData.GroupMembers[1] != null && !GameData.GroupMembers[1].isPuller)
			{
				foreach (Character nearbyEnemy3 in GameData.GroupMembers[1].MyAvatar.MyStats.Myself.NearbyEnemies)
				{
					if (!Combatants.Contains(nearbyEnemy3) && nearbyEnemy3.MyNPC.CurrentAggroTarget != null && !nearbyEnemy3.MyStats.Unstunnable)
					{
						Combatants.Add(nearbyEnemy3);
					}
				}
			}
			if (GameData.GroupMembers[2] != null && !GameData.GroupMembers[2].isPuller)
			{
				foreach (Character nearbyEnemy4 in GameData.GroupMembers[2].MyAvatar.MyStats.Myself.NearbyEnemies)
				{
					if (!Combatants.Contains(nearbyEnemy4) && nearbyEnemy4.MyNPC.CurrentAggroTarget != null && !nearbyEnemy4.MyStats.Unstunnable)
					{
						Combatants.Add(nearbyEnemy4);
					}
				}
			}
			if (GameData.GroupMembers[3] != null && !GameData.GroupMembers[3].isPuller)
			{
				foreach (Character nearbyEnemy5 in GameData.GroupMembers[3].MyAvatar.MyStats.Myself.NearbyEnemies)
				{
					if (!Combatants.Contains(nearbyEnemy5) && nearbyEnemy5.MyNPC.CurrentAggroTarget != null && !nearbyEnemy5.MyStats.Unstunnable)
					{
						Combatants.Add(nearbyEnemy5);
					}
				}
			}
			{
				foreach (Character combatant in Combatants)
				{
					if (((!(combatant != null) || !(combatant.MyNPC.CurrentAggroTarget != null) || combatant.IsMezzed() || !combatant.isNPC || !(Vector3.Distance(base.transform.position, combatant.transform.position) < 32f) || GameData.SimPlayerGrouping.Tanks.Count <= 0 || GameData.SimPlayerGrouping.Tanks[0] == null || !(GameData.SimPlayerGrouping.Tanks[0].MyAvatar.MyStats.Myself.MyNPC.CurrentAggroTarget != combatant)) && (GameData.SimPlayerGrouping.Tanks.Count != 0 || !(GameData.PlayerControl.CurrentTarget != combatant) || combatant.IsMezzed() || combatant.MyStats.Unstunnable)) || (GameData.SimPlayerGrouping.Puller != null && GameData.SimPlayerGrouping.Puller.MyAvatar != null && GameData.SimPlayerGrouping.Puller.MyAvatar.MyStats.Myself.MyNPC.CurrentAggroTarget == combatant) || Combatants.Count <= 1)
					{
						continue;
					}
					foreach (Spell myCCSpell in MyCCSpells)
					{
						if (ThisSim.GetSpellCooldownByIndex(ThisSim.GetSpellIndexInBook(myCCSpell)) > 0f || ThisSim.GetSpellCooldownByIndex(ThisSim.GetSpellIndexInBook(myCCSpell)) > 0f)
						{
							continue;
						}
						if (MySpells != null && MySpells.isCasting() && MySpells.GetCurrentCast() != null && MySpells.GetCurrentCast().CrowdControlSpell)
						{
							if (flag)
							{
								MySpells.EndCastWithScaling();
							}
							else
							{
								MySpells.InterruptCast();
							}
						}
						MySpells.StartSpell(myCCSpell, combatant.MyStats);
						if (SimPlayer)
						{
							ThisSim.SetSpellCooldownBySpell(myCCSpell, myCCSpell.Cooldown + myCCSpell.SpellChargeTime / 60f);
						}
						UpdateSocialLog.LogAdd(NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString("casting " + myCCSpell.SpellName + " on " + combatant.MyNPC.NPCName, ThisSim), "#00B2B7");
						return;
					}
				}
				return;
			}
		}
		if (!GameData.SimMngr.IsSimGrouped(GameData.SimMngr.Sims[ThisSim.myIndex]) || GameData.SimMngr.GetGroupLead(GameData.SimMngr.Sims[ThisSim.myIndex]) == GameData.SimMngr.Sims[ThisSim.myIndex].MyAvatar)
		{
			return;
		}
		Combatants.Clear();
		if (MySpells.isCasting())
		{
			return;
		}
		if (GameData.SimMngr.GetSimGroup(GameData.SimMngr.Sims[ThisSim.myIndex]).Members[0] != null && GameData.SimMngr.GetSimGroup(GameData.SimMngr.Sims[ThisSim.myIndex]).Members[0].MyAvatar != null)
		{
			foreach (Character nearbyEnemy6 in GameData.SimMngr.GetSimGroup(GameData.SimMngr.Sims[ThisSim.myIndex]).Members[0].MyAvatar.MyStats.Myself.NearbyEnemies)
			{
				if (!Combatants.Contains(nearbyEnemy6))
				{
					Combatants.Add(nearbyEnemy6);
				}
			}
		}
		if (GameData.SimMngr.GetSimGroup(GameData.SimMngr.Sims[ThisSim.myIndex]).Members.Count > 1 && GameData.SimMngr.GetSimGroup(GameData.SimMngr.Sims[ThisSim.myIndex]).Members[1] != null && GameData.SimMngr.GetSimGroup(GameData.SimMngr.Sims[ThisSim.myIndex]).Members[1].MyAvatar != null)
		{
			foreach (Character nearbyEnemy7 in GameData.SimMngr.GetSimGroup(GameData.SimMngr.Sims[ThisSim.myIndex]).Members[1].MyAvatar.MyStats.Myself.NearbyEnemies)
			{
				if (!Combatants.Contains(nearbyEnemy7))
				{
					Combatants.Add(nearbyEnemy7);
				}
			}
		}
		if (GameData.SimMngr.GetSimGroup(GameData.SimMngr.Sims[ThisSim.myIndex]).Members.Count > 2 && GameData.SimMngr.GetSimGroup(GameData.SimMngr.Sims[ThisSim.myIndex]).Members[2] != null && GameData.SimMngr.GetSimGroup(GameData.SimMngr.Sims[ThisSim.myIndex]).Members[2].MyAvatar != null)
		{
			foreach (Character nearbyEnemy8 in GameData.SimMngr.GetSimGroup(GameData.SimMngr.Sims[ThisSim.myIndex]).Members[2].MyAvatar.MyStats.Myself.NearbyEnemies)
			{
				if (!Combatants.Contains(nearbyEnemy8))
				{
					Combatants.Add(nearbyEnemy8);
				}
			}
		}
		if (GameData.SimMngr.GetSimGroup(GameData.SimMngr.Sims[ThisSim.myIndex]).Members.Count > 3 && GameData.SimMngr.GetSimGroup(GameData.SimMngr.Sims[ThisSim.myIndex]).Members[3] != null && GameData.SimMngr.GetSimGroup(GameData.SimMngr.Sims[ThisSim.myIndex]).Members[3].MyAvatar != null)
		{
			foreach (Character nearbyEnemy9 in GameData.SimMngr.GetSimGroup(GameData.SimMngr.Sims[ThisSim.myIndex]).Members[3].MyAvatar.MyStats.Myself.NearbyEnemies)
			{
				if (!Combatants.Contains(nearbyEnemy9))
				{
					Combatants.Add(nearbyEnemy9);
				}
			}
		}
		foreach (Character combatant2 in Combatants)
		{
			if (!Myself.NearbyEnemies.Contains(combatant2) || !(combatant2 != null) || !(combatant2.MyNPC.CurrentAggroTarget != null) || combatant2.IsMezzed() || !combatant2.isNPC || !(GameData.SimMngr.GetGroupLead(GameData.SimMngr.Sims[ThisSim.myIndex]) != null) || !(GameData.SimMngr.GetGroupLead(GameData.SimMngr.Sims[ThisSim.myIndex]).MyStats.Myself.MyNPC.CurrentAggroTarget != null) || !(GameData.SimMngr.GetGroupLead(GameData.SimMngr.Sims[ThisSim.myIndex]).MyStats.Myself.MyNPC.CurrentAggroTarget != combatant2))
			{
				continue;
			}
			foreach (Spell myCCSpell2 in MyCCSpells)
			{
				if (ThisSim.GetSpellCooldownByIndex(ThisSim.GetSpellIndexInBook(myCCSpell2)) > 0f)
				{
					continue;
				}
				if (MySpells != null && MySpells.isCasting() && MySpells.GetCurrentCast() != null && MySpells.GetCurrentCast().CrowdControlSpell)
				{
					if (flag)
					{
						MySpells.EndCastWithScaling();
					}
					else
					{
						MySpells.InterruptCast();
					}
				}
				MySpells.StartSpell(myCCSpell2, combatant2.MyStats);
				if (SimPlayer)
				{
					ThisSim.SetSpellCooldownBySpell(myCCSpell2, myCCSpell2.Cooldown + myCCSpell2.SpellChargeTime / 60f);
				}
				return;
			}
		}
	}

	private void SetBackupMA()
	{
		if (GameData.GroupMembers[0] != null && GameData.GroupMembers[0].MyStats != null && GameData.GroupMembers[0].MyStats.CharacterClass != GameData.ClassDB.Arcanist && GameData.GroupMembers[0].MyStats.CurrentHP > 0)
		{
			GameData.SimPlayerGrouping.MainAssist = GameData.GroupMembers[0];
			GameData.SimPlayerGrouping.PlayerIsMA = false;
			UpdateSocialLog.LogAdd(GameData.SimPlayerGrouping.GetMA() + " tells the group: " + GameData.SimMngr.PersonalizeString("I am the new Main Assist, assist me!", ThisSim), "#00B2B7");
		}
		else if (GameData.GroupMembers[1] != null && GameData.GroupMembers[1].MyStats != null && GameData.GroupMembers[1].MyStats.CharacterClass != GameData.ClassDB.Arcanist && GameData.GroupMembers[1].MyStats.CurrentHP > 0)
		{
			GameData.SimPlayerGrouping.MainAssist = GameData.GroupMembers[1];
			GameData.SimPlayerGrouping.PlayerIsMA = false;
			UpdateSocialLog.LogAdd(GameData.SimPlayerGrouping.GetMA() + " tells the group: " + GameData.SimMngr.PersonalizeString("I am the new Main Assist, assist me!", ThisSim), "#00B2B7");
		}
		else if (GameData.GroupMembers[2] != null && GameData.GroupMembers[2].MyStats != null && GameData.GroupMembers[2].MyStats.CharacterClass != GameData.ClassDB.Arcanist && GameData.GroupMembers[2].MyStats.CurrentHP > 0)
		{
			GameData.SimPlayerGrouping.MainAssist = GameData.GroupMembers[2];
			GameData.SimPlayerGrouping.PlayerIsMA = false;
			UpdateSocialLog.LogAdd(GameData.SimPlayerGrouping.GetMA() + " tells the group: " + GameData.SimMngr.PersonalizeString("I am the new Main Assist, assist me!", ThisSim), "#00B2B7");
		}
		else if (GameData.GroupMembers[3] != null && GameData.GroupMembers[3].MyStats != null && GameData.GroupMembers[3].MyStats.CharacterClass != GameData.ClassDB.Arcanist && GameData.GroupMembers[3].MyStats.CurrentHP > 0)
		{
			GameData.SimPlayerGrouping.MainAssist = GameData.GroupMembers[3];
			GameData.SimPlayerGrouping.PlayerIsMA = false;
			UpdateSocialLog.LogAdd(GameData.SimPlayerGrouping.GetMA() + " tells the group: " + GameData.SimMngr.PersonalizeString("I am the new Main Assist, assist me!", ThisSim), "#00B2B7");
		}
	}

	private void ResetToDesignatedMA()
	{
		if (((GameData.SimPlayerGrouping.DesignatedMA != null && GameData.SimPlayerGrouping.MainAssist == null) || (GameData.SimPlayerGrouping.DesignatedMA != null && GameData.SimPlayerGrouping.DesignatedMA != GameData.SimPlayerGrouping.MainAssist)) && GameData.SimPlayerGrouping.DesignatedMA.MyStats != null && GameData.SimPlayerGrouping.DesignatedMA.MyStats.CurrentHP > 0)
		{
			GameData.SimPlayerGrouping.MainAssist = GameData.SimPlayerGrouping.DesignatedMA;
			GameData.SimPlayerGrouping.PlayerIsMA = false;
			UpdateSocialLog.LogAdd(GameData.SimPlayerGrouping.GetMA() + " tells the group: " + GameData.SimMngr.PersonalizeString("Main assist will be me again, assist me!", ThisSim), "#00B2B7");
		}
		if (GameData.SimPlayerGrouping.PlayerIsDesignatedMA && !GameData.SimPlayerGrouping.PlayerIsMA && GameData.PlayerStats.CurrentHP > 0)
		{
			UpdateSocialLog.LogAdd(GameData.SimPlayerGrouping.GetMA() + " tells the group: " + GameData.SimMngr.PersonalizeString("Giving Main Assist back to " + GameData.PlayerStats.MyName + ".", ThisSim), "#00B2B7");
			GameData.SimPlayerGrouping.MainAssist = null;
			GameData.SimPlayerGrouping.PlayerIsMA = true;
		}
	}

	private void CheckAssist()
	{
		if ((ThisSim != null && ThisSim.CurrentPullPhase != 0) || MyStats.Invisible)
		{
			return;
		}
		if (GameData.SimPlayerGrouping.IsMADead())
		{
			if (!GameData.SimPlayerGrouping.IsMTDead())
			{
				if (GameData.SimPlayerGrouping.MainTank != null)
				{
					if (GameData.SimPlayerGrouping.MainAssist != GameData.SimPlayerGrouping.MainTank && GameData.SimPlayerGrouping.MainTank != null)
					{
						GameData.SimPlayerGrouping.PlayerIsMA = false;
						GameData.SimPlayerGrouping.MainAssist = GameData.SimPlayerGrouping.MainTank;
						UpdateSocialLog.LogAdd(GameData.SimPlayerGrouping.GetMA() + " tells the group: " + GameData.SimMngr.PersonalizeString("I am the new Main Assist, assit me!", ThisSim), "#00B2B7");
					}
				}
				else if (GameData.SimPlayerGrouping.PlayerIsTank && !GameData.SimPlayerGrouping.PlayerIsMA)
				{
					GameData.SimPlayerGrouping.PlayerIsMA = true;
					UpdateSocialLog.LogAdd(GameData.SimPlayerGrouping.GetMA() + " tells the group: " + GameData.SimMngr.PersonalizeString("I'm dead, assist " + GameData.PlayerStats.MyName + "!", ThisSim), "#00B2B7");
					GameData.SimPlayerGrouping.MainAssist = null;
				}
			}
			else if (GameData.PlayerStats.CurrentHP > 0 && GameData.PlayerStats.CharacterClass != GameData.ClassDB.Arcanist)
			{
				if (!GameData.SimPlayerGrouping.PlayerIsMA)
				{
					GameData.SimPlayerGrouping.PlayerIsMA = true;
					UpdateSocialLog.LogAdd(GameData.SimPlayerGrouping.GetMA() + " tells the group: " + GameData.SimMngr.PersonalizeString("I'm dead, assist " + GameData.PlayerStats.MyName + "!", ThisSim), "#00B2B7");
					GameData.SimPlayerGrouping.MainAssist = null;
				}
			}
			else
			{
				SetBackupMA();
			}
		}
		ResetToDesignatedMA();
		if (GameData.SimPlayerGrouping.PlayerIsMA)
		{
			GameData.SimPlayerGrouping.MainAssist = null;
		}
		if (GameData.SimPlayerGrouping.Puller != null && GameData.SimPlayerGrouping.Puller == GameData.SimPlayerGrouping.MainAssist && GameData.SimPlayerGrouping.MainAssist.MyAvatar.MyStats.Myself.MyNPC.CurrentAggroTarget != null && GameData.SimPlayerGrouping.Puller.MyAvatar != null && ThisSim.CurrentPullPhase != 0)
		{
			return;
		}
		if (GameData.SimPlayerGrouping.MainAssist != null && GameData.SimPlayerGrouping.MainAssist != GameData.SimMngr.Sims[ThisSim.myIndex])
		{
			if (GameData.SimPlayerGrouping.MainAssist != null && GameData.SimPlayerGrouping.MainAssist.MyAvatar != null && GameData.SimPlayerGrouping.MainAssist.MyAvatar.MyStats.Myself.MyNPC.CurrentAggroTarget != null && !GameData.SimPlayerGrouping.MainAssist.MyAvatar.MyStats.Myself.MyNPC.CurrentAggroTarget.MyStats.IsFullHP() && CurrentAggroTarget != GameData.SimPlayerGrouping.MainAssist.MyAvatar.MyStats.Myself.MyNPC.CurrentAggroTarget && Myself.NearbyEnemies.Contains(GameData.SimPlayerGrouping.MainAssist.MyAvatar.MyStats.Myself.MyNPC.CurrentAggroTarget))
			{
				CurrentAggroTarget = GameData.SimPlayerGrouping.MainAssist.MyAvatar.MyStats.Myself.MyNPC.CurrentAggroTarget;
				if (spamCD <= 0f)
				{
					spamCD = 100f;
					UpdateSocialLog.LogAdd(NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString("assisting " + GameData.SimPlayerGrouping.MainAssist.SimName + " on " + CurrentAggroTarget.transform.name, ThisSim), "#00B2B7");
					ThisSim.ClearPullTarget();
					ThisSim.IgnoreAllCombat = false;
				}
			}
		}
		else
		{
			if ((GameData.SimPlayerGrouping.MainAssist != null && !string.IsNullOrEmpty(GameData.SimPlayerGrouping.MainAssist.SimName)) || !(GameData.PlayerControl.CurrentTarget != null) || GameData.PlayerControl.CurrentTarget.MyStats.Charmed || !(GameData.PlayerControl.CurrentTarget.MyNPC != null) || !(GameData.PlayerControl.CurrentTarget.MyNPC.CurrentAggroTarget != null) || (!GameData.PlayerControl.CurrentTarget.MyStats.RecentlyDamagedByPlayer() && GameData.PlayerControl.CurrentTarget.MyStats.Stunned) || !(CurrentAggroTarget != GameData.PlayerControl.CurrentTarget) || !Myself.NearbyEnemies.Contains(GameData.PlayerControl.CurrentTarget))
			{
				return;
			}
			CurrentAggroTarget = GameData.PlayerControl.CurrentTarget;
			if (ThisSim.CurrentPullPhase == global::SimPlayer.PullPhases.NotPulling)
			{
				ThisSim.ClearPullTarget();
				ThisSim.IgnoreAllCombat = false;
			}
			if (spamCD <= 0f)
			{
				spamCD = 240f;
				switch (Random.Range(0, 10))
				{
				case 0:
					UpdateSocialLog.LogAdd(NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString("assisting " + GameData.PlayerStats.MyName + " on " + CurrentAggroTarget.transform.name, ThisSim), "#00B2B7");
					break;
				case 1:
					UpdateSocialLog.LogAdd(NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString("I'm on " + CurrentAggroTarget.transform.name + " with " + GameData.PlayerStats.MyName, ThisSim), "#00B2B7");
					break;
				case 2:
					UpdateSocialLog.LogAdd(NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString("Killing " + CurrentAggroTarget.transform.name + "!", ThisSim), "#00B2B7");
					break;
				case 3:
					UpdateSocialLog.LogAdd(NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString(GameData.PlayerStats.MyName + "'s target is " + CurrentAggroTarget.transform.name + ", killing it now!", ThisSim), "#00B2B7");
					break;
				case 4:
					UpdateSocialLog.LogAdd(NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString("assisting " + GameData.PlayerStats.MyName + "!", ThisSim), "#00B2B7");
					break;
				case 5:
					UpdateSocialLog.LogAdd(NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString("on " + CurrentAggroTarget.transform.name + "!", ThisSim), "#00B2B7");
					break;
				case 6:
					UpdateSocialLog.LogAdd(NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString("Assisting! I'm on " + CurrentAggroTarget.transform.name + "!", ThisSim), "#00B2B7");
					break;
				case 7:
					UpdateSocialLog.LogAdd(NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString(GameData.PlayerStats.MyName + " is on " + CurrentAggroTarget.transform.name + " and so am I!", ThisSim), "#00B2B7");
					break;
				case 8:
					UpdateSocialLog.LogAdd(NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString("assisting " + GameData.PlayerStats.MyName + ", watch for adds!", ThisSim), "#00B2B7");
					break;
				case 9:
					UpdateSocialLog.LogAdd(NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString(GameData.PlayerStats.MyName + " is the assist! I'm on " + CurrentAggroTarget.transform.name, ThisSim), "#00B2B7");
					break;
				default:
					UpdateSocialLog.LogAdd(NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString("assisting " + GameData.PlayerStats.MyName + " on " + CurrentAggroTarget.transform.name, ThisSim), "#00B2B7");
					break;
				}
			}
		}
	}

	private void CheckTaunt()
	{
		if (SimPlayer && ThisSim != null && (ThisSim.IgnoreAllCombat || MyStats.Invisible))
		{
			return;
		}
		if (SimPlayer && CurrentAggroTarget != null && Vector3.Distance(base.transform.position, CurrentAggroTarget.transform.position) < 30f && CurrentAggroTarget.isNPC && CurrentAggroTarget.Alive && CurrentAggroTarget.MyNPC.CurrentAggroTarget != null && (CurrentAggroTarget.MyNPC.CurrentAggroTarget != Myself || (CurrentAggroTarget != null && (CurrentAggroTarget.MyStats.Level - MyStats.Level > 4 || CurrentAggroTarget.BossXp > 1f || Random.Range(0, 100) < 5))) && ((GameData.SimPlayerGrouping.MainTank != null && GameData.SimMngr.Sims[ThisSim.myIndex].Grouped && GameData.SimPlayerGrouping.MainTank == GameData.SimMngr.Sims[ThisSim.myIndex]) || !GameData.SimMngr.Sims[ThisSim.myIndex].Grouped))
		{
			foreach (Spell item in MyTauntSpell)
			{
				if (!(ThisSim.GetSpellCooldownByIndex(ThisSim.GetSpellIndexInBook(item)) > 0f) && (item.Type != Spell.SpellType.PBAE || GameData.SimPlayerGrouping.CC.Count <= 0))
				{
					MySpells.StartSpell(item, CurrentAggroTarget.MyStats);
					if (SimPlayer)
					{
						ThisSim.SetSpellCooldownBySpell(item, item.Cooldown + item.SpellChargeTime / 60f);
						tauntCD = item.Cooldown * 45f;
					}
					if (GameData.SimMngr.Sims[ThisSim.myIndex].Grouped && Random.Range(0, 10) > 5)
					{
						UpdateSocialLog.LogAdd(NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString("taunting " + CurrentAggroTarget.transform.name + "!", ThisSim), "#00B2B7");
					}
					break;
				}
			}
		}
		if (!SimPlayer || !(CurrentAggroTarget != null) || Myself.NearbyEnemies.Count <= 1 || ((GameData.SimPlayerGrouping.MainTank == null || !GameData.SimMngr.Sims[ThisSim.myIndex].Grouped || GameData.SimPlayerGrouping.MainTank != GameData.SimMngr.Sims[ThisSim.myIndex]) && GameData.SimMngr.Sims[ThisSim.myIndex].Grouped && !GameData.SimMngr.IsSimGrouped(GameData.SimMngr.Sims[ThisSim.myIndex])))
		{
			return;
		}
		if (AETaunt != null)
		{
			int num = 0;
			bool flag = true;
			foreach (Character nearbyEnemy in Myself.NearbyEnemies)
			{
				if (nearbyEnemy != null && nearbyEnemy.MyNPC != null && nearbyEnemy.MyNPC.CurrentAggroTarget != null && nearbyEnemy.MyNPC.CurrentAggroTarget != Myself && !nearbyEnemy.MyStats.Stunned)
				{
					num++;
				}
			}
			if (ThisSim.GetSpellCooldownByIndex(ThisSim.GetSpellIndexInBook(AETaunt)) > 0f)
			{
				flag = false;
			}
			if (flag && num > 1)
			{
				MySpells.StartSpell(AETaunt, MyStats);
				if (SimPlayer)
				{
					if (ThisSim.InGroup && Random.Range(0, 10) > 3)
					{
						UpdateSocialLog.LogAdd(NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString("AE Taunting! Heals on me!", ThisSim), "#00B2B7");
					}
					ThisSim.SetSpellCooldownBySpell(AETaunt, AETaunt.Cooldown + AETaunt.SpellChargeTime / 60f);
				}
				return;
			}
		}
		foreach (Character nearbyEnemy2 in Myself.NearbyEnemies)
		{
			if (!(nearbyEnemy2 != null) || !nearbyEnemy2.Alive || !(nearbyEnemy2.MyNPC != null) || !(nearbyEnemy2.MyNPC.CurrentAggroTarget != null) || !(nearbyEnemy2.MyStats != null) || !(nearbyEnemy2.MyNPC.CurrentAggroTarget != Myself) || (nearbyEnemy2.MyNPC.CurrentAggroTarget.MyFaction != Character.Faction.PC && nearbyEnemy2.MyNPC.CurrentAggroTarget.MyFaction != 0))
			{
				continue;
			}
			foreach (Spell item2 in MyTauntSpell)
			{
				if (!(ThisSim.GetSpellCooldownByIndex(ThisSim.GetSpellIndexInBook(item2)) > 0f))
				{
					MySpells.StartSpell(item2, nearbyEnemy2.MyStats);
					if (SimPlayer)
					{
						ThisSim.SetSpellCooldownBySpell(item2, item2.Cooldown + item2.SpellChargeTime / 60f);
						tauntCD = item2.Cooldown * 45f;
					}
					if (ThisSim.InGroup && Random.Range(0, 10) > 7)
					{
						UpdateSocialLog.LogAdd(NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString("taunting " + nearbyEnemy2.transform.name + ", stay on your current target!", ThisSim), "#00B2B7");
					}
					break;
				}
			}
		}
	}

	private void CheckFearKite()
	{
		if (!(MySpells != null) || MySpells.isCasting() || !SimPlayer || !(CurrentAggroTarget != null) || !(CurrentAggroTarget.BossXp <= 1f) || !(ThisSim != null) || ThisSim.CurrentPullPhase != 0 || !GameData.SimPlayerGrouping.FearKite || !(Vector3.Distance(CurrentAggroTarget.transform.position, base.transform.position) < 20f) || (!GameData.SimMngr.Sims[ThisSim.myIndex].Grouped && GameData.SimMngr.Sims[ThisSim.myIndex].Grouped))
		{
			return;
		}
		Spell snareSpell = ThisSim.SnareSpell;
		Spell fearSpell = ThisSim.FearSpell;
		if (snareSpell != null && MyStats.CurrentMana > snareSpell.ManaCost && !CurrentAggroTarget.MyStats.CheckForStatus(snareSpell) && ThisSim.GetSpellCooldownByIndex(ThisSim.GetSpellIndexInBook(snareSpell)) <= 0f)
		{
			if (MyNav.isOnNavMesh)
			{
				MyNav.isStopped = true;
			}
			MyAnim.SetBool("Walking", value: false);
			MyAnim.SetBool("Patrol", value: false);
			MySpells.StartSpell(snareSpell, CurrentAggroTarget.MyStats);
			if (SimPlayer)
			{
				ThisSim.SetSpellCooldownBySpell(snareSpell, snareSpell.Cooldown + snareSpell.SpellChargeTime / 60f);
			}
			if (GameData.SimMngr.Sims[ThisSim.myIndex].Grouped && Random.Range(0, 10) > 7)
			{
				UpdateSocialLog.LogAdd(NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString("casting " + snareSpell.SpellName + " on " + CurrentAggroTarget.transform.name, ThisSim), "#00B2B7");
			}
		}
		else if (fearSpell != null && MyStats.CurrentMana > fearSpell.ManaCost && !CurrentAggroTarget.MyStats.CheckForStatus(fearSpell) && ThisSim.GetSpellCooldownByIndex(ThisSim.GetSpellIndexInBook(fearSpell)) <= 0f)
		{
			MySpells.StartSpell(fearSpell, CurrentAggroTarget.MyStats);
			if (SimPlayer)
			{
				ThisSim.SetSpellCooldownBySpell(fearSpell, fearSpell.Cooldown + fearSpell.SpellChargeTime / 60f);
			}
			if (GameData.SimMngr.Sims[ThisSim.myIndex].Grouped)
			{
				UpdateSocialLog.LogAdd(NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString(fearSpell.SpellName + " incoming on " + CurrentAggroTarget.transform.name + ", get ready to chase it!", ThisSim), "#00B2B7");
			}
		}
	}

	private void CheckSnareSpell()
	{
		if (ThisSim == null || !(MySpells != null) || MySpells.isCasting() || !SimPlayer || !(CurrentAggroTarget != null) || Myself.NearbyEnemies.Count != 1 || !(ThisSim != null) || ThisSim.CurrentPullPhase != 0 || !(Vector3.Distance(CurrentAggroTarget.transform.position, base.transform.position) < 20f) || (!GameData.SimMngr.Sims[ThisSim.myIndex].Grouped && GameData.SimMngr.Sims[ThisSim.myIndex].Grouped) || CurrentAggroTarget.MyFaction == Character.Faction.Undead || CurrentAggroTarget.BossXp > 1f || (float)CurrentAggroTarget.MyStats.CurrentHP > (float)CurrentAggroTarget.MyStats.CurrentMaxHP / 2f)
		{
			return;
		}
		Spell snareSpell = ThisSim.SnareSpell;
		if (snareSpell != null && MyStats.CurrentMana > snareSpell.ManaCost && !CurrentAggroTarget.MyStats.CheckForStatus(snareSpell) && ThisSim.GetSpellCooldownByIndex(ThisSim.GetSpellIndexInBook(snareSpell)) <= 0f)
		{
			if (MyNav.isOnNavMesh)
			{
				MyNav.isStopped = true;
			}
			MyAnim.SetBool("Walking", value: false);
			MyAnim.SetBool("Patrol", value: false);
			MySpells.StartSpell(snareSpell, CurrentAggroTarget.MyStats);
			if (SimPlayer)
			{
				ThisSim.SetSpellCooldownBySpell(snareSpell, snareSpell.Cooldown + snareSpell.SpellChargeTime / 60f);
			}
			if (GameData.SimMngr.Sims[ThisSim.myIndex].Grouped && Random.Range(0, 10) > 7)
			{
				UpdateSocialLog.LogAdd(NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString("casting " + snareSpell.SpellName + " on " + CurrentAggroTarget.transform.name + " in case it runs", ThisSim), "#00B2B7");
			}
		}
	}

	private void CheckPet()
	{
		if (Myself.MyCharmedNPC == null && ((SceneManager.GetActiveScene().name != "Azure" && SceneManager.GetActiveScene().name != "Reliquary") || ((SceneManager.GetActiveScene().name == "Azure" || SceneManager.GetActiveScene().name == "Reliquary") && Random.Range(0, 400) > 398)))
		{
			if (MyNav.isOnNavMesh)
			{
				MyNav.isStopped = true;
			}
			MyAnim.SetBool("Walking", value: false);
			MyAnim.SetBool("Patrol", value: false);
			MySpells.StartSpell(MyPetSpell, MyStats);
		}
	}

	public void AlertToEnvDmg()
	{
		TakingEnvironmentalDamage = true;
		takingEnvDmg = 360f;
		EventNode eventNode = InZoneEvents.FindNearestSafe(base.transform.position);
		if (eventNode != null)
		{
			MyNav.SetDestination(GameData.GetSafeNavMeshPoint(eventNode.transform.position));
		}
		if (SimPlayer && GameData.SimMngr.Sims[ThisSim.myIndex].Grouped)
		{
			UpdateSocialLog.LogAdd(NPCName + " tells the group: " + GameData.SimMngr.PersonalizeString(GameData.SimLang.GetEnvDmg(), ThisSim), "#00B2B7");
		}
		if (MySpells.isCasting())
		{
			MySpells.InterruptCast();
		}
	}

	private void MonitorEnvDmg()
	{
		if (TakingEnvironmentalDamage)
		{
			if (takingEnvDmg > 0f)
			{
				takingEnvDmg -= 1f;
			}
			else
			{
				TakingEnvironmentalDamage = false;
			}
		}
	}

	public SpawnPoint GetSpawnPoint()
	{
		return MySpawnPoint;
	}

	private void HandleNameTag()
	{
		if (Myself.Alive)
		{
			if (!SimPlayer)
			{
				if (GameData.ShowNPCName == 0)
				{
					if (!NamePlateTxt.enabled && Vector3.Distance(base.transform.position, GameData.GameCamPos.position) < (float)GameData.NamePlateDrawDist)
					{
						NamePlateTxt.enabled = true;
					}
					else if (NamePlateTxt.enabled && Vector3.Distance(base.transform.position, GameData.GameCamPos.position) > (float)GameData.NamePlateDrawDist)
					{
						NamePlateTxt.enabled = false;
					}
				}
				else if (GameData.ShowNPCName == 1)
				{
					if (GameData.PlayerControl.CurrentTarget == Myself && !NamePlateTxt.enabled)
					{
						NamePlateTxt.enabled = true;
					}
					else if (GameData.PlayerControl.CurrentTarget != Myself && NamePlateTxt.enabled)
					{
						NamePlateTxt.enabled = false;
					}
				}
				else if (GameData.ShowNPCName == 2 && NamePlateTxt.enabled)
				{
					NamePlateTxt.enabled = false;
				}
			}
			else if (GameData.ShowSimPlayerName == 0)
			{
				if (!NamePlateTxt.enabled && Vector3.Distance(base.transform.position, GameData.GameCamPos.position) < (float)GameData.NamePlateDrawDist)
				{
					NamePlateTxt.enabled = true;
				}
				else if (NamePlateTxt.enabled && Vector3.Distance(base.transform.position, GameData.GameCamPos.position) > (float)GameData.NamePlateDrawDist)
				{
					NamePlateTxt.enabled = false;
				}
			}
			else if (GameData.ShowSimPlayerName == 1)
			{
				if (GameData.PlayerControl.CurrentTarget == Myself && !NamePlateTxt.enabled)
				{
					NamePlateTxt.enabled = true;
				}
				else if (GameData.PlayerControl.CurrentTarget != Myself && NamePlateTxt.enabled)
				{
					NamePlateTxt.enabled = false;
				}
			}
			else if (GameData.ShowSimPlayerName == 2 && NamePlateTxt.enabled)
			{
				NamePlateTxt.enabled = false;
			}
		}
		else if (GameData.HideCorpseNametag && !SimPlayer && NamePlate.gameObject.activeSelf)
		{
			NamePlate.gameObject.SetActive(value: false);
		}
	}

	private IEnumerator NavUpdate(float waitSec)
	{
		while (Myself.Alive)
		{
			if (spawnCD <= 0f && (!SimPlayer || (SimPlayer && !GameData.SimMngr.Sims[ThisSim.myIndex].isPuller) || (GameData.SimMngr.Sims[ThisSim.myIndex].isPuller && !ThisSim.IgnoreAllCombat)))
			{
				if (Mobile && !MyStats.Stunned && !MyStats.Rooted && !MyStats.Feared)
				{
					if (!offNav && !Leashing)
					{
						if (TakingEnvironmentalDamage)
						{
							if (MyNav.isOnNavMesh)
							{
								MyNav.isStopped = false;
							}
							MyNav.enabled = true;
							MyAnim.SetBool("Walking", value: true);
							if (NeedsNavUpdate(InZoneEvents.FindNearestSafe(base.transform.position).transform.position))
							{
								MyNav.SetDestination(InZoneEvents.FindNearestSafe(base.transform.position).transform.position);
							}
							if (Vector3.Distance(base.transform.position, InZoneEvents.FindNearestSafe(base.transform.position).transform.position) < 3f)
							{
								TakingEnvironmentalDamage = false;
							}
						}
						else
						{
							UpdateNav();
						}
					}
					else if (offNav)
					{
						if (CurrentAggroTarget != null)
						{
							inMeleeRange = true;
							MyNav.Warp(CurrentAggroTarget.transform.position);
							if (IsAgentOnNavMesh(base.gameObject))
							{
								MyNav.enabled = true;
								offNavTolerance = 60f;
								offNav = false;
							}
						}
						else
						{
							MyNav.Warp(MySpawnPoint.transform.position);
							MyNav.enabled = true;
							offNavTolerance = 60f;
							offNav = false;
						}
					}
					else if (Leashing)
					{
						if (NeedsNavUpdate(HomePos))
						{
							MyNav.SetDestination(GameData.GetSafeNavMeshPoint(HomePos));
						}
						MyNav.speed = MyStats.actualRunSpeed / 3f;
						if (MyNav.isOnNavMesh)
						{
							MyNav.isStopped = false;
						}
						MyAnim.SetBool("Walking", value: true);
						MyAnim.SetBool("Patrol", value: true);
						if (MyStats.CurrentHP < MyStats.CurrentMaxHP)
						{
							MyStats.CurrentHP += 350;
							if (MyStats.CurrentHP > MyStats.CurrentMaxHP)
							{
								MyStats.CurrentHP = MyStats.CurrentMaxHP;
							}
						}
					}
				}
				if (MyStats.Feared && !MyStats.Rooted && !MyStats.Stunned)
				{
					if (SpawnPointManager.SpawnPointsInScene.Count > 0)
					{
						if (FearDestination == null || Vector3.Distance(base.transform.position, FearDestination.position) < 5f)
						{
							FearDestination = SpawnPointManager.SpawnPointsInScene[Random.Range(0, SpawnPointManager.SpawnPointsInScene.Count)].transform;
							HighPriorityNavUpdate(FearDestination.position);
							MyNav.speed = MyStats.actualRunSpeed;
							if (MyNav.isOnNavMesh)
							{
								MyNav.isStopped = false;
							}
							MyAnim.SetBool("Walking", value: true);
							MyAnim.SetBool("Patrol", value: false);
						}
						else if (NeedsNavUpdate(FearDestination.position))
						{
							if (MyNav.isOnNavMesh)
							{
								MyNav.SetDestination(FearDestination.position);
							}
							MyNav.speed = MyStats.actualRunSpeed;
							if (MyNav.isOnNavMesh)
							{
								MyNav.isStopped = false;
							}
							MyAnim.SetBool("Walking", value: true);
							MyAnim.SetBool("Patrol", value: false);
						}
					}
					else
					{
						MyNav.speed = 0f;
						if (MyNav.isOnNavMesh)
						{
							MyNav.isStopped = false;
						}
						MyAnim.SetBool("Walking", value: false);
						MyAnim.SetBool("Patrol", value: false);
					}
				}
				else if (MyStats.Rooted)
				{
					UpdateNav();
				}
			}
			yield return new WaitForSeconds(0.03f);
		}
	}

	private void DoStances()
	{
		if (ThisSim == null)
		{
			ThisSim = GetComponent<SimPlayer>();
		}
		if (ThisSim == null)
		{
			return;
		}
		Stance stance = GameData.SkillDatabase.NormalStance;
		if (GameData.SimMngr.Sims[ThisSim.myIndex].Grouped && CurrentAggroTarget != null)
		{
			if (ThisSim.PullerPulling && ThisSim.KnownStances.Contains(GameData.SkillDatabase.DefensiveStance))
			{
				stance = GameData.SkillDatabase.DefensiveStance;
			}
			else if ((GameData.SimPlayerGrouping.MainTank != null && GameData.SimMngr.Sims[ThisSim.myIndex].Grouped && GameData.SimPlayerGrouping.MainTank == GameData.SimMngr.Sims[ThisSim.myIndex]) || !GameData.SimMngr.Sims[ThisSim.myIndex].Grouped)
			{
				if (ThisSim.KnownStances.Contains(GameData.SkillDatabase.TauntingStance) && (float)MyStats.CurrentHP > (float)MyStats.CurrentMaxHP * 0.4f)
				{
					stance = GameData.SkillDatabase.TauntingStance;
				}
				else if (ThisSim.KnownStances.Contains(GameData.SkillDatabase.DefensiveStance) && (float)MyStats.CurrentHP <= (float)MyStats.CurrentMaxHP * 0.4f)
				{
					stance = GameData.SkillDatabase.DefensiveStance;
				}
			}
			else if (GameData.SimPlayerGrouping.PlayerIsTank || GameData.SimPlayerGrouping.MainTank == null || (GameData.SimMngr.Sims[ThisSim.myIndex].Grouped && GameData.SimPlayerGrouping.MainTank != GameData.SimMngr.Sims[ThisSim.myIndex]) || !GameData.SimMngr.Sims[ThisSim.myIndex].Grouped)
			{
				if (ThisSim.KnownStances.Contains(GameData.SkillDatabase.RecklessStance) && (float)MyStats.CurrentHP > (float)MyStats.CurrentMaxHP * 0.5f)
				{
					stance = GameData.SkillDatabase.RecklessStance;
				}
				else if (ThisSim.KnownStances.Contains(GameData.SkillDatabase.AggressiveStance) && (float)MyStats.CurrentHP <= (float)MyStats.CurrentMaxHP * 0.5f)
				{
					stance = GameData.SkillDatabase.AggressiveStance;
				}
			}
		}
		if (MyStats.CombatStance != stance)
		{
			MyStats.ChangeStance(stance);
		}
	}

	private IEnumerator BehaviorUpdate(float waitSec)
	{
		while (Myself.Alive)
		{
			if (MyStats.CharacterClass == GameData.ClassDB.Reaver)
			{
				DoStances();
			}
			if (!Leashing && spawnCD <= 0f)
			{
				if (GameData.AttackingPlayer.Contains(this) && CurrentAggroTarget != GameData.PlayerControl.Myself)
				{
					GameData.AttackingPlayer.Remove(this);
				}
				if (GameData.GroupMatesInCombat.Contains(this) && CurrentAggroTarget == null)
				{
					GameData.GroupMatesInCombat.Remove(this);
				}
				if (Myself.MyStats.Charmed && Myself.Master != null && CurrentAggroTarget == Myself.Master)
				{
					CurrentAggroTarget = null;
				}
				if (Myself.MyWorldFaction != null)
				{
					if ((Myself.AggressiveTowards.Contains(Character.Faction.Player) || Myself.AggressiveTowards.Contains(Character.Faction.PC)) && Myself.MyWorldFaction.FactionValue > 0f)
					{
						if (Myself.AggressiveTowards.Contains(Character.Faction.Player))
						{
							Myself.AggressiveTowards.Remove(Character.Faction.Player);
						}
						if (Myself.AggressiveTowards.Contains(Character.Faction.PC))
						{
							Myself.AggressiveTowards.Remove(Character.Faction.PC);
						}
					}
					if (!Myself.AggressiveTowards.Contains(Character.Faction.PC) && Myself.MyWorldFaction.FactionValue < 0f)
					{
						Myself.AggressiveTowards.Add(Character.Faction.PC);
					}
				}
				if (Myself.Alive)
				{
					if (MyStats.CurrentMana < MyStats.GetCurrentMaxMana() && Random.Range(0, 10) > 7 && !SimPlayer)
					{
						MyStats.CurrentMana++;
					}
					if (!MyStats.Stunned && !MyStats.Feared)
					{
						if (MyAnim != null && MyAnim.runtimeAnimatorController != null)
						{
							MyAnim.SetBool("Stunned", value: false);
						}
						if (!MySpells.isCasting() && !MiningNode && !TreasureChest)
						{
							if (SimPlayer && GameData.SimPlayerGrouping.IsSimInPlayerGroup(ThisSim))
							{
								CheckAssist();
							}
							if ((CurrentAggroTarget == null && !SimPlayer) || (SimPlayer && (!GameData.SimPlayerGrouping.IsSimInPlayerGroup(ThisSim) || GameData.SimPlayerGrouping.IsMADead() || GameData.SimPlayerGrouping.GetMA() == base.transform.name || GameData.SimPlayerGrouping.GroupTargets.Count <= 1)) || (CurrentAggroTarget != null && !CurrentAggroTarget.Alive))
							{
								CheckAggro();
							}
							if (((SimPlayer && MyStats.CharacterClass == GameData.ClassDB.Paladin) || (SimPlayer && MyStats.CharacterClass == GameData.ClassDB.Reaver)) && CurrentAggroTarget != null && tauntCD <= 0f)
							{
								CheckTaunt();
							}
							if (SimPlayer && CurrentAggroTarget != null)
							{
								CheckFearKite();
							}
							if (SimPlayer && CurrentAggroTarget != null && Myself.NearbyEnemies.Count == 1)
							{
								CheckSnareSpell();
							}
							if (((CurrentAggroTarget == null && !SimPlayer) || (SimPlayer && !GameData.SimMngr.Sims[ThisSim.myIndex].isPuller)) && MyPetSpell != null)
							{
								CheckPet();
							}
							if ((CurrentAggroTarget == null && !SimPlayer) || (SimPlayer && !GameData.SimMngr.Sims[ThisSim.myIndex].isPuller))
							{
								CheckBuffs();
							}
							if ((healCD <= 0f && (!SimPlayer || retreat || ThisSim.CurrentPullPhase == global::SimPlayer.PullPhases.NotPulling) && SimPlayer && GameData.SimPlayerGrouping.Heals.Contains(GameData.SimMngr.Sims[ThisSim.myIndex])) || !SimPlayer || (SimPlayer && !ThisSim.InGroup && (MyStats.CharacterClass == GameData.ClassDB.Arcanist || MyStats.CharacterClass == GameData.ClassDB.Druid || MyStats.CharacterClass == GameData.ClassDB.Paladin)))
							{
								CheckHeals();
							}
							if (CurrentAggroTarget != null && !SimPlayer && !MyStats.Charmed)
							{
								Character currentAggroTarget = CurrentAggroTarget;
								if (currentAggroTarget.MyStats != null)
								{
									currentAggroTarget.MyStats.RecentDmg = 240f;
								}
								CurrentAggroTarget = GetHighestAggro();
								ForceAggroOn(CurrentAggroTarget);
							}
							if (!SimPlayer || (SimPlayer && !ThisSim.IgnoreAllCombat && !MyStats.Invisible && !MyStats.Feared))
							{
								if ((SimPlayer && GameData.SimPlayerGrouping.CC.Contains(GameData.SimMngr.Sims[ThisSim.myIndex])) || (SimPlayer && GameData.SimMngr.IsSimGrouped(GameData.SimMngr.Sims[ThisSim.myIndex])))
								{
									CrowdControl();
								}
								SetThreatToAggro();
								Combat();
							}
						}
					}
					else if (MyStats.Feared && !MyStats.Rooted && !MyStats.Stunned && MyStats.Myself.Alive)
					{
						MyAnim.SetBool("Patrol", value: true);
						MyAnim.SetBool("Walking", value: false);
						MyNav.speed = MyStats.actualRunSpeed;
					}
					else if (MyStats.CurrentHP > 0 && MyStats.Stunned)
					{
						MyAnim.SetBool("Stunned", value: true);
						MyAnim.SetBool("Patrol", value: false);
						MyAnim.SetBool("Walking", value: false);
						if (!offNav && MyNav.isOnNavMesh)
						{
							MyNav.isStopped = true;
						}
					}
					if (Myself.NearbyEnemies.Count > 0)
					{
						for (int num = Myself.NearbyEnemies.Count - 1; num >= 0; num--)
						{
							if (Myself.NearbyEnemies[num] == null || !Myself.NearbyEnemies[num].Alive || !Myself.NearbyEnemies[num].gameObject.activeSelf)
							{
								Myself.NearbyEnemies.RemoveAt(num);
							}
						}
					}
				}
			}
			yield return new WaitForSeconds(0.05f);
		}
	}

	private void CountOffNav(NavMeshPath _path)
	{
		if (!MyStats.Charmed && (_path == null || _path.status == NavMeshPathStatus.PathInvalid || _path.status == NavMeshPathStatus.PathPartial))
		{
			if (SimPlayer)
			{
				return;
			}
			if (offNavTolerance > 0f)
			{
				offNavTolerance -= 1f;
			}
			else if (CurrentAggroTarget != null)
			{
				Leashing = true;
				if (CurrentAggroTarget == GameData.PlayerControl.Myself && GameData.PlayerControl.offNavWarnCD <= 0f)
				{
					GameData.PlayerControl.OffNavAbuse++;
					GameData.PlayerControl.offNavWarnCD = 60f;
				}
			}
		}
		else
		{
			offNavTolerance = 60f;
		}
	}

	private void DoImbued()
	{
		if (!(MyStats.CharacterClass == GameData.ClassDB.Stormcaller) || !IsCasting())
		{
			return;
		}
		CastSpell mySpells = MySpells;
		if ((object)mySpells == null || mySpells.GetCurrentCast()?.Type != Spell.SpellType.Damage || !(MyStats.Level - MySpells?.GetCurrentCast()?.RequiredLevel < 13))
		{
			return;
		}
		foreach (SimPlayerSkillSlot item in ThisSim.Skillbook)
		{
			if (item.skill.Id == "58018670" && MHBow && item.CD <= 0f)
			{
				MySkills.DoSkillNoChecks(item.skill, CurrentAggroTarget);
				AllHKCD = 60f;
				item.CD = item.skill.Cooldown;
				break;
			}
		}
	}

	private void DoAttackSkill()
	{
		if (ThisSim == null)
		{
			return;
		}
		bool flag = false;
		bool flag2 = false;
		if (ThisSim != null && ThisSim.MyEquipment[0] != null)
		{
			flag = ThisSim.MyEquipment[0].MyItem.ThisWeaponType == Item.WeaponType.TwoHandMelee || ThisSim.MyEquipment[0].MyItem.ThisWeaponType == Item.WeaponType.TwoHandStaff;
		}
		if (ThisSim != null && ThisSim.MyEquipment[1] != null)
		{
			flag2 = ThisSim.MyEquipment[1].MyItem.Shield;
		}
		foreach (SimPlayerSkillSlot item in ThisSim.Skillbook)
		{
			if (item.CD <= 0f && AllHKCD <= 0f && ((!item.skill.RequireShield && !item.skill.Require2H) || (item.skill.Require2H && flag) || (item.skill.RequireShield && flag2) || (item.skill.RequireBow && MHBow)) && (!item.skill.AESkill || !ThisSim.InGroup || (GameData.SimPlayerGrouping.CC.Count <= 0 && !GameData.SimPlayerGrouping.PlayerIsCC)))
			{
				MySkills.DoSkillNoChecks(item.skill, CurrentAggroTarget);
				if (item.skill.ProcShield && flag2 && ThisSim.MyEquipment[1].MyItem.WeaponProcOnHit != null && (float)Random.Range(0, 100) < ThisSim.MyEquipment[1].MyItem.WeaponProcChance)
				{
					MySpells.StartSpellFromProc(ThisSim.MyEquipment[1].MyItem.WeaponProcOnHit, CurrentAggroTarget.MyStats, 0.1f);
				}
				if (item.skill.CastOnTarget != null && !item.skill.RequireBow)
				{
					MySpells.StartSpellFromProc(item.skill.CastOnTarget, CurrentAggroTarget.MyStats, 0.1f);
				}
				if (item.skill.EffectToApply != null && !item.skill.RequireBow)
				{
					CurrentAggroTarget.MyStats.AddStatusEffect(item.skill.EffectToApply, _fromPlayer: false, 1, Myself);
				}
				AllHKCD = 60f;
				item.CD = item.skill.Cooldown;
				break;
			}
		}
	}

	public bool ForceSpellCast(Spell _spell, Stats _target)
	{
		if (!MySpells.isCasting())
		{
			return MySpells.StartSpell(_spell, _target);
		}
		return false;
	}

	public bool IsCasting()
	{
		return MySpells.isCasting();
	}

	public Vector3 SampleNavMeshNearTransform(Vector3 origin)
	{
		if (NavMesh.SamplePosition(origin, out var hit, 2.5f, -1))
		{
			return hit.position;
		}
		return origin;
	}

	public static Vector3 GetSafeNavMeshPoint(Vector3 _destination, float sampleRadius = 2f, float maxVerticalDiff = 0.25f, float verticalSearchHeight = 4f, float step = 0.5f)
	{
		if (NavMesh.SamplePosition(_destination + Vector3.up, out var hit, sampleRadius, -1) && Mathf.Abs(hit.position.y - _destination.y) <= maxVerticalDiff)
		{
			return hit.position;
		}
		if (NavMesh.SamplePosition(_destination + Vector3.up, out var hit2, 4f, -1) && Mathf.Abs(hit2.position.y - _destination.y) <= maxVerticalDiff)
		{
			return hit2.position;
		}
		if (NavMesh.SamplePosition(_destination + Vector3.up, out var hit3, 12f, -1) && Mathf.Abs(hit3.position.y - _destination.y) <= maxVerticalDiff)
		{
			return hit3.position;
		}
		return _destination;
	}

	private void PerformPhantomHit(int baseDamage, bool isOffhand)
	{
		bool criticalHit = false;
		int num = 0;
		if (SimPlayer)
		{
			num = MyStats.CalcMeleeDamage(baseDamage, CurrentAggroTarget.MyStats.Level, CurrentAggroTarget.MyStats, 0);
		}
		else
		{
			if (DamageRange.x == 1f && DamageRange.y == 1f)
			{
				DamageRange.x = Mathf.Pow(MyStats.Level, 1.9f) * 0.6f + 12f;
				DamageRange.y = Mathf.Pow(MyStats.Level, 1.9f) + 24f;
				DamageRange.x *= DamageMult;
				DamageRange.y *= DamageMult;
			}
			num = MyStats.CalcMeleeDamageForNPC(DamageRange, CurrentAggroTarget.MyStats);
		}
		if (!SimPlayer && !Myself.MyStats.Charmed)
		{
			num = Mathf.RoundToInt((float)num * GameData.ServerDMGMod);
		}
		if (SimPlayer)
		{
			if (!isOffhand && Myself.MyStats.MyInv.TwoHandPrimary && !MHWand)
			{
				num = Mathf.RoundToInt((float)num * 2f);
			}
			MyStats.CheckProc(isOffhand ? MyStats.MyInv.EquippedItems[1] : MyStats.MyInv.EquippedItems[0], CurrentAggroTarget);
			if ((!isOffhand && !MHWand && !MHBow) || isOffhand)
			{
				if (MyStats.isCriticalAttack() && num > 0)
				{
					criticalHit = true;
					num = Mathf.RoundToInt((float)num * 1.5f);
					if (MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByName("Crippling Blow")) && Random.Range(0, 10) > 8)
					{
						num *= 2;
						if (Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 10f)
						{
							UpdateSocialLog.CombatLogAdd(Myself.MyNPC.NPCName + " scores a CRIPPLING BLOW!", "lightblue");
						}
					}
					GameData.CamControl.ShakeScreen(0.1f, 3f);
				}
			}
			else
			{
				if (MHWand && !isOffhand && !MHBow)
				{
					DoWandAttack(CurrentAggroTarget);
					MyStats.ResetMHAtkDelay();
					return;
				}
				if (MHBow && !isOffhand && !MHWand)
				{
					DoBowAttack(CurrentAggroTarget, 0);
					if (Myself.MySkills.HasAscension("4547270"))
					{
						if (Myself.MySkills.GetAscensionRank("4547270") * 10 > Random.Range(0, 100))
						{
							MyStats.ResetMHAtkDelay(Mathf.RoundToInt(MyStats.GetMHAtkDelay() * 0.5f));
						}
					}
					else
					{
						MyStats.ResetMHAtkDelay();
					}
					return;
				}
			}
		}
		string text = CheckTargetInnateAvoidance(CurrentAggroTarget);
		string text2 = ((CurrentAggroTarget.transform.name == "Player") ? "YOU" : CurrentAggroTarget.transform.name);
		string text3 = ((text2 == "YOU") ? "YOUR" : "their");
		string colorAsString = ((text2 == "YOU") ? "red" : "white");
		if (Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 50f)
		{
			UpdateSocialLog.LogAdd(base.transform.name + " performs a SPECTRAL STRIKE!", "red");
		}
		if (text != "")
		{
			if (CurrentAggroTarget == GameData.PlayerControl.Myself)
			{
				UpdateSocialLog.CombatLogAdd(base.transform.name + " tries to SPECTRAL STRIKE " + text2 + ", but " + text2 + text, colorAsString);
			}
			return;
		}
		MyStats.ResetMHAtkDelay();
		if (isOffhand)
		{
			MyStats.ResetOHAtkDelay();
		}
		int num2 = CurrentAggroTarget.DamageMe(num, MyStats.Charmed || (ThisSim != null && ThisSim.InGroup), GameData.DamageType.Physical, Myself, _animEffect: true, criticalHit);
		if (NPCProcOnHit != null && (float)Random.Range(0, 100) < NPCProcOnHitChance)
		{
			MySpells.StartSpellFromProc(NPCProcOnHit, CurrentAggroTarget.MyStats, 0.1f);
		}
		if (Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 50f || CurrentAggroTarget == GameData.PlayerControl.Myself)
		{
			if (num2 > 0)
			{
				UpdateSocialLog.CombatLogAdd(base.transform.name + " SPECTRAL STRIKES " + text2 + " for " + num2 + " damage.", colorAsString);
			}
			else
			{
				switch (num2)
				{
				case 0:
					UpdateSocialLog.CombatLogAdd(base.transform.name + " tries to SPECTRAL STRIKE " + text2 + ", but misses!", colorAsString);
					break;
				case -2:
					UpdateSocialLog.CombatLogAdd(base.transform.name + " tries to SPECTRAL STRIKE " + text2 + ", but " + text3 + " shield absorbs the blow!", colorAsString);
					break;
				}
			}
			MyStats.HealMe(Mathf.RoundToInt((float)num2 * (MyStats.PercentLifesteal / 100f)));
		}
		if (Myself.MyAttackSound != null && (Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 8f || CurrentAggroTarget == GameData.PlayerControl.Myself))
		{
			Myself.MyAudio.PlayOneShot(Myself.MyAttackSound, Myself.MyAudio.volume * GameData.CombatVol * GameData.MasterVol);
		}
	}

	public void StuckMonitor()
	{
		if (!SimPlayer || ThisSim == null || !ThisSim.InGroup || MyNav == null || !MyNav.enabled)
		{
			return;
		}
		bool flag = lastDest != Vector3.zero && Vector3.SqrMagnitude(MyNav.destination - lastDest) > 0.0001f;
		if ((!prevHasPath && MyNav.hasPath) || flag)
		{
			pathAcquireTime = Time.time;
		}
		prevHasPath = MyNav.hasPath;
		lastDest = MyNav.destination;
		if (MyNav.pathPending || MyNav.isOnOffMeshLink)
		{
			return;
		}
		if (MyNav.hasPath && Time.time - pathAcquireTime > 0.25f && MyNav.remainingDistance > MyNav.stoppingDistance + 0.05f && MyNav.desiredVelocity.sqrMagnitude > 0.01f && MyNav.velocity.sqrMagnitude < 0.01f)
		{
			stuckTime += Time.deltaTime;
		}
		else
		{
			stuckTime = 0f;
		}
		if (!(stuckTime >= 3f) || !(Time.time - lastUnstickTime > 1f))
		{
			return;
		}
		MyNav.speed = MyStats.actualRunSpeed;
		if (MyNav.isOnNavMesh)
		{
			MyNav.isStopped = true;
			MyNav.ResetPath();
			Vector3 vector = base.transform.right * 0.25f;
			if (NavMesh.SamplePosition(base.transform.position + vector, out var hit, 0.5f, -1))
			{
				MyNav.Warp(hit.position);
			}
			MyNav.isStopped = false;
		}
		MyNav.SetDestination(GameData.GetSafeNavMeshPoint(GameData.PlayerControl.transform.position));
		if (!MyAnim.GetBool("Walking"))
		{
			MyAnim.SetBool("Walking", value: true);
		}
		if (MyAnim.GetBool("Patrol"))
		{
			MyAnim.SetBool("Patrol", value: false);
		}
		UpdateSocialLog.LogAdd(base.transform.name + " says: " + GameData.SimMngr.PersonalizeString("I lagged out for a sec... what were we doing again?", ThisSim));
		lastUnstickTime = Time.time;
		stuckTime = 0f;
		pathAcquireTime = Time.time;
	}
}
