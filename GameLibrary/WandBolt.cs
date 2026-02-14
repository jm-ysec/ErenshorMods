// WandBolt
using UnityEngine;

public class WandBolt : MonoBehaviour
{
	public Character SourceChar;

	public Character TargetChar;

	public int Dmg;

	public Spell Proc;

	public float MoveSpeed;

	public GameData.DamageType DmgType = GameData.DamageType.Magic;

	public ParticleSystem MyParticle;

	public AudioSource MyAud;

	private AudioClip AtkSound;

	private float moveDel = 40f;

	private bool didSFX;

	private int dmgMod = 1;

	private bool interrupt;

	private bool forceEffectOntoTarget;

	private bool scaledIncoming;

	private void Start()
	{
		MyAud = GetComponent<AudioSource>();
		MyAud.volume = GameData.SFXVol * MyAud.volume * GameData.MasterVol;
	}

	private void Update()
	{
		if (!TargetChar || !SourceChar)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		if (moveDel <= 0f)
		{
			Vector3 normalized = (TargetChar.transform.position + Vector3.up - base.transform.position).normalized;
			if (Vector3.Distance(base.transform.position, TargetChar.transform.position + Vector3.up) > 2f)
			{
				base.transform.position += normalized * MoveSpeed * Time.deltaTime;
			}
			else
			{
				DeliverDamage();
			}
			MoveSpeed += 10f * Time.deltaTime;
			return;
		}
		moveDel -= 60f * Time.deltaTime;
		if (moveDel < 15f && !didSFX)
		{
			if (AtkSound != null && SourceChar != null)
			{
				SourceChar.MyAudio.PlayOneShot(AtkSound, SourceChar.MyAudio.volume * GameData.SFXVol * GameData.MasterVol);
			}
			didSFX = true;
		}
		if (moveDel <= 0f)
		{
			base.transform.position = SourceChar.transform.position + base.transform.forward + Vector3.up;
			if (Vector3.Distance(base.transform.position, TargetChar.transform.position + Vector3.up) > 5f)
			{
				MyAud.Play();
			}
		}
	}

	public void DisableCrits()
	{
		scaledIncoming = true;
	}

