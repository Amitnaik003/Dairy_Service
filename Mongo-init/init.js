db = db.getSiblingDB('DairyDB');

db.dairyProducts.insertMany([
    {
        FatContentPercentage: 3.2,
        PasteurizationDate: new Date(new Date().setDate(new Date().getDate() - 2)),
        StorageTemperatureRange: "2°C - 4°C"
    },
    {
        FatContentPercentage: 1.5,
        PasteurizationDate: new Date(new Date().setDate(new Date().getDate() - 5)),
        StorageTemperatureRange: "2°C - 4°C"
    },
    {
        FatContentPercentage: 0.0,
        PasteurizationDate: new Date(new Date().setDate(new Date().getDate() - 20)),
        StorageTemperatureRange: "2°C - 4°C"
    }
]);
