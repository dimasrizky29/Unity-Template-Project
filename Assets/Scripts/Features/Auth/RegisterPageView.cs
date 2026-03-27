using UnityEngine;

public class RegisterPageView : BasePagePanel
{
    public AuthView authView;

    public override void SetPayload(object payload)
    {
        base.SetPayload(payload);
        LoggerService.Info($"Payload: {payload}");
        // set payload do
        authView._referralCode = payload as string;
    }
}
