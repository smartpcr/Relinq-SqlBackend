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
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.StreamedData;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.SqlBackend.SqlPreparation.ResultOperatorHandlers
{
  /// <summary>
  /// Default implementation of <see cref="IResultOperatorHandler"/> providing commonly needed functionality.
  /// </summary>
  /// <typeparam name="T">The result operator type handled by the concrete subclass of <see cref="ResultOperatorHandler{T}"/>.</typeparam>
  public abstract class ResultOperatorHandler<T> : IResultOperatorHandler where T: ResultOperatorBase
  {
    public Type SupportedResultOperatorType 
    { 
      get { return typeof (T); }
    }

    public abstract void HandleResultOperator (T resultOperator, QueryModel queryModel, ref SqlStatementBuilder sqlStatementBuilder, UniqueIdentifierGenerator generator, ISqlPreparationStage stage);

    // TODO Review 2620: Add unit tests for these two methods (add a ResultOperatorHandlerTest and a TestResultOperatorHandler)
    protected void EnsureNoTopExpressionAndSetDataInfo (ResultOperatorBase resultOperator, ref SqlStatementBuilder sqlStatementBuilder, UniqueIdentifierGenerator generator, ISqlPreparationStage stage)
    {
      ArgumentUtility.CheckNotNull ("resultOperator", resultOperator);
      ArgumentUtility.CheckNotNull ("sqlStatementBuilder", sqlStatementBuilder);
      ArgumentUtility.CheckNotNull ("generator", generator);
      ArgumentUtility.CheckNotNull ("stage", stage);
      
      if (sqlStatementBuilder.TopExpression != null)
        sqlStatementBuilder = MoveStatementToSubQuery(sqlStatementBuilder, generator);
    }

    protected void UpdateDataInfo (ResultOperatorBase resultOperator, ref SqlStatementBuilder sqlStatementBuilder, IStreamedDataInfo dataInfo)
    {
      sqlStatementBuilder.DataInfo = resultOperator.GetOutputDataInfo (dataInfo);
    }

    void IResultOperatorHandler.HandleResultOperator (ResultOperatorBase resultOperator, QueryModel queryModel, ref SqlStatementBuilder sqlStatementBuilder, UniqueIdentifierGenerator generator, ISqlPreparationStage stage)
    {
      ArgumentUtility.CheckNotNull ("resultOperator", resultOperator);
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);
      ArgumentUtility.CheckNotNull ("sqlStatementBuilder", sqlStatementBuilder);
      ArgumentUtility.CheckNotNull ("generator", generator);
      ArgumentUtility.CheckNotNull ("stage", stage);

      var castOperator = ArgumentUtility.CheckNotNullAndType<T> ("resultOperator", resultOperator);
      HandleResultOperator (castOperator, queryModel, ref sqlStatementBuilder, generator, stage);
    }

    protected SqlStatement GetStatementAndResetBuilder (ref SqlStatementBuilder sqlStatementBuilder)
    {
      var sqlSubStatement = sqlStatementBuilder.GetSqlStatement ();
      sqlStatementBuilder = new SqlStatementBuilder ();
      return sqlSubStatement;
    }

    private SqlStatementBuilder MoveStatementToSubQuery (SqlStatementBuilder sqlStatementBuilder, UniqueIdentifierGenerator generator)
    {
      var sqlStatement = GetStatementAndResetBuilder (ref sqlStatementBuilder);
      var subStatementTableInfo = new ResolvedSubStatementTableInfo (
          generator.GetUniqueIdentifier ("q"),
          sqlStatement);
      var sqlTable = new SqlTable (subStatementTableInfo);

      sqlStatementBuilder.SqlTables.Add (sqlTable);
      sqlStatementBuilder.SelectProjection = new SqlTableReferenceExpression (sqlTable);
      // the new statement is an identity query that selects the result of its subquery, so it starts with the same data type
      sqlStatementBuilder.DataInfo = sqlStatement.DataInfo;
      return sqlStatementBuilder;
    }
  }
}