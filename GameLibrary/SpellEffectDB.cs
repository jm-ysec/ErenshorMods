// SpellEffectDB
using System.Collections.Generic;
using UnityEngine;

public class SpellEffectDB : MonoBehaviour
{
	public Class DefaultClass;

	public List<GameObject> SpellEffects;

	public List<ParticleDensityControl> SpellEffectsDefaults;

	public GameObject ParticleFXVessel;

	public GameObject AEcollector;

	public List<GameObject> DruidPets;

	public GameObject SpellLighting;

	public GameObject GearSparkles;

	private void Awake()
	{
		GameData.EffectDB = this;
	}

	private void Start()
	{
		foreach (GameObject spellEffect in SpellEffects)
		{
			ParticleSystem component = spellEffect.GetComponent<ParticleSystem>();
			if (component != null)
			{
				ParticleSystem.MainModule main = component.main;
				main.stopAction = ParticleSystemStopAction.Disable;
			}
		}
	}
}
