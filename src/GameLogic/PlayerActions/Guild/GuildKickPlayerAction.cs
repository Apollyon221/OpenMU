﻿// <copyright file="GuildKickPlayerAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Guild
{
    using Interfaces;
    using MUnique.OpenMU.Interfaces;
    using Views;

    /// <summary>
    /// Action to kick a player out of a guild.
    /// </summary>
    public class GuildKickPlayerAction
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(GuildKickPlayerAction));

        private readonly IGameServerContext gameContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuildKickPlayerAction"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public GuildKickPlayerAction(IGameServerContext gameContext)
        {
            this.gameContext = gameContext;
        }

        /// <summary>
        /// Kicks the player out of the guild.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="nickname">The nickname.</param>
        /// <param name="securityCode">The security code.</param>
        public void KickPlayer(Player player, string nickname, string securityCode)
        {
            if (player.Account.SecurityCode != null && player.Account.SecurityCode != securityCode)
            {
                player.PlayerView.ShowMessage("Wrong Security Code.", MessageType.BlueNormal);
                Log.DebugFormat("Wrong Security Code: [{0}] <> [{1}], Player: {2}", securityCode, player.Account.SecurityCode, player.SelectedCharacter.Name);

                player.PlayerView.GuildView.GuildKickResult(GuildKickSuccess.Failed);
                return;
            }

            var isKickingHimself = player.SelectedCharacter.Name == nickname;
            if (!isKickingHimself && player.GuildStatus?.Position != GuildPosition.GuildMaster)
            {
                Log.WarnFormat("Suspicious kick request for player with name: {0} (player is not a guild master) to kick {1}, could be hack attempt.", player.Name, nickname);
                player.PlayerView.GuildView.GuildKickResult(GuildKickSuccess.Failed);
                return;
            }

            if (isKickingHimself && player.GuildStatus?.Position == GuildPosition.GuildMaster)
            {
                var guildId = player.GuildStatus.GuildId;
                this.gameContext.GuildServer.KickMember(guildId, nickname);
                this.gameContext.GuildCache.Invalidate(guildId);
                player.GuildStatus = null;
                player.PlayerView.GuildView.GuildKickResult(GuildKickSuccess.GuildDisband);
                return;
            }

            this.gameContext.GuildServer.KickMember(player.GuildStatus.GuildId, nickname);
        }
    }
}
