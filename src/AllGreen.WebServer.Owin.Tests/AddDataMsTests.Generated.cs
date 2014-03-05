using System;

namespace TemplateAttributes
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage()]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	internal class DataTestMethodAttribute : Attribute
	{
		public DataTestMethodAttribute(params object[] args)
		{
			if (args == null)
				Arguments = new object[] { null };
			else
				Arguments = args;
		}
		public object[] Arguments { get; private set; }
		public object Result { get; set; }
		public string Message { get; set; }
		public string TestName { get; set; }
		public string Description { get; set; }
		public Type ExpectedException { get; set; }
		public bool Ignore { get; set; }
		public int Priority { get; set; }
		public int Timeout { get; set; }
		public int WorkItem { get; set; }
	}

	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage()]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	internal class DataRowAttribute : Attribute
	{
		public DataRowAttribute(params object[] args)
		{
			Arguments = args;
		}
		public object[] Arguments { get; private set; }
		public string DisplayName { get; set; }
	}
}
