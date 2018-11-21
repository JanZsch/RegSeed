using System;
using Regseed.Common.Ranges;
using Regseed.Common.Results;
using Regseed.Parser.Settings;
using Regseed.Resources;
using Regseed.Streams;

namespace Regseed.Parser.PrimitiveParsers
{
    internal class PrimitiveParser : IPrimitiveParser
    {
        private readonly IParserAlphabet _alphabet;

        public PrimitiveParser(IParserAlphabet alphabet)
        {
            _alphabet = alphabet ?? throw new ArgumentNullException();
        }

        public IParseResult<CharacterRange> TryParseCharacterRange(IStringStream stream, string rangeSeparator = null)
        {
            rangeSeparator = rangeSeparator ?? SpecialCharacters.CharacterRangeSeparator;

            if (stream == null)
                throw new ArgumentNullException();

            var initialPosition = stream.CurrentPosition;

            var startCharacterResult = TryParseCharacterInternal(stream, 0, false);
            if (!startCharacterResult.IsSuccess)
                return new FailureParseResult<CharacterRange>(initialPosition);

            var popCalls = startCharacterResult.Value.Item2;

            if (popCalls >= stream.Count || !stream.LookAhead(popCalls).Equals(rangeSeparator))
                return new FailureParseResult<CharacterRange>(initialPosition + popCalls);

            popCalls++;

            var endCharacterResult = TryParseCharacterInternal(stream, popCalls, false);
            if (!endCharacterResult.IsSuccess)
                return new FailureParseResult<CharacterRange>(initialPosition, RegSeedErrorType.CharacterRangeExpected);

            CallPop(stream, endCharacterResult.Value.Item2);

            var value = new CharacterRange();
            
            return value.TrySetRange(startCharacterResult.Value.Item1, endCharacterResult.Value.Item1, _alphabet).IsSuccess 
                ? (IParseResult<CharacterRange>) new SuccessParseResult<CharacterRange>(initialPosition, value)
                : new FailureParseResult<CharacterRange>(initialPosition, RegSeedErrorType.InvalidRange);            
        }

        public IParseResult<int> TryParseInteger(IStringStream stream)
        {
            if (stream == null)
                throw new ArgumentNullException();

            var initialPosition = stream.CurrentPosition;

            var result = TryParseIntegerInternal(stream, 0, true);

            return result.IsSuccess && result.Value.Item1 != null
                ? (IParseResult<int>) new SuccessParseResult<int>(initialPosition, result.Value.Item1.Value)
                : new FailureParseResult<int>(initialPosition, result.ErrorType);
        }

        public IParseResult<IntegerInterval> TryParseIntegerInterval(IStringStream stream, ParseIntervalSettings settings = null)
        {
            if (stream == null)
                throw new ArgumentNullException();

            settings = settings ?? ParseIntervalSettings.Default;

            var initialPosition = stream.CurrentPosition;
            var popCalls = 0;

            if (popCalls >= stream.Count || !stream.LookAhead(popCalls).Equals(settings.OpenSymbol))
                return new FailureParseResult<IntegerInterval>(initialPosition, RegSeedErrorType.IntegerIntervalExpected);

            popCalls++;

            var lowerBound = IsNextCharacterSymbol(settings.Separator, stream, popCalls)
                ? new SuccessParseResult<Tuple<int?, int>>(0, new Tuple<int?, int>(null, popCalls))
                : TryParseIntegerInternal(stream, popCalls, false);

            if (!lowerBound.IsSuccess)
                return new FailureParseResult<IntegerInterval>(initialPosition + popCalls, RegSeedErrorType.IntegerIntervalExpected);

            popCalls = lowerBound.Value.Item2;

            if (lowerBound.Value.Item1 != null && IsNextCharacterSymbol(settings.CloseSymbol, stream, popCalls))
                return SingleIntegerInterval(lowerBound.Value.Item1.Value, stream, popCalls);

            if (!IsNextCharacterSymbol(settings.Separator, stream, popCalls))
                return new FailureParseResult<IntegerInterval>(initialPosition + popCalls, RegSeedErrorType.IntegerIntervalSeparatorExpected);

            popCalls++;

            var upperBound = IsNextCharacterSymbol(settings.CloseSymbol, stream, popCalls)
                ? new SuccessParseResult<Tuple<int?, int>>(0, new Tuple<int?, int>(null, popCalls))
                : TryParseIntegerInternal(stream, popCalls, false);

            if (!upperBound.IsSuccess)
                return new FailureParseResult<IntegerInterval>(initialPosition, RegSeedErrorType.CharacterRangeExpected);

            popCalls = upperBound.Value.Item2;

            if (popCalls >= stream.Count || !stream.LookAhead(popCalls).Equals(settings.CloseSymbol))
                return new FailureParseResult<IntegerInterval>(initialPosition, RegSeedErrorType.IntegerIntervalExpected);

            popCalls++;
            CallPop(stream, popCalls);

            var value = new IntegerInterval();

            return value.TrySetValue(lowerBound.Value.Item1, upperBound.Value.Item1).IsSuccess
                ? new SuccessParseResult<IntegerInterval>(initialPosition, value)
                : (IParseResult<IntegerInterval>) new FailureParseResult<IntegerInterval>(initialPosition, RegSeedErrorType.InvalidInterval);
        }

