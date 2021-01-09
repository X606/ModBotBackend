using HttpUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ModBotBackend;
using ModLibrary;
using Newtonsoft.Json;
using ModBotBackend.Users;

namespace ModBotBackend.Operations
{
	[Operation("search")]
	public class ModSearchOperation : OperationBase
	{
		public override bool ParseAsJson => true;
		public override string[] Arguments => new string[] { };
		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;
		public override string OverrideAPICallJavascript =>
			@"this.searchString = null;
	this.includeDescriptionsInSearch = false;
	this.userID = null;
	this.modID = null;
	this.sortOrder = 'liked';

	this.Send = function () {
		return new Promise(async resolve => {

			var result = await Post('/api/?operation=search',
				{
					searchString: this.searchString,
					includeDescriptionsInSearch: this.includeDescriptionsInSearch,
					userID: this.userID,
					sortOrder: this.sortOrder,
					modID: this.modID
				});
			result = JSON.parse(result);

			resolve(result);

		});
	}
	this.searchSortTypes = {
		Liked: 'liked',
		Downloads: 'downloads',
		PostDate: 'postedDate',
		EditedDate: 'editedDate'
	};";
		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			context.Response.ContentType = "text/plain";

			byte[] data = Misc.ToByteArray(context.Request.InputStream);
			string json = Encoding.UTF8.GetString(data);

			ModSearchMessage request = Newtonsoft.Json.JsonConvert.DeserializeObject<ModSearchMessage>(json);

			KeyValuePair<SpecialModData, ModInfo>[] mods = UploadedModsManager.Instance.GetAllUploadedMods();
			List<string> selectedModIds = new List<string>();

			for(int i = 0; i < mods.Length; i++)
			{
				bool include = Search(mods[i], request);

				if (include)
					selectedModIds.Add(mods[i].Value.UniqueID);
			}

			if(request.sortOrder == "liked")
			{
				selectedModIds.Sort(delegate (string a, string b)
				{
					SpecialModData specialAData = UploadedModsManager.Instance.GetSpecialModInfoFromId(a);
					SpecialModData specialBData = UploadedModsManager.Instance.GetSpecialModInfoFromId(b);

					return specialBData.Likes - specialAData.Likes;
				});
			}
			else if(request.sortOrder == "downloads")
			{
				selectedModIds.Sort(delegate (string a, string b)
				{
					SpecialModData specialAData = UploadedModsManager.Instance.GetSpecialModInfoFromId(a);
					SpecialModData specialBData = UploadedModsManager.Instance.GetSpecialModInfoFromId(b);

					return specialBData.Downloads - specialAData.Downloads;
				});
			}
			else if(request.sortOrder == "postedDate")
			{
				selectedModIds.Sort(delegate (string a, string b)
				{
					SpecialModData specialAData = UploadedModsManager.Instance.GetSpecialModInfoFromId(a);
					SpecialModData specialBData = UploadedModsManager.Instance.GetSpecialModInfoFromId(b);

					return (int)(specialBData.PostedDate - specialAData.PostedDate);
				});
			}
			else if(request.sortOrder == "editedDate")
			{
				selectedModIds.Sort(delegate (string a, string b)
				{
					SpecialModData specialAData = UploadedModsManager.Instance.GetSpecialModInfoFromId(a);
					SpecialModData specialBData = UploadedModsManager.Instance.GetSpecialModInfoFromId(b);

					return (int)(specialBData.UpdatedDate - specialAData.UpdatedDate);
				});
			}

			string responseJson = JsonConvert.SerializeObject(selectedModIds);
			HttpStream httpStream = new HttpStream(context.Response);
			httpStream.Send(responseJson);
			httpStream.Close();
		}

		static bool Search(KeyValuePair<SpecialModData, ModInfo> item, ModSearchMessage request)
		{
			bool shouldIncludeItem = true;

			if (request.searchString != null)
			{
				string searchString = request.searchString.ToLower();
				string name = item.Value.DisplayName != null ? item.Value.DisplayName.ToLower() : "";
				string description = item.Value.Description != null ? item.Value.Description.ToLower() : "";
				bool nameContains = name.Contains(searchString);
				bool descriptionContains = description.Contains(searchString);

				shouldIncludeItem = nameContains;

				if (request.includeDescriptionsInSearch)
				{
					shouldIncludeItem |= descriptionContains;
				}
			}

			if (request.userID != null)
			{
				if (request.userID != item.Key.OwnerID)
					shouldIncludeItem = false;
			}
			if (request.modID != null)
			{
				if (request.userID != item.Key.ModId)
					shouldIncludeItem = false;
			}

			return shouldIncludeItem;
		}

		static bool stringIncludesString(string totalString, string lowerString)
		{
			return totalString.Contains(lowerString);
		}

		public class ModSearchMessage
		{
			public string searchString = null;
			public bool includeDescriptionsInSearch = false;
			public string userID = null;
			public string modID = null;

			public string sortOrder = "liked";
		}

	}
}
