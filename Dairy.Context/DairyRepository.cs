using System.Collections.Generic;
using System.Threading.Tasks;
using Dairy.DMO;
using MongoDB.Driver;

namespace Dairy.Context
{
    public class DairyRepository
    {
        private readonly IMongoCollection<DairyProduct> _dairyProducts;

        public DairyRepository(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _dairyProducts = database.GetCollection<DairyProduct>("dairyProducts");
        }

        public async Task<List<DairyProduct>> GetAllProductsAsync()
        {
            return await _dairyProducts.Find(_ => true).ToListAsync();
        }

        public async Task<DairyProduct> GetProductAsync(string id)
        {
            return await _dairyProducts.Find(p => p.Id == MongoDB.Bson.ObjectId.Parse(id)).FirstOrDefaultAsync();
        }

        public async Task AddProductAsync(DairyProduct product)
        {
            await _dairyProducts.InsertOneAsync(product);
        }

        public async Task UpdateProductAsync(string id, DairyProduct product)
        {
            product.Id = MongoDB.Bson.ObjectId.Parse(id);
            await _dairyProducts.ReplaceOneAsync(p => p.Id == product.Id, product);
        }

        public async Task DeleteProductAsync(string id)
        {
            await _dairyProducts.DeleteOneAsync(p => p.Id == MongoDB.Bson.ObjectId.Parse(id));
        }
    }
}