        public IParseResult<string> TryParseCharacter(IStringStream stream)
        {
            if (stream == null)
                throw new ArgumentNullException();

            var initialPosition = stream.CurrentPosition;

            var result = TryParseCharacterInternal(stream, 0, true);

            return result.IsSuccess
                ? (IParseResult<string>) new SuccessParseResult<string>(initialPosition, result.Value.Item1)
                : new FailureParseResult<string>(initialPosition, result.ErrorType);
        }

        private IParseResult<Tuple<string, int>> TryParseCharacterInternal(IStringStream stream, int initialPosition,
            bool executePop)
        {
            var popCalls = initialPosition;
            var currentPosition = stream.CurrentPosition;

            if (popCalls < stream.Count &&
                !stream.LookAhead(popCalls).Equals(SpecialCharacters.Escape) &&
                _alphabet.IsValid(stream.LookAhead(popCalls)))
                return ParseCharacterSuccessResult(stream, executePop, popCalls, currentPosition);

            var escapedCandidate = stream.LookAhead(popCalls++);

            if (popCalls >= stream.Count ||
                !escapedCandidate.Equals(SpecialCharacters.Escape) ||
                !_alphabet.IsValid(stream.LookAhead(popCalls)))
                return new FailureParseResult<Tuple<string, int>>(stream.CurrentPosition, RegSeedErrorType.SingleCharacterExpected);

            return ParseCharacterSuccessResult(stream, executePop, popCalls, currentPosition);
        }

        private static IParseResult<Tuple<string, int>> ParseCharacterSuccessResult(IStringStream stream,
            bool executePop, int popCalls, long currentPosition)
        {
            var value = stream.LookAhead(popCalls);

            popCalls++;

            if (executePop)
                CallPop(stream, popCalls);

            return new SuccessParseResult<Tuple<string, int>>(currentPosition, new Tuple<string, int>(value, popCalls));
        }

        private static void CallPop(IStringStream stream, int popCalls)
        {
            for (var i = 0; i < popCalls; i++)
                stream.Pop();
        }

        private static IParseResult<Tuple<int?, int>> TryParseIntegerInternal(IStringStream stream, int initialPops, bool executePop)
        {
            var popCalls = initialPops;

            var initialPosition = stream.CurrentPosition;

            if (popCalls >= stream.Count)
                return new FailureParseResult<Tuple<int?, int>>(stream.CurrentPosition, RegSeedErrorType.UnexpectedEndOfStream);

            var numberString = string.Empty;
            var charCandidate = stream.LookAhead(popCalls);

            if (charCandidate.Equals(SpecialCharacters.Minus))
            {
                popCalls++;
                numberString = "-";
            }

            if (popCalls >= stream.Count && numberString.Equals("-"))
                return new FailureParseResult<Tuple<int?, int>>(stream.CurrentPosition, RegSeedErrorType.IntegerExpected);

            if (popCalls >= stream.Count)
                return new FailureParseResult<Tuple<int?, int>>(stream.CurrentPosition, RegSeedErrorType.UnexpectedEndOfStream);

            PopAndAppendWhileIsDigit(stream, ref popCalls, ref numberString);

            if (executePop)
                CallPop(stream, popCalls);

            if (string.IsNullOrEmpty(numberString))
                return new FailureParseResult<Tuple<int?, int>>(initialPosition, RegSeedErrorType.IntegerExpected);

            var value = new Tuple<int?, int>(int.Parse(numberString), popCalls);
            return new SuccessParseResult<Tuple<int?, int>>(initialPosition, value);
        }

        private static void PopAndAppendWhileIsDigit(IStringStream stream, ref int initialPopCalls, ref string numberString)
        {
            var charCandidate = stream.LookAhead(initialPopCalls);

            while (charCandidate.Length == 1 && char.IsDigit(charCandidate[0]))
            {
                numberString = $"{numberString}{charCandidate}";
                initialPopCalls++;

                if (initialPopCalls >= stream.Count)
                    return;

                charCandidate = stream.LookAhead(initialPopCalls);
            }
        }

        private static IParseResult<IntegerInterval> SingleIntegerInterval(int value, IStringStream stream, int popCalls)
        {
            popCalls++;
            CallPop(stream, popCalls);
            return new SuccessParseResult<IntegerInterval>(stream.CurrentPosition, new IntegerInterval(value));
        }

        private static bool IsNextCharacterSymbol(string symbol, IStringStream stream, int position) =>
            position < stream.Count && symbol.Equals(stream.LookAhead(position));
    }
}