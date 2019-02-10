﻿// <copyright file="ChatView.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView
{
    using System.Text;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Network;
    using Network.Interfaces;

    /// <summary>
    /// The default implementation of the chat view which is forwarding everything to the game client which specific data packets.
    /// </summary>
    public class ChatView : IChatView
    {
        private readonly IConnection connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatView"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public ChatView(IConnection connection)
        {
            this.connection = connection;
        }

        /// <inheritdoc/>
        public void ChatMessage(string message, string sender, ChatMessageType type)
        {
            using (var writer = this.connection.StartSafeWrite(0xC1, Encoding.UTF8.GetByteCount(message) + 14))
            {
                var packet = writer.Span;
                packet[2] = this.GetChatMessageTypeByte(type);
                packet.Slice(3, 10).WriteString(sender, Encoding.UTF8);
                packet.Slice(13).WriteString(message, Encoding.UTF8);
                writer.Commit();
            }
        }

        private byte GetChatMessageTypeByte(ChatMessageType type)
        {
            if (type == ChatMessageType.Whisper)
            {
                return 0x02;
            }

            return 0x00;
        }
    }
}
