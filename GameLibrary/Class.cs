// Class
using UnityEngine;

[CreateAssetMenu(fileName = "Class", menuName = "ScriptableObjects/Class", order = 3)]
public class Class : ScriptableObject
{
	public string ClassName;

	public float MitigationBonus;

	public int StrBenefit;

	public int EndBenefit;

	public int DexBenefit;

	public int AgiBenefit;

	public int IntBenefit;

	public int WisBenefit;

	public int ChaBenefit;

	public float AggroMod;

	public string DisplayName;
}
