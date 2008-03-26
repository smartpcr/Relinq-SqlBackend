using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using Rubicon.Data.Linq.Clauses;
using Rubicon.Data.Linq.Parsing.Structure;
using Rubicon.Utilities;

namespace Rubicon.Data.Linq.Parsing.Structure
{
  public class QueryExpressionCreator
  {
    private readonly Expression _expressionTreeRoot;
    private readonly ParseResultCollector _result;
    private readonly List<IBodyClause> _bodyClauses = new List<IBodyClause> ();

    private IClause _previousClause;
    private int _currentProjection;
    private OrderByClause _previousOrderByClause;

    public QueryExpressionCreator (Expression expressionTreeRoot, ParseResultCollector result)
    {
      ArgumentUtility.CheckNotNull ("expressionTreeRoot", expressionTreeRoot);
      ArgumentUtility.CheckNotNull ("result", result);

      _expressionTreeRoot = expressionTreeRoot;
      _result = result;
    }

    public QueryExpression CreateQueryExpression()
    {
      var mainFromClause = CreateMainFromClause (_result);

      _previousClause = mainFromClause;
      _currentProjection = 0;
      _previousOrderByClause = null;

      foreach (BodyExpressionBase bodyExpression in _result.BodyExpressions)
      {
        var clause = CreateBodyClause (bodyExpression);
        if (clause != _previousClause)
          _bodyClauses.Add (clause);

        _previousClause = clause;
      }

      var selectClause = CreateSelectClause();
      var queryExpression = new QueryExpression (_expressionTreeRoot.Type, mainFromClause, selectClause, _expressionTreeRoot);

      foreach (IBodyClause bodyClause in _bodyClauses)
        queryExpression.AddBodyClause (bodyClause);

      return queryExpression;
    }

    private MainFromClause CreateMainFromClause (ParseResultCollector resultCollector)
    {
      Assertion.IsTrue (resultCollector.BodyExpressions.Count > 0 && resultCollector.BodyExpressions[0] is FromExpression);

      FromExpression mainFromExpression = resultCollector.ExtractMainFromExpression ();
      return new MainFromClause (mainFromExpression.Identifier, (IQueryable) ((ConstantExpression) mainFromExpression.Expression).Value);
    }

    private IBodyClause CreateBodyClause (BodyExpressionBase expression)
    {
      AdditionalFromClause fromClause = CreateFromClause (expression);
      if (fromClause != null)
        return fromClause;

      WhereClause whereClause = CreateWhereClause(expression);
      if (whereClause != null)
        return whereClause;

      OrderByClause orderByClause = CreateOrderByClause(expression);
      if (orderByClause != null)
        return orderByClause;

      throw new ParserException ("The FromLetWhereExpression type " + expression.GetType ().Name + " is not supported.");
    }

    private AdditionalFromClause CreateFromClause (BodyExpressionBase expression)
    {
      var fromExpression = expression as FromExpression;
      if (fromExpression == null)
        return null;

      if (_currentProjection >= _result.ProjectionExpressions.Count)
      {
        string message = string.Format ("From expression '{0}' ({1}) doesn't have a projection expression.", fromExpression.Identifier,
            fromExpression.Expression);
        throw new ParserException (message, _expressionTreeRoot, _expressionTreeRoot, null);
      }

      var additionalFromClause = new AdditionalFromClause (_previousClause, fromExpression.Identifier,
          (LambdaExpression) fromExpression.Expression, _result.ProjectionExpressions[_currentProjection]);
      ++_currentProjection;
      return additionalFromClause;
    }

    private WhereClause CreateWhereClause (BodyExpressionBase expression)
    {
      var whereExpression = expression as WhereExpression;
      if (whereExpression == null)
        return null;

      var whereClause = new WhereClause (_previousClause, whereExpression.Expression);
      return whereClause;
    }

    private OrderByClause CreateOrderByClause (BodyExpressionBase expression)
    {
      var orderExpression = expression as OrderExpression;
      if (orderExpression == null)
        return null;

      var orderingClause = new OrderingClause (_previousClause, orderExpression.Expression, orderExpression.OrderDirection);
      if (orderExpression.FirstOrderBy)
      {
        var orderByClause = new OrderByClause (orderingClause);
        _previousOrderByClause = orderByClause;
        return orderByClause;
      }
      else
      {
        if (_previousOrderByClause == null)
          throw ParserUtility.CreateParserException ("OrderBy or OrderByDescending", orderExpression, "beginning of an OrderBy clause",
              _expressionTreeRoot);
        else
        {
          _previousOrderByClause.Add (orderingClause);
          return _previousOrderByClause;
        }
      }
    }

    private SelectClause CreateSelectClause ()
    {
      if (_result.ProjectionExpressions.Count == 0)
      {
        string message = "There is no projection for the select clause.";
        throw new ParserException (message, _expressionTreeRoot, _expressionTreeRoot, null);
      }

      LambdaExpression selectProjection = _result.ProjectionExpressions.Last ();
      return new SelectClause (_previousClause, selectProjection, _result.IsDistinct);
    }
  }
}