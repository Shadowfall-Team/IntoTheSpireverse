using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.DevConsole;
using MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Runs;
using Shadowfall.ShadowfallCode.Ammo;

namespace Shadowfall.ShadowfallCode.Commands;

/// <summary>
/// Usage:
///   ammo load [amount]   — gain ammo (default 1)
///   ammo fire            — enqueue PlayAmmoCardAction for the issuing player
///   ammo status          — print current ammo count
/// </summary>
public class AmmoConsoleCmd : AbstractConsoleCmd
{
    public override string CmdName => "ammo";
    public override string Args => "load [amount] | fire | status";
    public override string Description => "Load or fire ammo for testing the ammo system.";
    public override bool IsNetworked => false;

    public override CmdResult Process(Player? issuingPlayer, string[] args)
    {
        if (issuingPlayer == null)
            return new CmdResult(success: false, "This command requires an active run.");

        if (issuingPlayer.PlayerCombatState == null)
            return new CmdResult(success: false, "This command only works in combat.");

        if (args.Length == 0)
            return new CmdResult(success: false, $"Subcommand required. Usage: {CmdName} {Args}");

        return args[0].ToLowerInvariant() switch
        {
            "load" => HandleLoad(issuingPlayer, args),
            "fire" => HandleFire(issuingPlayer),
            "status" => HandleStatus(issuingPlayer),
            _ => new CmdResult(success: false, $"Unknown subcommand '{args[0]}'. Usage: {CmdName} {Args}")
        };
    }

    private static CmdResult HandleLoad(Player player, string[] args)
    {
        int amount = 1;
        if (args.Length > 1)
        {
            if (!int.TryParse(args[1], out amount) || amount <= 0)
                return new CmdResult(success: false, "Amount must be a positive integer.");
        }

        _ = AmmoResource.GainAmmo(amount, player);
        return new CmdResult(success: true, $"Loaded {amount} ammo.");
    }

    private static CmdResult HandleFire(Player player)
    {
        var ammo = AmmoResource.GetAmmo(player);
        if (ammo <= 0)
            return new CmdResult(success: false, "No ammo to fire.");

        if (player.PlayerCombatState!.Energy < 1)
            return new CmdResult(success: false, "Not enough energy to fire (need 1).");

        if (!CombatManager.Instance.IsInProgress)
            return new CmdResult(success: false, "Combat is not in progress.");

        RunManager.Instance.ActionQueueSynchronizer.RequestEnqueue(new PlayAmmoCardAction(player));
        return new CmdResult(success: true, $"PlayAmmoCardAction enqueued. Ammo before: {ammo}.");
    }

    private static CmdResult HandleStatus(Player player)
    {
        var ammo = AmmoResource.GetAmmo(player);
        var energy = player.PlayerCombatState!.Energy;
        return new CmdResult(success: true, $"Ammo: {ammo} | Energy: {energy}");
    }
}
