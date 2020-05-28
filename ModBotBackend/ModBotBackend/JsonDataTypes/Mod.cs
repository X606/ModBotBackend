using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ModBotBackend.JsonDataTypes
{
	public class Mod
	{
		public string ModName;
		public string Description;
		public string Creator;
		public string UniqueID;
		public bool Checked;

		public string ModFileName;
		public string ModImageName;
	}
}
