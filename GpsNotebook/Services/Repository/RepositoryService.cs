using GpsNotebook.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GpsNotebook.Services.Repository
{
    public class RepositoryService : IRepositoryService
    {
        private readonly SQLiteAsyncConnection database;

        public RepositoryService()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            database = new SQLiteAsyncConnection(Path.Combine(path, "MyDataBase.db"));
        }

        #region -- IRepositoryService implementation --

        public async Task<int> DeleteAsync<T>(T entity) where T : IEntityBase, new()
        {
            return await database.DeleteAsync(entity);
        }

        public async Task<List<T>> GetAllAsync<T>(Expression<Func<T, bool>> predicate = null) where T : IEntityBase, new()
        {
            var table = database.Table<T>();
            List<T> result;
            if (predicate == null)
            {
                result = await table.ToListAsync();
            }
            else
            {
                result = await table.Where(predicate).ToListAsync();
            }
            return result;
        }

        public async Task<List<T>> GetAllAsync<T>() where T : IEntityBase, new()
        {
            var table = database.Table<T>();
            List<T> result = await table.ToListAsync();
            return result;
        }

        public async Task<T> GetAsync<T>(Expression<Func<T, bool>> predicate) where T : IEntityBase, new()
        {
            return await database.FindAsync(predicate);
        }

        public async Task<T> GetByIdAsync<T>(int id) where T : IEntityBase, new()
        {
            return await database.GetAsync<T>(id);
        }

        public void InitTable<T>() where T : IEntityBase, new()
        {
            database.CreateTableAsync<T>();
        }

        public async Task<int> InsertAsync<T>(T entity) where T : IEntityBase, new()
        {
            int result = -1;
            try
            {
                if (entity.Id != 0)
                {
                    result = await UpdateAsync(entity);
                }
                else
                {
                    result = await database.InsertAsync(entity);
                }
            }
            catch
            {
            }

            return result;
        }

        public async Task<int> UpdateAsync<T>(T entity) where T : IEntityBase, new()
        {
            return await database.UpdateAsync(entity);
        }

        #endregion
    }
}
