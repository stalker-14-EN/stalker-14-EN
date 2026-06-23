using System.Numerics;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Content.Shared._Stalker.WarZone;

namespace Content.Server._Stalker.WarZone;

/// <summary>
/// System for spawning entities based on the current owner of a war zone.
/// </summary>
public sealed class WarZoneSpawnerSystem : EntitySystem
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly TransformSystem _transform = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<WarZoneSpawnerComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<WarZoneSpawnerComponent, ComponentShutdown>(OnComponentShutdown);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<WarZoneSpawnerComponent>();
        while (query.MoveNext(out var uid, out var spawner))
        {
            UpdateWarZoneSpawner(uid, spawner, frameTime);
        }
    }

    private void OnComponentInit(Entity<WarZoneSpawnerComponent> entity, ref ComponentInit args)
    {
        var comp = entity.Comp;
        var ownerId = GetZoneOwner(comp);
        comp.LastKnownOwner = ownerId;

        if (comp.SpawnOnCapture && ownerId != null)
        {
            SpawnForOwner(entity.Owner, comp, ownerId);
        }
    }

    private void OnComponentShutdown(Entity<WarZoneSpawnerComponent> entity, ref ComponentShutdown args)
    {
        ClearSpawnedEntities(entity.Comp);
    }

    private void UpdateWarZoneSpawner(EntityUid uid, WarZoneSpawnerComponent spawner, float frameTime)
    {
        var currentOwner = GetZoneOwner(spawner);

        if (currentOwner != spawner.LastKnownOwner)
        {
            spawner.LastKnownOwner = currentOwner;

            if (spawner.ClearOnOwnerChange)
            {
                ClearSpawnedEntities(spawner);
            }

            if (currentOwner != null)
            {
                SpawnForOwner(uid, spawner, currentOwner);
            }
            else if (spawner.NeutralSpawns.Count > 0)
            {
                SpawnNeutral(uid, spawner);
            }
        }
    }

    private string? GetCurrentOwner(WarZoneComponent warZone)
    {
        if (warZone.DefendingBandProtoId != null)
            return warZone.DefendingBandProtoId;

        if (warZone.DefendingFactionProtoId != null)
            return warZone.DefendingFactionProtoId;

        return null;
    }

    private string? GetZoneOwner(WarZoneSpawnerComponent spawner)
    {
        var query = EntityQueryEnumerator<WarZoneComponent>();
        while (query.MoveNext(out _, out var warZone))
        {
            if (warZone.ZoneProto == spawner.ZoneProto)
            {
                return GetCurrentOwner(warZone);
            }
        }

        Logger.WarningS("warzone-spawner", $"No WarZone entity found for zoneProto '{spawner.ZoneProto}' on spawner {spawner}");
        return null;
    }

    private void SpawnForOwner(EntityUid uid, WarZoneSpawnerComponent comp, string ownerId)
    {
        if (!comp.OwnerSpawns.TryGetValue(ownerId, out var protos) || protos.Count == 0)
        {
            Logger.WarningS("warzone-spawner", $"No spawn prototypes defined for owner '{ownerId}' for spawner {uid}");
            return;
        }

        var spawnCount = _random.Next(comp.MinSpawnCount, comp.MaxSpawnCount + 1);
        for (int i = 0; i < spawnCount; i++)
        {
            var proto = _random.Pick(protos);
            SpawnEntity(uid, comp, proto);
        }

        comp.LastSpawnTime = _gameTiming.CurTime;
    }

    private void SpawnNeutral(EntityUid uid, WarZoneSpawnerComponent comp)
    {
        if (comp.NeutralSpawns.Count == 0)
            return;

        var spawnCount = _random.Next(comp.MinSpawnCount, comp.MaxSpawnCount + 1);
        for (int i = 0; i < spawnCount; i++)
        {
            var proto = _random.Pick(comp.NeutralSpawns);
            SpawnEntity(uid, comp, proto);
        }

        comp.LastSpawnTime = _gameTiming.CurTime;
    }

    private void SpawnEntity(EntityUid uid, WarZoneSpawnerComponent comp, EntProtoId protoId)
    {
        var transform = _entityManager.GetComponent<TransformComponent>(uid);
        var spawnPos = transform.WorldPosition;

        // Add random offset
        var angle = _random.NextFloat(0f, MathF.PI * 2);
        var distance = _random.NextFloat(0f, comp.SpawnRadius);
        var offset = new Vector2(MathF.Cos(angle) * distance, MathF.Sin(angle) * distance);
        spawnPos += offset;

        var spawnedUid = _entityManager.SpawnEntity(protoId, new MapCoordinates(spawnPos, transform.MapID));
        comp.SpawnedEntities.Add(spawnedUid);

        Logger.DebugS("warzone-spawner", $"Spawned {protoId} at {spawnPos} for zone {uid}");
    }

    private void ClearSpawnedEntities(WarZoneSpawnerComponent spawner)
    {
        foreach (var uid in spawner.SpawnedEntities)
        {
            if (_entityManager.EntityExists(uid))
            {
                _entityManager.DeleteEntity(uid);
            }
        }

        spawner.SpawnedEntities.Clear();
    }
}
