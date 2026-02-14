// IDLog
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IDLog : MonoBehaviour
{
	private TextMeshProUGUI myText;

	public TextMeshProUGUI CombatText;

	public TextMeshProUGUI ChatText;

	private int autoTypeWord;

	private float scroll;

	public ScrollRect scrollRect;

	public RectTransform content;

	public RectTransform combatContent;

	private float lineHeight;

	public void ScrollToBottom(ScrollRect scrollRect)
	{
		Canvas.ForceUpdateCanvases();
		scrollRect.verticalNormalizedPosition = 0f;
	}

	private void Start()
	{
		GameData.ChatLog = this;
		myText = GetComponent<TextMeshProUGUI>();
		Vector2 sizeDelta = content.sizeDelta;
		sizeDelta.y = 1f;
		content.sizeDelta = sizeDelta;
		UpdateCombatHeight(CombatText);
		ScrollToBottom(scrollRect);
		lineHeight = myText.fontSize * 1.25f;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0) || (GameData.PlayerControl.usingGamepad && Input.GetKeyDown(KeyCode.JoystickButton0)))
		{
			OnClick();
		}
		if (GameData.PlayerControl.usingGamepad && Mathf.Abs(Input.GetAxis("DPADY")) > 0.2f && Input.GetKey(KeyCode.JoystickButton1))
		{
			float y = scrollRect.content.sizeDelta.y;
			float num = 5f * Input.GetAxis("DPADY");
			scrollRect.verticalNormalizedPosition += num / y;
		}
		if (GameData.UsingSteam && SteamUtils.GetAppID() == (AppId_t)2522260u && !GameData.GM.DemoBuild)
		{
			GameData.GM.DemoBuild = true;
			UpdateSocialLog.LogAdd("Welcome to the Erenshor Demo!", "lightblue");
		}
	}

	public void UpdateChatLog()
	{
		myText.SetText(UpdateSocialLog.GameLog);
		UpdateContentHeight(myText);
	}

	public void UpdateCombatLog()
	{
		CombatText.SetText(UpdateSocialLog.CombatLog);
		UpdateCombatHeight(CombatText);
	}

	private void AutotypeWord()
	{
		autoTypeWord = TMP_TextUtilities.FindIntersectingCharacter(myText, Input.mousePosition, null, visibleOnly: false);
	}

	private string FindWholeWordFromIndex(string txt, int index)
	{
		string text = "";
		int num = 0;
		int num2 = 0;
		if (!string.IsNullOrEmpty(txt) && index > 0 && index < txt.Length)
		{
			for (int num3 = index; num3 > 0; num3--)
			{
				if (txt[num3] == ' ')
				{
					num = num3 + 1;
					break;
				}
			}
			for (int i = index; i < txt.Length; i++)
			{
				if (txt[i] == ' ')
				{
					num2 = i - 1;
					break;
				}
			}
			for (int j = num; j <= num2; j++)
			{
				text += txt[j];
			}
			return text;
		}
		return null;
	}

	private void OnClick()
	{
		int num = TMP_TextUtilities.FindIntersectingWord(myText, Input.mousePosition, null);
		if (num != -1)
		{
			string word = myText.textInfo.wordInfo[num].GetWord();
			if (GameData.PlayerControl.CurrentTarget != null && GameData.PlayerControl.CurrentTarget.GetComponent<NPCDialogManager>() != null && GameData.PlayerControl.CurrentTarget.GetComponent<NPCDialogManager>().SearchForKeyword(word))
			{
				GameData.TextInput.ForceTextInput(word);
			}
		}
	}

	public void UpdateContentHeight(TextMeshProUGUI text)
	{
		if (content.sizeDelta.y >= 3000f)
		{
			if (content.sizeDelta.y != 3000f)
			{
				Vector2 sizeDelta = content.sizeDelta;
				sizeDelta.y = 3000f;
				content.sizeDelta = sizeDelta;
			}
		}
		else
		{
			Vector2 sizeDelta = content.sizeDelta;
			sizeDelta.y += lineHeight;
			content.sizeDelta = sizeDelta;
		}
	}

	public void UpdateCombatHeight(TextMeshProUGUI text)
	{
		if (!(combatContent.sizeDelta.y >= 600f))
		{
			float preferredHeight = text.preferredHeight;
			Vector2 sizeDelta = combatContent.sizeDelta;
			sizeDelta.y = preferredHeight;
			combatContent.sizeDelta = sizeDelta;
		}
	}
}
