﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ModBotBackend.Operations
{
	public class TestOperation : OperationBase
	{
		static int i = 1;

		public override void OnOperation(HttpListenerContext context)
		{
			//context.Response.Redirect("https://clonedronemodbot.com/error.html?error=test works&notError=true");
			HttpStream httpStream = new HttpStream(context.Response);
			//httpStream.Send("<head><title>Test</title></head><body><div style=\"height: 100px; background-color: red; margin-left: auto; margin-right: auto; margin-top: auto; margin-bottom: auto; width:" + i +"em;\"></div></body>");
			//httpStream.SendFile("D:/pictures/1uuutl7e3pt11.png");
			httpStream.Send("<head><title>Test</title></head><body> <form action=\"?operation=post\" enctype=\"multipart/form-data\" method=\"post\">Name: <input type=\"text\" name=\"name\"><br>E-mail: <input type=\"text\" name=\"email\"><br><input type=\"file\" name=\"file\"><br><input type=\"submit\"></form> </body>");
			httpStream.Close();

			i++;
		}

	}
}