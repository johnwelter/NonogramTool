using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictoPuzzleMaker.Data
{
    public class BankObserver
    {
        public ObservableCollection<Puzzle> observablePuzzles;

        public BankObserver()
        {
            observablePuzzles = new ObservableCollection<Puzzle>();
        }
        public void LoadBank(PuzzleBank bank)
        {
            if (observablePuzzles == null)
                return;

            observablePuzzles.Clear();

            for(int i = 0; i < bank.puzzles.Length; i++)
            {
                observablePuzzles.Add(bank.puzzles[i]);
            }
        }

    }

}
