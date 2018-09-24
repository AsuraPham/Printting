using System;
using System.Collections.Generic;
using System.Text;
using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces
{
   public interface IRepository<T> where T: BaseEntity
   {
       T GetById(int id);
       IEnumerable<T> ListAll();
       T Add(T entity);
       void Delete(T entity);
       void Update(T entity);
   }
}
