﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ModLibrary;
using System.IO;
using ModBotBackend.Users;
using System.Drawing;

namespace ModBotBackend.Operations
{
	
	[Operation("getModImage")]
	public class GetModImageOperation : OperationBase
	{
		public override bool ParseAsJson => false;
		public override string[] Arguments => new string[] { "element", "id"};
		public override bool ArgumentsInQuerystring => true;
		public override AuthenticationLevel MinimumAuthenticationLevelToCall => AuthenticationLevel.None;
		public override string OverrideAPICallJavascript => "element.src = \"/api/?operation=getModImage&id=\" + id;";

		public override void OnOperation(HttpListenerContext context, Authentication authentication)
		{
			string id = context.Request.QueryString["id"];

			if (!UploadedModsManager.Instance.HasModWithIdBeenUploaded(id))
			{
				ImageConverter converter = new ImageConverter();

				byte[] data = (byte[])converter.ConvertTo(Properties.Resources.cross, typeof(byte[]));
				context.Response.ContentLength64 = data.LongLength;
				context.Response.OutputStream.Write(data, 0, data.Length);
				context.Response.Close();
				return;
			}

			ModInfo modInfo = UploadedModsManager.Instance.GetModInfoFromId(id);

			if (!modInfo.HasImage)
			{
				ImageConverter converter = new ImageConverter();

				byte[] data = (byte[])converter.ConvertTo(Properties.Resources.cross, typeof(byte[]));
				context.Response.ContentLength64 = data.LongLength;
				context.Response.OutputStream.Write(data, 0, data.Length);
				context.Response.Close();
				return;
			}


			string imageFilePath = UploadedModsManager.Instance.GetModPathFromID(id) + modInfo.ImageFileName;

			string[] imageFilePathSplit = imageFilePath.Split('.');
			string format = imageFilePathSplit[imageFilePathSplit.Length-1].ToLower();
			if (format == "png")
			{
				context.Response.ContentType = "image/png";
			}
			else if(format == "jpg")
			{
				context.Response.ContentType = "image/jpeg";
			}
			else if(format == "gif")
			{
				context.Response.ContentType = "image/gif";
			}

			byte[] imageData = File.ReadAllBytes(imageFilePath);

			
			context.Response.ContentLength64 = imageData.LongLength;
			context.Response.OutputStream.Write(imageData, 0, imageData.Length);
			context.Response.Close();
		}

	}
}
