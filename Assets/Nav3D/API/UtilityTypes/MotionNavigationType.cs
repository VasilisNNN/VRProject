namespace Nav3D.API
{
    /// <summary>
    /// Nav3DAgent motion navigation modes.
    /// </summary>
    public enum MotionNavigationType
    {
        //Use only pathfinding
        GLOBAL,
        //Use both pathfinding and local avoidance
        GLOBAL_AND_LOCAL,
        //Use only local avoidance
        LOCAL
    }
}
