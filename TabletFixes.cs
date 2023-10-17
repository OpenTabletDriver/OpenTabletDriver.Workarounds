using System.Numerics;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Tablet;

namespace OpenTabletDriver.Compatibility;

[PluginName("Tablet Fixes")]
public class TabletFixes : IPositionedPipelineElement<IDeviceReport>
{
    private Vector2 _lastPos;
    private float _lastPressure;

    public PipelinePosition Position => PipelinePosition.PreTransform;
    public event Action<IDeviceReport>? Emit;

    [BooleanProperty("Fix tail end", "Fixes the erroneous tail end produced by some tablets when lifting pen.")]
    public bool FixTailEnd { get; set; }

    [BooleanProperty("Reverse vertical tilt", "Reverses the direction of the vertical tilt.")]
    public bool ReverseVerticalTilt { get; set; }

    public void Consume(IDeviceReport value)
    {
        if (FixTailEnd)
            ApplyTailEndFix(value);

        if (ReverseVerticalTilt)
            ApplyVerticalTiltReverse(value);

        Emit?.Invoke(value);
    }

    private void ApplyTailEndFix(IDeviceReport value)
    {
        if (value is ITabletReport tabletReport)
        {
            if (_lastPressure > 0 && tabletReport.Pressure == 0)
            {
                var currentPos = tabletReport.Position;
                tabletReport.Position = _lastPos;

                Emit?.Invoke(tabletReport);

                tabletReport.Position = currentPos;
            }

            _lastPos = tabletReport.Position;
            _lastPressure = tabletReport.Pressure;
        }
    }

    private static void ApplyVerticalTiltReverse(IDeviceReport value)
    {
        if (value is ITiltReport tiltReport)
        {
            tiltReport.Tilt = new Vector2(tiltReport.Tilt.X, -tiltReport.Tilt.Y);
        }
    }
}
