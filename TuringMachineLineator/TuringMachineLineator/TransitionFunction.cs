using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringMachineLineator
{
    class TransitionFunction
    {
        public string StartState { get; private set; }
        public string EndState { get; private set; }

        public List<string> Parameters { get; private set; }
        public List<string> Output { get; private set; }
        public List<string> Actions { get; private set; }

        public bool IsGood = true;
        /// <summary>
        /// Expects a domain line of the form
        /// state, p1, p2
        /// 
        /// Expects a range line of the form
        /// state, o1, o2, Act1, Act2
        /// Valid 'Actx' characters are '<', '>', '-'
        /// </summary>
        /// <param name="DomainLine"></param>
        /// <param name="RangeLine"></param>
        public TransitionFunction(string DomainLine, string RangeLine)
        {
            List<string> parseDL = DomainLine.Split(',').ToList();
            List<string> parseRL = RangeLine.Split(',').ToList();

            if (parseDL.Count < 2 || parseRL.Count < 3)
            {
                IsGood = false;
            }

            if (IsGood)
            {
                StartState = parseDL[0];
                parseDL.RemoveAt(0);
                Parameters = parseDL;

                EndState = parseRL[0];
                parseRL.RemoveAt(0);

                uint iOutputEnd = Math.Min((uint)parseRL.IndexOf("<"), (uint)parseRL.IndexOf(">"));
                iOutputEnd = Math.Min(iOutputEnd, (uint)parseRL.IndexOf("-"));
                iOutputEnd = iOutputEnd & 0xEFFF;
                int iOEnd = (int)iOutputEnd;
                Output = parseRL.GetRange(0, iOEnd - 1);
                Actions = parseRL.GetRange(iOEnd, parseRL.Count - iOEnd);
            }
        }
    }
}
