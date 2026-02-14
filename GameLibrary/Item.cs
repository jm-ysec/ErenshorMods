// Item
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item", order = 1)]
public class Item : BaseScriptableObject
{
	public enum SlotType
	{
		General,
		Head,
		Neck,
		Chest,
		Shoulder,
		Arm,
		Bracer,
		Ring,
		Hand,
		Foot,
		Leg,
		Back,
		Waist,
		Primary,
		Secondary,
		PrimaryOrSecondary,
		Aura,
		Charm
	}

	public enum WeaponType
	{
		None,
		OneHandMelee,
		TwoHandMelee,
		OneHandDagger,
		TwoHandStaff,
		TwoHandBow
	}

	public enum FuelTier
	{
		one,
		two,
		three,
		four,
		five
	}

	public string ItemName;

	public string EquipmentToActivate;

	public string ShoulderTrimL;

	public string ShoulderTrimR;

	public string ElbowTrimL;

	public string ElbowTrimR;

	public string KneeTrimL;

	public string KneeTrimR;

	public int ItemLevel;

	public List<Class> Classes;

	public Sprite ItemIcon;

	public int HP;

	public int AC;

	public int Mana;

	public int WeaponDmg;

	public float WeaponDly;

	public int Str;

	public int End;

	public int Dex;

	public int Agi;

	public int Int;

	public int Wis;

	public int Cha;

	public int Res;

	public int MR;

	public int ER;

	public int PR;

	public int VR;

	public SlotType RequiredSlot;

	public Color ItemPrimaryColor;

	public Color ItemSecondaryColor;

	public Color ItemMetalPrimary;

	public Color ItemLeatherPrimary;

	public Color ItemMetalDark;

	public Color ItemMetalSecondary;

	public Color ItemLeatherSecondary;

	public WeaponType ThisWeaponType;

	public int ItemValue;

	[TextArea(5, 20)]
	public string Lore;

	public bool Shield;

	public Spell TeachSpell;

	public Spell ItemEffectOnClick;

	public Spell WeaponProcOnHit;

	public Skill ItemSkillUse;

	public float WeaponProcChance;

	public Skill TeachSkill;

	public float SpellCastTime;

	public AudioClip AttackSound;

	public bool HideHairWhenEquipped;

	public bool HideHeadWhenEquipped;

	public bool Stackable;

	public bool Disposable;

	public Quest AssignQuestOnRead;

	public Quest CompleteOnRead;

	public bool Unique;

	public int Mining;

	public bool FuelSource;

	public bool Template;

	public List<Item> TemplateIngredients;

	public List<Item> TemplateRewards;

	public bool SimPlayersCantGet;

	public bool FurnitureSet;

	public FuelTier FuelLevel;

	public Spell Aura;

	public Spell WornEffect;

	public bool IsWand;

	public Spell WandEffect;

	public float WandProcChance;

	public Color WandBoltColor;

	public int WandRange;

	public float WandBoltSpeed;

	public AudioClip WandAttackSound;

	public bool IsBow;

	public Spell BowEffect;

	public float BowProcChance;

	public int BowRange;

	public float BowArrowSpeed;

	public AudioClip BowAttackSound;

	public bool NoTradeNoDestroy;

	public bool Relic;

	public bool RareItem;

	public string BookTitle;

	public float StrScaling;

	public float EndScaling;

	public float DexScaling;

	public float AgiScaling;

	public float IntScaling;

	public float WisScaling;

	public float ChaScaling;

	public float ResistScaling;

	public float MitigationScaling;

	public static implicit operator List<object>(Item v)
	{
		throw new NotImplementedException();
	}

	public int CalcRes(int _stat, int _qual)
	{
		if (_qual <= 1)
		{
			return _stat;
		}
		return _qual switch
		{
			2 => _stat + 1, 
			3 => _stat + 2, 
			_ => _stat, 
		};
	}

	public int CalcStat(int _stat, int _qual)
	{
		if (_qual <= 1)
		{
			return _stat;
		}
		if (_qual == 2 && _stat > 0)
		{
			return _stat + Mathf.RoundToInt((float)_stat / 2f);
		}
		if (_qual == 3 && _stat > 0)
		{
			return _stat + _stat;
		}
		return _stat;
	}

	public int CalcDmg(int _stat, int _qual)
	{
		if (_qual <= 1)
		{
			return _stat;
		}
		return _qual switch
		{
			2 => _stat + 1, 
			3 => _stat + 2, 
			_ => _stat, 
		};
	}

	public int CalcACHPMC(int _stat, int _qual)
	{
		if (_qual <= 1)
		{
			return _stat;
		}
		return _qual switch
		{
			2 => _stat + Mathf.RoundToInt((float)_stat / 4f), 
			3 => _stat + Mathf.RoundToInt((float)_stat / 2f), 
			_ => _stat, 
		};
	}
}
