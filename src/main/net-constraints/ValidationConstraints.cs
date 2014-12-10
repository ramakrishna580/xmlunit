/*
  This file is licensed to You under the Apache License, Version 2.0
  (the "License"); you may not use this file except in compliance with
  the License.  You may obtain a copy of the License at

  http://www.apache.org/licenses/LICENSE-2.0

  Unless required by applicable law or agreed to in writing, software
  distributed under the License is distributed on an "AS IS" BASIS,
  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  See the License for the specific language governing permissions and
  limitations under the License.
*/

using System.Linq;
using System.Text;
using NUnit.Framework.Constraints;
using Org.XmlUnit.Validation;

namespace Org.XmlUnit.Constraints {

    /// <summary>
    /// Constraint that validates a document against a given XML
    /// schema.
    /// </summary>
    public class SchemaValidConstraint : Constraint {
        private readonly Validator validator;
        private ValidationResult result;

        /// <summary>
        /// Creates the constraint validating against the given schema(s).
        /// </summary>
        public SchemaValidConstraint(params ISource[] schema) : base(schema) {
            validator = Validator.ForLanguage(Languages.W3C_XML_SCHEMA_NS_URI);
            validator.SchemaSources = schema;
        }

        public override bool Matches(object o) {
            this.actual = o;
            if (o is ISource) {
                result = validator.ValidateInstance(o as ISource);
                return result.Valid;
            }
            return false;
        }

        public override void WriteDescriptionTo(MessageWriter writer) {
            writer.Write("{0} validates against {1}", GrabActual(),
                         GrabSystemIds());
        }

        public override void WriteActualValueTo(MessageWriter writer) {
            if (actual is ISource) {
                writer.Write("got validation errors: {0}", GrabProblems());
            } else {
                writer.Write("{0} that is not ISource", GrabActual());
            }
        }

        private string GrabSystemIds() {
            return validator.SchemaSources.Select(GrabSystemId)
                .Aggregate(new StringBuilder(),
                           (sb, systemId) => sb.AppendLine(systemId),
                           sb => sb.Remove(sb.Length - 1, 1).ToString());
        }

        private string GrabActual() {
            ISource actualSource = actual as ISource;
            return actualSource != null ? GrabSystemId(actualSource)
                : (actual ?? "null").ToString();
        }

        private string GrabSystemId(ISource s) {
            return s.SystemId;
        }

        private string GrabProblems() {
            return result.Problems
                .Aggregate(new StringBuilder(),
                           (sb, p) => sb.AppendFormat("{0}, ", p),
                           sb => sb.Remove(sb.Length - 2, 2).ToString());
        }

        private string ProblemToString(ValidationProblem problem) {
            return problem.ToString();
        }
    }
}