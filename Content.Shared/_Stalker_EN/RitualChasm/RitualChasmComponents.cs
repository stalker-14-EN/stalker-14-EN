using System.Numerics;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Stalker_EN.RitualChasm;

/// <summary>
///     Chasm that accepts artifacts in return for
///         reward that matches with tier
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class RitualChasmComponent : Component
{
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan RelocatedStunDuration = TimeSpan.Zero;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan RelocatedFlashDuration = TimeSpan.Zero;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float ThrowForce = 25f;

    /// <summary>
    ///     Multiplied by tier of given artifact,
    ///         and then floored to determine how many entities to throw back
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float RewardedPerTier = 1.4f;

    /// <summary>
    ///     Multiplied by amount of teeth of given people,
    ///         and then floored to determine how many entities to throw back
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float RewardedPerTooth = 6f;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public EntProtoId RewardedEntityProtoId = "ToothStalker";

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public SoundSpecifier? ThrowSound = null;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public SoundSpecifier? FallSound = null;

    /// <summary>
    ///     Emote played by something falling into the chasm if alive and can emote.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public ProtoId<EmotePrototype>? FallEmote = null;

    /// <summary>
    ///     Sound played after something is relocated.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public SoundSpecifier? RelocateSound = null;

    /// <summary>
    ///     Sound played, only for something being relocated,
    ///         when it is being relocated.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public SoundSpecifier? RelocatedLocalSound = null;

    /// <summary>
    ///     Locale for popup shown for thing being relocated.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public string RelocatedLocalPopup = string.Empty;

    /// <summary>
    ///     If an entity is falling, it will be
    ///         relocated to somewherever when entering the chasm.
    ///         Otherwise the thing will just be deleted.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public EntityWhitelist RelocatableEntities = new();

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public EntityWhitelist PunishedEntities = new();

    /// <summary>
    ///     Things currently falling into the chasm, and their expected time of total descent.
    /// </summary>
    public Queue<(EntityUid, TimeSpan)> FallQueue = new();

    /// <summary>
    ///     It is assumed that first entity to leave will have lowest time-value,
    ///         and last to leave will have highest time-value, in order
    /// </summary>
    public Queue<(EntityUid?, Vector2, TimeSpan)> ThrowBackQueue = new();

    /// <summary>
    ///    Entities that are currently pending to be thrown back by the chasm, which
    ///         are in nullspace. They will be deleted if not yet thrown AND the chasm gets deleted.
    /// </summary>
    public List<EntityUid> EntitiesPendingThrowBack = new();
}

[RegisterComponent, NetworkedComponent]
public sealed partial class CreatedByRitualChasmComponent : Component;

[RegisterComponent, NetworkedComponent]
public sealed partial class DontStartCollideWithRitualChasmOnceComponent : Component;

/// <summary>
///     Exit point for things relocated by ritual chasm
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class RitualChasmExitPointComponent : Component;
