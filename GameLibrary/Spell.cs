// Spell
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spell", menuName = "ScriptableObjects/Spell", order = 2)]
public class Spell : BaseScriptableObject
{
	public enum SpellType
	{
		Damage,
		StatusEffect,
		Beneficial,
		AE,
		PBAE,
		Misc,
		Heal,
		Pet
	}

	public enum SpellLine
	{
		Dru_Poison_DOT,
		Dru_Void_DOT,
		Dru_Magic_DOT,
		Dru_Element_DOT,
		Dru_Other_Debuff,
		Arc_Poison_DOT,
		Arc_Void_DOT,
		Arc_Magic_DOT,
		Arc_Element_DOT,
		Arc_Other_Debuff,
		Pal_Poison_DOT,
		Pal_Void_DOT,
		Pal_Magic_DOT,
		Pal_Element_DOT,
		Pal_Other_Debuff,
		Duel_Poison_DOT,
		Duel_Void_DOT,
		Duel_Magic_DOT,
		Duel_Element_DOT,
		Duel_Poison_DD,
		Duel_Other_Debuff,
		Buff_Haste_Worn,
		Buff_Resists_Worn,
		Buff_Combat_Worn,
		Buff_Defense_Worn,
		Buff_Hardiness_Worn,
		Buff_Movespeed_Worn,
		Global_Haste,
		Global_Resists,
		Global_Buff,
		Global_Move,
		Global_Magic_Debuff,
		Global_Other_Debuff,
		Global_Poison_DOT,
		Global_Magic_DOT,
		Global_Element_DOT,
		Global_Void_DOT,
		Generic,
		Charm,
		Sleep,
		Stun,
		Shield,
		Regen,
		Aura_Duelist,
		Aura_Paladin,
		Aura_Druid,
		Aura_Arcanist,
		Aura_Stormcaller,
		Aura_Buff_Legs,
		Aura_Buff_Body,
		Aura_Buff_Arms,
		Aura_Lifesteal,
		Aura_AC,
		Aura_Resonate,
		Aura_DamageShield,
		XPBonus,
		Bleed,
		Direct_Damage,
		Mind_Buff,
		Regeneration,
		Mana_Recovery_Buff,
		Aura_Buff_Mind,
		Aura_Buff_Void,
		Aura_Buff_Magic,
		Aura_Buff_Physical,
		Aura_Buff_Resists,
		Healing,
		Combat_Buff,
		Attack_Buff,
		DPS_Buff,
		Global_Root,
		Duelist_Attack_Buff,
		Duelist_Combat_Buff,
		Vithean_Buff,
		Solunarian_Buff,
		Braxonian_Buff,
		Sivakayan_Buff,
		Azynthian_Buff,
		Fernallan_Buff,
		Attack_Speed_Debuff,
		Stormcaller_Magic_DOT,
		Stormcaller_Buff,
		Stormcaller_Move_Debuff,
		Braxonian_DOT,
		AggroDecrease,
		Reav_Movespeed,
		Reav_Fear,
		Reav_VoidDOT,
		Reav_Suffering,
		Aura_Reaver
	}

	public List<Class> UsedBy;

	public SpellType Type;

	public SpellLine Line;

	public int RequiredLevel;

	public string SpellName;

	public int SpellChargeFXIndex;

	public int SpellResolveFXIndex;

	public float SpellChargeTime;

	public Sprite SpellIcon;

	public int SpellDurationInTicks;

	public bool UnstableDuration;

	public string StatusEffectMessageOnPlayer;

	public string StatusEffectMessageOnNPC;

	public string SpellDesc;

	public bool InstantEffect;

	public int ManaCost;

	public int Aggro;

	public int TargetDamage;

	public int TargetHealing;

	public int CasterHealing;

	public float Cooldown;

	public int ShieldingAmt;

	public int HP;

	public int AC;

	public int Mana;

	public int PercentManaRestoration;

	public float MovementSpeed;

	public int Str;

	public int Dex;

	public int End;

	public int Agi;

	public int Wis;

	public int Int;

	public int Cha;

	public int MR;

	public int ER;

	public int PR;

	public int VR;

	public int DamageShield;

	public float Haste;

	public float percentLifesteal;

	public int AtkRollModifier;

	public int BleedDamagePercent;

	public bool RootTarget;

	public bool StunTarget;

	public bool CharmTarget;

	public bool FearTarget;

	public bool Lifetap;

	public bool InflictOnSelf;

	public bool GroupEffect;

	public GameData.DamageType MyDamageType;

	public float ResistModifier;

	public AudioClip ChargeSound;

	public List<AudioClip> ChargeVariations;

	public AudioClip CompleteSound;

	public List<AudioClip> CompleteVariations;

	public float SpellRange;

	public bool SelfOnly;

	public int MaxLevelTarget;

	public GameObject PetToSummon;

	public Spell StatusEffectToApply;

	public bool ApplyToCaster;

	public bool NoResonate;

	public float ShakeDur;

	public float ShakeAmp;

	public Color color;

	public bool SimUsable = true;

	public bool HardcodedUseCase;

	public bool CanHitPlayers = true;

	public bool BreakOnDamage;

	public bool CrowdControlSpell;

	public bool JoltSpell;

	public bool TauntSpell;

	public bool ReapAndRenew;

	public int ResonateChance;

	public float XPBonus;

	public bool AutomateAttack;

	public bool WornEffect;

	public bool BreakOnAnyAction;

	public Spell AddProc;

	public int AddProcChance;

	public string SpecialDescriptor = "";

	public bool ForHardEncounters;

	public bool GrantInvisibility;

	public bool CannotInterrupt;
}
