namespace CORA
{
    /// <summary>
    /// Horizontal: Moves only horizontally
    /// Vertical: Moves only vertically
    /// Oblique: Moves both horizontally and vertically
    /// </summary>
    public enum MovingPlatformDirectionType
    {
        Horizontal,
        Vertical,
        Oblique
    }
    /// <summary>
    /// Bouncing: Moving platform will move from A to B, then from B to A.
    /// Cycle: Moving platform will move from A to B, then teleport to A and repeat.
    /// </summary>
    public enum MovingPlatformRotationType
    {
        Bouncing,
        Cycle
    }
    /// <summary>
    /// uninitialized: No particle type
    /// </summary>
    public enum ParticleType
    {
        uninitialized,
        firework,
        water
    }
    /// <summary>
    /// player is any entity directly controlled by the player. Minibots refer to any instance of that minibot, player controlled or not.
    /// </summary>
    public enum InteractorType
    {
        player,
        toolbot,
        rocketbot,
        elevatorbot,
        swarmbot,
        cutterbot,
        batterybot,
        bucketbot
    }
    /// <summary>
    /// Abstract enumeration for any object which can be activated and/or deactivated.
    /// </summary>
    public enum ActivationState
    {
        inactive,
        activating,
        active,
        deactivating
    }
    /// <summary>
    /// This enumerates all of the physics object types for use with the level editor and possibly other places.
    /// </summary>
    public enum DrawableType
    {
        wall,
        rust,
        movingPlatform,
        slope,
        hangingLedge,
        movingHangingLedge,
        pressurePlate,
        controlPanel,
        elevatorSurface,
        doodad,
        animatedDoodad
    }

    public enum CSLCommandType
    {
        spawn,
        spawnui,
        create,
        delete,
        despawn,
        move,
        walk,
        waittime,
        waitforinstruction,
        displaytext,
        takePlayerControl,
        givePlayerControl,
        drawableEnabled,
        drawableVisible,
        moveCamera,
        slideCamera,
        fade,
        go,
        end,
        loadNextLevel,
        drawNextLevel,
        switchLevels,
        unloadPreviousLevel,
        changebotai,
        assignbot
    }

    public enum CSLObjectType
    {
        batterybot,
        bucketbot,
        cutterbot,
        elevatorbot,
        player,
        rocketbot,
        swarmbot,
        toolbot,
        door,
        movingplatform,
        rust,
        slope,
        wall,
        controlpanel,
        elevatorsurface,
        hangingledge,
        movinghangingledge,
        pressureplate,
        doodad,
        animateddoodad
    }
}