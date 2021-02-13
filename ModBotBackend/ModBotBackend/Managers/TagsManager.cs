using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ModBotBackend.Managers
{
    [FolderName("Tags")]
    public class TagsManager : OwnFolderObject<TagsManager>
    {
        public string TagsPath => GetPathForFile("Tags") + "/";
        public string UsersPath => GetPathForFile("Users") + "/";

        Dictionary<string, TagInfo> _tags = new Dictionary<string, TagInfo>();
        Dictionary<string, PlayerTagsInfo> _users = new Dictionary<string, PlayerTagsInfo>();

        public override void OnStartup()
        {
            Directory.CreateDirectory(TagsPath);
            Directory.CreateDirectory(UsersPath);

            foreach (string tagPath in Directory.GetFiles(TagsPath))
            {
                TagInfo tag = new TagInfo(File.ReadAllBytes(tagPath));
                _tags.Add(tag.TagID, tag);
            }

            foreach (string user in Directory.GetFiles(UsersPath))
            {
                string playfabID = Path.GetFileNameWithoutExtension(user);
                PlayerTagsInfo playerTag = new PlayerTagsInfo(File.ReadAllBytes(user));
                _users.Add(playfabID, playerTag);
            }


        }

        public string GetTagPath(TagInfo tag) => TagsPath + tag.TagID + ".tag";
        public string GetUserTagSavePath(string playfabID) => UsersPath + playfabID + ".playerTags";
        public void SaveTag(TagInfo tag)
        {
            string tagPath = GetTagPath(tag);
            File.WriteAllBytes(tagPath, tag.GetData());

            _tags[tag.TagID] = tag;
        }
        public void RemoveTag(TagInfo tag)
        {
            if (!_tags.ContainsKey(tag.TagID))
                throw new Exception("Can't remove tag that doesn't exist");

            string tagPath = GetTagPath(tag);
            File.Delete(tagPath);

            _tags.Remove(tag.TagID);

            string[] keys = _users.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                PlayerTagsInfo user = _users[keys[i]];
                if (user.TagIds.Contains(tag.TagID))
                {
                    string[] tags = user.TagIds;
                    List<string> output = new List<string>();
                    for (int j = 0; j < tags.Length; j++)
                    {
                        if (tags[j] != tag.TagID)
                        {
                            output.Add(tags[j]);
                        }
                    }

                    user.TagIds = output.ToArray();

                    SaveUserTags(keys[i], user);
                }

            }
        }
        public void SaveUserTags(string playfabId, PlayerTagsInfo playerTags)
        {
            string userPath = GetUserTagSavePath(playfabId);
            File.WriteAllBytes(userPath, playerTags.GetData());

            _users[playfabId] = playerTags;
        }

        public TagInfo[] GetTagsForPlayfabId(string playfabID)
        {
            if (playfabID == null || !_users.ContainsKey(playfabID))
                return new TagInfo[0];

            string[] tags = _users[playfabID].TagIds;
            TagInfo[] tagInfos = new TagInfo[tags.Length];

            for (int i = 0; i < tags.Length; i++)
            {
                tagInfos[i] = _tags[tags[i]];
            }

            return tagInfos;
        }
        public TagInfo GetTag(string tagID)
        {
            if (_tags.TryGetValue(tagID, out TagInfo tag))
                return tag;

            return null;
        }

        public TagInfo[] GetTags()
        {
            return _tags.Values.ToArray();
        }
    }

    [Serializable]
    public class PlayerTagsInfo
    {
        public string[] TagIds;

        public PlayerTagsInfo(string[] tagIds)
        {
            TagIds = tagIds;
        }

        public PlayerTagsInfo(byte[] data) => BitCompressor.FillObj(this, data);
        public byte[] GetData() => BitCompressor.GetBytes(this);
    }

    [Serializable]
    public class TagInfo
    {
        public TagInfo(string creatorId, string tagName, string body)
        {
            TagID = Guid.NewGuid().ToString();
            CreatorId = creatorId;
            TagName = tagName;
            Body = body;
            Verified = false;
        }
        public TagInfo(byte[] data) => BitCompressor.FillObj(this, data);
        public byte[] GetData() => BitCompressor.GetBytes(this);

        public bool Verified = false;

        public readonly string TagID;
        public readonly string TagName;
        public readonly string CreatorId;
        public string Body;
    }
}
