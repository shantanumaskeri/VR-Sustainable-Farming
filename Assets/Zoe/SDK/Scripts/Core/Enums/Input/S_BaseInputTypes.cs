public enum S_BaseInputTypes
{
    START_BUTTON = 0,

    PRIMARY_BUTTON,
    SECONDARY_BUTTON,

    // We set this explicitely to 5 because when removing
    // A_BUTTON, B_BUTTON, X_BUTTON and Y_BUTTON, the indices
    // of this enum changed. Unfortunately, these are serialized
    // in the Camera (IO) prefab for the default grab input
    // making it switch to GRIP if we don't assign it its previous
    // 5 value...
    INDEX = 5,

    THUMBREST,

    GRIP,

    STICK
}