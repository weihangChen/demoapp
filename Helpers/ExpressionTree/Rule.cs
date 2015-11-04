using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.ExpressionTree
{

    public enum Operator
    {
        GreaterThan, Equal, Contains, ToLower
    }

    public enum ParamName
    {
        x, y, t, k
    }

    public class Rule
    {
        public int RuleId { get; set; }

        public Rule(string MemberName, Operator Operator, string TargetValue)
        {
            this.MemberName = MemberName;
            this.Operator = Operator;
            this.TargetValue = TargetValue;
        }

        //rule engine related
        public dynamic TargetObj { get; set; }
        public string MemberName { get; set; }
        public Operator Operator { get; set; }
        public string TargetValue { get; set; }
        public int CompareEntityId { get; set; }
        public string Exp_DynamicLinq { get; set; }
        public dynamic ForwardingValue { get; set; }
        public bool ShouldStopAtValidationFailure { get; set; }
        public Expression Exp { get; set; }
        public T GetCompareEntity<T>(Func<int, T> callback)
        {
            if (CompareEntityId == 0)
                return default(T);
            return callback(CompareEntityId);
        }

    }

    public class ChainedRule : Rule
    {
        public ChainedRule(string MemberName, Operator Operator, string TargetValue)
            : base(MemberName, Operator, TargetValue)
        {

        }
        public int ChainedRuleId { get; set; }
        public int ParentRuleId { get; set; }
        public int ChildRuleId { get; set; }
        public ChainedRule ParentRule { get; set; }
        public ChainedRule ChildRule { get; set; }
        public int OrderInCollection { get; set; }
    }
}
