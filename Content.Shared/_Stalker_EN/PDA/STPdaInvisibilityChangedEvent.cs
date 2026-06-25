using Robust.Shared.Serialization;

namespace Content.Shared._Stalker_EN.PDA;

[Serializable, NetSerializable]
public sealed class PdaInvisibilityChangedEvent(bool enabled) : EntityEventArgs
{
    public bool Enabled = enabled;
}
