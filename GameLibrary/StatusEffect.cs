// StatusEffect
using System;

[Serializable]
public class StatusEffect
{
	public Spell Effect;

	public float Duration;

	public bool fromPlayer;

	public int bonusDmg;

	public bool CastedByPC;

	public Character Owner;

	public Character CreditDPS;

	public StatusEffect(Spell _spell, bool _fromPlayer, bool _CastedByPC, Character _owner)
	{
		Effect = _spell;
		fromPlayer = _fromPlayer;
		Owner = _owner;
		if (_spell != null)
		{
			Duration = _spell.SpellDurationInTicks;
		}
	}
}
