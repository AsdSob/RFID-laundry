using TravianCommanderAccount.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;

namespace TravianCommanderAccount.Data
{
    public interface IRepository : IDisposable
    {
        DbContext DbContext { get; }

        void Add<TModel>(TModel instance) where TModel : class, IEntity;
        void Add<TModel>(IEnumerable<TModel> instances) where TModel : class, IEntity;
        
        IQueryable<TModel> All<TModel>(params string[] includePaths) where TModel : class, IEntity;

        void Delete<TModel>(object key) where TModel : class, IEntity;
        void Delete<TModel>(TModel instance) where TModel : class, IEntity;
        void Delete<TModel>(Expression<Func<TModel, bool>> predicate) where TModel : class, IEntity;

        TModel Single<TModel>(object key) where TModel : class, IEntity;
        TModel Single<TModel>(Expression<Func<TModel, bool>> predicate, params string[] includePaths) where TModel : class, IEntity;

        IQueryable<TModel> Query<TModel>(Expression<Func<TModel, bool>> predicate, params string[] includePaths) where TModel : class, IEntity;
    }
}