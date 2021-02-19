using ModBotBackend.Users;
using ModLibrary;
using System.Collections.Generic;

namespace ModBotBackend.Operations
{
    [Operation("search")]
    public class ModSearchOperation : JsonOperationBase
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
    this.includeUnchecked = false;    

	this.Send = function () {
		return new Promise(async resolve => {

			var result = await Post('/api/?operation=search',
				{
					searchString: this.searchString,
					includeDescriptionsInSearch: this.includeDescriptionsInSearch,
					userID: this.userID,
					sortOrder: this.sortOrder,
					modID: this.modID,
                    includeUnchecked: this.includeUnchecked
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
        public override JsonOperationResponseBase OnOperation(Arguments arguments, Authentication authentication)
        {
            KeyValuePair<SpecialModData, ModInfo>[] mods = UploadedModsManager.Instance.GetAllUploadedMods();
            List<string> selectedModIds = new List<string>();

            for (int i = 0; i < mods.Length; i++)
            {
                bool include = Search(mods[i], arguments);

                if (include)
                    selectedModIds.Add(mods[i].Value.UniqueID);
            }

            if (arguments["sortOrder"] == "liked")
            {
                selectedModIds.Sort(delegate (string a, string b)
                {
                    SpecialModData specialAData = UploadedModsManager.Instance.GetSpecialModInfoFromId(a);
                    SpecialModData specialBData = UploadedModsManager.Instance.GetSpecialModInfoFromId(b);

                    return specialBData.Likes - specialAData.Likes;
                });
            }
            else if (arguments["sortOrder"] == "downloads")
            {
                selectedModIds.Sort(delegate (string a, string b)
                {
                    SpecialModData specialAData = UploadedModsManager.Instance.GetSpecialModInfoFromId(a);
                    SpecialModData specialBData = UploadedModsManager.Instance.GetSpecialModInfoFromId(b);

                    return specialBData.Downloads - specialAData.Downloads;
                });
            }
            else if (arguments["sortOrder"] == "postedDate")
            {
                selectedModIds.Sort(delegate (string a, string b)
                {
                    SpecialModData specialAData = UploadedModsManager.Instance.GetSpecialModInfoFromId(a);
                    SpecialModData specialBData = UploadedModsManager.Instance.GetSpecialModInfoFromId(b);

                    return (int)(specialBData.PostedDate - specialAData.PostedDate);
                });
            }
            else if (arguments["sortOrder"] == "editedDate")
            {
                selectedModIds.Sort(delegate (string a, string b)
                {
                    SpecialModData specialAData = UploadedModsManager.Instance.GetSpecialModInfoFromId(a);
                    SpecialModData specialBData = UploadedModsManager.Instance.GetSpecialModInfoFromId(b);

                    return (int)(specialBData.UpdatedDate - specialAData.UpdatedDate);
                });
            }

            return new SearchOperationResponse()
            {
                ModIds = selectedModIds
            };
        }

        static bool Search(KeyValuePair<SpecialModData, ModInfo> item, Arguments arguments)
        {
            bool shouldIncludeItem = true;
            if (!string.IsNullOrWhiteSpace(arguments["searchString"]))
            {
                string searchString = ((string)arguments["searchString"]).ToLower();

                string name = item.Value.DisplayName != null ? item.Value.DisplayName.ToLower() : "";
                string description = item.Value.Description != null ? item.Value.Description.ToLower() : "";
                bool nameContains = name.Contains(searchString);
                bool descriptionContains = description.Contains(searchString);

                shouldIncludeItem = nameContains;

                if (((bool)arguments["includeDescriptionsInSearch"]))
                {
                    shouldIncludeItem |= descriptionContains;
                }
            }

            if (!string.IsNullOrWhiteSpace(arguments["userID"]))
            {
                if (((string)arguments["userID"]) != item.Key.OwnerID)
                    shouldIncludeItem = false;
            }
            if (!string.IsNullOrWhiteSpace(arguments["modID"]))
            {
                if (((string)arguments["userID"]) != item.Key.ModId)
                    shouldIncludeItem = false;
            }

            if (arguments["includeUnchecked"] == false && !item.Key.Verified)
            {
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
    public class SearchOperationResponse : JsonOperationResponseBase
    {
        public List<string> ModIds = new List<string>();
    }
}
