// UpdateSocialLog
using System.Text;

public static class UpdateSocialLog
{
	public static StringBuilder GameLog = new StringBuilder();

	public static StringBuilder LocalLog = new StringBuilder();

	public static StringBuilder CombatLog = new StringBuilder();

	private static string updateStr;

	public static void LogAdd(string _string)
	{
		GameLog.AppendLine(_string);
		if (GameLog.Length > 6000)
		{
			GameLog = GameLog.Remove(0, GameLog.Length - 5000);
		}
		GameData.ChatLog.UpdateChatLog();
	}

	public static void LogAdd(string _string, bool _append)
	{
		GameLog.AppendLine(_string);
		if (GameLog.Length > 5000 && _append)
		{
			GameLog = GameLog.Remove(0, GameLog.Length - 5000);
		}
		GameData.ChatLog.UpdateChatLog();
	}

	public static string LogAdd(string _string, string _colorAsString)
	{
		if (_colorAsString == "red")
		{
			_colorAsString = "#F01010";
		}
		GameLog.AppendLine("<color=" + _colorAsString + ">" + _string + "</color>");
		if (GameLog.Length > 5000)
		{
			GameLog = GameLog.Remove(0, GameLog.Length - 5000);
		}
		if (GameData.ChatLog != null)
		{
			GameData.ChatLog.UpdateChatLog();
		}
		return _string;
	}

	public static string LogAdd(string _string, string _colorAsString, bool _append)
	{
		if (_colorAsString == "red")
		{
			_colorAsString = "#F01010";
		}
		GameLog.AppendLine("<color=" + _colorAsString + ">" + _string + "</color>");
		if (GameLog.Length > 5000 && _append)
		{
			GameLog = GameLog.Remove(0, GameLog.Length - 5000);
		}
		if (GameData.ChatLog != null)
		{
			GameData.ChatLog.UpdateChatLog();
		}
		return _string;
	}

	public static void CombatLogAdd(string _string)
	{
		CombatLog.AppendLine(_string);
		if (CombatLog.Length > 1800)
		{
			CombatLog.Append(CombatLog.Remove(0, CombatLog.Length - 1800));
		}
		GameData.ChatLog.UpdateCombatLog();
	}

	public static void CombatLogAdd(string _string, string _colorAsString)
	{
		if (_colorAsString == "red")
		{
			_colorAsString = "#F01010";
		}
		CombatLog.AppendLine("<color=" + _colorAsString + ">" + _string + "</color>");
		if (CombatLog.Length > 1800)
		{
			CombatLog.Append(CombatLog.Remove(0, CombatLog.Length - 1800));
		}
		GameData.ChatLog.UpdateCombatLog();
	}

	public static void LocalLogAdd(string _string)
	{
		GameData.GM.GlobalChatShift = 3600f;
		LocalLog.AppendLine(_string);
		if (LocalLog.Length > 3000)
		{
			LocalLog = LocalLog.Remove(0, LocalLog.Length - 3000);
		}
		GameData.LocalLog.UpdateChatLog();
		if (GameData.VendorLog != null)
		{
			GameData.VendorLog.UpdateChatLog();
		}
		if (GameData.AHLog != null)
		{
			GameData.AHLog.UpdateChatLog();
		}
	}

	public static void LocalLogAdd(string _string, bool _append)
	{
		GameData.GM.GlobalChatShift = 3600f;
		LocalLog.AppendLine(_string);
		if (LocalLog.Length > 3000 && _append)
		{
			LocalLog = GameLog.Remove(0, LocalLog.Length - 3000);
		}
		GameData.LocalLog.UpdateChatLog();
		if (GameData.VendorLog != null)
		{
			GameData.VendorLog.UpdateChatLog();
		}
		if (GameData.AHLog != null)
		{
			GameData.AHLog.UpdateChatLog();
		}
	}

	public static string LocalLogAdd(string _string, string _colorAsString)
	{
		GameData.GM.GlobalChatShift = 3600f;
		if (_colorAsString == "red")
		{
			_colorAsString = "#F01010";
		}
		LocalLog.AppendLine("<color=" + _colorAsString + ">" + _string + "</color>");
		if (LocalLog.Length > 3000)
		{
			LocalLog = LocalLog.Remove(0, GameLog.Length - 3000);
		}
		if (GameData.LocalLog != null)
		{
			GameData.LocalLog.UpdateChatLog();
		}
		if (GameData.VendorLog != null)
		{
			GameData.VendorLog.UpdateChatLog();
		}
		if (GameData.AHLog != null)
		{
			GameData.AHLog.UpdateChatLog();
		}
		return _string;
	}

	public static string LocalLogAdd(string _string, string _colorAsString, bool _append)
	{
		GameData.GM.GlobalChatShift = 3600f;
		if (_colorAsString == "red")
		{
			_colorAsString = "#F01010";
		}
		LocalLog.AppendLine("<color=" + _colorAsString + ">" + _string + "</color>");
		if (LocalLog.Length > 3000 && _append)
		{
			LocalLog = LocalLog.Remove(0, GameLog.Length - 3000);
		}
		if (GameData.LocalLog != null)
		{
			GameData.LocalLog.UpdateChatLog();
		}
		if (GameData.VendorLog != null)
		{
			GameData.VendorLog.UpdateChatLog();
		}
		if (GameData.AHLog != null)
		{
			GameData.AHLog.UpdateChatLog();
		}
		return _string;
	}
}
