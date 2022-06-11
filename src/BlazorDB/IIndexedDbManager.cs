using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorDB;

public interface IIndexedDbManager
{
    /// <summary>
    /// A notification event that is raised when an action is completed
    /// </summary>
    event EventHandler<BlazorDbEvent> ActionCompleted;

    List<StoreSchema> Stores { get; }
    int CurrentVersion { get; }
    string DbName { get; }

    /// <summary>
    /// Opens the IndexedDB defined in the DbStore. Under the covers will create the database if it does not exist
    /// and create the stores defined in DbStore.
    /// </summary>
    /// <returns></returns>
    Task<Guid> OpenDb(Action<BlazorDbEvent> action = null);

    /// <summary>
    /// Deletes the database corresponding to the dbName passed in
    /// </summary>
    /// <param name="dbName">The name of database to delete</param>
    /// <returns></returns>
    Task<Guid> DeleteDb(string dbName, Action<BlazorDbEvent> action = null);

    /// <summary>
    /// Deletes the database corresponding to the dbName passed in
    /// Waits for response
    /// </summary>
    /// <param name="dbName">The name of database to delete</param>
    /// <returns></returns>
    Task<BlazorDbEvent> DeleteDbAsync(string dbName);

    /// <summary>
    /// Adds a new record/object to the specified store
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="recordToAdd">An instance of StoreRecord that provides the store name and the data to add</param>
    /// <returns></returns>
    Task<Guid> AddRecord<T>(StoreRecord<T> recordToAdd, Action<BlazorDbEvent> action = null);

    /// <summary>
    /// Adds a new record/object to the specified store
    /// Waits for response
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="recordToAdd">An instance of StoreRecord that provides the store name and the data to add</param>
    /// <returns></returns>
    Task<BlazorDbEvent> AddRecordAsync<T>(StoreRecord<T> recordToAdd);

    /// <summary>
    /// Adds records/objects to the specified store in bulk
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="recordsToBulkAdd">The data to add</param>
    /// <returns></returns>
    Task<Guid> BulkAddRecord<T>(string storeName, IEnumerable<T> recordsToBulkAdd, Action<BlazorDbEvent> action = null);

    /// <summary>
    /// Adds records/objects to the specified store in bulk
    /// Waits for response
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="recordsToBulkAdd">An instance of StoreRecord that provides the store name and the data to add</param>
    /// <returns></returns>
    Task<BlazorDbEvent> BulkAddRecordAsync<T>(string storeName, IEnumerable<T> recordsToBulkAdd);

    /// <summary>
    /// Puts a new record/object to the specified store
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="recordToPut">An instance of StoreRecord that provides the store name and the data to put</param>
    /// <returns></returns>
    Task<Guid> PutRecord<T>(StoreRecord<T> recordToPut, Action<BlazorDbEvent> action = null);

    /// <summary>
    /// Puts a new record/object to the specified store
    /// Waits for response
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="recordToPut">An instance of StoreRecord that provides the store name and the data to put</param>
    /// <returns></returns>
    Task<BlazorDbEvent> PutRecordAsync<T>(StoreRecord<T> recordToPut);

    /// <summary>
    /// Updates and existing record
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="recordToUpdate">An instance of UpdateRecord with the store name and the record to update</param>
    /// <returns></returns>
    Task<Guid> UpdateRecord<T>(UpdateRecord<T> recordToUpdate, Action<BlazorDbEvent> action = null);

    /// <summary>
    /// Updates and existing record
    /// Waits for response
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="recordToUpdate">An instance of UpdateRecord with the store name and the record to update</param>
    /// <returns></returns>
    Task<BlazorDbEvent> UpdateRecordAsync<T>(UpdateRecord<T> recordToUpdate);

    /// <summary>
    /// Retrieve a record by id
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="storeName">The name of the store to retrieve the record from</param>
    /// <param name="id">the id of the record</param>
    /// <returns></returns>
    Task<TResult> GetRecordByIdAsync<TInput, TResult>(string storeName, TInput key);

    /// <summary>
    /// Retrieve a record by index
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="storeName">The name of the store to retrieve the record from</param>
    /// <param name="index">the index field name</param>
    /// <param name="filterValue">the value to filter on</param>
    /// <returns></returns>
    Task<TResult> GetRecordByIndexAsync<TResult>(string storeName, string index, object filterValue);

    /// <summary>
    /// Retrieve a record by index
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="storeName">The name of the store to retrieve the record from</param>
    /// <param name="filter">the index name and value to filter on</param>
    /// <returns></returns>
    Task<TResult> GetRecordByIndexAsync<TResult>(string storeName, IndexFilterValue filter);

    /// <summary>
    /// Filter a store on an indexed value 
    /// </summary>
    /// <param name="storeName">The name of the store to retrieve the records from</param>
    /// <param name="indexName">index field name to filter on</param>
    /// <param name="filterValue">filter's value</param>
    /// <returns></returns>
    Task<IList<TRecord>> Where<TRecord>(string storeName, string indexName, object filterValue);

    /// <summary>
    /// Filter a store on indexed values 
    /// </summary>
    /// <param name="storeName">The name of the store to retrieve the records from</param>
    /// <param name="filters">A collection of index names and filters conditions</param>
    /// <returns></returns>
    Task<IList<TRecord>> Where<TRecord>(string storeName, IEnumerable<IndexFilterValue> filters);

    /// <summary>
    /// Retrieve all the records in a store
    /// </summary>
    /// <typeparam name="TRecord"></typeparam>
    /// <param name="storeName">The name of the store to retrieve the records from</param>
    /// <returns></returns>
    Task<IList<TRecord>> ToArray<TRecord>(string storeName);

    /// <summary>
    /// Deletes a record from the store based on the id
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <param name="storeName"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<Guid> DeleteRecord<TInput>(string storeName, TInput key, Action<BlazorDbEvent> action = null);

    /// <summary>
    /// Deletes a record from the store based on the id
    /// Wait for response
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <param name="storeName"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<BlazorDbEvent> DeleteRecordAsync<TInput>(string storeName, TInput key);

    /// <summary>
    /// Clears all data from a Table but keeps the table
    /// </summary>
    /// <param name="storeName"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    Task<Guid> ClearTable(string storeName, Action<BlazorDbEvent> action = null);

    /// <summary>
    /// Clears all data from a Table but keeps the table
    /// Wait for response
    /// </summary>
    /// <param name="storeName"></param>
    /// <returns></returns>
    Task<BlazorDbEvent> ClearTableAsync(string storeName);

    void CalledFromJS(Guid transaction, bool failed, string message);
}