using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MetalGearLiquid.Persistence
{
    public class MetalGearLiquidFileDataAccess : MetalGearLiquidDataAccess
    {

        /// <summary>
        /// Fájl betöltése.
        /// </summary>
        /// <param name="path">Elérési útvonal.</param>
        /// <returns>A fájlból beolvasott játéktábla.</returns>
        /// 
        public async Task<MetalGearLiquidTable> LoadAsync(String path)
        {
            try
            {
                using StreamReader reader = new StreamReader(path);
                String line = await reader.ReadLineAsync();
                String[] numbers = line.Split(' ');
                Int32 tableSizeX = Int32.Parse(numbers[0]);
                Int32 tableSizeY = Int32.Parse(numbers[1]);
                Pair tableSize = new Pair(tableSizeX, tableSizeY);
                MetalGearLiquidTable table = new MetalGearLiquidTable(tableSize);
                Int32 time = Int32.Parse(numbers[2]);
                table._elapsedTime = time;
                table.maxGuards = Int32.Parse(numbers[3]);
                int numberOfGuards = 0;
                table.Guards = new Guard[table.maxGuards];
                table.spotPoints = new Pair[table.maxGuards * 24];

                for (Int32 i = 0; i < table.TableSize.x; i++)
                {
                    line = await reader.ReadLineAsync();
                    numbers = line.Split(' ');

                    for (Int32 j = 0; j < table.TableSize.y; j++)
                    {
                        table.SetValue(i, j, table.GetFieldTypeFromNumeric(Int32.Parse(numbers[j])));
                        if(table[i,j] == FieldType.Player)
                        {
                            table.Snek = new Pair(i, j);
                        }
                        if(table[i,j] == FieldType.Guard)
                        {
                            table.Guards[numberOfGuards] = new Guard(new Pair(i, j), 'w');
                            numberOfGuards++;
                        }
                    }
                }

                return table;
            }
            catch
            {
                throw new MetalGearLiquidDataException();
            }
        }
    }
}
