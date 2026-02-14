// IDLogLocal
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IDLogLocal : MonoBehaviour
{
	public TextMeshProUGUI myText;

	private int autoTypeWord;

	private float scroll;

	public ScrollRect scrollRect;

	private Vector3 DialogPosition = Vector3.zero;

	public TextMeshProUGUI DialogHeader;

	public bool VendorLog;

	public bool AHLog;

	public RectTransform content;

	private void Awake()
	{
		if (VendorLog)
		{
			GameData.VendorLog = this;
		}
		if (AHLog)
		{
			GameData.AHLog = this;
		}
	}

	private void Start()
	{
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
		if (DialogPosition != Vector3.zero && Vector3.Distance(GameData.PlayerControl.transform.position, DialogPosition) > 10f)
		{
			DialogPosition = Vector3.zero;
			GameData.Misc.DisableLocalChatLog();
		}
	}

	public void ResetDialogPosition()
	{
		DialogPosition = Vector3.zero;
	}

	public void UpdateChatLog()
	{
		myText.SetText(UpdateSocialLog.LocalLog);
	}

	public void ClearChatLog()
	{
		UpdateSocialLog.LocalLog.Clear();
	}

	public void StartLocalDialog(string _NPCName, Vector3 _pos)
	{
		ClearChatLog();
		DialogHeader.text = "Conversation with: " + _NPCName;
		UpdateSocialLog.LocalLogAdd("  ");
		DialogPosition = _pos;
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
		text.ForceMeshUpdate();
		float preferredHeight = text.preferredHeight;
		Vector2 sizeDelta = content.sizeDelta;
		sizeDelta.y = preferredHeight;
		content.sizeDelta = sizeDelta;
	}
}
