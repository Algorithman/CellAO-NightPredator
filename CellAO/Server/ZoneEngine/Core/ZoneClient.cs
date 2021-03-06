﻿#region License

// Copyright (c) 2005-2013, CellAO Team
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//     * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
// EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#endregion

namespace ZoneEngine.Core
{
    #region Usings ...

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Sockets;

    using Cell.Core;

    using CellAO.Core.Components;
    using CellAO.Core.Entities;
    using CellAO.Core.EventHandlers.Events;
    using CellAO.Core.Network;
    using CellAO.Core.Playfields;
    using CellAO.Core.Vector;
    using CellAO.Database.Dao;
    using CellAO.Database.Entities;

    using NiceHexOutput;

    using SmokeLounge.AOtomation.Messaging.GameData;
    using SmokeLounge.AOtomation.Messaging.Messages;
    using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

    using Utility;

    using zlib;

    using Quaternion = CellAO.Core.Vector.Quaternion;

    #endregion

    /// <summary>
    /// </summary>
    public class ZoneClient : ClientBase, IZoneClient
    {
        #region Fields

        /// <summary>
        /// </summary>
        public IPlayfield Playfield;

        /// <summary>
        /// </summary>
        private readonly ZoneServer server;

        /// <summary>
        /// </summary>
        private IBus bus;

        /// <summary>
        /// </summary>
        private Character character;

        /// <summary>
        /// </summary>
        private IMessageSerializer messageSerializer;

        /// <summary>
        /// </summary>
        private NetworkStream netStream;

        /// <summary>
        /// </summary>
        private short packetNumber = 0;

        /// <summary>
        /// </summary>
        private ZOutputStream zStream;

