// UseSkill
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UseSkill : MonoBehaviour
{
	public List<Skill> KnownSkills;

	private PlayerCombat PC;

	public bool isPlayer;

	private Animator MyAnim;

	private Stats MyStats;

	private bool passCheck;

	public Fishing MyFishing;

	public ParticleSystem LearnSkill;

	[SerializeField]
	public List<AscensionSkillEntry> MyAscensions;

	public int AscensionPoints;

	private void Start()
	{
		if (base.transform.name == "Player")
		{
			PC = GetComponent<PlayerCombat>();
			isPlayer = true;
		}
		else
		{
			isPlayer = false;
		}
		MyStats = GetComponent<Stats>();
		MyAnim = GetComponent<Animator>();
		if (isPlayer)
		{
			MyFishing = GetComponent<Fishing>();
		}
		for (int num = MyAscensions.Count - 1; num >= 0; num--)
		{
			if (MyAscensions[num].id == "empty")
			{
				MyAscensions.RemoveAt(num);
			}
		}
	}

	public bool DoSkill(Skill _skill, Character _target)
	{
		bool flag = false;
		bool result = false;
		float num = 1f;
		bool criticalHit = false;
		num = (float)MyStats.Level / 28f;
		if (num > 1f)
		{
			num = 1f;
		}
		if (num < 0.4f)
		{
			num = 0.4f;
		}
		if (GameData.PlayerStats.Myself.Alive)
		{
			if (GameData.PlayerStats.Invisible)
			{
				GameData.PlayerStats.BreakEffectsOnAction();
			}
			passCheck = true;
			if (_skill.RequireShield && !MyStats.MyInv.SecondaryShield)
			{
				passCheck = false;
				if (isPlayer)
				{
					UpdateSocialLog.LogAdd("This skill requires a shield to be equipped!", "yellow");
				}
			}
			if (_skill.Require2H && !MyStats.MyInv.TwoHandPrimary)
			{
				passCheck = false;
				if (isPlayer)
				{
					UpdateSocialLog.LogAdd("This skill requires a two-handed weapon to be equipped!", "yellow");
				}
			}
			if (_skill.RequireBow && !MyStats.MyInv.PrimaryBow)
			{
				passCheck = false;
				if (isPlayer)
				{
					UpdateSocialLog.LogAdd("This skill requires a bow to be equipped!", "yellow");
				}
			}
			if (MyStats.Stunned || MyStats.Feared)
			{
				passCheck = false;
				if (isPlayer)
				{
					UpdateSocialLog.LogAdd("Can't do this while stunned...", "yellow");
				}
			}
			if ((_skill.RequireBehind && PC != null && _target != null && !PC.CheckIfBehind(_target)) || (MyStats.Myself.MyNPC != null && _target != null && !MyStats.Myself.MyNPC.CheckIfBehind(_target)))
			{
				passCheck = false;
				if (isPlayer)
				{
					UpdateSocialLog.LogAdd("You must be behind your target!", "yellow");
				}
			}
			if (MyStats.Myself != null && _target != null && _target == MyStats.Myself && _skill.TypeOfSkill == Skill.SkillType.Attack)
			{
				passCheck = false;
				if (isPlayer)
				{
					UpdateSocialLog.LogAdd("You can't hit yourself with this skill!", "yellow");
				}
			}
			if (_skill.Id == "10823930")
			{
				if (!(MyStats?.Myself?.MySpells?.isCasting()).GetValueOrDefault())
				{
					passCheck = false;
					if (isPlayer)
					{
						UpdateSocialLog.LogAdd("You must be channeling a spell to use this skill", "yellow");
					}
				}
				else
				{
					Spell currentCast = MyStats.Myself.MySpells.GetCurrentCast();
					if ((object)currentCast == null || currentCast.Type != 0)
					{
						Spell currentCast2 = MyStats.Myself.MySpells.GetCurrentCast();
						if ((object)currentCast2 == null || currentCast2.Type != Spell.SpellType.AE)
						{
							Spell currentCast3 = MyStats.Myself.MySpells.GetCurrentCast();
							if ((object)currentCast3 == null || currentCast3.Type != Spell.SpellType.PBAE)
							{
								passCheck = false;
								if (isPlayer)
								{
									UpdateSocialLog.LogAdd("This skill only works on DAMAGE spells", "yellow");
								}
							}
						}
					}
				}
			}
			if (passCheck)
			{
				if (isPlayer && _skill.AutomateAttack && GameData.GM.AutoEngageAttackOnSkill)
				{
					GameData.PlayerCombat.ForceAttackOn();
				}
				int num2 = 0;
				if (_skill.TypeOfSkill == Skill.SkillType.Attack)
				{
					if (!_skill.AESkill)
					{
						if (_target != null)
						{
							if (!_skill.ScaleOffWeapon && _skill.PercentDmg == 0f)
							{
								num2 = Mathf.RoundToInt(Random.Range(5f, 20f) / 20f * (float)_skill.SkillPower * (float)MyStats.Level);
							}
							else if (_skill.PercentDmg != 0f)
							{
								int num3 = Mathf.RoundToInt((float)(Mathf.RoundToInt(MyStats.MyInv.MHDmg / 2) + Mathf.RoundToInt(Mathf.Max(MyStats.MyInv.MHLevel / 4, 6))) + (float)Mathf.RoundToInt(Mathf.Max((float)MyStats.Level / 4f, 6f)) * num);
								_ = _target.transform.name == "Training Dummy";
								float scalingBonus = GetScalingBonus(MyStats.Level);
								num2 = Mathf.RoundToInt((float)(num3 * num3 * Random.Range(Mathf.RoundToInt((float)MyStats.Level / 2f), MyStats.Level)) * (Random.Range(4f, 22f) / 20f));
								num2 = Mathf.RoundToInt((float)num2 * scalingBonus);
								float maxInclusive = Random.Range(17, 21);
								num2 = Mathf.RoundToInt((float)num2 * Random.Range(10f, maxInclusive) / 20f);
								GameData.CamControl.ShakeScreen(2f, 0.2f);
								if (_skill.Id == "63114341" && HasAscension("31074428") && Random.Range(0, 8) < GetAscensionRank("31074428"))
								{
									num2 = Mathf.RoundToInt((float)num2 * 1.5f);
									UpdateSocialLog.CombatLogAdd("Critical BACKSTAB!");
								}
								if (HasAscension("3637371") && Random.Range(0, 8) < GetAscensionRank("3637371"))
								{
									flag = true;
								}
							}
							else if (MyStats.Myself.isNPC)
							{
								num2 = MyStats.CalcMeleeDamage(MyStats.Myself.MyNPC.BaseAtkDmg, _target.MyStats.Level, _target.MyStats, 0);
							}
							else if (MyStats.MyInv.MH.MyItem != null && MyStats.MyInv.MH.MyItem.WeaponDmg > 0)
							{
								num2 = MyStats.CalcMeleeDamage(MyStats.MyInv.MH.MyItem.WeaponDmg, _target.MyStats.Level, _target.MyStats, 0);
								if (num2 == 0)
								{
									num2 = MyStats.MyInv.MH.MyItem.WeaponDmg * Mathf.Max(_skill.SkillPower, 1);
								}
								if (MyStats.MyInv.TwoHandPrimary)
								{
									num2 *= 2;
								}
							}
							else
							{
								UpdateSocialLog.LogAdd("This skill requires a weapon to be equipped!", "yellow");
							}
						}
						if (isPlayer)
						{
							if (_target != null && num2 > 0)
							{
								if (PC.CheckTargetInMeleeRange(_target))
								{
									float num4 = MyStats?.CombatStance?.AggroGenMod ?? 1f;
									if (MyStats.CurrentHP < MyStats.CurrentMaxHP / 2)
									{
										num4 *= 1.25f;
									}
									if (MyStats.CurrentHP < MyStats.CurrentMaxHP / 4)
									{
										num4 *= 1.75f;
									}
									result = true;
									if (_skill.SkillAnimName != "" && _skill.SkillAnimName != null)
									{
										MyAnim.SetTrigger(_skill.SkillAnimName);
									}
									int num5 = _target.DamageMe(num2, _fromPlayer: true, _skill.DmgType, GetComponent<Character>(), _animEffect: true, criticalHit);
									if (_target.MyNPC != null && !_target.MyNPC.SimPlayer)
									{
										_target.MyNPC.ManageAggro(Mathf.RoundToInt((float)num2 * num4), MyStats.Myself);
									}
									if (num5 > 0)
									{
										UpdateSocialLog.CombatLogAdd(_skill.PlayerUses + " " + _target.transform.name + " for " + num5 + " damage.");
										GameData.CamControl.ShakeScreen(1f, 0.2f);
										if (_skill.EffectToApply != null)
										{
											_target.MyStats.AddStatusEffect(_skill.EffectToApply, isPlayer, 0, MyStats.Myself);
										}
										if (_skill.Interrupt)
										{
											_target.transform.position += new Vector3(0.2f, 0f, 0.2f);
											InterruptTargetSpell(_target);
										}
										TryProc(_skill, _target);
									}
									else
									{
										switch (num5)
										{
										case 0:
											UpdateSocialLog.CombatLogAdd("You try to use " + _skill.SkillName + ", but miss!");
											break;
										case -1:
											UpdateSocialLog.CombatLogAdd("You try to use " + _skill.SkillName + ", but " + _target.transform.name + " is INVULNERABLE!");
											break;
										}
									}
								}
								else
								{
									UpdateSocialLog.CombatLogAdd("Target out of range!");
								}
							}
							else
							{
								UpdateSocialLog.CombatLogAdd("No target selected.");
							}
						}
						else
						{
							TryProc(_skill, _target);
							string text = ((!(_target.transform.tag == "Player")) ? _target.transform.name : "YOU");
							UpdateSocialLog.CombatLogAdd(base.transform.name + " " + _skill.NPCUses + " " + text + " for " + num2 + " damage!");
							MyAnim.SetTrigger(_skill.SkillAnimName);
							result = true;
							result = true;
						}
						if (flag)
						{
							DoAnotherBackstab(_skill, _target);
						}
					}
					else
					{
						List<Character> list = new List<Character>();
						if (GameData.PlayerControl.Myself == MyStats.Myself)
						{
							foreach (Character item in GameData.PlayerCombat.MyMeleeRange.GetNPCsInRange())
							{
								if (item.Alive && item != MyStats.Myself)
								{
									list.Add(item);
								}
							}
						}
						else if (MyStats.Myself != null && MyStats.Myself.NearbyEnemies != null && MyStats.Myself.NearbyEnemies.Count > 0)
						{
							foreach (Character nearbyEnemy in MyStats.Myself.NearbyEnemies)
							{
								if (nearbyEnemy.Alive && nearbyEnemy != MyStats.Myself)
								{
									list.Add(nearbyEnemy);
								}
							}
						}
						if (list.Count <= 0)
						{
							UpdateSocialLog.LogAdd("No valid targets in range", "yellow");
							return false;
						}
						foreach (Character item2 in list)
						{
							if (item2 != null)
							{
								if (!_skill.ScaleOffWeapon && _skill.PercentDmg == 0f)
								{
									num2 = Mathf.RoundToInt(Random.Range(5f, 20f) / 20f * (float)_skill.SkillPower * (float)MyStats.Level);
								}
								else if (_skill.PercentDmg != 0f)
								{
									int num6 = Mathf.RoundToInt((float)(Mathf.RoundToInt((float)MyStats.MyInv.MHDmg / 1.5f) + Mathf.RoundToInt(Mathf.Max(MyStats.MyInv.MHLevel / 4, 6))) + (float)Mathf.RoundToInt(Mathf.Max((float)MyStats.Level / 4f, 6f)) * num);
									_ = item2.transform.name == "Training Dummy";
									float scalingBonus2 = GetScalingBonus(MyStats.Level);
									num2 = Mathf.RoundToInt((float)(num6 * num6 * Random.Range(Mathf.RoundToInt((float)MyStats.Level / 2f), MyStats.Level)) * (Random.Range(4f, 22f) / 20f));
									num2 = Mathf.RoundToInt((float)num2 * scalingBonus2);
									float maxInclusive2 = Random.Range(17, 21);
									num2 = Mathf.RoundToInt((float)num2 * Random.Range(10f, maxInclusive2) / 20f);
									GameData.CamControl.ShakeScreen(1f, 0.2f);
								}
								else if (MyStats.Myself.isNPC)
								{
									num2 = MyStats.CalcMeleeDamage(MyStats.Myself.MyNPC.BaseAtkDmg, item2.MyStats.Level, item2.MyStats, 0);
								}
								else if (MyStats.MyInv.MH.MyItem != null && MyStats.MyInv.MH.MyItem.WeaponDmg > 0)
								{
									num2 = MyStats.CalcMeleeDamage(MyStats.MyInv.MH.MyItem.WeaponDmg, item2.MyStats.Level, item2.MyStats, 0);
								}
								else
								{
									UpdateSocialLog.LogAdd("This skill requires a weapon to be equipped!", "yellow");
								}
							}
							if (isPlayer)
							{
								if (item2 != null && num2 > 0)
								{
									if (PC.CheckTargetInMeleeRange(item2))
									{
										result = true;
										if (_skill.SkillAnimName != "" && _skill.SkillAnimName != null)
										{
											MyAnim.SetTrigger(_skill.SkillAnimName);
										}
										int num7 = item2.DamageMe(num2, _fromPlayer: true, _skill.DmgType, GetComponent<Character>(), _animEffect: true, criticalHit);
										if (num7 > 0)
										{
											UpdateSocialLog.CombatLogAdd(_skill.PlayerUses + " " + item2.transform.name + " for " + num7 + " damage.");
											if (_skill.EffectToApply != null)
											{
												item2.MyStats.AddStatusEffect(_skill.EffectToApply, isPlayer, 0, MyStats.Myself);
											}
											if (_skill.Interrupt)
											{
												item2.transform.position += new Vector3(0.2f, 0f, 0.2f);
												InterruptTargetSpell(item2);
											}
											GameData.CamControl.ShakeScreen(1f, 0.2f);
											TryProc(_skill, item2);
										}
										else
										{
											switch (num7)
											{
											case 0:
												UpdateSocialLog.CombatLogAdd("You try to use " + _skill.SkillName + ", but miss!");
												break;
											case -1:
												UpdateSocialLog.CombatLogAdd("You try to use " + _skill.SkillName + ", but " + item2.transform.name + " is INVULNERABLE!");
												break;
											}
										}
									}
									else
									{
										UpdateSocialLog.CombatLogAdd("Target out of range!");
									}
								}
								else
								{
									UpdateSocialLog.CombatLogAdd("No target selected.");
								}
							}
							else
							{
								TryProc(_skill, item2);
								string text2 = ((!(item2.transform.tag == "Player")) ? item2.transform.name : "YOU");
								UpdateSocialLog.CombatLogAdd(base.transform.name + " " + _skill.NPCUses + " " + text2 + " for " + num2 + " damage!");
								MyAnim.SetTrigger(_skill.SkillAnimName);
								result = true;
								result = true;
							}
						}
					}
				}
				if (_skill.TypeOfSkill == Skill.SkillType.Utility)
				{
					if (_skill.EffectToApply != null)
					{
						result = true;
						MyAnim.SetTrigger("SkillBuff");
						MyStats.AddStatusEffect(_skill.EffectToApply, isPlayer, 0, MyStats.Myself);
						Object.Instantiate(GameData.EffectDB.SpellEffects[_skill.EffectToApply.SpellResolveFXIndex], base.transform.position, base.transform.rotation);
						if (isPlayer)
						{
							UpdateSocialLog.LogAdd("You " + _skill.EffectToApply.StatusEffectMessageOnPlayer, "lightblue");
						}
					}
					if (_skill.StanceToUse != null)
					{
						if (_skill.SkillAnimName != "" && _skill.SkillAnimName != null)
						{
							MyAnim.SetTrigger(_skill.SkillAnimName);
						}
						MyStats.ChangeStance(_skill.StanceToUse);
					}
				}
				if (_skill.TypeOfSkill == Skill.SkillType.Other && _skill.Id == "10823930" && (MyStats?.Myself?.MySpells?.isCasting()).GetValueOrDefault())
				{
					result = MyStats.Myself.MySpells.ControlledChant();
				}
				if (_skill.TypeOfSkill == Skill.SkillType.Ranged)
				{
					if (GameData.PlayerInv.PrimaryBow && _skill.RequireBow && _target != null)
					{
						if (Vector3.Distance(base.transform.position, _target.transform.position) >= 25f)
						{
							UpdateSocialLog.LogAdd("You must be closer to use this ability", "yellow");
							return false;
						}
						if (!isPlayer)
						{
							return false;
						}
						MyAnim.SetTrigger(_skill.SkillAnimName);
						Spell spell = null;
						switch (_skill.Id)
						{
						case "58018670":
							if (MyStats.Myself.MySpells.isCasting())
							{
								spell = MyStats.Myself.MySpells.GetCurrentCast();
								UpdateSocialLog.LogAdd("Your arrow is imbued with the " + spell.SpellName + " spell!", "lightblue");
								if (GetAscensionRank("63904469") * 15 > Random.Range(0, 100))
								{
									MyStats.Myself.MySpells.CompleteCastEarly();
								}
								else
								{
									MyStats.Myself.MySpells.EndCastWithPenalty();
								}
							}
							GameData.PlayerCombat.DoBowAttack(_target, spell, 0, _noCheckEffect: true, 0.25f);
							break;
						case "2744280":
							spell = _skill.CastOnTarget;
							UpdateSocialLog.LogAdd("You fire a flaming arrow!", "lightblue");
							GameData.PlayerCombat.DoBowAttack(_target, spell, 1, _noCheckEffect: false);
							break;
						case "45445146":
							spell = _skill.CastOnTarget;
							UpdateSocialLog.LogAdd("You fire a Skycall arrow!", "lightblue");
							GameData.PlayerCombat.DoBowAttack(_target, spell, 2, _noCheckEffect: false);
							break;
						case "24603690":
							spell = _skill.CastOnTarget;
							UpdateSocialLog.LogAdd("You fire a razor-tipped arrow!", "lightblue");
							GameData.PlayerCombat.DoBowAttack(_target, spell, 0, _noCheckEffect: false);
							break;
						case "1068264":
							spell = _skill.CastOnTarget;
							UpdateSocialLog.LogAdd("You fire an envenomed arrow!", "lightblue");
							GameData.PlayerCombat.DoBowAttack(_target, spell, 0, _noCheckEffect: false);
							break;
						case "48749016":
							spell = _skill.CastOnTarget;
							UpdateSocialLog.LogAdd("You fire an blunted arrow!", "lightblue");
							GameData.PlayerCombat.DoBowAttack(_target, 0, _interrupt: true, spell);
							break;
						case "26241651":
							spell = _skill.CastOnTarget;
							UpdateSocialLog.LogAdd("You fire a lethal arrow!", "lightblue");
							if ((float)_target.MyStats.CurrentHP <= (float)_target.MyStats.CurrentMaxHP * 0.1f)
							{
								GameData.PlayerCombat.DoBowAttack(_target, 8, 0);
							}
							else
							{
								GameData.PlayerCombat.DoBowAttack(_target, 0);
							}
							break;
						default:
							return false;
						}
						return true;
					}
					if (_skill.RequireBow && !GameData.PlayerInv.PrimaryBow)
					{
						UpdateSocialLog.LogAdd("This skill requires a bow in the main hand", "yellow");
						return false;
					}
				}
			}
		}
		else if (isPlayer)
		{
			UpdateSocialLog.CombatLogAdd("Can't use skills while dead!");
		}
		return result;
	}

	public bool DoSkillNoChecks(Skill _skill, Character _target)
	{
		bool flag = false;
		bool result = false;
		float num = 1f;
		bool criticalHit = false;
		num = (float)MyStats.Level / 28f;
		if (num > 1f)
		{
			num = 1f;
		}
		if (num < 0.4f)
		{
			num = 0.4f;
		}
		if (MyStats.Myself.Alive)
		{
			passCheck = true;
			if (passCheck)
			{
				int num2 = 0;
				if (!_skill.AESkill)
				{
					if (_skill.TypeOfSkill == Skill.SkillType.Attack)
					{
						if (_target != null)
						{
							if (!_skill.ScaleOffWeapon && _skill.PercentDmg == 0f)
							{
								num2 = Mathf.RoundToInt(Random.Range(5f, 20f) / 20f * (float)_skill.SkillPower * (float)MyStats.Level);
							}
							else if (_skill.PercentDmg != 0f)
							{
								int num3 = Mathf.RoundToInt((float)(Mathf.RoundToInt(MyStats.MyInv.MHDmg / 2) + Mathf.RoundToInt(Mathf.Max(MyStats.MyInv.MHLevel / 4, 6))) + (float)Mathf.RoundToInt(Mathf.Max((float)MyStats.Level / 4f, 6f)) * num);
								_ = _target.transform.name == "Training Dummy";
								float scalingBonus = GetScalingBonus(MyStats.Level);
								num2 = Mathf.RoundToInt((float)(num3 * num3 * Random.Range(Mathf.RoundToInt((float)MyStats.Level / 2f), MyStats.Level)) * (Random.Range(8f, 13f) / 20f));
								num2 = Mathf.RoundToInt((float)num2 * scalingBonus);
								float maxInclusive = Random.Range(17, 21);
								num2 = Mathf.RoundToInt((float)num2 * Random.Range(10f, maxInclusive) / 20f);
								if (!isPlayer && Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 15f)
								{
									UpdateSocialLog.CombatLogAdd(MyStats.transform.name + " BACKSTABS " + _target.transform.name + "!", "lightblue");
								}
								if (_skill.Id == "63114341" && HasAscension("31074428") && Random.Range(0, 8) < GetAscensionRank("31074428"))
								{
									num2 = Mathf.RoundToInt((float)num2 * 1.5f);
									UpdateSocialLog.CombatLogAdd("Critical BACKSTAB!");
								}
								if (HasAscension("3637371") && Random.Range(0, 8) < GetAscensionRank("3637371"))
								{
									flag = true;
								}
							}
							else
							{
								num2 = MyStats.CalcMeleeDamage(MyStats.Level, _target.MyStats.Level, _target.MyStats, 0) * _skill.SkillPower;
							}
						}
						if (isPlayer)
						{
							if (_target != null && PC.CheckTargetInMeleeRange(_target))
							{
								result = true;
								if (_skill.SkillAnimName != "" && _skill.SkillAnimName != null)
								{
									MyAnim.SetTrigger(_skill.SkillAnimName);
								}
								if (_target.DamageMe(num2, _fromPlayer: true, _skill.DmgType, GetComponent<Character>(), _animEffect: true, criticalHit) > 0)
								{
									if (_skill.EffectToApply != null)
									{
										_target.MyStats.AddStatusEffect(_skill.EffectToApply, isPlayer, 0, MyStats.Myself);
									}
									TryProc(_skill, _target);
								}
							}
						}
						else
						{
							TryProc(_skill, _target);
							if (!(_target.transform.tag == "Player"))
							{
								_ = _target.transform.name;
							}
							bool fromPlayer = false;
							if (MyStats.Myself.MyNPC != null && MyStats.Myself.MyNPC.SimPlayer && GameData.SimMngr.Sims[MyStats.Myself.MyNPC.ThisSim.myIndex].Grouped)
							{
								fromPlayer = true;
							}
							int num4 = _target.DamageMe(num2, fromPlayer, _skill.DmgType, GetComponent<Character>(), _animEffect: true, criticalHit);
							MyAnim.SetTrigger(_skill.SkillAnimName);
							if (num4 > 0 && _skill.Interrupt)
							{
								_target.transform.position += new Vector3(0.2f, 0f, 0.2f);
								InterruptTargetSpell(_target);
							}
							result = true;
						}
						if (flag)
						{
							DoAnotherBackstab(_skill, _target);
						}
					}
				}
				else if (_skill.AESkill)
				{
					List<Character> list = new List<Character>();
					if (MyStats.Myself != null && MyStats.Myself.NearbyEnemies != null && MyStats.Myself.NearbyEnemies.Count > 0)
					{
						foreach (Character nearbyEnemy in MyStats.Myself.NearbyEnemies)
						{
							if (nearbyEnemy.Alive && nearbyEnemy != MyStats.Myself)
							{
								list.Add(nearbyEnemy);
							}
						}
					}
					if (list.Count <= 0)
					{
						return false;
					}
					if (MyStats.Myself.MyNPC != null && MyStats.Myself.MyNPC.SimPlayer && GameData.SimMngr.Sims[MyStats.Myself.MyNPC.ThisSim.myIndex].Grouped && (GameData.SimPlayerGrouping.CC.Count > 0 || GameData.SimPlayerGrouping.PlayerIsCC))
					{
						return false;
					}
					foreach (Character item in list)
					{
						if (item != null)
						{
							if (!_skill.ScaleOffWeapon && _skill.PercentDmg == 0f)
							{
								num2 = Mathf.RoundToInt(Random.Range(5f, 20f) / 20f * (float)_skill.SkillPower * (float)MyStats.Level);
							}
							else if (_skill.PercentDmg != 0f)
							{
								int num5 = Mathf.RoundToInt((float)(Mathf.RoundToInt(MyStats.MyInv.MHDmg / 2) + Mathf.RoundToInt(Mathf.Max(MyStats.MyInv.MHLevel / 4, 6))) + (float)Mathf.RoundToInt(Mathf.Max((float)MyStats.Level / 4f, 6f)) * num);
								_ = item.transform.name == "Training Dummy";
								float scalingBonus2 = GetScalingBonus(MyStats.Level);
								num2 = Mathf.RoundToInt((float)(num5 * num5 * Random.Range(Mathf.RoundToInt((float)MyStats.Level / 2f), MyStats.Level)) * (Random.Range(4f, 22f) / 20f));
								num2 = Mathf.RoundToInt((float)num2 * scalingBonus2);
								float maxInclusive2 = Random.Range(17, 21);
								num2 = Mathf.RoundToInt((float)num2 * Random.Range(10f, maxInclusive2) / 20f);
								GameData.CamControl.ShakeScreen(1f, 0.2f);
							}
							else
							{
								if (_skill.Require2H && !isPlayer && MyStats.MyInv.SimMH != null && MyStats.MyInv.SimMH.MyItem != null && MyStats.MyInv.SimMH.MyItem.ThisWeaponType != Item.WeaponType.TwoHandMelee && MyStats.MyInv.SimMH.MyItem.ThisWeaponType != Item.WeaponType.TwoHandStaff)
								{
									return false;
								}
								if (MyStats.Myself.isNPC)
								{
									num2 = MyStats.CalcMeleeDamage(MyStats.Myself.MyNPC.BaseAtkDmg, item.MyStats.Level, item.MyStats, 0);
								}
								else if (MyStats.MyInv.MH.MyItem != null && MyStats.MyInv.MH.MyItem.WeaponDmg > 0)
								{
									num2 = MyStats.CalcMeleeDamage(MyStats.MyInv.MH.MyItem.WeaponDmg, item.MyStats.Level, item.MyStats, 0);
								}
								else
								{
									UpdateSocialLog.LogAdd("This skill requires a weapon to be equipped!", "yellow");
								}
							}
						}
						if (item != null)
						{
							TryProc(_skill, item);
							string text = ((!(item.transform.tag == "Player")) ? item.transform.name : "YOU");
							if (text == "YOU")
							{
								UpdateSocialLog.CombatLogAdd(base.transform.name + " " + _skill.NPCUses + " " + text + " for " + num2 + " damage!");
							}
							MyAnim.SetTrigger(_skill.SkillAnimName);
							result = true;
							result = true;
						}
					}
				}
				if (_skill.TypeOfSkill == Skill.SkillType.Utility && _skill.EffectToApply != null)
				{
					result = true;
					MyAnim.SetTrigger("SkillBuff");
					MyStats.AddStatusEffect(_skill.EffectToApply, isPlayer, 0, MyStats.Myself);
					Object.Instantiate(GameData.EffectDB.SpellEffects[_skill.EffectToApply.SpellResolveFXIndex], base.transform.position, base.transform.rotation);
					if (isPlayer)
					{
						UpdateSocialLog.LogAdd("You " + _skill.EffectToApply.StatusEffectMessageOnPlayer, "lightblue");
					}
				}
				if (_skill.TypeOfSkill == Skill.SkillType.Ranged)
				{
					Inventory myInv = MyStats.MyInv;
					if ((object)myInv != null && myInv.PrimaryBow)
					{
						if (Vector3.Distance(base.transform.position, _target.transform.position) >= 31f)
						{
							return false;
						}
						MyAnim.SetTrigger(_skill.SkillAnimName);
						Spell spell = null;
						switch (_skill.Id)
						{
						case "2744280":
							spell = _skill.CastOnTarget;
							MyStats.Myself.MyNPC.DoBowAttack(_target, spell, 1, _noCheckEffect: false);
							break;
						case "45445146":
							if (!(MyStats?.Myself?.MyNPC?.SimPlayer).GetValueOrDefault() || !GameData.GroupMembers.Contains(GameData.SimMngr.Sims[MyStats.Myself.MyNPC.ThisSim.myIndex]) || GameData.SimPlayerGrouping.CC.Contains(GameData.SimMngr.Sims[MyStats.Myself.MyNPC.ThisSim.myIndex]))
							{
								spell = _skill.CastOnTarget;
								MyStats.Myself.MyNPC.DoBowAttack(_target, spell, 2, _noCheckEffect: false);
							}
							break;
						case "26241651":
							if (_target != null && (float)_target.MyStats.CurrentHP < (float)_target.MyStats.CurrentMaxHP * 0.1f)
							{
								int dmgMod = ((!((float)_target.MyStats.CurrentHP < (float)_target.MyStats.CurrentMaxHP * 0.1f)) ? 1 : 8);
								MyStats.Myself.MyNPC.DoBowAttack(_target, dmgMod, 0);
								break;
							}
							return false;
						case "48749016":
							spell = _skill.CastOnTarget;
							MyStats.Myself.MyNPC.DoBowAttack(_target, 0, _interrupt: true, spell);
							break;
						case "24603690":
							spell = _skill.CastOnTarget;
							MyStats.Myself.MyNPC.DoBowAttack(_target, spell, 0, _noCheckEffect: false);
							break;
						case "58018670":
							if (MyStats.Myself.MySpells.isCasting() && MyStats.Myself.MySpells.GetCurrentCast().Type == Spell.SpellType.Damage)
							{
								spell = MyStats.Myself.MySpells.GetCurrentCast();
								MyStats.Myself.MyNPC.DoBowAttack(_target, spell, 0, _noCheckEffect: true, MyStats.DexScaleMod / 100);
								if (Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 15f)
								{
									UpdateSocialLog.CombatLogAdd(base.transform.name + " imbues an arrow with " + spell.SpellName);
								}
								if (GetAscensionRank("63904469") * 15 > Random.Range(0, 100))
								{
									MyStats.Myself.MySpells.CompleteCastEarly();
								}
								else
								{
									MyStats.Myself.MySpells.EndCastWithPenalty();
								}
								break;
							}
							return false;
						case "1068264":
							spell = _skill.CastOnTarget;
							MyStats.Myself.MyNPC.DoBowAttack(_target, spell, 0, _noCheckEffect: true);
							break;
						}
						return true;
					}
				}
			}
		}
		else if (isPlayer)
		{
			UpdateSocialLog.CombatLogAdd("Can't use skills while dead!");
		}
		return result;
	}

	public bool HasAscension(string _id)
	{
		if (MyAscensions.Count == 0)
		{
			return false;
		}
		foreach (AscensionSkillEntry myAscension in MyAscensions)
		{
			if (myAscension.id == _id)
			{
				return true;
			}
		}
		return false;
	}

	public int GetAscensionRank(string _id)
	{
		if (MyAscensions.Count == 0)
		{
			return 0;
		}
		foreach (AscensionSkillEntry myAscension in MyAscensions)
		{
			if (myAscension.id == _id)
			{
				return myAscension.level;
			}
		}
		return 0;
	}

	public bool AddAscension(string _id)
	{
		if (MyAscensions.Count > 0)
		{
			foreach (AscensionSkillEntry myAscension in MyAscensions)
			{
				if (myAscension.id == _id)
				{
					return false;
				}
			}
		}
		MyAscensions.Add(new AscensionSkillEntry(_id, 1));
		return true;
	}

	public void LevelUpAscension(string _id)
	{
		if (MyAscensions.Count <= 0)
		{
			return;
		}
		foreach (AscensionSkillEntry myAscension in MyAscensions)
		{
			if (myAscension.id == _id && myAscension.level < GameData.SkillDatabase.GetAscensionByID(_id).MaxRank)
			{
				myAscension.level++;
				break;
			}
		}
	}

	public int GetPointsSpent()
	{
		int num = 0;
		foreach (AscensionSkillEntry myAscension in MyAscensions)
		{
			num += myAscension.level;
		}
		return num;
	}

	private float GetScalingBonus(int level)
	{
		level = Mathf.Clamp(level, 6, 30);
		float t = ((float)level - 6f) / 24f;
		return Mathf.Lerp(0.5f, 1f, t);
	}

	private void DoAnotherBackstab(Skill _skill, Character _target)
	{
		float num = 1f;
		bool criticalHit = false;
		num = (float)MyStats.Level / 28f;
		if (num > 1f)
		{
			num = 1f;
		}
		if (num < 0.4f)
		{
			num = 0.4f;
		}
		int num2 = 0;
		if (_target != null)
		{
			if (!_skill.ScaleOffWeapon && _skill.PercentDmg == 0f)
			{
				num2 = Mathf.RoundToInt(Random.Range(5f, 20f) / 20f * (float)_skill.SkillPower * (float)MyStats.Level);
			}
			else if (_skill.PercentDmg != 0f)
			{
				int num3 = Mathf.RoundToInt((float)(Mathf.RoundToInt(MyStats.MyInv.MHDmg / 2) + Mathf.RoundToInt(Mathf.Max(MyStats.MyInv.MHLevel / 4, 6))) + (float)Mathf.RoundToInt(Mathf.Max((float)MyStats.Level / 4f, 6f)) * num);
				_ = _target.transform.name == "Training Dummy";
				float scalingBonus = GetScalingBonus(MyStats.Level);
				num2 = Mathf.RoundToInt((float)(num3 * num3 * Random.Range(Mathf.RoundToInt((float)MyStats.Level / 2f), MyStats.Level)) * (Random.Range(4f, 22f) / 20f));
				num2 = Mathf.RoundToInt((float)num2 * scalingBonus);
				float maxInclusive = Random.Range(17, 21);
				num2 = Mathf.RoundToInt((float)num2 * Random.Range(10f, maxInclusive) / 20f);
				GameData.CamControl.ShakeScreen(1f, 0.2f);
				if (_skill.Id == "63114341" && HasAscension("31074428") && Random.Range(0, 8) < GetAscensionRank("31074428"))
				{
					num2 = Mathf.RoundToInt((float)num2 * 1.5f);
					UpdateSocialLog.CombatLogAdd("Critical BACKSTAB!");
				}
			}
			else if (MyStats.Myself.isNPC)
			{
				num2 = MyStats.CalcMeleeDamage(MyStats.Myself.MyNPC.BaseAtkDmg, _target.MyStats.Level, _target.MyStats, 0);
			}
			else if (MyStats.MyInv.MH.MyItem != null && MyStats.MyInv.MH.MyItem.WeaponDmg > 0)
			{
				num2 = MyStats.CalcMeleeDamage(MyStats.MyInv.MH.MyItem.WeaponDmg, _target.MyStats.Level, _target.MyStats, 0);
			}
			else
			{
				UpdateSocialLog.LogAdd("This skill requires a weapon to be equipped!", "yellow");
			}
		}
		if (isPlayer)
		{
			if (_target != null && num2 > 0)
			{
				if (PC.CheckTargetInMeleeRange(_target))
				{
					if (_skill.SkillAnimName != "" && _skill.SkillAnimName != null)
					{
						MyAnim.SetTrigger(_skill.SkillAnimName);
					}
					int num4 = _target.DamageMe(num2, _fromPlayer: true, _skill.DmgType, GetComponent<Character>(), _animEffect: true, criticalHit);
					if (num4 > 0)
					{
						UpdateSocialLog.CombatLogAdd(_skill.PlayerUses + " " + _target.transform.name + " for " + num4 + " damage.");
						if (_skill.EffectToApply != null)
						{
							_target.MyStats.AddStatusEffect(_skill.EffectToApply, isPlayer, 0, MyStats.Myself);
						}
						if (_skill.Interrupt)
						{
							_target.transform.position += new Vector3(0.2f, 0f, 0.2f);
							InterruptTargetSpell(_target);
						}
						TryProc(_skill, _target);
					}
					else
					{
						switch (num4)
						{
						case 0:
							UpdateSocialLog.CombatLogAdd("You try to use " + _skill.SkillName + ", but miss!");
							break;
						case -1:
							UpdateSocialLog.CombatLogAdd("You try to use " + _skill.SkillName + ", but " + _target.transform.name + " is INVULNERABLE!");
							break;
						}
					}
				}
				else
				{
					UpdateSocialLog.CombatLogAdd("Target out of range!");
				}
			}
			else
			{
				UpdateSocialLog.CombatLogAdd("No target selected.");
			}
		}
		else
		{
			string text = ((!(_target.transform.tag == "Player")) ? _target.transform.name : "YOU");
			int num5 = _target.DamageMe(num2, _fromPlayer: true, _skill.DmgType, GetComponent<Character>(), _animEffect: true, criticalHit);
			if (Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 15f)
			{
				UpdateSocialLog.CombatLogAdd(base.transform.name + " " + _skill.NPCUses + " " + text + " for " + num5 + " damage!");
			}
			MyAnim.SetTrigger(_skill.SkillAnimName);
		}
	}

	public void TryProc(Skill _skill, Character _target)
	{
		if (!(_target == null))
		{
			if (_skill.CastOnTarget != null && MyStats.Myself.MyNPC != null && MyStats.Myself.MyNPC.SimPlayer)
			{
				MyStats.Myself.MySpells.StartSpellFromProc(_skill.CastOnTarget, _target.MyStats, 0.1f);
			}
			if (_skill.ProcShield && MyStats.MyInv.SecondaryShield && MyStats.Myself.MyNPC != null && MyStats.Myself.MyNPC.SimPlayer && MyStats.MyInv.SimOH.MyItem != null && MyStats.MyInv.SimOH.MyItem.WeaponProcOnHit != null && (float)Random.Range(0, 100) < MyStats.MyInv.SimOH.MyItem.WeaponProcChance)
			{
				MyStats.Myself.MySpells.StartSpellFromProc(MyStats.MyInv.SimOH.MyItem.WeaponProcOnHit, _target.MyStats, 0.1f);
			}
			if (_skill.ProcWeap && MyStats.Myself.MyNPC != null && MyStats.Myself.MyNPC.SimPlayer && MyStats.MyInv.SimMH.MyItem != null && MyStats.MyInv.SimMH.MyItem.WeaponProcOnHit != null && (float)Random.Range(0, 100) < MyStats.MyInv.SimMH.MyItem.WeaponProcChance)
			{
				MyStats.Myself.MySpells.StartSpellFromProc(MyStats.MyInv.SimMH.MyItem.WeaponProcOnHit, _target.MyStats, 0.1f);
			}
			if (_skill.ProcWeap && !MyStats.Myself.isNPC && MyStats.MyInv.MH.MyItem != null && MyStats.MyInv.MH.MyItem.WeaponProcOnHit != null && (float)Random.Range(0, 100) < MyStats.MyInv.MH.MyItem.WeaponProcChance)
			{
				MyStats.Myself.MySpells.StartSpellFromProc(MyStats.MyInv.MH.MyItem.WeaponProcOnHit, _target.MyStats, 0.1f);
			}
			if (_skill.ProcShield && !MyStats.Myself.isNPC && MyStats.MyInv.OH.MyItem != null && MyStats.MyInv.OH.MyItem.WeaponProcOnHit != null && (float)Random.Range(0, 100) < MyStats.MyInv.OH.MyItem.WeaponProcChance)
			{
				MyStats.Myself.MySpells.StartSpellFromProc(MyStats.MyInv.OH.MyItem.WeaponProcOnHit, _target.MyStats, 0.1f);
			}
			if (_skill.CastOnTarget != null && !MyStats.Myself.MyNPC)
			{
				MyStats.Myself.MySpells.StartSpellFromProc(_skill.CastOnTarget, _target.MyStats, 0.1f);
			}
		}
	}

	private void InterruptTargetSpell(Character _target)
	{
		if ((_target?.MySpells?.isCasting()).GetValueOrDefault())
		{
			_target.MySpells.InterruptCast();
		}
	}
}
