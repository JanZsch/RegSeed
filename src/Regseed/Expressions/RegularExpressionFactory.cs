using System;
using System.Collections.Generic;
using Regseed.Common.Random;
using Regseed.Common.Ranges;
using Regseed.Common.Results;
using Regseed.Parser;
using Regseed.Parser.RegexTokens;
using Regseed.Resources;
using Regseed.Streams;

namespace Regseed.Expressions
{
    internal class RegularExpressionFactory
    {
        private readonly IParserAlphabet _alphabet;
        private readonly IRandomGenerator _random;
        private readonly IDictionary<RegexTokenType, TryGetExpression> _regexTokenToExpressionMapper;
        private readonly IDictionary<RegexTokenType, TryGetExpression> _regexTokenToExpressionListMapper;
        private readonly int _maxCharClassInverseLength;

        private readonly  IDictionary<Type, Func<List<IExpression>, IExpression>> _listTypeExpressionFactories;
        private readonly  IDictionary<Type, RegexTokenType> _expressionTokenTypeMapper;

        
        public RegularExpressionFactory(IParserAlphabet alphabet, IRandomGenerator random, int maxCharClassInverseLength)
        {
            _random = random;
            _alphabet = alphabet;
            _maxCharClassInverseLength = maxCharClassInverseLength;
            
            _regexTokenToExpressionMapper = new Dictionary<RegexTokenType, TryGetExpression>
            {
                {RegexTokenType.Complement, TryGetInverseElementaryExpression},
                {RegexTokenType.OpenPrecedence, TryGetGroupExpression},
                {RegexTokenType.OpenCharacterClass, TryGetCharacterClassExpression},
                {RegexTokenType.OpenNegateCharacterClass, TryGetCharacterClassExpression},
                {RegexTokenType.AnyCharacter, TryGetAnyCharacterExpression},
                {RegexTokenType.Character, TryGetSingleCharacterExpression}
            };
            
            _regexTokenToExpressionListMapper = new Dictionary<RegexTokenType, TryGetExpression>
            {
                {RegexTokenType.Union, TryGetIntersectionExpression},
                {RegexTokenType.Intersection, TryGetConcatenationExpression}
            };

            _listTypeExpressionFactories = new Dictionary<Type, Func<List<IExpression>, IExpression>>
            {
                {typeof(SeedUnionExpression), x => new SeedUnionExpression(x, _random)},
                {typeof(UnionExpression), x => new UnionExpression(x, _random)},
                {typeof(IntersectionExpression), x => new IntersectionExpression(x, _random)}
            };
            
            _expressionTokenTypeMapper = new Dictionary<Type, RegexTokenType>
            {
                {typeof(SeedUnionExpression), RegexTokenType.Union},
                {typeof(UnionExpression), RegexTokenType.Union},
                {typeof(IntersectionExpression), RegexTokenType.Intersection}
            };
            
        }

        public IParseResult<ExpressionMetaData> TryGetRegularExpression(string pattern, out IExpression expression)
        {
            expression = null;
            var lexer = new Lexer(_alphabet);

            var parseResult = lexer.TryConvertToTokenStream(pattern, out var tokenStream);

            if (!parseResult.IsSuccess)
                return new FailureParseResult<ExpressionMetaData>(parseResult.Position, parseResult.ErrorType);

            var unionResult = TryGetUnionExpression<SeedUnionExpression>(tokenStream, out var regex);

            if (!unionResult.IsSuccess)
                return unionResult;

            if (tokenStream.LookAhead(0).GetType<RegexTokenType>() != RegexTokenType.EndOfStream)
                return new FailureParseResult<ExpressionMetaData>(tokenStream.CurrentPosition, RegSeedErrorType.UnionExpressionExpected);
            
            expression = regex;
            return unionResult;
        }

        private IParseResult<ExpressionMetaData> TryGetUnionExpression<TExpression>(ITokenStream tokenStream, out IExpression unionExpression) where TExpression : UnionExpression =>
            TryGetExpressionListSeparatedByToken<TExpression>(tokenStream, out unionExpression);


        private IParseResult<ExpressionMetaData> TryGetIntersectionExpression(ITokenStream tokenStream, out IExpression intersectionExpression) =>
            TryGetExpressionListSeparatedByToken<IntersectionExpression>(tokenStream, out intersectionExpression);