        /// <summary>
        /// </summary>
        private bool zStreamSetup;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        /// <param name="server">
        /// </param>
        /// <param name="messageSerializer">
        /// </param>
        /// <param name="bus">
        /// </param>
        public ZoneClient(ZoneServer server, IMessageSerializer messageSerializer, IBus bus)
            : base(server)
        {
            this.server = server;
            this.messageSerializer = messageSerializer;
            this.bus = bus;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// </summary>
        public ICharacter Character
        {
            get
            {
                return this.character;
            }

            set
            {
                this.character = (Character)value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <param name="charId">
        /// </param>
        /// <exception cref="Exception">
        /// </exception>
        public void CreateCharacter(int charId)
        {
            this.character = new Character(this, new Identity { Type = IdentityType.CanbeAffected, Instance = charId });
            IEnumerable<DBCharacter> daochar = CharacterDao.GetById(charId);
            if (daochar.Count() == 0)
            {
                throw new Exception("Character " + charId.ToString() + " not found.");
            }

            if (daochar.Count() > 1)
            {
                throw new Exception(
                    daochar.Count().ToString() + " Characters with id " + charId.ToString()
                    + " found??? Check Database setup!");
            }

            DBCharacter character = daochar.First();
            this.character.Name = character.Name;
            this.character.LastName = character.LastName;
            this.character.FirstName = character.FirstName;
            this.character.Coordinates = new Coordinate(character.X, character.Y, character.Z);
            this.character.Heading = new Quaternion(
                character.HeadingX, 
                character.HeadingY, 
                character.HeadingZ, 
                character.HeadingW);
            this.character.Playfield = this.server.PlayfieldById(character.Playfield);
            this.Playfield = this.character.Playfield;
            this.Playfield.Entities.Add(this.character);
            this.character.Stats.Read();
            this.character.BaseInventory.Read();
        }

        /// <summary>
        /// </summary>
        /// <param name="text">
        /// </param>
        /// <returns>
        /// </returns>
        public bool SendChatText(string text)
        {
            var message = new ChatTextMessage
                          {
                              Identity = this.Character.Identity, 
                              Unknown = 0x00, 
                              Text = text, 
                              Unknown1 = 0x1000, 
                              Unknown2 = 0x00000000
                          };

            this.SendCompressed(message);
            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="messageBody">
        /// </param>
        public void SendCompressed(MessageBody messageBody)
        {
            var message = new Message
                          {
                              Body = messageBody, 
                              Header =
                                  new Header
                                  {
                                      MessageId = BitConverter.ToUInt16(new byte[] { 0xDF, 0xDF }, 0), 
                                      PacketType = messageBody.PacketType, 
                                      Unknown = 0x0001, 
                                      Sender = this.server.Id, 
                                      Receiver = this.Character.Identity.Instance
                                  }
                          };

            byte[] buffer = this.messageSerializer.Serialize(message);
            this.SendCompressed(buffer);
            LogUtil.Debug(messageBody.GetType().ToString());
            LogUtil.Debug(NiceHexOutput.Output(buffer));
        }

        /// <summary>
        /// </summary>
        /// <param name="buffer">
        /// </param>
        public void SendCompressed(byte[] buffer)
        {
            lock (this.zStream)
            {
                byte[] pn = BitConverter.GetBytes(this.packetNumber++);
                buffer[0] = pn[1];
                buffer[1] = pn[0];

                try
                {
                    // We can not be multithreaded here. packet numbers would be jumbled
                    this.zStream.Write(buffer, 0, buffer.Length);
                    this.zStream.Flush();
                }
                catch (Exception e)
                {
                    LogUtil.Debug("Error writing to zStream");
                    LogUtil.ErrorException(e);
                    this.server.DisconnectClient(this);
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="messageBody">
        /// </param>
        public void SendInitiateCompressionMessage(MessageBody messageBody)
        {
            // TODO: Investigate if reciever is a timestamp
            var message = new Message
                          {
                              Body = messageBody, 
                              Header =
                                  new Header
                                  {
                                      MessageId = 0xdfdf, 
                                      PacketType = messageBody.PacketType, 
                                      Unknown = 0x0001, 
                                      // TODO: Make compression choosable in config.xml
                                      Sender = 0x01000000, // 01000000 = uncompressed, 03000000 = compressed
                                      Receiver = 0 // this.character.Identity.Instance 
                                  }
                          };
            byte[] buffer = this.messageSerializer.Serialize(message);

#if DEBUG
            Colouring.Push(ConsoleColor.Green);
            Console.WriteLine(NiceHexOutput.Output(buffer));
            Colouring.Pop();
            LogUtil.Debug(NiceHexOutput.Output(buffer));
#endif
            this.packetNumber = 1;

            this.Send(buffer);

            // Now create the compressed stream
            try
            {
                if (!this.zStreamSetup)
                {
                    // Create the zStream
                    this.netStream = new NetworkStream(this.TcpSocket);
                    this.zStream = new ZOutputStream(this.netStream, zlibConst.Z_BEST_COMPRESSION);
                    this.zStream.FlushMode = zlibConst.Z_SYNC_FLUSH;
                    this.zStreamSetup = true;
                }
            }
            catch (Exception e)
            {
                LogUtil.ErrorException(e);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="segment">
        /// </param>
        /// <returns>
        /// </returns>
        protected uint GetMessageNumber(BufferSegment segment)
        {
            var messageNumberArray = new byte[4];
            messageNumberArray[3] = segment.SegmentData[16];
            messageNumberArray[2] = segment.SegmentData[17];
            messageNumberArray[1] = segment.SegmentData[18];
            messageNumberArray[0] = segment.SegmentData[19];
            uint reply = BitConverter.ToUInt32(messageNumberArray, 0);
            return reply;
        }

        /// <summary>
        /// </summary>
        /// <param name="segment">
        /// </param>
        /// <returns>
        /// </returns>
        protected uint GetMessageNumber(byte[] segment)
        {
            var messageNumberArray = new byte[4];
            messageNumberArray[3] = segment[16];
            messageNumberArray[2] = segment[17];
            messageNumberArray[1] = segment[18];
            messageNumberArray[0] = segment[19];
            uint reply = BitConverter.ToUInt32(messageNumberArray, 0);
            return reply;
        }

        /// <summary>
        /// </summary>
        /// <param name="buffer">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        protected override bool OnReceive(BufferSegment buffer)
        {
            Message message = null;

            var packet = new byte[this._remainingLength];
            Array.Copy(buffer.SegmentData, packet, this._remainingLength);

#if DEBUG
            Console.WriteLine("Receiving");
            Console.WriteLine("Offset: " + buffer.Offset.ToString() + " -- RemainingLength: " + this._remainingLength);
            Console.WriteLine(NiceHexOutput.Output(packet));
            LogUtil.Debug("\r\nReceived: \r\n" + NiceHexOutput.Output(packet));
#endif
            this._remainingLength = 0;
            try
            {
                message = this.messageSerializer.Deserialize(packet);
            }
            catch (Exception)
            {
                uint messageNumber = this.GetMessageNumber(packet);
                this.Server.Warning(
                    this, 
                    "Client sent malformed message {0}", 
                    messageNumber.ToString(CultureInfo.InvariantCulture));
                this.server.Warning(this, NiceHexOutput.Output(packet));
                return false;
            }

            buffer.IncrementUsage();

            if (message == null)
            {
                uint messageNumber = this.GetMessageNumber(packet);
                this.Server.Warning(
                    this, 
                    "Client sent unknown message {0}", 
                    messageNumber.ToString(CultureInfo.InvariantCulture));
                return false;
            }

            this.bus.Publish(new MessageReceivedEvent(this, message));

            return true;
        }

        #endregion
    }
}