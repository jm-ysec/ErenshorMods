// SpellVessel
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class SpellVessel : MonoBehaviour
{
	private float EffectLife;

	private float overChantLife;

	private float overChantTotal;

	private float totalLife;

	private GameObject ChargeFX;

	private Vector2 castScale = new Vector2(1f, 13f);

	private Vector2 overChant = new Vector2(1f, 10f);

	private Vector2 npcCastScale = new Vector3(0f, 1f, 0.1f);

	private CastSpell SpellSource;

	private Stats targ;

	private Transform AECollector;

	private Transform casterTrans;

	private Vector3 startPos;

	public Spell spell;

	private int DynamicAudIndex = -1;

	public float maxTime = 900f;

	private bool interruptable = true;

	public bool UseMana = true;

	private bool resonating;

	private bool instant;

	private float resistModifier;

	public bool isProc;

	private Transform CastBar;

	private float CDMult = 1f;

	public AudioSource MyAud;

	private float scaleDmg = 1f;

	private bool ControlledChant;

	private void Awake()
	{
		MyAud.volume = GameData.SpellVol * 0.015f * GameData.MasterVol;
	}

	public void ScaleParticles(GameObject root)
	{
		if (!root)
		{
			return;
		}
		float num = 1f;
		num = ((!SpellSource.isPlayer) ? GameData.ParticleSystemScalingOther : GameData.ParticleSystemScaling);
		ParticleSystem[] componentsInChildren = root.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
		foreach (ParticleSystem obj in componentsInChildren)
		{
			ParticleSystem.MainModule main = obj.main;
			ParticleSystem.EmissionModule emission = obj.emission;
			main.maxParticles = Mathf.Max(1, Mathf.RoundToInt((float)main.maxParticles * num));
			emission.rateOverTimeMultiplier *= num;
			emission.rateOverDistanceMultiplier *= num;
			int burstCount = emission.burstCount;
			if (burstCount > 0)
			{
				ParticleSystem.Burst[] array = new ParticleSystem.Burst[burstCount];
				emission.GetBursts(array);
				for (int j = 0; j < burstCount; j++)
				{
					array[j].minCount = (short)Mathf.RoundToInt((float)array[j].minCount * num);
					array[j].maxCount = (short)Mathf.RoundToInt((float)array[j].maxCount * num);
				}
				emission.SetBursts(array);
			}
		}
	}

	public void CreateSpellProc(Spell _spell, Transform _caster, Stats _target, CastSpell _source, float _castTime, bool _useMana, bool _resonate, float _scaleDmg = 1f)
	{
		scaleDmg = _scaleDmg;
		maxTime = 900f;
		isProc = true;
		CDMult = 1f;
		resistModifier = _spell.ResistModifier;
		SpellSource = _source;
		spell = _spell;
		targ = _target;
		EffectLife = 0f;
		totalLife = 1f;
		casterTrans = _caster;
		ControlledChant = false;
		base.transform.position = _caster.transform.position + _caster.transform.forward + Vector3.up * 1.5f;
		base.transform.parent = _caster;
		startPos = _caster.transform.position;
		ChargeFX = Object.Instantiate(GameData.EffectDB.SpellEffects[_spell.SpellChargeFXIndex], base.transform.position, base.transform.rotation);
		ParticleSystem component = ChargeFX.GetComponent<ParticleSystem>();
		if (component != null)
		{
			component.Stop();
		}
		UseMana = _useMana;
		resonating = _resonate;
		resistModifier += CalculateCharismaModifier(SpellSource.MyChar.MyStats.Level, Mathf.RoundToInt((float)(SpellSource.MyChar.MyStats.GetCurrentCha() * 5) * ((float)SpellSource.MyChar.MyStats.ChaScaleMod / 100f)));
		if (_source.MyChar.MySkills != null)
		{
			resistModifier += resistModifier * ((float)_source.MyChar.MySkills.GetAscensionRank("5805602") * 0.1f);
		}
		interruptable = false;
		if (_spell.Type == Spell.SpellType.AE)
		{
			AECollector = Object.Instantiate(GameData.EffectDB.AEcollector, targ.transform.position + Vector3.up, base.transform.rotation, targ.transform).transform;
		}
		if (_spell.Type == Spell.SpellType.PBAE)
		{
			AECollector = Object.Instantiate(GameData.EffectDB.AEcollector, _caster.transform.position + Vector3.up, _target.transform.rotation).transform;
		}
	}

	public void CreateSpellChargeEffect(Spell _spell, Transform _caster, Stats _target, CastSpell _source, float _castTime, bool _useMana, bool _resonate, float _scaleDmg = 1f)
	{
		scaleDmg = _scaleDmg;
		maxTime = 900f;
		CDMult = 1f;
		resistModifier = _spell.ResistModifier;
		_source.GetComponent<Animator>().ResetTrigger("CastAtkEnd");
		SpellSource = _source;
		spell = _spell;
		targ = _target;
		EffectLife = 0f;
		totalLife = _castTime;
		if (_source.MyChar.MyStats.CharacterClass == GameData.ClassDB.Arcanist)
		{
			overChantTotal = _castTime * 0.4f;
		}
		else
		{
			overChantTotal = 0f;
		}
		overChantLife = 0f;
		overChant.x = 0f;
		casterTrans = _caster;
		ControlledChant = false;
		base.transform.position = _caster.transform.position + _caster.transform.forward + Vector3.up * 1.5f;
		base.transform.parent = _caster;
		startPos = _caster.transform.position;
		ChargeFX = Object.Instantiate(GameData.EffectDB.SpellEffects[_spell.SpellChargeFXIndex], base.transform.position, base.transform.rotation);
		ParticleSystem component = ChargeFX.GetComponent<ParticleSystem>();
		if (component != null)
		{
			component.Stop();
			ParticleSystem.MainModule main = component.main;
			_ = component.emission;
			main.loop = true;
			ScaleParticles(ChargeFX);
			component.Play();
		}
		UseMana = _useMana;
		ChargeFX.transform.parent = base.transform;
		resonating = _resonate;
		_ = _source.transform.name == "Player";
		resistModifier += CalculateCharismaModifier(SpellSource.MyChar.MyStats.Level, Mathf.RoundToInt((float)(SpellSource.MyChar.MyStats.GetCurrentCha() * 5) * ((float)SpellSource.MyChar.MyStats.ChaScaleMod / 100f)));
		if (_source.MyChar.MySkills != null)
		{
			resistModifier += resistModifier * ((float)_source.MyChar.MySkills.GetAscensionRank("5805602") * 0.1f);
		}
		if (_castTime <= 1f || _spell.CannotInterrupt)
		{
			interruptable = false;
		}
		if (_castTime <= 0f)
		{
			instant = true;
		}
		if (_source.transform.name == "Player")
		{
			GameData.CB.NewCast(_spell.SpellName);
		}
		if (_spell.Type == Spell.SpellType.AE)
		{
			AECollector = Object.Instantiate(GameData.EffectDB.AEcollector, targ.transform.position + Vector3.up, base.transform.rotation, targ.transform).transform;
		}
		if (_spell.Type == Spell.SpellType.PBAE)
		{
			AECollector = Object.Instantiate(GameData.EffectDB.AEcollector, _caster.transform.position + Vector3.up, _target.transform.rotation).transform;
		}
		if (SpellSource.isSimPlayer || !SpellSource.isPlayer)
		{
			if (CastBar == null)
			{
				CastBar = SpellSource.MyChar.MyNPC.NameFlash.CastBar;
			}
			else
			{
				CastBar.localScale = new Vector3(0.7f, 1f, 0.1f);
			}
		}
	}

	public float CalculateCharismaModifier(int level, int charisma)
	{
		float result = 0f;
		if (SpellSource.isPlayer || (SpellSource.MyChar.MyNPC != null && SpellSource.MyChar.MyNPC.SimPlayer))
		{
			result = (float)charisma / 100f;
		}
		return result;
	}

	private void FixedUpdate()
	{
		if (instant && targ != null)
		{
			if (!targ.Myself.Invulnerable && !targ.Myself.MiningNode && targ.Myself.Alive)
			{
				if ((!SpellSource.MyChar.MyStats.Stunned && !SpellSource.MyChar.MyStats.Feared && SpellSource.MyChar.Alive && Vector3.Distance(casterTrans.position, startPos) < 0.2f) || !interruptable || (SpellSource.MyChar.isNPC && SpellSource.MyChar.MyNPC.SimPlayer))
				{
					if (spell.Type != Spell.SpellType.Misc && !spell.CanHitPlayers && (!targ.Myself.isNPC || targ.GetComponent<SimPlayer>() != null))
					{
						UpdateSocialLog.LogAdd(SpellSource.transform.name + " the spell could not take hold...", "#FF9000");
						if (SpellSource.isPlayer)
						{
							GameData.CB.CloseBar();
						}
						EndSpell();
					}
					else
					{
						ResolveSpell();
					}
				}
				else
				{
					if (DynamicAudIndex != -1)
					{
						DynamicAudio.StopAudioAtIndex(DynamicAudIndex);
					}
					if (SpellSource.isPlayer && !SpellSource.MyChar.isNPC)
					{
						if (interruptable)
						{
							UpdateSocialLog.LogAdd("You have moved and interrupted your casting!", "#FF9000");
							GameData.CB.CloseBar();
							EndSpellNoCD();
						}
					}
					else if (Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 10f)
					{
						UpdateSocialLog.LogAdd(SpellSource.transform.name + "'s casting has been interrupted!", "#FF9000");
					}
					EndSpellNoCD();
				}
			}
			else
			{
				if (SpellSource.isPlayer && SpellSource.MyChar != null && !SpellSource.MyChar.isNPC)
				{
					UpdateSocialLog.LogAdd("This target is invulnerable.", "#FF9000");
					GameData.CB.CloseBar();
				}
				else if (Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 15f)
				{
					UpdateSocialLog.LogAdd(targ.transform.name + " absorbed the spell.", "#FF9000");
				}
				EndSpell();
			}
		}
		else if (targ == null)
		{
			if (SpellSource.isPlayer && SpellSource.MyChar != null && !SpellSource.MyChar.isNPC && interruptable)
			{
				UpdateSocialLog.LogAdd("You have moved and interrupted your casting!", "#FF9000");
				GameData.CB.CloseBar();
			}
			EndSpellNoCD();
		}
		if (EffectLife < totalLife + overChantTotal)
		{
			EffectLife += 1f * (60f * Time.deltaTime);
			if (SpellSource.isPlayer)
			{
				castScale.x = Mathf.Clamp01(EffectLife / totalLife) * 275f;
				GameData.CB.TopBar.sizeDelta = castScale;
			}
			else if (CastBar != null)
			{
				if (SpellSource.MyChar.MyNPC != null && ((SpellSource.MyChar.MyNPC.SimPlayer && GameData.SimCastBars) || (!SpellSource.MyChar.MyNPC.SimPlayer && GameData.NPCCastBars)))
				{
					npcCastScale.x = (totalLife - EffectLife) / totalLife * 0.7f;
					CastBar.localScale = npcCastScale;
				}
				else if (npcCastScale.x > 0f)
				{
					npcCastScale.x = 0f;
					CastBar.localScale = npcCastScale;
				}
			}
			if (EffectLife >= totalLife && targ != null)
			{
				if (!targ.Myself.Invulnerable && !targ.Myself.MiningNode && targ.Myself.Alive)
				{
					if ((SpellSource.MyChar.Alive && Vector3.Distance(casterTrans.position, startPos) < 0.2f) || !interruptable || (SpellSource.MyChar.isNPC && SpellSource.MyChar.MyNPC.SimPlayer))
					{
						if (spell.Type != Spell.SpellType.Misc && !spell.CanHitPlayers && (!targ.Myself.isNPC || targ.GetComponent<SimPlayer>() != null))
						{
							UpdateSocialLog.LogAdd(SpellSource.transform.name + " the spell could not take hold...", "#FF9000");
							if (SpellSource.isPlayer)
							{
								GameData.CB.CloseBar();
							}
							EndSpell();
						}
						else if (!ControlledChant)
						{
							ResolveSpell();
						}
						else if (overChantLife < overChantTotal)
						{
							overChantLife = EffectLife - totalLife;
							GameData.CamControl.ShakeScreen(1.5f * overChantLife / overChantTotal, 0.3f);
							scaleDmg = 1f + 0.35f * (EffectLife / totalLife);
							if (SpellSource.isPlayer)
							{
								overChant.x = overChantLife / overChantTotal * 275f;
								GameData.CB.OCBarRect.sizeDelta = overChant;
							}
							if (overChantLife >= overChantTotal)
							{
								CompleteSpellEarlyWithScaling();
							}
						}
					}
					else
					{
						if (DynamicAudIndex != -1)
						{
							DynamicAudio.StopAudioAtIndex(DynamicAudIndex);
						}
						if (SpellSource.isPlayer && SpellSource.MyChar != null && !SpellSource.MyChar.isNPC)
						{
							if (interruptable)
							{
								UpdateSocialLog.LogAdd("You have moved and interrupted your casting!", "#FF9000");
								GameData.CB.CloseBar();
								EndSpellNoCD();
							}
						}
						else if (Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 10f)
						{
							UpdateSocialLog.LogAdd(SpellSource.transform.name + "'s casting has been interrupted!", "#FF9000");
						}
						EndSpellNoCD();
					}
				}
				else
				{
					if (SpellSource.isPlayer)
					{
						UpdateSocialLog.LogAdd("This target is invulnerable.", "#FF9000");
						GameData.CB.CloseBar();
					}
					else if (Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position) < 15f)
					{
						UpdateSocialLog.LogAdd(targ.transform.name + " absorbed the spell.", "#FF9000");
					}
					EndSpell();
				}
			}
			else if (targ == null)
			{
				if (SpellSource.isPlayer && SpellSource.MyChar != null && !SpellSource.MyChar.isNPC && interruptable)
				{
					UpdateSocialLog.LogAdd("You have moved and interrupted your casting!", "#FF9000");
					GameData.CB.CloseBar();
				}
				EndSpellNoCD();
			}
		}
		if (maxTime > 0f)
		{
			maxTime -= 1f;
		}
		if (maxTime <= 0f)
		{
			GameData.CB.CloseBar();
			UpdateSocialLog.LogAdd("Spell did not complete - commands missed. Please alert dev team", "green");
			UpdateSocialLog.LogAdd("DEBUG: EffectLifeValue = " + EffectLife + "/" + totalLife + " | " + DynamicAudIndex + " DynamicAudIndex", "green");
			UpdateSocialLog.LogAdd("DEBUG: Caster: = " + SpellSource.transform.name + "/ Spell: " + spell.SpellName + " | Target: " + targ.transform.name, "green");
			EndSpell();
		}
	}

	public float GetCastProgress()
	{
		if (EffectLife <= totalLife)
		{
			return EffectLife / totalLife;
		}
		return 1f + 0.2f * (EffectLife / totalLife);
	}

	public bool DoControlledChant()
	{
		bool flag = false;
		if (!ControlledChant)
		{
			ControlledChant = true;
			GameData.CB.OCTxt.gameObject.SetActive(value: true);
			return false;
		}
		CompleteSpellEarlyWithScaling();
		return true;
	}

	public void ResolveEarly()
	{
		ResolveSpell();
	}

	public void CompleteSpellEarlyWithPenalty()
	{
		CDMult = 3f;
		ResolveSpell();
	}

	public void CompleteSpellEarlyWithScaling()
	{
		scaleDmg = Mathf.Clamp01(EffectLife / totalLife) + 0.2f * (overChantLife / overChantTotal);
		scaleDmg = ConvertToCurve(scaleDmg * 100f) / 100f;
		ResolveSpell();
	}

	private float ConvertToCurve(float v, float p = 2f)
	{
		float f = v / 100f;
		return 100f * Mathf.Pow(f, p);
	}

	private bool isStrafingInputs()
	{
		if (!Input.GetKey(InputManager.StrafeL) && !Input.GetKey(InputManager.StrafeR))
		{
			if (Input.GetKey(InputManager.Left) || Input.GetKey(InputManager.Right))
			{
				return Input.GetMouseButton(1);
			}
			return false;
		}
		return true;
	}

	private void Update()
	{
		if (SpellSource.isPlayer && SpellSource.MyChar != null && !SpellSource.MyChar.isNPC && !GameData.PlayerTyping && (Input.GetKeyDown(InputManager.Jump) || Input.GetKeyDown(InputManager.Forward) || Input.GetKeyDown(InputManager.Backward) || isStrafingInputs()) && interruptable)
		{
			UpdateSocialLog.LogAdd("You have moved and interrupted your casting!", "#FF9000");
			GameData.CB.CloseBar();
			EndSpellNoCD();
		}
	}

	private void ResolveSpell()
	{
		float num = 0f;
		int dmgBonus = 0;
		bool flag = false;
		Stance stance = SpellSource?.MyChar?.MyStats?.CombatStance ?? GameData.SkillDatabase.NormalStance;
		if (SpellSource.isPlayer && !isProc)
		{
			overChant.x = 0f;
			GameData.CB.OCBarRect.sizeDelta = overChant;
			GameData.CB.CloseBar();
		}
		if (UseMana)
		{
			SpellSource.MyChar.MyStats.ReduceMana(spell.ManaCost);
		}
		float num2 = Vector3.Distance(base.transform.position, GameData.PlayerControl.transform.position);
		if (num2 < 30f && spell.ShakeDur > 0f)
		{
			GameData.CamControl.ShakeScreen(spell.ShakeAmp, spell.ShakeDur);
		}
		ParticleSystem particleSystem = null;
		GameObject gameObject = null;
		float aggroGenMod = stance.AggroGenMod;
		switch (spell.Type)
		{
		case Spell.SpellType.Damage:
		{
			flag = true;
			bool flag2 = false;
			if ((float)Random.Range(0, 2000) < (float)SpellSource.MyChar.MyStats.GetCurrentInt() * ((float)SpellSource.MyChar.MyStats.IntScaleMod / 100f) && SpellSource.MyChar.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByID("8312964")))
			{
				flag2 = true;
			}
			if (Vector3.Distance(targ.transform.position, GameData.PlayerControl.transform.position) < 40f)
			{
				gameObject = Object.Instantiate(GameData.EffectDB.SpellEffects[spell.SpellResolveFXIndex], targ.transform.position, Quaternion.identity);
				particleSystem = gameObject.GetComponent<ParticleSystem>();
				if (particleSystem != null)
				{
					ScaleParticles(gameObject);
					ParticleSystem.MainModule main = particleSystem.main;
					main.stopAction = ParticleSystemStopAction.Destroy;
					main.cullingMode = ParticleSystemCullingMode.AlwaysSimulate;
				}
			}
			dmgBonus = 0;
			if (spell.TargetDamage > 0 && (SpellSource.isPlayer || SpellSource.isSimPlayer))
			{
				dmgBonus = CalcDmgBonus(Mathf.RoundToInt((float)spell.TargetDamage * scaleDmg));
			}
			else if (spell.TargetDamage > 0)
			{
				dmgBonus = Mathf.RoundToInt((float)spell.TargetDamage * scaleDmg * stance.SpellDamageMod);
			}
			if (flag2)
			{
				dmgBonus = Mathf.RoundToInt((float)dmgBonus * Random.Range(1.1f, 1.3f));
			}
			int num4 = targ.GetComponent<Character>().MagicDamageMe(dmgBonus, SpellSource.MyChar.MyStats.Charmed || (SpellSource.MyChar.isNPC && SpellSource.MyChar.MyNPC.ThisSim != null && SpellSource.MyChar.MyNPC.InGroup) || SpellSource.isPlayer, spell.MyDamageType, SpellSource.MyChar, resistModifier, spell.TargetDamage);
			if (num4 > spell.TargetDamage * 15)
			{
				num4 = spell.TargetDamage * 15;
			}
			if (num4 > 0)
			{
				num4 += Mathf.RoundToInt((float)num4 * ((float)SpellSource.MyChar.MySkills.GetAscensionRank("71342250") * 0.05f));
			}
			if (num4 > 0)
			{
				if (targ.Myself.isNPC && SpellSource != null)
				{
					targ.Myself.MyNPC.ManageAggro(Mathf.RoundToInt((float)spell.Aggro * aggroGenMod), SpellSource.MyChar);
				}
				if (spell.JoltSpell)
				{
					targ.transform.position += new Vector3(0.2f, 0f, 0.2f);
					if ((targ?.Myself?.MySpells?.isCasting()).GetValueOrDefault())
					{
						targ.Myself.MySpells.InterruptCast();
					}
				}
				if (SpellSource.isPlayer)
				{
					if (spell.ShakeDur == 0f)
					{
						GameData.CamControl.ShakeScreen(1f, 0.2f);
					}
					else
					{
						GameData.CamControl.ShakeScreen(1f, spell.ShakeDur);
					}
				}
			}
			if (num4 <= 0 && num4 != -1)
			{
				if (targ.Myself.isNPC && SpellSource != null)
				{
					targ.Myself.MyNPC.ManageAggro(Mathf.RoundToInt((float)spell.Aggro * aggroGenMod), SpellSource.MyChar);
					if (targ.Stunned && spell.TauntSpell)
					{
						targ.Myself.MyNPC.CurrentAggroTarget = SpellSource.MyChar;
					}
				}
				if (spell.TauntSpell && SpellSource.isPlayer)
				{
					if (spell.ShakeDur == 0f)
					{
						GameData.CamControl.ShakeScreen(1f, 0.2f);
					}
					else
					{
						GameData.CamControl.ShakeScreen(1f, spell.ShakeDur);
					}
				}
			}
			if (SpellSource.isPlayer)
			{
				UpdateSocialLog.CombatLogAdd(targ.transform.name + " " + spell.StatusEffectMessageOnNPC, "lightblue");
				targ.Myself.FlagForFactionHit(_bool: true);
				if (GameData.PlayerControl.Myself.MyCharmedNPC != null && targ.Myself != null && targ.Myself.MyFaction != Character.Faction.Mineral && SpellSource.MyChar == GameData.PlayerControl.Myself && targ.Myself.MyFaction != 0 && !targ.Myself.MyStats.Charmed)
				{
					GameData.PlayerControl.Myself.MyCharmedNPC.CurrentAggroTarget = targ.Myself;
				}
				if (num4 > 0 || spell.TauntSpell)
				{
					if (flag2)
					{
						UpdateSocialLog.CombatLogAdd("You land a critical blast!", "yellow");
						if (num2 < 20f)
						{
							GameData.CamControl.ShakeScreen(2f, 0.4f);
						}
						if (SpellSource.MyChar.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByID("403426")))
						{
							targ.AddStatusEffect(GameData.SpellDatabase.GetSpellByID("10644536"), SpellSource.isPlayer, 0, SpellSource.MyChar);
							UpdateSocialLog.LogAdd(targ.transform.name + " is ravaged by a Lingering Inferno.", "lightblue");
						}
					}
					if (num4 > 0)
					{
						UpdateSocialLog.CombatLogAdd("Your " + spell.SpellName + " spell hits " + targ.transform.name + " for " + num4 + " spell damage!", "lightblue");
					}
					if (spell.StatusEffectToApply != null)
					{
						if (!spell.ApplyToCaster)
						{
							targ.AddStatusEffect(spell.StatusEffectToApply, SpellSource.isPlayer, dmgBonus, SpellSource.MyChar);
						}
						else
						{
							SpellSource.MyChar.MyStats.AddStatusEffect(spell.StatusEffectToApply, SpellSource.isPlayer, dmgBonus, SpellSource.MyChar);
						}
					}
					if (spell.Lifetap)
					{
						SpellSource.MyChar.MyStats.HealMe(num4);
					}
				}
				else if (num4 == -1 || targ.Myself.MyFaction == Character.Faction.Player)
				{
					UpdateSocialLog.CombatLogAdd(targ.transform.name + " cannot be damaged by you.", "lightblue");
				}
				else
				{
					UpdateSocialLog.CombatLogAdd(targ.transform.name + " resisted the " + spell.SpellName + " spell!", "#FF9000");
					if (targ.Myself.isNPC)
					{
						targ.Myself.GetComponent<NPC>().AggroOn(SpellSource.MyChar);
					}
				}
			}
			else if (num4 > 0 || spell.TauntSpell)
			{
				if (!targ.Myself.isNPC)
				{
					UpdateSocialLog.CombatLogAdd("You " + spell.StatusEffectMessageOnPlayer, "lightblue");
					UpdateSocialLog.CombatLogAdd(SpellSource.transform.name + " hits YOU for " + num4 + " spell damage!", "red");
				}
				else
				{
					if (SpellSource.GetComponent<SimPlayer>() != null && Random.Range(0, 2000) < SpellSource.MyChar.MyStats.GetCurrentInt() && SpellSource.MyChar.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByID("8312964")))
					{
						num4 = Mathf.RoundToInt((float)num4 * Random.Range(1.2f, 1.6f));
						if (num2 < 10f)
						{
							UpdateSocialLog.CombatLogAdd(SpellSource.MyChar.MyNPC.NPCName + " lands a critical blast!", "yellow");
						}
						if (SpellSource.MyChar.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByID("403426")))
						{
							targ.AddStatusEffect(GameData.SpellDatabase.GetSpellByID("10644536"), SpellSource.isPlayer, 0, SpellSource.MyChar);
							if (num2 < 10f)
							{
								UpdateSocialLog.LogAdd(targ.transform.name + " is ravaged by a Lingering Inferno.", "lightblue");
							}
						}
					}
					if (SpellSource.isPlayer)
					{
						UpdateSocialLog.CombatLogAdd(targ.transform.name + " " + spell.StatusEffectMessageOnNPC, "lightblue");
						UpdateSocialLog.CombatLogAdd(SpellSource.transform.name + " hits " + targ.transform.name + " for " + num4 + " spell damage!", "lightblue");
					}
				}
				if (spell.StatusEffectToApply != null)
				{
					if (!spell.ApplyToCaster)
					{
						targ.AddStatusEffect(spell.StatusEffectToApply, SpellSource.isPlayer, dmgBonus, SpellSource.MyChar);
					}
					else
					{
						SpellSource.MyChar.MyStats.AddStatusEffect(spell.StatusEffectToApply, SpellSource.isPlayer, dmgBonus, SpellSource.MyChar);
					}
				}
				if (spell.Lifetap)
				{
					SpellSource.MyChar.MyStats.HealMe(num4);
				}
			}
			else if (!targ.Myself.isNPC)
			{
				UpdateSocialLog.CombatLogAdd("You resisted the " + spell.SpellName + " spell!", "yellow");
			}
			else if (num2 < 10f)
			{
				UpdateSocialLog.CombatLogAdd(targ.transform.name + " resisted the " + spell.SpellName + " spell!", "yellow");
			}
			if (Random.Range(0, 100) >= SpellSource.MyChar.MyStats.GetCurrentRes() || !(SpellSource.MyChar.MyStats.resonanceCD <= 0f) || spell.NoResonate)
			{
				break;
			}
			if (SpellSource.isPlayer)
			{
				UpdateSocialLog.CombatLogAdd("Your spell resonates and casts again!", "yellow");
			}
			else if (!(num2 < 15f))
			{
				_ = targ.Myself.isNPC;
			}
			if (!isProc)
			{
				SpellSource.StartSpell(spell, targ, 1.1f, _resonate: true, scaleDmg);
			}
			else
			{
				SpellSource.StartSpellFromProc(spell, targ, 1f, _resonating: true, scaleDmg);
			}
			if (SpellSource.MyChar.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByName("Arcane Recovery")))
			{
				SpellSource.MyChar.MyStats.CurrentMana += Mathf.RoundToInt((float)spell.ManaCost * Random.Range(0.15f, 0.65f));
				if (SpellSource.MyChar.MyStats.CurrentMana > SpellSource.MyChar.MyStats.GetCurrentMaxMana())
				{
					SpellSource.MyChar.MyStats.CurrentMana = SpellSource.MyChar.MyStats.GetCurrentMaxMana();
				}
				if (SpellSource.isPlayer)
				{
					UpdateSocialLog.CombatLogAdd("You feel a surge of returning mana.", "yellow");
				}
				else if (!(num2 < 15f))
				{
					_ = targ.Myself.isNPC;
				}
			}
			SpellSource.MyChar.MyStats.resonanceCD = 60f;
			break;
		}
		case Spell.SpellType.StatusEffect:
			flag = true;
			num = 0f;
			dmgBonus = 0;
			if (Vector3.Distance(targ.transform.position, GameData.PlayerControl.transform.position) < 40f)
			{
				gameObject = Object.Instantiate(GameData.EffectDB.SpellEffects[spell.SpellResolveFXIndex], targ.transform.position, Quaternion.identity);
				ScaleParticles(gameObject);
				gameObject.AddComponent<DestroyObjectTimer>().TimeToDestroy = 600f;
				if (spell.RootTarget && gameObject != null)
				{
					ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
					if (component != null)
					{
						ParticleSystem.MainModule main2 = component.main;
						component.Stop();
						main2.duration = spell.SpellDurationInTicks * 3;
						main2.startLifetime = spell.SpellDurationInTicks * 3;
						component.Play();
					}
				}
			}
			if (spell.TargetDamage > 0)
			{
				dmgBonus = Mathf.RoundToInt((float)(SpellSource.MyChar.MyStats.Level / 4) * ((float)SpellSource.MyChar.MyStats.IntScaleMod / 100f * (float)SpellSource.MyChar.MyStats.GetCurrentInt()) * stance.SpellDamageMod);
			}
			if (targ.Myself.isNPC && !targ.Myself.Invulnerable && (targ.Myself.MyFaction != 0 || !SpellSource.isPlayer))
			{
				if (spell.TargetDamage > 0 && GameData.PlayerControl.Myself.MyCharmedNPC != null && targ.Myself != null && targ.Myself.MyFaction != Character.Faction.Mineral && SpellSource.MyChar == GameData.PlayerControl.Myself && targ.Myself.MyFaction != 0 && !targ.Myself.MyStats.Charmed)
				{
					GameData.PlayerControl.Myself.MyCharmedNPC.CurrentAggroTarget = targ.Myself;
				}
				if (targ.Myself.isNPC && SpellSource != null)
				{
					targ.Myself.MyNPC.ManageAggro(Mathf.RoundToInt((float)spell.Aggro * aggroGenMod), SpellSource.MyChar);
				}
				targ.GetComponent<NPC>().AggroOn(SpellSource.MyChar);
				num = targ.Myself.GetRawResist(spell.MyDamageType) + (float)(targ.Level - SpellSource.MyChar.MyStats.Level) * Random.Range(7f, 11f);
				num -= (float)SpellSource.MyChar.MyStats.GetCurrentCha() * ((float)SpellSource.MyChar.MyStats.ChaScaleMod / 200f);
				num -= spell.ResistModifier;
				if (SpellSource.isPlayer)
				{
					targ.Myself.FlagForFactionHit(_bool: true);
				}
				bool flag3 = (float)Random.Range(0, 100) < num;
				if (num < 95f)
				{
					int num5 = -1;
					if (SpellSource.isPlayer)
					{
						UpdateSocialLog.LogAdd(targ.transform.name + " " + spell.StatusEffectMessageOnNPC, "lightblue");
					}
					num5 = targ.GetComponent<Character>().MyStats.AddStatusEffect(spell, SpellSource.MyChar.MyStats.Charmed || (SpellSource.MyChar.isNPC && SpellSource.MyChar.MyNPC.ThisSim != null && SpellSource.MyChar.MyNPC.InGroup) || SpellSource.isPlayer, dmgBonus, SpellSource.MyChar);
					if (spell.StatusEffectToApply != null)
					{
						if (!spell.ApplyToCaster)
						{
							targ.AddStatusEffect(spell.StatusEffectToApply, SpellSource.isPlayer, dmgBonus, SpellSource.MyChar);
						}
						else
						{
							SpellSource.MyChar.MyStats.AddStatusEffect(spell.StatusEffectToApply, SpellSource.isPlayer, dmgBonus, SpellSource.MyChar);
						}
					}
					if (flag3 && num5 != -1)
					{
						targ.ModifySEDurationByPercent(num5, 100f - num);
					}
				}
				else if (SpellSource.isPlayer)
				{
					UpdateSocialLog.CombatLogAdd(targ.transform.name + " resisted the " + spell.SpellName + " spell!", "#FF9000");
				}
			}
			else if (!targ.Myself.isNPC)
			{
				num = targ.Myself.GetRawResist(spell.MyDamageType) + (float)(targ.Level - SpellSource.MyChar.MyStats.Level) * Random.Range(7f, 10f);
				bool flag4 = (float)Random.Range(0, 100) < num;
				if (num < 95f)
				{
					int num6 = -1;
					UpdateSocialLog.LogAdd("You " + spell.StatusEffectMessageOnPlayer, "lightblue");
					num6 = targ.GetComponent<Character>().MyStats.AddStatusEffect(spell, !SpellSource.isPlayer, dmgBonus, SpellSource.MyChar);
					if (spell.StatusEffectToApply != null)
					{
						if (!spell.ApplyToCaster)
						{
							targ.AddStatusEffect(spell.StatusEffectToApply, SpellSource.isPlayer, dmgBonus, SpellSource.MyChar);
						}
						else
						{
							SpellSource.MyChar.MyStats.AddStatusEffect(spell.StatusEffectToApply, SpellSource.isPlayer, dmgBonus, SpellSource.MyChar);
						}
					}
					if (flag4 && num6 != -1)
					{
						targ.ModifySEDurationByPercent(num6, 100f - num);
					}
				}
				else
				{
					UpdateSocialLog.CombatLogAdd("You resisted the " + spell.SpellName + " spell!", "#FF9000");
				}
			}
			if ((targ.Myself.Invulnerable || num >= 100f || (targ.Myself.MyFaction == Character.Faction.Player && SpellSource.MyChar.MyFaction == Character.Faction.Player)) && (SpellSource.isPlayer || num2 < 10f))
			{
				UpdateSocialLog.CombatLogAdd("Your spell did not take hold...", "lightblue");
			}
			break;
		case Spell.SpellType.Beneficial:
			if (Vector3.Distance(targ.transform.position, GameData.PlayerControl.transform.position) < 40f)
			{
				gameObject = Object.Instantiate(GameData.EffectDB.SpellEffects[spell.SpellResolveFXIndex], targ.transform.position, Quaternion.identity);
				ScaleParticles(gameObject);
			}
			if (targ.Myself.isNPC && num2 < 10f)
			{
				UpdateSocialLog.LogAdd(targ.transform.name + " " + spell.StatusEffectMessageOnNPC, "lightblue");
			}
			else if (!targ.Myself.isNPC)
			{
				UpdateSocialLog.LogAdd("You " + spell.StatusEffectMessageOnPlayer, "lightblue");
			}
			if (!targ.Myself.Invulnerable)
			{
				if (!targ.Myself.MyStats.CheckForStatus(spell) && !targ.Myself.MyStats.CheckForHigherLevelSE(spell))
				{
					targ.GetComponent<Character>().MyStats.AddStatusEffect(spell, SpellSource.isPlayer, 0, SpellSource.MyChar);
				}
				else if (targ.Myself.MyStats.CheckForStatus(spell))
				{
					targ.GetComponent<Character>().MyStats.RefreshWornSE(spell);
				}
			}
			if (!spell.GroupEffect)
			{
				break;
			}
			if (!targ.Myself.isNPC)
			{
				if (GameData.GroupMembers[0] != null && GameData.GroupMembers[0].MyAvatar != null && GameData.GroupMembers[0].MyAvatar.MyStats.CurrentHP > 0 && Vector3.Distance(targ.transform.position, GameData.GroupMembers[0].MyAvatar.transform.position) < 20f)
				{
					Object.Instantiate(GameData.EffectDB.SpellEffects[spell.SpellResolveFXIndex], GameData.GroupMembers[0].MyAvatar.transform.position, Quaternion.identity);
					if (!GameData.GroupMembers[0].MyStats.CheckForStatus(spell) && !GameData.GroupMembers[0].MyStats.CheckForHigherLevelSE(spell))
					{
						GameData.GroupMembers[0].MyStats.AddStatusEffect(spell, SpellSource.isPlayer, 0, SpellSource.MyChar);
					}
					else if (GameData.GroupMembers[0].MyStats.CheckForStatus(spell))
					{
						GameData.GroupMembers[0].MyStats.RefreshWornSE(spell);
					}
					UpdateSocialLog.LogAdd(GameData.GroupMembers[0].SimName + " " + spell.StatusEffectMessageOnNPC, "lightblue");
				}
				if (GameData.GroupMembers[1] != null && GameData.GroupMembers[1].MyAvatar != null && GameData.GroupMembers[1].MyAvatar.MyStats.CurrentHP > 0 && Vector3.Distance(targ.transform.position, GameData.GroupMembers[1].MyAvatar.transform.position) < 20f)
				{
					Object.Instantiate(GameData.EffectDB.SpellEffects[spell.SpellResolveFXIndex], GameData.GroupMembers[1].MyAvatar.transform.position, Quaternion.identity);
					if (!GameData.GroupMembers[1].MyStats.CheckForStatus(spell) && !GameData.GroupMembers[1].MyStats.CheckForHigherLevelSE(spell))
					{
						GameData.GroupMembers[1].MyStats.AddStatusEffect(spell, SpellSource.isPlayer, 0, SpellSource.MyChar);
					}
					else if (GameData.GroupMembers[1].MyStats.CheckForStatus(spell))
					{
						GameData.GroupMembers[1].MyStats.RefreshWornSE(spell);
					}
					UpdateSocialLog.LogAdd(GameData.GroupMembers[1].SimName + " " + spell.StatusEffectMessageOnNPC, "lightblue");
				}
				if (GameData.GroupMembers[2] != null && GameData.GroupMembers[2].MyAvatar != null && GameData.GroupMembers[2].MyAvatar.MyStats.CurrentHP > 0 && Vector3.Distance(targ.transform.position, GameData.GroupMembers[2].MyAvatar.transform.position) < 20f)
				{
					Object.Instantiate(GameData.EffectDB.SpellEffects[spell.SpellResolveFXIndex], GameData.GroupMembers[2].MyAvatar.transform.position, Quaternion.identity);
					if (!GameData.GroupMembers[2].MyStats.CheckForStatus(spell) && !GameData.GroupMembers[2].MyStats.CheckForHigherLevelSE(spell))
					{
						GameData.GroupMembers[2].MyStats.AddStatusEffect(spell, SpellSource.isPlayer, 0, SpellSource.MyChar);
					}
					else if (GameData.GroupMembers[2].MyStats.CheckForStatus(spell))
					{
						GameData.GroupMembers[2].MyStats.RefreshWornSE(spell);
					}
					UpdateSocialLog.LogAdd(GameData.GroupMembers[2].SimName + " " + spell.StatusEffectMessageOnNPC, "lightblue");
				}
				if (GameData.GroupMembers[3] != null && GameData.GroupMembers[3].MyAvatar != null && GameData.GroupMembers[3].MyAvatar.MyStats.CurrentHP > 0 && Vector3.Distance(targ.transform.position, GameData.GroupMembers[3].MyAvatar.transform.position) < 20f)
				{
					Object.Instantiate(GameData.EffectDB.SpellEffects[spell.SpellResolveFXIndex], GameData.GroupMembers[3].MyAvatar.transform.position, Quaternion.identity);
					if (!GameData.GroupMembers[3].MyStats.CheckForStatus(spell) && !GameData.GroupMembers[3].MyStats.CheckForHigherLevelSE(spell))
					{
						GameData.GroupMembers[3].MyStats.AddStatusEffect(spell, SpellSource.isPlayer, 0, SpellSource.MyChar);
					}
					else if (GameData.GroupMembers[3].MyStats.CheckForStatus(spell))
					{
						GameData.GroupMembers[3].MyStats.RefreshWornSE(spell);
					}
					UpdateSocialLog.LogAdd(GameData.GroupMembers[3].SimName + " " + spell.StatusEffectMessageOnNPC, "lightblue");
				}
				if ((targ.Myself.MyCharmedNPC != null && targ.Myself.MyCharmedNPC.GetComponent<Stats>().CurrentHP > 0) || (SpellSource.isPlayer && GameData.PlayerControl.Myself.MyCharmedNPC != null))
				{
					Object.Instantiate(GameData.EffectDB.SpellEffects[spell.SpellResolveFXIndex], targ.Myself.MyCharmedNPC.transform.position, Quaternion.identity);
					if (!targ.Myself.MyCharmedNPC.GetComponent<Stats>().CheckForStatus(spell) && !targ.Myself.MyCharmedNPC.GetComponent<Stats>().CheckForHigherLevelSE(spell))
					{
						targ.Myself.MyCharmedNPC.GetComponent<Stats>().AddStatusEffect(spell, SpellSource.isPlayer, 0, SpellSource.MyChar);
					}
					else if (targ.Myself.MyCharmedNPC.GetComponent<Stats>().CheckForStatus(spell))
					{
						targ.Myself.MyCharmedNPC.GetComponent<Stats>().RefreshWornSE(spell);
					}
					UpdateSocialLog.LogAdd(targ.Myself.MyCharmedNPC.transform.name + " " + spell.StatusEffectMessageOnNPC, "lightblue");
				}
			}
			if (!targ.Myself.isNPC || targ.Myself.NearbyFriends.Count <= 0)
			{
				break;
			}
			foreach (Character nearbyFriend in targ.Myself.NearbyFriends)
			{
				if (nearbyFriend != null && nearbyFriend.MyStats.CurrentHP > 0)
				{
					Object.Instantiate(GameData.EffectDB.SpellEffects[spell.SpellResolveFXIndex], nearbyFriend.transform.position, Quaternion.identity);
					if (!nearbyFriend.MyStats.CheckForStatus(spell) && !nearbyFriend.MyStats.CheckForHigherLevelSE(spell))
					{
						nearbyFriend.MyStats.AddStatusEffect(spell, SpellSource.isPlayer, 0, SpellSource.MyChar);
					}
					else if (nearbyFriend.MyStats.CheckForStatus(spell))
					{
						nearbyFriend.MyStats.RefreshWornSE(spell);
					}
					if (nearbyFriend.isNPC && num2 < 20f)
					{
						UpdateSocialLog.LogAdd(nearbyFriend.transform.name + " " + spell.StatusEffectMessageOnNPC, "lightblue");
					}
					else if (!nearbyFriend.isNPC)
					{
						UpdateSocialLog.LogAdd("You " + spell.StatusEffectMessageOnPlayer, "lightblue");
					}
				}
			}
			break;
		case Spell.SpellType.AE:
			flag = true;
			foreach (Character nearbyCharacter in AECollector.GetComponent<CharacterCollector>().NearbyCharacters)
			{
				if (nearbyCharacter == null || nearbyCharacter.Invulnerable || nearbyCharacter.transform == null || Vector3.Distance(nearbyCharacter.transform.position, AECollector.transform.position) > spell.SpellRange)
				{
					continue;
				}
				int num3 = 0;
				if (!(nearbyCharacter != null) || nearbyCharacter.MyStats.Charmed || !(nearbyCharacter != SpellSource.MyChar) || SpellSource.MyChar.MyFaction == nearbyCharacter.MyFaction || (SpellSource.MyChar.MyFaction == Character.Faction.PC && nearbyCharacter.MyFaction == Character.Faction.Player) || (SpellSource.MyChar.MyFaction == Character.Faction.Player && nearbyCharacter.MyFaction == Character.Faction.PC) || !CheckLOS(nearbyCharacter))
				{
					continue;
				}
				targ = nearbyCharacter.MyStats;
				if (Vector3.Distance(targ.transform.position, GameData.PlayerControl.transform.position) < 40f)
				{
					gameObject = Object.Instantiate(GameData.EffectDB.SpellEffects[spell.SpellResolveFXIndex], targ.transform.position, Quaternion.identity);
					ScaleParticles(gameObject);
					gameObject.AddComponent<DestroyObjectTimer>().TimeToDestroy = 600f;
				}
				dmgBonus = 0;
				if (spell.TargetDamage > 0)
				{
					dmgBonus = Mathf.RoundToInt((float)(SpellSource.MyChar.MyStats.Level / 4) * ((float)SpellSource.MyChar.MyStats.IntScaleMod / 100f * (float)SpellSource.MyChar.MyStats.GetCurrentInt()) * stance.SpellDamageMod);
				}
				num3 = nearbyCharacter.MagicDamageMe(spell.TargetDamage + dmgBonus, SpellSource.MyChar.MyStats.Charmed || (SpellSource.MyChar.isNPC && SpellSource.MyChar.MyNPC.ThisSim != null && SpellSource.MyChar.MyNPC.InGroup) || SpellSource.isPlayer, spell.MyDamageType, SpellSource.MyChar, spell.ResistModifier, spell.TargetDamage);
				if (spell.StatusEffectToApply != null)
				{
					targ.AddStatusEffect(spell.StatusEffectToApply, SpellSource.isPlayer, dmgBonus, SpellSource.MyChar);
				}
				if (num3 > 0)
				{
					if (targ.Myself.isNPC && SpellSource != null)
					{
						targ.Myself.MyNPC.ManageAggro(Mathf.RoundToInt((float)spell.Aggro * aggroGenMod), SpellSource.MyChar);
					}
					if (SpellSource.isPlayer)
					{
						if (spell.ShakeDur == 0f)
						{
							GameData.CamControl.ShakeScreen(1f, 0.2f);
						}
						else
						{
							GameData.CamControl.ShakeScreen(1f, spell.ShakeDur);
						}
					}
				}
				if (SpellSource.isPlayer)
				{
					if (!(nearbyCharacter.GetComponent<NPC>() != null) || nearbyCharacter.MyNPC.SummonedByPlayer)
					{
						continue;
					}
					nearbyCharacter.FlagForFactionHit(_bool: true);
					if (num3 > 0)
					{
						UpdateSocialLog.CombatLogAdd("Your " + spell.SpellName + " spell hits " + targ.transform.name + " for " + num3 + " damage!", "lightblue");
						UpdateSocialLog.CombatLogAdd(targ.transform.name + " " + spell.StatusEffectMessageOnNPC, "lightblue");
					}
					else if (num3 == -1)
					{
						UpdateSocialLog.CombatLogAdd(targ.transform.name + " cannot be damaged by you.", "lightblue");
					}
					else
					{
						UpdateSocialLog.CombatLogAdd(targ.transform.name + " resisted the " + spell.SpellName + " spell!", "#FF9000");
						if (targ.Myself.isNPC)
						{
							targ.Myself.GetComponent<NPC>().AggroOn(SpellSource.MyChar);
						}
					}
				}
				else
				{
					if (!(num2 < 10f))
					{
						continue;
					}
					if (num3 > 0)
					{
						if (targ.transform.name == "Player")
						{
							UpdateSocialLog.CombatLogAdd("You " + spell.StatusEffectMessageOnPlayer, "lightblue");
							UpdateSocialLog.CombatLogAdd(SpellSource.transform.name + " hits YOU for " + num3 + " spell damage!", "red");
						}
					}
					else if (targ.transform.name == "Player" && targ == GameData.PlayerControl.Myself)
					{
						UpdateSocialLog.CombatLogAdd("You resisted the" + spell.SpellName + " spell!", "yellow");
					}
				}
			}
			if (Random.Range(0, 100) >= SpellSource.MyChar.MyStats.GetCurrentRes() || !(SpellSource.MyChar.MyStats.resonanceCD <= 0f) || spell.NoResonate)
			{
				break;
			}
			if (SpellSource.isPlayer)
			{
				UpdateSocialLog.CombatLogAdd("Your spell resonates and casts again!", "yellow");
			}
			else if (!(num2 < 15f))
			{
				_ = targ.Myself.isNPC;
			}
			if (SpellSource.MyChar.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByName("Arcane Recovery")))
			{
				SpellSource.MyChar.MyStats.CurrentMana += Mathf.RoundToInt((float)spell.ManaCost * Random.Range(0.15f, 0.65f));
				if (SpellSource.MyChar.MyStats.CurrentMana > SpellSource.MyChar.MyStats.GetCurrentMaxMana())
				{
					SpellSource.MyChar.MyStats.CurrentMana = SpellSource.MyChar.MyStats.GetCurrentMaxMana();
				}
				if (SpellSource.isPlayer)
				{
					UpdateSocialLog.CombatLogAdd("You feel a surge of returning mana.", "yellow");
				}
				else if (!(num2 < 15f))
				{
					_ = targ.Myself.isNPC;
				}
			}
			if (!isProc)
			{
				SpellSource.StartSpell(spell, targ, 1.1f, _resonate: true, scaleDmg);
			}
			else
			{
				SpellSource.StartSpellFromProc(spell, targ, 1f, _resonating: true, scaleDmg);
			}
			SpellSource.MyChar.MyStats.resonanceCD = 60f;
			break;
		case Spell.SpellType.PBAE:
			flag = true;
			foreach (Character nearbyCharacter2 in AECollector.GetComponent<CharacterCollector>().NearbyCharacters)
			{
				if (!(nearbyCharacter2 != null) || nearbyCharacter2.MyStats.Charmed || !(nearbyCharacter2 != SpellSource.MyChar) || SpellSource.MyChar.MyFaction == nearbyCharacter2.MyFaction || (SpellSource.MyChar.MyFaction == Character.Faction.PC && nearbyCharacter2.MyFaction == Character.Faction.Player) || (SpellSource.MyChar.MyFaction == Character.Faction.Player && nearbyCharacter2.MyFaction == Character.Faction.PC) || !CheckLOS(nearbyCharacter2) || nearbyCharacter2.Invulnerable || Vector3.Distance(nearbyCharacter2.transform.position, GameData.PlayerControl.transform.position) > spell.SpellRange)
				{
					continue;
				}
				int num7 = 0;
				targ = nearbyCharacter2.MyStats;
				if (Vector3.Distance(targ.transform.position, GameData.PlayerControl.transform.position) < 40f)
				{
					gameObject = Object.Instantiate(GameData.EffectDB.SpellEffects[spell.SpellResolveFXIndex], targ.transform.position, Quaternion.identity);
					ScaleParticles(gameObject);
					gameObject.AddComponent<DestroyObjectTimer>().TimeToDestroy = 600f;
				}
				dmgBonus = 0;
				if (spell.TargetDamage > 0)
				{
					dmgBonus = Mathf.RoundToInt((float)(SpellSource.MyChar.MyStats.Level / 4) * ((float)SpellSource.MyChar.MyStats.IntScaleMod / 100f * (float)SpellSource.MyChar.MyStats.GetCurrentInt()) * stance.SpellDamageMod);
				}
				if (SpellSource.MyChar.Allies.Contains(targ.Myself.MyFaction))
				{
					continue;
				}
				num7 = targ.GetComponent<Character>().MagicDamageMe(spell.TargetDamage + dmgBonus, SpellSource.MyChar.MyStats.Charmed || (SpellSource.MyChar.isNPC && SpellSource.MyChar.MyNPC.ThisSim != null && SpellSource.MyChar.MyNPC.InGroup) || SpellSource.isPlayer, spell.MyDamageType, SpellSource.MyChar, spell.ResistModifier, spell.TargetDamage);
				if (spell.StatusEffectToApply != null)
				{
					targ.AddStatusEffect(spell.StatusEffectToApply, SpellSource.isPlayer, dmgBonus, SpellSource.MyChar);
				}
				if (num7 > 0)
				{
					if (targ.Myself.isNPC && SpellSource != null)
					{
						targ.Myself.MyNPC.ManageAggro(Mathf.RoundToInt((float)spell.Aggro * aggroGenMod), SpellSource.MyChar);
					}
					if (SpellSource.isPlayer)
					{
						if (spell.ShakeDur == 0f)
						{
							GameData.CamControl.ShakeScreen(1f, 0.2f);
						}
						else
						{
							GameData.CamControl.ShakeScreen(1f, spell.ShakeDur);
						}
					}
				}
				if (SpellSource.isPlayer)
				{
					if (!(nearbyCharacter2.GetComponent<NPC>() != null) || nearbyCharacter2.MyNPC.SummonedByPlayer)
					{
						continue;
					}
					if (num7 > 0)
					{
						nearbyCharacter2.FlagForFactionHit(_bool: true);
						UpdateSocialLog.CombatLogAdd("Your " + spell.SpellName + " spell hits " + targ.transform.name + " for " + num7 + " damage!", "lightblue");
						UpdateSocialLog.CombatLogAdd(targ.transform.name + " " + spell.StatusEffectMessageOnNPC, "lightblue");
						if (spell.StatusEffectToApply != null)
						{
							if (!spell.ApplyToCaster)
							{
								targ.AddStatusEffect(spell.StatusEffectToApply, SpellSource.isPlayer, dmgBonus, SpellSource.MyChar);
							}
							else
							{
								SpellSource.MyChar.MyStats.AddStatusEffect(spell.StatusEffectToApply, SpellSource.isPlayer, dmgBonus, SpellSource.MyChar);
							}
						}
						if (spell.Lifetap)
						{
							SpellSource.MyChar.MyStats.HealMe(num7);
						}
						if (spell.InflictOnSelf)
						{
							int num8 = SpellSource.MyChar.SelfDamageMeFlat(num7);
							if (SpellSource.isPlayer)
							{
								UpdateSocialLog.CombatLogAdd("You inflict " + num8 + " damage upon yourself!", "lightblue");
							}
							else if (Vector3.Distance(GameData.PlayerControl.transform.position, SpellSource.transform.position) < 10f)
							{
								UpdateSocialLog.CombatLogAdd(SpellSource.transform.name + " inflicts " + num8 + " damage upon themself!", "lightblue");
							}
						}
					}
					else if (num7 == -1)
					{
						UpdateSocialLog.CombatLogAdd(targ.transform.name + " cannot be damaged by you.", "lightblue");
					}
					else
					{
						UpdateSocialLog.CombatLogAdd(targ.transform.name + " resisted the " + spell.SpellName + " spell!", "#FF9000");
						if (targ.Myself.isNPC)
						{
							targ.Myself.GetComponent<NPC>().AggroOn(SpellSource.MyChar);
						}
					}
				}
				else
				{
					if (!(num2 < 10f))
					{
						continue;
					}
					if (num7 > 0)
					{
						if (targ.transform.name == "Player")
						{
							UpdateSocialLog.CombatLogAdd("You " + spell.StatusEffectMessageOnPlayer, "lightblue");
							UpdateSocialLog.CombatLogAdd(SpellSource.transform.name + " hits YOU for " + num7 + " spell damage!", "red");
						}
						if (spell.StatusEffectToApply != null)
						{
							if (!spell.ApplyToCaster)
							{
								targ.AddStatusEffect(spell.StatusEffectToApply, SpellSource.isPlayer, dmgBonus, SpellSource.MyChar);
							}
							else
							{
								SpellSource.MyChar.MyStats.AddStatusEffect(spell.StatusEffectToApply, SpellSource.isPlayer, dmgBonus, SpellSource.MyChar);
							}
						}
						if (spell.Lifetap)
						{
							SpellSource.MyChar.MyStats.HealMe(num7);
						}
						if (spell.InflictOnSelf)
						{
							int num9 = SpellSource.MyChar.SelfDamageMeFlat(num7);
							if (SpellSource.isPlayer)
							{
								UpdateSocialLog.CombatLogAdd("You inflict " + num9 + " damage upon yourself!", "lightblue");
							}
							else if (Vector3.Distance(GameData.PlayerControl.transform.position, SpellSource.transform.position) < 10f)
							{
								UpdateSocialLog.CombatLogAdd(SpellSource.transform.name + " inflicts " + num9 + " damage upon themself!", "lightblue");
							}
						}
					}
					else if (targ == GameData.PlayerControl.Myself)
					{
						UpdateSocialLog.CombatLogAdd("You resisted the" + spell.SpellName + " spell!", "yellow");
					}
				}
			}
			if (Random.Range(0, 100) >= SpellSource.MyChar.MyStats.GetCurrentRes() || !(SpellSource.MyChar.MyStats.resonanceCD <= 0f) || spell.NoResonate)
			{
				break;
			}
			if (SpellSource.isPlayer)
			{
				UpdateSocialLog.CombatLogAdd("Your spell resonates and casts again!", "yellow");
			}
			else if (!(num2 < 15f))
			{
				_ = targ.Myself.isNPC;
			}
			if (SpellSource.MyChar.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByName("Arcane Recovery")))
			{
				SpellSource.MyChar.MyStats.CurrentMana += Mathf.RoundToInt((float)spell.ManaCost * Random.Range(0.15f, 0.65f));
				if (SpellSource.MyChar.MyStats.CurrentMana > SpellSource.MyChar.MyStats.GetCurrentMaxMana())
				{
					SpellSource.MyChar.MyStats.CurrentMana = SpellSource.MyChar.MyStats.GetCurrentMaxMana();
				}
				if (SpellSource.isPlayer)
				{
					UpdateSocialLog.CombatLogAdd("You feel a surge of returning mana.", "yellow");
				}
				else if (!(num2 < 15f))
				{
					_ = targ.Myself.isNPC;
				}
			}
			if (!isProc)
			{
				SpellSource.StartSpell(spell, targ, 1.1f, _resonate: true, scaleDmg);
			}
			else
			{
				SpellSource.StartSpellFromProc(spell, targ, 1f, _resonating: true, scaleDmg);
			}
			SpellSource.MyChar.MyStats.resonanceCD = 60f;
			break;
		case Spell.SpellType.Heal:
		{
			bool isCrit = false;
			if (Vector3.Distance(targ.transform.position, GameData.PlayerControl.transform.position) < 40f)
			{
				gameObject = Object.Instantiate(GameData.EffectDB.SpellEffects[spell.SpellResolveFXIndex], targ.transform.position, Quaternion.identity);
				ScaleParticles(gameObject);
				gameObject.AddComponent<DestroyObjectTimer>().TimeToDestroy = 600f;
			}
			if (targ.CurrentHP > 0 && targ.Myself.Alive)
			{
				if (spell.MyDamageType == GameData.DamageType.Physical)
				{
					int hP = spell.HP;
					int currentWis = SpellSource.MyChar.MyStats.GetCurrentWis();
					float num10 = (float)SpellSource.MyChar.MyStats.WisScaleMod / 100f;
					float num11 = 1f + num10 * (float)currentWis;
					num11 += (float)(SpellSource.MyChar.MyStats.Level - spell.RequiredLevel) * num11;
					if (num11 < 1f)
					{
						num11 = 1f;
					}
					float num12 = ((SpellSource.MyChar.MyStats.CharacterClass == GameData.ClassDB.Druid) ? 1.1f : 1f);
					int num13 = Mathf.RoundToInt(((float)hP + 4f * num11) * num12);
					if (num13 > spell.HP * 5)
					{
						num13 = spell.HP * 5;
					}
					if (Random.Range(0, 100) < SpellSource.MyChar.MySkills.GetAscensionRank("29551128") * 20)
					{
						isCrit = true;
						num13 += Mathf.RoundToInt((float)num13 * 1.5f);
					}
					num13 += Mathf.RoundToInt((float)(num13 * SpellSource.MyChar.MySkills.GetAscensionRank("1625336")) * 0.1f);
					int incdmg = targ.HealMe(spell, num13, isCrit, _isMana: false, SpellSource.MyChar);
					if (Random.Range(0, 100) < SpellSource.MyChar.MySkills.GetAscensionRank("928370") * 7)
					{
						Character character = null;
						if (targ.Myself.MyNPC != null && targ.Myself.MyNPC.CurrentAggroTarget != null)
						{
							character = targ.Myself.MyNPC.CurrentAggroTarget;
						}
						else if (!targ.Myself.isNPC && targ.GetComponent<PlayerControl>().CurrentTarget != null)
						{
							character = targ.GetComponent<PlayerControl>().CurrentTarget;
							if (character == targ.Myself)
							{
								character = null;
								UpdateSocialLog.LogAdd("Cannot hit yourself with VENGEFUL HEALING! (Target a hostile NPC before healing yourself!", "green");
							}
						}
						if (character != null)
						{
							int num14 = character.DamageMe(incdmg, SpellSource.isPlayer, GameData.DamageType.Void, SpellSource.MyChar, _animEffect: false, _criticalHit: false);
							if (Vector3.Distance(SpellSource.transform.position, GameData.PlayerControl.transform.position) < 15f)
							{
								UpdateSocialLog.CombatLogAdd(character.transform.name + " takes " + num14 + " VENGEFUL HEALING damage!", "green");
							}
						}
					}
					if (spell.StatusEffectToApply != null)
					{
						if (!spell.ApplyToCaster)
						{
							targ.AddStatusEffect(spell.StatusEffectToApply, SpellSource.isPlayer, dmgBonus, SpellSource.MyChar);
						}
						else
						{
							SpellSource.MyChar.MyStats.AddStatusEffect(spell.StatusEffectToApply, SpellSource.isPlayer, dmgBonus, SpellSource.MyChar);
						}
					}
					if (spell.GroupEffect)
					{
						if (!targ.Myself.isNPC)
						{
							if (GameData.GroupMembers[0] != null && GameData.GroupMembers[0].MyAvatar != null && GameData.GroupMembers[0].MyAvatar.MyStats.CurrentHP > 0 && Vector3.Distance(targ.transform.position, GameData.GroupMembers[0].MyAvatar.transform.position) < 60f)
							{
								if (Vector3.Distance(GameData.GroupMembers[0].MyAvatar.transform.position, GameData.PlayerControl.transform.position) < 40f)
								{
									gameObject = Object.Instantiate(GameData.EffectDB.SpellEffects[spell.SpellResolveFXIndex], GameData.GroupMembers[0].MyAvatar.transform.position, Quaternion.identity);
									gameObject.AddComponent<DestroyObjectTimer>().TimeToDestroy = 600f;
								}
								GameData.GroupMembers[0].MyStats.HealMe(spell, num13, isCrit, _isMana: false, SpellSource.MyChar);
							}
							if (GameData.GroupMembers[1] != null && GameData.GroupMembers[1].MyAvatar != null && GameData.GroupMembers[1].MyAvatar.MyStats.CurrentHP > 0 && Vector3.Distance(targ.transform.position, GameData.GroupMembers[1].MyAvatar.transform.position) < 60f)
							{
								if (Vector3.Distance(GameData.GroupMembers[1].MyAvatar.transform.position, GameData.PlayerControl.transform.position) < 40f)
								{
									gameObject = Object.Instantiate(GameData.EffectDB.SpellEffects[spell.SpellResolveFXIndex], GameData.GroupMembers[1].MyAvatar.transform.position, Quaternion.identity);
									gameObject.AddComponent<DestroyObjectTimer>().TimeToDestroy = 600f;
								}
								GameData.GroupMembers[1].MyStats.HealMe(spell, num13, isCrit, _isMana: false, SpellSource.MyChar);
							}
							if (GameData.GroupMembers[2] != null && GameData.GroupMembers[2].MyAvatar != null && GameData.GroupMembers[2].MyAvatar.MyStats.CurrentHP > 0 && Vector3.Distance(targ.transform.position, GameData.GroupMembers[2].MyAvatar.transform.position) < 60f)
							{
								if (Vector3.Distance(GameData.GroupMembers[2].MyAvatar.transform.position, GameData.PlayerControl.transform.position) < 40f)
								{
									gameObject = Object.Instantiate(GameData.EffectDB.SpellEffects[spell.SpellResolveFXIndex], GameData.GroupMembers[2].MyAvatar.transform.position, Quaternion.identity);
									gameObject.AddComponent<DestroyObjectTimer>().TimeToDestroy = 600f;
								}
								GameData.GroupMembers[2].MyStats.HealMe(spell, num13, isCrit, _isMana: false, SpellSource.MyChar);
							}
							if (GameData.GroupMembers[3] != null && GameData.GroupMembers[3].MyAvatar != null && GameData.GroupMembers[3].MyAvatar.MyStats.CurrentHP > 0 && Vector3.Distance(targ.transform.position, GameData.GroupMembers[3].MyAvatar.transform.position) < 60f)
							{
								if (Vector3.Distance(GameData.GroupMembers[3].MyAvatar.transform.position, GameData.PlayerControl.transform.position) < 40f)
								{
									gameObject = Object.Instantiate(GameData.EffectDB.SpellEffects[spell.SpellResolveFXIndex], GameData.GroupMembers[3].MyAvatar.transform.position, Quaternion.identity);
									gameObject.AddComponent<DestroyObjectTimer>().TimeToDestroy = 600f;
								}
								GameData.GroupMembers[3].MyStats.HealMe(spell, num13, isCrit, _isMana: false, SpellSource.MyChar);
							}
							if ((targ.Myself.MyCharmedNPC != null && targ.Myself.MyCharmedNPC.GetComponent<Stats>().CurrentHP > 0) || (SpellSource.isPlayer && GameData.PlayerControl.Myself.MyCharmedNPC != null))
							{
								if (Vector3.Distance(targ.transform.position, GameData.PlayerControl.transform.position) < 40f)
								{
									gameObject = Object.Instantiate(GameData.EffectDB.SpellEffects[spell.SpellResolveFXIndex], targ.transform.position, Quaternion.identity);
									gameObject.AddComponent<DestroyObjectTimer>().TimeToDestroy = 600f;
								}
								targ.Myself.MyCharmedNPC.GetComponent<Stats>().HealMe(spell, num13, isCrit, _isMana: false, SpellSource.MyChar);
							}
						}
						if (targ.Myself.isNPC && targ.Myself.NearbyFriends.Count > 0)
						{
							foreach (Character nearbyFriend2 in targ.Myself.NearbyFriends)
							{
								if (nearbyFriend2 != null && nearbyFriend2.MyStats.CurrentHP > 0)
								{
									if (Vector3.Distance(targ.transform.position, GameData.PlayerControl.transform.position) < 40f)
									{
										gameObject = Object.Instantiate(GameData.EffectDB.SpellEffects[spell.SpellResolveFXIndex], targ.transform.position, Quaternion.identity);
										gameObject.AddComponent<DestroyObjectTimer>().TimeToDestroy = 600f;
									}
									nearbyFriend2.MyStats.HealMe(spell, num13, isCrit, _isMana: false, SpellSource.MyChar);
								}
							}
						}
					}
				}
				else if (spell.MyDamageType == GameData.DamageType.Magic)
				{
					int amt = spell.Mana;
					if (spell.PercentManaRestoration > 0)
					{
						amt = (int)((float)spell.PercentManaRestoration / 100f * (float)targ.GetCurrentMaxMana());
					}
					targ.HealMe(spell, amt, _isCrit: false, _isMana: true, SpellSource.MyChar);
					if (spell.GroupEffect)
					{
						if (!targ.Myself.isNPC && targ.Myself.Alive)
						{
							if (GameData.GroupMembers[0] != null && GameData.GroupMembers[0].MyAvatar != null && Vector3.Distance(targ.transform.position, GameData.GroupMembers[0].MyAvatar.transform.position) < 60f)
							{
								if (Vector3.Distance(GameData.GroupMembers[0].MyAvatar.transform.position, GameData.PlayerControl.transform.position) < 40f)
								{
									gameObject = Object.Instantiate(GameData.EffectDB.SpellEffects[spell.SpellResolveFXIndex], GameData.GroupMembers[0].MyAvatar.transform.position, Quaternion.identity);
									gameObject.AddComponent<DestroyObjectTimer>().TimeToDestroy = 600f;
								}
								GameData.GroupMembers[0].MyStats.HealMe(spell, amt, _isCrit: false, _isMana: true, SpellSource.MyChar);
							}
							if (GameData.GroupMembers[1] != null && GameData.GroupMembers[1].MyAvatar != null && Vector3.Distance(targ.transform.position, GameData.GroupMembers[1].MyAvatar.transform.position) < 60f)
							{
								if (Vector3.Distance(GameData.GroupMembers[1].MyAvatar.transform.position, GameData.PlayerControl.transform.position) < 40f)
								{
									gameObject = Object.Instantiate(GameData.EffectDB.SpellEffects[spell.SpellResolveFXIndex], GameData.GroupMembers[1].MyAvatar.transform.position, Quaternion.identity);
									gameObject.AddComponent<DestroyObjectTimer>().TimeToDestroy = 600f;
								}
								GameData.GroupMembers[1].MyStats.HealMe(spell, amt, _isCrit: false, _isMana: true, SpellSource.MyChar);
							}
							if (GameData.GroupMembers[2] != null && GameData.GroupMembers[2].MyAvatar != null && Vector3.Distance(targ.transform.position, GameData.GroupMembers[2].MyAvatar.transform.position) < 60f)
							{
								if (Vector3.Distance(GameData.GroupMembers[2].MyAvatar.transform.position, GameData.PlayerControl.transform.position) < 40f)
								{
									gameObject = Object.Instantiate(GameData.EffectDB.SpellEffects[spell.SpellResolveFXIndex], GameData.GroupMembers[2].MyAvatar.transform.position, Quaternion.identity);
									gameObject.AddComponent<DestroyObjectTimer>().TimeToDestroy = 600f;
								}
								GameData.GroupMembers[2].MyStats.HealMe(spell, amt, _isCrit: false, _isMana: true, SpellSource.MyChar);
							}
							if (GameData.GroupMembers[3] != null && GameData.GroupMembers[3].MyAvatar != null && Vector3.Distance(targ.transform.position, GameData.GroupMembers[3].MyAvatar.transform.position) < 60f)
							{
								if (Vector3.Distance(GameData.GroupMembers[3].MyAvatar.transform.position, GameData.PlayerControl.transform.position) < 40f)
								{
									gameObject = Object.Instantiate(GameData.EffectDB.SpellEffects[spell.SpellResolveFXIndex], GameData.GroupMembers[3].MyAvatar.transform.position, Quaternion.identity);
									gameObject.AddComponent<DestroyObjectTimer>().TimeToDestroy = 600f;
								}
								GameData.GroupMembers[3].MyStats.HealMe(spell, amt, _isCrit: false, _isMana: true, SpellSource.MyChar);
							}
							if (targ.Myself.MyCharmedNPC != null && targ.Myself.MyCharmedNPC.GetComponent<Stats>().CurrentHP > 0)
							{
								gameObject = Object.Instantiate(GameData.EffectDB.SpellEffects[spell.SpellResolveFXIndex], targ.Myself.MyCharmedNPC.transform.position, Quaternion.identity);
								gameObject.AddComponent<DestroyObjectTimer>().TimeToDestroy = 600f;
								targ.Myself.MyCharmedNPC.GetComponent<Stats>().HealMe(spell, amt, _isCrit: false, _isMana: true, SpellSource.MyChar);
							}
						}
						if (targ.Myself.isNPC && targ.Myself.NearbyFriends.Count > 0)
						{
							foreach (Character nearbyFriend3 in targ.Myself.NearbyFriends)
							{
								if (nearbyFriend3 != null && nearbyFriend3.MyStats.CurrentHP > 0)
								{
									if (Vector3.Distance(nearbyFriend3.transform.position, GameData.PlayerControl.transform.position) < 40f)
									{
										gameObject = Object.Instantiate(GameData.EffectDB.SpellEffects[spell.SpellResolveFXIndex], nearbyFriend3.transform.position, Quaternion.identity);
										gameObject.AddComponent<DestroyObjectTimer>().TimeToDestroy = 600f;
									}
									nearbyFriend3.MyStats.HealMe(spell, amt, _isCrit: false, _isMana: true, SpellSource.MyChar);
								}
							}
						}
					}
				}
				else if (targ.StatusEffects.Length != 0)
				{
					StatusEffect[] statusEffects = targ.StatusEffects;
					foreach (StatusEffect statusEffect in statusEffects)
					{
						if (statusEffect != null && statusEffect.Effect != null && statusEffect.Effect.MyDamageType == spell.MyDamageType)
						{
							statusEffect.Duration -= spell.TargetHealing;
						}
					}
				}
			}
			if (Random.Range(0, 100) >= SpellSource.MyChar.MyStats.GetCurrentRes() || !(SpellSource.MyChar.MyStats.resonanceCD <= 0f) || spell.NoResonate)
			{
				break;
			}
			if (SpellSource.isPlayer)
			{
				UpdateSocialLog.CombatLogAdd("Your spell resonates and casts again!", "yellow");
			}
			else if (!(num2 < 15f))
			{
				_ = targ.Myself.isNPC;
			}
			if (SpellSource.MyChar.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByName("Arcane Recovery")))
			{
				SpellSource.MyChar.MyStats.CurrentMana += Mathf.RoundToInt((float)spell.ManaCost * Random.Range(0.15f, 0.65f));
				if (SpellSource.MyChar.MyStats.CurrentMana > SpellSource.MyChar.MyStats.GetCurrentMaxMana())
				{
					SpellSource.MyChar.MyStats.CurrentMana = SpellSource.MyChar.MyStats.GetCurrentMaxMana();
				}
				if (SpellSource.isPlayer)
				{
					UpdateSocialLog.CombatLogAdd("You feel a surge of returning mana.", "yellow");
				}
				else if (!(num2 < 15f))
				{
					_ = targ.Myself.isNPC;
				}
			}
			if (!isProc)
			{
				SpellSource.StartSpell(spell, targ, 1.1f, _resonate: true);
			}
			else
			{
				SpellSource.StartSpellFromProc(spell, targ, 1f, _resonating: true);
			}
			SpellSource.MyChar.MyStats.resonanceCD = 60f;
			break;
		}
		case Spell.SpellType.Pet:
			gameObject = Object.Instantiate(GameData.EffectDB.SpellEffects[spell.SpellResolveFXIndex], SpellSource.transform.position + new Vector3(0f, 0f, -1.5f), base.transform.rotation);
			ScaleParticles(gameObject);
			if (Vector3.Distance(gameObject.transform.position, GameData.PlayerControl.transform.position) > 40f)
			{
				Object.Destroy(gameObject);
			}
			else if (Vector3.Distance(targ.transform.position, GameData.PlayerControl.transform.position) < 40f)
			{
				gameObject = Object.Instantiate(GameData.EffectDB.SpellEffects[spell.SpellResolveFXIndex], targ.transform.position, Quaternion.identity);
				gameObject.AddComponent<DestroyObjectTimer>().TimeToDestroy = 600f;
			}
			if (SpellSource.MyChar.MyCharmedNPC == null)
			{
				Vector3? vector = null;
				vector = GameData.GetSafeNavMeshPoint(SpellSource.transform.position);
				if (!vector.HasValue)
				{
					if (NavMesh.SamplePosition(base.transform.position, out var hit, 5f, -1))
					{
						vector = hit.position;
					}
					if (!vector.HasValue)
					{
						vector = SpellSource.transform.position;
					}
				}
				GameObject gameObject2 = Object.Instantiate(spell.PetToSummon, vector.Value, base.transform.rotation);
				SpellSource.MyChar.MyCharmedNPC = gameObject2.GetComponent<NPC>();
				gameObject2.GetComponent<Character>().Master = SpellSource.MyChar;
				SpellSource.MyChar.MyCharmedNPC.SummonedByPlayer = true;
				SpellSource.MyChar.MyCharmedNPC.GetComponent<Stats>().Level += SpellSource.MyChar.MySkills.GetAscensionRank("34995747");
				if (SpellSource.isPlayer)
				{
					UpdateSocialLog.LogAdd("You've summoned a companion!", "lightblue");
				}
				else
				{
					gameObject2.GetComponent<NPC>().NPCName = SpellSource.transform.name + "'s pet";
				}
			}
			else
			{
				UpdateSocialLog.LogAdd("You cannot have two summoned companions!", "lightblue");
			}
			break;
		case Spell.SpellType.Misc:
			DoMiscSpells();
			break;
		}
		if (flag)
		{
			CastSpell spellSource = SpellSource;
			if ((object)spellSource != null && spellSource.MyChar?.MyStats?.CombatStance?.SelfDamagePerCast > 0f)
			{
				SpellSource.MyChar.SelfDamageMe(SpellSource.MyChar.MyStats.CombatStance.SelfDamagePerCast);
			}
		}
		if (spell.CompleteVariations.Count <= 0)
		{
			if (SpellSource.isPlayer)
			{
				if (!isProc)
				{
					SpellSource.MyChar.MyAudio.PlayOneShot(spell.CompleteSound, SpellSource.MyChar.MyAudio.volume * 0.5f * GameData.SpellVol * GameData.MasterVol);
				}
				else
				{
					SpellSource.MyChar.MyAudio.PlayOneShot(spell.CompleteSound, SpellSource.MyChar.MyAudio.volume * 0.3f * GameData.SpellVol * GameData.MasterVol);
				}
			}
			else if (targ.Myself.MyAudio != null)
			{
				targ.Myself.MyAudio.PlayOneShot(spell.CompleteSound, targ.Myself.MyAudio.volume * 0.4f * GameData.SpellVol * GameData.MasterVol);
			}
			else
			{
				SpellSource.MyChar.MyAudio.PlayOneShot(spell.CompleteSound, SpellSource.MyChar.MyAudio.volume * 0.4f * GameData.SpellVol * GameData.MasterVol);
			}
		}
		if (totalLife > 1f)
		{
			CheckBracerProcs();
		}
		if (spell.InflictOnSelf)
		{
			int num15 = SpellSource.MyChar.SelfDamageMeFlat(-spell.CasterHealing);
			if (SpellSource.isPlayer)
			{
				UpdateSocialLog.CombatLogAdd("You inflict " + num15 + " damage upon yourself!", "lightblue");
			}
			else if (Vector3.Distance(GameData.PlayerControl.transform.position, SpellSource.transform.position) < 10f)
			{
				UpdateSocialLog.CombatLogAdd(SpellSource.transform.name + " inflicts " + num15 + " damage upon themself!", "lightblue");
			}
		}
		EndSpell();
	}

	private void EndSpell()
	{
		SpellSource.MyChar.MyStats.RecentCast = 480f;
		if (SpellSource.isPlayer && SpellSource.MyChar != null && SpellSource.MyChar.MyStats != null && !SpellSource.MyChar.MyStats.Charmed)
		{
			if (spell.AutomateAttack && GameData.GM.AutoEngageAttackOnSkill)
			{
				GameData.PlayerCombat.ForceAttackOn();
			}
			foreach (Hotkeys allHotkey in GameData.GM.HKManager.AllHotkeys)
			{
				if (isProc)
				{
					continue;
				}
				if (allHotkey.AssignedSpell == spell || (allHotkey.AssignedItem != null && allHotkey.AssignedItem.MyItem.ItemEffectOnClick == spell))
				{
					if (allHotkey.Cooldown <= spell.Cooldown * 60f * CDMult * scaleDmg)
					{
						allHotkey.Cooldown = spell.Cooldown * 60f * CDMult * scaleDmg;
					}
					if (SpellSource.MyChar.MySkills != null)
					{
						allHotkey.Cooldown -= spell.Cooldown * 60f * ((float)SpellSource.MyChar.MySkills.GetAscensionRank("7758218") * 0.1f) * CDMult;
					}
				}
				else if (allHotkey.Cooldown < 2f)
				{
					allHotkey.Cooldown = 20f;
				}
			}
		}
		if (AECollector != null)
		{
			Object.Destroy(AECollector.gameObject);
		}
		if (!isProc)
		{
			SpellSource.Reset();
		}
		overChant.x = 0f;
		overChantLife = 0f;
		totalLife = 0f;
		if (SpellSource.isPlayer && !isProc && (bool)GameData.CB.OCBarRect)
		{
			GameData.CB.OCBarRect.sizeDelta = overChant;
		}
		if (!SpellSource.isPlayer)
		{
			CastSpell spellSource = SpellSource;
			bool? obj;
			if ((object)spellSource == null)
			{
				obj = null;
			}
			else
			{
				Character myChar = spellSource.MyChar;
				if ((object)myChar == null)
				{
					obj = null;
				}
				else
				{
					NPC myNPC = myChar.MyNPC;
					obj = (((object)myNPC != null) ? new bool?(!myNPC.SimPlayer) : null);
				}
			}
			bool? flag = obj;
			if (flag.GetValueOrDefault())
			{
				SpellSource.MyChar.MyNPC.NPCSpellCooldown = Random.Range(120, 360);
			}
		}
		if ((bool)ChargeFX)
		{
			ChargeFX.GetComponent<ParticleSystem>().Stop();
		}
		if (CastBar != null && !isProc)
		{
			castScale = new Vector3(0f, 0.1f, 1f);
			CastBar.localScale = castScale;
		}
		Object.Destroy(base.gameObject);
	}

	public void EndSpellNoCD()
	{
		if (SpellSource.isPlayer && SpellSource.MyChar != null && SpellSource.MyChar.MyStats != null && !SpellSource.MyChar.MyStats.Charmed)
		{
			foreach (Hotkeys allHotkey in GameData.GM.HKManager.AllHotkeys)
			{
				if (!isProc)
				{
					if (allHotkey.AssignedSpell == spell || (allHotkey.AssignedItem != null && allHotkey.AssignedItem.MyItem.ItemEffectOnClick == spell))
					{
						allHotkey.Cooldown = 20f;
					}
					else if (allHotkey.Cooldown < 2f)
					{
						allHotkey.Cooldown = 20f;
					}
				}
			}
		}
		overChant.x = 0f;
		if (SpellSource.isPlayer && !isProc)
		{
			if ((bool)GameData.CB.OCBarRect)
			{
				GameData.CB.OCBarRect.sizeDelta = overChant;
			}
		}
		else if (!SpellSource.isPlayer)
		{
			CastSpell spellSource = SpellSource;
			bool? obj;
			if ((object)spellSource == null)
			{
				obj = null;
			}
			else
			{
				Character myChar = spellSource.MyChar;
				if ((object)myChar == null)
				{
					obj = null;
				}
				else
				{
					NPC myNPC = myChar.MyNPC;
					obj = (((object)myNPC != null) ? new bool?(!myNPC.SimPlayer) : null);
				}
			}
			bool? flag = obj;
			if (flag.GetValueOrDefault())
			{
				SpellSource.MyChar.MyNPC.NPCSpellCooldown = Random.Range(260, 420);
			}
		}
		if (CastBar != null && !isProc)
		{
			npcCastScale.x = EffectLife / totalLife * 0.7f;
			CastBar.localScale = npcCastScale;
			CastBar.localScale = castScale;
		}
		if (AECollector != null)
		{
			Object.Destroy(AECollector.gameObject);
		}
		if (!isProc)
		{
			SpellSource.Reset();
		}
		ChargeFX.GetComponent<ParticleSystem>().Stop();
		Object.Destroy(base.gameObject);
	}

	private void OnDestroy()
	{
		if (CastBar != null && !isProc)
		{
			castScale = new Vector3(0f, 0.1f, 1f);
			overChant.x = 0f;
			CastBar.localScale = castScale;
		}
		if (SpellSource.isPlayer && !isProc && (bool)GameData.CB.OCBarRect)
		{
			GameData.CB.OCBarRect.sizeDelta = overChant;
		}
	}

	private void DoMiscSpells()
	{
		switch (spell.SpellName)
		{
		case "Invoke Storm":
			UpdateSocialLog.LogAdd("You " + spell.StatusEffectMessageOnPlayer, "lightblue");
			GameData.Atmos.SinceRain = -1f;
			GameData.Atmos.WeightGoal = 0.9f;
			GameData.Atmos.CloudAmt = 0.4f;
			break;
		case "Dissipate Storm":
			UpdateSocialLog.LogAdd("You " + spell.StatusEffectMessageOnPlayer, "lightblue");
			GameData.Atmos.WeightGoal = 0f;
			GameData.Atmos.CloudAmt = 0.1f;
			break;
		case "Portal to Windwashed":
			SetAchievement.Unlock("TELEPORT");
			SimPlayerDataManager.SaveAllSimData();
			GameData.SceneChange.ChangeScene("Windwashed", new Vector3(755.27f, 66f, 474.4f), _useSun: true, 90f);
			break;
		case "Portal to Silkengrass":
			SetAchievement.Unlock("TELEPORT");
			SimPlayerDataManager.SaveAllSimData();
			GameData.SceneChange.ChangeScene("Silkengrass", new Vector3(188.5f, 63.52f, 712.92f), _useSun: true, 180f);
			break;
		case "Portal to Braxonian":
			SetAchievement.Unlock("TELEPORT");
			SimPlayerDataManager.SaveAllSimData();
			GameData.SceneChange.ChangeScene("Braxonian", new Vector3(382.6f, 49.3f, 878f), _useSun: true, 180f);
			break;
		case "Portal to Soluna's Landing":
			SetAchievement.Unlock("TELEPORT");
			SimPlayerDataManager.SaveAllSimData();
			GameData.SceneChange.ChangeScene("Soluna", new Vector3(225f, 77f, 249f), _useSun: true, 180f);
			break;
		case "Portal to Ripper's Keep":
			SetAchievement.Unlock("TELEPORT");
			SimPlayerDataManager.SaveAllSimData();
			GameData.SceneChange.ChangeScene("Ripper", new Vector3(572f, 54.4f, 293f), _useSun: true, 180f);
			break;
		case "Portal to Hidden":
			SetAchievement.Unlock("TELEPORT");
			SimPlayerDataManager.SaveAllSimData();
			GameData.SceneChange.ChangeScene("Hidden", new Vector3(9.34f, 1f, -114.33f), _useSun: true, 180f);
			break;
		case "Returning Wish":
			SetAchievement.Unlock("TELEPORT");
			SimPlayerDataManager.SaveAllSimData();
			GameData.SceneChange.ChangeScene(GameData.BindZone, GameData.BindLoc, GameData.SunInBindZone, 180f);
			break;
		case "Portal to Reliquary":
			SetAchievement.Unlock("TELEPORT");
			GameData.ReliqDest = SceneManager.GetActiveScene().name;
			GameData.ReliqLanding = GameData.PlayerControl.transform.position;
			if (GameData.usingSun)
			{
				GameData.SunInReliqZone = 1;
			}
			else
			{
				GameData.SunInReliqZone = 0;
			}
			SimPlayerDataManager.SaveAllSimData();
			GameData.SceneChange.ChangeScene("Reliquary", new Vector3(275f, 1.82f, 309f), _useSun: false, 0f);
			break;
		case "Break Fossil":
		{
			Item item = GameData.Misc.FossilGame[Random.Range(0, GameData.Misc.FossilGame.Count)];
			UpdateSocialLog.LogAdd("You smash the Braxonian Fossil to reveal the contents inside...", "lightblue");
			UpdateSocialLog.LogAdd("It's a " + item.ItemName + "...", "lightblue");
			if (!GameData.PlayerInv.AddItemToInv(item))
			{
				GameData.PlayerInv.ForceItemToInv(item);
			}
			break;
		}
		case "Wyrm's Bane":
		{
			Character currentTarget = GameData.PlayerControl.CurrentTarget;
			if (currentTarget.IsWyrm)
			{
				LootTable component = currentTarget.GetComponent<LootTable>();
				component.GuaranteeOneDrop.Clear();
				component.CommonDrop.Clear();
				component.UncommonDrop.Clear();
				component.RareDrop.Clear();
				component.LegendaryDrop.Clear();
				component.ActualDrops.Clear();
				component.MinGold = 0;
				component.MaxGold = 0;
				component.MyGold = 0;
				currentTarget.BossXp = 0f;
				currentTarget.MyStats.CurrentHP = -1;
				UpdateSocialLog.LogAdd("The Wyrm succumbs to the Wyrm's Bane", "lightblue");
			}
			else
			{
				UpdateSocialLog.LogAdd("This target was not a Wyrm.", "lightblue");
			}
			break;
		}
		case "Dimensional Rift":
			if (SceneManager.GetActiveScene().name == "Azynthi")
			{
				SimPlayerDataManager.SaveAllSimData();
				GameData.SceneChange.ChangeScene("AzynthiClear", base.transform.position, _useSun: true, 180f);
			}
			else if (SceneManager.GetActiveScene().name == "AzynthiClear")
			{
				SimPlayerDataManager.SaveAllSimData();
				GameData.SceneChange.ChangeScene("Azynthi", base.transform.position, _useSun: false, 180f);
			}
			else
			{
				UpdateSocialLog.LogAdd("Nothing happens...", "grey");
			}
			break;
		case "Time Stone":
			if (SceneManager.GetActiveScene().name == "ShiveringTomb2")
			{
				UpdateSocialLog.LogAdd("Time does not exist here... your Time Stone returns to your hand.", "grey");
				GameData.PlayerInv.AddItemToInv(GameData.ItemDB.GetItemByID("2936548"));
			}
			else
			{
				if (SpawnPointManager.SpawnPointsInScene.Count <= 0)
				{
					break;
				}
				{
					foreach (SpawnPoint item2 in SpawnPointManager.SpawnPointsInScene)
					{
						if (!item2.MyNPCAlive)
						{
							item2.actualSpawnDelay = Random.Range(1, 60);
						}
					}
					break;
				}
			}
			break;
		case "Light of the Lantern":
			if ((GameData.usingSun || SceneManager.GetActiveScene().name == "Brake" || SceneManager.GetActiveScene().name == "Loomingwood" || SceneManager.GetActiveScene().name == "Rottenfoot") && SceneManager.GetActiveScene().name != "Azynthi" && SceneManager.GetActiveScene().name != "Underspine" && SceneManager.GetActiveScene().name != "PrielPlateau" && SceneManager.GetActiveScene().name != "Krakengard")
			{
				bool flag = false;
				foreach (SpawnPoint item3 in SpawnPointManager.SpawnPointsInScene)
				{
					if (item3 != null && Vector3.Distance(item3.transform.position, base.transform.position) < 20f)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					GameData.GM.GetComponent<SetBind>().SetBindPoint();
				}
				else
				{
					UpdateSocialLog.LogAdd("Another being calls this spot home... your lantern flickers and fails. Try moving somewhere else.", "yellow");
				}
			}
			else
			{
				UpdateSocialLog.LogAdd("Cannot use this item here...", "yellow");
			}
			break;
		case "Offering Stone":
			if (!GameData.PlayerInv.AddItemToInv(GameData.ItemDB.GetItemByID("340104")))
			{
				GameData.PlayerInv.ForceItemToInv(GameData.ItemDB.GetItemByID("340104"));
			}
			break;
		case "Read Map":
			GameData.GM.GetComponent<TreasureHunting>().SetTreasureZone();
			break;
		case "Repel Darkness":
			if (RenderSettings.fogMode == FogMode.Linear)
			{
				RenderSettings.fogStartDistance += 100f;
				RenderSettings.fogEndDistance += 200f;
				if (RenderSettings.fogStartDistance > 200f)
				{
					RenderSettings.fogStartDistance = 200f;
				}
				if (RenderSettings.fogEndDistance > 3500f)
				{
					RenderSettings.fogEndDistance = 3500f;
				}
			}
			break;
		case "Restore Balance":
			GameData.GuildManager.TopGuildAngryAtReset = true;
			UpdateSocialLog.LogAdd("The Spirits of Erenshor have chosen Balance...", "lightblue");
			{
				foreach (LiveGuildData guild in GameData.GuildManager.Guilds)
				{
					if (guild.GuildScore > 0)
					{
						guild.GuildScore = Mathf.RoundToInt(guild.GuildScore / 2);
					}
				}
				break;
			}
		}
	}

	private void CheckBracerProcs()
	{
		Spell spell = null;
		if (SpellSource.isPlayer)
		{
			foreach (ItemIcon equipmentSlot in GameData.PlayerInv.EquipmentSlots)
			{
				if (equipmentSlot.MyItem.RequiredSlot == Item.SlotType.Bracer && equipmentSlot.MyItem.WeaponProcOnHit != null && (float)Random.Range(0, 100) < equipmentSlot.MyItem.WeaponProcChance)
				{
					spell = equipmentSlot.MyItem.WeaponProcOnHit;
					break;
				}
			}
		}
		else if (SpellSource.isSimPlayer)
		{
			foreach (SimInvSlot item in SpellSource.MyChar.MyNPC.ThisSim.MyEquipment)
			{
				if (item.MyItem.RequiredSlot == Item.SlotType.Bracer && item.MyItem.WeaponProcOnHit != null && (float)Random.Range(0, 100) < item.MyItem.WeaponProcChance)
				{
					spell = item.MyItem.WeaponProcOnHit;
					break;
				}
			}
		}
		if (!(spell != null))
		{
			return;
		}
		if (SpellSource.isPlayer)
		{
			UpdateSocialLog.LogAdd("Your bracer begins to glow!", "yellow");
		}
		if (spell.Type == Spell.SpellType.Damage || spell.Type == Spell.SpellType.AE || spell.Type == Spell.SpellType.PBAE || spell.Type == Spell.SpellType.StatusEffect)
		{
			if (targ != SpellSource && targ.Myself.MyFaction != 0 && targ.Myself.MyFaction != Character.Faction.PC)
			{
				SpellSource.StartSpellFromProc(spell, targ, 1f, _resonating: true);
			}
			else if (SpellSource != null && SpellSource.isPlayer)
			{
				UpdateSocialLog.LogAdd("The glow fizzes... (invalid target)", "yellow");
			}
		}
		else if (spell.Type == Spell.SpellType.Heal || spell.Type == Spell.SpellType.Beneficial)
		{
			SpellSource.StartSpellFromProc(spell, SpellSource.MyChar.MyStats, 1f, _resonating: true);
		}
	}

	private int CalcDmgBonus(int _baseDamage)
	{
		float num = _baseDamage;
		if (resonating)
		{
			num *= 0.3f;
			if (SpellSource.MyChar != null && Random.Range(0, 100) < SpellSource.MyChar.MySkills.GetAscensionRank("32723648") * 30)
			{
				int num2 = SpellSource.MyChar.MyStats.GetCurrentRes() - 100;
				if (num2 > 0)
				{
					if (Vector3.Distance(SpellSource.transform.position, GameData.PlayerControl.transform.position) < 15f)
					{
						UpdateSocialLog.CombatLogAdd("A ROARING ECHO courses through the air");
					}
					num = _baseDamage;
					num += (float)(_baseDamage * (num2 / 100));
				}
			}
		}
		int num3 = Mathf.RoundToInt(3f * (float)SpellSource.MyChar.MyStats.GetCurrentInt());
		int num4 = Mathf.RoundToInt((float)SpellSource.MyChar.MyStats.Level / 8f * ((float)SpellSource.MyChar.MyStats.IntScaleMod / 100f * ((float)num3 + num)));
		return Mathf.RoundToInt(num + (float)num4);
	}

	private bool CheckForCharmGroupEffect(Character _source)
	{
		if (!_source.isNPC || !_source.MyNPC.SimPlayer)
		{
			_ = _source.isNPC;
		}
		return false;
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
}
