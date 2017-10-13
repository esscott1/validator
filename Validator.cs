using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TextfileValidator
{
	class Validator
	{
		Dictionary<string, Func<string, ItemDefintion, bool>> ValidationStrategy =
			new Dictionary<string, Func<string, ItemDefintion, bool>>()
			{ 
				{"int",Validator.IsInt},
				{"integer",Validator.IsInt},
				{"bigint",Validator.IsBigInt},
				{"decimal",Validator.IsDecimal},
				{"char",Validator.IsChar},
				{"varchar",Validator.IsVarchar},
				{"timestamp",Validator.IsTimeStamp},
				{"date",Validator.IsDate}
			};
		
		public bool IsValid(string item, ItemDefintion definition)
		{
			bool result = ValidationStrategy[definition.Type.ToLower()](item, definition);
			return result;
		}
	
		private static bool IsInt(string item, ItemDefintion definition )
		{
			int result;
			return (int.TryParse(item, out result));
			
		}
		private static bool IsBigInt(string item, ItemDefintion definition)
		{
			Int64 result;
			return Int64.TryParse(item, out result);
		}

		private static bool IsDecimal(string item, ItemDefintion definition)
		{
			Decimal result;
			return Decimal.TryParse(item, out result);
		}

		private static bool IsChar(string item, ItemDefintion definition)
		{
			if(!string.IsNullOrWhiteSpace(item))
				return !(item.Length > 1);
			return true;
		}

		private static bool IsVarchar(string item, ItemDefintion definition)
		{
			return !definition.Length.HasValue || item.Length <= definition.Length;
		}

		private static bool IsTimeStamp(string item, ItemDefintion definition)
		{
			return IsDate(item, definition);
		}
		private static bool IsDate(string item, ItemDefintion definition)
		{
			DateTime result;
			return DateTime.TryParse(item, out result);
		}

		

	}

	class ItemDefintion
	{
        private static Regex varcharLimitedLengthRegex = new Regex(@"varchar\((\d+)\)");
        public ItemDefintion()
		{ }
		public ItemDefintion(string type)
		{
            originalType = type;
            Match m = varcharLimitedLengthRegex.Match(type);
            if (m.Success)
            {
                Type = "varchar";
                Length = int.Parse(m.Groups[1].Value);
            }
            else
            {
                Type = type;
                Length = null;
            }
		}
		public string Type { get; set; }
        public string Precision { get; set; }
		public int? Length {get;set;}
		public string scale {get;set;}
        public string originalType { get; set; }

	}

}
