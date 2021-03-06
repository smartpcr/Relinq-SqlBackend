// This file is part of the re-linq project (relinq.codeplex.com)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// re-linq is free software; you can redistribute it and/or modify it under 
// the terms of the GNU Lesser General Public License as published by the 
// Free Software Foundation; either version 2.1 of the License, 
// or (at your option) any later version.
// 
// re-linq is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-linq; if not, see http://www.gnu.org/licenses.
// 

using System;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.SqlBackend.SqlStatementModel;

namespace Remotion.Linq.SqlBackend.SqlPreparation.ResultOperatorHandlers
{
  /// <summary>
  /// Handles the <see cref="MinResultOperator"/>. When the <see cref="MinResultOperator"/> occurs after a 
  /// <see cref="SqlStatementBuilder.TopExpression"/> has been set, a sub-statement is created for 
  /// everything up to the <see cref="SqlStatementBuilder.TopExpression"/>.
  /// </summary>
  public class MinResultOperatorHandler : AggregationResultOperatorHandler<MinResultOperator>
  {
    public override AggregationModifier AggregationModifier
    {
      get { return AggregationModifier.Min; }
    }
  }
}