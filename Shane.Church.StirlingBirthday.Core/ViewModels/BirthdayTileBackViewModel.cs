﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using GalaSoft.MvvmLight;

namespace Shane.Church.StirlingBirthday.ViewModels
{
	public class BirthdayTileBackViewModel : ObservableObject
	{

		private string _name1;
		public string Name1
		{
			get { return _name1; }
			set
			{
				Set(() => Name1, ref _name1, value);
			}
		}

		private string _name2;
		public string Name2
		{
			get { return _name2; }
			set
			{
				Set(() => Name2, ref _name2, value);
			}
		}

		private string _name3;
		public string Name3
		{
			get { return _name3; }
			set
			{
				Set(() => Name3, ref _name3, value);
			}
		}

		private string _date1;
		public string Date1
		{
			get { return _date1; }
			set
			{
				Set(() => Date1, ref _date1, value);
			}
		}

		private string _date2;
		public string Date2
		{
			get { return _date2; }
			set
			{
				Set(() => Date2, ref _date2, value);
			}
		}

		private string _date3;
		public string Date3
		{
			get { return _date3; }
			set
			{
				Set(() => Date3, ref _date3, value);
			}
		}
	}
}