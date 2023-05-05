using UnityEngine;

/// <summary>
/// Holds the information to help Automatically update the <see cref="UnityEditor.PlayerSettings.bundleVersion"/>
/// </summary>
/// <seealso cref="AutoVersioning"/>
public class VersionSettings : ScriptableObject
{
    /// <summary>
    /// The different major versions the game can be in
    /// </summary>
    public enum MajorVersion
    {
        PreRelease = 0,
        Alpha = 1,
        Beta = 2,
        Release = 3
    }

    /// <summary>
    /// The first number in the version. X.n.n
    /// </summary>
    public MajorVersion majorVersion;
    /// <summary>
    /// The second number in the version. n.X.n
    /// </summary>
    public int minorVersion;

    private void OnValidate()
    {
        //Update the settings every time we update our settings
        AutoVersioning.UpdateVersion();
    }
}