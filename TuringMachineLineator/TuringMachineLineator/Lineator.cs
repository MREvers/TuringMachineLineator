using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TuringMachineLineator
{
    class Lineator
    {
        private List<string> m_lstFile = null;

        public Lineator() { }

        /// <summary>
        /// Illegal Line Characters ':'
        /// </summary>
        /// <param name="FileName"></param>
        public void Import(string FileName)
        {
            if (!File.Exists(FileName)) { return; }

            StreamReader file = new StreamReader(FileName);

            m_lstFile = new List<string>();

            string line;
            while ((line = file.ReadLine()) != null)
            {
                // Ignore Comments
                if (line.Length > 2 && line.Substring(0, 2) != "\\\\" && line.Substring(0, 2) != "//")
                {
                    m_lstFile.Add(line);
                }
            }
        }

        /// <summary>
        /// Takes a K-Tape Machine and transforms it into a single tape machine
        /// where all prior tapes are contained on a single line with '#' acting
        /// as boundaries between the tapes and $ indicating the END of all tapes.
        /// e.g.
        /// Tape 1: 0110
        /// Tape 2: 1000
        /// Flattened: #0110#1000#$
        /// 
        /// Definitions:
        /// Undetermined State: A state on the Flattened machine that does not uniquely identify a state on
        ///   the unflatted state.
        /// Determined State: A state on the flattened machine that uniquely identifies a state in the
        ///   unflatted states.
        /// Virtual Head: On an unflattened machine, each tape has a "Head". On flat machine, each 'tape' has
        ///   a Virtual Head that is indicated by a different special symbol for each symbol in the tape library.
        /// 
        /// Uses the following symbols.
        /// '#' Indicates Tape Boundaries.
        /// 'R' Is a place holder.
        /// '~' Is null.
        /// '$' Indicates END of tape.
        /// Use of these symbols in the input is not allowed.
        /// </summary>
        /// <param name="OutputFileName"></param>
        public void Lineate(string OutputFileName)
        {
            TuringMachine UnFlattenedTM = TuringMachine.BuildTuringMachine(m_lstFile);

            // Verify the tape library is safe.
            if (UnFlattenedTM.TapeLibrary.Contains("#") ||
                UnFlattenedTM.TapeLibrary.Contains("R") ||
                UnFlattenedTM.TapeLibrary.Contains("~") ||
                UnFlattenedTM.TapeLibrary.Contains("$"))
            { return; }

            // Determine the number of unflattened tapes.
            int iKTapes = UnFlattenedTM.TransitionSet.First().Parameters.Count;

            // Assume all input is now on a flat tape as described in description.
            List<TransitionFunction> lstFlatTransitionSet = new List<TransitionFunction>();

            // We need to find out what each tapes 'head' is pointing to.
            // Move the Real Head right until it sees each Virtual Head. Once this is done,
            // we are in a determined state, and can move forward.
            // So create the Real Transition Functions that allow the Real Head to move
            // right until it has done so.
            // The way this works is by creating a STATE for each virtual head that it has seen.
            // e.g. #0.100#11^.10# where . indicates virtual head and ^ is real head.
            // Then the state would be q11, since it has seen two virtual heads, each of which
            // is on 1.
            List<string> lstMoveRightTapeLibraryBase = new List<string>() { "#" };
            Dictionary<int, List<string>> mapStatesBySeenTapes = new Dictionary<int, List<string>>();
            mapStatesBySeenTapes.Add(0, new List<string>() { UnFlattenedTM.StartState });
            for (int i = 0; i < iKTapes; i++)
            {
                mapStatesBySeenTapes[i + 1] = new List<string>();

                List<string> lstTrunkStates = mapStatesBySeenTapes[i];
                foreach (string szState in lstTrunkStates)
                {
                    // Get all the TFs that start with this state to determine what TFs we need to move right.
                    List<string> lstTFsWithThisState = UnFlattenedTM.TransitionSet.Where(x => flatStateMatch(szState, x)).Select(x => x.Parameters[i]).ToList();
                    List<string> lstMoveRightTapeLibrary = lstMoveRightTapeLibraryBase.Concat(lstTFsWithThisState).ToList();
                    foreach (string szMoveRightChar in lstMoveRightTapeLibrary)
                    {
                        string szRangeState = constructStateName(szState, szMoveRightChar);
                        string szDomainLine = szState + "," + szMoveRightChar;
                        string szRangeLine = szRangeState + "," + szMoveRightChar + "," + ">";
                        TransitionFunction newTF = new TransitionFunction(
                            szDomainLine,
                            szRangeLine);
                        lstFlatTransitionSet.Add(newTF);

                        mapStatesBySeenTapes[i + 1].Add(szRangeState);
                    }
                }

            }

        }

        private string constructStateName(string szBase, string szNextChar)
        {
            return szBase + "-" + szNextChar;
        }

        private List<string> extractStateParms(string szFlatState)
        {
            return szFlatState.Split('-').ToList();
        }

        private bool flatStateMatch(string szFlat, TransitionFunction TFTest)
        {
            List<string> lstParameters = extractStateParms(szFlat);
            string szName = lstParameters[0];
            lstParameters.RemoveAt(0);

            bool bRetval = szName == TFTest.StartState;
            for (int i = 0; i < lstParameters.Count && bRetval; i++)
            {
                bRetval &= (TFTest.Parameters[i] == lstParameters[i]);
            }

            return bRetval;
        }
    }
}
