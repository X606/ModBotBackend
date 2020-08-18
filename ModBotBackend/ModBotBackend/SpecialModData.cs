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

		public ulong PostedDate;
		public ulong UpdatedDate;

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

		public Comment GetCommentWithCommentID(string commentID)
		{
			foreach(Comment comment in Comments)
			{
				if (comment.CommentID == commentID)
					return comment;

			}

			return null;
		}
		public void DeleteCommentWithId(string commentID)
		{
			for(int i = Comments.Count - 1; i >= 0; i--)
			{
				if (Comments[i].CommentID == commentID)
				{
					Comments.RemoveAt(i);
					break;
				}
			}
			
			
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
			specialModData.PostedDate = (ulong)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
			specialModData.UpdatedDate = specialModData.PostedDate;

			specialModData.Comments = new List<Comment>();
			specialModData.Reports = new List<ModReport>();

			return specialModData;
		}
	}

	[Serializable]
	public class Comment
	{
		public static Comment CreateNewComment(string posterUserId, string commentBody)
		{
			Comment comment = new Comment();
			comment.PosterUserId = posterUserId;
			comment.CommentBody = commentBody;
			comment.CommentID = Guid.NewGuid().ToString();
			comment.PostedUTCTime = (ulong)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

			return comment;
		}

		public string PosterUserId;
		public string CommentBody;

		public string CommentID;

		public ulong PostedUTCTime;

		public List<string> UsersWhoLikedThis = new List<string>();
	}
}
