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
using System.Xml;

namespace DotFuzzy
{
    /// <summary>
    /// Represents the inferential engine.
    /// </summary>
    public class FuzzyEngine
    {
        #region Private Properties

        private LinguisticVariableCollection linguisticVariableCollection = new LinguisticVariableCollection();
        private string consequent = String.Empty;
        private FuzzyRuleCollection fuzzyRuleCollection = new FuzzyRuleCollection();
        private string filePath = String.Empty;

        #endregion

        #region Private Methods

        private LinguisticVariable GetConsequent()
        {
            return this.linguisticVariableCollection.Find(this.consequent);
        }

        private double Parse(string text)
        {
            int counter = 0;
            int firstMatch = 0;

            if (!text.StartsWith("("))
            {
                string[] tokens = text.Split();
                return this.linguisticVariableCollection.Find(tokens[0]).Fuzzify(tokens[2]);
            }

            for (int i = 0; i < text.Length; i++)
            {
                switch (text[i])
                {
                    case '(':
                        counter++;
                        if (counter == 1)
                            firstMatch = i;
                        break;

                    case ')':
                        counter--;
                        if ((counter == 0) && (i > 0))
                        {
                            string substring = text.Substring(firstMatch + 1, i - firstMatch - 1);
                            string substringBrackets = text.Substring(firstMatch, i - firstMatch + 1);
                            int length = substringBrackets.Length;
                            text = text.Replace(substringBrackets, Parse(substring).ToString());
                            i = i - (length - 1);
                        }
                        break;

                    default:
                        break;
                }
            }

            return Evaluate(text);
        }

        private double Evaluate(string text)
        {
            string[] tokens = text.Split();
            string connective = "";
            double value = 0;

            for (int i = 0; i <= ((tokens.Length / 2) + 1); i = i + 2)
            {
                double tokenValue = Convert.ToDouble(tokens[i]);

                switch (connective)
                {
                    case "AND":
                        if (tokenValue < value)
                            value = tokenValue;
                        break;

                    case "OR":
                        if (tokenValue > value)
                            value = tokenValue;
                        break;

                    default:
                        value = tokenValue;
                        break;
                }

                if ((i + 1) < tokens.Length)
                    connective = tokens[i + 1];
            }

            return value;
        }

