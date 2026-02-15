// SimPlayer
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class SimPlayer : MonoBehaviour
{
	public enum PullPhases
	{
		NotPulling,
		FindTarget,
		GoToTarget,
		EngageTarget,
		ReturnTarget,
		AttackTarget
	}

	public float Dedication = 5f;

	public float SkillLevel;

	public bool MetPlayer;

	public float OpinionOfPlayer = 3f;

	public Stats MyStats;

	[SerializeField]
	private NPC MyNPC;

	[SerializeField]
	private Character Myself;

	[SerializeField]
	private NavMeshAgent MyNav;

	[SerializeField]
	private CastSpell MySpells;

	[SerializeField]
	private Animator MyAnim;

	[SerializeField]
	private Inventory MyInv;

	private bool resting;

	public string HairName = "";

	public int HairColor;

	public int SkinColor;

	public float TimeOnTask;

	public float WaitForInv;

	public PointOfInterest MyPOI;

	public int myIndex;

	public bool SeekPlayer;

	public bool awaitInvite;

	public bool InGroup;

	public bool GuardSpot;

	private Vector3 GuardPos;

	[TextArea(3, 5)]
	public string Bio;

	public int LoreChase;

	public int GearChase;

	public int SocialChase;

	public int Troublemaker;

	public int DedicationLevel;

	public float Greed;

	public int Patience = 3000;

	public bool Abbreviates;

	public float TypoChance;

	public float banterDel = 60f;

	public bool didGreet;

	private float aucDel = 120f;

	private bool hailed;

	private bool shouted;

	public bool AskForInv;

	public bool InTutorial;

	private Vector3 randomizeOffset;

	public bool IgnoreAllCombat;

	public Item WTB;

	private bool askForInv;

	public Character PullTarget;

	private float doorCheck = 60f;

	public PointOfInterest.POIType MyTask;

	public List<SimInvSlot> MyEquipment = new List<SimInvSlot>();

	public ModularParts Mods;

	public List<SimPlayerSpellSlot> Spellbook = new List<SimPlayerSpellSlot>();

	public List<SimPlayerSkillSlot> Skillbook = new List<SimPlayerSkillSlot>();

	public SimPlayerLanguage MyDialog;

	private float deadTimer = 60f;

	public bool NavWatch;

	public bool InMeleeRange;

	private SimInvSlot AuraSlot;

	private SimInvSlot CharmSlot;

	public List<Item> StoredItems;

	public bool PullerPulling;

	public bool RunningAway;

	private float healAskCD;

	private bool holdPullsForHeals;

	private bool training;

	private float randomizeActions;

	private float debugDeadCD = 360f;

	private float nearbyMOBsCD;

	public int TiedToSlot = 99;

	public List<Item> LootWanted;

	public int PersonalityType;

	public int BioIndex = -1;

	private int PullRange = 5;

	private Spell PullSpell;

	public Spell FearSpell;

	public Spell SnareSpell;

	public Spell InvisSpell;

	public bool pursuing;

	public bool assistCall;

	public bool InCampWhilePulling;

	private float NothingUpCD;

	public List<ItemSaveData> AllHeldItems = new List<ItemSaveData>();

	public bool NearFlamewell;

	public bool NearForge;

	public bool DisallowUpgrades;

	public bool CanRespec;

	private float AFKCheck = 120f;

	private float AFKcounter;

	private float atkCalloutCD;

	private float sitTolerance;

	private float sitTimer;

	public bool sitting;

	private float randomizeYRot;

	public bool TypesInAllCaps;

	public bool TypesInAllLowers;

	public bool TypesInThirdPerson;

	public float TypoRate = 0.5f;

	public bool LovesEmojis;

	public string RefersToSelfAs = "";

	public List<string> SignOffLine = new List<string>();

	public bool IsGMCharacter;

	private bool informedPlayerOfAggroOnWayToGroup;

	public List<Stance> KnownStances;

	public bool Rival;

	public float InvisPlayerIfISeeHim;

	public SimPlayerTracking MySimTracking;

	public PullPhases CurrentPullPhase;

	public int returnItemQual;

	public int returnSecondItemQual;

	public bool IsFullyLoaded { get; private set; }

	private SimPlayerSaveData PopulateSave { get; set; }

	public NPC GetThisNPC()
	{
		return MyNPC;
	}

	public void ClearPullTarget()
	{
		PullTarget = null;
	}

	private void Awake()
	{
		if (MyDialog == null)
		{
			MyDialog = GetComponent<SimPlayerLanguage>();
		}
		if (MyInv == null)
		{
			MyInv = GetComponent<Inventory>();
		}
		if (MyStats == null)
		{
			MyStats = GetComponent<Stats>();
		}
		if (MyNPC == null)
		{
			MyNPC = GetComponent<NPC>();
		}
		if (Myself == null)
		{
			Myself = GetComponent<Character>();
		}
		MyStats.Myself = Myself;
		if (MyNav == null)
		{
			MyNav = GetComponent<NavMeshAgent>();
		}
		if (MySpells == null)
		{
			MySpells = GetComponent<CastSpell>();
		}
		if (MyAnim == null)
		{
			MyAnim = GetComponent<Animator>();
		}
		do
		{
			randomizeOffset = new Vector3(UnityEngine.Random.Range(-3, 3), 0f, UnityEngine.Random.Range(-3, 3));
		}
		while (randomizeOffset.magnitude < 2f);
		MyNPC.HoldDPS = 100f;
		LoadItemIcons();
		LoadSimData();
		LoadSimSkills();
		LoadSimSpells();
		LoadSimBuffs();
		banterDel = UnityEngine.Random.Range(100, 25000);
		MyStats.CurrentMana = MyStats.GetCurrentMaxMana();
		if (SkillLevel == 0f)
		{
			SkillLevel = UnityEngine.Random.Range(20, 80);
		}
		sitTimer = 0f;
		Myself.SeeInvisible = true;
	}

	private void Start()
	{
		MySimTracking = GameData.SimMngr.Sims[myIndex];
		if (MySimTracking != null)
		{
			MySimTracking.GoingToReliq = false;
			MyNPC.HoldDPS = MySimTracking.HoldDPSNum;
		}
		MyDialog = GetComponent<SimPlayerLanguage>();
		if (UnityEngine.Random.Range(0, 10) > 6)
		{
			BuyingItem();
		}
		if (MyNPC != null && MyNPC.AggroArea != null && MySimTracking.Caution)
		{
			MyNPC.AggroArea.transform.localScale = Vector3.one * 3f;
		}
		AuditInventory();
		base.transform.gameObject.layer = 9;
		MyStats.CurrentMana = MyStats.GetCurrentMaxMana();
		if (Myself.MySkills.AscensionPoints > 0)
		{
			Myself.MyStats.SimPlayerChooseAscension();
		}
		sitTolerance = UnityEngine.Random.Range(60, 1200);
		if (SceneManager.GetActiveScene().name == "Azure")
		{
			sitTolerance = UnityEngine.Random.Range(60, 500);
		}
	}

	private void Update()
	{
		if (atkCalloutCD > 0f)
		{
			atkCalloutCD -= 60f * Time.deltaTime;
		}
		if (InvisPlayerIfISeeHim > 0f)
		{
			InvisPlayerIfISeeHim -= 60f * Time.deltaTime;
		}
		if (AFKCheck > 0f && InGroup)
		{
			AFKCheck -= 60f * Time.deltaTime;
			if (AFKCheck <= 0f)
			{
				AFKCheck = 120f;
				if (MyNav.velocity == Vector3.zero && GetThisNPC()?.CurrentAggroTarget == null && !GuardSpot && !PullerPulling)
				{
					if (Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) > 20f)
					{
						if (AFKcounter > 3f)
						{
							GameData.SimPlayerGrouping.GroupResetMovementDebug(this);
							AFKcounter = 0f;
						}
						else
						{
							AFKcounter += 60f * Time.deltaTime;
						}
					}
				}
				else
				{
					AFKcounter = 0f;
				}
			}
		}
		SitLogic();
		if (MyStats.TotalAvailableProficiencies > 0 && !CanRespec)
		{
			MyStats.AssignPoint();
		}
		SeekPlayer = GameData.SimMngr.Sims[myIndex].seekingPlayer;
		if (MyStats.RecentDmg > 0f && GameData.SimMngr.Sims[myIndex].seekingPlayer && !informedPlayerOfAggroOnWayToGroup)
		{
			GameData.SimMngr.LoadResponse("[WHISPER FROM] " + base.transform.name + GameData.SimMngr.PersonalizeString(": I have aggro!, trying to get to you!", this), base.transform.name);
			informedPlayerOfAggroOnWayToGroup = true;
		}
		if (MyNav.isOnNavMesh)
		{
			NavWatch = MyNav.isStopped;
		}
		InMeleeRange = MyNPC.inMeleeRange;
		foreach (SimPlayerSpellSlot item in Spellbook)
		{
			if (item.CD > 0f)
			{
				item.CD -= 60f * Time.deltaTime;
			}
		}
		foreach (SimPlayerSkillSlot item2 in Skillbook)
		{
			if (item2.CD > 0f)
			{
				item2.CD -= 60f * Time.deltaTime;
			}
		}
		if (NothingUpCD > 0f)
		{
			NothingUpCD -= 60f * Time.deltaTime;
		}
		if (MyNPC != null && MyNPC.CurrentAggroTarget != null && (MyNPC.CurrentAggroTarget == GameData.PlayerControl.Myself || MyNPC.CurrentAggroTarget.MyFaction == Character.Faction.PC || MyNPC.CurrentAggroTarget.MyFaction == Character.Faction.Player))
		{
			MyNPC.CurrentAggroTarget = null;
		}
		if (randomizeActions > 0f)
		{
			randomizeActions -= 60f * Time.deltaTime;
		}
		if (banterDel > 0f)
		{
			banterDel -= 60f * Time.deltaTime;
		}
		if (aucDel > 0f)
		{
			aucDel -= 60f * Time.deltaTime;
		}
		if (WaitForInv > 0f && !InGroup)
		{
			WaitForInv -= 60f * Time.deltaTime;
			MyPOI = null;
			awaitInvite = true;
			if (WaitForInv <= 0f && !InGroup && GameData.SimMngr.Sims[myIndex].PersonalInvite)
			{
				GameData.SimMngr.Sims[myIndex].FakeInvited = true;
				GameData.SimMngr.Sims[myIndex].PersonalInvite = false;
			}
		}
		else
		{
			awaitInvite = false;
		}
		if (doorCheck >= 0f)
		{
			doorCheck -= 60f * Time.deltaTime;
			if (doorCheck < 0f)
			{
				doorCheck = 30f;
				if (Myself.NearbyDoors.Count > 0)
				{
					foreach (Door nearbyDoor in Myself.NearbyDoors)
					{
						if (Vector3.Distance(nearbyDoor.transform.position, base.transform.position) < 5f && nearbyDoor.isClosed && !nearbyDoor.swinging)
						{
							nearbyDoor.OpenOrShut();
						}
					}
				}
			}
		}
		if (WTB != null && !GameData.SimMngr.IsSimInPlayerGroup(base.transform.name) && aucDel <= 0f && UnityEngine.Random.Range(0f, 1000f) > 998.8f && GameData.ShowWTB && !IsGMCharacter)
		{
			UpdateSocialLog.LogAdd(MyNPC.NPCName + " shouts: " + GameData.SimMngr.PersonalizeString("WTB " + WTB.ItemName + ", offering " + WTB.ItemValue + " gold. Open a trade with me.", this), "#FF9000");
			aucDel = UnityEngine.Random.Range(3000, 12000);
		}
		if (!training && !RunningAway)
		{
			if (!InGroup && !GameData.SimMngr.IsSimGrouped(GameData.SimMngr.Sims[myIndex]))
			{
				if (MyStats.Level <= GameData.PlayerStats.Level && UnityEngine.Random.Range(0, 1000) > 998 && UnityEngine.Random.Range(0, 1000) > 998 && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) > 20f)
				{
					MyStats.EarnedXP(Mathf.RoundToInt((float)MyStats.ExperienceToLevelUp / 25f));
					UpdateSocialLog.LogAdd(MyNPC.NPCName + " shouts: " + GameData.SimMngr.PersonalizeString(MyDialog.GetExclamation() + "!", this), "#FF9000");
				}
				IgnoreAllCombat = false;
				if (TimeOnTask >= 0f)
				{
					if (MyStats.GetCurrentHP() > MyStats.CurrentMaxHP / 3)
					{
						resting = false;
						GoToTask();
					}
					else
					{
						ForceRest();
					}
				}
				else if (!IsGMCharacter)
				{
					FindNewTask();
				}
			}
			else if (InGroup && !GameData.SimMngr.IsSimGrouped(GameData.SimMngr.Sims[myIndex]))
			{
				if (!Myself.Allies.Contains(Character.Faction.PC))
				{
					Myself.Allies.Add(Character.Faction.PC);
				}
				if (Myself.Alive)
				{
					if (!GuardSpot && !GameData.SimMngr.Sims[myIndex].isPuller && !IgnoreAllCombat)
					{
						FollowPlayer();
					}
					else if (randomizeActions <= 0f)
					{
						if (!GameData.SimMngr.Sims[myIndex].isPuller)
						{
							DoGuard();
						}
						else if (GameData.SimPlayerGrouping.Puller != null && GameData.SimPlayerGrouping.Puller == GameData.SimMngr.Sims[myIndex])
						{
							DoPulling();
						}
						else if (!GameData.SimMngr.Sims[myIndex].isPuller && IgnoreAllCombat)
						{
							IgnoreAllCombat = false;
						}
						if (CurrentPullPhase == PullPhases.NotPulling)
						{
							IgnoreAllCombat = false;
						}
						if (!GameData.SimMngr.Sims[myIndex].isPuller)
						{
							CurrentPullPhase = PullPhases.NotPulling;
							PullTarget = null;
						}
						randomizeActions = UnityEngine.Random.Range(3, 35);
					}
				}
				else
				{
					if (GameData.SimPlayerGrouping.Puller == GameData.SimMngr.Sims[myIndex] && GameData.SimPlayerGrouping.PullConstant)
					{
						GameData.SimPlayerGrouping.TogglePullConstant();
					}
					if (IsSimGroupRelaxed() && IsSimGroupMemberAlive() && IsGroupMemberNearby())
					{
						if (deadTimer <= 0f)
						{
							if (GameData.GroupMatesInCombat.Contains(GetComponent<NPC>()))
							{
								GameData.GroupMatesInCombat.Remove(GetComponent<NPC>());
							}
							MyNPC.TickTime = 20f;
							GetComponent<NPC>().NamePlate.GetComponent<TextMeshPro>().text = base.transform.name;
							if (MyNPC.GuildName != "")
							{
								TextMeshPro component = MyNPC.NamePlate.GetComponent<TextMeshPro>();
								component.text = component.text + "\n<" + MyNPC.GuildName + ">";
							}
							if (MyNav.isOnNavMesh)
							{
								MyNav.isStopped = false;
							}
							MyNav.speed = MyStats.actualRunSpeed;
							if (!training)
							{
								MyNav.SetDestination(GameData.GetSafeNavMeshPoint(base.transform.position));
							}
							IgnoreAllCombat = false;
							GameData.SimMngr.Sims[myIndex].isPuller = false;
							GuardSpot = false;
							MyStats.CurrentHP = 10;
							MyNPC.noClick = false;
							MyAnim.SetBool("Dead", value: false);
							MyNPC.retreat = false;
							Myself.Alive = true;
							MyAnim.SetTrigger("Revive");
							deadTimer = 60f;
							base.transform.gameObject.layer = 9;
							GetComponent<CapsuleCollider>().center = new Vector3(0f, 1f, 0f);
							GetComponent<CapsuleCollider>().height = 2f;
							UpdateSocialLog.LogAdd(base.transform.name + " tells the group: Close one!", "#00B2B7");
							MyNPC.Revive();
							ResetNearbyFriend();
						}
						else
						{
							deadTimer -= 60f * Time.deltaTime;
						}
					}
					else
					{
						deadTimer = 60f;
						if (debugDeadCD > 0f)
						{
							debugDeadCD -= 60f * Time.deltaTime;
							if (debugDeadCD <= 0f)
							{
								if (IsSimGroupInCombat())
								{
									UpdateSocialLog.LogAdd(base.transform.name + " tells the group: " + GameData.SimMngr.PersonalizeString(MyDialog.Died[UnityEngine.Random.Range(0, MyDialog.Died.Count)], this), "#00B2B7");
								}
								else if (!IsGroupMemberNearby())
								{
									UpdateSocialLog.LogAdd(base.transform.name + " tells the group: " + GameData.SimMngr.PersonalizeString(MyDialog.Died[UnityEngine.Random.Range(0, MyDialog.Died.Count)], this), "#00B2B7");
								}
								else if (!IsSimGroupMemberAlive())
								{
									UpdateSocialLog.LogAdd(base.transform.name + " tells the group: " + GameData.SimMngr.PersonalizeString("It's telling me you're all dead! I can't revive!", this), "#00B2B7");
								}
								debugDeadCD = Patience;
							}
						}
					}
				}
			}
			else if (GameData.SimMngr.IsSimGrouped(GameData.SimMngr.Sims[myIndex]))
			{
				SimPlayerIndependentGroup simGroup = GameData.SimMngr.GetSimGroup(GameData.SimMngr.Sims[myIndex]);
				if (simGroup == null)
				{
					return;
				}
				if (Myself.Alive)
				{
					if (MyPOI == null || simGroup.AmILeader(GameData.SimMngr.Sims[myIndex]))
					{
						if (MyStats.Level <= GameData.PlayerStats.Level && UnityEngine.Random.Range(0, 1000) > 996 && UnityEngine.Random.Range(0, 1000) > 990 && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) > 20f)
						{
							MyStats.EarnedXP(Mathf.RoundToInt((float)MyStats.ExperienceToLevelUp / 25f));
						}
						if (TimeOnTask >= 0f)
						{
							if (MyStats.GetCurrentHP() > MyStats.CurrentMaxHP / 3)
							{
								resting = false;
								GoToTask();
							}
							else
							{
								ForceRest();
							}
						}
						else
						{
							FindNewTask();
						}
					}
					else if (MyPOI != null && MyNPC.CurrentAggroTarget == null)
					{
						if (Vector3.Distance(base.transform.position, MyPOI.transform.position + randomizeOffset) > 6f && !resting && !MyStats.Rooted && !MySpells.isCasting())
						{
							MyNav.speed = MyStats.actualRunSpeed;
							if (MyNav.isOnNavMesh)
							{
								MyNav.isStopped = false;
							}
							MyAnim.SetBool("Walking", value: true);
							MyAnim.SetBool("Patrol", value: false);
							if (MyNav.destination != MyPOI.transform.position)
							{
								MyNav.SetDestination(GameData.GetSafeNavMeshPoint(MyPOI.transform.position));
							}
						}
						else if (Vector3.Distance(base.transform.position, MyPOI.transform.position + randomizeOffset) <= 6f)
						{
							if (MyNav.isOnNavMesh)
							{
								MyNav.isStopped = true;
							}
							MyAnim.SetBool("Walking", value: false);
							MyAnim.SetBool("Patrol", value: false);
						}
					}
				}
				else if (GameData.SimMngr.GetSimGroup(GameData.SimMngr.Sims[myIndex]).GroupMembersAlive() > 0)
				{
					if (!GameData.SimMngr.GetSimGroup(GameData.SimMngr.Sims[myIndex]).IsGroupInCombat())
					{
						if (deadTimer <= 0f)
						{
							MyNPC.TickTime = 20f;
							GetComponent<NPC>().NamePlate.GetComponent<TextMeshPro>().text = base.transform.name;
							if (MyNav.isOnNavMesh)
							{
								MyNav.isStopped = false;
							}
							MyNav.speed = MyStats.actualRunSpeed;
							if (!training)
							{
								MyNav.SetDestination(GameData.GetSafeNavMeshPoint(base.transform.position));
							}
							IgnoreAllCombat = false;
							GameData.SimMngr.Sims[myIndex].isPuller = false;
							GuardSpot = false;
							MyStats.CurrentHP = 10;
							MyNPC.noClick = false;
							MyAnim.SetBool("Dead", value: false);
							MyAnim.SetBool("Patrol", value: false);
							MyAnim.SetBool("Walking", value: false);
							Myself.Alive = true;
							MyAnim.SetTrigger("Revive");
							deadTimer = 60f;
							MyNPC.retreat = false;
							base.transform.gameObject.layer = 9;
							GetComponent<CapsuleCollider>().center = new Vector3(0f, 1f, 0f);
							GetComponent<CapsuleCollider>().height = 2f;
							deadTimer = 180f;
							MyNPC.Revive();
						}
						else
						{
							deadTimer -= 60f * Time.deltaTime;
						}
					}
				}
				else
				{
					SimPlayerIndependentGroup simGroup2 = GameData.SimMngr.GetSimGroup(GameData.SimMngr.Sims[myIndex]);
					if (simGroup2.Lead == GameData.SimMngr.Sims[myIndex])
					{
						for (int num = simGroup2.Members.Count - 1; num >= 0; num--)
						{
							if (simGroup2.Members[num].MyAvatar != null && simGroup2.Members[num].MyAvatar != this)
							{
								UnityEngine.Object.Destroy(simGroup2.Members[num].MyAvatar.gameObject);
							}
						}
						simGroup2.Members.Clear();
						simGroup2.Lead = null;
						UnityEngine.Object.Destroy(base.gameObject);
					}
				}
			}
		}
		else if (Myself.Alive)
		{
			MyNav.speed = MyStats.actualRunSpeed;
			if (MyNav.isOnNavMesh)
			{
				MyNav.isStopped = false;
			}
			MyAnim.SetBool("Walking", value: true);
			MyAnim.SetBool("Patrol", value: false);
		}
		else if ((training || RunningAway || MyNPC.retreat) && !Myself.Alive)
		{
			training = false;
			RunningAway = false;
			MyNPC.retreat = false;
		}
		if (awaitInvite)
		{
			if (!askForInv && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 25f && GameData.SimMngr.Sims[myIndex].PersonalInvite)
			{
				UpdateSocialLog.LogAdd(Myself.GetComponent<NPC>().NPCName + " says: " + GameData.SimMngr.PersonalizeString("I'm here, joining your group now.", this));
				askForInv = true;
				awaitInvite = false;
				GameData.SimPlayerGrouping.InviteToGroup(Myself);
			}
		}
		else
		{
			askForInv = false;
		}
		if (MyStats.MyAura == null || (AuraSlot != null && AuraSlot.MyItem.Aura != null && AuraSlot.MyItem.Aura != MyStats.MyAura))
		{
			MyStats.MyAura = AuraSlot.MyItem.Aura;
		}
		if (UnityEngine.Random.Range(0, 1000) > 998)
		{
			do
			{
				randomizeOffset = new Vector3(UnityEngine.Random.Range(-3, 3), 0f, UnityEngine.Random.Range(-3, 3));
			}
			while (randomizeOffset.magnitude < 2f);
		}
		MyNPC.StuckMonitor();
	}

	private void ResetNearbyFriend()
	{
		if (GameData.GroupMembers[0] != null && GameData.GroupMembers[0].MyAvatar != null && GameData.GroupMembers[0].MyAvatar != this)
		{
			if (!GameData.GroupMembers[0].MyAvatar.Myself.NearbyFriends.Contains(Myself))
			{
				GameData.GroupMembers[0].MyAvatar.Myself.NearbyFriends.Add(Myself);
			}
			if (!Myself.NearbyFriends.Contains(GameData.GroupMembers[0].MyAvatar.Myself))
			{
				Myself.NearbyFriends.Add(GameData.GroupMembers[0].MyAvatar.Myself);
			}
		}
		if (GameData.GroupMembers[1] != null && GameData.GroupMembers[1].MyAvatar != null && GameData.GroupMembers[1].MyAvatar != this)
		{
			if (!GameData.GroupMembers[1].MyAvatar.Myself.NearbyFriends.Contains(Myself))
			{
				GameData.GroupMembers[1].MyAvatar.Myself.NearbyFriends.Add(Myself);
			}
			if (!Myself.NearbyFriends.Contains(GameData.GroupMembers[1].MyAvatar.Myself))
			{
				Myself.NearbyFriends.Add(GameData.GroupMembers[1].MyAvatar.Myself);
			}
		}
		if (GameData.GroupMembers[2] != null && GameData.GroupMembers[2].MyAvatar != null && GameData.GroupMembers[2].MyAvatar != this)
		{
			if (!GameData.GroupMembers[2].MyAvatar.Myself.NearbyFriends.Contains(Myself))
			{
				GameData.GroupMembers[2].MyAvatar.Myself.NearbyFriends.Add(Myself);
			}
			if (!Myself.NearbyFriends.Contains(GameData.GroupMembers[2].MyAvatar.Myself))
			{
				Myself.NearbyFriends.Add(GameData.GroupMembers[2].MyAvatar.Myself);
			}
		}
		if (GameData.GroupMembers[3] != null && GameData.GroupMembers[3].MyAvatar != null && GameData.GroupMembers[3].MyAvatar != this)
		{
			if (!GameData.GroupMembers[3].MyAvatar.Myself.NearbyFriends.Contains(Myself))
			{
				GameData.GroupMembers[3].MyAvatar.Myself.NearbyFriends.Add(Myself);
			}
			if (!Myself.NearbyFriends.Contains(GameData.GroupMembers[3].MyAvatar.Myself))
			{
				Myself.NearbyFriends.Add(GameData.GroupMembers[3].MyAvatar.Myself);
			}
		}
	}

	private bool IsSimGroupRelaxed()
	{
		if (GameData.PlayerControl.Myself.UnderThreat > 0f)
		{
			return false;
		}
		if (GameData.GroupMembers[0] != null && GameData.GroupMembers[0].MyAvatar != null && GameData.GroupMembers[0].MyAvatar != this && (GameData.GroupMembers[0].MyAvatar.MyStats.Myself.UnderThreat > 0f || GameData.GroupMembers[0].MyAvatar.MyNPC.CurrentAggroTarget != null))
		{
			return false;
		}
		if (GameData.GroupMembers[1] != null && GameData.GroupMembers[1].MyAvatar != null && GameData.GroupMembers[1].MyAvatar != this && (GameData.GroupMembers[1].MyAvatar.MyStats.Myself.UnderThreat > 0f || GameData.GroupMembers[1].MyAvatar.MyNPC.CurrentAggroTarget != null))
		{
			return false;
		}
		if (GameData.GroupMembers[2] != null && GameData.GroupMembers[2].MyAvatar != null && GameData.GroupMembers[2].MyAvatar != this && (GameData.GroupMembers[2].MyAvatar.MyStats.Myself.UnderThreat > 0f || GameData.GroupMembers[2].MyAvatar.MyNPC.CurrentAggroTarget != null))
		{
			return false;
		}
		if (GameData.GroupMembers[3] != null && GameData.GroupMembers[3].MyAvatar != null && GameData.GroupMembers[3].MyAvatar != this && (GameData.GroupMembers[3].MyAvatar.MyStats.Myself.UnderThreat > 0f || GameData.GroupMembers[3].MyAvatar.MyNPC.CurrentAggroTarget != null))
		{
			return false;
		}
		return true;
	}

	private bool IsSimGroupMemberAlive()
	{
		if (GameData.PlayerControl.Myself.Alive)
		{
			return true;
		}
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
		if (GameData.PlayerControl.Myself.Alive && Vector3.Distance(GameData.PlayerControl.transform.position, base.transform.position) < 7f)
		{
			return true;
		}
		if (GameData.GroupMembers[0] != null && GameData.GroupMembers[0].MyAvatar != null && GameData.GroupMembers[0].MyAvatar != this && GameData.GroupMembers[0].MyAvatar.MyStats.Myself.Alive && Vector3.Distance(GameData.GroupMembers[0].MyAvatar.transform.position, base.transform.position) < 7f)
		{
			return true;
		}
		if (GameData.GroupMembers[1] != null && GameData.GroupMembers[1].MyAvatar != null && GameData.GroupMembers[1].MyAvatar != this && GameData.GroupMembers[1].MyAvatar.MyStats.Myself.Alive && Vector3.Distance(GameData.GroupMembers[1].MyAvatar.transform.position, base.transform.position) < 7f)
		{
			return true;
		}
		if (GameData.GroupMembers[2] != null && GameData.GroupMembers[2].MyAvatar != null && GameData.GroupMembers[2].MyAvatar != this && GameData.GroupMembers[2].MyAvatar.MyStats.Myself.Alive && Vector3.Distance(GameData.GroupMembers[2].MyAvatar.transform.position, base.transform.position) < 7f)
		{
			return true;
		}
		if (GameData.GroupMembers[3] != null && GameData.GroupMembers[3].MyAvatar != null && GameData.GroupMembers[3].MyAvatar != this && GameData.GroupMembers[3].MyAvatar.MyStats.Myself.Alive && Vector3.Distance(GameData.GroupMembers[3].MyAvatar.transform.position, base.transform.position) < 7f)
		{
			return true;
		}
		return false;
	}

	public bool IsSimGroupInCombat()
	{
		if (GameData.Autoattacking)
		{
			return true;
		}
		if (GameData.GroupMembers[0] != null && GameData.GroupMembers[0].MyAvatar != null && GameData.GroupMembers[0].MyAvatar != this && GameData.GroupMembers[0].MyAvatar.MyNPC.CurrentAggroTarget != null)
		{
			return true;
		}
		if (GameData.GroupMembers[1] != null && GameData.GroupMembers[1].MyAvatar != null && GameData.GroupMembers[1].MyAvatar != this && GameData.GroupMembers[1].MyAvatar.MyNPC.CurrentAggroTarget != null)
		{
			return true;
		}
		if (GameData.GroupMembers[2] != null && GameData.GroupMembers[2].MyAvatar != null && GameData.GroupMembers[2].MyAvatar != this && GameData.GroupMembers[2].MyAvatar.MyNPC.CurrentAggroTarget != null)
		{
			return true;
		}
		if (GameData.GroupMembers[3] != null && GameData.GroupMembers[3].MyAvatar != null && GameData.GroupMembers[3].MyAvatar != this && GameData.GroupMembers[3].MyAvatar.MyNPC.CurrentAggroTarget != null)
		{
			return true;
		}
		return false;
	}

	private void FindNewTask()
	{
		MyPOI = null;
		hailed = false;
		shouted = false;
		if (!GameData.InDungeon)
		{
			MyTask = (PointOfInterest.POIType)UnityEngine.Random.Range(0, 3);
			if (UnityEngine.Random.Range(0, 10) > 8)
			{
				MyTask = PointOfInterest.POIType.vendor;
			}
			if (UnityEngine.Random.Range(0, 10) > 6)
			{
				MyTask = PointOfInterest.POIType.talk;
				hailed = false;
			}
			else if (UnityEngine.Random.Range(0, 10) > 8)
			{
				MyTask = PointOfInterest.POIType.zoneline;
			}
			if (MyTask == PointOfInterest.POIType.rest || MyTask == PointOfInterest.POIType.vendor)
			{
				TimeOnTask = UnityEngine.Random.Range(240, 600);
			}
			else if (MyTask == PointOfInterest.POIType.talk)
			{
				TimeOnTask = UnityEngine.Random.Range(240, 600);
			}
			else
			{
				TimeOnTask = UnityEngine.Random.Range(1200, 3200);
			}
			if (!GameData.SimMngr.IsSimGrouped(GameData.SimMngr.Sims[myIndex]))
			{
				if (UnityEngine.Random.Range(0, 10) > 2)
				{
					MyTask = PointOfInterest.POIType.vendor;
					TimeOnTask = UnityEngine.Random.Range(300, 900);
				}
				else
				{
					MyTask = PointOfInterest.POIType.safe;
					TimeOnTask = UnityEngine.Random.Range(600, 4800);
				}
				if (UnityEngine.Random.Range(0, 10) > 8)
				{
					MyTask = PointOfInterest.POIType.talk;
					TimeOnTask = UnityEngine.Random.Range(300, 900);
				}
				if (MyTask != PointOfInterest.POIType.zoneline)
				{
					IgnoreAllCombat = false;
					TimeOnTask = UnityEngine.Random.Range(600, 3200);
				}
			}
		}
		else if (!GameData.SimMngr.IsSimGrouped(GameData.SimMngr.Sims[myIndex]))
		{
			TimeOnTask = UnityEngine.Random.Range(12000, 24000);
			IgnoreAllCombat = true;
			MyTask = PointOfInterest.POIType.zoneline;
		}
		else
		{
			MyTask = PointOfInterest.POIType.camp;
			if (UnityEngine.Random.Range(0, 100) > 98)
			{
				MyTask = PointOfInterest.POIType.safe;
			}
			if (MyTask != PointOfInterest.POIType.zoneline)
			{
				IgnoreAllCombat = false;
			}
			TimeOnTask = UnityEngine.Random.Range(6400, 7200);
			if (UnityEngine.Random.Range(0, 100) > 98)
			{
				MyTask = PointOfInterest.POIType.zoneline;
			}
		}
	}

	private void ForceRest()
	{
		resting = true;
	}

	private void GoToTask()
	{
		if (MyNPC.HailTimer > 0f)
		{
			MyNav.velocity = Vector3.zero;
			if (MyNav.isOnNavMesh)
			{
				MyNav.isStopped = true;
			}
			MyAnim.SetBool("Walking", value: false);
			MyAnim.SetBool("Patrol", value: false);
			if (!sitting)
			{
				base.transform.LookAt(new Vector3(GameData.PlayerControl.transform.position.x, base.transform.position.y, GameData.PlayerControl.transform.position.z));
			}
		}
		else if (GameData.SimMngr.Sims[myIndex].seekingPlayer && !GameData.SimMngr.IsSimGrouped(GameData.SimMngr.Sims[myIndex]))
		{
			if (Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) > 4f && !resting && !MyStats.Rooted && !MySpells.isCasting())
			{
				MyNav.speed = MyStats.actualRunSpeed;
				if (MyNav.isOnNavMesh)
				{
					MyNav.isStopped = false;
				}
				MyAnim.SetBool("Walking", value: true);
				MyAnim.SetBool("Patrol", value: false);
				if (MyNPC.NeedsNavUpdate(GameData.PlayerControl.transform.position + randomizeOffset))
				{
					MyNPC.HighPriorityNavUpdate(GameData.PlayerControl.transform.position + randomizeOffset);
				}
				MyPOI = null;
			}
			else if (Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) <= 4f)
			{
				TimeOnTask -= 60f * Time.deltaTime;
				MyNav.velocity = Vector3.zero;
				if (MyNav.isOnNavMesh)
				{
					MyNav.isStopped = true;
				}
				MyAnim.SetBool("Walking", value: false);
				MyAnim.SetBool("Patrol", value: false);
				GameData.SimMngr.Sims[myIndex].seekingPlayer = false;
				WaitForInv = 1200f;
				MyPOI = null;
			}
			else
			{
				TimeOnTask -= 60f * Time.deltaTime;
				MyNav.velocity = Vector3.zero;
				if (MyNav.isOnNavMesh)
				{
					MyNav.isStopped = true;
				}
				MyAnim.SetBool("Walking", value: false);
				MyAnim.SetBool("Patrol", value: false);
			}
		}
		else if ((WaitForInv <= 0f && !GameData.SimMngr.IsSimGrouped(GameData.SimMngr.Sims[myIndex])) || MyNPC.retreat)
		{
			if (MyPOI == null)
			{
				randomizeYRot = UnityEngine.Random.Range(0, 360);
				List<PointOfInterest> list = new List<PointOfInterest>();
				for (int i = 0; i < POI.POIs.Count; i++)
				{
					if (POI.POIs[i].Use == MyTask)
					{
						list.Add(POI.POIs[i]);
					}
				}
				if (list.Count > 0)
				{
					MyPOI = list[UnityEngine.Random.Range(0, list.Count)];
				}
				else if (!IsGMCharacter)
				{
					FindNewTask();
				}
			}
			else
			{
				if (!(MyNPC.CurrentAggroTarget == null))
				{
					return;
				}
				if (Vector3.Distance(base.transform.position, MyPOI.transform.position + randomizeOffset) > 4f && !resting && !MyStats.Rooted && !MySpells.isCasting())
				{
					if (MyNav.isOnNavMesh)
					{
						MyNav.isStopped = false;
					}
					if (MyNav.velocity.magnitude > 0.1f)
					{
						MyAnim.SetBool("Walking", value: true);
						MyAnim.SetBool("Patrol", value: false);
					}
					if (!training && !MyNav.pathPending)
					{
						MyNav.SetDestination(GameData.GetSafeNavMeshPoint(MyPOI.transform.position + randomizeOffset));
					}
					return;
				}
				TimeOnTask -= 60f * Time.deltaTime;
				if (!hailed && MyTask == PointOfInterest.POIType.talk && Vector3.Distance(base.transform.position, MyPOI.transform.position) <= 4f)
				{
					hailed = true;
					if (MyPOI != null && MyPOI.NPCName != null && !IsGMCharacter && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) <= 30f)
					{
						UpdateSocialLog.LogAdd(MyNPC.NPCName + " says: Hail, " + MyPOI.NPCName);
						UpdateSocialLog.LogAdd(MyPOI.NPCName + " mutters something inaudible to " + MyNPC.NPCName);
					}
					base.transform.LookAt(MyPOI.transform.position + Vector3.up);
					if (MyPOI.transform.parent != null)
					{
						MyPOI.transform.parent.LookAt(new Vector3(base.transform.position.x, MyPOI.transform.parent.position.y, base.transform.position.z));
					}
				}
				if (!shouted && !IsGMCharacter && MyPOI.TaskRequiresGroup && MyStats.Level >= MyPOI.LvlRec && MyStats.Level < MyPOI.LvlRec + 2 && TimeOnTask > 10f)
				{
					shouted = true;
					UpdateSocialLog.LogAdd(MyNPC.NPCName + " shouts: " + GameData.SimMngr.PersonalizeString(MyDialog.LFGPublic[UnityEngine.Random.Range(0, MyDialog.LFGPublic.Count)] + " " + MyPOI.AreaName + " you can lead.", this), "#FF9000");
					awaitInvite = true;
					WaitForInv = 2200f;
					GameData.ShoutParse.ParseShout(GetComponent<NPC>().NPCName, MyNPC.NPCName, _isPlayer: false);
					AskForInv = false;
				}
				MyNav.velocity = Vector3.zero;
				if (MyNav.isOnNavMesh)
				{
					MyNav.isStopped = true;
				}
				MyAnim.SetBool("Walking", value: false);
				MyAnim.SetBool("Patrol", value: false);
				if (!IsGMCharacter)
				{
					if (Mathf.Abs(base.transform.eulerAngles.y - randomizeYRot) > 10f)
					{
						MyAnim.SetBool("pivotRight", value: true);
						base.transform.Rotate(Vector3.up, 120f * Time.deltaTime);
					}
					else
					{
						MyAnim.SetBool("pivotRight", value: false);
					}
				}
			}
		}
		else
		{
			if (!GameData.SimMngr.IsSimGrouped(GameData.SimMngr.Sims[myIndex]))
			{
				return;
			}
			SimPlayerIndependentGroup simGroup = GameData.SimMngr.GetSimGroup(GameData.SimMngr.Sims[myIndex]);
			if (simGroup == null)
			{
				return;
			}
			if (simGroup.AmILeader(GameData.SimMngr.Sims[myIndex]))
			{
				if (simGroup.OnlyForGodTarg && GameData.LiveGodTargetPOI != null && MyPOI != GameData.LiveGodTargetPOI)
				{
					MyPOI = GameData.LiveGodTargetPOI;
				}
				if (MyPOI == null)
				{
					List<PointOfInterest> list2 = new List<PointOfInterest>();
					for (int j = 0; j < POI.POIs.Count; j++)
					{
						if (POI.POIs[j].Use == MyTask && Mathf.Abs(POI.POIs[j].LvlRec - simGroup.LevelOfGroup) < 3)
						{
							list2.Add(POI.POIs[j]);
						}
					}
					if (list2.Count > 0)
					{
						MyPOI = list2[UnityEngine.Random.Range(0, list2.Count)];
					}
					else if (GameData.EgressLocations.Count > 0)
					{
						MyPOI = GameData.EgressLocations[UnityEngine.Random.Range(0, GameData.EgressLocations.Count)];
					}
				}
				else if (MyNPC.CurrentAggroTarget == null)
				{
					float num = MyStats.actualRunSpeed;
					bool value = true;
					bool value2 = false;
					bool flag = false;
					if (simGroup.DoesGroupNeedRest())
					{
						value = false;
						value2 = false;
						MyNav.speed = 0f;
						if (MyNav.isOnNavMesh)
						{
							MyNav.isStopped = true;
						}
						MyAnim.SetBool("Walking", value);
						MyAnim.SetBool("Patrol", value2);
						flag = true;
					}
					else if (simGroup.CheckDistanceBetweenAll(base.transform) > 12f && !MyNav.isStopped && !MySpells.isCasting() && !MyStats.Rooted)
					{
						value2 = true;
						value = false;
						num *= 0.5f;
						MyNav.speed = num;
						if (MyNav.isOnNavMesh)
						{
							MyNav.isStopped = false;
						}
						MyAnim.SetBool("Walking", value);
						MyAnim.SetBool("Patrol", value2);
					}
					else if (!MySpells.isCasting() && !MyStats.Rooted)
					{
						MyNav.speed = num;
						if (MyNav.isOnNavMesh)
						{
							MyNav.isStopped = false;
						}
						MyAnim.SetBool("Walking", value);
						MyAnim.SetBool("Patrol", value2);
					}
					if (Myself.NearbyEnemies.Count > 0)
					{
						if (Myself.NearbyEnemies[0] != null && Myself.NearbyEnemies[0].Alive)
						{
							MyNPC.CurrentAggroTarget = Myself.NearbyEnemies[0];
							simGroup.CallForAssist(MyNPC.CurrentAggroTarget);
						}
					}
					else if (Vector3.Distance(base.transform.position, MyPOI.transform.position + randomizeOffset) > 4f && !flag && !MyStats.Rooted && !MySpells.isCasting() && !flag)
					{
						MyNav.speed = num;
						if (MyNav.isOnNavMesh)
						{
							MyNav.isStopped = false;
						}
						MyAnim.SetBool("Walking", value);
						MyAnim.SetBool("Patrol", value2);
						MyNav.SetDestination(GameData.GetSafeNavMeshPoint(MyPOI.transform.position + randomizeOffset));
					}
					else
					{
						if (flag)
						{
							return;
						}
						TimeOnTask -= 60f * Time.deltaTime;
						if (!hailed && MyTask == PointOfInterest.POIType.talk && Vector3.Distance(base.transform.position, MyPOI.transform.position) <= 4f)
						{
							hailed = true;
							if (Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) <= 15f)
							{
								UpdateSocialLog.LogAdd(MyNPC.NPCName + " says: Hail, " + MyPOI.NPCName);
								UpdateSocialLog.LogAdd(MyPOI.NPCName + " mutters something inaudible to " + MyNPC.NPCName);
							}
							base.transform.LookAt(MyPOI.transform.position + Vector3.up);
							if (MyPOI.transform.parent != null)
							{
								MyPOI.transform.parent.LookAt(new Vector3(base.transform.position.x, MyPOI.transform.parent.position.y, base.transform.position.z));
							}
						}
						if (!shouted && MyPOI.TaskRequiresGroup && MyStats.Level >= MyPOI.LvlRec && MyStats.Level < MyPOI.LvlRec + 2 && TimeOnTask > 10f)
						{
							shouted = true;
							UpdateSocialLog.LogAdd(MyNPC.NPCName + " shouts: " + GameData.SimMngr.PersonalizeString(MyDialog.LFGPublic[UnityEngine.Random.Range(0, MyDialog.LFGPublic.Count)] + " " + MyPOI.AreaName + " you can lead.", this), "#FF9000");
							awaitInvite = true;
							WaitForInv = 2200f;
							GameData.ShoutParse.ParseShout(GetComponent<NPC>().NPCName, MyNPC.NPCName, _isPlayer: false);
							AskForInv = false;
						}
						MyNav.velocity = Vector3.zero;
						if (MyNav.isOnNavMesh)
						{
							MyNav.isStopped = true;
						}
						MyAnim.SetBool("Walking", value: false);
						MyAnim.SetBool("Patrol", value: false);
					}
				}
				else if (MyNPC.CurrentAggroTarget != null)
				{
					simGroup.CallForAssist(MyNPC.CurrentAggroTarget);
				}
			}
			else if (MyNPC.CurrentAggroTarget == null)
			{
				if (simGroup.Lead?.MyAvatar == null)
				{
					return;
				}
				if (Vector3.Distance(base.transform.position, simGroup.Lead.MyAvatar.MyNPC.transform.position) > 4f && !MySpells.isCasting())
				{
					float num2 = simGroup?.Lead?.MyAvatar?.MyStats.actualRunSpeed ?? 10f;
					if (num2 == 0f)
					{
						num2 = MyStats.actualRunSpeed;
					}
					MyNav.speed = num2;
					if (MyNav.isOnNavMesh)
					{
						MyNav.isStopped = false;
					}
					MyAnim.SetBool("Walking", value: true);
					MyAnim.SetBool("Patrol", value: false);
					MyNav.SetDestination(GameData.GetSafeNavMeshPoint(simGroup.Lead.MyAvatar.MyNPC.transform.position + randomizeOffset));
				}
				else
				{
					MyNav.speed = 0f;
					if (MyNav.isOnNavMesh)
					{
						MyNav.isStopped = true;
					}
					MyAnim.SetBool("Walking", value: false);
					MyAnim.SetBool("Patrol", value: false);
				}
			}
			else if (MyNPC.CurrentAggroTarget != null && simGroup.GroupMembersAlive() > 1)
			{
				if (simGroup == null)
				{
					return;
				}
				if (simGroup.Lead != null && simGroup.Lead.MyAvatar != null)
				{
					if (simGroup.Lead.MyAvatar.MyNPC.CurrentAggroTarget == null)
					{
						simGroup.Lead.MyAvatar.MyNPC.CurrentAggroTarget = MyNPC.CurrentAggroTarget;
					}
				}
				else
				{
					simGroup.CallForAssist(MyNPC.CurrentAggroTarget);
				}
			}
			else
			{
				if (!(MyNPC.CurrentAggroTarget != null) || simGroup.GroupMembersAlive() != 1)
				{
					return;
				}
				TimeOnTask = UnityEngine.Random.Range(12000, 24000);
				if (!IgnoreAllCombat && MyTask != PointOfInterest.POIType.zoneline && GameData.EgressLocations.Count > 0)
				{
					GameData.SimMngr.GetSimGroup(GameData.SimMngr.Sims[myIndex]).Lead = GameData.SimMngr.Sims[myIndex];
					IgnoreAllCombat = true;
					UpdateSocialLog.LogAdd(MyNPC.NPCName + " shouts: " + GameData.SimMngr.PersonalizeString("TRAIN!!! " + MyNPC.CurrentAggroTarget.transform.name.ToUpper() + " TO ZONE!!", this), "#FF9000");
					MyTask = PointOfInterest.POIType.zoneline;
					MyNPC.retreat = true;
					MyNav.speed = MyStats.actualRunSpeed;
					if (MyNav.isOnNavMesh)
					{
						MyNav.isStopped = false;
					}
					MyPOI = GameData.EgressLocations[UnityEngine.Random.Range(0, GameData.EgressLocations.Count)];
					MyAnim.SetBool("Walking", value: true);
				}
			}
		}
	}

	public void ZoneSim(Zoneline zl)
	{
		SaveSim();
		GameData.SimMngr.SimChangeScene(GameData.SimMngr.Sims[myIndex], zl.DestinationZone, zl.LandingPosition, _FromTut: false);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void FollowPlayer()
	{
		if (!(MyNPC.CurrentAggroTarget == null) || MySpells.isCasting())
		{
			return;
		}
		TimeOnTask -= 60f * Time.deltaTime;
		if (Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position + randomizeOffset) >= 7f)
		{
			MyNav.speed = MyStats.actualRunSpeed;
			if (MyNav.isOnNavMesh)
			{
				MyNav.isStopped = false;
			}
			MyAnim.SetBool("Walking", value: true);
			MyAnim.SetBool("Patrol", value: false);
			if (MyNPC.NeedsNavUpdate(GameData.PlayerControl.transform.position + randomizeOffset))
			{
				MyNPC.HighPriorityNavUpdate(GameData.GetSafeNavMeshPoint(GameData.PlayerControl.transform.position) + randomizeOffset);
			}
		}
		if (!(Vector3.Distance(base.transform.position, MyNav.destination) < 5f))
		{
			return;
		}
		if (Vector3.Distance(base.transform.position, MyNav.destination) >= 1f)
		{
			MyNav.speed = 3f;
			if (MyNav.isOnNavMesh)
			{
				MyNav.isStopped = false;
			}
			MyAnim.SetBool("Walking", value: false);
			MyAnim.SetBool("Patrol", value: true);
		}
		else
		{
			MyNav.speed = 0f;
			MyNav.velocity = Vector3.zero;
			if (MyNav.isOnNavMesh)
			{
				MyNav.isStopped = true;
			}
			MyAnim.SetBool("Walking", value: false);
			MyAnim.SetBool("Patrol", value: false);
		}
		if (MyNPC.NeedsNavUpdate(GameData.PlayerControl.transform.position + randomizeOffset))
		{
			MyNPC.HighPriorityNavUpdate(GameData.GetSafeNavMeshPoint(GameData.PlayerControl.transform.position) + randomizeOffset);
		}
	}

	public void AssignGuardSpot(Vector3 pos)
	{
		GuardPos = pos + randomizeOffset;
		GuardSpot = true;
	}

	public void FreeFollow()
	{
		GuardSpot = false;
	}

	public bool CheckNearbyGroupTargetsForEngagement()
	{
		Character character = GameData.GroupMembers[0]?.MyAvatar?.Myself;
		Character character2 = GameData.GroupMembers[1]?.MyAvatar?.Myself;
		Character character3 = GameData.GroupMembers[2]?.MyAvatar?.Myself;
		Character character4 = GameData.GroupMembers[3]?.MyAvatar?.Myself;
		foreach (Character groupTarget in GameData.SimPlayerGrouping.GroupTargets)
		{
			if ((object)groupTarget != null && groupTarget.Alive && groupTarget.MyNPC?.CurrentAggroTarget != null && (groupTarget.MyNPC.CurrentAggroTarget == character || groupTarget.MyNPC.CurrentAggroTarget == character2 || groupTarget.MyNPC.CurrentAggroTarget == character3 || groupTarget.MyNPC.CurrentAggroTarget == character4))
			{
				return true;
			}
		}
		return false;
	}

	private Transform GetGroupAggroTarget(Character mob)
	{
		if (mob?.MyNPC?.CurrentAggroTarget == null)
		{
			return null;
		}
		Character currentAggroTarget = mob.MyNPC.CurrentAggroTarget;
		if (currentAggroTarget == GameData.GroupMembers[0]?.MyAvatar?.Myself || currentAggroTarget == GameData.GroupMembers[1]?.MyAvatar?.Myself || currentAggroTarget == GameData.GroupMembers[2]?.MyAvatar?.Myself || currentAggroTarget == GameData.GroupMembers[3]?.MyAvatar?.Myself)
		{
			return currentAggroTarget.transform;
		}
		return null;
	}

	private bool CheckPullReadiness()
	{
		if (MyStats.CurrentHP < Mathf.RoundToInt(MyStats.CurrentMaxHP / 2) && (PullSpell == null || MyStats.CurrentMana < PullSpell.ManaCost))
		{
			return false;
		}
		if (GameData.SimPlayerGrouping.Heals.Count > 0)
		{
			foreach (SimPlayerTracking heal in GameData.SimPlayerGrouping.Heals)
			{
				if (heal != null && !(heal.MyAvatar == null) && !(heal.MyAvatar.MyStats == null) && (float)heal.MyAvatar.MyStats.CurrentMana < (float)heal.MyAvatar.MyStats.GetCurrentMaxMana() * GameData.SimPlayerGrouping.ManaNeededForPull)
				{
					return false;
				}
			}
		}
		return true;
	}

	private void DoPulling()
	{
		bool flag = false;
		if (PullTarget == null || !PullTarget.Alive)
		{
			CurrentPullPhase = PullPhases.FindTarget;
			PullTarget = null;
		}
		if (CurrentPullPhase == PullPhases.NotPulling && (PullTarget == null || !PullTarget.Alive))
		{
			PullTarget = null;
			CurrentPullPhase = PullPhases.FindTarget;
		}
		if (PullTarget != null && GameData.SimPlayerGrouping.ForcePullTarget != null && PullTarget != GameData.SimPlayerGrouping.ForcePullTarget)
		{
			PullTarget = GameData.SimPlayerGrouping.ForcePullTarget;
			CurrentPullPhase = PullPhases.GoToTarget;
		}
		if (PullTarget != null && !IsPullTargetAggrodOnGroup(PullTarget) && PullTarget.MyNPC.CurrentAggroTarget != Myself)
		{
			CurrentPullPhase = PullPhases.GoToTarget;
		}
		if (PullTarget != null && IsPullTargetAggrodOnGroup(PullTarget) && PullTarget.MyNPC.CurrentAggroTarget != Myself)
		{
			CurrentPullPhase = PullPhases.AttackTarget;
		}
		if (MyNPC.CurrentAggroTarget != null && MyNPC.CurrentAggroTarget != PullTarget)
		{
			PullTarget = MyNPC.CurrentAggroTarget;
		}
		if (CurrentPullPhase == PullPhases.FindTarget)
		{
			bool flag2 = MyNPC.CurrentAggroTarget == null && !CheckNearbyGroupTargetsForEngagement() && GameData.SimPlayerGrouping.PullConstant && (PullTarget == null || (PullTarget != null && (!PullTarget.Alive || PullTarget.MyStats.Charmed)));
			if (GameData.SimPlayerGrouping?.ForcePullTarget != null)
			{
				flag2 = false;
				PullTarget = GameData.SimPlayerGrouping.ForcePullTarget;
				GameData.SimPlayerGrouping.ForcePullTarget = null;
				flag = true;
			}
			else
			{
				flag = false;
			}
			if (flag2)
			{
				if (PullTarget != null && !PullTarget.Alive)
				{
					PullTarget = null;
				}
				if (healAskCD > 0f)
				{
					healAskCD -= 60f * Time.deltaTime;
				}
				MyNPC.CurrentAggroTarget = null;
				if (CheckPullReadiness())
				{
					MyAnim.SetBool("Walking", value: false);
					MyAnim.SetBool("Patrol", value: false);
					holdPullsForHeals = false;
					if (GameData.PlayerControl.CurrentTarget != null && GameData.PlayerControl.CurrentTarget.AggressiveTowards.Contains(Myself.MyFaction))
					{
						PullTarget = GameData.PlayerControl.CurrentTarget;
					}
					if (PullTarget == null)
					{
						PullTarget = FindNearestSpawn();
					}
				}
				else
				{
					holdPullsForHeals = true;
					if (healAskCD <= 0f)
					{
						InCampWhilePulling = true;
						MyAnim.SetBool("Walking", value: false);
						MyAnim.SetBool("Patrol", value: false);
						if (PullSpell != null && MyStats.CurrentMana < PullSpell.ManaCost)
						{
							switch (UnityEngine.Random.Range(0, 3))
							{
							case 0:
								UpdateSocialLog.LogAdd(base.transform.name + " tells the group: " + GameData.SimMngr.PersonalizeString("need mana... ", this), "#00B2B7");
								break;
							case 1:
								UpdateSocialLog.LogAdd(base.transform.name + " tells the group: " + GameData.SimMngr.PersonalizeString("medding up for a sec... ", this), "#00B2B7");
								break;
							default:
								UpdateSocialLog.LogAdd(base.transform.name + " tells the group: " + GameData.SimMngr.PersonalizeString("I'm OOM hang on... ", this), "#00B2B7");
								break;
							}
						}
						else if (MyStats.CurrentHP < Mathf.RoundToInt(MyStats.CurrentMaxHP / 2))
						{
							switch (UnityEngine.Random.Range(0, 3))
							{
							case 0:
								UpdateSocialLog.LogAdd(base.transform.name + " tells the group: " + GameData.SimMngr.PersonalizeString("Waiting on a heal", this), "#00B2B7");
								break;
							case 1:
								UpdateSocialLog.LogAdd(base.transform.name + " tells the group: " + GameData.SimMngr.PersonalizeString("gotta heal up.", this), "#00B2B7");
								break;
							default:
								UpdateSocialLog.LogAdd(base.transform.name + " tells the group: " + GameData.SimMngr.PersonalizeString("need some life before I pull more.", this), "#00B2B7");
								break;
							}
						}
						else
						{
							UpdateSocialLog.LogAdd(base.transform.name + " tells the group: " + GameData.SimMngr.PersonalizeString("waiting on group mana...", this), "#00B2B7");
						}
						healAskCD = UnityEngine.Random.Range(100, 340);
					}
				}
			}
			else if (!flag2 && PullTarget == null && !GameData.SimPlayerGrouping.PullConstant)
			{
				GameData.SimMngr.Sims[myIndex].isPuller = false;
				IgnoreAllCombat = false;
			}
			if (PullTarget != null)
			{
				CurrentPullPhase = PullPhases.GoToTarget;
			}
		}
		if (CurrentPullPhase == PullPhases.GoToTarget)
		{
			float num = PullSpell?.SpellRange ?? 10f;
			IgnoreAllCombat = true;
			if (Vector3.Distance(base.transform.position, PullTarget.transform.position) <= num && MyNPC.CheckLOS(PullTarget))
			{
				CurrentPullPhase = PullPhases.EngageTarget;
			}
			else if (!MyNav.pathPending)
			{
				EnableAllNavAgentFunction();
				MyNav.SetDestination(GameData.GetSafeNavMeshPoint(PullTarget.transform.position));
			}
			Character character = DetectAddsOnWayToPull();
			if (character != null && !flag)
			{
				PullTarget = character;
				CurrentPullPhase = PullPhases.EngageTarget;
			}
		}
		if (CurrentPullPhase == PullPhases.EngageTarget)
		{
			IgnoreAllCombat = true;
			if (Vector3.Distance(PullTarget.transform.position, GameData.SimPlayerGrouping.GetPullDestination(base.transform).position) < 10f || (Vector3.Distance(PullTarget.transform.position, GameData.SimPlayerGrouping.GetPullDestination(base.transform).position) < 30f && PullTarget.MyNPC.IsCasting()))
			{
				CurrentPullPhase = PullPhases.AttackTarget;
			}
			if (PullTarget.MyNPC.CurrentAggroTarget == Myself && CurrentPullPhase == PullPhases.EngageTarget)
			{
				CurrentPullPhase = PullPhases.ReturnTarget;
				UpdateSocialLog.LogAdd(base.transform.name + " tells the group: " + GameData.SimMngr.PersonalizeString("pulling " + PullTarget.transform.name, this), "#00B2B7");
			}
			if (PullTarget == null || !PullTarget.Alive)
			{
				CurrentPullPhase = PullPhases.FindTarget;
				return;
			}
			if (PullSpell != null && !MyNPC.IsCasting() && MyStats.CurrentMana > PullSpell.ManaCost && CurrentPullPhase == PullPhases.EngageTarget)
			{
				if (MyStats.Invisible)
				{
					MyStats.BreakEffectsOnAction();
				}
				DisableAllNavAgentFunction();
				MyNPC.ForceSpellCast(PullSpell, PullTarget.MyStats);
				MyNPC.ForceAggroOn(PullTarget);
				IgnoreAllCombat = true;
			}
			else if (!MyNPC.IsCasting() && CurrentPullPhase == PullPhases.EngageTarget)
			{
				if (MyStats.Invisible)
				{
					MyStats.BreakEffectsOnAction();
				}
				DisableAllNavAgentFunction();
				MyNPC.ForceAggroOn(PullTarget);
				PullTarget.MyNPC.ForceAggroOn(Myself);
				PullTarget.DamageMe(-1, _fromPlayer: false, GameData.DamageType.Physical, Myself, _animEffect: false, _criticalHit: false);
				IgnoreAllCombat = true;
			}
		}
		if (CurrentPullPhase == PullPhases.ReturnTarget)
		{
			IgnoreAllCombat = true;
			if (IsPullTargetAggrodOnGroup(PullTarget) && PullTarget.MyNPC.CurrentAggroTarget != Myself)
			{
				CurrentPullPhase = PullPhases.AttackTarget;
			}
			if (PullTarget.MyNPC.CurrentAggroTarget != Myself)
			{
				if (!IsPullTargetAggrodOnGroup(PullTarget))
				{
					CurrentPullPhase = PullPhases.GoToTarget;
				}
				else if (IsPullTargetAggrodOnGroup(PullTarget))
				{
					CurrentPullPhase = PullPhases.AttackTarget;
				}
				return;
			}
			if (PullTarget == null || !PullTarget.Alive)
			{
				CurrentPullPhase = PullPhases.FindTarget;
				return;
			}
			if (Vector3.Distance(base.transform.position, GameData.SimPlayerGrouping.GetPullDestination(base.transform).position) > 4f)
			{
				if (!MyNav.pathPending)
				{
					EnableAllNavAgentFunction();
					MyNav.SetDestination(GameData.GetSafeNavMeshPoint(GameData.SimPlayerGrouping.GetPullDestination(base.transform).position + new Vector3(UnityEngine.Random.Range(-1, 1), 0f, UnityEngine.Random.Range(-2, 2))));
				}
			}
			else
			{
				DisableAllNavAnimations();
				CurrentPullPhase = PullPhases.AttackTarget;
			}
		}
		if (CurrentPullPhase != PullPhases.AttackTarget)
		{
			return;
		}
		if (PullTarget.MyNPC.CurrentAggroTarget != Myself && !IsPullTargetAggrodOnGroup(PullTarget) && Vector3.Distance(PullTarget.transform.position, GameData.SimPlayerGrouping.GetPullDestination(base.transform).position) > 12f)
		{
			CurrentPullPhase = PullPhases.GoToTarget;
			return;
		}
		if (PullTarget == null || !PullTarget.Alive)
		{
			CurrentPullPhase = PullPhases.FindTarget;
			return;
		}
		if (Vector3.Distance(base.transform.position, GameData.SimPlayerGrouping.GetPullDestination(base.transform).position) > 25f)
		{
			CurrentPullPhase = PullPhases.ReturnTarget;
		}
		if ((!IsPullTargetAggrodOnGroup(PullTarget) || !(PullTarget.MyNPC.CurrentAggroTarget != Myself)) && !(Vector3.Distance(PullTarget.transform.position, GameData.SimPlayerGrouping.GetPullDestination(base.transform).position) <= 12f) && (!(Vector3.Distance(PullTarget.transform.position, GameData.SimPlayerGrouping.GetPullDestination(base.transform).position) < 30f) || !PullTarget.MyNPC.IsCasting()))
		{
			return;
		}
		Vector3 worldPosition = new Vector3(PullTarget.transform.position.x, base.transform.position.y, PullTarget.transform.position.z);
		base.transform.LookAt(worldPosition);
		EnableAllNavAgentFunction();
		if (!IsGroupAggrodOnTarget(PullTarget))
		{
			GameData.SimPlayerGrouping.GroupAttack(PullTarget);
			if (PullTarget != null && atkCalloutCD <= 0f)
			{
				UpdateSocialLog.LogAdd(base.transform.name + " tells the group: " + GameData.SimMngr.PersonalizeString(PullTarget.transform.name + " is here, attack it!", this), "#00B2B7");
				atkCalloutCD = 180f;
			}
		}
		CurrentPullPhase = PullPhases.NotPulling;
		IgnoreAllCombat = false;
	}

	private Character DetectAddsOnWayToPull()
	{
		if (Myself.NearbyEnemies.Count == 0)
		{
			return null;
		}
		foreach (Character nearbyEnemy in Myself.NearbyEnemies)
		{
			if (nearbyEnemy != null && nearbyEnemy.MyNPC != null && nearbyEnemy.MyNPC.CurrentAggroTarget == Myself)
			{
				return nearbyEnemy;
			}
		}
		foreach (Character nearbyEnemy2 in Myself.NearbyEnemies)
		{
			if (nearbyEnemy2 != null && nearbyEnemy2.MyFaction == PullTarget.MyFaction && MyNPC.CheckLOS(nearbyEnemy2))
			{
				return nearbyEnemy2;
			}
		}
		return null;
	}

	private bool IsPullTargetAggrodOnGroup(Character _targ)
	{
		if (_targ == null)
		{
			return false;
		}
		if (_targ.MyNPC == null)
		{
			return false;
		}
		if (_targ.MyNPC.CurrentAggroTarget == null)
		{
			return false;
		}
		if (_targ.MyNPC.CurrentAggroTarget == GameData.PlayerControl.Myself)
		{
			return true;
		}
		SimPlayerTracking[] groupMembers = GameData.GroupMembers;
		foreach (SimPlayerTracking simPlayerTracking in groupMembers)
		{
			if (_targ.MyNPC.CurrentAggroTarget == simPlayerTracking?.MyStats?.Myself)
			{
				return true;
			}
		}
		return false;
	}

	private void EnableAllNavAgentFunction()
	{
		MyNav.speed = MyStats.actualRunSpeed;
		if (MyNav.isOnNavMesh)
		{
			MyNav.isStopped = false;
		}
		MyAnim.SetBool("Walking", value: true);
		MyAnim.SetBool("Patrol", value: false);
	}

	private void DisableAllNavAgentFunction()
	{
		MyNav.speed = 0f;
		if (MyNav.isOnNavMesh)
		{
			MyNav.isStopped = true;
		}
		MyNav.SetDestination(base.transform.position);
		MyAnim.SetBool("Walking", value: false);
		MyAnim.SetBool("Patrol", value: false);
	}

	private void DisableAllNavAnimations()
	{
		if (MyNav.isOnNavMesh)
		{
			MyNav.isStopped = true;
		}
		MyAnim.SetBool("Walking", value: false);
		MyAnim.SetBool("Patrol", value: false);
	}

	private void DoPullingDEPRECATED()
	{
	}

	private bool EnemyInCamp()
	{
		foreach (Character nearbyEnemy in Myself.NearbyEnemies)
		{
			if (nearbyEnemy != null && nearbyEnemy.Alive && !nearbyEnemy.MyStats.Charmed && nearbyEnemy.MyNPC.CurrentAggroTarget == null && Vector3.Distance(nearbyEnemy.transform.position, GameData.PlayerControl.transform.position) <= 15f)
			{
				return true;
			}
		}
		return false;
	}

	public Vector3 SampleNavMeshNearTransform(Vector3 origin)
	{
		if (NavMesh.SamplePosition(origin, out var hit, 2.5f, -1))
		{
			return hit.position;
		}
		return origin;
	}

	public void Flee()
	{
		CurrentPullPhase = PullPhases.NotPulling;
		PullTarget = null;
		Myself.MyNPC.CurrentAggroTarget = null;
		GameData.SimMngr.Sims[myIndex].isPuller = false;
		if (!Myself.Alive)
		{
			return;
		}
		if (GameData.EgressLocations.Count > 0)
		{
			float num = float.PositiveInfinity;
			PointOfInterest pointOfInterest = null;
			RunningAway = true;
			foreach (PointOfInterest egressLocation in GameData.EgressLocations)
			{
				if (Vector3.Distance(base.transform.position, egressLocation.transform.position) < num)
				{
					num = Vector3.Distance(base.transform.position, egressLocation.transform.position);
					pointOfInterest = egressLocation;
				}
			}
			if (pointOfInterest != null)
			{
				MyPOI = pointOfInterest;
			}
			else
			{
				MyPOI = GameData.EgressLocations[UnityEngine.Random.Range(0, GameData.EgressLocations.Count)];
			}
			IgnoreAllCombat = true;
			MyNPC.retreat = true;
			UpdateSocialLog.LogAdd(base.transform.name + " tells the group: " + GameData.SimMngr.PersonalizeString("Zoning out!", this), "#00B2B7");
			MyTask = PointOfInterest.POIType.zoneline;
			MyNav.speed = MyStats.actualRunSpeed;
			if (MyNav.isOnNavMesh)
			{
				MyNav.isStopped = false;
			}
			MyNav.SetDestination(GameData.GetSafeNavMeshPoint(MyPOI.transform.position));
			MyAnim.SetBool("Walking", value: true);
		}
		else
		{
			UpdateSocialLog.LogAdd(base.transform.name + " tells the group: " + GameData.SimMngr.PersonalizeString("There's nowhere to go!", this), "#00B2B7");
		}
	}

	private Character FindNearestSpawn()
	{
		float num = float.PositiveInfinity;
		SpawnPoint spawnPoint = null;
		int num2 = 3;
		int num3 = -3;
		int num4 = 1000;
		if (GameData.SimMngr.Sims.Count > myIndex && GameData.SimMngr.Sims[myIndex] != null)
		{
			num2 = GameData.SimPlayerGrouping.PullerRangeHigh;
			num3 = GameData.SimPlayerGrouping.PullerRangeLow;
			num4 = GameData.SimPlayerGrouping.MaxPullDist;
		}
		foreach (SpawnPoint item in SpawnPointManager.SpawnPointsInScene)
		{
			if (item == null || item.SpawnedNPC == null)
			{
				continue;
			}
			Character @char = item.SpawnedNPC.GetChar();
			if (@char == null || !@char.Alive || @char.MyStats.Charmed || item.SpawnedNPC.GroupEncounter || !@char.Alive || @char.MyStats.Charmed || @char.MyStats.CurrentHP <= 0 || @char.MyStats.Level > MyStats.Level + num2 || @char.MyStats.Level <= MyStats.Level + num3 || GameData.SimMngr.AvoidPulls.Contains(@char.MyFaction) || @char.MyStats.RunSpeed <= 0f || Vector3.Distance(base.transform.position, @char.transform.position) > (float)num4)
			{
				continue;
			}
			int areaFromName = NavMesh.GetAreaFromName("Jump");
			int areaMask = -1 & ~(1 << areaFromName);
			NavMeshPath navMeshPath = new NavMeshPath();
			if (NavMesh.CalculatePath(base.transform.position, item.transform.position, areaMask, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
			{
				float pathLength = GetPathLength(navMeshPath);
				if (pathLength < num)
				{
					num = pathLength;
					spawnPoint = item;
				}
			}
		}
		if (!(spawnPoint != null))
		{
			return null;
		}
		return spawnPoint.SpawnedNPC.GetComponent<Character>();
	}

	private float GetPathLength(NavMeshPath navPath)
	{
		if (navPath.corners.Length < 2)
		{
			return 0f;
		}
		float num = 0f;
		for (int i = 0; i < navPath.corners.Length - 1; i++)
		{
			num += Vector3.Distance(navPath.corners[i], navPath.corners[i + 1]);
		}
		return num;
	}

	private void DoGuard()
	{
		if (!(MyNPC.CurrentAggroTarget == null))
		{
			return;
		}
		if (Vector3.Distance(base.transform.position, GuardPos) > 3f)
		{
			MyNav.speed = MyStats.actualRunSpeed;
			if (MyNav.isOnNavMesh)
			{
				MyNav.isStopped = false;
			}
			MyAnim.SetBool("Walking", value: true);
			MyAnim.SetBool("Patrol", value: false);
			MyNav.SetDestination(GameData.GetSafeNavMeshPoint(GuardPos + randomizeOffset));
		}
		else
		{
			TimeOnTask -= 60f * Time.deltaTime;
			MyNav.velocity = Vector3.zero;
			if (MyNav.isOnNavMesh)
			{
				MyNav.isStopped = true;
			}
			MyAnim.SetBool("Walking", value: false);
			MyAnim.SetBool("Patrol", value: false);
		}
	}

	public void SaveSim()
	{
		new List<string>();
		PopulateSave = new SimPlayerSaveData(MyNPC.NPCName, MyStats.Level, new List<Item>(), SkillLevel);
		PopulateSave.DisallowUpgrades = DisallowUpgrades;
		foreach (SimInvSlot item in MyEquipment)
		{
			PopulateSave.MyEquippedItems.Add(item.MyItem.Id);
			PopulateSave.ItemQuantities.Add(item.Quant);
			if (item.ThisSlotType == Item.SlotType.Primary)
			{
				PopulateSave.MHQ = item.Quant;
			}
			if (item.ThisSlotType == Item.SlotType.Secondary)
			{
				PopulateSave.OHQ = item.Quant;
			}
		}
		foreach (ItemSaveData allHeldItem in AllHeldItems)
		{
			PopulateSave.AllInventory.Add(JsonUtility.ToJson(allHeldItem, prettyPrint: true));
		}
		PopulateSave.War = MyStats.CharacterClass == GameData.ClassDB.Paladin;
		PopulateSave.Arc = MyStats.CharacterClass == GameData.ClassDB.Arcanist;
		PopulateSave.Dru = MyStats.CharacterClass == GameData.ClassDB.Druid;
		PopulateSave.Duel = MyStats.CharacterClass == GameData.ClassDB.Duelist;
		PopulateSave.Storm = MyStats.CharacterClass == GameData.ClassDB.Stormcaller;
		PopulateSave.Reav = MyStats.CharacterClass == GameData.ClassDB.Reaver;
		PopulateSave.Greed = Greed;
		if (Myself.MySkills != null)
		{
			foreach (Skill knownSkill in Myself.MySkills.KnownSkills)
			{
				PopulateSave.AcquiredSkills.Add(knownSkill.Id);
			}
			foreach (AscensionSkillEntry myAscension in Myself.MySkills.MyAscensions)
			{
				PopulateSave.MyAscensions.Add(new AscensionSkillEntry(myAscension.id, myAscension.level));
			}
			PopulateSave.AscensionPoints = Myself.MySkills.AscensionPoints;
			PopulateSave.AscensionXP = Myself.MyStats.CurrentAscensionXP;
		}
		if (GameData.SimMngr.Sims[myIndex] != null)
		{
			PopulateSave.HasMetPlayer = GameData.SimMngr.Sims[myIndex].KnowsPlayer;
			PopulateSave.XpForLevelUp = MyStats.CurrentExperience;
			if (GameData.SimMngr.Sims[myIndex].OpinionOfPlayer > 500f)
			{
				GameData.SimMngr.Sims[myIndex].OpinionOfPlayer = 500f;
			}
			if (GameData.SimMngr.Sims[myIndex].OpinionOfPlayer < 0f)
			{
				GameData.SimMngr.Sims[myIndex].OpinionOfPlayer = 0f;
			}
			PopulateSave.OpinionOfPlayer = GameData.SimMngr.Sims[myIndex].OpinionOfPlayer;
			PopulateSave.TiedToSlot = GameData.SimMngr.Sims[myIndex].TiedToSlot;
			PopulateSave.Year = DateTime.Now.Year;
			PopulateSave.Day = DateTime.Now.DayOfYear;
			PopulateSave.Hour = DateTime.Now.Hour;
			PopulateSave.Min = DateTime.Now.Minute;
			PopulateSave.FriendedBy = GameData.SimMngr.Sims[myIndex].FriendedBy;
			PopulateSave.PlanarStones = GameData.SimMngr.Sims[myIndex].Planars;
			PopulateSave.Sivakruxes = GameData.SimMngr.Sims[myIndex].Sivaks;
			PopulateSave.GuildID = GameData.SimMngr.Sims[myIndex].GuildID;
			PopulateSave.IsGuildLeader = GameData.SimMngr.Sims[myIndex].IsGuildLeader;
			PopulateSave.HideHelm = GameData.SimMngr.Sims[myIndex].HideHelm;
			PopulateSave.StrPointsSpent = MyStats.StrScaleSpent;
			PopulateSave.EndPointsSpent = MyStats.EndScaleSpent;
			PopulateSave.DexPointsSpent = MyStats.DexScaleSpent;
			PopulateSave.AgiPointsSpent = MyStats.AgiScaleSpent;
			PopulateSave.IntPointsSpent = MyStats.IntScaleSpent;
			PopulateSave.WisPointsSpent = MyStats.WisScaleSpent;
			PopulateSave.ChaPointsSpent = MyStats.ChaScaleSpent;
			PopulateSave.HairColorIndex = HairColor;
			PopulateSave.SkinColorIndex = SkinColor;
			if (!string.IsNullOrEmpty(HairName))
			{
				PopulateSave.hairName = HairName;
			}
			else
			{
				PopulateSave.hairName = "Chr_Hair_01";
			}
			if (GameData.SimMngr.Sims[myIndex].Gender == "Male")
			{
				PopulateSave.Male = 1;
			}
			else
			{
				PopulateSave.Male = 0;
			}
			if (!string.IsNullOrEmpty(GameData.SimMngr.Sims[myIndex].MyCurrentMemory.NameOfPlayerCharacter))
			{
				PopulateSave.MyLastAdventure = new SimPlayerMemory(GameData.SimMngr.Sims[myIndex].MyCurrentMemory.ZoneName, GameData.SimMngr.Sims[myIndex].MyCurrentMemory.Died, GameData.SimMngr.Sims[myIndex].MyCurrentMemory.Wipes, GameData.SimMngr.Sims[myIndex].MyCurrentMemory.XPGain, GameData.SimMngr.Sims[myIndex].MyCurrentMemory.LevelGain, GameData.SimMngr.Sims[myIndex].MyCurrentMemory.LootPieces, GameData.SimMngr.Sims[myIndex].MyCurrentMemory.ItemGained, GameData.SimMngr.Sims[myIndex].MyCurrentMemory.PlayedDay, GameData.SimMngr.Sims[myIndex].MyCurrentMemory.PlayedYear, GameData.SimMngr.Sims[myIndex].MyCurrentMemory.GroupedLastDay, GameData.SimMngr.Sims[myIndex].MyCurrentMemory.GroupedLastYear, GameData.SimMngr.Sims[myIndex].MyCurrentMemory.NameOfPlayerCharacter);
			}
			else if (GameData.SimMngr.Sims[myIndex].MyPreviousMemory != null && !string.IsNullOrEmpty(GameData.SimMngr.Sims[myIndex].MyPreviousMemory.NameOfPlayerCharacter))
			{
				PopulateSave.MyLastAdventure = new SimPlayerMemory(GameData.SimMngr.Sims[myIndex].MyPreviousMemory.ZoneName, GameData.SimMngr.Sims[myIndex].MyPreviousMemory.Died, GameData.SimMngr.Sims[myIndex].MyPreviousMemory.Wipes, GameData.SimMngr.Sims[myIndex].MyPreviousMemory.XPGain, GameData.SimMngr.Sims[myIndex].MyPreviousMemory.LevelGain, GameData.SimMngr.Sims[myIndex].MyPreviousMemory.LootPieces, GameData.SimMngr.Sims[myIndex].MyPreviousMemory.ItemGained, GameData.SimMngr.Sims[myIndex].MyPreviousMemory.PlayedDay, GameData.SimMngr.Sims[myIndex].MyPreviousMemory.PlayedYear, GameData.SimMngr.Sims[myIndex].MyPreviousMemory.GroupedLastDay, GameData.SimMngr.Sims[myIndex].MyPreviousMemory.GroupedLastYear, GameData.SimMngr.Sims[myIndex].MyPreviousMemory.NameOfPlayerCharacter);
			}
			SimPlayerDataManager.SaveSimData(PopulateSave);
			GameData.SimMngr.Sims[myIndex].Level = MyStats.Level;
			if (GameData.SimMngr.Sims[myIndex].MyAHData == null || StoredItems.Count <= 0)
			{
				return;
			}
			List<string> list = new List<string>();
			foreach (Item storedItem in StoredItems)
			{
				if (storedItem.ItemValue > 0)
				{
					list.Add(storedItem.Id);
				}
			}
			if (list.Count > 0)
			{
				AuctionHouse.UpdateAH(list, MyNPC.NPCName, Greed);
			}
			StoredItems.Clear();
		}
		else
		{
			UpdateSocialLog.LogAdd("[SIMPLAYER] -> Error saving SimPlayer Data for " + MyNPC.NPCName, "yellow");
		}
	}

	private void UpdateSimMngr()
	{
		GameData.SimMngr.Sims[myIndex].Level = MyStats.Level;
	}

	private void LoadSimData()
	{
		SimInvSlot simInvSlot = null;
		SimInvSlot simInvSlot2 = null;
		foreach (SimInvSlot item in MyEquipment)
		{
			if (item.ThisSlotType == Item.SlotType.Bracer && simInvSlot == null)
			{
				simInvSlot = item;
			}
			if (item.ThisSlotType == Item.SlotType.Bracer)
			{
			}
			if (item.ThisSlotType == Item.SlotType.Ring && simInvSlot2 == null)
			{
				simInvSlot2 = item;
			}
			if (item.ThisSlotType != Item.SlotType.Ring)
			{
			}
		}
		SimPlayerSaveData myData = SimPlayerDataManager.GetMyData(MyNPC.NPCName);
		if (myData == null)
		{
			return;
		}
		Greed = myData.Greed;
		if (Greed == 1f || Greed == 0f)
		{
			Greed = UnityEngine.Random.Range(1.85f, 2.2f);
			if (UnityEngine.Random.Range(0, 10) > 5)
			{
				Greed = UnityEngine.Random.Range(2.2f, 2.9f);
			}
		}
		if (GameData.SimMngr.Sims[myIndex].Greed != Greed)
		{
			GameData.SimMngr.Sims[myIndex].Greed = Greed;
		}
		DisallowUpgrades = myData.DisallowUpgrades;
		BioIndex = myData.BioIndex;
		PersonalityType = myData.PersonalityType;
		HairColor = myData.HairColorIndex;
		SkinColor = myData.SkinColorIndex;
		HairName = myData.hairName;
		if (PersonalityType == 0 && UnityEngine.Random.Range(0, 10) > 3)
		{
			PersonalityType = UnityEngine.Random.Range(1, 5);
			if (PersonalityType == 1)
			{
				BioIndex = UnityEngine.Random.Range(0, GameData.SimMngr.NiceDesciptions.Count);
			}
			if (PersonalityType == 2)
			{
				BioIndex = UnityEngine.Random.Range(0, GameData.SimMngr.TryhardDescriptions.Count);
			}
			if (PersonalityType == 3)
			{
				BioIndex = UnityEngine.Random.Range(0, GameData.SimMngr.MeanDescriptions.Count);
			}
			if (UnityEngine.Random.Range(0, 10) > 7)
			{
				PersonalityType = 1;
			}
		}
		else if (PersonalityType == 0)
		{
			PersonalityType = 5;
		}
		if (myData.AcquiredSkills != null && myData.AcquiredSkills.Count > 0)
		{
			foreach (string acquiredSkill in myData.AcquiredSkills)
			{
				if (!Myself.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByID(acquiredSkill)))
				{
					Myself.MySkills.KnownSkills.Add(GameData.SkillDatabase.GetSkillByID(acquiredSkill));
				}
			}
		}
		if (myData.MyAscensions != null && myData.MyAscensions.Count > 0)
		{
			Myself.MySkills.MyAscensions = new List<AscensionSkillEntry>();
			foreach (AscensionSkillEntry myAscension in myData.MyAscensions)
			{
				Myself.MySkills.MyAscensions.Add(new AscensionSkillEntry(myAscension.id, myAscension.level));
			}
		}
		if (myData.War)
		{
			MyStats.CharacterClass = GameData.ClassDB.Paladin;
		}
		else if (myData.Arc)
		{
			MyStats.CharacterClass = GameData.ClassDB.Arcanist;
		}
		else if (myData.Dru)
		{
			MyStats.CharacterClass = GameData.ClassDB.Druid;
		}
		else if (myData.Duel)
		{
			MyStats.CharacterClass = GameData.ClassDB.Duelist;
		}
		else if (myData.Storm)
		{
			MyStats.CharacterClass = GameData.ClassDB.Stormcaller;
		}
		else if (myData.Reav)
		{
			MyStats.CharacterClass = GameData.ClassDB.Reaver;
		}
		Myself.MySkills.AscensionPoints = myData.AscensionPoints;
		Myself.MyStats.CurrentAscensionXP = myData.AscensionXP;
		MyNPC.NPCName = myData.NPCName;
		foreach (SimPlayerTracking sim in GameData.SimMngr.Sims)
		{
			if (sim.SimName == MyNPC.NPCName)
			{
				myIndex = sim.simIndex;
			}
		}
		MyStats.Level = myData.MyLevel;
		MyStats.SetXpForLevelUp();
		MyStats.CurrentExperience = myData.XpForLevelUp;
		MetPlayer = myData.HasMetPlayer;
		OpinionOfPlayer = myData.OpinionOfPlayer;
		_ = myData.ItemQuantities.Count;
		_ = myData.MyEquippedItems.Count;
		MyInv.EquippedItems.Clear();
		if (myData.AllInventory != null && myData.AllInventory.Count >= 0)
		{
			LoadInventory(myData.AllInventory, myData);
		}
		MyStats.StrScaleSpent = myData.StrPointsSpent;
		MyStats.EndScaleSpent = myData.EndPointsSpent;
		MyStats.DexScaleSpent = myData.DexPointsSpent;
		MyStats.AgiScaleSpent = myData.AgiPointsSpent;
		MyStats.IntScaleSpent = myData.IntPointsSpent;
		MyStats.WisScaleSpent = myData.WisPointsSpent;
		MyStats.ChaScaleSpent = myData.ChaPointsSpent;
		SkillLevel = myData.MySkill;
	}

	public SimInvSlot GetOHWeapon()
	{
		foreach (SimInvSlot item in MyEquipment)
		{
			if (item.ThisSlotType == Item.SlotType.Secondary)
			{
				return item;
			}
		}
		return null;
	}

	public void AuditInventory()
	{
		if (MyInv == null)
		{
			MyInv = GetComponent<Inventory>();
		}
		MyInv.WornEffects.Clear();
		SimInvSlot simInvSlot = new SimInvSlot(Item.SlotType.Primary);
		SimInvSlot simInvSlot2 = new SimInvSlot(Item.SlotType.Secondary);
		MyInv.EquippedItems.Clear();
		foreach (SimInvSlot item in MyEquipment)
		{
			MyInv.EquippedItems.Add(item.MyItem);
		}
		foreach (SimInvSlot item2 in MyEquipment)
		{
			if (item2.ThisSlotType == Item.SlotType.Primary)
			{
				simInvSlot = item2;
				if (simInvSlot != null && simInvSlot.MyItem != GameData.PlayerInv.Empty)
				{
					if (simInvSlot.MyItem.WeaponDly != 0f)
					{
						MyStats.BaseMHAtkDelay = simInvSlot.MyItem.WeaponDly * 60f;
					}
					else
					{
						MyStats.BaseMHAtkDelay = 999f;
					}
				}
				else if (simInvSlot != null && simInvSlot.MyItem == GameData.PlayerInv.Empty && MyStats.CharacterClass == GameData.ClassDB.Stormcaller)
				{
					simInvSlot.MyItem = GameData.SimMngr.EnsureBow;
				}
				if (simInvSlot != null && simInvSlot.MyItem != null && simInvSlot.MyItem.ThisWeaponType == Item.WeaponType.TwoHandBow)
				{
					MyInv.PrimaryBow = true;
				}
				else
				{
					MyInv.PrimaryBow = false;
				}
			}
			if (item2.ThisSlotType == Item.SlotType.Secondary)
			{
				simInvSlot2 = item2;
				MyInv.SimOH = null;
				if (simInvSlot2 != null && simInvSlot2.MyItem != GameData.PlayerInv.Empty)
				{
					if (simInvSlot.MyItem.ThisWeaponType != Item.WeaponType.TwoHandMelee && simInvSlot.MyItem.ThisWeaponType != Item.WeaponType.TwoHandStaff && simInvSlot.MyItem.ThisWeaponType != Item.WeaponType.TwoHandBow)
					{
						if (simInvSlot2.MyItem.WeaponDly != 0f)
						{
							MyStats.BaseOHAtkDelay = simInvSlot2.MyItem.WeaponDly * 60f;
						}
						else
						{
							MyStats.BaseOHAtkDelay = 999f;
						}
						if (simInvSlot2.MyItem.Shield)
						{
							MyInv.SecondaryShield = true;
							MyInv.SimOH = simInvSlot2;
						}
						else
						{
							MyInv.SecondaryShield = false;
							MyInv.SimOH = null;
						}
					}
					else
					{
						AllHeldItems.Add(new ItemSaveData(item2.MyItem.Id, item2.Quant));
						item2.MyItem = GameData.PlayerInv.Empty;
						simInvSlot2 = item2;
						MyInv.SimOH = null;
					}
				}
				else
				{
					simInvSlot2.MyItem = GameData.PlayerInv.Empty;
					simInvSlot2.Quant = 1;
				}
			}
			if (item2.MyItem.WornEffect != null)
			{
				MyInv.WornEffects.Add(item2.MyItem.WornEffect);
			}
		}
		Mods.UpdateSimPlayerVisuals(MyEquipment, simInvSlot, simInvSlot2);
		MyInv.UpdateInvStats();
		MyStats.CalcSimStats();
		MyStats.CalcStats();
		LimitHeldQuantities();
	}

	private bool CheckRelic(Item _item)
	{
		if (!_item.Relic)
		{
			return false;
		}
		foreach (SimInvSlot item in MyEquipment)
		{
			if (item.MyItem == _item)
			{
				return true;
			}
		}
		return false;
	}

	private bool IsSecondaryEmptyWhenGivingToPrimary(Item.SlotType _slotType)
	{
		bool flag = false;
		foreach (SimInvSlot item in MyEquipment)
		{
			if (item.ThisSlotType == _slotType)
			{
				if (item.MyItem == GameData.PlayerInv.Empty && !flag)
				{
					return false;
				}
				if (!flag)
				{
					flag = true;
				}
				else if (item.MyItem == GameData.PlayerInv.Empty)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool ReceiveNewEquippable(Item _item, bool _fromPlayer, string _forceSlot, int _qual)
	{
		Item item = null;
		returnItemQual = 1;
		Item item2 = null;
		returnSecondItemQual = 1;
		bool result = false;
		bool flag = false;
		bool flag2 = false;
		SimPlayerTracking simPlayerTracking = GameData.SimMngr.Sims[myIndex];
		if (simPlayerTracking != null && !string.IsNullOrEmpty(simPlayerTracking.GuildID))
		{
			LiveGuildData guildDataByID = GameData.GuildManager.GetGuildDataByID(simPlayerTracking.GuildID);
			if (guildDataByID != null && guildDataByID.OngoingGuildQuests.Count > 0)
			{
				foreach (GuildQuest ongoingGuildQuest in guildDataByID.OngoingGuildQuests)
				{
					if (ongoingGuildQuest.QuestGiver == simPlayerTracking && ongoingGuildQuest.QuestObjective == _item)
					{
						flag2 = true;
						break;
					}
				}
			}
		}
		if (_item.Classes.Contains(MyStats.CharacterClass) && _item != GameData.PlayerInv.Empty && !CheckRelic(_item) && !HandConflict(_item, _forceSlot) && !PreventDowngrade(_item, _qual) && _item.ItemValue > 0)
		{
			if (_item.RequiredSlot == Item.SlotType.Bracer)
			{
				int num = 0;
				num = ((!(_forceSlot == "0")) ? 1 : (IsSecondaryEmptyWhenGivingToPrimary(_item.RequiredSlot) ? 1 : 0));
				foreach (SimInvSlot item3 in MyEquipment)
				{
					if (item3.ThisSlotType == Item.SlotType.Bracer && num == 0)
					{
						item = item3.MyItem;
						returnItemQual = item3.Quant;
						item3.MyItem = _item;
						item3.Quant = _qual;
						result = true;
						break;
					}
					if (item3.ThisSlotType == Item.SlotType.Bracer)
					{
						num--;
					}
				}
			}
			else if (_item.RequiredSlot == Item.SlotType.Ring)
			{
				int num2 = 0;
				num2 = ((!(_forceSlot == "0")) ? 1 : (IsSecondaryEmptyWhenGivingToPrimary(_item.RequiredSlot) ? 1 : 0));
				foreach (SimInvSlot item4 in MyEquipment)
				{
					if (item4.ThisSlotType == Item.SlotType.Ring && num2 == 0)
					{
						item = item4.MyItem;
						returnItemQual = item4.Quant;
						item4.MyItem = _item;
						item4.Quant = _qual;
						result = true;
						break;
					}
					if (item4.ThisSlotType == Item.SlotType.Ring)
					{
						num2--;
					}
				}
			}
			else if (_item.RequiredSlot == Item.SlotType.PrimaryOrSecondary)
			{
				int num3 = 0;
				num3 = ((!(_forceSlot == "0")) ? 1 : 0);
				foreach (SimInvSlot item5 in MyEquipment)
				{
					if (item5.ThisSlotType == Item.SlotType.Primary && num3 == 0)
					{
						item = item5.MyItem;
						returnItemQual = item5.Quant;
						item5.MyItem = _item;
						item5.Quant = _qual;
						result = true;
						break;
					}
					if (item5.ThisSlotType == Item.SlotType.Secondary && num3 == 1 && !flag)
					{
						item = item5.MyItem;
						returnItemQual = item5.Quant;
						item5.MyItem = _item;
						item5.Quant = _qual;
						result = true;
						break;
					}
				}
				if (flag && !flag2)
				{
					UpdateSocialLog.LogAdd(MyNPC.NPCName + " says: " + GameData.SimMngr.PersonalizeString("I can't use that with my current setup.", this));
				}
			}
			else
			{
				foreach (SimInvSlot item6 in MyEquipment)
				{
					if (item6.ThisSlotType == _item.RequiredSlot && !flag)
					{
						if (!CheckForSameItemUpgrade(_item, _qual))
						{
							item = item6.MyItem;
							returnItemQual = item6.Quant;
						}
						else
						{
							item = GameData.PlayerInv.Empty;
							returnItemQual = 0;
						}
						item6.MyItem = _item;
						item6.Quant = _qual;
						result = true;
					}
				}
			}
			if (flag && !flag2)
			{
				UpdateSocialLog.LogAdd(MyNPC.NPCName + " says: " + GameData.SimMngr.PersonalizeString("I can't use that with my current setup.", this));
			}
			item2 = CheckSlotCompat();
			if (_fromPlayer && item != null && _item.ItemLevel > item.ItemLevel)
			{
				if (item != null)
				{
					if (item != GameData.PlayerInv.Empty)
					{
						UpdateSocialLog.LogAdd(MyNPC.NPCName + " says: " + GameData.SimMngr.PersonalizeString("Thank you! I'll put my old item away in case I need it later!", this));
						AllHeldItems.Add(new ItemSaveData(item.Id, returnItemQual));
						if (GameData.SimMngr.Sims[myIndex] != null)
						{
							GameData.SimMngr.Sims[myIndex].OpinionOfPlayer += 0.5f;
						}
					}
					else
					{
						UpdateSocialLog.LogAdd(MyNPC.NPCName + " says: " + GameData.SimMngr.PersonalizeString("Thanks! I really needed this.", this));
						if (GameData.SimMngr.Sims[myIndex] != null)
						{
							GameData.SimMngr.Sims[myIndex].OpinionOfPlayer += 0.5f;
						}
					}
				}
				if (item2 != null && item2 != GameData.PlayerInv.Empty)
				{
					AllHeldItems.Add(new ItemSaveData(item2.Id, returnSecondItemQual));
				}
			}
			else if (!flag)
			{
				UpdateSocialLog.LogAdd(MyNPC.NPCName + " says: " + GameData.SimMngr.PersonalizeString("Ok, I'll use this for now.", this));
				AllHeldItems.Add(new ItemSaveData(item.Id, returnItemQual));
				if (item2 != null && item2 != GameData.PlayerInv.Empty)
				{
					AllHeldItems.Add(new ItemSaveData(item2.Id, returnSecondItemQual));
				}
				if (StoredItems.Count < 17)
				{
					if (item != null)
					{
						StoredItems.Add(item);
					}
					if (item2 != null)
					{
						StoredItems.Add(item2);
					}
				}
				else
				{
					if (item != null)
					{
						StoredItems[UnityEngine.Random.Range(0, 17)] = item;
					}
					if (item2 != null)
					{
						StoredItems[UnityEngine.Random.Range(0, 17)] = item2;
					}
				}
			}
		}
		else if (_fromPlayer && _item != GameData.PlayerInv.Empty && _item != WTB && !flag2)
		{
			UpdateSocialLog.LogAdd(MyNPC.NPCName + " says: " + GameData.SimMngr.PersonalizeString("Ah, I can't use this right now but thanks anyhow", this));
			UpdateSocialLog.LogAdd(MyNPC.NPCName + " has returned your " + _item.ItemName, "yellow");
			if (_item.RequiredSlot == Item.SlotType.General)
			{
				for (int i = 1; i <= _qual; i++)
				{
					if (!GameData.PlayerInv.AddItemToInv(_item))
					{
						GameData.PlayerInv.ForceItemToInv(_item);
					}
				}
			}
			else if (!GameData.PlayerInv.AddItemToInv(_item, _qual))
			{
				GameData.PlayerInv.ForceItemToInv(_item, _qual);
			}
			UpdateSocialLog.LogAdd(MyNPC.NPCName + " says: " + GameData.SimMngr.PersonalizeString("Oops, here's your " + _item.ItemName + " back.", this));
		}
		AuditInventory();
		if (GameData.InspectSim.InspectWindow.activeSelf)
		{
			GameData.InspectSim.CloseWindow();
			GameData.InspectSim.InspectSim(this);
		}
		_ = AllHeldItems.Count;
		_ = 0;
		LimitHeldQuantities();
		SaveSim();
		if (flag2)
		{
			result = true;
			GameData.GuildManager.CompleteGuildQuest(GameData.SimMngr.Sims[myIndex], _item);
		}
		return result;
	}

	private bool HandConflict(Item _item, string _force)
	{
		foreach (SimInvSlot item in MyEquipment)
		{
			_ = item;
			if (_item.RequiredSlot != Item.SlotType.Secondary && (_item.RequiredSlot != Item.SlotType.PrimaryOrSecondary || !(_force == "1")))
			{
				continue;
			}
			foreach (SimInvSlot item2 in MyEquipment)
			{
				if (item2.ThisSlotType == Item.SlotType.Primary && (item2.MyItem.ThisWeaponType == Item.WeaponType.TwoHandMelee || item2.MyItem.ThisWeaponType == Item.WeaponType.TwoHandStaff))
				{
					UpdateSocialLog.LogAdd(MyNPC.NPCName + " says: " + GameData.SimMngr.PersonalizeString("My primary weapon is two handed, so I can't equip this right now.", this));
					return true;
				}
			}
		}
		return false;
	}

	private Item CheckSlotCompat()
	{
		SimInvSlot simInvSlot = null;
		SimInvSlot simInvSlot2 = null;
		foreach (SimInvSlot item in MyEquipment)
		{
			if (item.ThisSlotType == Item.SlotType.Primary)
			{
				simInvSlot = item;
			}
			if (item.ThisSlotType == Item.SlotType.Secondary)
			{
				simInvSlot2 = item;
			}
		}
		if ((simInvSlot.MyItem.ThisWeaponType == Item.WeaponType.TwoHandMelee || simInvSlot.MyItem.ThisWeaponType == Item.WeaponType.TwoHandStaff) && simInvSlot2.MyItem != GameData.PlayerInv.Empty)
		{
			Item myItem = simInvSlot2.MyItem;
			returnSecondItemQual = simInvSlot2.Quant;
			simInvSlot2.MyItem = GameData.PlayerInv.Empty;
			return myItem;
		}
		return GameData.PlayerInv.Empty;
	}

	public void LoadSimSpells()
	{
		Spellbook.Clear();
		MyNPC.MyHealSpells.Clear();
		MyNPC.MyBuffSpells.Clear();
		MyNPC.MyAttackSpells.Clear();
		MyNPC.MyCCSpells.Clear();
		MyNPC.MyTauntSpell.Clear();
		InvisSpell = null;
		Spell[] spellDatabase = GameData.SpellDatabase.SpellDatabase;
		foreach (Spell spell2 in spellDatabase)
		{
			if (!spell2.UsedBy.Contains(MyStats.CharacterClass) || !spell2.SimUsable)
			{
				continue;
			}
			switch (spell2.Type)
			{
			case Spell.SpellType.Heal:
				if (MyNPC.MyHealSpells.Contains(spell2) || MyStats.Level < spell2.RequiredLevel)
				{
					break;
				}
				if (spell2.StatusEffectToApply == null)
				{
					MyNPC.MyHealSpells.Add(spell2);
					Spellbook.Add(new SimPlayerSpellSlot(spell2));
				}
				if (spell2.RequiredLevel <= MyStats.Level && spell2.StatusEffectToApply != null && spell2.StatusEffectToApply.TargetHealing > 0 && spell2.StatusEffectToApply.SpellDurationInTicks > 0)
				{
					if (MyNPC.MyHOTSpell == null)
					{
						MyNPC.MyHOTSpell = spell2;
					}
					else if (spell2.RequiredLevel > MyNPC.MyHOTSpell.RequiredLevel)
					{
						MyNPC.MyHOTSpell = spell2;
					}
				}
				break;
			case Spell.SpellType.Beneficial:
				if (!MyNPC.MyBuffSpells.Contains(spell2) && MyStats.Level >= spell2.RequiredLevel)
				{
					MyNPC.MyBuffSpells.Add(spell2);
					Spellbook.Add(new SimPlayerSpellSlot(spell2));
				}
				break;
			case Spell.SpellType.Damage:
				if (!MyNPC.MyAttackSpells.Contains(spell2) && MyStats.Level >= spell2.RequiredLevel)
				{
					MyNPC.MyAttackSpells.Add(spell2);
					Spellbook.Add(new SimPlayerSpellSlot(spell2));
				}
				break;
			case Spell.SpellType.PBAE:
				if (!MyNPC.MyAttackSpells.Contains(spell2) && MyStats.Level >= spell2.RequiredLevel)
				{
					MyNPC.MyAttackSpells.Add(spell2);
					Spellbook.Add(new SimPlayerSpellSlot(spell2));
				}
				break;
			case Spell.SpellType.AE:
				if (!MyNPC.MyAttackSpells.Contains(spell2) && MyStats.Level >= spell2.RequiredLevel)
				{
					MyNPC.MyAttackSpells.Add(spell2);
					Spellbook.Add(new SimPlayerSpellSlot(spell2));
				}
				break;
			case Spell.SpellType.StatusEffect:
				if (!MyNPC.MyAttackSpells.Contains(spell2) && MyStats.Level >= spell2.RequiredLevel)
				{
					MyNPC.MyAttackSpells.Add(spell2);
					Spellbook.Add(new SimPlayerSpellSlot(spell2));
					if (spell2.FearTarget && (FearSpell == null || FearSpell.RequiredLevel < spell2.RequiredLevel))
					{
						FearSpell = spell2;
					}
					if (spell2.MovementSpeed < 0f && (SnareSpell == null || SnareSpell.MovementSpeed > spell2.MovementSpeed))
					{
						SnareSpell = spell2;
					}
				}
				break;
			case Spell.SpellType.Pet:
				if (spell2.RequiredLevel <= MyStats.Level)
				{
					if (MyNPC.MyPetSpell == null)
					{
						MyNPC.MyPetSpell = spell2;
					}
					else if (spell2.RequiredLevel > MyNPC.MyPetSpell.RequiredLevel)
					{
						MyNPC.MyPetSpell = spell2;
					}
				}
				break;
			}
		}
		foreach (ItemSaveData allHeldItem in AllHeldItems)
		{
			Item itemByID = GameData.ItemDB.GetItemByID(allHeldItem.ID);
			Spell itemEffectOnClick = itemByID.ItemEffectOnClick;
			if (!(itemByID != null) || !(itemEffectOnClick != null) || itemEffectOnClick.Type == Spell.SpellType.Heal)
			{
				continue;
			}
			if (itemByID.ItemEffectOnClick.PetToSummon != null)
			{
				if (MyNPC.MyPetSpell == null)
				{
					MyNPC.MyPetSpell = itemEffectOnClick;
				}
				else if (itemEffectOnClick.RequiredLevel > MyNPC.MyPetSpell.RequiredLevel)
				{
					MyNPC.MyPetSpell = itemEffectOnClick;
				}
			}
			if (itemByID.ItemEffectOnClick.Type == Spell.SpellType.Heal)
			{
				MyNPC.MyHealSpells.Add(itemEffectOnClick);
				Spellbook.Add(new SimPlayerSpellSlot(itemEffectOnClick));
			}
			if (itemByID.ItemEffectOnClick.Type == Spell.SpellType.Damage)
			{
				MyNPC.MyAttackSpells.Add(itemEffectOnClick);
				Spellbook.Add(new SimPlayerSpellSlot(itemEffectOnClick));
			}
			if (itemByID.ItemEffectOnClick.Type == Spell.SpellType.StatusEffect)
			{
				MyNPC.MyAttackSpells.Add(itemEffectOnClick);
				Spellbook.Add(new SimPlayerSpellSlot(itemEffectOnClick));
			}
			if (itemByID.ItemEffectOnClick.Type == Spell.SpellType.Beneficial)
			{
				MyNPC.MyBuffSpells.Add(itemEffectOnClick);
				Spellbook.Add(new SimPlayerSpellSlot(itemEffectOnClick));
			}
		}
		foreach (SimInvSlot item in MyEquipment)
		{
			Item myItem = item.MyItem;
			Spell itemEffectOnClick2 = myItem.ItemEffectOnClick;
			if (!(myItem != null) || !(itemEffectOnClick2 != null) || itemEffectOnClick2.Type == Spell.SpellType.Heal)
			{
				continue;
			}
			if (myItem.ItemEffectOnClick.PetToSummon != null)
			{
				if (MyNPC.MyPetSpell == null)
				{
					MyNPC.MyPetSpell = itemEffectOnClick2;
				}
				else if (itemEffectOnClick2.RequiredLevel > MyNPC.MyPetSpell.RequiredLevel)
				{
					MyNPC.MyPetSpell = itemEffectOnClick2;
				}
			}
			if (myItem.ItemEffectOnClick.Type == Spell.SpellType.Heal)
			{
				MyNPC.MyHealSpells.Add(itemEffectOnClick2);
				Spellbook.Add(new SimPlayerSpellSlot(itemEffectOnClick2));
			}
			if (myItem.ItemEffectOnClick.Type == Spell.SpellType.Damage)
			{
				MyNPC.MyAttackSpells.Add(itemEffectOnClick2);
				Spellbook.Add(new SimPlayerSpellSlot(itemEffectOnClick2));
			}
			if (myItem.ItemEffectOnClick.Type == Spell.SpellType.StatusEffect)
			{
				MyNPC.MyAttackSpells.Add(itemEffectOnClick2);
				Spellbook.Add(new SimPlayerSpellSlot(itemEffectOnClick2));
			}
			if (myItem.ItemEffectOnClick.Type == Spell.SpellType.Beneficial)
			{
				MyNPC.MyBuffSpells.Add(itemEffectOnClick2);
				Spellbook.Add(new SimPlayerSpellSlot(itemEffectOnClick2));
			}
		}
		if (MyNPC.MyHealSpells.Count > 1)
		{
			for (int num = MyNPC.MyHealSpells.Count - 1; num >= 0; num--)
			{
				if (MyNPC.MyHealSpells[num].GroupEffect)
				{
					MyNPC.GroupHeals.Add(MyNPC.MyHealSpells[num]);
				}
			}
			MyNPC.MyHealSpells = MyNPC.MyHealSpells.OrderByDescending((Spell x) => x.RequiredLevel).ToList();
			if (MyNPC.GroupHeals.Count > 1)
			{
				MyNPC.GroupHeals = MyNPC.GroupHeals.OrderByDescending((Spell x) => x.RequiredLevel).ToList();
			}
		}
		if (MyStats.CharacterClass == GameData.ClassDB.Druid || MyStats.CharacterClass == GameData.ClassDB.Paladin)
		{
			List<Spell> list = (from spell in MyNPC.MyHealSpells
				where !spell.GroupEffect
				orderby spell.HP descending
				select spell).Take(3).ToList();
			Spell spell3 = (from spell in MyNPC.MyHealSpells
				where spell.GroupEffect
				orderby spell.HP descending
				select spell).FirstOrDefault();
			if (spell3 != null && !list.Contains(spell3))
			{
				if (list.Count < 3)
				{
					list.Add(spell3);
				}
				else
				{
					list[list.Count - 1] = spell3;
				}
			}
			MyNPC.MemmedHealSpells = list.OrderByDescending((Spell spell) => spell.HP).ToList();
		}
		else
		{
			MyNPC.MemmedHealSpells = MyNPC.MyHealSpells;
		}
		if (MyStats.CharacterClass == GameData.ClassDB.Druid)
		{
			foreach (Spell myHealSpell in MyNPC.MyHealSpells)
			{
				if (myHealSpell.PercentManaRestoration > 0)
				{
					MyNPC.MemmedHealSpells.Add(myHealSpell);
				}
			}
		}
		foreach (Spell myAttackSpell in MyNPC.MyAttackSpells)
		{
			if (myAttackSpell.CrowdControlSpell)
			{
				MyNPC.MyCCSpells.Add(myAttackSpell);
			}
			if (myAttackSpell.TauntSpell && myAttackSpell.Type != Spell.SpellType.PBAE)
			{
				MyNPC.MyTauntSpell.Add(myAttackSpell);
			}
			if (myAttackSpell.TauntSpell && myAttackSpell.Type == Spell.SpellType.PBAE)
			{
				MyNPC.AETaunt = myAttackSpell;
			}
		}
		foreach (Spell myBuffSpell in MyNPC.MyBuffSpells)
		{
			if (myBuffSpell.GrantInvisibility)
			{
				InvisSpell = myBuffSpell;
			}
		}
		foreach (Spell myCCSpell in MyNPC.MyCCSpells)
		{
			MyNPC.MyAttackSpells.Remove(myCCSpell);
		}
		foreach (Spell item2 in MyNPC.MyTauntSpell)
		{
			if (MyStats.CharacterClass != GameData.ClassDB.Reaver)
			{
				MyNPC.MyAttackSpells.Remove(item2);
			}
			if (MyNPC.MyTauntSpell != null)
			{
				MyNPC.MyTauntSpell = MyNPC.MyTauntSpell.OrderByDescending((Spell spell) => spell.RequiredLevel).Take(2).ToList();
			}
		}
		if (MyNPC.MyTauntSpell.Count > 0)
		{
			for (int j = 0; j < MyNPC.MyTauntSpell.Count; j++)
			{
				if (MyNPC.MyTauntSpell[j].Type != Spell.SpellType.PBAE)
				{
					PullSpell = MyNPC.MyTauntSpell[j];
					PullRange = (int)PullSpell.SpellRange;
					break;
				}
			}
		}
		else if (MyNPC.MyAttackSpells.Count > 0)
		{
			PullSpell = MyNPC.MyAttackSpells[0];
			PullRange = 15;
		}
		else
		{
			PullRange = 5;
			PullSpell = null;
		}
	}

	private void LoadSimBuffs()
	{
		if (GameData.SimMngr.Sims[myIndex].CurrentSEs.Count <= 0)
		{
			return;
		}
		foreach (string currentSE in GameData.SimMngr.Sims[myIndex].CurrentSEs)
		{
			MyStats.AddStatusEffect(GameData.SpellDatabase.GetSpellByID(currentSE), _fromPlayer: false, 1);
		}
	}

	private void BuyingItem()
	{
		WTB = GameData.ItemDB.GetRandomGeneric();
	}

	private void LoadItemIcons()
	{
		MyEquipment.Add(new SimInvSlot(Item.SlotType.Primary));
		MyEquipment.Add(new SimInvSlot(Item.SlotType.Secondary));
		MyEquipment.Add(new SimInvSlot(Item.SlotType.Foot));
		MyEquipment.Add(new SimInvSlot(Item.SlotType.Arm));
		MyEquipment.Add(new SimInvSlot(Item.SlotType.Back));
		MyEquipment.Add(new SimInvSlot(Item.SlotType.Bracer));
		MyEquipment.Add(new SimInvSlot(Item.SlotType.Bracer));
		MyEquipment.Add(new SimInvSlot(Item.SlotType.Chest));
		MyEquipment.Add(new SimInvSlot(Item.SlotType.Hand));
		MyEquipment.Add(new SimInvSlot(Item.SlotType.Ring));
		MyEquipment.Add(new SimInvSlot(Item.SlotType.Ring));
		MyEquipment.Add(new SimInvSlot(Item.SlotType.Leg));
		MyEquipment.Add(new SimInvSlot(Item.SlotType.Head));
		MyEquipment.Add(new SimInvSlot(Item.SlotType.Neck));
		MyEquipment.Add(new SimInvSlot(Item.SlotType.Waist));
		AuraSlot = new SimInvSlot(Item.SlotType.Aura);
		CharmSlot = new SimInvSlot(Item.SlotType.Charm);
		MyEquipment.Add(AuraSlot);
		MyEquipment.Add(CharmSlot);
	}

	public int GetSpellIndexInBook(Spell _spell)
	{
		int num = 0;
		foreach (SimPlayerSpellSlot item in Spellbook)
		{
			if (item.spell.SpellName == _spell.SpellName)
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	public float GetSpellCooldownByIndex(int _index)
	{
		return Spellbook[_index].CD;
	}

	public void SetSpellCooldownByIndex(int _index, float _CD)
	{
		Spellbook[_index].CD = _CD * 60f;
	}

	public void SetSpellCooldownBySpell(Spell _spell, float _CD)
	{
		foreach (SimPlayerSpellSlot item in Spellbook)
		{
			if (item.spell.SpellName == _spell.SpellName)
			{
				item.CD = _CD * 60f;
			}
		}
	}

	private void CalcTimeSinceLastSeen(SimPlayerSaveData _load)
	{
		int num = 0;
		if (_load.Year >= 1 && _load.Year <= 9999 && _load.Day >= 1 && _load.Day <= 366 && _load.Hour >= 0 && _load.Hour < 24 && _load.Min >= 0 && _load.Min < 60)
		{
			try
			{
				DateTime dateTime = new DateTime(_load.Year, 1, 1).AddDays(_load.Day - 1).AddHours(_load.Hour).AddMinutes(_load.Min);
				num = Mathf.FloorToInt((float)(DateTime.Now - dateTime).TotalHours);
				if (num > 1000 && TiedToSlot != 99)
				{
					num = UnityEngine.Random.Range(0, 15);
				}
			}
			catch (Exception)
			{
				num = 0;
			}
		}
		if (_load.newlyGenerated)
		{
			num = 7200;
			_load.newlyGenerated = false;
		}
		if (_load.TiedToSlot == 99)
		{
			num = ((num >= 10) ? (num * 2) : 10);
		}
		CheckGearProgress(num);
	}

	private float GetClassModItemLvl(Item _item)
	{
		if (_item.RequiredSlot == Item.SlotType.Aura)
		{
			return _item.ItemLevel;
		}
		float result = 1f;
		if (MyStats.CharacterClass == GameData.ClassDB.Paladin && _item.WeaponDmg == 0)
		{
			result = _item.ItemLevel + _item.Str + _item.End + Mathf.RoundToInt(_item.HP / 2) + _item.AC;
		}
		if (MyStats.CharacterClass == GameData.ClassDB.Duelist && _item.WeaponDmg == 0)
		{
			result = _item.ItemLevel + _item.Dex + _item.Str + Mathf.RoundToInt(_item.HP / 2);
		}
		if (MyStats.CharacterClass == GameData.ClassDB.Arcanist)
		{
			result = _item.ItemLevel + _item.Int + _item.Wis + Mathf.RoundToInt(_item.Mana / 2) + _item.Cha;
		}
		if (MyStats.CharacterClass == GameData.ClassDB.Druid)
		{
			result = _item.ItemLevel + _item.Int + _item.Wis + Mathf.RoundToInt(_item.Mana / 2) + Mathf.RoundToInt(_item.HP / 2);
		}
		if (MyStats.CharacterClass == GameData.ClassDB.Paladin && _item.WeaponDmg != 0)
		{
			result = _item.ItemLevel + _item.Str + _item.End + Mathf.RoundToInt(_item.HP / 2) + _item.AC;
		}
		if (MyStats.CharacterClass == GameData.ClassDB.Duelist && _item.WeaponDmg != 0)
		{
			result = (float)(10 * _item.ItemLevel) * ((float)_item.WeaponDmg / _item.WeaponDly);
		}
		return result;
	}

	public void Force2H()
	{
		foreach (SimInvSlot item in MyEquipment)
		{
			if (item.ThisSlotType == Item.SlotType.Primary)
			{
				item.MyItem = GameData.ItemDB.GetItemByID("72779188");
			}
		}
		AuditInventory();
		LimitHeldQuantities();
		SaveSim();
	}

	public void CheckGearProgress(int _hours)
	{
		if (IsGMCharacter)
		{
			return;
		}
		if (MyStats != null && MyStats.Level > 12 && _hours != 7200)
		{
			for (int i = 0; i <= _hours; i++)
			{
				if (UnityEngine.Random.Range(0, 150) == 0)
				{
					GameData.SimMngr.Sims[myIndex].Sivaks++;
				}
				if (UnityEngine.Random.Range(0, 700) == 0)
				{
					GameData.SimMngr.Sims[myIndex].Planars++;
				}
			}
		}
		if (DisallowUpgrades)
		{
			AuditInventory();
			return;
		}
		bool flag = false;
		if (_hours == 2000)
		{
			flag = true;
		}
		if (GameData.GM.DemoBuild)
		{
			return;
		}
		int num = 0;
		int num2 = 8;
		int num3 = 200;
		SimInvSlot simInvSlot = null;
		SimInvSlot simInvSlot2 = null;
		bool flag2 = false;
		bool flag3 = false;
		int num4 = 1;
		if (MyStats != null && MyStats.Level > 10)
		{
			num4 = 2;
		}
		if (_hours == 7200)
		{
			num2 = 10;
		}
		if (_hours > 12 && Myself.MySkills.GetPointsSpent() < GameData.PlayerControl.Myself.MySkills.GetPointsSpent() - 1 && TiedToSlot == GameData.CurrentCharacterSlot.index && MyStats.Level >= 35)
		{
			Myself.MySkills.AscensionPoints++;
		}
		if (_hours > 6)
		{
			for (int j = 0; j <= 10; j++)
			{
				int count = StoredItems.Count;
				if (count == 0)
				{
					break;
				}
				if (UnityEngine.Random.Range(0, 10) < _hours)
				{
					StoredItems.RemoveAt(UnityEngine.Random.Range(0, count));
				}
			}
		}
		SimInvSlot simInvSlot3 = null;
		SimInvSlot simInvSlot4 = null;
		foreach (SimInvSlot item3 in MyEquipment)
		{
			if (item3.ThisSlotType == Item.SlotType.Primary)
			{
				simInvSlot3 = item3;
			}
			if (item3.ThisSlotType == Item.SlotType.Secondary)
			{
				simInvSlot4 = item3;
			}
			if (item3.ThisSlotType == Item.SlotType.Bracer && simInvSlot == null)
			{
				simInvSlot = item3;
			}
			if (item3.ThisSlotType == Item.SlotType.Bracer)
			{
			}
			if (item3.ThisSlotType == Item.SlotType.Ring && simInvSlot2 == null)
			{
				simInvSlot2 = item3;
			}
			if (item3.ThisSlotType != Item.SlotType.Ring)
			{
			}
		}
		for (int k = 0; k < _hours; k++)
		{
			if (UnityEngine.Random.Range(0, 20) < GearChase)
			{
				num++;
			}
		}
		if (num > 10)
		{
			num = 10;
		}
		if (_hours >= 2000)
		{
			num = 50;
		}
		for (int l = 0; l < num; l++)
		{
			num3 = 200;
			Item item2 = null;
			int num5 = -1;
			if (UnityEngine.Random.Range(0, 100) > 97)
			{
				num5 = -3;
			}
			if (flag)
			{
				num5 = -2;
				num3 = 25000;
				num2 = 20;
			}
			do
			{
				num3--;
				item2 = GameData.ItemDB.ItemDB[UnityEngine.Random.Range(0, GameData.ItemDB.ItemDB.Length)];
			}
			while ((item2.SimPlayersCantGet || (item2.RareItem && UnityEngine.Random.Range(0, 100) < 98) || item2.ItemLevel <= 0 || item2.ItemLevel > MyStats.Level + num4 || item2.ItemLevel > 39 || item2.ItemLevel < Mathf.Max(1, MyStats.Level + num5)) && num3 > 0);
			if (!(item2 != null) || item2.SimPlayersCantGet || item2.ItemLevel <= 0 || item2.ItemLevel > MyStats.Level + num4 || item2.ItemLevel < Mathf.Max(1, MyStats.Level + num5))
			{
				item2 = GameData.SimMngr.LowLevelFallbacks[UnityEngine.Random.Range(0, GameData.SimMngr.LowLevelFallbacks.Count)];
			}
			bool flag4 = false;
			if (item2 != null && item2.WornEffect != null && item2.WornEffect.Id == "45807425" && MyStats.CharacterClass == GameData.ClassDB.Duelist)
			{
				flag4 = true;
			}
			if (item2 != null && item2.WornEffect != null && (item2.WornEffect.Id == "18068040" || item2.WornEffect.Id == "16613100") && MyStats.CharacterClass == GameData.ClassDB.Paladin)
			{
				flag4 = true;
			}
			if (item2.Classes.Contains(MyStats.CharacterClass) && !flag4)
			{
				float num6 = 999f;
				SimInvSlot simInvSlot5 = null;
				foreach (SimInvSlot item4 in MyEquipment)
				{
					if (item4.ThisSlotType != item2.RequiredSlot && ((item4.ThisSlotType != Item.SlotType.Primary && item4.ThisSlotType != Item.SlotType.Secondary) || item2.RequiredSlot != Item.SlotType.PrimaryOrSecondary))
					{
						continue;
					}
					if (item4.ThisSlotType != Item.SlotType.Primary && item4.ThisSlotType != Item.SlotType.Secondary)
					{
						if (GetClassModItemLvl(item4.MyItem) < GetClassModItemLvl(item2) && GetClassModItemLvl(item4.MyItem) < num6 && !CheckRelic(item2))
						{
							if (item4 == simInvSlot && !flag2)
							{
								simInvSlot5 = item4;
								num6 = GetClassModItemLvl(item4.MyItem);
								flag2 = true;
							}
							else if (item4 == simInvSlot2 && !flag3)
							{
								simInvSlot5 = item4;
								num6 = GetClassModItemLvl(item4.MyItem);
								flag3 = true;
							}
							else
							{
								simInvSlot5 = item4;
								num6 = GetClassModItemLvl(item4.MyItem);
							}
						}
						continue;
					}
					if (item4.ThisSlotType == Item.SlotType.Primary)
					{
						if (MyStats.CharacterClass == GameData.ClassDB.Paladin)
						{
							if ((item2.WeaponDmg > 0 && item2.ThisWeaponType != Item.WeaponType.TwoHandMelee && item2.ThisWeaponType != Item.WeaponType.TwoHandStaff) || ((item2.ThisWeaponType == Item.WeaponType.TwoHandMelee || item2.ThisWeaponType == Item.WeaponType.TwoHandStaff) && item2.ItemLevel - simInvSlot4.MyItem.ItemLevel > 3))
							{
								if (item4.MyItem.ItemLevel < item2.ItemLevel && (float)item4.MyItem.ItemLevel < num6 && !CheckRelic(item2))
								{
									simInvSlot5 = item4;
									num6 = item4.MyItem.ItemLevel;
								}
								else if (StoredItems.Count < 18)
								{
									StoredItems.Add(item2);
								}
								else
								{
									StoredItems[UnityEngine.Random.Range(0, 17)] = item2;
								}
							}
							else if (StoredItems.Count < 18)
							{
								StoredItems.Add(item2);
							}
							else
							{
								StoredItems[UnityEngine.Random.Range(0, 17)] = item2;
							}
						}
						else if (MyStats.CharacterClass == GameData.ClassDB.Duelist)
						{
							if (item2.WeaponDmg > 0)
							{
								if (item4.MyItem.ItemLevel < item2.ItemLevel && (float)item4.MyItem.ItemLevel < num6 && !CheckRelic(item2))
								{
									simInvSlot5 = item4;
									num6 = item4.MyItem.ItemLevel;
								}
								else if (StoredItems.Count < 18)
								{
									StoredItems.Add(item2);
								}
								else
								{
									StoredItems[UnityEngine.Random.Range(0, 17)] = item2;
								}
							}
							else if (StoredItems.Count < 18)
							{
								StoredItems.Add(item2);
							}
							else
							{
								StoredItems[UnityEngine.Random.Range(0, 17)] = item2;
							}
						}
						else if (MyStats.CharacterClass == GameData.ClassDB.Stormcaller)
						{
							if (item2.WeaponDmg > 0 && item2.IsBow)
							{
								if (item4.MyItem.ItemLevel < item2.ItemLevel && (float)item4.MyItem.ItemLevel < num6 && !CheckRelic(item2))
								{
									simInvSlot5 = item4;
									num6 = item4.MyItem.ItemLevel;
								}
								else if (StoredItems.Count < 18)
								{
									StoredItems.Add(item2);
								}
								else
								{
									StoredItems[UnityEngine.Random.Range(0, 17)] = item2;
								}
							}
							else if (StoredItems.Count < 18)
							{
								StoredItems.Add(item2);
							}
							else
							{
								StoredItems[UnityEngine.Random.Range(0, 17)] = item2;
							}
						}
						else if (item4.MyItem.ItemLevel < item2.ItemLevel && (float)item4.MyItem.ItemLevel < num6 && !CheckRelic(item2))
						{
							simInvSlot5 = item4;
							num6 = item4.MyItem.ItemLevel;
						}
						else if (StoredItems.Count < 18)
						{
							StoredItems.Add(item2);
						}
						else
						{
							StoredItems[UnityEngine.Random.Range(0, 17)] = item2;
						}
					}
					if (item4.ThisSlotType != Item.SlotType.Secondary)
					{
						continue;
					}
					if (MyStats.CharacterClass == GameData.ClassDB.Paladin)
					{
						if (item2.Shield && simInvSlot3.MyItem.ThisWeaponType != Item.WeaponType.TwoHandMelee && simInvSlot3.MyItem.ThisWeaponType != Item.WeaponType.TwoHandStaff)
						{
							if (item4.MyItem.ItemLevel < item2.ItemLevel && (float)item4.MyItem.ItemLevel < num6 && !CheckRelic(item2))
							{
								simInvSlot5 = item4;
								num6 = item4.MyItem.ItemLevel;
							}
							else if (StoredItems.Count < 18)
							{
								StoredItems.Add(item2);
							}
							else
							{
								StoredItems[UnityEngine.Random.Range(0, 17)] = item2;
							}
						}
						else if (StoredItems.Count < 18)
						{
							StoredItems.Add(item2);
						}
						else
						{
							StoredItems[UnityEngine.Random.Range(0, 17)] = item2;
						}
					}
					else if (MyStats.CharacterClass == GameData.ClassDB.Duelist)
					{
						if (item2.WeaponDmg > 0)
						{
							if (item4.MyItem.ItemLevel < item2.ItemLevel && (float)item4.MyItem.ItemLevel < num6 && !CheckRelic(item2))
							{
								simInvSlot5 = item4;
								num6 = item4.MyItem.ItemLevel;
							}
							else if (StoredItems.Count < 18)
							{
								StoredItems.Add(item2);
							}
							else
							{
								StoredItems[UnityEngine.Random.Range(0, 17)] = item2;
							}
						}
						else if (StoredItems.Count < 18)
						{
							StoredItems.Add(item2);
						}
						else
						{
							StoredItems[UnityEngine.Random.Range(0, 17)] = item2;
						}
					}
					else if (GetClassModItemLvl(item4.MyItem) < GetClassModItemLvl(item2) && GetClassModItemLvl(item4.MyItem) < num6 && !CheckRelic(item2))
					{
						simInvSlot5 = item4;
						num6 = GetClassModItemLvl(item4.MyItem);
					}
					else if (StoredItems.Count < 18)
					{
						StoredItems.Add(item2);
					}
					else
					{
						StoredItems[UnityEngine.Random.Range(0, 17)] = item2;
					}
				}
				if (simInvSlot5 != null)
				{
					bool flag5 = false;
					if (AllHeldItems.Count > 0)
					{
						foreach (ItemSaveData allHeldItem in AllHeldItems)
						{
							if (allHeldItem.ID == simInvSlot5.MyItem.Id)
							{
								flag5 = true;
							}
						}
					}
					if (!flag5 || simInvSlot5.MyItem.RequiredSlot == Item.SlotType.Primary || simInvSlot5.MyItem.RequiredSlot == Item.SlotType.Secondary || simInvSlot5.MyItem.RequiredSlot == Item.SlotType.PrimaryOrSecondary)
					{
						AllHeldItems.Add(new ItemSaveData(simInvSlot5.MyItem.Id, simInvSlot5.Quant));
					}
					simInvSlot5.MyItem = item2;
					num2--;
					if (num2 == 0)
					{
						l = num;
					}
				}
			}
			else if (StoredItems.Count < 18)
			{
				StoredItems.Add(item2);
			}
			else
			{
				StoredItems[UnityEngine.Random.Range(0, 17)] = item2;
			}
			if (MyStats.Level > 5 && _hours >= 6)
			{
				foreach (SimInvSlot item5 in MyEquipment)
				{
					if (item5.MyItem == GameData.PlayerInv.Empty)
					{
						item5.MyItem = FindLowLevelItem(item5.ThisSlotType, MyStats.CharacterClass);
					}
				}
			}
			foreach (SimInvSlot item6 in MyEquipment)
			{
				if (item6.MyItem == GameData.PlayerInv.Empty && (item6.ThisSlotType == Item.SlotType.Leg || item6.ThisSlotType == Item.SlotType.Chest))
				{
					item6.MyItem = FindLowLevelItem(item6.ThisSlotType, MyStats.CharacterClass);
				}
			}
		}
		if (MyStats.Level > 8 && _hours >= 6)
		{
			List<SimInvSlot> list = new List<SimInvSlot>();
			foreach (SimInvSlot item7 in MyEquipment)
			{
				if (item7.MyItem.ItemLevel < MyStats.Level - 6)
				{
					list.Add(item7);
				}
			}
			if (list.Count > 0)
			{
				foreach (SimInvSlot slot in list)
				{
					List<Item> list2 = GameData.ItemDB.ItemDB.Where((Item item) => item != GameData.PlayerInv.Empty && (item.RequiredSlot == slot.ThisSlotType || ((slot.ThisSlotType == Item.SlotType.Primary || slot.ThisSlotType == Item.SlotType.Secondary) && item.RequiredSlot == Item.SlotType.PrimaryOrSecondary)) && item.ItemLevel >= MyStats.Level - 8 && item.ItemLevel <= MyStats.Level - 3 && item.Classes.Contains(Myself.MyStats.CharacterClass) && !CheckRelic(item)).ToList();
					Item empty = GameData.PlayerInv.Empty;
					int num7 = 3000;
					if (list2.Count <= 0)
					{
						continue;
					}
					empty = list2[UnityEngine.Random.Range(0, list2.Count)];
					num7--;
					if (!(empty != null) || empty.RequiredSlot != slot.ThisSlotType || num7 <= 0)
					{
						continue;
					}
					bool flag6 = false;
					if (AllHeldItems.Count > 0)
					{
						foreach (ItemSaveData allHeldItem2 in AllHeldItems)
						{
							if (allHeldItem2.ID == slot.MyItem.Id)
							{
								flag6 = true;
							}
						}
					}
					if (!flag6)
					{
						AllHeldItems.Add(new ItemSaveData(slot.MyItem.Id, slot.Quant));
					}
					slot.MyItem = empty;
				}
			}
		}
		AuditInventory();
		SaveSim();
	}

	private Item FindLowLevelItem(Item.SlotType _type, Class _class)
	{
		Item[] itemDB = GameData.ItemDB.ItemDB;
		foreach (Item item in itemDB)
		{
			if (((MyStats.Level <= 12 && item.ItemLevel < 6) || (MyStats.Level > 12 && item.ItemLevel < MyStats.Level / 2 && item.ItemLevel > MyStats.Level / 4)) && item.RequiredSlot == _type && item.Classes.Contains(_class))
			{
				return item;
			}
		}
		return GameData.PlayerInv.Empty;
	}

	public bool IsThatAnUpgrade(Item _item)
	{
		if (!_item.Classes.Contains(Myself.MyStats.CharacterClass))
		{
			return false;
		}
		foreach (SimInvSlot item in MyEquipment)
		{
			if (item.MyItem == _item && _item.Relic)
			{
				return false;
			}
		}
		if (AllHeldItems.Count > 0)
		{
			foreach (ItemSaveData allHeldItem in AllHeldItems)
			{
				if (allHeldItem != null && allHeldItem.ID == _item.Id)
				{
					return false;
				}
			}
		}
		foreach (SimInvSlot item2 in MyEquipment)
		{
			if ((item2.ThisSlotType == _item.RequiredSlot || ((item2.ThisSlotType == Item.SlotType.Primary || item2.ThisSlotType == Item.SlotType.Secondary) && _item.RequiredSlot == Item.SlotType.PrimaryOrSecondary)) && (_item.ItemLevel > item2.MyItem.ItemLevel + 3 || item2.MyItem == GameData.PlayerInv.Empty) && CheckStats(_item, item2.MyItem, item2.Quant))
			{
				return true;
			}
		}
		return false;
	}

	private bool CheckStats(Item _inc, Item _cur, int _itemLv)
	{
		if (_inc == null)
		{
			return false;
		}
		if (_cur == null)
		{
			return false;
		}
		if (_cur == GameData.PlayerInv.Empty && _inc.WeaponDmg <= 0)
		{
			return true;
		}
		int num = Mathf.RoundToInt((float)MyStats.Level / 2f);
		if (MyStats.CharacterClass == GameData.ClassDB.Paladin)
		{
			bool flag = true;
			if (MyStats.StrScaleMod > MyStats.EndScaleMod)
			{
				flag = false;
			}
			if (!flag)
			{
				if (_inc.WeaponDmg >= _cur.CalcDmg(_cur.WeaponDmg, _itemLv) && !_cur.Shield)
				{
					return true;
				}
				if (MyStats.IntScaleMod >= MyStats.StrScaleMod)
				{
					if (_inc.Int >= _cur.CalcStat(_cur.Int, _itemLv))
					{
						return true;
					}
				}
				else if (_inc.Str >= _cur.CalcStat(_cur.Str, _itemLv))
				{
					return true;
				}
			}
			else
			{
				if (_inc.End >= _cur.CalcStat(_cur.End, _itemLv) && _inc.AC >= _cur.CalcACHPMC(_cur.AC, _itemLv) && _inc.HP >= _cur.CalcACHPMC(_cur.HP, _itemLv))
				{
					return true;
				}
				if (_inc.Agi >= _cur.CalcStat(_cur.Agi, _itemLv) && _inc.AC >= _cur.CalcACHPMC(_cur.AC, _itemLv) - num && _inc.HP >= _cur.CalcACHPMC(_cur.HP, _itemLv) - num * 2)
				{
					return true;
				}
				if (_inc.AC >= _cur.CalcACHPMC(_cur.AC, _itemLv) && _inc.End >= _cur.CalcStat(_cur.End, _itemLv) - num && _inc.Agi >= _cur.CalcStat(_cur.Agi, _itemLv) - num)
				{
					return true;
				}
			}
		}
		if (MyStats.CharacterClass == GameData.ClassDB.Duelist)
		{
			float num2 = 0f;
			float num3 = 0f;
			if (_cur != null && _cur.WeaponDmg > 0 && _cur.WeaponDly > 0f && _inc.WeaponDly > 0f && _inc.WeaponDmg > 0)
			{
				num2 = (float)_cur.CalcDmg(_cur.WeaponDmg, _itemLv) / _cur.WeaponDly;
				num3 = (float)_inc.WeaponDmg / _inc.WeaponDly;
			}
			if (_inc.Dex >= _cur.CalcStat(_cur.Dex, _itemLv) && num2 == 0f)
			{
				return true;
			}
			if (_inc.Str >= _cur.CalcStat(_cur.Str, _itemLv) && num2 == 0f)
			{
				return true;
			}
			if (num3 > num2 && _inc.WeaponDly <= _cur.WeaponDly && _inc.Str >= _cur.CalcStat(_cur.Str, _itemLv) - num && _inc.Dex >= _cur.CalcStat(_cur.Dex, _itemLv) - num)
			{
				return true;
			}
			if (_inc.WeaponProcOnHit != null && num3 >= num2 - 0.2f && (_cur.WeaponProcOnHit == null || _cur.WeaponProcOnHit.TargetDamage < _inc.WeaponProcOnHit.TargetDamage))
			{
				return true;
			}
		}
		if (MyStats.CharacterClass == GameData.ClassDB.Druid)
		{
			if (MyStats.WisScaleMod > MyStats.IntScaleMod)
			{
				if (_inc.Wis >= _cur.CalcStat(_cur.Wis, _itemLv) && _inc.Res >= _cur.CalcRes(_cur.Res, _itemLv))
				{
					return true;
				}
				if (_inc.HP > _cur.CalcACHPMC(_cur.HP, _itemLv) + num && _inc.Wis >= _cur.CalcStat(_cur.Wis, _itemLv) - num && _inc.Res >= _cur.CalcRes(_cur.Res, _itemLv) - 1)
				{
					return true;
				}
			}
			else
			{
				if (_inc.Int >= _cur.CalcStat(_cur.Int, _itemLv) && _inc.Res >= _cur.CalcRes(_cur.Res, _itemLv))
				{
					return true;
				}
				if (_inc.HP > _cur.CalcACHPMC(_cur.HP, _itemLv) + num && _inc.Int >= _cur.CalcStat(_cur.Int, _itemLv) - num && _inc.Res >= _cur.CalcRes(_cur.Res, _itemLv) - 1)
				{
					return true;
				}
			}
			if (_inc.Res > _cur.CalcRes(_cur.Res, _itemLv))
			{
				return true;
			}
		}
		if (MyStats.CharacterClass == GameData.ClassDB.Arcanist)
		{
			if (_inc.Res >= _cur.CalcRes(_cur.Res, _itemLv))
			{
				return true;
			}
			if (_inc.Int >= _cur.CalcStat(_cur.Int, _itemLv))
			{
				return true;
			}
			if (_inc.WeaponProcOnHit != null && _inc.RequiredSlot == Item.SlotType.Bracer && _cur.RequiredSlot == Item.SlotType.Bracer && (_cur.WeaponProcOnHit == null || _cur.WeaponProcOnHit.TargetDamage < _inc.WeaponProcOnHit.TargetDamage))
			{
				return true;
			}
			if (_inc.WeaponProcOnHit != null && (_inc.RequiredSlot == Item.SlotType.Primary || _inc.RequiredSlot == Item.SlotType.PrimaryOrSecondary) && (_cur.WeaponProcOnHit == null || _cur.WeaponProcOnHit.TargetDamage < _inc.WeaponProcOnHit.TargetDamage))
			{
				return true;
			}
		}
		if (MyStats.CharacterClass == GameData.ClassDB.Stormcaller)
		{
			if ((_inc.RequiredSlot == Item.SlotType.Primary || _inc.RequiredSlot == Item.SlotType.Secondary || _inc.RequiredSlot == Item.SlotType.PrimaryOrSecondary) && !_inc.IsBow)
			{
				return false;
			}
			if (MyStats.IntScaleMod > MyStats.StrScaleMod && MyStats.IntScaleMod > MyStats.DexScaleMod)
			{
				if (_inc.Int >= _cur.CalcStat(_cur.Int, _itemLv))
				{
					return true;
				}
				if (_inc.WeaponProcOnHit != null && (_cur.WeaponProcOnHit == null || _inc.WeaponProcOnHit.TargetDamage > _cur.WeaponProcOnHit.TargetDamage) && _inc.IsBow)
				{
					return true;
				}
				if (_inc.WeaponProcOnHit != null && _inc.RequiredSlot == Item.SlotType.Bracer && _cur.RequiredSlot == Item.SlotType.Bracer && (_cur.WeaponProcOnHit == null || _inc.WeaponProcOnHit.TargetDamage > _cur.WeaponProcOnHit.TargetDamage))
				{
					return true;
				}
			}
			else
			{
				if (_inc.Dex >= _cur.CalcStat(_cur.Dex, _itemLv))
				{
					return true;
				}
				if (_inc.WeaponDmg >= _cur.CalcDmg(_cur.WeaponDmg, _itemLv) && _inc.WeaponDly <= _cur.WeaponDly && _inc.IsBow)
				{
					return true;
				}
			}
			if (_inc.Res > _cur.CalcRes(_cur.Res, _itemLv))
			{
				return true;
			}
		}
		if (MyStats.CharacterClass == GameData.ClassDB.Reaver)
		{
			if (_inc.Str >= _cur.CalcStat(_cur.Str, _itemLv) && _inc.HP >= _cur.CalcACHPMC(_cur.HP, _itemLv))
			{
				return true;
			}
			if (_inc.Int >= _cur.CalcStat(_cur.Int, _itemLv) && _inc.HP >= _cur.CalcACHPMC(_cur.HP, _itemLv))
			{
				return true;
			}
			if (_inc.WeaponDmg >= _cur.CalcDmg(_cur.WeaponDmg, _itemLv))
			{
				return true;
			}
			if (_inc.WeaponProcOnHit != null && (_cur.WeaponProcOnHit == null || _cur.WeaponProcOnHit.TargetDamage < _inc.WeaponProcOnHit.TargetDamage))
			{
				return true;
			}
		}
		return false;
	}

	public void LoadSimSkills()
	{
		MyNPC.MyAttackSkills.Clear();
		KnownStances.Clear();
		Skill[] skillDatabase = GameData.SkillDatabase.SkillDatabase;
		foreach (Skill skill in skillDatabase)
		{
			if (skill.TypeOfSkill == Skill.SkillType.Attack || skill.TypeOfSkill == Skill.SkillType.Ranged)
			{
				if (skill.PaladinRequiredLevel > 0 && Myself.MyStats.CharacterClass.ClassName == "Paladin" && MyStats.Level >= skill.PaladinRequiredLevel)
				{
					MyNPC.MyAttackSkills.Add(skill);
					Skillbook.Add(new SimPlayerSkillSlot(skill));
				}
				if (skill.DuelistRequiredLevel > 0 && Myself.MyStats.CharacterClass.ClassName == "Duelist" && MyStats.Level >= skill.DuelistRequiredLevel)
				{
					MyNPC.MyAttackSkills.Add(skill);
					Skillbook.Add(new SimPlayerSkillSlot(skill));
				}
				if (skill.DruidRequiredLevel > 0 && Myself.MyStats.CharacterClass.ClassName == "Druid" && MyStats.Level >= skill.DruidRequiredLevel)
				{
					MyNPC.MyAttackSkills.Add(skill);
					Skillbook.Add(new SimPlayerSkillSlot(skill));
				}
				if (skill.ArcanistRequiredLevel > 0 && Myself.MyStats.CharacterClass.ClassName == "Arcanist" && MyStats.Level >= skill.ArcanistRequiredLevel)
				{
					MyNPC.MyAttackSkills.Add(skill);
					Skillbook.Add(new SimPlayerSkillSlot(skill));
				}
				if (skill.StormcallerRequiredLevel > 0 && Myself.MyStats.CharacterClass.ClassName == "Stormcaller" && MyStats.Level >= skill.StormcallerRequiredLevel)
				{
					MyNPC.MyAttackSkills.Add(skill);
					Skillbook.Add(new SimPlayerSkillSlot(skill));
				}
				if (skill.ReaverRequiredLevel > 0 && Myself.MyStats.CharacterClass.ClassName == "Reaver" && MyStats.Level >= skill.ReaverRequiredLevel)
				{
					MyNPC.MyAttackSkills.Add(skill);
					Skillbook.Add(new SimPlayerSkillSlot(skill));
				}
			}
			if (skill.TypeOfSkill == Skill.SkillType.Utility && Myself.MyStats.CharacterClass.ClassName == "Reaver" && skill.ReaverRequiredLevel > 0 && MyStats.Level >= skill.ReaverRequiredLevel && skill.StanceToUse != null)
			{
				KnownStances.Add(skill.StanceToUse);
			}
		}
	}

	private void CleanAllHeldItems()
	{
		Dictionary<Item.SlotType, List<Item>> dictionary = new Dictionary<Item.SlotType, List<Item>>();
		new Dictionary<string, int>();
		Dictionary<string, ItemSaveData> dictionary2 = new Dictionary<string, ItemSaveData>();
		foreach (ItemSaveData allHeldItem in AllHeldItems)
		{
			Item itemByID = GameData.ItemDB.GetItemByID(allHeldItem.ID);
			if (dictionary2.TryGetValue(allHeldItem.ID, out var value))
			{
				if (allHeldItem.Quality > value.Quality)
				{
					dictionary2[allHeldItem.ID] = allHeldItem;
				}
			}
			else
			{
				dictionary2[allHeldItem.ID] = allHeldItem;
			}
			Item.SlotType requiredSlot = itemByID.RequiredSlot;
			if (!dictionary.ContainsKey(requiredSlot))
			{
				dictionary[requiredSlot] = new List<Item>();
			}
			dictionary[requiredSlot].Add(itemByID);
		}
		List<ItemSaveData> list = new List<ItemSaveData>();
		foreach (KeyValuePair<Item.SlotType, List<Item>> item in dictionary)
		{
			List<Item> value2 = item.Value;
			value2.Sort((Item a, Item b) => b.ItemLevel.CompareTo(a.ItemLevel));
			foreach (Item item2 in value2.Take(8))
			{
				if (dictionary2.TryGetValue(item2.Id, out var value3))
				{
					list.Add(value3);
				}
			}
		}
		AllHeldItems = list;
	}

	public void LimitHeldQuantities()
	{
		new HashSet<Item.SlotType>
		{
			Item.SlotType.Bracer,
			Item.SlotType.Ring,
			Item.SlotType.Primary,
			Item.SlotType.Secondary,
			Item.SlotType.PrimaryOrSecondary
		};
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		Dictionary<Item.SlotType, int> dictionary2 = new Dictionary<Item.SlotType, int>();
		foreach (SimInvSlot item in MyEquipment)
		{
			if (!(item.MyItem == null) && !(item.MyItem == GameData.PlayerInv.Empty))
			{
				string id = item.MyItem.Id;
				Item.SlotType requiredSlot = item.MyItem.RequiredSlot;
				if (!dictionary.TryAdd(id, 1))
				{
					dictionary[id]++;
				}
				if (!dictionary2.TryAdd(requiredSlot, 1))
				{
					dictionary2[requiredSlot]++;
				}
			}
		}
		IEnumerable<IGrouping<string, ItemSaveData>> enumerable = from item in AllHeldItems
			group item by item.ID;
		List<ItemSaveData> itemsToRemove = new List<ItemSaveData>();
		Dictionary<string, Item> dictionary3 = new Dictionary<string, Item>();
		foreach (IGrouping<string, ItemSaveData> item2 in enumerable)
		{
			string key = item2.Key;
			if (!dictionary3.TryGetValue(key, out var value))
			{
				value = (dictionary3[key] = GameData.ItemDB.GetItemByID(key));
			}
			if (value == null)
			{
				itemsToRemove.AddRange(item2);
				Debug.LogWarning("SimPlayer '" + MyNPC.NPCName + "' has an unknown item with ID '" + key + "' in AllHeldItems. Removing it.");
				continue;
			}
			int num = (value.Relic ? 1 : 2);
			int value2;
			int num2 = (dictionary.TryGetValue(key, out value2) ? value2 : 0);
			int num3 = Mathf.Max(num - num2, 0);
			if (item2.Count() > num3)
			{
				itemsToRemove.AddRange(item2.OrderByDescending((ItemSaveData x) => x.Quality).Skip(num3));
			}
		}
		if (itemsToRemove.Count > 0)
		{
			AllHeldItems.RemoveAll((ItemSaveData item) => itemsToRemove.Contains(item));
		}
	}

	private bool PreventDowngrade(Item _item, int _qual)
	{
		if ((_item.RequiredSlot == Item.SlotType.PrimaryOrSecondary || _item.RequiredSlot == Item.SlotType.Ring || _item.RequiredSlot == Item.SlotType.Bracer) && !_item.Relic)
		{
			return false;
		}
		foreach (ItemSaveData allHeldItem in AllHeldItems)
		{
			if (allHeldItem.ID == _item.Id && allHeldItem.Quality > _qual)
			{
				UpdateSocialLog.LogAdd(MyNPC.NPCName + " says: " + GameData.SimMngr.PersonalizeString("Oops, I have a better one of these.", this));
				return true;
			}
		}
		return false;
	}

	private bool CheckForSameItemUpgrade(Item _item, int _qual)
	{
		if ((_item.RequiredSlot == Item.SlotType.PrimaryOrSecondary || _item.RequiredSlot == Item.SlotType.Ring || _item.RequiredSlot == Item.SlotType.Bracer) && !_item.Relic)
		{
			return false;
		}
		foreach (SimInvSlot item in MyEquipment)
		{
			if (item.MyItem.Id == _item.Id && item.Quant < _qual)
			{
				return true;
			}
		}
		return false;
	}

	public bool SwapForStoredItem(Item _item, string _forceSlot, int _qual)
	{
		Item item = null;
		returnItemQual = 1;
		Item item2 = null;
		returnSecondItemQual = 1;
		bool result = false;
		bool flag = false;
		if (_item.Classes.Contains(MyStats.CharacterClass) && _item != GameData.PlayerInv.Empty && !CheckRelic(_item) && !HandConflict(_item, _forceSlot))
		{
			if (_item.RequiredSlot == Item.SlotType.Bracer)
			{
				int num = 0;
				num = ((!(_forceSlot == "0")) ? 1 : 0);
				foreach (SimInvSlot item3 in MyEquipment)
				{
					if (item3.ThisSlotType == Item.SlotType.Bracer && num == 0)
					{
						item = item3.MyItem;
						returnItemQual = item3.Quant;
						item3.MyItem = _item;
						item3.Quant = _qual;
						result = true;
						break;
					}
					if (item3.ThisSlotType == Item.SlotType.Bracer)
					{
						num--;
					}
				}
			}
			else if (_item.RequiredSlot == Item.SlotType.Ring)
			{
				int num2 = 0;
				num2 = ((!(_forceSlot == "0")) ? 1 : 0);
				foreach (SimInvSlot item4 in MyEquipment)
				{
					if (item4.ThisSlotType == Item.SlotType.Ring && num2 == 0)
					{
						item = item4.MyItem;
						returnItemQual = item4.Quant;
						item4.MyItem = _item;
						item4.Quant = _qual;
						result = true;
						break;
					}
					if (item4.ThisSlotType == Item.SlotType.Ring)
					{
						num2--;
					}
				}
			}
			else if (_item.RequiredSlot == Item.SlotType.PrimaryOrSecondary)
			{
				int num3 = 0;
				num3 = ((!(_forceSlot == "0")) ? 1 : 0);
				foreach (SimInvSlot item5 in MyEquipment)
				{
					if (item5.ThisSlotType == Item.SlotType.Primary && num3 == 0)
					{
						item = item5.MyItem;
						returnItemQual = item5.Quant;
						item5.MyItem = _item;
						item5.Quant = _qual;
						result = true;
						break;
					}
					if (item5.ThisSlotType == Item.SlotType.Secondary && num3 == 1 && !flag)
					{
						item = item5.MyItem;
						returnItemQual = item5.Quant;
						item5.MyItem = _item;
						item5.Quant = _qual;
						result = true;
						break;
					}
				}
				if (flag)
				{
					UpdateSocialLog.LogAdd(MyNPC.NPCName + " says: " + GameData.SimMngr.PersonalizeString("I can't use that with my current setup.", this));
				}
			}
			else
			{
				foreach (SimInvSlot item6 in MyEquipment)
				{
					if (item6.ThisSlotType == _item.RequiredSlot)
					{
						if (!CheckForSameItemUpgrade(_item, _qual))
						{
							item = item6.MyItem;
							returnItemQual = item6.Quant;
						}
						else
						{
							item = GameData.PlayerInv.Empty;
							returnItemQual = 0;
						}
						item6.MyItem = _item;
						item6.Quant = _qual;
						result = true;
					}
				}
			}
			item2 = CheckSlotCompat();
			if (item != null && _item.ItemLevel > item.ItemLevel)
			{
				if (item != null)
				{
					if (item != GameData.PlayerInv.Empty)
					{
						UpdateSocialLog.LogAdd(MyNPC.NPCName + " says: " + GameData.SimMngr.PersonalizeString("Ok, I'll switch that item out.", this));
						AllHeldItems.Add(new ItemSaveData(item.Id, returnItemQual));
					}
					else
					{
						UpdateSocialLog.LogAdd(MyNPC.NPCName + " says: " + GameData.SimMngr.PersonalizeString("Ok, I'll equip that.", this));
					}
				}
				if (item2 != null && item2 != GameData.PlayerInv.Empty)
				{
					AllHeldItems.Add(new ItemSaveData(item2.Id, returnSecondItemQual));
				}
			}
			else if (item != null)
			{
				UpdateSocialLog.LogAdd(MyNPC.NPCName + " says: " + GameData.SimMngr.PersonalizeString("Ok, I'll use this for now.", this));
				AllHeldItems.Add(new ItemSaveData(item.Id, returnItemQual));
				if (item2 != null && item2 != GameData.PlayerInv.Empty)
				{
					AllHeldItems.Add(new ItemSaveData(item2.Id, returnSecondItemQual));
				}
			}
		}
		AuditInventory();
		if (GameData.InspectSim.InspectWindow.activeSelf)
		{
			GameData.InspectSim.CloseWindow();
			GameData.InspectSim.InspectSim(this);
		}
		SaveSim();
		return result;
	}

	public void ImpressedByGear()
	{
		int num = 0;
		string text = "";
		int num2 = 0;
		foreach (ItemIcon equipmentSlot in GameData.PlayerInv.EquipmentSlots)
		{
			if (!(equipmentSlot != null) || (equipmentSlot.ThisSlotType != Item.SlotType.Primary && equipmentSlot.ThisSlotType != Item.SlotType.Secondary && equipmentSlot.ThisSlotType != Item.SlotType.PrimaryOrSecondary && equipmentSlot.ThisSlotType != Item.SlotType.Chest && equipmentSlot.ThisSlotType != Item.SlotType.Head && equipmentSlot.ThisSlotType != Item.SlotType.Back) || !(equipmentSlot != GameData.PlayerInv.Empty) || num >= equipmentSlot.MyItem.ItemLevel)
			{
				continue;
			}
			num2 = UnityEngine.Random.Range(-6, 6);
			num = equipmentSlot.MyItem.ItemLevel + num2;
			switch (equipmentSlot.ThisSlotType)
			{
			case Item.SlotType.Primary:
				text = ((equipmentSlot.MyItem.WeaponDmg <= 0) ? "thing you're holding" : "weapon");
				break;
			case Item.SlotType.Secondary:
				if (equipmentSlot.MyItem.Shield)
				{
					text = "shield";
				}
				text = ((equipmentSlot.MyItem.WeaponDmg <= 0) ? "thing you're holding" : "weapon");
				break;
			case Item.SlotType.Chest:
				text = ((UnityEngine.Random.Range(0, 10) <= 5) ? "bp" : "chest item");
				break;
			case Item.SlotType.Head:
				text = ((UnityEngine.Random.Range(0, 10) <= 5) ? "helm" : "hat");
				break;
			case Item.SlotType.Back:
				text = ((UnityEngine.Random.Range(0, 10) <= 5) ? "cape" : "back slot");
				break;
			}
		}
		if (text != "")
		{
			GameData.SimMngr.LoadResponse("[WHISPER FROM] " + base.transform.name + ": " + GameData.SimMngr.PersonalizeString(MyDialog.GetImpressed() + " " + text + " " + MyDialog.GetImpressedEnd(), this), base.transform.name);
		}
	}

	public void ReceivePlanar()
	{
	}

	private void ReplaceObsoleteGear()
	{
	}

	private void LoadInventory(List<string> inventory, SimPlayerSaveData myData)
	{
		AllHeldItems.Clear();
		foreach (string item in inventory)
		{
			if (string.IsNullOrEmpty(item))
			{
				continue;
			}
			string text = item.Trim();
			if (text.Length < 2 || text[0] != '{' || text[^1] != '}')
			{
				continue;
			}
			try
			{
				ItemSaveData itemSaveData = JsonUtility.FromJson<ItemSaveData>(text);
				if (itemSaveData != null)
				{
					AllHeldItems.Add(itemSaveData);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("Error deserializing item: " + ex.Message);
			}
		}
		if (myData != null && myData.MyEquippedItems != null && MyEquipment != null && MyInv != null)
		{
			int num = 0;
			int num2 = 0;
			bool flag = myData.ItemQuantities != null && myData.ItemQuantities.Count >= myData.MyEquippedItems.Count;
			bool flag2 = false;
			bool flag3 = false;
			int num3 = -1;
			int num4 = -1;
			SimInvSlot simInvSlot = null;
			SimInvSlot simInvSlot2 = null;
			SimInvSlot simInvSlot3 = null;
			SimInvSlot simInvSlot4 = null;
			foreach (SimInvSlot item2 in MyEquipment)
			{
				if (item2 != null && item2 != simInvSlot && item2 != simInvSlot2 && item2 == simInvSlot3)
				{
				}
			}
			foreach (string myEquippedItem in myData.MyEquippedItems)
			{
				if (!string.IsNullOrEmpty(myEquippedItem))
				{
					Item itemByID = GameData.ItemDB.GetItemByID(myEquippedItem);
					if (itemByID != null)
					{
						foreach (SimInvSlot item3 in MyEquipment)
						{
							if (item3 == null || (!(item3.MyItem == null) && !(item3.MyItem == GameData.PlayerInv.Empty)) || item3.ThisSlotType != itemByID.RequiredSlot)
							{
								continue;
							}
							if (item3 == simInvSlot && !flag2)
							{
								num3 = num;
								item3.MyItem = itemByID;
								if (flag)
								{
									item3.Quant = myData.ItemQuantities[num2];
								}
								flag2 = true;
							}
							else if (item3 == simInvSlot3 && !flag3)
							{
								num4 = num;
								item3.MyItem = itemByID;
								if (flag)
								{
									item3.Quant = myData.ItemQuantities[num2];
								}
								flag3 = true;
							}
							else if (item3 == simInvSlot2 && num != num3)
							{
								item3.MyItem = itemByID;
								if (flag)
								{
									item3.Quant = myData.ItemQuantities[num2];
								}
							}
							else if (item3 == simInvSlot4 && num != num4)
							{
								item3.MyItem = itemByID;
								if (flag)
								{
									item3.Quant = myData.ItemQuantities[num2];
								}
							}
							else
							{
								item3.MyItem = itemByID;
								if (flag)
								{
									item3.Quant = myData.ItemQuantities[num2];
								}
							}
							break;
						}
					}
					else
					{
						UpdateSocialLog.LogAdd("[Debug] Load issue on SimPlayer - item id not found in DB.", "yellow");
					}
				}
				else
				{
					UpdateSocialLog.LogAdd("[Debug] Load issue on SimPlayer - saved item string was NULL OR EMPTY.", "yellow");
				}
				num2++;
				num++;
			}
			foreach (string myEquippedItem2 in myData.MyEquippedItems)
			{
				if (string.IsNullOrEmpty(myEquippedItem2))
				{
					continue;
				}
				Item itemByID2 = GameData.ItemDB.GetItemByID(myEquippedItem2);
				if (!(itemByID2 != null) || itemByID2.RequiredSlot != Item.SlotType.PrimaryOrSecondary)
				{
					continue;
				}
				foreach (SimInvSlot item4 in MyEquipment)
				{
					if (item4 == null)
					{
						continue;
					}
					bool num5 = item4.MyItem == null || item4.MyItem == GameData.PlayerInv.Empty;
					bool flag4 = item4.ThisSlotType == Item.SlotType.Primary || item4.ThisSlotType == Item.SlotType.Secondary;
					if (num5 && flag4)
					{
						item4.MyItem = itemByID2;
						if (item4.ThisSlotType == Item.SlotType.Primary)
						{
							item4.Quant = myData.MHQ;
						}
						if (item4.ThisSlotType == Item.SlotType.Secondary)
						{
							item4.Quant = myData.OHQ;
						}
						break;
					}
				}
			}
			MyInv.EquippedItems.Clear();
			foreach (SimInvSlot item5 in MyEquipment)
			{
				if (item5 != null)
				{
					MyInv.EquippedItems.Add(item5.MyItem);
				}
			}
		}
		else
		{
			Debug.LogWarning("LoadInventoryAsync: myData/MyEquipment/MyInv missing. Equipped item pass skipped.");
		}
		CalcTimeSinceLastSeen(myData);
		OnInventoryLoaded();
	}

	private void OnInventoryLoaded()
	{
		IsFullyLoaded = true;
		if (GameData.SimMngr.Sims[myIndex].MyAHData != null && StoredItems.Count > 0)
		{
			List<string> list = new List<string>();
			foreach (Item storedItem in StoredItems)
			{
				if (storedItem.ItemValue > 0)
				{
					list.Add(storedItem.Id);
				}
			}
			if (list.Count > 0)
			{
				AuctionHouse.UpdateAH(list, MyNPC.NPCName, Greed);
			}
			StoredItems.Clear();
		}
		AuditInventory();
	}

	private void SitLogic()
	{
		if (MyNav.velocity == Vector3.zero && !MySpells.isCasting() && MyNPC.CurrentAggroTarget == null && !IsGMCharacter)
		{
			if ((InGroup && MyStats.CurrentHP < MyStats.CurrentMaxHP) || MyStats.CurrentMana < MyStats.GetCurrentMaxMana())
			{
				sitTimer = sitTolerance;
			}
			if (sitTimer < sitTolerance)
			{
				sitTimer += 60f * Time.deltaTime;
			}
			else if (UnityEngine.Random.Range(0, 1000) > 995 && !sitting)
			{
				MyAnim.SetTrigger("SitDown");
				MyAnim.ResetTrigger("StandUp");
				Myself.MyAudio.PlayOneShot(GameData.Misc.SitSound, Myself.MyAudio.volume * 0.5f * GameData.SFXVol * GameData.MasterVol);
				sitting = true;
			}
		}
		else if (sitting)
		{
			sitting = false;
			sitTimer = 0f;
			MyAnim.SetTrigger("StandUp");
			Myself.MyAudio.PlayOneShot(GameData.Misc.StandSound, Myself.MyAudio.volume * 0.5f * GameData.SFXVol * GameData.MasterVol);
		}
	}

	public bool IsGroupAggrodOnTarget(Character _targ)
	{
		int num = 0;
		int num2 = 0;
		SimPlayerTracking[] groupMembers = GameData.GroupMembers;
		foreach (SimPlayerTracking simPlayerTracking in groupMembers)
		{
			if (simPlayerTracking != null && simPlayerTracking.MyAvatar != null && simPlayerTracking.MyAvatar != this)
			{
				num++;
				if (simPlayerTracking.MyAvatar.GetThisNPC().CurrentAggroTarget == _targ)
				{
					num2++;
				}
			}
		}
		if (num == num2 && num > 0)
		{
			return true;
		}
		return false;
	}
}
