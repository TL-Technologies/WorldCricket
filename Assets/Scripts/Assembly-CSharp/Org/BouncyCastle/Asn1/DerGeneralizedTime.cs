using System;
using System.Globalization;
using System.Text;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1
{
	public class DerGeneralizedTime : Asn1Object
	{
		private readonly string time;

		public string TimeString => time;

		private bool HasFractionalSeconds => time.IndexOf('.') == 14;

		public DerGeneralizedTime(string time)
		{
			this.time = time;
			try
			{
				ToDateTime();
			}
			catch (FormatException ex)
			{
				throw new ArgumentException("invalid date string: " + ex.Message);
			}
		}

		public DerGeneralizedTime(DateTime time)
		{
			this.time = time.ToString("yyyyMMddHHmmss\\Z");
		}

		internal DerGeneralizedTime(byte[] bytes)
		{
			time = Strings.FromAsciiByteArray(bytes);
		}

		public static DerGeneralizedTime GetInstance(object obj)
		{
			if (obj == null || obj is DerGeneralizedTime)
			{
				return (DerGeneralizedTime)obj;
			}
			throw new ArgumentException("illegal object in GetInstance: " + Platform.GetTypeName(obj), "obj");
		}

		public static DerGeneralizedTime GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			Asn1Object @object = obj.GetObject();
			if (isExplicit || @object is DerGeneralizedTime)
			{
				return GetInstance(@object);
			}
			return new DerGeneralizedTime(((Asn1OctetString)@object).GetOctets());
		}

		public string GetTime()
		{
			if (time[time.Length - 1] == 'Z')
			{
				return time.Substring(0, time.Length - 1) + "GMT+00:00";
			}
			int num = time.Length - 5;
			char c = time[num];
			if (c == '-' || c == '+')
			{
				return time.Substring(0, num) + "GMT" + time.Substring(num, 3) + ":" + time.Substring(num + 3);
			}
			num = time.Length - 3;
			c = time[num];
			if (c == '-' || c == '+')
			{
				return time.Substring(0, num) + "GMT" + time.Substring(num) + ":00";
			}
			return time + CalculateGmtOffset();
		}

		private string CalculateGmtOffset()
		{
			char c = '+';
			DateTime dateTime = ToDateTime();
			long num = dateTime.Ticks - dateTime.ToUniversalTime().Ticks;
			if (num < 0)
			{
				c = '-';
				num = -num;
			}
			int num2 = (int)(num / 36000000000L);
			int num3 = (int)(num / 600000000) % 60;
			return "GMT" + c + Convert(num2) + ":" + Convert(num3);
		}

		private static string Convert(int time)
		{
			if (time < 10)
			{
				return "0" + time;
			}
			return time.ToString();
		}

		public DateTime ToDateTime()
		{
			string text = time;
			bool makeUniversal = false;
			string format;
			if (Platform.EndsWith(text, "Z"))
			{
				if (HasFractionalSeconds)
				{
					int count = text.Length - text.IndexOf('.') - 2;
					format = "yyyyMMddHHmmss." + FString(count) + "\\Z";
				}
				else
				{
					format = "yyyyMMddHHmmss\\Z";
				}
			}
			else if (time.IndexOf('-') > 0 || time.IndexOf('+') > 0)
			{
				text = GetTime();
				makeUniversal = true;
				if (HasFractionalSeconds)
				{
					int count2 = Platform.IndexOf(text, "GMT") - 1 - text.IndexOf('.');
					format = "yyyyMMddHHmmss." + FString(count2) + "'GMT'zzz";
				}
				else
				{
					format = "yyyyMMddHHmmss'GMT'zzz";
				}
			}
			else if (HasFractionalSeconds)
			{
				int count3 = text.Length - 1 - text.IndexOf('.');
				format = "yyyyMMddHHmmss." + FString(count3);
			}
			else
			{
				format = "yyyyMMddHHmmss";
			}
			return ParseDateString(text, format, makeUniversal);
		}

		private string FString(int count)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < count; i++)
			{
				stringBuilder.Append('f');
			}
			return stringBuilder.ToString();
		}

		private DateTime ParseDateString(string s, string format, bool makeUniversal)
		{
			DateTimeStyles dateTimeStyles = DateTimeStyles.None;
			if (Platform.EndsWith(format, "Z"))
			{
				try
				{
					dateTimeStyles = (DateTimeStyles)(object)Enums.GetEnumValue(typeof(DateTimeStyles), "AssumeUniversal");
				}
				catch (Exception)
				{
				}
				dateTimeStyles |= DateTimeStyles.AdjustToUniversal;
			}
			DateTime dateTime = DateTime.ParseExact(s, format, DateTimeFormatInfo.InvariantInfo, dateTimeStyles);
			return (!makeUniversal) ? dateTime : dateTime.ToUniversalTime();
		}

		private byte[] GetOctets()
		{
			return Strings.ToAsciiByteArray(time);
		}

		internal override void Encode(DerOutputStream derOut)
		{
			derOut.WriteEncoded(24, GetOctets());
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			DerGeneralizedTime derGeneralizedTime = asn1Object as DerGeneralizedTime;
			if (derGeneralizedTime == null)
			{
				return false;
			}
			return time.Equals(derGeneralizedTime.time);
		}

		protected override int Asn1GetHashCode()
		{
			return time.GetHashCode();
		}
	}
}
