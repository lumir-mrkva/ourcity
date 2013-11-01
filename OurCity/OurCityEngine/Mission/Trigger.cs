using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace OurCityEngine.Mission
{
    /// <summary>
    /// Delegate what will be called if all Checks are true
    /// </summary>
    public delegate void DoIt();

    /// <summary>
    /// This delegate chesk conditions
    /// </summary>
    /// <returns>Result of check</returns>
    public delegate bool Check();

    /// <summary>
    /// Simple trigger concept with multiple checks
    /// </summary>
    public class Trigger
    {
        private DoIt doIt;
        private ArrayList checkArray = new ArrayList();

        public Trigger(DoIt doIt) {
            this.doIt = doIt;
        }

        public Trigger(DoIt doIt, Check check) : this(doIt) {
            checkArray.Add(check);
        }

        /// <summary>
        /// Check all conditions, if true - Do It
        /// </summary>
        public void Check()
        {
            foreach (Check check in checkArray)
            {
                if (!check()) return;
            }

            if (doIt != null) doIt();
        }

        /// <summary>
        /// Gets / Sets Do It function
        /// </summary>
        public DoIt DoIt { get; set; }

        /// <summary>
        /// Add check condition
        /// </summary>
        /// <param name="check">Check function</param>
        public void Add(Check check)
        {
            checkArray.Add(check);
        }

        /// <summary>
        /// Remove check condition
        /// </summary>
        /// <param name="check">Check function</param>
        public void Remove(Check check) {
            checkArray.Remove(check);
        }



    }
}
