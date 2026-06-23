using Content.Shared._Stalker.WarZone;
using Robust.Shared.Prototypes;

namespace Content.Server._Stalker.WarZone;

/// <summary>
/// Spawns entities based on the current owner (faction/band) of a war zone.
/// </summary>
[RegisterComponent, Access(typeof(WarZoneSpawnerSystem))]
public sealed partial class WarZoneSpawnerComponent : Component
{
    /// <summary>
    /// War zone prototype that this spawner is bound to.
    /// </summary>
    [DataField("zoneProto")]
    public ProtoId<STWarZonePrototype> ZoneProto = default!;

    /// <summary>
    /// List of entities to spawn for each faction/band owner.
    /// Key is the faction or band proto ID, value is a list of entity proto IDs to spawn.
    /// </summary>
    [DataField]
    public Dictionary<string, List<EntProtoId>> OwnerSpawns = new();

    /// <summary>
    /// List of entities to spawn when the zone is uncaptured (neutral).
    /// </summary>
    [DataField]
    public List<EntProtoId> NeutralSpawns = new();

    /// <summary>
    /// Minimum number of entities to spawn.
    /// </summary>
    [DataField]
    public int MinSpawnCount = 1;

    /// <summary>
    /// Maximum number of entities to spawn.
    /// </summary>
    [DataField]
    public int MaxSpawnCount = 1;

    /// <summary>
    /// Maximum offset in tiles from the zone center.
    /// </summary>
    [DataField]
    public float SpawnRadius = 5f;

    /// <summary>
    /// Cooldown between spawns in seconds.
    /// </summary>
    [DataField]
    public float SpawnCooldown = 300f;

    /// <summary>
    /// Last spawn time for cooldown tracking.
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public TimeSpan? LastSpawnTime = null;

    /// <summary>
    /// Last known owner (faction or band ID) to detect changes.
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public string? LastKnownOwner = null;

    /// <summary>
    /// Whether to spawn immediately when zone is captured.
    /// </summary>
    [DataField]
    public bool SpawnOnCapture = true;

    /// <summary>
    /// Whether to clear previously spawned entities when owner changes.
    /// </summary>
    [DataField]
    public bool ClearOnOwnerChange = false;

    /// <summary>
    /// Track spawned entities to optionally clear them later.
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public HashSet<EntityUid> SpawnedEntities = new();
}
