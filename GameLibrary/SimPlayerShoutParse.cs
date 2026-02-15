// SimPlayerShoutParse
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class SimPlayerShoutParse : MonoBehaviour
{
	private List<string> QueueShout = new List<string>();

	private List<string> QueueSay = new List<string>();

	private List<string> QueueWhisper = new List<string>();

	private string shoutee;

	private float shoutDel = 77f;

	private float sayDel = 77f;

	private float whisperDel = 77f;

	private float dist = -1f;

	private void Start()
	{
		GameData.ShoutParse = this;
	}

	private void Update()
	{
		if (QueueShout.Count > 0)
		{
			shoutDel -= 60f * Time.deltaTime;
			if (shoutDel <= 0f)
			{
				UpdateSocialLog.LogAdd(QueueShout[0], "#FF9000");
				QueueShout.RemoveAt(0);
				shoutDel = Random.Range(40, 140);
			}
		}
		if (QueueSay.Count > 0)
		{
			sayDel -= 60f * Time.deltaTime;
			if (sayDel <= 0f)
			{
				UpdateSocialLog.LogAdd(QueueSay[0]);
				QueueSay.RemoveAt(0);
				sayDel = Random.Range(35, 160);
			}
		}
		if (QueueWhisper.Count > 0)
		{
			whisperDel -= 60f * Time.deltaTime;
			if (whisperDel <= 0f)
			{
				whisperDel = Random.Range(100, 220);
			}
		}
		if (sayDel <= 0f)
		{
			sayDel = Random.Range(35, 160);
		}
		if (shoutDel <= 0f)
		{
			shoutDel = Random.Range(35, 160);
		}
		if (whisperDel <= 0f)
		{
			whisperDel = Random.Range(35, 160);
		}
	}

	private bool CheckObscenities(string msg)
	{
		foreach (string obscenity in GameData.SimMngr.Obscenities)
		{
			if (Regex.IsMatch(msg, "\\b" + Regex.Escape(obscenity) + "\\b", RegexOptions.IgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	public void ParseShout(string _name, string _shout, bool _isPlayer)
	{
		dist = -1f;
		if (_isPlayer || GameData.SimBanter)
		{
			if (CheckObscenities(_shout) && GameData.SimMngr.ActiveSimInstances.Count > 0)
			{
				RespondToObscene(_shout);
				GameData.SimMngr.GMWarn();
			}
			if (isInvis(_shout))
			{
				FindInvisCaster(_wasShout: true);
			}
			if (infoWanted(_shout))
			{
				StartCoroutine(DirectShoutSearch(_isPlayer, _name, _shout));
			}
			else if (isLFG(_shout))
			{
				RespondToLfg(_isPlayer, _name, _shout);
			}
			else if (isDing(_shout))
			{
				RespondToLvlUp(_isPlayer, _name, _shout);
			}
			else if (isGreeting(_shout))
			{
				RespondToGreeting(_isPlayer, _name, _shout);
			}
			else if (isGoodnight(_shout))
			{
				RespondToGoodnight(_isPlayer, _name, _shout);
			}
		}
	}

	public void ParseSay(string _name, string _shout, bool _isPlayer)
	{
		dist = -1f;
		if (CheckObscenities(_shout) && GameData.SimMngr.ActiveSimInstances.Count > 0)
		{
			RespondToObscene(_shout);
			GameData.SimMngr.GMWarn();
		}
		if (isInvis(_shout))
		{
			FindInvisCaster(_wasShout: false);
		}
		if (isLFG(_shout))
		{
			RespondToLfgSay(_isPlayer, _name, _shout);
			return;
		}
		if (isDing(_shout))
		{
			RespondToLvlUpSay(_isPlayer, _name, _shout);
			return;
		}
		if (CheckSlotRequest(_shout))
		{
			RespondToSlotRequest(_shout);
		}
		if (infoWanted(_shout))
		{
			StartCoroutine(DirectSaySearch(_isPlayer, _name, _shout));
		}
		else if (isGreeting(_shout))
		{
			RespondToGreetingSay(_isPlayer, _name, _shout);
		}
		else if (isGoodnight(_shout))
		{
			RespondToGoodnightSay(_isPlayer, _name, _shout);
		}
	}

	private bool CheckSlotRequest(string msg)
	{
		foreach (string item in GameData.SimMngr.WhereDidYouGet)
		{
			if (Regex.IsMatch(msg, "\\b" + Regex.Escape(item) + "\\b", RegexOptions.IgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	private void RespondToObscene(string _msg)
	{
		if (GameData.SimMngr.ActiveSimInstances.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < GameData.SimMngr.ActiveSimInstances.Count; i++)
		{
			SimPlayer simPlayer = GameData.SimMngr.ActiveSimInstances[i];
			if (!IsThisNotMeSpecifically(simPlayer, _msg) && !IsThisSomeoneElse(simPlayer, _msg) && (Random.Range(0, 10) > 5 || IsThisMeSpecifically(simPlayer, _msg)))
			{
				string exclamation = simPlayer.MyDialog.GetExclamation();
				exclamation = ((!GameData.SimMngr.Sims[simPlayer.GetComponent<SimPlayer>().myIndex].KnowsPlayer) ? Regex.Replace(exclamation, "\\bNN\\b", "") : Regex.Replace(exclamation, "\\bNN\\b", GameData.PlayerStats.MyName));
				exclamation = ((!GameData.SimMngr.Sims[simPlayer.GetComponent<SimPlayer>().myIndex].KnowsPlayer) ? Regex.Replace(exclamation, "\\bNN\\b", "") : Regex.Replace(exclamation, "\\bNN\\b", GameData.PlayerStats.MyName));
				QueueShout.Add(simPlayer.GetComponent<NPC>().NPCName + " says: " + GameData.SimMngr.PersonalizeString(exclamation, simPlayer));
			}
			if (IsThisNotMeSpecifically(simPlayer, _msg))
			{
				QueueShout.Add(simPlayer.GetComponent<NPC>().NPCName + " shouts: " + GameData.SimMngr.PersonalizeString(GameData.PlayerStats.MyName + " " + simPlayer.GetComponent<SimPlayerLanguage>().GetInsult(), simPlayer));
			}
		}
	}

	private void RespondToSlotRequest(string _msg)
	{
		bool done = false;
		if (GameData.SimMngr.ActiveSimInstances.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < GameData.SimMngr.ActiveSimInstances.Count; i++)
		{
			SimPlayer simPlayer = GameData.SimMngr.ActiveSimInstances[i];
			if (simPlayer.IsGMCharacter || Vector3.Distance(simPlayer.transform.position, GameData.PlayerControl.transform.position) > 15f || (!IsThisMeSpecifically(simPlayer, _msg) && !(GameData.PlayerControl.CurrentTarget == simPlayer.MyStats.Myself)))
			{
				continue;
			}
			SimPlayerTracking whichSim = null;
			foreach (SimPlayerTracking sim in GameData.SimMngr.Sims)
			{
				if (sim.SimName == simPlayer.transform.name)
				{
					whichSim = sim;
					break;
				}
			}
			StartCoroutine(GameData.SimMngr.DoSlotRequest(whichSim, _msg, _isWhisper: false, delegate
			{
				done = true;
			}));
		}
	}

	private bool infoWanted(string _shout)
	{
		foreach (string item in GameData.SimMngr.InfoWanted)
		{
			if (Regex.IsMatch(_shout, "\\b" + Regex.Escape(item) + "\\b", RegexOptions.IgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	private bool isGoodnight(string _shout)
	{
		foreach (string item in GameData.SimMngr.Goodnight)
		{
			string str = Regex.Replace(item, "\\bNN\\b", GameData.PlayerStats.MyName);
			if (Regex.IsMatch(_shout, "\\b" + Regex.Escape(str) + "\\b", RegexOptions.IgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	private bool isInvis(string _shout)
	{
		foreach (string item in GameData.SimMngr.InvisReq)
		{
			if (Regex.IsMatch(_shout, "\\b" + Regex.Escape(item) + "\\b", RegexOptions.IgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	private bool isGreeting(string _shout)
	{
		foreach (string item in GameData.SimMngr.GenericGreeting)
		{
			string str = Regex.Replace(item, "\\bNN\\b", GameData.PlayerStats.MyName);
			if (Regex.IsMatch(_shout, "\\b" + Regex.Escape(str) + "\\b", RegexOptions.IgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	private bool isLFG(string _shout)
	{
		foreach (string lFG in GameData.SimMngr.LFGs)
		{
			if (Regex.IsMatch(_shout, "\\b" + Regex.Escape(lFG) + "\\b", RegexOptions.IgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	private bool isDing(string _shout)
	{
		foreach (string levelUpCelebration in GameData.SimMngr.LevelUpCelebrations)
		{
			if (Regex.IsMatch(_shout, "\\b" + Regex.Escape(levelUpCelebration) + "\\b", RegexOptions.IgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	public void RespondToLfg(bool _isPlayer, string _shouter, string _msg)
	{
		if (GameData.SimMngr.ActiveSimInstances.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < GameData.SimMngr.ActiveSimInstances.Count; i++)
		{
			if (!_isPlayer)
			{
				continue;
			}
			SimPlayer simPlayer = GameData.SimMngr.ActiveSimInstances[i];
			if (!simPlayer.IsGMCharacter)
			{
				if (!IsThisNotMeSpecifically(simPlayer, _msg) && !IsThisSomeoneElse(simPlayer, _msg) && !AmIAlreadyGrouped(simPlayer) && Mathf.Abs(simPlayer.MyStats.Level - GameData.PlayerStats.Level) < 3 && (Random.Range(0, 10) > 7 || IsThisMeSpecifically(simPlayer, _msg)) && simPlayer.GetComponent<NPC>().NPCName != _shouter && GameData.SimMngr.Sims[simPlayer.myIndex].GroupedWithOtherSim == null)
				{
					string oTW = simPlayer.MyDialog.GetOTW();
					oTW = ((!(GameData.SimMngr.Sims[simPlayer.GetComponent<SimPlayer>().myIndex].KnowsPlayer && _isPlayer)) ? Regex.Replace(oTW, "\\bNN\\b", "") : Regex.Replace(oTW, "\\bNN\\b", GameData.PlayerStats.MyName));
					QueueShout.Add(simPlayer.GetComponent<NPC>().NPCName + " shouts: " + GameData.SimMngr.PersonalizeString(oTW, simPlayer));
					GameData.SimMngr.Sims[simPlayer.myIndex].seekingPlayer = true;
				}
				else if (!IsThisSomeoneElse(simPlayer, _msg) && Random.Range(0, 10) > 2 && simPlayer.GetComponent<NPC>().NPCName != _shouter && !AmIAlreadyGrouped(simPlayer))
				{
					string declineGroup = simPlayer.MyDialog.GetDeclineGroup();
					declineGroup = ((!(GameData.SimMngr.Sims[simPlayer.GetComponent<SimPlayer>().myIndex].KnowsPlayer && _isPlayer)) ? Regex.Replace(declineGroup, "\\bNN\\b", "") : Regex.Replace(declineGroup, "\\bNN\\b", GameData.PlayerStats.MyName));
					QueueShout.Add(simPlayer.GetComponent<NPC>().NPCName + " shouts: " + GameData.SimMngr.PersonalizeString(declineGroup, simPlayer));
				}
				if (IsThisNotMeSpecifically(simPlayer, _msg))
				{
					QueueShout.Add(simPlayer.GetComponent<NPC>().NPCName + " shouts: " + GameData.SimMngr.PersonalizeString(GameData.PlayerStats.MyName + " " + simPlayer.GetComponent<SimPlayerLanguage>().GetInsult(), simPlayer));
				}
			}
		}
	}

	private void RespondToLvlUp(bool _isPlayer, string _shouter, string _msg)
	{
		if (GameData.SimMngr.ActiveSimInstances.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < GameData.SimMngr.ActiveSimInstances.Count; i++)
		{
			if (_isPlayer)
			{
				SimPlayer simPlayer = GameData.SimMngr.ActiveSimInstances[i];
				if (Random.Range(0, 10) > 5 && simPlayer.Troublemaker <= 6 && simPlayer.GetComponent<NPC>().NPCName != _shouter)
				{
					QueueShout.Add(simPlayer.transform.name + " shouts: " + GameData.SimMngr.PersonalizeString(simPlayer.MyDialog.GetLevelUpCelebration(), simPlayer));
				}
				else if (Random.Range(0, 10) > 4 && simPlayer.Troublemaker > 6 && simPlayer.GetComponent<NPC>().NPCName != _shouter)
				{
					QueueShout.Add(simPlayer.transform.name + " shouts: " + GameData.SimMngr.PersonalizeString(simPlayer.MyDialog.GetLevelUpCelebration(), simPlayer));
				}
			}
		}
	}

	private IEnumerator DirectShoutSearch(bool _isPlayer, string _shouter, string _msg)
	{
		SearchKindResult result = new SearchKindResult
		{
			Kind = KnowledgeDatabase.SearchKind.Unknown
		};
		yield return StartCoroutine(GameData.KnowledgeDatabase.IdentifySearch(_isPlayer, _shouter, _msg, result));
		switch (result.Kind)
		{
		case KnowledgeDatabase.SearchKind.NPC:
			_msg = GameData.KnowledgeDatabase.StripJunkWords(_msg);
			_msg = GameData.KnowledgeDatabase.Normalize(_msg);
			GameData.ShoutParse.ShareInfoNPCInShout(_isPlayer, _shouter, _msg);
			break;
		case KnowledgeDatabase.SearchKind.Item:
			_msg = GameData.KnowledgeDatabase.StripJunkWords(_msg);
			_msg = GameData.KnowledgeDatabase.Normalize(_msg);
			GameData.ShoutParse.ShareInfoItemInShout(_isPlayer, _shouter, _msg);
			break;
		case KnowledgeDatabase.SearchKind.Quest:
			_msg = GameData.KnowledgeDatabase.StripJunkWords(_msg);
			_msg = GameData.KnowledgeDatabase.Normalize(_msg);
			GameData.ShoutParse.ShareInfoQuestInShout(_isPlayer, _shouter, _msg);
			break;
		}
	}

	private IEnumerator DirectSaySearch(bool _isPlayer, string _shouter, string _msg)
	{
		SearchKindResult result = new SearchKindResult
		{
			Kind = KnowledgeDatabase.SearchKind.Unknown
		};
		yield return StartCoroutine(GameData.KnowledgeDatabase.IdentifySearch(_isPlayer, _shouter, _msg, result));
		switch (result.Kind)
		{
		case KnowledgeDatabase.SearchKind.NPC:
			_msg = GameData.KnowledgeDatabase.StripJunkWords(_msg);
			_msg = GameData.KnowledgeDatabase.Normalize(_msg);
			GameData.ShoutParse.ShareInfoNPCSay(_isPlayer, _shouter, _msg);
			break;
		case KnowledgeDatabase.SearchKind.Item:
			_msg = GameData.KnowledgeDatabase.StripJunkWords(_msg);
			_msg = GameData.KnowledgeDatabase.Normalize(_msg);
			GameData.ShoutParse.ShareInfoItemSay(_isPlayer, _shouter, _msg);
			break;
		case KnowledgeDatabase.SearchKind.Quest:
			_msg = GameData.KnowledgeDatabase.StripJunkWords(_msg);
			_msg = GameData.KnowledgeDatabase.Normalize(_msg);
			GameData.ShoutParse.ShareInfoQuestSay(_isPlayer, _shouter, _msg);
			break;
		}
	}

	public void ShareInfoItemInShout(bool _isPlayer, string _shouter, string _msg)
	{
		int num = Random.Range(1, 5);
		Item item = GameData.KnowledgeDatabase.FindBestItemMatch(_msg);
		if (item == null || GameData.SimMngr.ActiveSimInstances.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < GameData.SimMngr.ActiveSimInstances.Count; i++)
		{
			if (!_isPlayer)
			{
				continue;
			}
			SimPlayer simPlayer = GameData.SimMngr.ActiveSimInstances[i];
			if (Random.Range(0, 10) <= 5 || !(simPlayer.GetComponent<NPC>().NPCName != _shouter) || num <= 0)
			{
				continue;
			}
			num--;
			KnowledgeEntry knowledgeEntry = null;
			string zoneInfo = "";
			if (item != null)
			{
				knowledgeEntry = GameData.KnowledgeDatabase.GetKnowledgeByItem(item.ItemName);
				bool flag = false;
				if (knowledgeEntry != null)
				{
					if (!string.IsNullOrEmpty(knowledgeEntry.NPCZoneName))
					{
						zoneInfo = knowledgeEntry.NPCZoneName;
					}
					if (simPlayer.MyStats.Level >= knowledgeEntry.NPCLevel - 10 && !string.IsNullOrEmpty(knowledgeEntry.NPCName))
					{
						QueueShout.Add(simPlayer.transform.name + " shouts: " + GameData.SimMngr.PersonalizeString(GameData.KnowledgeDatabase.GetItemDropAnswer(item, knowledgeEntry, zoneInfo), simPlayer));
						flag = true;
					}
					else
					{
						QueueShout.Add(simPlayer.transform.name + " shouts: " + GameData.SimMngr.PersonalizeString(GameData.KnowledgeDatabase.GetItemDropAnswer(item, null, zoneInfo), simPlayer));
						flag = true;
					}
				}
				if (!flag && item != null)
				{
					string text = "";
					Quest[] questDatabase = GameData.QuestDB.QuestDatabase;
					foreach (Quest quest in questDatabase)
					{
						if (quest.ItemOnComplete == item)
						{
							text = quest.QuestName;
							break;
						}
					}
					if (!string.IsNullOrEmpty(text))
					{
						QueueShout.Add(simPlayer.transform.name + " shouts: " + GameData.SimMngr.PersonalizeString(GameData.KnowledgeDatabase.GetItemQuestAnswer(item, text), simPlayer));
						flag = true;
					}
				}
				if (!flag)
				{
					Item[] itemDB = GameData.ItemDB.ItemDB;
					foreach (Item item2 in itemDB)
					{
						if (item2.TemplateRewards.Contains(item))
						{
							QueueShout.Add(simPlayer.transform.name + " shouts: " + GameData.SimMngr.PersonalizeString(GameData.KnowledgeDatabase.GetItemCraftedAnswer(item, item2), simPlayer));
							flag = true;
							break;
						}
					}
				}
				if (!flag && item.ItemName == "Offering Stone")
				{
					QueueShout.Add(simPlayer.transform.name + " shouts: " + GameData.SimMngr.PersonalizeString("You can summon it with a Bag of Offering Stones.", simPlayer));
				}
				if (!flag)
				{
					QueueShout.Add(simPlayer.transform.name + " shouts: " + GameData.SimMngr.PersonalizeString(GameData.KnowledgeDatabase.GetIDKGeneric(), simPlayer));
				}
				continue;
			}
			QueueShout.Add(simPlayer.transform.name + " shouts: " + GameData.SimMngr.PersonalizeString("I haven't seen of that one yet", simPlayer));
			break;
		}
	}

	public void ShareInfoNPCInShout(bool _isPlayer, string _shouter, string _msg)
	{
		int num = Random.Range(1, 5);
		KnowledgeEntry knowledgeEntry = GameData.KnowledgeDatabase.FindBestNPCMatch(_msg);
		if (knowledgeEntry == null || GameData.SimMngr.ActiveSimInstances.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < GameData.SimMngr.ActiveSimInstances.Count; i++)
		{
			if (!_isPlayer)
			{
				continue;
			}
			SimPlayer simPlayer = GameData.SimMngr.ActiveSimInstances[i];
			if (Random.Range(0, 10) <= 5 || !(simPlayer.GetComponent<NPC>().NPCName != _shouter) || num <= 0)
			{
				continue;
			}
			num--;
			KnowledgeEntry knowledgeEntry2 = null;
			string value = "";
			if (knowledgeEntry != null)
			{
				knowledgeEntry2 = knowledgeEntry;
				if (knowledgeEntry2 != null)
				{
					if (!string.IsNullOrEmpty(knowledgeEntry2.NPCZoneName))
					{
						value = knowledgeEntry2.NPCZoneName;
					}
					if (simPlayer.MyStats.Level >= knowledgeEntry2.NPCLevel - 10)
					{
						QueueShout.Add(simPlayer.transform.name + " shouts: " + GameData.SimMngr.PersonalizeString(GameData.KnowledgeDatabase.GetNPCAnswer(knowledgeEntry2, _knowsDetails: true), simPlayer));
					}
					else if (string.IsNullOrEmpty(value))
					{
						QueueShout.Add(simPlayer.transform.name + " shouts: " + GameData.SimMngr.PersonalizeString(GameData.KnowledgeDatabase.CheckTheWiki(), simPlayer));
					}
					else
					{
						QueueShout.Add(simPlayer.transform.name + " shouts: " + GameData.SimMngr.PersonalizeString(GameData.KnowledgeDatabase.GetNPCAnswer(knowledgeEntry2, _knowsDetails: false), simPlayer));
					}
				}
				else
				{
					QueueShout.Add(simPlayer.transform.name + " shouts: " + GameData.SimMngr.PersonalizeString("I haven't heard of that NPC yet", simPlayer));
				}
				continue;
			}
			QueueShout.Add(simPlayer.transform.name + " shouts: " + GameData.SimMngr.PersonalizeString(GameData.KnowledgeDatabase.CheckTheWiki(), simPlayer));
			break;
		}
	}

	public void ShareInfoQuestInShout(bool _isPlayer, string _shouter, string _msg)
	{
		Quest quest = GameData.KnowledgeDatabase.FindBestQuestMatch(_msg);
		int num = Random.Range(1, 5);
		if (GameData.SimMngr.ActiveSimInstances.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < GameData.SimMngr.ActiveSimInstances.Count; i++)
		{
			if (!_isPlayer)
			{
				continue;
			}
			SimPlayer simPlayer = GameData.SimMngr.ActiveSimInstances[i];
			if (Random.Range(0, 10) > 5 && simPlayer.GetComponent<NPC>().NPCName != _shouter && num > 0)
			{
				num--;
				if (quest == null)
				{
					QueueShout.Add(simPlayer.transform.name + " shouts: " + GameData.SimMngr.PersonalizeString(GameData.KnowledgeDatabase.CheckTheWiki(), simPlayer));
					break;
				}
				if (quest != null)
				{
					QueueShout.Add(simPlayer.transform.name + " shouts: " + GameData.SimMngr.PersonalizeString(GameData.KnowledgeDatabase.GetQuestAnswer(quest), simPlayer));
				}
			}
		}
	}

	private void RespondToGreeting(bool _isPlayer, string _shouter, string _msg)
	{
		if (GameData.SimMngr.ActiveSimInstances.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < GameData.SimMngr.ActiveSimInstances.Count; i++)
		{
			if (!_isPlayer)
			{
				continue;
			}
			SimPlayer simPlayer = GameData.SimMngr.ActiveSimInstances[i];
			string text = "";
			if (!IsThisNotMeSpecifically(simPlayer, _msg) && !IsThisSomeoneElse(simPlayer, _msg) && (Random.Range(0, 10) > 5 || IsThisMeSpecifically(simPlayer, _msg)) && simPlayer.GetComponent<NPC>().NPCName != _shouter && ((GameData.SimMngr.Sims[simPlayer.myIndex].KnowsPlayer && GameData.SimMngr.Sims[simPlayer.myIndex].OpinionOfPlayer > 7f) || IsThisMeSpecifically(simPlayer, _msg)))
			{
				text = simPlayer.MyDialog.GetGreeting() + " " + simPlayer.MyDialog.GetTargetedHello(GameData.SimMngr.Sims[simPlayer.myIndex]);
				switch (text)
				{
				case "NN":
				case "NN!":
				case "NN!!":
					text = GameData.PlayerStats.MyName + "!";
					break;
				}
				text = ((!(GameData.SimMngr.Sims[simPlayer.GetComponent<SimPlayer>().myIndex].KnowsPlayer && _isPlayer)) ? Regex.Replace(text, "\\bNN\\b", "") : Regex.Replace(text, "\\bNN\\b", GameData.PlayerStats.MyName));
				QueueWhisper.Add("[WHISPER FROM] " + simPlayer.GetComponent<NPC>().NPCName + ": " + GameData.SimMngr.PersonalizeString(text, simPlayer));
				GameData.SimMngr.Responses.Add(new WhisperData("[WHISPER FROM] " + simPlayer.GetComponent<NPC>().NPCName + ": " + GameData.SimMngr.PersonalizeString(text, simPlayer), simPlayer.transform.name));
			}
			if (!IsThisNotMeSpecifically(simPlayer, _msg) && !IsThisSomeoneElse(simPlayer, _msg) && Random.Range(0, 10) > 5 && simPlayer.GetComponent<NPC>().NPCName != _shouter)
			{
				text = simPlayer.MyDialog.GetGreeting();
				text = ((!(GameData.SimMngr.Sims[simPlayer.GetComponent<SimPlayer>().myIndex].KnowsPlayer && _isPlayer)) ? Regex.Replace(text, "\\bNN\\b", "") : Regex.Replace(text, "\\bNN\\b", GameData.PlayerStats.MyName));
				QueueShout.Add(simPlayer.GetComponent<NPC>().NPCName + " shouts: " + GameData.SimMngr.PersonalizeString(text, simPlayer));
			}
			if (IsThisNotMeSpecifically(simPlayer, _msg))
			{
				QueueShout.Add(simPlayer.GetComponent<NPC>().NPCName + " shouts: " + GameData.PlayerStats.MyName + " " + GameData.SimMngr.PersonalizeString(simPlayer.GetComponent<SimPlayerLanguage>().GetInsult(), simPlayer));
			}
		}
	}

	private void RespondToGoodnight(bool _isPlayer, string _shouter, string _msg)
	{
		if (GameData.SimMngr.ActiveSimInstances.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < GameData.SimMngr.ActiveSimInstances.Count; i++)
		{
			if (_isPlayer)
			{
				SimPlayer simPlayer = GameData.SimMngr.ActiveSimInstances[i];
				if (!IsThisNotMeSpecifically(simPlayer, _msg) && !IsThisSomeoneElse(simPlayer, _msg) && (Random.Range(0, 10) > 5 || IsThisMeSpecifically(simPlayer, _msg)) && simPlayer.GetComponent<NPC>().NPCName != _shouter)
				{
					string goodnight = simPlayer.MyDialog.GetGoodnight();
					goodnight = ((!(GameData.SimMngr.Sims[simPlayer.GetComponent<SimPlayer>().myIndex].KnowsPlayer && _isPlayer)) ? Regex.Replace(goodnight, "\\bNN\\b", "") : Regex.Replace(goodnight, "\\bNN\\b", GameData.PlayerStats.MyName));
					goodnight = ((!GameData.SimMngr.Sims[simPlayer.GetComponent<SimPlayer>().myIndex].KnowsPlayer) ? Regex.Replace(goodnight, "\\bNN\\b", "") : Regex.Replace(goodnight, "\\bNN\\b", GameData.PlayerStats.MyName));
					QueueShout.Add(simPlayer.GetComponent<NPC>().NPCName + " says: " + GameData.SimMngr.PersonalizeString(goodnight, simPlayer));
				}
				if (IsThisNotMeSpecifically(simPlayer, _msg))
				{
					QueueShout.Add(simPlayer.GetComponent<NPC>().NPCName + " shouts: " + GameData.SimMngr.PersonalizeString(GameData.PlayerStats.MyName + " " + simPlayer.GetComponent<SimPlayerLanguage>().GetInsult(), simPlayer));
				}
			}
		}
	}

	public void RespondToLfgSay(bool _isPlayer, string _shouter, string _msg)
	{
		if (GameData.SimMngr.ActiveSimInstances.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < GameData.SimMngr.ActiveSimInstances.Count; i++)
		{
			if (!_isPlayer)
			{
				continue;
			}
			SimPlayer simPlayer = GameData.SimMngr.ActiveSimInstances[i];
			if (!simPlayer.IsGMCharacter && !AmIAlreadyGrouped(simPlayer) && Vector3.Distance(simPlayer.transform.position, GameData.PlayerControl.transform.position) < 10f)
			{
				if (!IsThisNotMeSpecifically(simPlayer, _msg) && !IsThisSomeoneElse(simPlayer, _msg) && Mathf.Abs(simPlayer.MyStats.Level - GameData.PlayerStats.Level) < 2 && (Random.Range(0, 10) > 4 || IsThisMeSpecifically(simPlayer, _msg)) && simPlayer.GetComponent<NPC>().NPCName != _shouter && GameData.SimMngr.Sims[simPlayer.myIndex].GroupedWithOtherSim == null)
				{
					string oTW = simPlayer.MyDialog.GetOTW();
					oTW = ((!(GameData.SimMngr.Sims[simPlayer.GetComponent<SimPlayer>().myIndex].KnowsPlayer && _isPlayer)) ? Regex.Replace(oTW, "\\bNN\\b", "") : Regex.Replace(oTW, "\\bNN\\b", GameData.PlayerStats.MyName));
					QueueSay.Add(simPlayer.GetComponent<NPC>().NPCName + " says: " + GameData.SimMngr.PersonalizeString(oTW, simPlayer));
					GameData.SimMngr.Sims[simPlayer.myIndex].seekingPlayer = true;
				}
				else if (!IsThisSomeoneElse(simPlayer, _msg) && Random.Range(0, 10) > 1 && simPlayer.GetComponent<NPC>().NPCName != _shouter)
				{
					string declineGroup = simPlayer.MyDialog.GetDeclineGroup();
					declineGroup = ((!(GameData.SimMngr.Sims[simPlayer.GetComponent<SimPlayer>().myIndex].KnowsPlayer && _isPlayer)) ? Regex.Replace(declineGroup, "\\bNN\\b", "") : Regex.Replace(declineGroup, "\\bNN\\b", GameData.PlayerStats.MyName));
					QueueSay.Add(simPlayer.GetComponent<NPC>().NPCName + " says: " + GameData.SimMngr.PersonalizeString(declineGroup, simPlayer));
				}
				if (IsThisNotMeSpecifically(simPlayer, _msg))
				{
					QueueSay.Add(simPlayer.GetComponent<NPC>().NPCName + " says: " + GameData.SimMngr.PersonalizeString(GameData.PlayerStats.MyName + " " + simPlayer.GetComponent<SimPlayerLanguage>().GetInsult(), simPlayer));
				}
			}
		}
	}

	private void ShareInfoItemSay(bool _isPlayer, string _shouter, string _msg)
	{
		int num = Random.Range(1, 5);
		Item item = GameData.KnowledgeDatabase.FindBestItemMatch(_msg);
		if (item == null || GameData.SimMngr.ActiveSimInstances.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < GameData.SimMngr.ActiveSimInstances.Count; i++)
		{
			if (!_isPlayer)
			{
				continue;
			}
			SimPlayer simPlayer = GameData.SimMngr.ActiveSimInstances[i];
			if (!(Vector3.Distance(simPlayer.transform.position, GameData.PlayerControl.transform.position) < 10f) || Random.Range(0, 10) <= 5 || !(simPlayer.GetComponent<NPC>().NPCName != _shouter) || num <= 0)
			{
				continue;
			}
			num--;
			KnowledgeEntry knowledgeEntry = null;
			string zoneInfo = "";
			if (item != null)
			{
				knowledgeEntry = GameData.KnowledgeDatabase.GetKnowledgeByItem(item.ItemName);
				bool flag = false;
				if (knowledgeEntry != null)
				{
					if (!string.IsNullOrEmpty(knowledgeEntry.NPCZoneName))
					{
						zoneInfo = knowledgeEntry.NPCZoneName;
					}
					if (simPlayer.MyStats.Level >= knowledgeEntry.NPCLevel - 10 && !string.IsNullOrEmpty(knowledgeEntry.NPCName))
					{
						QueueSay.Add(simPlayer.transform.name + " says: " + GameData.SimMngr.PersonalizeString(GameData.KnowledgeDatabase.GetItemDropAnswer(item, knowledgeEntry, zoneInfo), simPlayer));
						flag = true;
					}
					else
					{
						QueueSay.Add(simPlayer.transform.name + " says: " + GameData.SimMngr.PersonalizeString(GameData.KnowledgeDatabase.GetItemDropAnswer(item, null, zoneInfo), simPlayer));
						flag = true;
					}
				}
				if (!flag && item != null)
				{
					string text = "";
					Quest[] questDatabase = GameData.QuestDB.QuestDatabase;
					foreach (Quest quest in questDatabase)
					{
						if (quest.ItemOnComplete == item)
						{
							text = quest.QuestName;
							break;
						}
					}
					if (!string.IsNullOrEmpty(text))
					{
						QueueSay.Add(simPlayer.transform.name + " says: " + GameData.SimMngr.PersonalizeString(GameData.KnowledgeDatabase.GetItemQuestAnswer(item, text), simPlayer));
						flag = true;
					}
				}
				if (!flag)
				{
					Item[] itemDB = GameData.ItemDB.ItemDB;
					foreach (Item item2 in itemDB)
					{
						if (item2.TemplateRewards.Contains(item))
						{
							QueueSay.Add(simPlayer.transform.name + " says: " + GameData.SimMngr.PersonalizeString(GameData.KnowledgeDatabase.GetItemCraftedAnswer(item, item2), simPlayer));
							flag = true;
							break;
						}
					}
				}
				if (!flag && item.ItemName == "Offering Stone")
				{
					QueueSay.Add(simPlayer.transform.name + " says: " + GameData.SimMngr.PersonalizeString("You can summon it with a Bag of Offering Stones.", simPlayer));
					flag = true;
				}
				if (!flag)
				{
					QueueSay.Add(simPlayer.transform.name + " says: " + GameData.SimMngr.PersonalizeString(GameData.KnowledgeDatabase.GetIDKGeneric(), simPlayer));
				}
				continue;
			}
			QueueSay.Add(simPlayer.transform.name + " says: " + GameData.SimMngr.PersonalizeString("I haven't seen of that one yet", simPlayer));
			break;
		}
	}

	private void ShareInfoNPCSay(bool _isPlayer, string _shouter, string _msg)
	{
		int num = Random.Range(1, 5);
		KnowledgeEntry knowledgeEntry = GameData.KnowledgeDatabase.FindBestNPCMatch(_msg);
		if (knowledgeEntry == null || GameData.SimMngr.ActiveSimInstances.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < GameData.SimMngr.ActiveSimInstances.Count; i++)
		{
			if (!_isPlayer)
			{
				continue;
			}
			SimPlayer simPlayer = GameData.SimMngr.ActiveSimInstances[i];
			if (!(Vector3.Distance(simPlayer.transform.position, GameData.PlayerControl.transform.position) < 10f) || Random.Range(0, 10) <= 5 || !(simPlayer.GetComponent<NPC>().NPCName != _shouter) || num <= 0)
			{
				continue;
			}
			num--;
			KnowledgeEntry knowledgeEntry2 = null;
			string value = "";
			if (knowledgeEntry != null)
			{
				knowledgeEntry2 = knowledgeEntry;
				if (knowledgeEntry2 != null)
				{
					if (!string.IsNullOrEmpty(knowledgeEntry2.NPCZoneName))
					{
						value = knowledgeEntry2.NPCZoneName;
					}
					if (simPlayer.MyStats.Level >= knowledgeEntry2.NPCLevel - 10)
					{
						QueueSay.Add(simPlayer.transform.name + " says: " + GameData.SimMngr.PersonalizeString(GameData.KnowledgeDatabase.GetNPCAnswer(knowledgeEntry2, _knowsDetails: true), simPlayer));
					}
					else if (string.IsNullOrEmpty(value))
					{
						QueueSay.Add(simPlayer.transform.name + " says: " + GameData.SimMngr.PersonalizeString(GameData.KnowledgeDatabase.CheckTheWiki(), simPlayer));
					}
					else
					{
						QueueSay.Add(simPlayer.transform.name + " says: " + GameData.SimMngr.PersonalizeString(GameData.KnowledgeDatabase.GetNPCAnswer(knowledgeEntry2, _knowsDetails: false), simPlayer));
					}
				}
				else
				{
					QueueSay.Add(simPlayer.transform.name + " says: " + GameData.SimMngr.PersonalizeString("I haven't heard of that NPC yet", simPlayer));
				}
				continue;
			}
			QueueSay.Add(simPlayer.transform.name + " says: " + GameData.SimMngr.PersonalizeString(GameData.KnowledgeDatabase.CheckTheWiki(), simPlayer));
			break;
		}
	}

	private void ShareInfoQuestSay(bool _isPlayer, string _shouter, string _msg)
	{
		Quest quest = GameData.KnowledgeDatabase.FindBestQuestMatch(_msg);
		int num = Random.Range(1, 5);
		if (GameData.SimMngr.ActiveSimInstances.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < GameData.SimMngr.ActiveSimInstances.Count; i++)
		{
			if (!_isPlayer)
			{
				continue;
			}
			SimPlayer simPlayer = GameData.SimMngr.ActiveSimInstances[i];
			if (Vector3.Distance(simPlayer.transform.position, GameData.PlayerControl.transform.position) < 10f && Random.Range(0, 10) > 5 && simPlayer.GetComponent<NPC>().NPCName != _shouter && num > 0)
			{
				num--;
				if (quest == null)
				{
					QueueShout.Add(simPlayer.transform.name + " says: " + GameData.SimMngr.PersonalizeString(GameData.KnowledgeDatabase.CheckTheWiki(), simPlayer));
					break;
				}
				if (quest != null)
				{
					QueueShout.Add(simPlayer.transform.name + " says: " + GameData.SimMngr.PersonalizeString(GameData.KnowledgeDatabase.GetQuestAnswer(quest), simPlayer));
				}
			}
		}
	}

	private void RespondToLvlUpSay(bool _isPlayer, string _shouter, string _msg)
	{
		if (GameData.SimMngr.ActiveSimInstances.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < GameData.SimMngr.ActiveSimInstances.Count; i++)
		{
			if (!_isPlayer)
			{
				continue;
			}
			SimPlayer simPlayer = GameData.SimMngr.ActiveSimInstances[i];
			if (Vector3.Distance(simPlayer.transform.position, GameData.PlayerControl.transform.position) < 10f && Vector3.Distance(simPlayer.transform.position, GameData.PlayerControl.transform.position) < 10f)
			{
				if ((Random.Range(0, 10) > 4 || IsThisMeSpecifically(simPlayer, _msg)) && simPlayer.Troublemaker <= 6 && simPlayer.GetComponent<NPC>().NPCName != _shouter)
				{
					QueueSay.Add(simPlayer.GetComponent<NPC>().NPCName + " says: " + GameData.SimMngr.PersonalizeString(GameData.SimMngr.LevelUpCongratulations[Random.Range(0, GameData.SimMngr.LevelUpCongratulations.Count)], simPlayer));
				}
				else if (Random.Range(0, 10) > 4 && simPlayer.Troublemaker > 6 && simPlayer.GetComponent<NPC>().NPCName != _shouter)
				{
					QueueSay.Add(simPlayer.GetComponent<NPC>().NPCName + " says: " + GameData.SimMngr.PersonalizeString(GameData.SimMngr.LevelUpCongratulations[Random.Range(14, GameData.SimMngr.LevelUpCongratulations.Count)], simPlayer));
				}
			}
		}
	}

	private void RespondToGreetingSay(bool _isPlayer, string _shouter, string _msg)
	{
		if (GameData.SimMngr.ActiveSimInstances.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < GameData.SimMngr.ActiveSimInstances.Count; i++)
		{
			if (!_isPlayer)
			{
				continue;
			}
			SimPlayer simPlayer = GameData.SimMngr.ActiveSimInstances[i];
			if (Vector3.Distance(simPlayer.transform.position, GameData.PlayerControl.transform.position) < 10f && !GameData.SimMngr.Sims[simPlayer.myIndex].Grouped)
			{
				string text = "";
				if (!IsThisNotMeSpecifically(simPlayer, _msg) && !IsThisSomeoneElse(simPlayer, _msg) && (IsThisMeSpecifically(simPlayer, _msg) || Random.Range(0, 10) > 2) && simPlayer.GetComponent<NPC>().NPCName != _shouter)
				{
					text = simPlayer.MyDialog.GetGreeting() + " " + simPlayer.MyDialog.GetTargetedHello(GameData.SimMngr.Sims[simPlayer.myIndex]);
					text = ((!(GameData.SimMngr.Sims[simPlayer.GetComponent<SimPlayer>().myIndex].KnowsPlayer && _isPlayer)) ? Regex.Replace(text, "\\bNN\\b", "") : Regex.Replace(text, "\\bNN\\b", GameData.PlayerStats.MyName));
					QueueSay.Add(simPlayer.GetComponent<NPC>().NPCName + " says: " + GameData.SimMngr.PersonalizeString(text, simPlayer));
				}
				if (IsThisNotMeSpecifically(simPlayer, _msg))
				{
					QueueSay.Add(simPlayer.GetComponent<NPC>().NPCName + " says: " + GameData.SimMngr.PersonalizeString(GameData.PlayerStats.MyName + " " + simPlayer.GetComponent<SimPlayerLanguage>().GetInsult(), simPlayer));
				}
			}
		}
	}

	private void RespondToGoodnightSay(bool _isPlayer, string _shouter, string _msg)
	{
		if (GameData.SimMngr.ActiveSimInstances.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < GameData.SimMngr.ActiveSimInstances.Count; i++)
		{
			if (!_isPlayer)
			{
				continue;
			}
			SimPlayer simPlayer = GameData.SimMngr.ActiveSimInstances[i];
			if (Vector3.Distance(simPlayer.transform.position, GameData.PlayerControl.transform.position) < 10f)
			{
				if (!IsThisNotMeSpecifically(simPlayer, _msg) && !IsThisSomeoneElse(simPlayer, _msg) && (IsThisMeSpecifically(simPlayer, _msg) || Random.Range(0, 10) > 3) && simPlayer.GetComponent<NPC>().NPCName != _shouter)
				{
					string goodnight = simPlayer.MyDialog.GetGoodnight();
					goodnight = ((!(GameData.SimMngr.Sims[simPlayer.GetComponent<SimPlayer>().myIndex].KnowsPlayer && _isPlayer)) ? Regex.Replace(goodnight, "\\bNN\\b", "") : Regex.Replace(goodnight, "\\bNN\\b", GameData.PlayerStats.MyName));
					goodnight = ((!GameData.SimMngr.Sims[simPlayer.GetComponent<SimPlayer>().myIndex].KnowsPlayer) ? Regex.Replace(goodnight, "\\bNN\\b", "") : Regex.Replace(goodnight, "\\bNN\\b", GameData.PlayerStats.MyName));
					QueueSay.Add(simPlayer.GetComponent<NPC>().NPCName + " says: " + GameData.SimMngr.PersonalizeString(goodnight, simPlayer));
				}
				if (IsThisNotMeSpecifically(simPlayer, _msg))
				{
					QueueSay.Add(simPlayer.GetComponent<NPC>().NPCName + " says: " + GameData.SimMngr.PersonalizeString(GameData.PlayerStats.MyName + " " + simPlayer.GetComponent<SimPlayerLanguage>().GetInsult(), simPlayer));
				}
			}
		}
	}

	public bool IsThisMeSpecifically(SimPlayer _sim, string _msg)
	{
		_msg = _msg.ToLower();
		if (_msg.Contains("druid") || _msg.Contains("paladin") || _msg.Contains("windblade") || (_msg.Contains("arcanist") && !_msg.Contains(_sim.MyStats.CharacterClass.DisplayName)))
		{
			if (!_msg.Contains("no") && !_msg.Contains("not") && !_msg.Contains("except") && !_msg.Contains("but"))
			{
				return false;
			}
			return true;
		}
		if ((_msg.Contains("dps") || _msg.Contains("damage") || _msg.Contains("dpsers")) && !_msg.Contains("no") && !_msg.Contains("not") && !_msg.Contains("except") && !_msg.Contains("but"))
		{
			if (_sim.MyStats.CharacterClass == GameData.ClassDB.Duelist || _sim.MyStats.CharacterClass == GameData.ClassDB.Arcanist)
			{
				return true;
			}
			return false;
		}
		if ((_msg.Contains("heal") || _msg.Contains("heals") || _msg.Contains("healer")) && !_msg.Contains("no") && !_msg.Contains("not") && !_msg.Contains("except") && !_msg.Contains("but"))
		{
			if (_sim.MyStats.CharacterClass == GameData.ClassDB.Druid || _sim.MyStats.CharacterClass == GameData.ClassDB.Paladin)
			{
				return true;
			}
			return false;
		}
		if ((_msg.Contains("tank") || _msg.Contains("tanks")) && !_msg.Contains("no") && !_msg.Contains("not") && !_msg.Contains("except") && !_msg.Contains("but"))
		{
			if (_sim.MyStats.CharacterClass != GameData.ClassDB.Paladin)
			{
				return false;
			}
			return true;
		}
		if ((_msg.Contains(_sim.MyStats.MyName.ToLower()) || _msg.Contains(_sim.MyStats.CharacterClass.DisplayName.ToLower())) && (!_msg.Contains("no") || !_msg.Contains("not") || !_msg.Contains("except") || !_msg.Contains("but")))
		{
			return true;
		}
		return false;
	}

	public bool IsThisSomeoneElse(SimPlayer _sim, string _msg)
	{
		_msg = _msg.ToLower();
		if ((_msg.Contains("druid") || _msg.Contains("paladin") || _msg.Contains("windblade") || _msg.Contains("arcanist")) && !_msg.Contains("no") && !_msg.Contains("not") && !_msg.Contains("except") && !_msg.Contains("but") && !_msg.Contains(_sim.MyStats.CharacterClass.DisplayName.ToLower()))
		{
			return true;
		}
		if ((_msg.Contains("dps") || _msg.Contains("damage") || _msg.Contains("dpsers")) && !_msg.Contains("no") && !_msg.Contains("not") && !_msg.Contains("except") && !_msg.Contains("but") && _sim.MyStats.CharacterClass != GameData.ClassDB.Duelist && _sim.MyStats.CharacterClass != GameData.ClassDB.Arcanist)
		{
			return true;
		}
		if ((_msg.Contains("heal") || _msg.Contains("heals") || _msg.Contains("healer")) && !_msg.Contains("no") && !_msg.Contains("not") && !_msg.Contains("except") && !_msg.Contains("but") && _sim.MyStats.CharacterClass != GameData.ClassDB.Druid && _sim.MyStats.CharacterClass != GameData.ClassDB.Paladin)
		{
			return true;
		}
		if ((_msg.Contains("tank") || _msg.Contains("tanks")) && !_msg.Contains("no") && !_msg.Contains("not") && !_msg.Contains("except") && !_msg.Contains("but") && _sim.MyStats.CharacterClass != GameData.ClassDB.Paladin)
		{
			return true;
		}
		foreach (SimPlayer activeSimInstance in GameData.SimMngr.ActiveSimInstances)
		{
			if (_msg.Contains(activeSimInstance.MyStats.MyName.ToLower()) && activeSimInstance.MyStats.MyName != _sim.MyStats.MyName)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsThisNotMeSpecifically(SimPlayer _sim, string _msg)
	{
		_msg = _msg.ToLower();
		if ((_msg.Contains(_sim.MyStats.MyName.ToLower()) || _msg.Contains(_sim.MyStats.CharacterClass.DisplayName.ToLower())) && (_msg.Contains("no") || _msg.Contains("except") || _msg.Contains("not") || _msg.Contains("but")))
		{
			return true;
		}
		return false;
	}

	public bool AmIAlreadyGrouped(SimPlayer _sim)
	{
		if (GameData.GroupMembers[0] != null && GameData.GroupMembers[0] == GameData.SimMngr.Sims[_sim.myIndex])
		{
			return true;
		}
		if (GameData.GroupMembers[1] != null && GameData.GroupMembers[1] == GameData.SimMngr.Sims[_sim.myIndex])
		{
			return true;
		}
		if (GameData.GroupMembers[2] != null && GameData.GroupMembers[2] == GameData.SimMngr.Sims[_sim.myIndex])
		{
			return true;
		}
		if (GameData.GroupMembers[3] != null && GameData.GroupMembers[3] == GameData.SimMngr.Sims[_sim.myIndex])
		{
			return true;
		}
		return false;
	}

	private void FindInvisCaster(bool _wasShout)
	{
		SimPlayer simPlayer = null;
		foreach (SimPlayer activeSimInstance in GameData.SimMngr.ActiveSimInstances)
		{
			if (activeSimInstance.MyStats.CharacterClass == GameData.ClassDB.Arcanist && activeSimInstance.MyStats.Level >= 9 && !activeSimInstance.Rival)
			{
				activeSimInstance.InvisPlayerIfISeeHim = 10000f;
				if (Vector3.Distance(activeSimInstance.transform.position, GameData.PlayerControl.transform.position) < 15f && !activeSimInstance.GetComponent<CastSpell>().isCasting())
				{
					simPlayer = activeSimInstance;
					break;
				}
			}
		}
		if (!(simPlayer == null))
		{
			simPlayer.GetThisNPC().CastInvisOnPlayer();
			QueueSay.Add(simPlayer.GetThisNPC().NPCName + " says: " + GameData.SimMngr.PersonalizeString("I can do it for you, hang on", simPlayer));
		}
	}
}
