using System;

namespace Domain.Api.Base
{
    public abstract class BaseApiService
    {
        protected readonly IGlobalUIService GlobalUI;

        protected BaseApiService(IGlobalUIService globalUI) => GlobalUI = globalUI;

        protected void HandleTechnicalError(Exception ex)
        {
            if (ex.Message == "NETWORK_ERROR")
            {
                LoggerService.Warning("[BaseApi] Network connection lost.");
                GlobalUI.ShowNetworkError();
            }
        }

        protected bool IsNetworkError(Exception ex)
        {
            // Sesuaikan dengan logic HttpClient Anda. Biasanya:
            // return ex is SocketException || ex is TimeoutException || ex.Message.Contains("NETWORK_ERROR");
            return ex.Message == "NETWORK_ERROR";
        }
    }
}