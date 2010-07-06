// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Linq.Expressions;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.SqlBackend.SqlPreparation
{
  /// <summary>
  /// <see cref="SqlPreparationSelectExpressionVisitor"/> transforms the expressions stored by <see cref="SqlStatement.SelectProjection"/> to a SQL-specific
  /// format.
  /// </summary>
  public class SqlPreparationSelectExpressionVisitor : SqlPreparationExpressionVisitor
  {
    public new static Expression TranslateExpression (
        Expression expression,
        ISqlPreparationContext context,
        ISqlPreparationStage stage,
        UniqueIdentifierGenerator generator,
        MethodCallTransformerRegistry registry)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("stage", stage);
      ArgumentUtility.CheckNotNull ("generator", generator);
      ArgumentUtility.CheckNotNull ("registry", registry);

      var visitor = new SqlPreparationSelectExpressionVisitor (context, stage, generator, registry);
      var result = visitor.VisitExpression (expression);
      return result;
    }

    protected SqlPreparationSelectExpressionVisitor (
        ISqlPreparationContext context, ISqlPreparationStage stage, UniqueIdentifierGenerator generator, MethodCallTransformerRegistry registry)
        : base (context, stage, generator, registry)
    {
    }

    public override Expression VisitSqlSubStatementExpression (SqlSubStatementExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      //TODO: RM-3007
      //var subStatementTableInfo = new ResolvedSubStatementTableInfo (Generator.GetUniqueIdentifier ("q"), expression.SqlStatement);
      //var sqlTable = new SqlTable (subStatementTableInfo);
      //var sqlTableReferenceExpression = new SqlTableReferenceExpression (sqlTable);
      //var fromExpressionInfo = new FromExpressionInfo (sqlTable, new Ordering[0], sqlTableReferenceExpression, null, true);
      //Context.AddFromExpression (fromExpressionInfo);
      //return sqlTableReferenceExpression;
      
      return base.VisitSqlSubStatementExpression (expression);
    }
  }
}