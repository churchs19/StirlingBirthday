using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shane.Church.StirlingBirthday.Core.Exceptions
{
	public class AgentManagementException : Exception
	{
		public AgentManagementException()
			: base()
		{

		}

		public AgentManagementException(string message)
			: base(message)
		{

		}

		public AgentManagementException(string message, Exception innerException)
			: base(message, innerException)
		{

		}
	}
}