        private IParseResult<ExpressionMetaData> TryGetExpressionListSeparatedByToken<TExpression>(ITokenStream tokenStream, out IExpression expression)
        {
            expression = new EmptyExpression();
            
            if (tokenStream.LookAhead(0).GetType<RegexTokenType>() == RegexTokenType.EndOfStream)
                return new SuccessParseResult<ExpressionMetaData>(tokenStream.CurrentPosition, new ExpressionMetaData());

            var token = _expressionTokenTypeMapper[typeof(TExpression)];
            var subExpressionResult = TryGetSubExpressionList(token, tokenStream, out var subExpressionList);

            if(!subExpressionResult.IsSuccess)
                return subExpressionResult;

            expression = CreateExpression<TExpression>(subExpressionList);
            return new SuccessParseResult<ExpressionMetaData>(tokenStream.CurrentPosition, subExpressionResult.Value);
        }

        private IParseResult<ExpressionMetaData> TryGetSubExpressionList(RegexTokenType token, ITokenStream tokenStream, out List<IExpression> subExpressionList)
        {
            subExpressionList = null;            
            
            var subExpressionResult = _regexTokenToExpressionListMapper[token](tokenStream, out var subExpression);

            if (!subExpressionResult.IsSuccess)
                return subExpressionResult;

            subExpressionList = new List<IExpression> {subExpression};

            var nextToken = tokenStream.LookAhead(0).GetType<RegexTokenType>();
            var metaData = subExpressionResult.Value;

            while (nextToken == token)
            {
                if (token == RegexTokenType.Intersection)
                    metaData.HasIntersection = true;

                tokenStream.Pop();

                subExpressionResult = _regexTokenToExpressionListMapper[token](tokenStream, out subExpression);

                if (!subExpressionResult.IsSuccess)
                    return subExpressionResult;

                metaData.UpdateWith(subExpressionResult.Value);
                subExpressionList.Add(subExpression);
                nextToken = tokenStream.LookAhead(0).GetType<RegexTokenType>();
            }

            return new SuccessParseResult<ExpressionMetaData>(tokenStream.CurrentPosition, metaData);
        }

        private IExpression CreateExpression<TExpression>(List<IExpression> expressions) =>
         _listTypeExpressionFactories.TryGetValue(typeof(TExpression), out var factory)
                ? factory(expressions)
                : new EmptyExpression();
        
        private IParseResult<ExpressionMetaData> TryGetConcatenationExpression(ITokenStream tokenStream, out IExpression expression)
        {
            expression = null;
            var concatExpression = new ConcatenationExpression(_random);

            var elementaryExpressionResult = TryGetElementaryExpression(tokenStream, out var elementaryExpression);

            if (!elementaryExpressionResult.IsSuccess)
                return elementaryExpressionResult;

            var metaData = elementaryExpressionResult.Value;
            elementaryExpression.RepeatRange = GetRepeatConcatenationInterval(tokenStream);
            concatExpression.Append(elementaryExpression);

            var result = TryGetElementaryExpression(tokenStream, out elementaryExpression);
            while (result.IsSuccess)
            {
                metaData.UpdateWith(result.Value);
                elementaryExpression.RepeatRange = GetRepeatConcatenationInterval(tokenStream);
                concatExpression.Append(elementaryExpression);
                result = TryGetElementaryExpression(tokenStream, out elementaryExpression);
            }

            if (result.ErrorType != RegSeedErrorType.None)
                return result;
            
            expression = concatExpression;
            return new SuccessParseResult<ExpressionMetaData>(tokenStream.CurrentPosition, metaData);
        }

        private IParseResult<ExpressionMetaData> TryGetElementaryExpression(ITokenStream tokenStream, out IExpression expression)
        {
            expression = null;
            var nextToken = tokenStream.LookAhead(0).GetType<RegexTokenType>();
           
            return _regexTokenToExpressionMapper.TryGetValue(nextToken, out var tryGetElementaryExpression)
                ? tryGetElementaryExpression(tokenStream, out expression)
                : new FailureParseResult<ExpressionMetaData>(tokenStream.CurrentPosition);
        }

        private IParseResult<ExpressionMetaData> TryGetInverseElementaryExpression(ITokenStream tokenStream, out IExpression expression)
        {
            expression = null;
            tokenStream.Pop();
            var elementaryResult = TryGetElementaryExpression(tokenStream, out var toInvertExpression);

            if (!elementaryResult.IsSuccess)
                return elementaryResult;

            expression = toInvertExpression.GetInverse();
            expression.MaxExpansionRange = IntegerInterval.MaxInterval;

            return new SuccessParseResult<ExpressionMetaData>(tokenStream.CurrentPosition, elementaryResult.Value);
        }

