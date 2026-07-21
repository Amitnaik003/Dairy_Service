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

db.users.insertMany([
    {
        Username: "admin",
        PasswordHash: "$2a$11$yYy.F.U9nU45wQ6qG74/kueqG.jT6x1K6pXm.4s5X7gG.J3.hW7/q",
        Role: "Admin"
    },
    {
        Username: "user",
        PasswordHash: "$2a$11$61J2yN9.hG9/12.698gUHu.N8n8g.Y/U8a8163mB.n3.U.D7456d9",
        Role: "User"
    }
]);
