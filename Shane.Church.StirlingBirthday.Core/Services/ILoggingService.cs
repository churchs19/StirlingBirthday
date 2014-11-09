using Shane.Church.StirlingBirthday.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shane.Church.StirlingBirthday.Core.Services
{
    public interface ILoggingService
    {
        void LogMessage(string message);
        void LogException(Exception ex, string message = null);
        void LogPurchaseComplete(ProductPurchaseInfo purchaseInfo);
    }
}
