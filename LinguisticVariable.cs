#region GNU Lesser General Public License
/*
This file is part of DotFuzzy.

DotFuzzy is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

DotFuzzy is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with DotFuzzy.  If not, see <http://www.gnu.org/licenses/>.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace DotFuzzy
{
    /// <summary>
    /// Represents a linguistic variable.
    /// </summary>
    public class LinguisticVariable
    {
        #region Private Properties

        private string name = String.Empty;
        private MembershipFunctionCollection membershipFunctionCollection = new MembershipFunctionCollection();
        private double inputValue = 0;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public LinguisticVariable()
        {
        }

        /// <param name="name">The name that identificates the linguistic variable.</param>
        public LinguisticVariable(string name)
        {
            this.Name = name;
        }

        /// <param name="name">The name that identificates the linguistic variable.</param>
        /// <param name="membershipFunctionCollection">A membership functions collection for the lingusitic variable.</param>
        public LinguisticVariable(string name, MembershipFunctionCollection membershipFunctionCollection)
        {
            this.Name = name;
            this.MembershipFunctionCollection = membershipFunctionCollection;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The name that identificates the linguistic variable.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// A membership functions collection for the lingusitic variable.
        /// </summary>
        public MembershipFunctionCollection MembershipFunctionCollection
        {
            get { return membershipFunctionCollection; }
            set { membershipFunctionCollection = value; }
        }

        /// <summary>
        /// The input value for the linguistic variable.
        /// </summary>
        public double InputValue
        {
            get { return inputValue; }
            set { inputValue = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Implements the fuzzification of the linguistic variable.
        /// </summary>
        /// <param name="membershipFunctionName">The membership function for which fuzzify the variable.</param>
        /// <returns>The degree of membership.</returns>
        public double Fuzzify(string membershipFunctionName)
        {
            MembershipFunction membershipFunction = this.membershipFunctionCollection.Find(membershipFunctionName);

            if ((membershipFunction.X0 <= this.InputValue) && (this.InputValue < membershipFunction.X1))
                return (this.InputValue - membershipFunction.X0) / (membershipFunction.X1 - membershipFunction.X0);
            else if ((membershipFunction.X1 <= this.InputValue) && (this.InputValue <= membershipFunction.X2))
                return 1;
            else if ((membershipFunction.X2 < this.InputValue) && (this.InputValue <= membershipFunction.X3))
                return (membershipFunction.X3 - this.InputValue) / (membershipFunction.X3 - membershipFunction.X2);
            else
                return 0;
        }

        /// <summary>
        /// Returns the minimum value of the linguistic variable.
        /// </summary>
        /// <returns>The minimum value of the linguistic variable.</returns>
        public double MinValue()
        {
            double minValue = this.membershipFunctionCollection[0].X0;

            for (int i = 1; i < this.membershipFunctionCollection.Count; i++)
            {
                if (this.membershipFunctionCollection[i].X0 < minValue)
                    minValue = this.membershipFunctionCollection[i].X0;
            }

            return minValue;
        }

        /// <summary>
        /// Returns the maximum value of the linguistic variable.
        /// </summary>
        /// <returns>The maximum value of the linguistic variable.</returns>
        public double MaxValue()
        {
            double maxValue = this.membershipFunctionCollection[0].X3;

            for (int i = 1; i < this.membershipFunctionCollection.Count; i++)
            {
                if (this.membershipFunctionCollection[i].X3 > maxValue)
                    maxValue = this.membershipFunctionCollection[i].X3;
            }

            return maxValue;
        }

        /// <summary>
        /// Returns the difference between MaxValue() and MinValue().
        /// </summary>
        /// <returns>The difference between MaxValue() and MinValue().</returns>
        public double Range()
        {
            return this.MaxValue() - this.MinValue();
        }

        #endregion
    }
}
