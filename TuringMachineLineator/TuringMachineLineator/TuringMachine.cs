using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringMachineLineator
{
    class TuringMachine
    {
        /// <summary>
        /// Static Constructor.
        /// 
        /// Requires lines
        /// "Name: ..."
        /// "StartState: ..."
        /// "AcceptStates: x, y, z..."
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static TuringMachine BuildTuringMachine(List<string> Input)
        {
            bool foundNeededInfo = true;

            Dictionary<string, string> mapKeyVals = new Dictionary<string, string>();
            string szName = "";
            string szStart = "";
            List<TransitionFunction> lstTransitionSet = new List<TransitionFunction>();
            List<string> lstAcceptStates = new List<string>();

            // Get the name, transition functions, and accept states.
            string szPrevLine = ""; // TF's come in line pairs.
            foreach (string line in Input)
            {
                if (line.IndexOf(":") != -1) // This line contains machine info.
                {
                    List<string> lstKeyVal = line.Split(':').ToList();
                    mapKeyVals.Add(lstKeyVal[0], lstKeyVal[1]);
                }
                else if (!string.IsNullOrWhiteSpace(szPrevLine)) // Its a transition function
                {
                    TransitionFunction newTF = new TransitionFunction(szPrevLine, line);
                    if (newTF.IsGood)
                    {
                        lstTransitionSet.Add(newTF);
                    }
                    szPrevLine = "";
                }
                else
                {
                    szPrevLine = line;
                }
            }

            if (foundNeededInfo &= mapKeyVals.ContainsKey("Name"))
            {
                szName = mapKeyVals["Name"].Trim();
            }

            if (foundNeededInfo &= mapKeyVals.ContainsKey("StartState"))
            {
                szStart = mapKeyVals["StartState"].Trim();
            }

            if (foundNeededInfo &= mapKeyVals.ContainsKey("AcceptStates"))
            {
                lstAcceptStates = mapKeyVals["AcceptStates"].Split(',').Select(x=>x.Trim()).ToList();
            }

            return foundNeededInfo ? new TuringMachine(szName, szStart, lstTransitionSet, lstAcceptStates) : null;
        }

        public string MachineName { get; private set; }

        public List<string> TapeLibrary;
        public string StartState;
        public List<TransitionFunction> TransitionSet;
        public List<string> AcceptStates;

        // I can't remember if this is needed.
        public List<string> RejectStates;

        /// <summary>
        /// The tape library is infered from the transition set.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="TransitionSet"></param>
        /// <param name="AcceptStates"></param>
        public TuringMachine(string Name, string StartState, List<TransitionFunction> TransitionSet, List<string> AcceptStates)
        {
            MachineName = Name;
            this.StartState = StartState;
            this.TransitionSet = TransitionSet;
            this.AcceptStates = AcceptStates;

            extractTapeLibrary();
        }

        private void extractTapeLibrary()
        {
            List<string> lstTapeLibrary = new List<string>();
            foreach(TransitionFunction TF in TransitionSet)
            {
                foreach(string szParmKey in TF.Parameters)
                {
                    if (!lstTapeLibrary.Contains(szParmKey))
                    {
                        lstTapeLibrary.Add(szParmKey);
                    }
                }
            }

            TapeLibrary = lstTapeLibrary;
        }

    }
}
