using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BlazorDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bunit;

public class BunitIndexedDbManager : IIndexedDbManager
{
    public event EventHandler<BlazorDbEvent> ActionCompleted;
    public List<StoreSchema> Stores => _store.StoreSchemas;
    public int CurrentVersion => _store.Version;
    public string DbName => _store.Name;

    private readonly DbStore _store;

    private readonly Dictionary<string, List<object>> _data = new();

    public BunitIndexedDbManager(DbStore store)
    {
        _store = store;
        foreach (var storeSchema in Stores)
        {
            _data.Add(storeSchema.Name, new List<object>());
        }
    }

    public Task<Guid> OpenDb(Action<BlazorDbEvent> action = null)
    {
        return Task.FromResult(Guid.NewGuid());
    }

    public Task<Guid> DeleteDb(string dbName, Action<BlazorDbEvent> action = null)
    {
        throw new NotImplementedException();
    }

    public Task<BlazorDbEvent> DeleteDbAsync(string dbName)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> AddRecord<T>(StoreRecord<T> recordToAdd, Action<BlazorDbEvent> action = null)
    {
        var guid = Guid.NewGuid();
        var table = _data[recordToAdd.StoreName];
        table.Add(recordToAdd.Record);
        action?.Invoke(new BlazorDbEvent() { Failed = false, Message = "", Transaction = guid });
        return Task.FromResult(guid);
    }

    public Task<BlazorDbEvent> AddRecordAsync<T>(StoreRecord<T> recordToAdd)
    {
        var table = _data[recordToAdd.StoreName];
        table.Add(recordToAdd.Record);
        return Task.FromResult(new BlazorDbEvent() { Failed = false, Message = "", Transaction = Guid.NewGuid() });
    }

    public Task<Guid> BulkAddRecord<T>(string storeName, IEnumerable<T> recordsToBulkAdd,
        Action<BlazorDbEvent> action = null)
    {
        var guid = Guid.NewGuid();
        var table = _data[storeName];
        table.AddRange(recordsToBulkAdd.Cast<object>());
        action?.Invoke(new BlazorDbEvent() { Failed = false, Message = "", Transaction = guid });
        return Task.FromResult(guid);
    }

    public Task<BlazorDbEvent> BulkAddRecordAsync<T>(string storeName, IEnumerable<T> recordsToBulkAdd)
    {
        var table = _data[storeName];
        table.AddRange(recordsToBulkAdd.Cast<object>());
        return Task.FromResult(new BlazorDbEvent() { Failed = false, Message = "", Transaction = Guid.NewGuid() });
    }

    public Task<Guid> PutRecord<T>(StoreRecord<T> recordToPut, Action<BlazorDbEvent> action = null)
    {
        throw new NotImplementedException();
    }

    public Task<BlazorDbEvent> PutRecordAsync<T>(StoreRecord<T> recordToPut)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> UpdateRecord<T>(UpdateRecord<T> recordToUpdate, Action<BlazorDbEvent> action = null)
    {
        throw new NotImplementedException();
    }

    public Task<BlazorDbEvent> UpdateRecordAsync<T>(UpdateRecord<T> recordToUpdate)
    {
        throw new NotImplementedException();
    }

    public Task<TResult> GetRecordByIdAsync<TInput, TResult>(string storeName, TInput key)
    {
        throw new NotImplementedException();
    }

    public Task<TResult> GetRecordByIndexAsync<TResult>(string storeName, string index, object filterValue)
    {
        var table = _data[storeName];
        dynamic filterVal = filterValue;

        return Task.FromResult((TResult)table.FirstOrDefault(obj =>
        {
            dynamic tableVal = obj.GetType()
                .GetProperty(index, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public)
                ?.GetValue(obj);
            return filterVal == tableVal;
        }));
    }

    public Task<TResult> GetRecordByIndexAsync<TResult>(string storeName, IndexFilterValue filter)
    {
        throw new NotImplementedException();
    }

    public Task<IList<TRecord>> Where<TRecord>(string storeName, string indexName, object filterValue)
    {
        var table = _data[storeName];
        dynamic filterVal = filterValue;
        IList<TRecord> res = table.Where(obj =>
        {
            dynamic tableVal = obj.GetType()
                .GetProperty(indexName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public)
                ?.GetValue(obj);
            return filterVal == tableVal;
        }).Cast<TRecord>().ToList();
        return Task.FromResult(res);
    }

    public Task<IList<TRecord>> Where<TRecord>(string storeName, IEnumerable<IndexFilterValue> filters)
    {
        throw new NotImplementedException();
    }

    public Task<IList<TRecord>> ToArray<TRecord>(string storeName)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> DeleteRecord<TInput>(string storeName, TInput key, Action<BlazorDbEvent> action = null)
    {
        throw new NotImplementedException();
    }

    public Task<BlazorDbEvent> DeleteRecordAsync<TInput>(string storeName, TInput key)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> ClearTable(string storeName, Action<BlazorDbEvent> action = null)
    {
        throw new NotImplementedException();
    }

    public Task<BlazorDbEvent> ClearTableAsync(string storeName)
    {
        throw new NotImplementedException();
    }

    public void CalledFromJS(Guid transaction, bool failed, string message)
    {
        throw new NotImplementedException();
    }
}

public class BunitBlazorDbFactory : IBlazorDbFactory
{
    private readonly Dictionary<string, IIndexedDbManager> _managers = new();

    private readonly IServiceProvider _provider;

    public BunitBlazorDbFactory(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task<IIndexedDbManager> GetDbManager(string dbName)
    {
        if (!_managers.Any())
        {
            foreach (var store in _provider.GetServices<DbStore>())
            {
                var newManager = new BunitIndexedDbManager(store);
                await newManager.OpenDb();
                _managers.Add(store.Name, newManager);
            }
        }

        return _managers.TryGetValue(dbName, out var manager) ? manager : null;
    }

    public async Task<IIndexedDbManager> GetDbManager(DbStore dbStore)
    {
        return await GetDbManager(dbStore.Name);
    }
}

public static class BUnitExtension
{
    public static void AddBlazorDb(this TestContext context, Action<DbStore> options)
    {
        var store = new DbStore();
        options(store);

        context.Services.AddTransient<DbStore>((_) => store);
        context.Services.TryAddSingleton<IBlazorDbFactory, BunitBlazorDbFactory>();
    }
}