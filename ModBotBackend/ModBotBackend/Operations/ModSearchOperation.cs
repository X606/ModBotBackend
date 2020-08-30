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

namespace ModBotBackend.Operations
{
	public class ModSearchOperation : OperationBase
	{
		public override void OnOperation(HttpListenerContext context)
		{
			context.Response.ContentType = "text/plain";

			byte[] data = Misc.ToByteArray(context.Request.InputStream);
			string json = Encoding.UTF8.GetString(data);

			ModSearchMessage request = Newtonsoft.Json.JsonConvert.DeserializeObject<ModSearchMessage>(json);

			KeyValuePair<SpecialModData, ModInfo>[] mods = UploadedModsManager.GetAllUploadedMods();
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
					SpecialModData specialAData = UploadedModsManager.GetSpecialModInfoFromId(a);
					SpecialModData specialBData = UploadedModsManager.GetSpecialModInfoFromId(b);

					return specialBData.Likes - specialAData.Likes;
				});
			}
			else if(request.sortOrder == "downloads")
			{
				selectedModIds.Sort(delegate (string a, string b)
				{
					SpecialModData specialAData = UploadedModsManager.GetSpecialModInfoFromId(a);
					SpecialModData specialBData = UploadedModsManager.GetSpecialModInfoFromId(b);

					return specialBData.Downloads - specialAData.Downloads;
				});
			}
			else if(request.sortOrder == "postedDate")
			{
				selectedModIds.Sort(delegate (string a, string b)
				{
					SpecialModData specialAData = UploadedModsManager.GetSpecialModInfoFromId(a);
					SpecialModData specialBData = UploadedModsManager.GetSpecialModInfoFromId(b);

					return (int)(specialAData.PostedDate - specialBData.PostedDate);
				});
			}
			else if(request.sortOrder == "editedDate")
			{
				selectedModIds.Sort(delegate (string a, string b)
				{
					SpecialModData specialAData = UploadedModsManager.GetSpecialModInfoFromId(a);
					SpecialModData specialBData = UploadedModsManager.GetSpecialModInfoFromId(b);

					return (int)(specialAData.UpdatedDate - specialBData.UpdatedDate);
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

				string name = item.Value.DisplayName.ToLower();
				string description = item.Value.Description.ToLower();

				bool nameContains = name.Contains(searchString);
				bool descriptionContains = description.Contains(searchString);

				shouldIncludeItem = nameContains || descriptionContains;
			}
			
			if (request.userID != null)
			{
				if(request.userID != item.Key.OwnerID)
					shouldIncludeItem = false;
			}
			if (request.modID != null)
			{
				if(request.userID != item.Key.ModId)
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
