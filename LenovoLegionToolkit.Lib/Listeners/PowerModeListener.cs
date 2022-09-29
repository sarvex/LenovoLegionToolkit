﻿using System;
using System.Management;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.Listeners
{
    public class PowerModeListener : AbstractWMIListener<PowerModeState>, INotifyingListener<PowerModeState>
    {
        private readonly AIModeController _aiModeController;

        public PowerModeListener(AIModeController aiModeController) : base("ROOT\\WMI", "LENOVO_GAMEZONE_SMART_FAN_MODE_EVENT")
        {
            _aiModeController = aiModeController ?? throw new ArgumentNullException(nameof(aiModeController)); ;
        }

        protected override PowerModeState GetValue(PropertyDataCollection properties)
        {
            var property = properties["mode"];
            var propertyValue = Convert.ToInt32(property.Value);
            var value = (PowerModeState)(object)(propertyValue - 1);
            return value;
        }

        protected override async Task OnChangedAsync(PowerModeState value)
        {
            await _aiModeController.StartStopAsync(value).ConfigureAwait(false);
            await Power.ActivatePowerPlanAsync(value).ConfigureAwait(false);
        }

        public async Task NotifyAsync(PowerModeState value)
        {
            await OnChangedAsync(value).ConfigureAwait(false);
            RaiseChanged(value);
        }
    }
}
