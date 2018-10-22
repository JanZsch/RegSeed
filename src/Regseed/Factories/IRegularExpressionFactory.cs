using Regseed.Common.Results;
using Regseed.Expressions;
using Regseed.Streams;

namespace Regseed.Factories
{
    public interface IRegularExpressionFactory
    {
        IParseResult<ExpressionMetaData> TryGetRegularExpression(string pattern, out IExpression expression);
        IParseResult<ExpressionMetaData> TryGetUnionExpression(ITokenStream tokenStream, out IExpression expression);
        IParseResult<ExpressionMetaData> TryGetConcatenationExpression(ITokenStream tokenStream, out IExpression expression);
        IParseResult<ExpressionMetaData> TryGetElementaryExpression(ITokenStream tokenStream, out IExpression expression);
        IParseResult<ExpressionMetaData> TryGetGroupExpression(ITokenStream tokenStream, out IExpression expression);
        IParseResult<ExpressionMetaData> TryGetCharacterClassExpression(ITokenStream tokenStream, out IExpression expression);
        IParseResult<ExpressionMetaData> TryGetSingleCharacterExpression(ITokenStream tokenStream, out IExpression expression);
        IParseResult<ExpressionMetaData> TryGetAnyCharacterExpression(ITokenStream tokenStream, out IExpression expression);
        IParseResult<ExpressionMetaData> TryGetComplementElementaryExpression(ITokenStream tokenStream, out IExpression expression);
    }
}