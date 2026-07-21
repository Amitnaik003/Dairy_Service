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
            SeedProductsIfEmpty();
        }

        private void SeedProductsIfEmpty()
        {
            if (_dairyProducts.CountDocuments(_ => true) == 0)
            {
                _dairyProducts.InsertMany(new[]
                {
                    new DairyProduct { Name = "Whole Milk", FatContentPercentage = 3.2, StockQuantity = 100, PasteurizationDate = System.DateTime.UtcNow.AddDays(-2), StorageTemperatureRange = "2°C - 4°C" },
                    new DairyProduct { Name = "Low Fat Milk", FatContentPercentage = 1.5, StockQuantity = 50, PasteurizationDate = System.DateTime.UtcNow.AddDays(-5), StorageTemperatureRange = "2°C - 4°C" },
                    new DairyProduct { Name = "Skim Milk", FatContentPercentage = 0.0, StockQuantity = 30, PasteurizationDate = System.DateTime.UtcNow.AddDays(-20), StorageTemperatureRange = "2°C - 4°C" }
                });
            }
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
