﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ModBotBackend
{
	public abstract class OwnFolderObject<T> where T: OwnFolderObject<T>
	{
		public void Init(string folderName)
		{
			_instance = (T)this;

			_folderName = folderName;

			Directory.CreateDirectory(FolderPath);

			OnStartup();
		}
		public static T Instance => _instance;
		static T _instance;

		string _folderName;

		public string FolderPath => Program.BasePath + "/" + _folderName + "/";


		public virtual void OnStartup() { }

	}

	public class FolderNameAttribute : Attribute
	{
		public string FolderName;
		public FolderNameAttribute(string folderName)
		{
			FolderName = folderName;
		}
	}
}