	private void DeliverDamage()
	{
		string text = "WAND BOLT";
		if (TargetChar == null || SourceChar == null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		if (TargetChar.Invulnerable)
		{
			if (!SourceChar.isNPC)
			{
				UpdateSocialLog.CombatLogAdd("You try to hit " + TargetChar.name + ", but " + TargetChar.name + " is INVULNERABLE!");
			}
			Object.Destroy(base.gameObject);
			return;
		}
		int num = 0;
		int num2 = 0;
		if (DmgType == GameData.DamageType.Magic)
		{
			num2 = CalcDmgBonusWand(Dmg, SourceChar);
			num = TargetChar.MagicDamageMe(num2, !SourceChar.isNPC, DmgType, SourceChar, 0f, num2);
		}
		if (DmgType == GameData.DamageType.Physical)
		{
			int atkRollBonus = 0;
			if (TargetChar.MyStats.CheckForHigherLevelSEFromMeToRefresh(GameData.SpellDatabase.GetSpellByID("31539688"), SourceChar))
			{
				atkRollBonus = 5;
			}
			bool flag = SourceChar.MyStats.isCriticalAttack();
			if (scaledIncoming)
			{
				flag = false;
			}
			num2 = SourceChar.MyStats.CalcBowDamage(Dmg, TargetChar.MyStats.Level, TargetChar.MyStats, atkRollBonus);
			num2 *= dmgMod;
			if (flag)
			{
				num2 = Mathf.RoundToInt((float)num2 * 1.5f);
			}
			num = TargetChar.DamageMe(num2, !SourceChar.isNPC, DmgType, SourceChar, _animEffect: false, flag);
			if (interrupt)
			{
				TargetChar.transform.position += new Vector3(0.2f, 0f, 0.2f);
			}
			text = "ARROW";
			if (num > 0 && flag && !SourceChar.isNPC)
			{
				UpdateSocialLog.CombatLogAdd("You score a critical hit!", "lightblue");
				GameData.CamControl.ShakeScreen(2f, 0.1f);
			}
		}
		switch (num)
		{
		case -1:
			if (!SourceChar.isNPC)
			{
				UpdateSocialLog.CombatLogAdd("You try to hit " + TargetChar.name + ", but " + TargetChar.name + " is INVULNERABLE!");
			}
			Object.Destroy(base.gameObject);
			return;
		case -5:
			if (TargetChar.MyNPC.MiningNode)
			{
				GameData.PlayerCombat.TryMine(TargetChar);
				Object.Destroy(base.gameObject);
				return;
			}
			break;
		}
		if (num == -6 && TargetChar != null && TargetChar.isNPC && TargetChar.MyNPC.MyChestEvent != null)
		{
			TargetChar.MyNPC.MyChestEvent.SpawnGuardians(GameData.PlayerStats.Level);
			Object.Destroy(base.gameObject);
			return;
		}
		if (num > 0)
		{
			if (SourceChar.MySkills.GetAscensionRank("64089634") * 10 > Random.Range(0, 100))
			{
				SourceChar.MySpells.StartSpellFromProc(GameData.SpellDatabase.GetSpellByID("12644334"), TargetChar.MyStats, 1f);
			}
			if (SourceChar.MySkills.GetAscensionRank("54315714") * 3 > Random.Range(0, 101) && TargetChar.MyStats.CurrentMaxHP > 0 && (float)TargetChar.MyStats.CurrentHP / (float)TargetChar.MyStats.CurrentMaxHP <= 0.15f && TargetChar.BossXp <= 1f)
			{
				TargetChar.MyStats.CurrentHP = 0;
				if (!SourceChar.isNPC)
				{
					UpdateSocialLog.LogAdd("You execute a FINALE!", "lightblue");
				}
				else
				{
					UpdateSocialLog.CombatLogAdd(SourceChar.transform.name + " executes a FINALE!", "lightblue");
				}
			}
		}
		if (!SourceChar.isNPC)
		{
			TargetChar.FlagForFactionHit(_bool: true);
			if (num > 0)
			{
				UpdateSocialLog.CombatLogAdd("Your " + text + " did " + num + " damage to " + TargetChar.transform.name);
			}
			else if (num == 0)
			{
				UpdateSocialLog.CombatLogAdd("You try to hit " + TargetChar.transform.name + " but your " + text + " missed!");
			}
		}
		if (Proc != null)
		{
			int num3 = 0;
			if (Proc.Type == Spell.SpellType.AE || Proc.Type == Spell.SpellType.PBAE)
			{
				num3 = 3;
			}
			bool flag2 = Proc.Type == Spell.SpellType.StatusEffect;
			if (forceEffectOntoTarget && flag2)
			{
				TargetChar.MyStats.AddStatusEffectNoChecks(Proc, SourceChar.isNPC, 0, SourceChar);
			}
			else
			{
				SourceChar.MySpells.StartSpellFromProc(Proc, TargetChar.MyStats, num3);
			}
		}
		Object.Destroy(base.gameObject);
	}

	public void LoadWandBolt(int _dmg, Spell _proc, Character _tar, Character _caster, float _speed, GameData.DamageType _dmgType, Color _boltCol, AudioClip _atkSound)
	{
		forceEffectOntoTarget = false;
		interrupt = false;
		dmgMod = 1;
		if (_atkSound != null)
		{
			AtkSound = _atkSound;
		}
		Dmg = _dmg;
		if (_proc != null)
		{
			Proc = _proc;
		}
		TargetChar = _tar;
		SourceChar = _caster;
		MoveSpeed = _speed;
		DmgType = _dmgType;
		ParticleSystem.MainModule main = MyParticle.main;
		main.startColor = _boltCol;
	}

	public void LoadArrow(int _dmg, Spell _proc, Character _tar, Character _caster, float _speed, GameData.DamageType _dmgType, AudioClip _atkSound)
	{
		forceEffectOntoTarget = false;
		interrupt = false;
		dmgMod = 1;
		AtkSound = _atkSound;
		Dmg = _dmg;
		if (_proc != null)
		{
			Proc = _proc;
		}
		TargetChar = _tar;
		SourceChar = _caster;
		MoveSpeed = _speed;
		DmgType = _dmgType;
	}

	public void LoadArrow(int _dmg, Spell _proc, Character _tar, Character _caster, float _speed, GameData.DamageType _dmgType, bool _forceEffectOnTarget, AudioClip _atkSound)
	{
		forceEffectOntoTarget = _forceEffectOnTarget;
		interrupt = false;
		dmgMod = 1;
		AtkSound = _atkSound;
		Dmg = _dmg;
		if (_proc != null)
		{
			Proc = _proc;
		}
		TargetChar = _tar;
		SourceChar = _caster;
		MoveSpeed = _speed;
		DmgType = _dmgType;
	}

	public void LoadArrow(int _dmg, Spell _proc, Character _tar, Character _caster, float _speed, GameData.DamageType _dmgType, AudioClip _atkSound, bool _interrupt)
	{
		forceEffectOntoTarget = false;
		interrupt = _interrupt;
		dmgMod = 1;
		AtkSound = _atkSound;
		Dmg = _dmg;
		if (_proc != null)
		{
			Proc = _proc;
		}
		TargetChar = _tar;
		SourceChar = _caster;
		MoveSpeed = _speed;
		DmgType = _dmgType;
	}

	public void LoadArrow(int _dmg, Spell _proc, Character _tar, Character _caster, float _speed, GameData.DamageType _dmgType, AudioClip _atkSound, int _dmgMod)
	{
		forceEffectOntoTarget = false;
		interrupt = false;
		dmgMod = _dmgMod;
		AtkSound = _atkSound;
		Dmg = _dmg;
		if (_proc != null)
		{
			Proc = _proc;
		}
		TargetChar = _tar;
		SourceChar = _caster;
		MoveSpeed = _speed;
		DmgType = _dmgType;
	}

	private int CalcDmgBonusWand(int _baseDamage, Character _caster)
	{
		float num = _baseDamage;
		num *= 0.3f;
		int num2 = Mathf.RoundToInt(3f * (float)_caster.MyStats.GetCurrentInt());
		int num3 = Mathf.RoundToInt((float)_caster.MyStats.Level / 8f * ((float)_caster.MyStats.IntScaleMod / 100f * ((float)num2 + num)));
		return Mathf.RoundToInt(num + (float)num3);
	}
}
