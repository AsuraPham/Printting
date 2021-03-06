﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces
{
    public interface ISpecification<T> where T : BaseEntity
    {
        Expression<Func<T, bool>> Criteria { get; }
        List<Expression<Func<T,object>>> Includes { get; }
        List<string> IncludeStrings { get; }
    }
}