        private IParseResult<ExpressionMetaData> TryGetAnyCharacterExpression(ITokenStream tokenStream, out IExpression expression)
        {
            tokenStream.Pop();
            
            var characterExpression = new CharacterClassExpression(_alphabet, _random, _maxCharClassInverseLength);
            characterExpression.AddCharacters(_alphabet.GetAllCharacters());
            
            expression = characterExpression;
        
            return new SuccessParseResult<ExpressionMetaData>(tokenStream.CurrentPosition, new ExpressionMetaData());
        }

        private IParseResult<ExpressionMetaData> TryGetGroupExpression(ITokenStream tokenStream, out IExpression expression)
        {
            tokenStream.Pop();

            var closePrecedenceToken = tokenStream.LookAhead(0).GetType<RegexTokenType>();

            if (closePrecedenceToken == RegexTokenType.ClosePrecedence)
            {
                tokenStream.Pop();
                expression = new EmptyExpression();
                return new SuccessParseResult<ExpressionMetaData>(tokenStream.CurrentPosition, new ExpressionMetaData());
            }

            var unionExpressionResult = TryGetUnionExpression<UnionExpression>(tokenStream, out expression);

            if (!unionExpressionResult.IsSuccess)
                return unionExpressionResult;

            closePrecedenceToken = tokenStream.LookAhead(0).GetType<RegexTokenType>();

            if (closePrecedenceToken != RegexTokenType.ClosePrecedence)
                return new FailureParseResult<ExpressionMetaData>(tokenStream.CurrentPosition, RegSeedErrorType.ClosePrecedenceExpected);

            tokenStream.Pop();
            return new SuccessParseResult<ExpressionMetaData>(tokenStream.CurrentPosition, unionExpressionResult.Value);
        }

        private IParseResult<ExpressionMetaData> TryGetCharacterClassExpression(ITokenStream tokenStream, out IExpression expression)
        {
            expression = null;

            var nextTokenType = tokenStream.Pop().GetType<RegexTokenType>();

            var getComplement = nextTokenType == RegexTokenType.OpenNegateCharacterClass;

            nextTokenType = tokenStream.LookAhead(0).GetType<RegexTokenType>();

            var characters = new List<string>();

            while (nextTokenType == RegexTokenType.Character || nextTokenType == RegexTokenType.CharacterRange)
            {
                var nextToken = tokenStream.Pop();

                if (nextToken.GetType<RegexTokenType>() == RegexTokenType.Character)
                    characters.Add(nextToken.GetValue<string>());
                else
                    characters.AddRange(nextToken.GetValue<CharacterRange>().Characters);

                nextTokenType = tokenStream.LookAhead(0).GetType<RegexTokenType>();
            }

            if (nextTokenType != RegexTokenType.CloseCharacterClass)
                return new FailureParseResult<ExpressionMetaData>(tokenStream.CurrentPosition, RegSeedErrorType.CloseCharacterClassExpected);

            tokenStream.Pop();

            var characterClassExpression = new CharacterClassExpression(_alphabet, _random, _maxCharClassInverseLength);

            characterClassExpression.AddCharacters(characters);

            expression = getComplement ? characterClassExpression.GetComplement() : characterClassExpression;
            return new SuccessParseResult<ExpressionMetaData>(tokenStream.CurrentPosition, new ExpressionMetaData());
        }

        private IParseResult<ExpressionMetaData> TryGetSingleCharacterExpression(ITokenStream tokenStream, out IExpression expression)
        {
            expression = null;
            var tokenValue = tokenStream.Pop().GetValue<string>();
            var characterClassExpression = new CharacterClassExpression(_alphabet, _random, _maxCharClassInverseLength);

            characterClassExpression.AddCharacters(new List<string> {tokenValue});
                
            expression = characterClassExpression;

            return new SuccessParseResult<ExpressionMetaData>(tokenStream.CurrentPosition, new ExpressionMetaData());
        }
        
        private static IntegerInterval GetRepeatConcatenationInterval(ITokenStream tokenStream)
        {
            var nextToken = tokenStream.LookAhead(0).GetType<RegexTokenType>();

            return nextToken != RegexTokenType.IntegerRange
                ? new IntegerInterval(1)
                : tokenStream.Pop().GetValue<IntegerInterval>();
        }

        private delegate IParseResult<ExpressionMetaData> TryGetExpression(ITokenStream stream, out IExpression expression);
    }
}