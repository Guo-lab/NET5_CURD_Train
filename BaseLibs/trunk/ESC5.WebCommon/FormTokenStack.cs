using System.Collections.Generic;

namespace ESC5.WebCommon
{
	/// <summary>
	/// @author Rainy
	/// </summary>
	public class FormTokenStack
	{
		public List<string> TokenStack { get; set; } = new List<string>();
		public List<string> ViewStack { get; set; } = new List<string>();

		public void Save(string vmTypeName, string token, bool clear)
		{
			if (clear)
			{
				ViewStack.Clear();
				TokenStack.Clear();
			}
			int i = ViewStack.IndexOf(vmTypeName);
			if (i >= 0)
			{
				ViewStack.RemoveRange(i, ViewStack.Count-i);
				TokenStack.RemoveRange(i, TokenStack.Count-i);
			}
			ViewStack.Add(vmTypeName);
			TokenStack.Add(token);
		}

		public bool HasToken(string vmTypeName, string token)
		{
			int i = ViewStack.IndexOf(vmTypeName);
			if (i == -1)
				return false;
			return TokenStack[i].Equals(token);
		}
		public bool HasToken(string vmTypeName)
		{
			return ViewStack.IndexOf(vmTypeName)>=0;
		}
		public bool HasToken()
		{
			return TokenStack.Count!=0;
		}
	}
}
