
namespace Shane.Church.StirlingBirthday.Core.Services
{
	public interface IAgentManagementService
	{
		void StartAgent(bool debugAgent = false);
		void RemoveAgent();

		bool IsAgentEnabled { get; }
		bool AreAgentsSupported { get; }
	}
}
