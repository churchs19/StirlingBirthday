using System;
using System.Collections.Generic;

namespace Shane.Church.StirlingBirthday.ViewModels
{
	public class Group<T> : IEnumerable<T>
	{
		public Group(string name, string shortName, IEnumerable<T> items)
		{
			this.Title = name;
			this.ShortTitle = shortName;
			try
			{
				this.Items = new List<T>(items);
			}
			catch (ArgumentOutOfRangeException)
			{
				this.Items = new List<T>();
			}
		}

		public override bool Equals(object obj)
		{
			Group<T> that = obj as Group<T>;

			return (that != null) && (this.Title.Equals(that.Title));
		}

		public string Title
		{
			get;
			set;
		}

		public string ShortTitle
		{
			get;
			set;
		}

		public IList<T> Items
		{
			get;
			set;
		}

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			return this.Items.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.Items.GetEnumerator();
		}

		#endregion
	}
}
