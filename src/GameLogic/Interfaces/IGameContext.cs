﻿// <copyright file="IGameContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Interfaces
{
    using System.Collections.Generic;
    using DataModel.Configuration;
    using Persistence;

    /// <summary>
    /// The context of the game.
    /// </summary>
    public interface IGameContext
    {
        /// <summary>
        /// Gets the repository manager. Used to retrieve data, e.g. from a database.
        /// </summary>
        IPersistenceContextProvider PersistenceContextProvider { get; }

        /// <summary>
        /// Gets the item power up factory.
        /// </summary>
        IItemPowerUpFactory ItemPowerUpFactory { get; }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        GameConfiguration Configuration { get; }

        /// <summary>
        /// Gets the players of the game in a list.
        /// </summary>
        IList<Player> PlayerList { get; }

        /// <summary>
        /// Gets the maps which is meant to be hosted by the game.
        /// </summary>
        /// <param name="mapId">The map identifier.</param>
        /// <returns>The hosted GameMap instance.</returns>
        GameMap GetMap(ushort mapId);

        /// <summary>
        /// Gets the player object by character name.
        /// </summary>
        /// <param name="name">The character name.</param>
        /// <returns>The player object.</returns>
        Player GetPlayerByCharacterName(string name);
    }
}