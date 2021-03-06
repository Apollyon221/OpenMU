﻿// -----------------------------------------------------------------------
// <copyright file="ItemModifyConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions
{
    using Interfaces;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// Consume handler to modify items which are specified by the target slot.
    /// </summary>
    public abstract class ItemModifyConsumeHandler : IItemConsumeHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemModifyConsumeHandler"/> class.
        /// </summary>
        /// <param name="persistenceContextProvider">The persistence context provider.</param>
        protected ItemModifyConsumeHandler(IPersistenceContextProvider persistenceContextProvider)
        {
            this.PersistenceContextProvider = persistenceContextProvider;
        }

        /// <summary>
        /// Gets the repository manager.
        /// </summary>
        protected IPersistenceContextProvider PersistenceContextProvider { get; }

        /// <inheritdoc/>
        public bool ConsumeItem(Player player, byte itemSlot, byte targetSlot)
        {
            if (player.PlayerState.CurrentState != PlayerState.EnteredWorld)
            {
                return false;
            }

            Item item = player.Inventory.GetItem(targetSlot);
            if (item == null)
            {
                return false;
            }

            if (item.ItemSlot <= InventoryConstants.LastEquippableItemSlotIndex)
            {
                // It shouldn't be possible to upgrade an equipped item.
                // The original server allowed this, however people managed to downgrade their maxed out weapons to +6 when some
                // visual bugs on the client occured :D Example: On the server side there is a jewel of bless on a certain slot,
                // but client shows a health potion. When the client then consumes the potion it would apply the bless to item slot 0.
                return false;
            }

            if (!this.ModifyItem(item, player.PersistenceContext))
            {
                return false;
            }

            player.PlayerView.InventoryView.ItemUpgraded(item);
            return true;
        }

        /// <summary>
        /// Modifies the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="persistenceContext">The persistence context.</param>
        /// <returns>Flag indicating whether the modification of the item occured.</returns>
        protected abstract bool ModifyItem(Item item, IContext persistenceContext);
    }
}
