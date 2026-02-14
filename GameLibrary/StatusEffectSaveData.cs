// StatusEffectSaveData
using System;

[Serializable]
public class StatusEffectSaveData
{
	public string id;

	public float durInTicks;

	public StatusEffectSaveData(string _id, float _dur)
	{
		id = _id;
		durInTicks = _dur;
	}
}
