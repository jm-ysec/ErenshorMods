// CastBar
using TMPro;
using UnityEngine;

public class CastBar : MonoBehaviour
{
	public GameObject castBar;

	public GameObject OverchantBar;

	public RectTransform TopBar;

	public RectTransform OCBarRect;

	public TextMeshProUGUI SpellName;

	public TextMeshProUGUI OCTxt;

	private float OCTime;

	private void Start()
	{
		GameData.CB = this;
	}

	public void NewCast(string _spellName)
	{
		OCTime = 30f;
		OCTxt.fontSize = 48f;
		castBar.SetActive(value: true);
		OverchantBar.SetActive(value: true);
		OCTxt.gameObject.SetActive(value: false);
		OCBarRect.sizeDelta = new Vector2(0f, 13f);
		SpellName.text = _spellName;
	}

	public void CloseBar()
	{
		castBar.SetActive(value: false);
	}

	private void Update()
	{
		if (OCTxt.gameObject.activeSelf && OCTxt.fontSize > 24f)
		{
			OCTxt.fontSize -= 140f * Time.deltaTime;
		}
	}
}
