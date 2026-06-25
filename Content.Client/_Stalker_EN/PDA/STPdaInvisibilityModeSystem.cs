using Content.Shared._Stalker_EN.PDA;
using Content.Shared._Stalker.CCCCVars;
using Robust.Shared.Configuration;

namespace Content.Client._Stalker_EN.PDA;

public sealed class STPdaInvisibilityModeSystem : EntitySystem
{
    [Dependency] private readonly IConfigurationManager _configurationManager = default!;

    public override void Initialize()
    {
        _configurationManager.OnValueChanged(CCCCVars.PdaInvisibilityEnabled, OnInvisibilityChanged, invokeImmediately: true);
    }

    private void OnInvisibilityChanged(bool enabled)
    {
        RaiseNetworkEvent(new PdaInvisibilityChangedEvent(enabled));
    }
}
