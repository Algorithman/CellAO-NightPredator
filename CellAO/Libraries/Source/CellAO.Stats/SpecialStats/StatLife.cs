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

namespace CellAO.Stats.SpecialStats
{
    #region Usings ...

    using System;

    using CellAO.Enums;

    #endregion

    /// <summary>
    /// </summary>
    public class StatLife : Stat
    {
        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        /// <param name="statList">
        /// </param>
        /// <param name="number">
        /// </param>
        /// <param name="defaultValue">
        /// </param>
        /// <param name="sendBaseValue">
        /// </param>
        /// <param name="dontWrite">
        /// </param>
        /// <param name="announceToPlayfield">
        /// </param>
        public StatLife(
            Stats statList, 
            int number, 
            uint defaultValue, 
            bool sendBaseValue, 
            bool dontWrite, 
            bool announceToPlayfield)
            : base(statList, number, defaultValue, sendBaseValue, dontWrite, announceToPlayfield)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// </summary>
        public override uint BaseValue
        {
            get
            {
                int[] breedBaseHitPoints = { 10, 15, 10, 25, 30, 30, 30 };
                uint breed = this.Stats[StatIds.breed].BaseValue;

                return (uint)breedBaseHitPoints[breed - 1];
            }
        }

        /// <summary>
        /// </summary>
        public override int Modifier
        {
            get
            {
                return base.Modifier;
            }

            set
            {
                base.Modifier = value;
                this.Stats[StatIds.life].Value = Math.Min(this.Value, this.Stats[StatIds.life].Value);
            }
        }

        /// <summary>
        /// </summary>
        public override int PercentageModifier
        {
            get
            {
                return base.PercentageModifier;
            }

            set
            {
                base.PercentageModifier = value;
                this.Stats[StatIds.life].Value = Math.Min(this.Value, this.Stats[StatIds.life].Value);
            }
        }

        /// <summary>
        /// </summary>
        public override int Trickle
        {
            get
            {
                return base.Trickle;
            }

            set
            {
                base.Trickle = value;
                this.Stats[StatIds.life].Value = Math.Min(this.Value, this.Stats[StatIds.life].Value);
            }
        }

        /// <summary>
        /// </summary>
        public override int Value
        {
            get
            {
                // Hitpoints by Profession and TitleLevel
                int[,] tableProfessionHitPoints =
                {
                    { 6, 6, 6, 6, 6, 6, 6, 6, 7, 6, 6, 6, 6, 6 }, 
                    { 7, 7, 6, 7, 7, 7, 6, 7, 8, 6, 6, 6, 7, 7 }, 
                    { 8, 7, 6, 7, 7, 8, 7, 7, 9, 6, 6, 6, 8, 7 }, 
                    { 9, 8, 6, 8, 8, 8, 7, 7, 10, 6, 6, 6, 9, 8 }, 
                    { 10, 9, 6, 9, 8, 9, 8, 8, 11, 6, 6, 6, 10, 9 }, 
                    { 11, 12, 6, 10, 9, 9, 9, 9, 12, 6, 6, 6, 11, 10 }, 
                    { 12, 13, 7, 11, 10, 10, 10, 10, 13, 7, 7, 7, 12, 11 }, 
                };

                int[] breedMultiplicatorHitPoints = { 3, 3, 2, 4, 8, 8, 10 };
                int[] breedModificatorHitPoints = { 0, -1, -1, 0, 0, 0, 0 };
                uint breed = this.Stats[StatIds.breed].BaseValue;
                uint profession = this.Stats[StatIds.profession].BaseValue;

                uint titleLevel = (uint)this.Stats[StatIds.titlelevel].Value;
                uint level = (uint)this.Stats[StatIds.level].Value;
                int beforeModifiers =
                    (int)
                        (this.BaseValue
                         + (level
                            * (tableProfessionHitPoints[titleLevel - 1, profession - 1]
                               + breedModificatorHitPoints[breed - 1]))
                         + (this.Stats[StatIds.bodydevelopment].Value * breedMultiplicatorHitPoints[breed - 1]));
                return (int)Math.Floor(
                    (double) // ReSharper disable PossibleLossOfFraction
                        ((beforeModifiers + this.Modifier + this.Trickle) * this.PercentageModifier / 100));

                // ReSharper restore PossibleLossOfFraction
            }

            set
            {
            }
        }

        #endregion
    }
}