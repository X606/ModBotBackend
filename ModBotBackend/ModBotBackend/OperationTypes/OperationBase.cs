using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using ModBotBackend.Users;

namespace ModBotBackend
{
	public abstract class OperationBase
	{
		public abstract void OnOperation(HttpListenerContext context, Authentication authentication);

		public virtual byte[] GetResponseForError(Exception e, out string contentType)
        {
			contentType = "text/plain";
			return Encoding.UTF8.GetBytes("ERROR!\nError:\n" + e.ToString());
		}

		public abstract string[] Arguments { get; }
		public abstract bool ParseAsJson { get; }
		public abstract AuthenticationLevel MinimumAuthenticationLevelToCall { get; }

		public virtual bool ArgumentsInQuerystring => false;
		public virtual string OverrideResolveJavascript => null;
		public virtual string OverrideAPICallJavascript => null;
		public virtual bool HideInAPI => false;

		public Arguments GetArguments(HttpListenerContext context)
		{
			Dictionary<string, object> arguments = new Dictionary<string, object>();
			if (ArgumentsInQuerystring)
			{
				foreach (string key in context.Request.QueryString.AllKeys)
				{
					arguments.Add(key, context.Request.QueryString.Get(key));
				}
				return new Arguments(arguments);
			}
			else
			{
				if (!Utils.TryGetRequestBody(context, out Dictionary<string, object> dict))
				{
					return null;
				}

				return new Arguments(dict);
			}

		}
	}

	public class OperationAttribute : Attribute
	{
		public OperationAttribute(string operationKey)
		{
			OperationKey = operationKey;
		}
		public string OperationKey;
	}

	public class Arguments
    {
		Dictionary<string, object> _values = new Dictionary<string, object>();

		public Arguments(Dictionary<string,object> values)
        {
			_values = values;
        }

		public Argument GetArgument(string argument)
        {
			if (_values.TryGetValue(argument, out object value))
            {
				return new Argument(value);
            }

			return null;
        }

		public Argument this[string argument] => GetArgument(argument);

    }
	public class Argument
    {
		object _value;

		public Argument(object value)
        {
			_value = value;
        }

		public static implicit operator string(Argument argument) => argument != null ? Convert.ToString(argument._value) : default(string);
		public static implicit operator char(Argument argument) => argument != null ? Convert.ToChar(argument._value) : default(char);

		public static implicit operator sbyte(Argument argument) => argument != null ? Convert.ToSByte(argument._value) : default(sbyte);
		public static implicit operator short(Argument argument) => argument != null ? Convert.ToInt16(argument._value) : default(short);
		public static implicit operator int(Argument argument) => argument != null ? Convert.ToInt32(argument._value) : default(int);
		public static implicit operator long(Argument argument) => argument != null ? Convert.ToInt64(argument._value) : default(long);

		public static implicit operator byte(Argument argument) => argument != null ? Convert.ToByte(argument._value) : default(byte);
		public static implicit operator ushort(Argument argument) => argument != null ? Convert.ToUInt16(argument._value) : default(ushort);
		public static implicit operator uint(Argument argument) => argument != null ? Convert.ToUInt32(argument._value) : default(uint);
		public static implicit operator ulong(Argument argument) => argument != null ? Convert.ToUInt64(argument._value) : default(ulong);

		public static implicit operator bool(Argument argument) => argument != null ? Convert.ToBoolean(argument._value) : default(bool);
		
		public static implicit operator float(Argument argument) => argument != null ? Convert.ToSingle(argument._value) : default(float);
		public static implicit operator double(Argument argument) => argument != null ? Convert.ToDouble(argument._value) : default(double);
		public static implicit operator decimal(Argument argument) => argument != null ? Convert.ToDecimal(argument._value) : default(decimal);

		public static implicit operator string[](Argument argument) => argument != null ? (string[])argument._value : default(string[]);

		public static bool operator ==(Argument left, Argument right)
        {
			if (left is null || right is null)
				return left is null && right is null;

			return left._value == right._value;
        }

		public static bool operator !=(Argument left, Argument right)
        {
			return !(left == right);
        }
	}
}
