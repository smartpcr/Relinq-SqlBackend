// This file is part of the re-linq project (relinq.codeplex.com)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq.Utilities;

namespace Remotion.Linq.SqlBackend.SqlPreparation
{
  /// <summary>
  /// Aggregates a number of <see cref="IMethodCallTransformerProvider"/> instances, checking each of them when the <see cref="GetTransformer"/>
  /// is called, until one of them returns a <see cref="IMethodCallTransformer"/>.
  /// </summary>
  public class CompoundMethodCallTransformerProvider : IMethodCallTransformerProvider
  {
    private readonly IMethodCallTransformerProvider[] _providers;

    public static CompoundMethodCallTransformerProvider CreateDefault ()
    {
      var methodInfoBasedRegistry = MethodInfoBasedMethodCallTransformerRegistry.CreateDefault ();
      var attributeBasedProvider = new AttributeBasedMethodCallTransformerProvider();
      var nameBasedRegistry = NameBasedMethodCallTransformerRegistry.CreateDefault ();
      return new CompoundMethodCallTransformerProvider (methodInfoBasedRegistry, attributeBasedProvider, nameBasedRegistry);
    }

    public IMethodCallTransformerProvider[] Providers
    {
      get { return _providers; }
    }

    public CompoundMethodCallTransformerProvider (params IMethodCallTransformerProvider[] providers)
    {
      ArgumentUtility.CheckNotNull ("providers", providers);

      _providers = providers;
    }

    public IMethodCallTransformer GetTransformer (MethodCallExpression methodCallExpression)
    {
      ArgumentUtility.CheckNotNull ("methodCallExpression", methodCallExpression);

      return _providers
        .Select (methodCallTransformerRegistry => methodCallTransformerRegistry.GetTransformer (methodCallExpression))
        .FirstOrDefault (transformer => transformer != null);
    }
  }
}