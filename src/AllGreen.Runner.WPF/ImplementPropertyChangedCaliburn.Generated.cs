﻿using System;

namespace TemplateAttributes
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	internal class ImplementPropertyChangedCaliburnAttribute : Attribute
	{
		public ImplementPropertyChangedCaliburnAttribute(Type aInterface)
		{
			Interface = aInterface;
		}
		public Type Interface { get; private set; }
	}
}
