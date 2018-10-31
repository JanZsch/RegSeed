using System.Collections.Generic;
using System.Linq;
using Regseed.Common.Helper;
using Regseed.Common.Random;
using Regseed.Common.Ranges;
using Regseed.Factories;

namespace Regseed.Expressions
{
    internal class ConcatenationExpression : BaseExpression
    {
        private readonly List<IExpression> _expressions;

        public ConcatenationExpression(IRandomGenerator random) : base(random)
        {
            _expressions = new List<IExpression>();
        }

        public ConcatenationExpression Append(IExpression expression)
        {
            _expressions.Add(expression);
            return this;
        }
     
        public override IExpression GetInverse()
        {
            var seed = new List<List<IExpression>> {_expressions};
            
            var expandedRepeatRepresentation = ExpandHelper.ExpandListRepresentation(seed, WasExpandedRepeatExpressionAddedToList);

            var intersectInverseList = expandedRepeatRepresentation.Select(GetInverseOfExpandedConcatExpression).ToList();
            
            return new IntersectionExpression(intersectInverseList, _random);
        }

        private IExpression GetInverseOfExpandedConcatExpression(List<IExpression> concatExpressionRepresentation)
        {
            var inverseList = new List<IExpression>();

            for (var i = 0; i <  concatExpressionRepresentation.Count; i++)
            {
                var predecessorRange = concatExpressionRepresentation.GetRange(0, i);
                var inverse = concatExpressionRepresentation[i].GetInverse();
                var successorRange = concatExpressionRepresentation.GetRange(i + 1, concatExpressionRepresentation.Count - i - 1);

                var concatExpression = new ConcatenationExpression(_random);
                concatExpression.AppendRange(predecessorRange)
                                .Append(inverse)
                                .AppendRange(successorRange);
                
                inverseList.Add(concatExpression);
            }

            return new UnionExpression(inverseList, _random);
        }

        public override IList<IStringBuilder> Expand()
        {
            var seed = new List<List<IExpression>> {_expressions};
            
            var expandedRepeatRepresentation = ExpandHelper.ExpandListRepresentation(seed, WasExpandedRepeatExpressionAddedToList);

            var convertedExpressions = new List<List<IList<IStringBuilder>>>();

            foreach (var expressionList in expandedRepeatRepresentation)
            {
                var expandedExpression = expressionList.Select(x => x.Expand()).ToList();
                convertedExpressions.Add(expandedExpression);
            }

            var expandedUnionList = ExpandHelper.ExpandListRepresentation(convertedExpressions, ExpandHelper.WasExpandedStringBuilderListAddedToList);

            return CreateConcatenatedStringBuilderForEachExpansion(expandedUnionList);
        }

        protected override IStringBuilder ToSingleStringBuilder()
        {
            IStringBuilder builder = StringBuilder.Empty;

            foreach (var expression in _expressions)
                builder = builder.ConcatWith(expression.ToStringBuilder());

            return builder;
        }

        internal IList<IExpression> ToConcatExpressionList() => _expressions.ToList();
        
        private static IList<IStringBuilder> CreateConcatenatedStringBuilderForEachExpansion(List<List<IList<IStringBuilder>>> expandedUnionList)
        {
            var resultList = new List<IStringBuilder>();

            foreach (var expandedUnionExpression in expandedUnionList)
            {
                IStringBuilder stringBuilder = StringBuilder.Empty;

                foreach (var singleStringBuilder in expandedUnionExpression)
                    stringBuilder = stringBuilder.ConcatWith(singleStringBuilder.FirstOrDefault());

                resultList.Add(stringBuilder);
            }

            return resultList;
        }

        private ConcatenationExpression AppendRange(IEnumerable<IExpression> expressions)
        {
            foreach (var expression in expressions)
                _expressions.Add(expression);
    
            return this;
        }
        
        private static bool WasExpandedRepeatExpressionAddedToList(List<IExpression> repeatExpressions,
            List<List<IExpression>> newExpandList, int concatElementPosition)
        {
            var repeatExpression = repeatExpressions[concatElementPosition];
            var repeatRange = repeatExpression.RepeatRange ?? new IntegerInterval(1);
            var start = repeatRange.Start ?? 0;
            var end = repeatRange.End ?? int.MaxValue - 1;
            end = end == int.MaxValue ? int.MaxValue - 1 : end;

            if (start == 1 && end == 1)
                return false;

            for (var i = start; i < end + 1; i++)
            {
                var distanceConcatPositionToListEnd = repeatExpressions.Count - concatElementPosition - 1;
                var expandedExpressions = new List<IExpression>();
                expandedExpressions.AddRange(repeatExpressions.GetRange(0, concatElementPosition));
                expandedExpressions.AddRange(ConcatElementaryExpression(repeatExpression, i));
                expandedExpressions.AddRange(repeatExpressions.GetRange(concatElementPosition + 1,distanceConcatPositionToListEnd));
                newExpandList.Add(expandedExpressions);
            }

            return true;
        }

       private static IEnumerable<IExpression> ConcatElementaryExpression(IExpression expression, int concatenations)
        {
            var expandedExpression = new List<IExpression>();

            expression.RepeatRange = new IntegerInterval(1);
            
            for (var i = 0; i < concatenations; i++)
                expandedExpression.Add(expression);

            return expandedExpression;
        }
    }
}