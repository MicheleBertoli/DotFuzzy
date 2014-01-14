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
    /// Represents a membership function.
    /// </summary>
    public class MembershipFunction
    {
        #region Private Properties

        private string name = String.Empty;
        private double x0 = 0;
        private double x1 = 0;
        private double x2 = 0;
        private double x3 = 0;
        private double value = 0;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MembershipFunction()
        {
        }

        /// <param name="name">The name that identificates the membership function.</param>
        public MembershipFunction(string name)
        {
            this.Name = name;
        }

        /// <param name="name">The name that identificates the linguistic variable.</param>
        /// <param name="x0">The value of the (x0, 0) point.</param>
        /// <param name="x1">The value of the (x1, 1) point.</param>
        /// <param name="x2">The value of the (x2, 1) point.</param>
        /// <param name="x3">The value of the (x3, 0) point.</param>
        public MembershipFunction(string name, double x0, double x1, double x2, double x3)
        {
            this.Name = name;
            this.X0 = x0;
            this.X1 = x1;
            this.X2 = x2;
            this.X3 = x3;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The name that identificates the membership function.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// The value of the (x0, 0) point.
        /// </summary>
        public double X0
        {
            get { return x0; }
            set { x0 = value; }
        }

        /// <summary>
        /// The value of the (x1, 1) point.
        /// </summary>
        public double X1
        {
            get { return x1; }
            set { x1 = value; }
        }

        /// <summary>
        /// The value of the (x2, 1) point.
        /// </summary>
        public double X2
        {
            get { return x2; }
            set { x2 = value; }
        }

        /// <summary>
        /// The value of the (x3, 0) point.
        /// </summary>
        public double X3
        {
            get { return x3; }
            set { x3 = value; }
        }

        /// <summary>
        /// The value of membership function after evaluation process.
        /// </summary>
        public double Value
        {
            get { return value; }
            set { this.value = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Calculate the centroid of a trapezoidal membership function.
        /// </summary>
        /// <returns>The value of centroid.</returns>
        public double Centorid()
        {
            double a = this.x2 - this.x1;
            double b = this.x3 - this.x0;
            double c = this.x1 - this.x0;

            return ((2 * a * c) + (a * a) + (c * b) + (a * b) + (b * b)) / (3 * (a + b)) + this.x0; 
        }

        /// <summary>
        /// Calculate the area of a trapezoidal membership function.
        /// </summary>
        /// <returns>The value of area.</returns>
        public double Area()
        {
            double a = this.Centorid() - this.x0;
            double b = this.x3 - this.x0;

            return (this.value * (b + (b - (a * this.value)))) / 2;
        }

        #endregion
    }
}
