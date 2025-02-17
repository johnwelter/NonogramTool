using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace PictoPuzzleMaker.Data
{
    static class Consts
    {
        public const int PalTiles_Loc = 0x24;
        public const int PalTile_Offset_x = 12;
        public const int PalTile_Offset_y = 12;
        public static readonly UInt32[] solutionColors = { 0xFF000000, 0xFFFFFFFF };
    }

    public class Palettes : ObservableCollection<string>
    {
        public enum eEasyPalette
        {
            Gray, 
            Blue,
            Lavender,
            Violet,
            Magenta,
            Pink,
            Red,
            Orange,
            Gold,
            Green,
            Gameboy,
            Teal,
            Skyblue
        }
        public Palettes()
        {
            Add("Gray");
            Add("Blue");
            Add("Lavender");
            Add("Violet");
            Add("Magenta");
            Add("Pink");
            Add("Red");
            Add("Orange");
            Add("Gold");
            Add("Green");
            Add("Gameboy");
            Add("Teal");
            Add("Skyblue");
        }
    }

    public class FullPalette
    {
        private static int[] paletteColors;
        public const int desiredBlack = 0x0F;
        public static void LoadFullpalette()
        {
            if (paletteColors == null)
                paletteColors = new int[64];

            String fileName = "PictoPuzzleMaker.Data.2C02G_wiki.pal";
            Assembly assembly = Assembly.GetExecutingAssembly();
            String[] test = assembly.GetManifestResourceNames();
            Stream paletteBytes = assembly.GetManifestResourceStream(fileName);
            if(paletteBytes == null)
            {
                throw new FileNotFoundException("can't find the palette!", fileName);
            }

            //all the palette bytes are read in- just in case, check the size to be a mult of 3
            if(paletteBytes.Length %3 != 0 && !(paletteBytes.Length/3 >= paletteColors.Length))
            {
                throw new InvalidDataException("bad data size, needs to be greater than 56");
            }

            //read in the pallete, 3 bytes at a time
            for (int i = 0; i < paletteColors.Length; i++)
            {
                byte[] argbByte = { 0xFF, 0x00, 0x00, 0x00};
                int colorIndex = i * 3;
                argbByte[1] = (byte)paletteBytes.ReadByte();
                argbByte[2] = (byte)paletteBytes.ReadByte();
                argbByte[3] = (byte)paletteBytes.ReadByte();
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(argbByte);

                paletteColors[i] = BitConverter.ToInt32(argbByte, 0);
                //Color dummyColor = Color.FromArgb(paletteColors[i]);
                //Console.WriteLine(paletteColors[i].ToString("X4") + dummyColor.ToString());
            }
        }

        public static int GetColor(int idx)
        {
            if (paletteColors == null)
                return 0;

            return paletteColors[idx];
        }
    }
}
