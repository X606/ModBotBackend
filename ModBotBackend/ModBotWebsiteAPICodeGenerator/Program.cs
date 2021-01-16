using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ModBotBackend;
using System.Reflection;
using ModBotWebsiteAPI;

namespace ModBotWebsiteAPICodeGenerator
{
	class Program
	{
		static void Main(string[] args)
		{
			string path = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName + "/ModBotWebsiteAPI/GeneratedCode/";
			string templatePath = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName + "/ModBotWebsiteAPI/Template.csTemplate";

			if (!Directory.Exists(path))
			{
				Console.WriteLine("Generated path is invalid \"" + path + "\"");
				Console.ReadLine();
				return;
			}
				
			if (!File.Exists(templatePath))
			{
				Console.WriteLine("Template path is invalid \"" + templatePath + "\"");
				Console.ReadLine();
				return;
			}
				

			string template = File.ReadAllText(templatePath);

			foreach (string file in Directory.GetFiles(path))
			{
				File.Delete(file);
			}

			Dictionary<string, OperationBase> _operations = new Dictionary<string, OperationBase>();
			Populate(_operations);
			foreach (KeyValuePair<string, OperationBase> operation in _operations)
			{
				Console.WriteLine("Generating code for operation \"" + operation.Key + "\"...");

				string formatedPrefix = Utils.FormatString(Properties.Resources.Prefix,
					operation.Key,
					string.Join(", ", operation.Value.Arguments),
					operation.Value.ArgumentsInQuerystring.ToString(),
					operation.Value.HideInAPI.ToString(),
					operation.Value.MinimumAuthenticationLevelToCall.ToString(),
					operation.Value.ParseAsJson.ToString()
				);
				
				bool shouldIncludeCodeForOperation = true;
				string reasonForNotIncluding = "";

				if (operation.Value.OverrideAPICallJavascript != null || operation.Value.OverrideResolveJavascript != null)
				{
					shouldIncludeCodeForOperation = false;
					reasonForNotIncluding = "it contained custom javascript code, and we cant convert that to c#";
				} else if (operation.Value.HideInAPI)
				{
					shouldIncludeCodeForOperation = false;
					reasonForNotIncluding = "HideInAPI was set to true";
				}

				string formatedCode;
				if (shouldIncludeCodeForOperation)
				{
					StringBuilder argumentsDefintionBuilder = new StringBuilder();
					StringBuilder argumentsCallingBuilder = new StringBuilder();
					for (int i = 0; i < operation.Value.Arguments.Length; i++)
					{
						argumentsDefintionBuilder.Append("string ");
						argumentsDefintionBuilder.Append(operation.Value.Arguments[i]);
						argumentsCallingBuilder.Append(operation.Value.Arguments[i]);

						argumentsDefintionBuilder.Append(", ");
						argumentsCallingBuilder.Append(", ");
					}

					string privateMethodName = operation.Key;
					privateMethodName = "_" + privateMethodName[0].ToString().ToLower() + privateMethodName.Substring(1);

					string publicMethodName = operation.Key;
					publicMethodName = publicMethodName[0].ToString().ToUpper() + publicMethodName.Substring(1);

					StringBuilder codeStringBuilder = new StringBuilder();
					codeStringBuilder.Append("url += \"" + operation.Key + "");

					if (operation.Value.ArgumentsInQuerystring)
					{
						for (int i = 0; i < operation.Value.Arguments.Length; i++)
						{
							codeStringBuilder.Append("&");
							codeStringBuilder.Append(operation.Value.Arguments[i]);
							codeStringBuilder.Append("=\" + ");
							codeStringBuilder.Append(operation.Value.Arguments[i]);
							
							if (i != (operation.Value.Arguments.Length-1))
							{
								codeStringBuilder.Append(" + \"");
							}
						}
						codeStringBuilder.Append(";\r\n");
						codeStringBuilder.Append("\t\t\t");
						codeStringBuilder.Append("data = \"{}\";\r\n");

					} else
					{
						codeStringBuilder.Append("\";\r\n");

						codeStringBuilder.Append("\t\t\t");
						codeStringBuilder.Append("JsonConstructor json = new JsonConstructor();");
						for (int i = 0; i < operation.Value.Arguments.Length; i++)
						{
							codeStringBuilder.Append("\r\n\t\t\t");
							codeStringBuilder.Append("json.AppendValue(\"");
							codeStringBuilder.Append(operation.Value.Arguments[i]);
							codeStringBuilder.Append("\", ");
							codeStringBuilder.Append(operation.Value.Arguments[i]);
							codeStringBuilder.Append(");");
						}
						codeStringBuilder.Append("\r\n\t\t\t");
						codeStringBuilder.Append("data = json.ToString();");
					}

					/*
					
					JsonConstructor json = new JsonConstructor();
					json.AppendValue("modId", modId);
					data = json.ToString();
					
					 */

					formatedCode = Utils.FormatString(template,
						privateMethodName,
						publicMethodName,
						argumentsDefintionBuilder.ToString(),
						argumentsCallingBuilder.ToString(),
						codeStringBuilder.ToString(),
						operation.Value.ParseAsJson ? "JsonObject" : "string"
					);
				}
				else 
				{
					formatedCode = "//This operation is not included in the API becuase " + reasonForNotIncluding;
				}



				File.WriteAllText(path + operation.Key + ".cs", formatedPrefix + formatedCode);
			}

			Console.WriteLine("Done!");
		}

		static void Populate(Dictionary<string, OperationBase> _operations)
		{
			_operations.Clear();

			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			
			for (int i = 0; i < assemblies.Length; i++)
			{
				Type[] loadedTypes = assemblies[i].GetTypes();
				for (int j = 0; j < loadedTypes.Length; j++)
				{
					OperationAttribute operationAttribute = (OperationAttribute)Attribute.GetCustomAttribute(loadedTypes[j], typeof(OperationAttribute));
					if (operationAttribute == null)
						continue;

					if (loadedTypes[j].BaseType != typeof(OperationBase))
						continue;

					_operations.Add(operationAttribute.OperationKey, (OperationBase)Activator.CreateInstance(loadedTypes[j]));
				}
			}

		}
	}
}
