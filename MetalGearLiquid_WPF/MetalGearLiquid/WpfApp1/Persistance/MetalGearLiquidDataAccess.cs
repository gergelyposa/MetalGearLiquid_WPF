using System;
using System.Threading.Tasks;

namespace MetalGearLiquid.Persistence
{
    /// <summary>
    /// MetalGearLiquid fájl kezelő felülete.
    /// </summary>
    public interface MetalGearLiquidDataAccess
    {
        Task<MetalGearLiquidTable> LoadAsync(String path);

    }
}
