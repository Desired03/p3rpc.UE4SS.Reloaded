using System.Runtime.InteropServices;
using p3rpc.UE4SS.Reloaded.Configuration;
using p3rpc.UE4SS.Reloaded.Template;
using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;

namespace p3rpc.UE4SS.Reloaded;
/// <summary>
/// Your mod logic goes here.
/// </summary>
public partial class Mod : ModBase // <= Do not Remove.
{
    /// <summary>
    /// Provides access to the mod loader API.
    /// </summary>
    private readonly IModLoader _modLoader;

    /// <summary>
    /// Provides access to the Reloaded.Hooks API.
    /// </summary>
    /// <remarks>This is null if you remove dependency on Reloaded.SharedLib.Hooks in your mod.</remarks>
    private readonly IReloadedHooks? _hooks;

    /// <summary>
    /// Provides access to the Reloaded logger.
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Entry point into the mod, instance that created this class.
    /// </summary>
    private readonly IMod _owner;

    /// <summary>
    /// Provides access to this mod's configuration.
    /// </summary>
    private Config _configuration;

    /// <summary>
    /// The configuration of the currently executing mod.
    /// </summary>
    private readonly IModConfig _modConfig;

    [LibraryImport("UE4SS.dll", EntryPoint = "setup_r2mod", StringMarshalling = StringMarshalling.Utf8)]
    private static partial void setup_r2mod([MarshalAs(UnmanagedType.LPWStr)] string mod_path_string);

    public Mod(ModContext context)
    {
        _modLoader = context.ModLoader;
        _hooks = context.Hooks;
        _logger = context.Logger;
        _owner = context.Owner;
        _configuration = context.Configuration;
        _modConfig = context.ModConfig;

        var dll = NativeLibrary.Load(_modLoader.GetDirectoryForModId(context.ModConfig.ModId) + @"\UE4SS.dll");

        _modLoader.ModLoading += (v1, configV1) =>
        {
            if (!Directory.Exists(_modLoader.GetDirectoryForModId(configV1.ModId) + @"\UE4SS")) return;
            setup_r2mod(_modLoader.GetDirectoryForModId(configV1.ModId) + @"\UE4SS");
        };

        _modLoader.ModUnloading += (v1, configV1) =>
        {
            NativeLibrary.Free(dll);
        };
    }

    #region Standard Overrides
    public override void ConfigurationUpdated(Config configuration)
    {
        // Apply settings from configuration.
        // ... your code here.
        _configuration = configuration;
        _logger.WriteLine($"[{_modConfig.ModId}] Config Updated: Applying");
    }
    #endregion

    #region For Exports, Serialization etc.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Mod() { }
#pragma warning restore CS8618
    #endregion
}