﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormSample.ViewModel
{
	using System.ComponentModel;
	using System.Runtime.CompilerServices;

	public abstract class BaseViewModel : INotifyPropertyChanged
	{

		protected bool ChangeAndNotify<T>(ref T property, T value, [CallerMemberName] string propertyName = "")
		{
			if (!EqualityComparer<T>.Default.Equals(property, value))
			{
				property = value;
				NotifyPropertyChanged(propertyName);
				return true;
			}


			return false;
		}

		protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
