using SQLite;
using SqliteNetRowsChangedIssue;

string databaseFile = "test.db";
if (File.Exists(databaseFile))
{
    File.Delete(databaseFile);
}

var database = new SQLiteAsyncConnection("test.db");

await database.CreateTableAsync<Person>();

var insertTask = Insert();
var updateTask = Update();

await Task.WhenAny(insertTask, updateTask);


async Task Insert()
{
    for (int index = 0; index < 10000000; index++)
    {
        await database.RunInTransactionAsync(db =>
        {
            var person = new Person()
            {
                Name = $"Person{index}",
                IsFriend = false
            };
            int rowsChanged = db.Execute(
                @"INSERT INTO Person (Name, IsFriend) VALUES(?1, ?2)",
                person.Name,
                person.IsFriend);

            if (rowsChanged != 1)
            {
                Console.WriteLine($"Added 1 person but rows changed was {rowsChanged}");
                throw new Exception($"Added 1 person but rows changed was {rowsChanged}");
            }
        });
    }
}

async Task Update()
{
    while (true)
    {
        // Changes all rows in the table
        await database.ExecuteAsync("UPDATE Person SET IsFriend=true");
    }
}