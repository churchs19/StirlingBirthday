using Microsoft.ApplicationInsights.Telemetry.WindowsStore;
using Shane.Church.StirlingBirthday.Core.Data;
using Shane.Church.StirlingBirthday.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shane.Church.StirlingBirthday.Core.WP.Services
{
    public class PhoneLoggingService : ILoggingService
    {
		public void LogMessage(string message)
		{
			ClientAnalyticsChannel.Default.LogEvent(message);
		}

		public void LogException(Exception ex, string message = null)
		{
			var properties = new Dictionary<string, object>() { { "exception", ex } };
			if (message == null)
			{

				ClientAnalyticsChannel.Default.LogEvent("Exception - " + ex.Message, properties);
			}
			else
			{
				ClientAnalyticsChannel.Default.LogEvent("Exception - " + message, properties);
			}
		}

		public void LogPurchaseComplete(ProductPurchaseInfo purchaseInfo)
		{
			var iap = new Dictionary<string, object>()
			{ { "ProductId", purchaseInfo.ProductId },
				{ "ProductName", purchaseInfo.ProductName },
				{ "CommerceEngine", purchaseInfo.CommerceEngine },
				{ "CurrentMarket", purchaseInfo.CurrentMarket },
				{ "Currency", purchaseInfo.Currency },
				{ "Price", purchaseInfo.Price } };
			ClientAnalyticsChannel.Default.LogEvent("In App Purchase Complete", iap);
		}


		public void LogPageView(string page)
		{
			ClientAnalyticsChannel.Default.LogPageView(page);
		}
    }
}
