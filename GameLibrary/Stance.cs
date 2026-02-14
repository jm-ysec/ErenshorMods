// Stance
using UnityEngine;

[CreateAssetMenu(fileName = "Stance", menuName = "ScriptableObjects/Stance", order = 22)]
public class Stance : BaseScriptableObject
{
	public string DisplayName;

	public float MaxHPMod = 1f;

	public float DamageMod = 1f;

	public float ProcRateMod = 1f;

	public float DamageTakenMod = 1f;

	public float SelfDamagePerAttack;

	public float AggroGenMod = 1f;

	public float SpellDamageMod = 1f;

	public float SelfDamagePerCast;

	public float LifestealAmount;

	public float ResonanceAmount;

	public bool StopRegen;

	public string SwitchMessage;

	[TextArea(2, 6)]
	public string StanceDesc;
}
