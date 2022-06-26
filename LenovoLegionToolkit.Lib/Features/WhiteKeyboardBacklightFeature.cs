﻿using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.Features
{
    public class WhiteKeyboardBacklightFeature : AbstractDriverFeature<WhiteKeyboardBacklightState>
    {
        public WhiteKeyboardBacklightFeature() : base(Drivers.GetEnergy, 0x83102144) { }

        public async override Task<WhiteKeyboardBacklightState> GetStateAsync()
        {
            await IsSupportedAsync().ConfigureAwait(false);
            return await base.GetStateAsync().ConfigureAwait(false);
        }

        public async override Task SetStateAsync(WhiteKeyboardBacklightState state)
        {
            await IsSupportedAsync().ConfigureAwait(false);
            await base.SetStateAsync(state).ConfigureAwait(false);
        }

        protected override uint GetInternalStatus() => 0x22;

        protected override uint[] ToInternal(WhiteKeyboardBacklightState state)
        {
            return state switch
            {
                WhiteKeyboardBacklightState.Off => new uint[] { 0x00023 },
                WhiteKeyboardBacklightState.Low => new uint[] { 0x10023 },
                WhiteKeyboardBacklightState.High => new uint[] { 0x20023 },
                _ => throw new Exception("Invalid state"),
            };
        }

        protected override WhiteKeyboardBacklightState FromInternal(uint state)
        {
            return state switch
            {
                0x1 => WhiteKeyboardBacklightState.Off,
                0x3 => WhiteKeyboardBacklightState.Low,
                0x5 => WhiteKeyboardBacklightState.High,
                _ => throw new Exception("Invalid state"),
            };
        }

        private async Task IsSupportedAsync()
        {
            var (_, outBuffer) = await SendCodeAsync(DriverHandle(), ControlCode, 0x1).ConfigureAwait(false);
            outBuffer >>= 1;
            if (outBuffer != 0x2)
                throw new InvalidOperationException("Not supported.");
        }
    }
}