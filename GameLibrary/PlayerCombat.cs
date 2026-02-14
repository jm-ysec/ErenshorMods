// PlayerCombat
using TMPro;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
	private bool Autoattack;

	private float AttackDel = 100f;

	private Animator MyAnim;

	private PlayerControl MyControl;

	private Stats myStats;

	public PlayerMeleeArea MyMeleeRange;

	public AnimationClip TwoHandSwordIdle;

	public AnimationClip TwoHandStaffIdle;

	public AnimationClip ArmedIdle;

	public AnimationClip RelaxedIdle;

	public AudioClip AutoAttackOn;

	public AudioClip AutoAttackoff;

	public TextMeshProUGUI DPS;

	private float DPSUpdateRate = 20f;

	private float SpamAAWarningCD;

	private void Start()
	{
		GameData.PlayerCombat = this;
		MyAnim = GetComponent<Animator>();
		MyControl = GetComponent<PlayerControl>();
		myStats = GetComponent<Stats>();
		MyMeleeRange = GetComponentInChildren<PlayerMeleeArea>();
	}

	private void Update()
	{
		if (((Input.GetKeyDown(InputManager.Attack) && !GameData.PlayerTyping) || (GameData.PlayerControl.usingGamepad && Input.GetKeyDown(KeyCode.JoystickButton9) && Input.GetAxis("RTrigger") <= 0.2f)) && myStats.Myself.Alive)
		{
			ToggleAttack();
		}
		if (SpamAAWarningCD > 0f)
		{
			SpamAAWarningCD -= 60f * Time.deltaTime;
		}
		if (Autoattack)
		{
			if (MyControl.Swimming)
			{
				ToggleAttack();
			}
			else
			{
				DoAuto();
			}
		}
		if (GameData.Autoattacking)
		{
			_ = GameData.PlayerControl.CurrentTarget == null;
		}
		GameData.InCombat = CheckForTrueCombat();
		if (myStats.CharacterClass == GameData.ClassDB.Paladin || myStats.CharacterClass == GameData.ClassDB.Reaver)
		{
			if (GameData.PlayerInv.SecondaryShield)
			{
				if (!GameData.HKMngr.ShieldBonus.activeSelf)
				{
					GameData.HKMngr.ShieldBonus.SetActive(value: true);
				}
			}
			else if (!GameData.PlayerInv.SecondaryShield && GameData.HKMngr.ShieldBonus.activeSelf)
			{
				GameData.HKMngr.ShieldBonus.SetActive(value: false);
			}
		}
		else if (GameData.HKMngr.ShieldBonus.activeSelf)
		{
			GameData.HKMngr.ShieldBonus.SetActive(value: false);
		}
		if (MyControl.CurrentTarget == null && myStats.Myself.GetDPS() > 0 && myStats.Myself.contributedDPS <= 0f)
		{
			MyControl.Myself.ResetRollingDPS();
		}
		if (DPSUpdateRate > 0f)
		{
			DPSUpdateRate -= 60f * Time.deltaTime;
			if (DPSUpdateRate <= 0f)
			{
				if (myStats.Myself.GetDPS() > 0)
				{
					DPS.text = myStats.Myself.GetDPS().ToString();
				}
				DPSUpdateRate = 10f;
			}
		}
		else
		{
			DPS.text = "-";
		}
	}

	public Character FindNearestLootable()
	{
		Character result = null;
		float num = float.PositiveInfinity;
		if (MyMeleeRange.GetNPCsInRange().Count > 0)
		{
			foreach (Character item in MyMeleeRange.GetNPCsInRange())
			{
				if (item != null && !item.Alive && Vector3.Distance(item.transform.position, base.transform.position) < num)
				{
					result = item;
					num = Vector3.Distance(item.transform.position, base.transform.position);
				}
			}
			return result;
		}
		return result;
	}

	private void ToggleAttack()
	{
		if (!GameData.PlayerControl.enabled)
		{
			return;
		}
		string text = "";
		Autoattack = !Autoattack;
		if (Autoattack && !MyControl.Swimming)
		{
			GameData.Autoattacking = true;
			if (GameData.PlayerStats.Invisible)
			{
				GameData.PlayerStats.BreakEffectsOnAction();
			}
			text = "ON!";
			if (MyAnim.GetBool("2HMelee"))
			{
				MyControl.AnimOverride["Idle"] = TwoHandSwordIdle;
			}
			else if (MyAnim.GetBool("2HStaff"))
			{
				MyControl.AnimOverride["Idle"] = TwoHandStaffIdle;
			}
			else
			{
				MyControl.AnimOverride["Idle"] = ArmedIdle;
			}
		}
		else
		{
			GameData.Autoattacking = false;
			text = "OFF.";
			if (MyControl.Swimming)
			{
				UpdateSocialLog.CombatLogAdd("You cannot fight underwater!");
			}
			else
			{
				MyControl.AnimOverride["Idle"] = RelaxedIdle;
			}
		}
		UpdateSocialLog.CombatLogAdd("Auto Attack " + text);
	}

	public void ForceAttackOff()
	{
		if (Autoattack)
		{
			GameData.Autoattacking = false;
			UpdateSocialLog.CombatLogAdd("Auto Attack OFF!");
			Autoattack = false;
			MyControl.AnimOverride["Idle"] = RelaxedIdle;
		}
	}

	public void ForceAttackOn()
	{
		if (!Autoattack)
		{
			if (GameData.PlayerStats.Invisible)
			{
				GameData.PlayerStats.BreakEffectsOnAction();
			}
			GameData.Autoattacking = true;
			UpdateSocialLog.CombatLogAdd("Auto Attack ON!!");
			Autoattack = true;
			if (MyAnim.GetBool("2HMelee"))
			{
				MyControl.AnimOverride["Idle"] = TwoHandSwordIdle;
			}
			else if (MyAnim.GetBool("2HStaff"))
			{
				MyControl.AnimOverride["Idle"] = TwoHandStaffIdle;
			}
			else
			{
				MyControl.AnimOverride["Idle"] = ArmedIdle;
			}
		}
	}

	private void DoAuto()
	{
		if (myStats.GetMHAtkDelay() > 0f || myStats.CurrentHP <= 0 || !GameData.PlayerControl.Myself.Alive || myStats.Myself.MySpells.isCasting())
		{
			return;
		}
		MyAnim.SetBool("DoubleAttack", value: false);
		MyAnim.SetBool("DualWield", value: false);
		MyAnim.SetInteger("AttackIndex", Random.Range(0, 2));
		myStats.RecentCast = 480f;
		bool flag = ShouldDualWield();
		int attackCount = GetAttackCount();
		Character currentTarget = MyControl.CurrentTarget;
		if (currentTarget != MyControl.Myself)
		{
			if (myStats.MyInv.PrimaryWeapon)
			{
				PerformAttacks(currentTarget, attackCount, isMainHand: true);
			}
			if (flag && myStats.GetOHAtkDelay() <= 0f)
			{
				PerformAttacks(currentTarget, GetOffhandAttackCount(), isMainHand: false);
			}
			TryCoupDeGrace(currentTarget);
			myStats.ResetMHAtkDelay();
			if (GameData.PlayerControl.Myself.MyCharmedNPC != null && currentTarget != null && currentTarget.MyFaction != Character.Faction.Mineral && currentTarget.MyFaction != 0 && !currentTarget.MyStats.Charmed)
			{
				GameData.PlayerControl.Myself.MyCharmedNPC.CurrentAggroTarget = currentTarget;
			}
		}
		else
		{
			HandleInvalidTarget();
			myStats.ResetMHAtkDelay(15f);
		}
	}

	private bool ShouldDualWield()
	{
		if (myStats.Myself.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByName("Dual Wield")) && Random.Range(0, 40) < myStats.Level)
		{
			return myStats.MyInv.SecondaryWeapon;
		}
		return false;
	}

	private int GetAttackCount()
	{
		if (Random.Range(0, 100) < myStats.Myself.MySkills.GetAscensionRank("48388782") * 10)
		{
			return 2;
		}
		if (Random.Range(0, 100) < myStats.Myself.MySkills.GetAscensionRank("27836061") * 12)
		{
			return 2;
		}
		if (!myStats.Myself.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByName("Double Attack")) || Random.Range(0, 35) >= myStats.Level)
		{
			return 0;
		}
		return 1;
	}

	private int GetOffhandAttackCount()
	{
		if (!myStats.Myself.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByName("Double Attack")) || Random.Range(0, 35) >= myStats.Level)
		{
			return 0;
		}
		return 1;
	}

	private bool CheckForWand(bool _isMainhand)
	{
		bool flag = false;
		if (_isMainhand)
		{
			return (GameData.PlayerInv.MH?.MyItem?.IsWand).GetValueOrDefault();
		}
		return (GameData.PlayerInv.OH?.MyItem?.IsWand).GetValueOrDefault();
	}

	private bool CheckForBow(bool _isMainhand)
	{
		bool flag = false;
		if (_isMainhand)
		{
			return (GameData.PlayerInv.MH?.MyItem?.IsBow).GetValueOrDefault();
		}
		return (GameData.PlayerInv.OH?.MyItem?.IsBow).GetValueOrDefault();
	}

	private bool CheckForRange(bool _isWand, bool _isBow, Character _target)
	{
		if (_target == null)
		{
			return false;
		}
		int num = 5;
		if (_isWand && (GameData.PlayerInv.MH?.MyItem?.IsWand).GetValueOrDefault())
		{
			num = GameData.PlayerInv.MH.MyItem.WandRange;
		}
		if (_isBow && (GameData.PlayerInv.MH?.MyItem?.IsBow).GetValueOrDefault())
		{
			num = GameData.PlayerInv.MH.MyItem.BowRange;
		}
		if (!_isBow && !_isWand)
		{
			return MyMeleeRange.GetNPCsInRange().Contains(_target);
		}
		return Vector3.Distance(_target.transform.position, base.transform.position) < (float)num;
	}

	private void PerformAttacks(Character target, int attackCount, bool isMainHand)
	{
		bool flag = CheckForWand(isMainHand);
		bool flag2 = CheckForBow(isMainHand);
		bool flag3 = CheckForRange(flag, flag2, target);
		for (int i = 0; i <= attackCount; i++)
		{
			if (flag3)
			{
				target.FlagForFactionHit(_bool: true);
				if (!flag && !flag2)
				{
					myStats.Myself.MyAudio.PlayOneShot(myStats.Myself.MyAttackSound, myStats.Myself.MyAudio.volume * GameData.SFXVol * GameData.MasterVol);
					if (i == 0)
					{
						MyAnim.SetTrigger(isMainHand ? "MeleeSwing" : "DualWield");
					}
					if (i == 1 && isMainHand)
					{
						MyAnim.SetBool("DoubleAttack", value: true);
					}
					if (i == 1 && !isMainHand)
					{
						MyAnim.SetBool("OHDoubleAttack", value: true);
					}
					if (i == 2)
					{
						MyAnim.SetTrigger("MeleeSwing");
					}
					int dmg = myStats.CalcMeleeDamage(isMainHand ? myStats.MyInv.MHDmg : myStats.MyInv.OHDmg, target.MyStats.Level, target.MyStats, 0);
					string text = CheckTargetInnateAvoidance(target);
					if (text != "")
					{
						UpdateSocialLog.CombatLogAdd("You try to hit " + target.name + ", but " + target.name + " " + text);
						continue;
					}
					if (i == 0 && isMainHand && myStats.CombatStance.SelfDamagePerAttack > 0f)
					{
						float num = myStats.CombatStance.SelfDamagePerAttack / 100f;
						float f = (float)myStats.CurrentMaxHP * num;
						myStats.Myself.SelfDamageMeFlat(Mathf.RoundToInt(f));
					}
					myStats.CheckProc(isMainHand ? GameData.PlayerInv.MH : GameData.PlayerInv.OH, target);
					HandleDamageResult(target, ref dmg, isMainHand);
					if (dmg > 0)
					{
						myStats.RecentDmg = 240f;
					}
					Stance combatStance = myStats.CombatStance;
					if ((object)combatStance != null && combatStance.SelfDamagePerAttack > 0f)
					{
						myStats.Myself.SelfDamageMe(myStats.CombatStance.SelfDamagePerAttack);
					}
				}
				else if (flag)
				{
					myStats.Myself.MyAudio.PlayOneShot(myStats.Myself.MyAttackSound, myStats.Myself.MyAudio.volume * GameData.SFXVol * GameData.MasterVol);
					if (i == 0)
					{
						MyAnim.SetTrigger(isMainHand ? "MeleeSwing" : "DualWield");
					}
					if (i == 1 && isMainHand)
					{
						MyAnim.SetBool("DoubleAttack", value: true);
					}
					if (i == 1 && !isMainHand)
					{
						MyAnim.SetBool("OHDoubleAttack", value: true);
					}
					if (i == 2)
					{
						MyAnim.SetTrigger("MeleeSwing");
					}
					if (IsFacingTarget(base.transform.transform, target.transform))
					{
						DoWandAttack(target);
					}
					else
					{
						UpdateSocialLog.CombatLogAdd("You must be facing your target!");
					}
				}
				else
				{
					if (!flag2)
					{
						continue;
					}
					if (IsFacingTarget(base.transform.transform, target.transform))
					{
						DoBowAttack(target, 0);
						if (myStats.Myself.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByName("Trick Shot")) && Random.Range(0, 100) > 94 && (bool)GameData.PlayerControl.CurrentTarget)
						{
							Vector3 safeNavMeshPointInRange = GameData.GetSafeNavMeshPointInRange(GameData.PlayerControl.CurrentTarget.transform.position, 20f, 4f);
							Object.Instantiate(GameData.Misc.TrickShotTarg, safeNavMeshPointInRange, base.transform.rotation);
							UpdateSocialLog.CombatLogAdd("You find an opening and throw a target into the fray", "lightblue");
						}
					}
					else
					{
						UpdateSocialLog.CombatLogAdd("You must be facing your target!");
					}
				}
			}
			else
			{
				HandleInvalidTarget();
			}
		}
		if (isMainHand)
		{
			if (!flag2)
			{
				myStats.ResetMHAtkDelay();
			}
			else if (myStats.Myself.MySkills.HasAscension("4547270"))
			{
				if (myStats.Myself.MySkills.GetAscensionRank("4547270") * 10 > Random.Range(0, 100))
				{
					myStats.ResetMHAtkDelay(Mathf.RoundToInt(myStats.GetMHAtkDelay() * 0.5f));
					UpdateSocialLog.CombatLogAdd("You perform a quick reload and prepare to fire again!", "lightblue");
				}
			}
			else
			{
				myStats.ResetMHAtkDelay();
			}
		}
		else
		{
			myStats.ResetOHAtkDelay();
		}
	}

	private bool IsFacingTarget(Transform source, Transform target, float angleThreshold = 80f)
	{
		if (source == null || target == null)
		{
			return false;
		}
		Vector3 normalized = (target.position - source.position).normalized;
		return Vector3.Angle(source.forward, normalized) <= angleThreshold;
	}

	private void DoWandAttack(Character _target)
	{
		Item item = GameData.PlayerInv?.MH?.MyItem;
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
				spell = myStats.CheckSEProcsOnly();
			}
			component.LoadWandBolt(item.WeaponDmg, spell, _target, myStats.Myself, item.WandBoltSpeed, GameData.DamageType.Magic, item.WandBoltColor, item.WandAttackSound);
		}
	}

	public void DoBowAttack(Character _target, int _arrowIndex)
	{
		Item item = GameData.PlayerInv?.MH?.MyItem;
		if (!(item == null))
		{
			MyAnim.SetTrigger("FireBow");
			GameData.CamControl.ShakeScreen(1f, 0.1f);
			WandBolt component = Object.Instantiate(GameData.Misc.ArcheryArrows[_arrowIndex], base.transform.position + Vector3.up + base.transform.forward, base.transform.rotation).GetComponent<WandBolt>();
			Spell spell = null;
			if (item.BowEffect != null && (float)Random.Range(0, 100) < item.BowProcChance)
			{
				spell = item.BowEffect;
			}
			if (spell == null)
			{
				spell = myStats.CheckSEProcsOnly();
			}
			component.LoadArrow(item.WeaponDmg, spell, _target, myStats.Myself, item.BowArrowSpeed, GameData.DamageType.Physical, item.BowAttackSound);
		}
	}

	public void DoBowAttack(Character _target, int _arrowIndex, bool _interrupt, Spell _force)
	{
		Item item = GameData.PlayerInv?.MH?.MyItem;
		if (!(item == null))
		{
			MyAnim.SetTrigger("FireBow");
			GameData.CamControl.ShakeScreen(1f, 0.1f);
			WandBolt component = Object.Instantiate(GameData.Misc.ArcheryArrows[_arrowIndex], base.transform.position + Vector3.up + base.transform.forward, base.transform.rotation).GetComponent<WandBolt>();
			Spell spell = _force;
			if (item.BowEffect != null && (float)Random.Range(0, 100) < item.BowProcChance)
			{
				spell = item.BowEffect;
			}
			if (spell == null)
			{
				spell = myStats.CheckSEProcsOnly();
			}
			component.LoadArrow(item.WeaponDmg, spell, _target, myStats.Myself, item.BowArrowSpeed, GameData.DamageType.Physical, item.BowAttackSound, _interrupt);
		}
	}

	public void DoBowAttack(Character _target, int _dmgMod, int _arrowIndex)
	{
		Item item = GameData.PlayerInv?.MH?.MyItem;
		if (!(item == null))
		{
			MyAnim.SetTrigger("FireBow");
			GameData.CamControl.ShakeScreen(1f, 0.1f);
			WandBolt component = Object.Instantiate(GameData.Misc.ArcheryArrows[_arrowIndex], base.transform.position + Vector3.up + base.transform.forward, base.transform.rotation).GetComponent<WandBolt>();
			Spell spell = null;
			if (item.BowEffect != null && (float)Random.Range(0, 100) < item.BowProcChance)
			{
				spell = item.BowEffect;
			}
			if (spell == null)
			{
				spell = myStats.CheckSEProcsOnly();
			}
			if (_dmgMod > 1)
			{
				GameData.CamControl.ShakeScreen(2f, 0.1f);
			}
			component.LoadArrow(item.WeaponDmg * _dmgMod, spell, _target, myStats.Myself, item.BowArrowSpeed, GameData.DamageType.Physical, item.BowAttackSound);
			if (_dmgMod > 4)
			{
				component.DisableCrits();
			}
		}
	}

	public void DoBowAttack(Character _target, Spell _forceProc, int _arrowIndex, bool _noCheckEffect)
	{
		Item item = GameData.PlayerInv?.MH?.MyItem;
		if (!(item == null))
		{
			MyAnim.SetTrigger("FireBow");
			GameData.CamControl.ShakeScreen(1f, 0.1f);
			WandBolt component = Object.Instantiate(GameData.Misc.ArcheryArrows[_arrowIndex], base.transform.position + Vector3.up + base.transform.forward, base.transform.rotation).GetComponent<WandBolt>();
			if (!_noCheckEffect)
			{
				component.LoadArrow(item.WeaponDmg, _forceProc, _target, myStats.Myself, item.BowArrowSpeed, GameData.DamageType.Physical, item.WandAttackSound);
			}
			else
			{
				component.LoadArrow(item.WeaponDmg, _forceProc, _target, myStats.Myself, item.BowArrowSpeed, GameData.DamageType.Physical, _forceEffectOnTarget: true, item.BowAttackSound);
			}
		}
	}

	public void DoBowAttack(Character _target, Spell _forceProc, int _arrowIndex, bool _noCheckEffect, float _scaleDmg)
	{
		Item item = GameData.PlayerInv?.MH?.MyItem;
		if (!(item == null))
		{
			MyAnim.SetTrigger("FireBow");
			GameData.CamControl.ShakeScreen(1f, 0.1f);
			WandBolt component = Object.Instantiate(GameData.Misc.ArcheryArrows[_arrowIndex], base.transform.position + Vector3.up + base.transform.forward, base.transform.rotation).GetComponent<WandBolt>();
			if (!_noCheckEffect)
			{
				component.LoadArrow(Mathf.RoundToInt((float)item.WeaponDmg * _scaleDmg), _forceProc, _target, myStats.Myself, item.BowArrowSpeed, GameData.DamageType.Physical, item.WandAttackSound);
			}
			else
			{
				component.LoadArrow(Mathf.RoundToInt((float)item.WeaponDmg * _scaleDmg), _forceProc, _target, myStats.Myself, item.BowArrowSpeed, GameData.DamageType.Physical, _forceEffectOnTarget: true, item.BowAttackSound);
			}
		}
	}

	private void HandleDamageResult(Character target, ref int dmg, bool isMainHand)
	{
		bool criticalHit = false;
		if (myStats.isCriticalAttack() && dmg > 0)
		{
			dmg = Mathf.RoundToInt((float)dmg * 1.5f);
			criticalHit = true;
			if (myStats.Myself.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByName("Crippling Blow")) && Random.Range(0, 10) > 8)
			{
				dmg *= 2;
				if (Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 10f)
				{
					UpdateSocialLog.CombatLogAdd("You score a CRIPPLING BLOW!", "lightblue");
				}
			}
			else
			{
				UpdateSocialLog.CombatLogAdd("You score a critical hit!", "lightblue");
			}
			GameData.CamControl.ShakeScreen(0.1f, 3f);
		}
		int num = 0;
		if (myStats.CharacterClass == GameData.ClassDB.Duelist)
		{
			num = ((!isMainHand) ? (myStats?.MyInv?.OH?.MyItem?.WeaponDmg).GetValueOrDefault() : (myStats?.MyInv?.MH?.MyItem?.WeaponDmg).GetValueOrDefault());
			Stats stats = myStats;
			if ((object)stats != null && stats.Level > 20)
			{
				num = Mathf.RoundToInt((float)num * 2.5f);
			}
			else
			{
				Stats stats2 = myStats;
				if ((object)stats2 != null && stats2.Level > 15)
				{
					num = Mathf.RoundToInt((float)num * 2f);
				}
				else
				{
					Stats stats3 = myStats;
					if ((object)stats3 != null && stats3.Level > 10)
					{
						num = Mathf.RoundToInt((float)num * 1.6f);
					}
					else
					{
						Stats stats4 = myStats;
						if ((object)stats4 != null && stats4.Level > 5)
						{
							num = Mathf.RoundToInt((float)num * 1.2f);
						}
					}
				}
			}
		}
		int num2 = target.DamageMe(dmg, _fromPlayer: true, GameData.DamageType.Physical, MyControl.Myself, _animEffect: true, criticalHit, num);
		if (num2 > 0)
		{
			UpdateSocialLog.CombatLogAdd($"You hit {target.name} for {num2} damage!");
			myStats.HealMe(Mathf.RoundToInt((float)num2 * (myStats.PercentLifesteal / 100f)));
			GameData.CamControl.ShakeScreen(0.03f, 3f);
			if (target != null && target.MyStats.GetCurrentDS() > 0)
			{
				myStats.Myself.DamageShieldTaken(target.MyStats.GetCurrentDS(), target.MyStats);
			}
			if (myStats.Myself.MySkills.HasAscension("24943180"))
			{
				int num3 = Mathf.RoundToInt((float)(num2 * myStats.Myself.MySkills.GetAscensionRank("24943180")) * 0.01f);
				myStats.CurrentMana += num3;
				UpdateSocialLog.CombatLogAdd("You pull " + num3 + " mana from your target's soul!", "lightblue");
			}
			return;
		}
		switch (num2)
		{
		case 0:
			UpdateSocialLog.CombatLogAdd("You try to hit " + target.name + ", but miss!");
			break;
		case -1:
			UpdateSocialLog.CombatLogAdd("You try to hit " + target.name + ", but " + target.name + " is INVULNERABLE!");
			break;
		case -2:
			UpdateSocialLog.CombatLogAdd("You try to hit " + target.name + ", but " + target.name + "'s shield absorbs the blow!");
			break;
		case -3:
			UpdateSocialLog.CombatLogAdd("You try to hit " + target.name + ", but you deal no damage!");
			break;
		case -5:
			TryMine(target);
			break;
		case -6:
			target.MyNPC.MyChestEvent.SpawnGuardians(myStats.Level);
			break;
		case -4:
			break;
		}
	}

	public void TryMine(Character target)
	{
		myStats.Myself.MyAudio.PlayOneShot(target.MyHurtSound, GameData.SFXVol * GameData.MasterVol);
		int miningPower = GetMiningPower();
		if (miningPower <= 0)
		{
			UpdateSocialLog.LogAdd("No mining tool in inventory", "yellow");
			return;
		}
		MiningNode component = target.GetComponent<MiningNode>();
		if (component == null)
		{
			return;
		}
		Item item = component.Mine(miningPower);
		if (item != null)
		{
			UpdateSocialLog.LogAdd("You've mined a " + item.ItemName + "!", "green");
			if (!GameData.PlayerInv.AddItemToInv(item))
			{
				GameData.PlayerInv.ForceItemToInv(item);
			}
		}
		else
		{
			UpdateSocialLog.LogAdd("You've struck a mineral deposit...");
		}
	}

	private void TryCoupDeGrace(Character target)
	{
		if (myStats.Myself.MySkills.HasAscension("2806936") && Random.Range(0, 10) < myStats.Myself.MySkills.GetAscensionRank("2806936") && CheckForRange(_isWand: false, _isBow: false, target))
		{
			int num = myStats.CalcMeleeDamage(myStats.MyInv.OHDmg, target.MyStats.Level, target.MyStats, 0);
			string text = CheckTargetInnateAvoidance(target);
			if (text != "")
			{
				UpdateSocialLog.CombatLogAdd("You try to perform a COUP DE GRACE on " + target.name + ", but " + target.name + " " + text, "lightblue");
			}
			else
			{
				UpdateSocialLog.CombatLogAdd("You perform a COUP DE GRACE on " + target.name + "!", "lightblue");
				num *= 2;
				myStats.CheckProc(GameData.PlayerInv.OH, target);
				HandleDamageResult(target, ref num, isMainHand: false);
			}
		}
	}

	private void HandleInvalidTarget()
	{
		if (!(SpamAAWarningCD > 0f))
		{
			SpamAAWarningCD = 60f;
			if (MyControl.CurrentTarget != MyControl.Myself && MyControl.CurrentTarget != null)
			{
				UpdateSocialLog.CombatLogAdd("[autoattack] You can't reach your target from here!");
			}
			else if (MyControl.CurrentTarget != MyControl.Myself && MyControl.CurrentTarget != null)
			{
				UpdateSocialLog.CombatLogAdd("No valid target, switching auto attack off.");
			}
			else
			{
				UpdateSocialLog.CombatLogAdd("You can't hit yourself!");
			}
		}
	}

	public bool CheckTargetInMeleeRange(Character _curTar)
	{
		if (MyMeleeRange.GetNPCsInRange().Contains(_curTar))
		{
			return true;
		}
		return false;
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
		string result = "";
		if ((_target.isNPC && _target.MyNPC.SimPlayer) || !_target.isNPC)
		{
			return "";
		}
		Vector3 forward = _target.transform.forward;
		Vector3 normalized = (base.transform.position - _target.transform.position).normalized;
		if (!(Vector3.Dot(forward, normalized) > 0f))
		{
			return "";
		}
		if (_target.MySkills.KnownSkills.Count > 0)
		{
			if (_target.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByName("Dodge")) && Random.Range(0, 100) < Mathf.RoundToInt((float)_target.MyStats.Level / 2f))
			{
				return " dodged the attack!";
			}
			if (_target.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByName("Block")) && Random.Range(0, 100) < Mathf.RoundToInt((float)_target.MyStats.Level / 2.25f))
			{
				return " blocked the attack!";
			}
		}
		return result;
	}

	private bool CheckForTrueCombat()
	{
		bool flag = true;
		bool flag2 = true;
		bool flag3 = true;
		bool flag4 = true;
		bool flag5 = true;
		if (GameData.GroupMembers[0] == null || GameData.GroupMembers[0].MyAvatar == null || GameData.GroupMembers[0].MyAvatar.MyStats == null || GameData.GroupMembers[0].MyAvatar.MyStats.Myself == null || GameData.GroupMembers[0].MyAvatar.MyStats.Myself.MyNPC == null || GameData.GroupMembers[0].MyAvatar.MyStats.Myself.MyNPC.CurrentAggroTarget == null || !GameData.GroupMembers[0].MyAvatar.MyStats.Myself.Alive)
		{
			flag = false;
			NPC nPC = GameData.GroupMembers[0]?.MyAvatar?.MyStats?.Myself?.MyNPC;
			if (nPC != null && GameData.GroupMatesInCombat.Contains(nPC))
			{
				GameData.GroupMatesInCombat.Remove(nPC);
			}
		}
		if (GameData.GroupMembers[1] == null || GameData.GroupMembers[1].MyAvatar == null || GameData.GroupMembers[1].MyAvatar.MyStats == null || GameData.GroupMembers[1].MyAvatar.MyStats.Myself == null || GameData.GroupMembers[1].MyAvatar.MyStats.Myself.MyNPC == null || GameData.GroupMembers[1].MyAvatar.MyStats.Myself.MyNPC.CurrentAggroTarget == null || !GameData.GroupMembers[1].MyAvatar.MyStats.Myself.Alive)
		{
			flag2 = false;
			NPC nPC2 = GameData.GroupMembers[1]?.MyAvatar?.MyStats?.Myself?.MyNPC;
			if (nPC2 != null && GameData.GroupMatesInCombat.Contains(nPC2))
			{
				GameData.GroupMatesInCombat.Remove(nPC2);
			}
		}
		if (GameData.GroupMembers[2] == null || GameData.GroupMembers[2].MyAvatar == null || GameData.GroupMembers[2].MyAvatar.MyStats == null || GameData.GroupMembers[2].MyAvatar.MyStats.Myself == null || GameData.GroupMembers[2].MyAvatar.MyStats.Myself.MyNPC == null || GameData.GroupMembers[2].MyAvatar.MyStats.Myself.MyNPC.CurrentAggroTarget == null || !GameData.GroupMembers[2].MyAvatar.MyStats.Myself.Alive)
		{
			flag3 = false;
			NPC nPC3 = GameData.GroupMembers[2]?.MyAvatar?.MyStats?.Myself?.MyNPC;
			if (nPC3 != null && GameData.GroupMatesInCombat.Contains(nPC3))
			{
				GameData.GroupMatesInCombat.Remove(nPC3);
			}
		}
		if (GameData.GroupMembers[3] == null || GameData.GroupMembers[3].MyAvatar == null || GameData.GroupMembers[3].MyAvatar.MyStats == null || GameData.GroupMembers[3].MyAvatar.MyStats.Myself == null || GameData.GroupMembers[3].MyAvatar.MyStats.Myself.MyNPC == null || GameData.GroupMembers[3].MyAvatar.MyStats.Myself.MyNPC.CurrentAggroTarget == null || !GameData.GroupMembers[3].MyAvatar.MyStats.Myself.Alive)
		{
			flag4 = false;
			NPC nPC4 = GameData.GroupMembers[3]?.MyAvatar?.MyStats?.Myself?.MyNPC;
			if (nPC4 != null && GameData.GroupMatesInCombat.Contains(nPC4))
			{
				GameData.GroupMatesInCombat.Remove(nPC4);
			}
		}
		if (!flag && !flag2 && !flag3 && !flag4)
		{
			GameData.GroupMatesInCombat.Clear();
		}
		if (GameData.PlayerControl.Myself.UnderThreat <= 0f && !GameData.Autoattacking)
		{
			flag5 = false;
		}
		if (!flag5 && !flag && !flag2 && !flag3 && !flag4)
		{
			return false;
		}
		return true;
	}

	public int GetMiningPower()
	{
		foreach (ItemIcon storedSlot in GameData.PlayerInv.StoredSlots)
		{
			if (storedSlot.MyItem.Mining > 0)
			{
				return storedSlot.MyItem.Mining;
			}
		}
		if (GameData.PlayerInv.MH.MyItem.Mining > 0)
		{
			return GameData.PlayerInv.MH.MyItem.Mining;
		}
		if (GameData.PlayerInv.OH.MyItem.Mining > 0)
		{
			return GameData.PlayerInv.OH.MyItem.Mining;
		}
		return 0;
	}
}
