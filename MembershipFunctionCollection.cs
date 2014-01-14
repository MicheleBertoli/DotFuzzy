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
using System.Collections.ObjectModel;
using System.Text;

namespace DotFuzzy
{
    /// <summary>
    /// Represents a collection of membership functions.
    /// </summary>
    public class MembershipFunctionCollection : Collection<MembershipFunction>
    {
        #region Public Methods

        /// <summary>
        /// Finds a membership function in a collection.
        /// </summary>
        /// <param name="membershipFunctionName">Membership function name.</param>
        /// <returns>The membership function, if founded.</returns>
        public MembershipFunction Find(string membershipFunctionName)
        {
            MembershipFunction membershipFunction = null;

            foreach (MembershipFunction function in this)
            {
                if (function.Name == membershipFunctionName)
                {
                    membershipFunction = function;
                    break;
                }
            }

            if (membershipFunction == null)
                throw new Exception("MembershipFunction not found: " + membershipFunctionName);
            else
                return membershipFunction;
        }

        #endregion
    }
}