        void ReadVariable(XmlNode xmlNode)
        {
            LinguisticVariable linguisticVariable = this.linguisticVariableCollection.Find(xmlNode.Attributes["NAME"].InnerText);

            foreach (XmlNode termNode in xmlNode.ChildNodes)
            {
                string[] points = termNode.Attributes["POINTS"].InnerText.Split();
                linguisticVariable.MembershipFunctionCollection.Add(new MembershipFunction(
                    termNode.Attributes["NAME"].InnerText,
                    Convert.ToDouble(points[0]),
                    Convert.ToDouble(points[1]),
                    Convert.ToDouble(points[2]),
                    Convert.ToDouble(points[3])));
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// A collection of linguistic variables.
        /// </summary>
        public LinguisticVariableCollection LinguisticVariableCollection
        {
            get { return linguisticVariableCollection; }
            set { linguisticVariableCollection = value; }
        }

        /// <summary>
        /// The consequent variable name.
        /// </summary>
        public string Consequent
        {
            get { return consequent; }
            set { consequent = value; }
        }

        /// <summary>
        /// A collection of rules.
        /// </summary>
        public FuzzyRuleCollection FuzzyRuleCollection
        {
            get { return fuzzyRuleCollection; }
            set { fuzzyRuleCollection = value; }
        }

        /// <summary>
        /// The path of the FCL-like XML file in which save the project.
        /// </summary>
        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Calculates the defuzzification value with the CoG (Center of Gravity) technique.
        /// </summary>
        /// <returns>The defuzzification value.</returns>
        public double Defuzzify()
        {
            double numerator = 0;
            double denominator = 0;

            // Reset values
            foreach (MembershipFunction membershipFunction in this.GetConsequent().MembershipFunctionCollection)
            {
                membershipFunction.Value = 0;
            }

            foreach (FuzzyRule fuzzyRule in this.fuzzyRuleCollection)
            {
                fuzzyRule.Value = Parse(fuzzyRule.Conditions());

                string[] tokens = fuzzyRule.Text.Split();
                MembershipFunction membershipFunction = this.GetConsequent().MembershipFunctionCollection.Find(tokens[tokens.Length - 1]);
                
                if (fuzzyRule.Value > membershipFunction.Value)
                    membershipFunction.Value = fuzzyRule.Value;
            }

            foreach (MembershipFunction membershipFunction in this.GetConsequent().MembershipFunctionCollection)
            {
                numerator += membershipFunction.Centorid() * membershipFunction.Area();
                denominator += membershipFunction.Area();
            }

            return numerator / denominator;
        }

        /// <summary>
        /// Sets the FilePath property and saves the project into a FCL-like XML file.
        /// </summary>
        /// <param name="path">Path of the destination document.</param>
        public void Save(string path)
        {
            this.FilePath = path;
            this.Save();
        }

        /// <summary>
        /// Saves the project into a FCL-like XML file.
        /// </summary>
        public void Save()
        {
            if (this.filePath == String.Empty)
                throw new Exception("FilePath not set");

            int i = 0;
            XmlTextWriter xmlTextWriter = new XmlTextWriter(this.filePath, Encoding.UTF8);
            xmlTextWriter.Formatting = Formatting.Indented;
            xmlTextWriter.WriteStartDocument(true);
            xmlTextWriter.WriteStartElement("FUNCTION_BLOCK");

            foreach (LinguisticVariable linguisticVariable in this.linguisticVariableCollection)
            {
                if (linguisticVariable.Name == this.consequent)
                    xmlTextWriter.WriteStartElement("VAR_OUTPUT");
                else
                    xmlTextWriter.WriteStartElement("VAR_INPUT");

                xmlTextWriter.WriteAttributeString("NAME", linguisticVariable.Name);
                xmlTextWriter.WriteAttributeString("TYPE", "REAL");
                xmlTextWriter.WriteAttributeString("RANGE",
                    linguisticVariable.MinValue().ToString() + " " +
                    linguisticVariable.MaxValue().ToString());
                xmlTextWriter.WriteEndElement();
            }

            foreach (LinguisticVariable linguisticVariable in this.linguisticVariableCollection)
            {
                if (linguisticVariable.Name == this.consequent)
                {
                    xmlTextWriter.WriteStartElement("DEFUZZIFY");
                    xmlTextWriter.WriteAttributeString("METHOD", "CoG");
                    xmlTextWriter.WriteAttributeString("ACCU", "MAX");
                }
                else
                    xmlTextWriter.WriteStartElement("FUZZIFY");
                
                xmlTextWriter.WriteAttributeString("NAME", linguisticVariable.Name);

                foreach (MembershipFunction membershipFunction in linguisticVariable.MembershipFunctionCollection)
                {
                    xmlTextWriter.WriteStartElement("TERM");
                    xmlTextWriter.WriteAttributeString("NAME", membershipFunction.Name);
                    xmlTextWriter.WriteAttributeString("POINTS",
                        membershipFunction.X0 + " " +
                        membershipFunction.X1 + " " +
                        membershipFunction.X2 + " " +
                        membershipFunction.X3);
                    xmlTextWriter.WriteEndElement();
                }

                xmlTextWriter.WriteEndElement();
            }

            xmlTextWriter.WriteStartElement("RULEBLOCK");
            xmlTextWriter.WriteAttributeString("AND", "MIN");
            xmlTextWriter.WriteAttributeString("OR", "MAX");

            foreach (FuzzyRule fuzzyRule in this.fuzzyRuleCollection)
            {
                i++;
                xmlTextWriter.WriteStartElement("RULE");
                xmlTextWriter.WriteAttributeString("NUMBER", i.ToString());
                xmlTextWriter.WriteAttributeString("TEXT", fuzzyRule.Text);
                xmlTextWriter.WriteEndElement();
            }
            
            xmlTextWriter.WriteEndElement();

            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteEndDocument();
            xmlTextWriter.Close();
        }

        /// <summary>
        /// Sets the FilePath property and loads a project from a FCL-like XML file.
        /// </summary>
        /// <param name="path">Path of the source file.</param>
        public void Load(string path)
        {
            this.FilePath = path;
            this.Load();
        }

        /// <summary>
        /// Loads a project from a FCL-like XML file.
        /// </summary>
        public void Load()
        {
            if (this.filePath == String.Empty)
                throw new Exception("FilePath not set");

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(this.filePath);

            foreach (XmlNode xmlNode in xmlDocument.GetElementsByTagName("VAR_INPUT"))
            {
                this.LinguisticVariableCollection.Add(new LinguisticVariable(xmlNode.Attributes["NAME"].InnerText));
            }

            this.consequent = xmlDocument.GetElementsByTagName("VAR_OUTPUT")[0].Attributes["NAME"].InnerText;
            this.LinguisticVariableCollection.Add(new LinguisticVariable(this.consequent));

            foreach (XmlNode xmlNode in xmlDocument.GetElementsByTagName("FUZZIFY"))
            {
                ReadVariable(xmlNode);
            }

            ReadVariable(xmlDocument.GetElementsByTagName("DEFUZZIFY")[0]);

            foreach (XmlNode xmlNode in xmlDocument.GetElementsByTagName("RULE"))
            {
                this.fuzzyRuleCollection.Add(new FuzzyRule(xmlNode.Attributes["TEXT"].InnerText));
            }
        }

        #endregion
    }
}
