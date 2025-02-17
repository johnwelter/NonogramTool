using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictoPuzzleMaker.Data
{
    public class PuzzleBank
    {
        public String name { get; set; }
        public Puzzle[] puzzles = new Puzzle[27];

        public PuzzleBank() 
        {
            for (int i = 0; i < puzzles.Length; i++)
            {
                puzzles[i] = new Puzzle(1);
                puzzles[i].name += i.ToString();
            }
        }
    }

    public class Puzzle
    {
        [JsonIgnore]
        public static int[] s_SideLengths = { 5, 10, 15 };
        [JsonIgnore]
        public static int[] s_TotalLengths = { 25, 100, 225 };
        [JsonProperty]
        public String name { get; set; }    //name of puzzle for display
        [JsonProperty]
        public int puzzleSize { get; set; } //size of puzzle

        //no need to store clues, since they will be created on export
        [JsonProperty]
        protected int[] puzzleSolution;
        [JsonProperty]
        protected int[] puzzleImage;

        [JsonProperty]
        private bool EasyPalette_Enable = true;
        [JsonProperty]
        private bool EasyPalette_Brighten = false;
        [JsonProperty]
        private bool EasyPalette_KeepWhite = false;
        [JsonProperty]
        public Palettes.eEasyPalette EasyPalette_Selection = Palettes.eEasyPalette.Gray;

        [JsonProperty]
        public int[] palette = { 0, 0, 0 };

        public Puzzle(int size)
        {
            name = "puzzle";
            puzzleSize = size;
            puzzleSolution = new int[s_TotalLengths[puzzleSize]];
            puzzleImage = new int[s_TotalLengths[puzzleSize]];
            UpdateEasyPalette();
            ClearPuzzle();
        }

        public void ClearPuzzle()
        {
            for(int i = 0; i < s_TotalLengths[puzzleSize]; i++)
            {
                puzzleSolution[i] = 0;
                puzzleImage[i] = 0;
            }
        }

        public void SetSolution(int x, int y, int val)
        {
            //convert index to 1D 
            // x goes across, y goes down
            SetSolution(makeOneDimensional(x, y, s_SideLengths[puzzleSize]), val);
        }
        public void SetSolution(int idx, int val)
        {
            puzzleSolution[idx] = val; 
        }

        public int GetSolution(int x, int y)
        {
            return GetSolution(makeOneDimensional(x, y, s_SideLengths[puzzleSize]));
        }

        public int GetSolution(int idx)
        {
           return puzzleSolution[idx];
        }

        public void SetImage(int x, int y, int val)
        {
            SetImage(makeOneDimensional(x, y, s_SideLengths[puzzleSize]), val);
        }
        public void SetImage(int idx, int val)
        {
            puzzleImage[idx] = val;
        }

        public int GetImage(int x, int y)
        {
            return GetImage(makeOneDimensional(x, y, s_SideLengths[puzzleSize]));
        }

        public int GetImage(int idx)
        {
            return puzzleImage[idx];
        }

        private int makeOneDimensional(int x, int y, int rowLength)
        {
            return (y * rowLength) + x;
        }

        public void SetSize(int newSize)
        {
            puzzleSize = newSize;
            Array.Resize(ref puzzleSolution, s_TotalLengths[puzzleSize]);
            Array.Resize(ref puzzleImage, s_TotalLengths[puzzleSize]);
            ClearPuzzle();
            
        }

        public void SetUseEasyPalette(bool useEasyPalette)
        {
            EasyPalette_Enable = useEasyPalette;
            UpdateEasyPalette();
        }

        public void SetUseBrighterColors(bool useBrighter)
        {
            EasyPalette_Brighten = useBrighter;
            UpdateEasyPalette();
        }

        public void SetKeepWhite(bool keepWhite)
        {
            EasyPalette_KeepWhite = keepWhite;
            UpdateEasyPalette();
        }

        public void SetEasyPaletteSelection(Palettes.eEasyPalette palette)
        {
            EasyPalette_Selection = palette;
            UpdateEasyPalette();
        }

        public void UpdateEasyPalette()
        {
            if (EasyPalette_Enable)
            {
                int brightenOffset = EasyPalette_Brighten ? 16 : 0;
                int baseIndex = ((int)EasyPalette_Selection) + brightenOffset;
                SetPaletteEntry(0, baseIndex);
                SetPaletteEntry(1, baseIndex + 16);
                int finalIndex = EasyPalette_KeepWhite ? 32 : baseIndex + 32;
                SetPaletteEntry(2, finalIndex);
            }
        }

        public void GetEasyPaletteData(out bool useEzp, out bool useBright, out bool keepWhite, out int selection )
        {
            useEzp = EasyPalette_Enable;
            useBright = EasyPalette_Brighten;
            keepWhite = EasyPalette_KeepWhite;
            selection = (int)EasyPalette_Selection;
            
        }
        public bool IsEasyPalEnabled()
        {
            return EasyPalette_Enable;
        }
        public void SetPaletteEntry(int idx, int val)
        {
            palette[idx] = val;
        }

        public int[] GetPalette()
        { 
            return palette; 
        }
        public int GetPaletteAt(int idx)
        {
            return palette[idx];
        }
    }
}
