// SimPlayerLanguage
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class SimPlayerLanguage : MonoBehaviour
{
	public bool Public;

	public List<string> Greetings;

	public List<string> ReturnGreeting;

	public List<string> Invites;

	public List<string> Justifications;

	public List<string> Confirms;

	public List<string> GenericLines;

	public List<string> Aggro;

	public List<string> Died;

	public List<string> InsultsFun;

	public List<string> RetortsFun;

	public List<string> Exclamations;

	public List<string> Denials;

	public List<string> DeclineGroup;

	public List<string> Negative;

	public List<string> LFGPublic;

	public List<string> OTW;

	public List<string> Goodnight;

	public List<string> Hello;

	public List<string> LocalFriendHello;

	public List<string> UnsureResponse;

	public List<string> AngerResponse;

	public List<string> Affirms;

	public List<string> EnvDmg;

	public List<string> WantsDrop;

	public List<string> Gratitude;

	public List<string> Impressed;

	public List<string> ImpressedEnd;

	public List<string> AcknowledgeGratitude;

	public List<string> LevelUpCelebration;

	public List<string> GoodLastOuting;

	public List<string> BadLastOuting;

	public List<string> GotAnItemLastOuting;

	public List<string> ReturnToZone;

	public List<string> BeenAWhile;

	public List<string> Unsure;

	private void Start()
	{
		if (Public)
		{
			GameData.SimLang = this;
		}
	}

	public string GetGreeting()
	{
		if (Greetings.Count > 0)
		{
			return Greetings[UnityEngine.Random.Range(0, Greetings.Count)];
		}
		return "Hi";
	}

	public string GetReturnGreeting()
	{
		if (ReturnGreeting.Count > 0)
		{
			return Regex.Replace(ReturnGreeting[UnityEngine.Random.Range(0, ReturnGreeting.Count)], "\\bNN\\b", GameData.PlayerStats.MyName);
		}
		return "Hi";
	}

	public string GetInvite()
	{
		if (Invites.Count > 0)
		{
			return Regex.Replace(Invites[UnityEngine.Random.Range(0, Invites.Count)], "\\bNN\\b", GameData.PlayerStats.MyName);
		}
		return "Come xp";
	}

	public string GetJustification()
	{
		if (Justifications.Count > 0)
		{
			return Justifications[UnityEngine.Random.Range(0, Justifications.Count)];
		}
		return "it'll be good";
	}

	public string GetConfirm()
	{
		if (Confirms.Count > 0)
		{
			return Confirms[UnityEngine.Random.Range(0, Confirms.Count)];
		}
		return "roger";
	}

	public string GetGeneric()
	{
		if (GameData.CurrentZoneAnnounce != null && GameData.CurrentZoneAnnounce.ZoneComments.Count > 0)
		{
			return GameData.CurrentZoneAnnounce.ZoneComments[UnityEngine.Random.Range(0, GameData.CurrentZoneAnnounce.ZoneComments.Count)];
		}
		return "oops";
	}

	public string GetAggro()
	{
		if (Aggro.Count > 0)
		{
			return Aggro[UnityEngine.Random.Range(0, Aggro.Count)];
		}
		return "it's on me";
	}

	public string GetDied()
	{
		if (Died.Count > 0)
		{
			return Died[UnityEngine.Random.Range(0, Died.Count)];
		}
		return "dang dang dang!";
	}

	public string GetInsult()
	{
		if (InsultsFun.Count > 0)
		{
			return InsultsFun[UnityEngine.Random.Range(0, InsultsFun.Count)];
		}
		return "You're stinky and your mother dresses you funny";
	}

	public string GetRetort()
	{
		if (RetortsFun.Count > 0)
		{
			return RetortsFun[UnityEngine.Random.Range(0, RetortsFun.Count)];
		}
		return "back at you haha take that";
	}

	public string GetExclamation()
	{
		if (Exclamations.Count > 0)
		{
			return Exclamations[UnityEngine.Random.Range(0, Exclamations.Count)];
		}
		return "Oh my GOSH";
	}

	public string GetDenials()
	{
		if (Denials.Count > 0)
		{
			return Denials[UnityEngine.Random.Range(0, Denials.Count)];
		}
		return "no";
	}

	public string GetDeclineGroup()
	{
		if (DeclineGroup.Count > 0)
		{
			return DeclineGroup[UnityEngine.Random.Range(0, DeclineGroup.Count)];
		}
		return "busy atm";
	}

	public string GetNegative()
	{
		if (Negative.Count > 0)
		{
			return Negative[UnityEngine.Random.Range(0, Negative.Count)];
		}
		return "busy atm";
	}

	public string GetLFGPublic()
	{
		if (LFGPublic.Count > 0)
		{
			return LFGPublic[UnityEngine.Random.Range(0, LFGPublic.Count)];
		}
		return ".";
	}

	public string GetOTW()
	{
		if (OTW.Count > 0)
		{
			return OTW[UnityEngine.Random.Range(0, OTW.Count)];
		}
		return "coming";
	}

	public string GetGoodnight()
	{
		if (Goodnight.Count > 0)
		{
			return Regex.Replace(Goodnight[UnityEngine.Random.Range(0, Goodnight.Count)], "\\bNN\\b", GameData.PlayerStats.MyName);
		}
		return "bye";
	}

	public string GetHello()
	{
		if (Hello.Count > 0)
		{
			return Regex.Replace(Hello[UnityEngine.Random.Range(0, Hello.Count)], "\\bNN\\b", GameData.PlayerStats.MyName);
		}
		return "heya";
	}

	public string GetTargetedHello(SimPlayerTracking _sim = null)
	{
		return Regex.Replace(HelloBuilder(_sim), "\\bNN\\b", GameData.PlayerStats.MyName);
	}

	public string GetUnsure()
	{
		if (UnsureResponse.Count > 0)
		{
			return UnsureResponse[UnityEngine.Random.Range(0, UnsureResponse.Count)];
		}
		return "uh what?";
	}

	public string GetAnger()
	{
		if (AngerResponse.Count > 0)
		{
			return AngerResponse[UnityEngine.Random.Range(0, AngerResponse.Count)];
		}
		return ":(";
	}

	public string GetAcknowledgeGratitude()
	{
		if (AcknowledgeGratitude.Count > 0)
		{
			return AcknowledgeGratitude[UnityEngine.Random.Range(0, AcknowledgeGratitude.Count)];
		}
		return ":)";
	}

	public string GetAffirm()
	{
		if (Affirms.Count > 0)
		{
			return Affirms[UnityEngine.Random.Range(0, Affirms.Count)];
		}
		return "Yeah";
	}

	public string GetEnvDmg()
	{
		if (EnvDmg.Count > 0)
		{
			return EnvDmg[UnityEngine.Random.Range(0, EnvDmg.Count)];
		}
		return "OW OW OW";
	}

	public string GetLootReq()
	{
		if (WantsDrop.Count > 0)
		{
			return WantsDrop[UnityEngine.Random.Range(0, WantsDrop.Count)];
		}
		return GameData.SimLang.WantsDrop[UnityEngine.Random.Range(0, GameData.SimLang.WantsDrop.Count)];
	}

	public string GetGratitude()
	{
		if (Gratitude.Count > 0)
		{
			return Gratitude[UnityEngine.Random.Range(0, Gratitude.Count)];
		}
		return "Yes! Thanks! ";
	}

	public string GetImpressed()
	{
		if (Impressed.Count > 0)
		{
			return Impressed[UnityEngine.Random.Range(0, Impressed.Count)];
		}
		return "nice";
	}

	public string GetImpressedEnd()
	{
		if (ImpressedEnd.Count > 0)
		{
			return ImpressedEnd[UnityEngine.Random.Range(0, ImpressedEnd.Count)];
		}
		return "";
	}

	public string GetLevelUpCelebration()
	{
		if (LevelUpCelebration.Count > 0)
		{
			return LevelUpCelebration[UnityEngine.Random.Range(0, LevelUpCelebration.Count)];
		}
		return "";
	}

	public string HelloBuilder(SimPlayerTracking _sim)
	{
		SimPlayer component = base.transform.GetComponent<SimPlayer>();
		SimPlayerLanguage component2 = GameData.SimMngr.GetComponent<SimPlayerLanguage>();
		if (component != null && GameData.SimMngr?.Sims != null && component.myIndex >= 0 && component.myIndex < GameData.SimMngr.Sims.Count && _sim?.MyPreviousMemory != null)
		{
			SimPlayerMemory simPlayerMemory = _sim?.MyPreviousMemory;
			string text = "";
			if (simPlayerMemory != null && string.IsNullOrEmpty(simPlayerMemory.NameOfPlayerCharacter))
			{
				if (LocalFriendHello.Count > 0)
				{
					return LocalFriendHello[UnityEngine.Random.Range(0, LocalFriendHello.Count)];
				}
				return "Hiya NN";
			}
			if (simPlayerMemory != null && !string.IsNullOrEmpty(simPlayerMemory.NameOfPlayerCharacter) && simPlayerMemory.NameOfPlayerCharacter != GameData.PlayerStats.MyName)
			{
				switch (UnityEngine.Random.Range(0, 10))
				{
				case 0:
					text = "You're on an alt?? Jump over onto " + simPlayerMemory.NameOfPlayerCharacter + " and let's go!";
					break;
				case 1:
					text = "Hey get on " + simPlayerMemory.NameOfPlayerCharacter + " so we can group!";
					break;
				case 2:
					text = "New character?? Why aren't you on " + simPlayerMemory.NameOfPlayerCharacter + " so we can xp?";
					break;
				case 3:
					text = "Where's " + simPlayerMemory.NameOfPlayerCharacter + "? I wanna group again!";
					break;
				case 4:
					text = "What are you... making alts now? Get on " + simPlayerMemory.NameOfPlayerCharacter + "!";
					break;
				case 5:
					text = "I'm gonna need you go get on " + simPlayerMemory.NameOfPlayerCharacter + " so we can group again.";
					break;
				case 6:
					text = "Hi! You gonna be playing " + simPlayerMemory.NameOfPlayerCharacter + " today? I wanna group!";
					break;
				case 7:
					text = "Who's got time for alts? You should be on " + simPlayerMemory.NameOfPlayerCharacter + "!";
					break;
				case 8:
					text = "I need to make an alt too so I can group with you when you're not playing " + simPlayerMemory.NameOfPlayerCharacter + "...";
					break;
				case 9:
					text = "Uh... " + GameData.PlayerStats.MyName + " is the toon you're gonna be on today? Lame!";
					break;
				}
				return text;
			}
			if (simPlayerMemory.PlayedDay < DateTime.Now.DayOfYear - 3 || simPlayerMemory.PlayedYear < DateTime.Now.Year)
			{
				simPlayerMemory.PlayedDay = DateTime.Now.DayOfYear;
				simPlayerMemory.PlayedYear = DateTime.Now.Year;
				if (BeenAWhile.Count > 0)
				{
					return text + BeenAWhile[UnityEngine.Random.Range(0, BeenAWhile.Count)];
				}
				return text + component2.BeenAWhile[UnityEngine.Random.Range(0, component2.BeenAWhile.Count)];
			}
			text = ((Greetings.Count <= 0) ? (text + component2.Greetings[UnityEngine.Random.Range(0, component2.Greetings.Count)] + " NN! ") : (text + Greetings[UnityEngine.Random.Range(0, Greetings.Count)] + "! "));
			if (simPlayerMemory.GroupedLastDay > DateTime.Now.DayOfYear - 5)
			{
				if (!string.IsNullOrEmpty(simPlayerMemory.ZoneName))
				{
					text = ((ReturnToZone.Count <= 0) ? (text + component2.ReturnToZone[UnityEngine.Random.Range(0, component2.ReturnToZone.Count)] + " " + simPlayerMemory.ZoneName + "! ") : (text + ReturnToZone[UnityEngine.Random.Range(0, ReturnToZone.Count)] + " " + simPlayerMemory.ZoneName + "! "));
				}
				if (((simPlayerMemory.LevelGain < 1 && simPlayerMemory.Died > 5) || simPlayerMemory.XPGain < 1) && !string.IsNullOrEmpty(simPlayerMemory.ZoneName))
				{
					text = ((BadLastOuting.Count <= 0) ? (text + component2.BadLastOuting[UnityEngine.Random.Range(0, component2.BadLastOuting.Count)] + ". ") : (text + BadLastOuting[UnityEngine.Random.Range(0, BadLastOuting.Count)] + ". "));
				}
				else if (!string.IsNullOrEmpty(simPlayerMemory.ZoneName))
				{
					text = ((GoodLastOuting.Count <= 0) ? (text + component2.GoodLastOuting[UnityEngine.Random.Range(0, component2.GoodLastOuting.Count)] + " ") : (text + GoodLastOuting[UnityEngine.Random.Range(0, GoodLastOuting.Count)] + " "));
				}
				if (!string.IsNullOrEmpty(simPlayerMemory.ItemGained))
				{
					string text2 = GameData.ItemDB.GetItemByID(simPlayerMemory.ItemGained)?.ItemName;
					if (text2 != null)
					{
						text = ((GotAnItemLastOuting.Count <= 0) ? (text + component2.GotAnItemLastOuting[UnityEngine.Random.Range(0, component2.GotAnItemLastOuting.Count)] + " " + text2 + ".") : (text + GotAnItemLastOuting[UnityEngine.Random.Range(0, GotAnItemLastOuting.Count)] + " " + text2 + "."));
					}
				}
			}
			return text;
		}
		if (LocalFriendHello.Count > 0)
		{
			return LocalFriendHello[UnityEngine.Random.Range(0, LocalFriendHello.Count)];
		}
		return "NN!!!";
	}
}
