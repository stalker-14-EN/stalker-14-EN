using Content.Shared._Stalker_EN.PDA;
using Robust.Server.Player;
using Robust.Shared.Network;

namespace Content.Server._Stalker_EN.PDA;

public sealed class STPdaInvisibilityModeSystem : EntitySystem
{
    private readonly HashSet<string> _invisibleUsers = new();

    [Dependency] private readonly IPlayerManager _playerManager = default!;

    public override void Initialize()
    {
        SubscribeNetworkEvent<PdaInvisibilityChangedEvent>(OnInvisibilityChanged);
    }

    private void OnInvisibilityChanged(PdaInvisibilityChangedEvent args, EntitySessionEventArgs session)
    {
        var username = session.SenderSession.Name;
        if (args.Enabled)
            _invisibleUsers.Add(username);
        else
            _invisibleUsers.Remove(username);
    }

    public bool IsInvisible(Guid userId)
    {
        if (!_playerManager.TryGetSessionById(new NetUserId(userId), out var session))
            return false;

        return _invisibleUsers.Contains(session.Name);
    }
}
