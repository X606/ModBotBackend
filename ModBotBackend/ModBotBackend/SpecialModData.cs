using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ModLibrary;

namespace ModBotBackend
{
	[Serializable]
	public class SpecialModData
	{
		private SpecialModData() { }

		public int Likes;
		public int Downloads;
		public string OwnerID;
		public string ModId;

		public List<Comment> Comments = new List<Comment>();
		public List<ModReport> Reports = new List<ModReport>();

		public string ToJson()
		{
			return JsonConvert.SerializeObject(this);
		}
		public static SpecialModData FromJson(string json)
		{
			return JsonConvert.DeserializeObject<SpecialModData>(json);
		}

		public void Save()
		{
			UploadedModsManager.SaveSpecialModData(this);
		}

		public static SpecialModData CreateNewSpecialModData(ModInfo mod, string ownerID)
		{
			SpecialModData specialModData = new SpecialModData();
			specialModData.Likes = 0;
			specialModData.Downloads = 0;
			specialModData.ModId = mod.UniqueID;
			specialModData.OwnerID = ownerID;

			specialModData.Comments = new List<Comment>();
			specialModData.Reports = new List<ModReport>();

			return specialModData;
		}
	}

	[Serializable]
	public class Comment
	{
		public string PosterUserId;
		public string CommentBody;
	}
}
