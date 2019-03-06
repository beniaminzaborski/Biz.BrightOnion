using Biz.BrightOnion.Identity.API.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Biz.BrightOnion.Identity.API.Infrastructure.Specifications
{
  public abstract class BaseSpecification<TEntity> : ISpecification<TEntity>
    where TEntity: Entity
  {
    public BaseSpecification(Expression<Func<TEntity, bool>> criteria)
    {
      Criteria = criteria;
    }
    public Expression<Func<TEntity, bool>> Criteria { get; }
    public List<Expression<Func<TEntity, object>>> Includes { get; } =
    new List<Expression<Func<TEntity, object>>>();
    public List<string> IncludeStrings { get; } = new List<string>();
    protected virtual void AddInclude(Expression<Func<TEntity, object>> includeExpression)
    {
      Includes.Add(includeExpression);
    }

    protected virtual void AddInclude(string includeString)
    {
      IncludeStrings.Add(includeString);
    }
  }
}
