using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Shane.Church.StirlingBirthday.Core.Data
{
	public class MonthGroup : IComparable
	{
		public MonthGroup() { }
		public MonthGroup(DateTime date)
		{
			LongName = date.ToString("MMMM");
			ShortName = date.ToString("MMM");
			MonthIndex = date.Month - DateTime.Today.Month;
			if (MonthIndex < 0)
				MonthIndex += 12;
		}

		public string LongName { get; set; }
		public string ShortName { get; set; }
		public int MonthIndex { get; set; }

		public override bool Equals(object obj)
		{
			if (obj is MonthGroup)
				return ((MonthGroup)obj).MonthIndex == this.MonthIndex;
			else
				return false;
		}

		public override int GetHashCode()
		{
			return MonthIndex.GetHashCode();
		}

		public int CompareTo(object obj)
		{
			var compareObj = ((MonthGroup)obj);
			int differential = 0;
			if (this.MonthIndex > compareObj.MonthIndex)
				differential = -(this.MonthIndex - compareObj.MonthIndex);
			else if (this.MonthIndex < compareObj.MonthIndex)
				differential = compareObj.MonthIndex - this.MonthIndex;
			Debug.WriteLine(new { ThisIndex = this.MonthIndex, CompareIndex = ((MonthGroup)obj).MonthIndex, Differential = differential });
			return differential;
		}

		public override string ToString()
		{
			return LongName;
		}
	}

}
