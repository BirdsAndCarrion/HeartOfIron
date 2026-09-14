using UnityEngine;
using System;
using UnityEngine.Events;

public interface IVisionEvents
{
    event EventHandler<VisionEventArgs> OnMechSpotted;
    event EventHandler<VisionEventArgs> OnMechLost;
}

public class VisionEventArgs : EventArgs
{
    public MechController SpottingMech { get; private set; }
    public MechController SpottedMech { get; private set; }

    public VisionEventArgs(MechController spottingMech, MechController spottedMech)
    {
        SpottingMech = spottingMech;
        SpottedMech = spottedMech;
    }
}